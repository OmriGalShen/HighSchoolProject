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
    public class AppSettings
    {
        private string greetingText = "Welcome to the Optical Simulation!";

        private bool settingsChanged = false;
        public static string appVersion = "0.64";
        public static Color screenColor = Color.White;
        public static int speed = 20;


        [CategoryAttribute("Global Settings"),
        ReadOnlyAttribute(true),
        DefaultValueAttribute("Welcome to your application!")]
        public string GreetingText
        {
            get { return greetingText; }
            set { greetingText = value; }
        }

        [BrowsableAttribute(false),
        DefaultValueAttribute(false)]
        public bool SettingsChanged
        {
            get { return settingsChanged; }
            set { settingsChanged = value; }
        }
       [DescriptionAttribute("The rate in milliseconds that the graphics will refresh."),
        CategoryAttribute("Screen Settings")]
        public int RenderRate
        {
            get { return speed; }
            set { speed = value; }
        }

        [CategoryAttribute("Version"),
        DefaultValueAttribute("0.3"),
        DescriptionAttribute("Indicate the software version"),
        ReadOnlyAttribute(true)]
        public string AppVersion
        {
            get { return appVersion; }
            set { appVersion = value; }
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
