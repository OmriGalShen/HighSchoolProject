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
    //interface of objects which can be used as a Light source
    interface ISource
    {
        List<Light> EmitLight(PointF start, double angle);
    }
}
