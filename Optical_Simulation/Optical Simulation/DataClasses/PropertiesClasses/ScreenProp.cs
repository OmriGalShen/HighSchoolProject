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
    class ScreenProp
    {
        public static double screenIndex=1;
        public static int screenSpeed = 20;
        public static Color screenColor = Color.White;

        [CategoryAttribute("Screen Settings"),
        DescriptionAttribute("The index of the screen"),
        DefaultValueAttribute(1)]
        public double ScreenIndex
        {
            get { return screenIndex; }
            set { screenIndex = value; }
        }
        [DescriptionAttribute("The rate in milliseconds that the graphics will refresh."),
        CategoryAttribute("Screen Settings")]
        public int RenderRate
        {
            get { return screenSpeed; }
            set { screenSpeed = value; }
        }
        [CategoryAttribute("Screen Settings"),
        DescriptionAttribute("Set the color of the screen")]
        public Color ScreenColor
        {
            get { return screenColor; }
            set { screenColor = value; }
        }
    }
}
