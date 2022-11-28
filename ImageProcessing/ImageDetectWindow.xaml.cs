using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using Microsoft.Win32;
using SpectrMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcessing {
    /// <summary>
    /// Логика взаимодействия для ImageDetectWindow.xaml
    /// </summary>
    public partial class ImageDetectWindow : Window {
        private ImageData imgOriginal;
        private string imgName;
        private GridDisplayer imgClassTable;

        public ImageDetectWindow() {
            InitializeComponent();

            imgClassTable = new GridDisplayer(classTable);
        }

        private void btnLoadImg_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                //load & remember bitmap
                LoadImage(openFileDialog.FileName);
                imgName = openFileDialog.SafeFileName;

                ImageDetectDataProvider.Instance.CopyImgToBin(openFileDialog.FileName, openFileDialog.SafeFileName);
            }
            else {
                return;
            }

            CalculateImageSpecialPoints();
            CalculateZondValues();
        }

        private void btnRefreshDB_Click(object sender, RoutedEventArgs e) {
            ImageDetectData[] images = ImageDetectDataProvider.Instance.ImageDetectDatas;
            foreach (ImageDetectData detectData in images) {
                LoadImage(ImageDetectDataProvider.ImgRepoFolder + detectData.imgName);
                imgName = detectData.imgName;
                txtClass.Text = detectData.className;
                CalculateImageSpecialPoints();
                CalculateZondValues();
                btnSaveToDB_Click(sender, e);
            }
        }

        int zondNothingColor = 0x00FFFFFF;
        private void LoadImage(string fileName) {
            imgOriginal = new ImageData(fileName);
            if (imgOriginal == null) {
                MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (imgOriginal.ColorDepth != 1) {
                imgOriginal = ImageUtility.ConvertToBinary(imgOriginal, true, 128); //new ConvertToBinaryThreshold().Process(imgOriginal);
                imgOriginal = new ZhangSuen().Process(new ImageData(imgOriginal));
            }

            if (ImageDetectDataProvider.Instance.zondBlue.IsImageLoaded) {
                ImageData zondImgOverlay = new ImageData(imgOriginal);
                zondImgOverlay = ImageUtility.Convert1BitToGray24BitManhattenDist(zondImgOverlay);
                int[] pixels = zondImgOverlay.GetPixels(),
                    zondBluePixels = ImageDetectDataProvider.Instance.zondBlue.GetPixels(),
                    zondRedPixels = ImageDetectDataProvider.Instance.zondRed.GetPixels();
                for (int i = 0; i < pixels.Length; i++) {

                    if (pixels[i] > 0) pixels[i] = 0xFFFFFF;
                    else pixels[i] = 0;

                    bool hasPixel = pixels[i] == 0;
                    if (zondBluePixels[i] != zondNothingColor) {
                        pixels[i] = hasPixel ? 0xAAAAAA : zondBluePixels[i];
                    }
                    if (zondRedPixels[i] != zondNothingColor) {
                        pixels[i] = hasPixel ? 0x999999 : zondRedPixels[i];
                    }
                }
                zondImgOverlay.SetPixels(pixels);
                zondImgOverlay.ApplyChanges();

                UpdateImageUI(zondImgOverlay);
            }
            else {
                UpdateImageUI(imgOriginal);
            }
        }

        private void CalculateImageSpecialPoints() {
            int[] matrixCn = ImageMatrixCalculator.GetCnMatrix(imgOriginal, true);
            int Ny = 0, Nk = 0;
            foreach (int Cn in matrixCn) {
                if (Cn == 1) {
                    Nk++;
                }
                else if (Cn > 2) {
                    Ny++;
                }
            }

            txtNk.Text = "Nk :\t" + Nk.ToString();
            txtNy.Text = "Ny :\t" + Ny.ToString();
        }

        private void CalculateZondValues() {
            var detectProvider = ImageDetectDataProvider.Instance;


            int[] pixels = imgOriginal.GetPixels();
            int[] redZondPixels = detectProvider.zondRed.GetPixels();
            int[] blueZondPixels = detectProvider.zondBlue.GetPixels();

            if (pixels.Length != redZondPixels.Length || blueZondPixels.Length != pixels.Length) {
                MessageBox.Show("Image size should be same as zond size", "error");
                return;
            }

            int blueZondCounter = 0, redZondCounter = 0;
            for (int i = 0; i < pixels.Length; i++) {
                if (pixels[i] != 0) {
                    continue;
                }

                if (blueZondPixels[i] != zondNothingColor) {
                    blueZondCounter++;
                }
                if (redZondPixels[i] != zondNothingColor) {
                    redZondCounter++;
                }
            }

            txtZondBlue.Text = "Blue z\t" + blueZondCounter.ToString();
            txtZondRed.Text = "Red z\t" + redZondCounter.ToString();
        }

        private void btnSaveToDB_Click(object sender, RoutedEventArgs e) {
            if (txtClass.Text == "Class" || txtClass.Text == string.Empty) {
                MessageBox.Show("Class name not set!", "error");
                return;
            }


            ImageDetectDataProvider.Instance.AddData(new ImageDetectData() {
                className = txtClass.Text,
                nodesBranchesCount = int.Parse(txtNy.Text.Split('\t')[1]),
                nodesEndCount = int.Parse(txtNk.Text.Split('\t')[1]),
                zondBlueIntersectCount = int.Parse(txtZondBlue.Text.Split('\t')[1]),
                zondRedIntersectCount = int.Parse(txtZondRed.Text.Split('\t')[1]),
                imgName = imgName,
            });

            MessageBox.Show("Save was successful!");
            ImageDetectDataProvider.Instance.SaveData();
        }

        private void btnTableDisplay_Click(object sender, RoutedEventArgs e) {
            var repoImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            string[] colomnNames = new string[] { "Ny", "Nk", "ZRed", "ZBlue", "D_NkNy", "D_Zonds" };
            int columnsCount = colomnNames.Length;
            string[] rowNames = new string[repoImgs.Length];
            float[][] dataArray = new float[repoImgs.Length][];

            for (int i = 0; i < dataArray.Length; i++) {
                dataArray[i] = new float[columnsCount];

                var img = repoImgs[i];
                dataArray[i][0] = img.nodesBranchesCount;
                dataArray[i][1] = img.nodesEndCount;
                dataArray[i][2] = img.zondRedIntersectCount;
                dataArray[i][3] = img.zondBlueIntersectCount;
                dataArray[i][4] = img.distance_NkNy;
                dataArray[i][5] = img.distance_Zonds;
                rowNames[i] = img.className;
            }

            float[] tableData = new float[columnsCount * dataArray.Length];
            int index = 0;
            foreach (float[] line in dataArray) {
                foreach (float value in line) {
                    tableData[index] = value;
                    index++;
                }
            }

            imgClassTable.DisplayMatrix<float>(tableData, columnsCount, rowNames.Length,
                                                    rowNames, colomnNames);
        }

        private void btnPointDistancesDisplay_Click(object sender, RoutedEventArgs e) {
            if (spectrResult == null) {
                MessageBox.Show("Use spectr method first then this.");
                return;
            }

            List<string> colomns = new List<string>();
            List<string> rows = new List<string>();

            var points = spectrResult.points;
            for (int i = 0; i < points.Count; i++) {
                colomns.Add((i + 1).ToString());
                rows.Add((i + 1).ToString());
            }

            int numbersCount = rows.Count;
            float[] tableData = new float[colomns.Count * rows.Count];
            for (int i = 0; i < numbersCount; i++) {
                for (int m = 0; m < numbersCount; m++) {
                    PointData pointA = points.Find(p => p.number == (i + 1));
                    PointData pointB = points.Find(p => p.number == (m + 1));
                    tableData[i * numbersCount + m] = PointData.Distance(pointA, pointB);
                }
            }

            imgClassTable.DisplayMatrix(tableData, colomns.Count, rows.Count,
                                                    rows.ToArray(), colomns.ToArray());
        }

        public void UpdateImageUI(ImageData image) {
            UpdateImageSize(image, this.workSpaceImage, this.imageCanvas);
        }

        private void UpdateImageSize(ImageData imgData, Image workSpaceImage, Canvas imgCanvas) {
            workSpaceImage.Source = imgData.GetBitmapImage();

            if (imgData.Width < 100) {
                workSpaceImage.Width = imgData.Width * 4;
                workSpaceImage.Height = imgData.Height * 4;
            }
            else if (imgData.Width < 250) {
                workSpaceImage.Width = imgData.Width * 2;
                workSpaceImage.Height = imgData.Height * 2;
            }
            else {
                workSpaceImage.Width = imgData.Width;
                workSpaceImage.Height = imgData.Height;
            }

            imgCanvas.Width = workSpaceImage.Width;
            imgCanvas.Height = workSpaceImage.Height;
        }

        private void btnDetectClassZonds_Click(object sender, RoutedEventArgs e) {
            if (imgOriginal == null) {
                btnLoadImg_Click(sender, e);
            }
            else {
                CalculateImageSpecialPoints();
            }

            int zondRedCount = int.Parse(txtZondRed.Text.Split('\t')[1]);
            int zondBlueCount = int.Parse(txtZondBlue.Text.Split('\t')[1]);

            int k = 3;

            var detectInfoImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            for (int i = 0; i < detectInfoImgs.Length; i++) {
                detectInfoImgs[i].distance_Zonds = (float)Math.Sqrt(
                    Math.Pow(zondRedCount - detectInfoImgs[i].zondRedIntersectCount, 2) +
                    Math.Pow(zondBlueCount - detectInfoImgs[i].zondBlueIntersectCount, 2));
            }

            List<ImageDetectData> sortedByDist = new List<ImageDetectData>(detectInfoImgs);
            sortedByDist = new List<ImageDetectData>(sortedByDist.OrderBy(img => img.distance_Zonds));
            Dictionary<string, int> detectDataCount = new Dictionary<string, int>();
            bool allDifferent = true;
            for (int i = 0; i < k; i++) {
                if (detectDataCount.ContainsKey(sortedByDist[i].className)) {
                    detectDataCount[sortedByDist[i].className]++;
                }
                else {
                    detectDataCount.Add(sortedByDist[i].className, 1);
                }
            }

            int maxValue = int.MinValue;
            string maxClass = string.Empty;
            foreach (var keypair in detectDataCount) {
                if (keypair.Value > maxValue) {
                    maxValue = keypair.Value;
                    maxClass = keypair.Key;
                }
            }

            int count = 0;
            foreach (var keypair in detectDataCount) {
                if (keypair.Value == maxValue) {
                    count++;
                    if (count > 1) {
                        allDifferent = false;
                        break;
                    }
                }
            }

            if (allDifferent) {
                txtClass.Text = maxClass;
                MessageBox.Show("Class was detected! It's " + maxClass);
            }
            else {
                MessageBox.Show("Can't detect class, multiple max distances!");
            }
        }

        private void btnDetectClass_NkNy_Click(object sender, RoutedEventArgs e) {
            if (imgOriginal == null) {
                btnLoadImg_Click(sender, e);
            }
            else {
                CalculateImageSpecialPoints();
            }

            int nodesBranchesCount = int.Parse(txtNy.Text.Split('\t')[1]);
            int nodesEndCount = int.Parse(txtNk.Text.Split('\t')[1]);

            int k = 3;

            var detectInfoImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            for (int i = 0; i < detectInfoImgs.Length; i++) {
                detectInfoImgs[i].distance_NkNy = (float)Math.Sqrt(
                    Math.Pow(nodesEndCount - detectInfoImgs[i].nodesEndCount, 2) +
                    Math.Pow(nodesBranchesCount - detectInfoImgs[i].nodesBranchesCount, 2));
            }

            List<ImageDetectData> sortedByDist = new List<ImageDetectData>(detectInfoImgs);
            sortedByDist = new List<ImageDetectData>(sortedByDist.OrderBy(img => img.distance_NkNy));
            Dictionary<string, int> detectDataCount = new Dictionary<string, int>();
            bool allDifferent = true;
            for (int i = 0; i < k; i++) {
                if (detectDataCount.ContainsKey(sortedByDist[i].className)) {
                    detectDataCount[sortedByDist[i].className]++;
                }
                else {
                    detectDataCount.Add(sortedByDist[i].className, 1);
                }
            }

            int maxValue = int.MinValue;
            string maxClass = string.Empty;
            foreach (var keypair in detectDataCount) {
                if (keypair.Value > maxValue) {
                    maxValue = keypair.Value;
                    maxClass = keypair.Key;
                }
            }

            int count = 0;
            foreach (var keypair in detectDataCount) {
                if (keypair.Value == maxValue) {
                    count++;
                    if (count > 1) {
                        allDifferent = false;
                        break;
                    }
                }
            }

            if (allDifferent) {
                txtClass.Text = maxClass;
                MessageBox.Show("Class was detected! It's " + maxClass);
            }
            else {
                MessageBox.Show("Can't detect class, multiple max distances!");
            }
        }

        float spectrT;
        int spectrStartPointNumb = 1;
        SpectrResult spectrResult;
        public SpectrResult SpectrResult => spectrResult;

        private void btnSpectrCount_NkNy_Click(object sender, RoutedEventArgs e) {
            spectrResult = null;
            bool inputCorrect = GetSpectrInputValues();
            if (!inputCorrect) return;

            spectrResult = new SpectrResult();
            spectrResult.selectionGroupName = "NkNy";
            List<PointData> processedPointsData = new List<PointData>();
            ImageDetectData[] dbImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            List<ImageDetectData> leftImgs = new List<ImageDetectData>(dbImgs);

            ImageDetectData tempImg = dbImgs[spectrStartPointNumb - 1];
            PointData pData = new PointData(tempImg.nodesBranchesCount, tempImg.nodesEndCount);
            pData.number = ImageDetectDataProvider.Instance.GetArrayIndexOf(tempImg) + 1;
            processedPointsData.Add(pData);
            leftImgs.Remove(tempImg);
            PointData lastCenter = new PointData(pData.x, pData.y);

            while (leftImgs.Count > 0) {
                foreach (var img in leftImgs) {
                    img.distance_NkNy = (float)Math.Sqrt(
                        Math.Pow(lastCenter.y - img.nodesEndCount, 2) +         // y = nodesEndCount
                        Math.Pow(lastCenter.x - img.nodesBranchesCount, 2));    // x = nodesBranchesCount
                }

                // find min dist NkNy point
                tempImg = leftImgs[0];
                for (int i = 0; i < leftImgs.Count; i++) {
                    if (leftImgs[i].distance_NkNy < tempImg.distance_NkNy) {
                        tempImg = leftImgs[i];
                    }
                }

                // calc new center point
                int processedPointsCount = processedPointsData.Count;
                lastCenter.x = (lastCenter.x * processedPointsCount + tempImg.nodesBranchesCount) / (processedPointsCount + 1);
                lastCenter.y = (lastCenter.y * processedPointsCount + tempImg.nodesEndCount) / (processedPointsCount + 1);

                // add new point to processed list
                pData = new PointData(tempImg.nodesBranchesCount, tempImg.nodesEndCount);
                pData.distanceFromPrevCenter = tempImg.distance_NkNy;
                pData.number = ImageDetectDataProvider.Instance.GetArrayIndexOf(tempImg) + 1;
                processedPointsData.Add(pData);

                // remove img from left images
                leftImgs.Remove(tempImg);
            }

            int classesCount = 1;
            // start from 2 to ignore first point (N point) and first found point by dist
            for (int i = 2; i < processedPointsData.Count; i++) {
                processedPointsData[i].scachokDelta =
                    processedPointsData[i].distanceFromPrevCenter - processedPointsData[i - 1].distanceFromPrevCenter;
            }

            List<List<int>> classes = new List<List<int>>();
            classes.Add(new List<int>());

            // add classes from scachoks
            for (int i = 0; i < processedPointsData.Count; i++) {
                if (processedPointsData[i].scachokDelta > spectrT) {
                    classesCount++;
                    classes.Add(new List<int>());
                }
                classes[classesCount - 1].Add(processedPointsData[i].number);
            }

            spectrResult.points = processedPointsData;
            spectrResult.classesCountResult = classesCount;
            spectrResult.threshold = spectrT;

            string pointsByClassesString = "";
            for (int i = 0; i < classes.Count; i++) {
                pointsByClassesString += $"\nclass {i + 1}:\t ";
                foreach (var pointNum in classes[i]) {
                    pointsByClassesString += pointNum + " ";
                }
            }

            MessageBox.Show("Classes count: " + classesCount + "\n" + pointsByClassesString);
        }

        private void btnSpectrCount_Zond_Click(object sender, RoutedEventArgs e) {
            spectrResult = null;
            bool inputCorrect = GetSpectrInputValues();
            if (!inputCorrect) return;

            spectrResult = new SpectrResult();
            spectrResult.selectionGroupName = "Zond";
            List<PointData> processedPointsData = new List<PointData>();
            ImageDetectData[] dbImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            List<ImageDetectData> leftImgs = new List<ImageDetectData>(dbImgs);

            ImageDetectData tempImg = dbImgs[spectrStartPointNumb - 1];
            PointData pData = new PointData(tempImg.zondBlueIntersectCount, tempImg.zondRedIntersectCount);
            pData.number = ImageDetectDataProvider.Instance.GetArrayIndexOf(tempImg) + 1;
            processedPointsData.Add(pData);
            leftImgs.Remove(tempImg);
            PointData lastCenter = new PointData(pData.x, pData.y);

            while (leftImgs.Count > 0) {
                foreach (var img in leftImgs) {
                    img.distance_NkNy = (float)Math.Sqrt(
                        Math.Pow(lastCenter.y - img.zondRedIntersectCount, 2) +   // y = zondRedIntersectCount
                        Math.Pow(lastCenter.x - img.zondBlueIntersectCount, 2));  // x = zondBlueIntersectCount
                }

                // find min dist NkNy point
                tempImg = leftImgs[0];
                for (int i = 0; i < leftImgs.Count; i++) {
                    if (leftImgs[i].distance_NkNy < tempImg.distance_NkNy) {
                        tempImg = leftImgs[i];
                    }
                }

                // calc new center point
                int processedPointsCount = processedPointsData.Count;
                lastCenter.x = (lastCenter.x * processedPointsCount + tempImg.zondBlueIntersectCount) / (processedPointsCount + 1);
                lastCenter.y = (lastCenter.y * processedPointsCount + tempImg.zondRedIntersectCount) / (processedPointsCount + 1);

                // add new point to processed list
                pData = new PointData(tempImg.zondBlueIntersectCount, tempImg.zondRedIntersectCount);
                pData.distanceFromPrevCenter = tempImg.distance_NkNy;
                pData.number = ImageDetectDataProvider.Instance.GetArrayIndexOf(tempImg) + 1;
                processedPointsData.Add(pData);

                // remove img from left images
                leftImgs.Remove(tempImg);
            }

            int classesCount = 1;
            // start from 2 to ignore first point (N point) and first found point by dist
            for (int i = 2; i < processedPointsData.Count; i++) {
                processedPointsData[i].scachokDelta =
                    processedPointsData[i].distanceFromPrevCenter - processedPointsData[i - 1].distanceFromPrevCenter;
            }

            List<List<int>> classes = new List<List<int>>();
            classes.Add(new List<int>());

            // add classes from scachoks
            for (int i = 0; i < processedPointsData.Count; i++) {
                if (processedPointsData[i].scachokDelta > spectrT) {
                    classesCount++;
                    classes.Add(new List<int>());
                }
                classes[classesCount - 1].Add(processedPointsData[i].number);
            }

            spectrResult.points = processedPointsData;
            spectrResult.classesCountResult = classesCount;
            spectrResult.threshold = spectrT;

            string pointsByClassesString = "";
            for (int i = 0; i < classes.Count; i++) {
                pointsByClassesString += $"\nclass {i + 1}:\t ";
                foreach (var pointNum in classes[i]) {
                    pointsByClassesString += pointNum + " ";
                }
            }

            MessageBox.Show("Classes count: " + classesCount + "\n" + pointsByClassesString);
        }

        public List<OxyPlot.DataPoint> GetSpectrGistogram() {
            if (spectrResult == null) {
                return null;
            }

            List<OxyPlot.DataPoint> points = new List<OxyPlot.DataPoint>();

            var pointsData = spectrResult.points;
            foreach (var point in pointsData) {
                points.Add(new OxyPlot.DataPoint(point.number, Math.Round(point.distanceFromPrevCenter, 2, MidpointRounding.ToEven)));
            }

            return points;
        }

        private void btnSpectrExport_Click(object sender, RoutedEventArgs e) {
            if (spectrResult == null) {
                MessageBox.Show("Save failure - there is no spectr data!");
            }

            string path = "C:\\Users\\lakey\\Desktop\\spectrResult.xml";
            XML_FileManager.SerializeToXML(spectrResult, path);
            MessageBox.Show("Save success! File name: " + path);
        }

        private bool GetSpectrInputValues() {
            try {
                spectrT = float.Parse(txtSpectrT.Text);
                if (spectrT < 0) throw new Exception();

                spectrStartPointNumb = int.Parse(txtSpectrN.Text);
                if (spectrStartPointNumb < 0) throw new Exception();
            }
            catch {
                MessageBox.Show("Incorrect input values,\nTry again.");
                return false;
            }
            return true;
        }
    }
}

namespace SpectrMethod {
    [Serializable]
    public class SpectrResult {
        public SpectrResult() { }
        public List<PointData> points = new List<PointData>();
        public string selectionGroupName = "NkNy";
        public float threshold = 0;
        public int classesCountResult = -1;
    }

    [Serializable]
    public class PointData {
        public PointData() { }
        public PointData(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public int number = 0;

        public static float Distance(PointData pointA, PointData pointB) {
            return (float)Math.Sqrt(
                        Math.Pow(pointA.x - pointB.x, 2) +
                        Math.Pow(pointA.y - pointB.y, 2));
        }

        public float x;
        public float y;

        // if -1 then it's first point (N start)
        public float distanceFromPrevCenter = 0f;
        public float scachokDelta = -1f;
    }
}
