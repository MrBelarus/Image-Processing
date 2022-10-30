using System.Collections.Generic;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace ImageProcessing.Core {
    class GraphDisplayer {
        public Plot Plot { get; private set; }
        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }

        private System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);

        public GraphDisplayer(Plot plot) {
            this.Points = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };

            this.Plot = plot;
        }

        public void DisplayColumnGraph(IList<DataPoint> points) {
            List<ColumnItem> items = new List<ColumnItem>();
            foreach (DataPoint point in points) {
                items.Add(new ColumnItem(point.Y));
            }
            var barSeries = new OxyPlot.Wpf.ColumnSeries {
                ItemsSource = items,
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0}"
            };

            string[] axesItems = new string[points.Count];
            for (int i = 0; i < axesItems.Length; i++) {
                axesItems[i] = points[i].X.ToString();
            }

            Plot.Series.Clear();
            Plot.Series.Add(barSeries);

            Plot.Axes.Clear();
            Plot.Axes.Add(new OxyPlot.Wpf.CategoryAxis {
                Position = AxisPosition.Bottom,
                Key = "axesItems",
                ItemsSource = axesItems,
            });

            if (points.Count < 5) {
                Plot.Width = 150;
            }
            else {
                Plot.Width = 25 * points.Count;
            }
            Plot.InvalidatePlot(true);
        }

        public void DisplayDetectDataPoints(ImageDetectData[] imageDetectDatas) {
            Plot.Series.Clear();

            List<string> classes = new List<string>();
            foreach (var data in imageDetectDatas) {
                if (!classes.Contains(data.className)) {
                    classes.Add(data.className);
                }
            }

            for (int i = 0; i < classes.Count; i++) {
                List<ScatterPoint> items = new List<ScatterPoint>();
                string curClass = classes[i];
                foreach (var data in imageDetectDatas) {
                    if (data.className == curClass) {
                        items.Add(new ScatterPoint(data.nodesBranchesCount + GetRndValue(), data.nodesEndCount + GetRndValue(), 5));
                    }
                }

                var series = new OxyPlot.Wpf.ScatterSeries {
                    ItemsSource = items,
                    MarkerType = (MarkerType)(i + 1),
                    Color = Color.FromRgb(255, 0, 0),
                };
                Plot.Series.Add(series);
                Plot.LegendTitle += curClass + ((MarkerType)(i + 1)).ToString();
            }

            Plot.Axes.Clear();
            Plot.Axes.Add(new OxyPlot.Wpf.CategoryAxis {
                Position = AxisPosition.Bottom,
                Key = "axesItems",
                ItemsSource = new string[6] { "0", "1", "2", "3", "4", "5" },
            });


            Plot.InvalidatePlot(true);
        }

        private float GetRndValue() {
            return rnd.Next(-15, 15) / 100f;
        }
    }
}
