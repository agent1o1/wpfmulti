using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfMulimedia
{
    /// <summary>
    /// Логика взаимодействия для LintCharts.xaml
    /// </summary>
    public partial class LintCharts : Window
    {
        private ChartStyleGridlines cs;
        private Legend lg;
        private DataCollection dc;
        private DataSeries ds;

        public LintCharts()
        {
            InitializeComponent();
            AddChart();
        }

        private void AddChart()
        {
            cs = new ChartStyleGridlines();
            lg = new Legend();
            dc = new DataCollection();
            ds = new DataSeries();
            cs.ChartCanvas = chartCanvas;
            cs.TextCanvas = textCanvas;
            cs.Title = "Sine and Cosine Chart";
            cs.Xmin = 0;
            cs.Xmax = 7;
            cs.Ymin = -1.5;
            cs.Ymax = 1.5;
            cs.YTick = 0.5;
            cs.GridlinePattern = ChartStyleGridlines.GridlinePatternEnum.Dot;
            cs.GridlineColor = Brushes.Black;
            cs.AddChartStyle(tbTitle, tbXLabel, tbYLabel);
            // Draw Sine curve:
            ds.LineColor = Brushes.Blue;
            ds.LineThickness = 1;
            ds.SeriesName = "Sine";
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            dc.DataList.Add(ds);
            // Draw cosine curve:
            ds = new DataSeries();
            ds.LineColor = Brushes.Red;
            ds.SeriesName = "Cosine";
            ds.LinePattern = DataSeries.LinePatternEnum.DashDot;
            ds.LineThickness = 2;
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Cos(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            dc.DataList.Add(ds);
            ds = new DataSeries();
            ds.LineColor = Brushes.DarkGreen;
            ds.SeriesName = "Sine^2";
            ds.LinePattern = DataSeries.LinePatternEnum.Dot;
            ds.LineThickness = 2;
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x) * Math.Sin(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            dc.DataList.Add(ds);
            dc.AddLines(cs);
            lg.LegendCanvas = legendCanvas;
            lg.IsLegend = true;
            lg.IsBorder = true;
            lg.LegendPosition = Legend.LegendPositionEnum.NorthWest;
            lg.AddLegend(cs.ChartCanvas, dc);
        }

        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            textCanvas.Width = chartGrid.ActualWidth;
            textCanvas.Height = chartGrid.ActualHeight;
            legendCanvas.Children.Clear();
            chartCanvas.Children.RemoveRange(1, chartCanvas.Children.Count - 1);
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();
        }
    }

    public class Legend
    {
        private bool isLegend;
        private bool isBorder;
        private Canvas legendCanvas;
        private LegendPositionEnum legendPosition;

        public Legend()
        {
            isLegend = false;
            isBorder = true;
            legendPosition = LegendPositionEnum.NorthEast;
        }

        public LegendPositionEnum LegendPosition
        {
            get { return legendPosition; }
            set { legendPosition = value; }
        }

        public Canvas LegendCanvas
        {
            get { return legendCanvas; }
            set { legendCanvas = value; }
        }

        public bool IsLegend
        {
            get { return isLegend; }
            set { isLegend = value; }
        }

        public bool IsBorder
        {
            get { return isBorder; }
            set { isBorder = value; }
        }

        public enum LegendPositionEnum
        {
            North,
            NorthWest,
            West,
            SouthWest,
            South,
            SouthEast,
            East,
            NorthEast
        }

        public void AddLegend(Canvas canvas, DataCollection dc)
        {
            TextBlock tb = new TextBlock();
            if (dc.DataList.Count < 1 || !IsLegend)
                return;
            int n = 0;
            string[] legendLabels = new string[dc.DataList.Count];
            foreach (DataSeries ds in dc.DataList)
            {
                legendLabels[n] = ds.SeriesName;
                n++;
            }
            double legendWidth = 0;
            Size size = new Size(0, 0);
            for (int i = 0; i < legendLabels.Length; i++)
            {
                tb = new TextBlock();
                tb.Text = legendLabels[i];
                tb.Measure(new Size(Double.PositiveInfinity,
                    Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (legendWidth < size.Width)
                    legendWidth = size.Width;
            }
            legendWidth += 50;
            legendCanvas.Width = legendWidth + 5;
            double legendHeight = 17 * dc.DataList.Count;
            double sx = 6;
            double sy = 0;
            double textHeight = size.Height;
            double lineLength = 34;
            Rectangle legendRect = new Rectangle();
            legendRect.Stroke = Brushes.Black;
            legendRect.Fill = Brushes.White;
            legendRect.Width = legendWidth;
            legendRect.Height = legendHeight;
            if (IsLegend && IsBorder)
                LegendCanvas.Children.Add(legendRect);
            Canvas.SetZIndex(LegendCanvas, 10);
            n = 1;
            foreach (DataSeries ds in dc.DataList)
            {
                double xSymbol = sx + lineLength / 2;
                double xText = 2 * sx + lineLength;
                double yText = n * sy + (2 * n - 1) * textHeight / 2;
                Line line = new Line();
                AddLinePattern(line, ds);
                line.X1 = sx;
                line.Y1 = yText;

                line.X2 = sx + lineLength;
                line.Y2 = yText;
                LegendCanvas.Children.Add(line);
                ds.Symbols.AddSymbol(legendCanvas,
                    new Point(0.5 * (line.X2 - line.X1 +
                                     ds.Symbols.SymbolSize) + 1, line.Y1));
                tb = new TextBlock();
                tb.Text = ds.SeriesName;
                LegendCanvas.Children.Add(tb);
                Canvas.SetTop(tb, yText - size.Height / 2);
                Canvas.SetLeft(tb, xText);
                n++;
            }
            legendCanvas.Width = legendRect.Width;
            legendCanvas.Height = legendRect.Height;
            double offSet = 7.0;
            switch (LegendPosition)
            {
                case LegendPositionEnum.East:
                    Canvas.SetRight(legendCanvas, offSet);
                    Canvas.SetTop(legendCanvas,
                        canvas.Height / 2 - legendRect.Height / 2);
                    break;
                case LegendPositionEnum.NorthEast:
                    Canvas.SetTop(legendCanvas, offSet);
                    Canvas.SetRight(legendCanvas, offSet);
                    break;
                case LegendPositionEnum.North:
                    Canvas.SetTop(legendCanvas, offSet);
                    Canvas.SetLeft(legendCanvas,
                        canvas.Width / 2 - legendRect.Width / 2);
                    break;
                case LegendPositionEnum.NorthWest:
                    Canvas.SetTop(legendCanvas, offSet);
                    Canvas.SetLeft(legendCanvas, offSet);
                    break;
                case LegendPositionEnum.West:
                    Canvas.SetTop(legendCanvas,
                        canvas.Height / 2 - legendRect.Height / 2);
                    Canvas.SetLeft(legendCanvas, offSet);
                    break;
                case LegendPositionEnum.SouthWest:
                    Canvas.SetBottom(legendCanvas, offSet);
                    Canvas.SetLeft(legendCanvas, offSet);
                    break;
                case LegendPositionEnum.South:
                    Canvas.SetBottom(legendCanvas, offSet);
                    Canvas.SetLeft(legendCanvas,
                        canvas.Width / 2 - legendRect.Width / 2);
                    break;
                case LegendPositionEnum.SouthEast:
                    Canvas.SetBottom(legendCanvas, offSet);
                    Canvas.SetRight(legendCanvas, offSet);
                    break;
            }
        }

        private void AddLinePattern(Line line, DataSeries ds)
        {
            line.Stroke = ds.LineColor;
            line.StrokeThickness = ds.LineThickness;
            switch (ds.LinePattern)
            {
                case DataSeries.LinePatternEnum.Dash:
                    line.StrokeDashArray =
                        new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case DataSeries.LinePatternEnum.Dot:
                    line.StrokeDashArray =
                        new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case DataSeries.LinePatternEnum.DashDot:
                    line.StrokeDashArray =
                        new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
                case DataSeries.LinePatternEnum.None:
                    line.Stroke = Brushes.Transparent;
                    break;
            }
        }
    }

    public class ChartStyleGridlines : ChartStyle
    {
        private string title;
        private string xLabel;
        private string yLabel;
        private Canvas textCanvas;
        private bool isXGrid = true;
        private bool isYGrid = true;
        private Brush gridlineColor = Brushes.LightGray;
        private double xTick = 1;
        private double yTick = 0.5;
        private GridlinePatternEnum gridlinePattern;
        private double leftOffset = 20;
        private double bottomOffset = 15;
        private double rightOffset = 10;
        private Line gridline = new Line();

        public ChartStyleGridlines()
        {
            title = "Title";
            xLabel = "X Axis";
            yLabel = "Y Axis";
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string XLabel
        {
            get { return xLabel; }

            set { xLabel = value; }
        }

        public string YLabel
        {
            get { return yLabel; }
            set { yLabel = value; }
        }

        public GridlinePatternEnum GridlinePattern
        {
            get { return gridlinePattern; }
            set { gridlinePattern = value; }
        }

        public double XTick
        {
            get { return xTick; }
            set { xTick = value; }
        }

        public double YTick
        {
            get { return yTick; }
            set { yTick = value; }
        }

        public Brush GridlineColor
        {
            get { return gridlineColor; }
            set { gridlineColor = value; }
        }

        public Canvas TextCanvas
        {
            get { return textCanvas; }
            set { textCanvas = value; }
        }

        public bool IsXGrid
        {
            get { return isXGrid; }
            set { isXGrid = value; }
        }

        public bool IsYGrid
        {
            get { return isYGrid; }
            set { isYGrid = value; }
        }

        public void AddChartStyle(TextBlock tbTitle, TextBlock tbXLabel,
            TextBlock tbYLabel)
        {
            Point pt = new Point();
            Line tick = new Line();
            double offset = 0;
            double dx, dy;
            TextBlock tb = new TextBlock();
            // determine right offset:
            tb.Text = Xmax.ToString();
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            rightOffset = size.Width / 2 + 2;
            // Determine left offset:
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity,
                    Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (offset < size.Width)
                    offset = size.Width;
            }
            leftOffset = offset + 5;
            Canvas.SetLeft(ChartCanvas, leftOffset);
            Canvas.SetBottom(ChartCanvas, bottomOffset);
            ChartCanvas.Width = Math.Abs(TextCanvas.Width -
                                         leftOffset - rightOffset);
            ChartCanvas.Height = Math.Abs(TextCanvas.Height -
                                          bottomOffset - size.Height / 2);
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);
            // Create vertical gridlines:
            if (IsYGrid == true)
            {
                for (dx = Xmin + XTick; dx < Xmax; dx += XTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }
            // Create horizontal gridlines:
            if (IsXGrid == true)
            {
                for (dy = Ymin + YTick; dy < Ymax; dy += YTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                    gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                    gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                    gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }
            // Create x-axis tick marks:
            for (dx = Xmin; dx <= Xmax; dx += xTick)
            {
                pt = NormalizePoint(new Point(dx, Ymin));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X;
                tick.Y2 = pt.Y - 5;
                ChartCanvas.Children.Add(tick);
                tb = new TextBlock();
                tb.Text = dx.ToString();
                tb.Measure(new Size(Double.PositiveInfinity,
                    Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                Canvas.SetTop(tb, pt.Y + 2 + size.Height / 2);
            }
            // Create y-axis tick marks:
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X + 5;
                tick.Y2 = pt.Y;
                ChartCanvas.Children.Add(tick);
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.Measure(new Size(Double.PositiveInfinity,
                    Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetRight(tb, ChartCanvas.Width + 10);
                Canvas.SetTop(tb, pt.Y);
            }
            // Add title and labels:
            tbTitle.Text = Title;
            tbXLabel.Text = XLabel;
            tbYLabel.Text = YLabel;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
        }

        public void AddLinePattern()
        {
            gridline.Stroke = GridlineColor;
            gridline.StrokeThickness = 1;
            switch (GridlinePattern)
            {
                case GridlinePatternEnum.Dash:
                    gridline.StrokeDashArray =
                        new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case GridlinePatternEnum.Dot:
                    gridline.StrokeDashArray =
                        new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case GridlinePatternEnum.DashDot:
                    gridline.StrokeDashArray =
                        new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        public enum GridlinePatternEnum
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4
        }
    }

    public class Symbols
    {
        private SymbolTypeEnum symbolType;
        private double symbolSize;
        private Brush borderColor;
        private Brush fillColor;
        private double borderThickness;

        public Symbols()
        {
            symbolType = SymbolTypeEnum.None;
            symbolSize = 8.0;
            borderColor = Brushes.Black;
            fillColor = Brushes.Black;
            borderThickness = 1.0;
        }

        public double BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; }
        }

        public Brush BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        public Brush FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public double SymbolSize
        {
            get { return symbolSize; }
            set { symbolSize = value; }
        }

        public SymbolTypeEnum SymbolType
        {
            get { return symbolType; }
            set { symbolType = value; }
        }

        public enum SymbolTypeEnum
        {
            Box = 0,
            Circle = 1,
            Cross = 2,
            Diamond = 3,
            Dot = 4,
            InvertedTriangle = 5,
            None = 6,
            OpenDiamond = 7,
            OpenInvertedTriangle = 8,
            OpenTriangle = 9,
            Square = 10,
            Star = 11,
            Triangle = 12,
            Plus = 13
        }

        public void AddSymbol(Canvas canvas, Point pt)
        {
            Polygon plg = new Polygon();
            plg.Stroke = BorderColor;
            plg.StrokeThickness = BorderThickness;
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = BorderColor;
            ellipse.StrokeThickness = BorderThickness;
            Line line = new Line();
            double halfSize = 0.5 * SymbolSize;
            Canvas.SetZIndex(plg, 5);
            Canvas.SetZIndex(ellipse, 5);
            switch (SymbolType)
            {
                case SymbolTypeEnum.Square:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.OpenDiamond:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Circle:
                    ellipse.Fill = Brushes.White;
                    ellipse.Width = SymbolSize;
                    ellipse.Height = SymbolSize;
                    Canvas.SetLeft(ellipse, pt.X - halfSize);
                    Canvas.SetTop(ellipse, pt.Y - halfSize);
                    canvas.Children.Add(ellipse);
                    break;
                case SymbolTypeEnum.OpenTriangle:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.None:
                    break;
                case SymbolTypeEnum.Cross:
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y + halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y - halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    Canvas.SetZIndex(line, 5);
                    break;
                case SymbolTypeEnum.Star:
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y + halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y - halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    break;
                case SymbolTypeEnum.OpenInvertedTriangle:
                    plg.Fill = Brushes.White;
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Plus:
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X - halfSize;
                    line.Y1 = pt.Y;
                    line.X2 = pt.X + halfSize;
                    line.Y2 = pt.Y;
                    canvas.Children.Add(line);
                    line = new Line();
                    Canvas.SetZIndex(line, 5);
                    line.Stroke = BorderColor;
                    line.StrokeThickness = BorderThickness;
                    line.X1 = pt.X;
                    line.Y1 = pt.Y - halfSize;
                    line.X2 = pt.X;
                    line.Y2 = pt.Y + halfSize;
                    canvas.Children.Add(line);
                    break;
                case SymbolTypeEnum.Dot:
                    ellipse.Fill = FillColor;
                    ellipse.Width = SymbolSize;
                    ellipse.Height = SymbolSize;
                    Canvas.SetLeft(ellipse, pt.X - halfSize);
                    Canvas.SetTop(ellipse, pt.Y - halfSize);
                    canvas.Children.Add(ellipse);
                    break;
                case SymbolTypeEnum.Box:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Diamond:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y));
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.InvertedTriangle:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y - halfSize));
                    canvas.Children.Add(plg);
                    break;
                case SymbolTypeEnum.Triangle:
                    plg.Fill = FillColor;
                    plg.Points.Add(new Point(pt.X - halfSize, pt.Y + halfSize));
                    plg.Points.Add(new Point(pt.X, pt.Y - halfSize));
                    plg.Points.Add(new Point(pt.X + halfSize, pt.Y + halfSize));
                    canvas.Children.Add(plg);
                    break;
            }
        }
    }
}