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
    class GeneralProp
    {
        private string greetingText = "Welcome to the Optical Simulation!";
        
        [CategoryAttribute("Global Settings"),
        ReadOnlyAttribute(true),
        DefaultValueAttribute("Welcome to your application!")]
        public string GreetingText
        {
            get { return greetingText; }
            set { greetingText = value; }
        }

    }
}
