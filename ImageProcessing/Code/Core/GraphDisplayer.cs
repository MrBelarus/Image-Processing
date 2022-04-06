using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace ImageProcessing.Core {
    class GraphDisplayer {
        public Plot Plot { get; private set; }
        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }

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
    }
}
