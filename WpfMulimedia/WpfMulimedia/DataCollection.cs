using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfMulimedia
{
    public class DataCollection
    {
        private List<DataSeries> dataList;
        public DataCollection()
        {
            dataList = new List<DataSeries>();
        }
        public List<DataSeries> DataList
        {
            get { return dataList; }
            set { dataList = value; }
        }
        public void AddLines(ChartStyle cs)
        {
            int j = 0;
            foreach (DataSeries ds in DataList)
            {
                if (ds.SeriesName == "Default Name")
                {
                    ds.SeriesName = "DataSeries" + j.ToString();
                }
                ds.AddLinePattern();
                for (int i = 0; i < ds.LineSeries.Points.Count; i++)
                {
                    ds.LineSeries.Points[i] =
                        cs.NormalizePoint(ds.LineSeries.Points[i]);
                }
                cs.ChartCanvas.Children.Add(ds.LineSeries);
                j++;
            }
        }
    }

    public class ChartStyle
    {
        private Canvas chartCanvas;
        private double xmin = 0;
        private double xmax = 10;
        private double ymin = 0;
        private double ymax = 10;
        public Canvas ChartCanvas
        {
            get { return chartCanvas; }
            set { chartCanvas = value; }
        }
        public double Xmin
        {
            get { return xmin; }
            set { xmin = value; }
        }
        public double Xmax
        {
            get { return xmax; }
            set { xmax = value; }
        }
        public double Ymin
        {
            get { return ymin; }
            set { ymin = value; }
        }
        public double Ymax
        {
            get { return ymax; }
            set { ymax = value; }
        }
        public Point NormalizePoint(Point pt)
        {
            if (ChartCanvas.Width.ToString("en-US") == "не число")
                ChartCanvas.Width = 270;
            if (ChartCanvas.Height.ToString("en_US") == "не число")
                ChartCanvas.Height = 250;
            Point result = new Point();
            result.X = (pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin);
            result.Y = ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin);
            return result;
        }
    }
}
