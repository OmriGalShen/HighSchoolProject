using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Optical_Simulation
{
    class MirrorInfo
    {
        public static PointF startPoint, imagePoint;
        public MirrorInfo()
        {
            startPoint = new PointF(0, 0);
            imagePoint = new PointF(0, 0);
        }
        [Category("Points"), Description("The starting point of light"), ReadOnlyAttribute(true)]
        public PointF StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }
        [Category("Points"), Description("The image of the starting point"), ReadOnlyAttribute(true)]
        public PointF ImagePoint
        {
            get { return imagePoint; }
            set { imagePoint = value; }
        }
    }
}
