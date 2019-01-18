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
    interface IReflect
    {
        void Interact(List<Light> lightList, Graphics g);
        bool IsIntersect(Light ray, Graphics g);
    }
}
