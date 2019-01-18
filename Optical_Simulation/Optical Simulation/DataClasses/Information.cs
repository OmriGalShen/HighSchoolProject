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
    public class Information
    {
        public Information()
        {
            this.Focal = 0;
            this.U = 0;
            this.V = 0;
            this.Ho = 0;
            this.Hi = 0;
        }
        [Category("Image"), Description("Focal distance"), ReadOnlyAttribute(true)]
        public double Focal
        {
            get;
            set;
        }
        [Category("Image"), Description("Distance from object"), ReadOnlyAttribute(true)]
        public double U
        {
            get;
            set;
        }
        [Category("Image"), Description("Distance from image"), ReadOnlyAttribute(true)]
        public double V
        {
            get;
            set;
        }
        [Category("Image"), Description("Height of object"), ReadOnlyAttribute(true)]
        public double Ho
        {
            get;
            set;
        }
        [Category("Image"), Description("Height of object"), ReadOnlyAttribute(true)]
        public double Hi
        {
            get;
            set;
        }
    }
}
