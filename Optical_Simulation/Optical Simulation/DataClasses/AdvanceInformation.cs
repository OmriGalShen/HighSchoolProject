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
    public class AdvanceInformation
    {
        public static double screenIndex;
        public AdvanceInformation()
        {
            this.ObjectCount = 0;
            this.ScreenIndex = 1;
        }
        [Category("Info"), Description("Number of objects"), ReadOnlyAttribute(true)]
        public double ObjectCount
        {
            get;
            set;
        }
        [CategoryAttribute("Info"),
        DescriptionAttribute("The index of the screen"),
        DefaultValueAttribute(1)]
        public double ScreenIndex
        {
            get { return screenIndex; }
            set { screenIndex = value; }
        }
    }
}
