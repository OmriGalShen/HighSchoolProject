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
    class GeneralInfo
    {
        public static int objectCount = 0;
        public static string appVersion = "0.64";

        [Category("General Settings"), Description("Number of objects"), ReadOnlyAttribute(true)]
        public int ObjectCount
        {
            get { return Main_Form.objectsList.Count; }
        }


        [CategoryAttribute("General Settings"),
        DefaultValueAttribute("0.3"),
        DescriptionAttribute("Indicate the software version"),
        ReadOnlyAttribute(true)]
        public string AppVersion
        {
            get { return appVersion; }
            set { appVersion = value; }
        }
    }
}
