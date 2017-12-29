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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WpfMulimedia
{
    /// <summary>
    /// Логика взаимодействия для ParametricSurface.xaml
    /// </summary>
    public partial class ParametricSurface : Window
    {
        private ParSurface ps = new ParSurface();
        public ParametricSurface()
        {       
            InitializeComponent();
            ps.IsHiddenLine = false;
            ps.Viewport3d = viewport;
            AddHelicoid();
        }
        private void AddHelicoid()
        {
            ps.Umin = 0;
            ps.Umax = 1;
            ps.Vmin = -3 * Math.PI;
            ps.Vmax = 3 * Math.PI;
            ps.Nv = 100;
            ps.Nu = 10;
            ps.Ymin = ps.Vmin;
            ps.Ymax = ps.Vmax;
            ps.CreateSurface(Helicoid);
        }
        private Point3D Helicoid(double u, double v)
        {
            double x = u * Math.Cos(v);
            double z = u * Math.Sin(v);
            double y = v;
            return new Point3D(x, y, z);
        }
    }
}
