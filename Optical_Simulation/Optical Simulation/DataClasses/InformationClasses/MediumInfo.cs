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
    class MediumInfo
    {
        public static double alphaAngle, betaAngle, mediumIndex, criticalAngle;
        public MediumInfo()
        {
            alphaAngle = 0;
            betaAngle = 0;
            mediumIndex = 0;
            criticalAngle = 0;
        }
        [Category("Angles"), Description("Hit angle"), ReadOnlyAttribute(true)]
        public double AlphaAngle
        {
            get { return alphaAngle; }
            set { alphaAngle = value; }
        }
        [Category("Angles"), Description("Angle of the curved light"), ReadOnlyAttribute(true)]
        public double BetaAngle
        {
            get { return betaAngle; }
            set { betaAngle = value; }
        }
        [Category("Angles"), Description("Critical Angle"), ReadOnlyAttribute(true)]
        public double CriticalAngle
        {
            get { return criticalAngle; }
            set { criticalAngle = value; }
        }
        [Category("Indexes"), Description("Index of the screen"), ReadOnlyAttribute(true)]
        public double ScreenIndex
        {
            get { return ScreenProp.screenIndex; }
        }
        [Category("Indexes"), Description("Index of the medium"), ReadOnlyAttribute(true)]
        public double MediumIndex
        {
            get { return mediumIndex; }
            set { mediumIndex = value; }
        }

    }
}
