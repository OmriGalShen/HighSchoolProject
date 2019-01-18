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
    [Serializable]
    public abstract class Source:PhysicalObject,ISource
    {
        protected int numberOfSources = 1;
        public Source()
        {
            this.IsOn = true;
            this.NumberOfSources = 1;
            this.Height = 60;
            this.Width = 60;
            this.DefaultColor = Color.Yellow;
            this.FocusColor = Color.Gold;
            this.LightColor = Color.Black;
        }
        public override PointF[] GetPoints()
        {
            PointF[] points = new PointF[1];
            points[0] = new PointF(this.X, this.Y);
            return points;
        }
        public override Region GetRegion()
        {
            return new Region(this.GetPath());
        }
        public override GraphicsPath GetPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(this.GetPoints());
            return path;
        }
        [Category("Source"), Description("If the source is turened on or off")]
        public bool IsOn { set; get; }
        [Category("Source"), Description("Number of sources")]
        public int NumberOfSources
        {
            set { if(value<150)this.numberOfSources = value; }
            get { return this.numberOfSources; }
        }
        [Category("Source"), Description("Default Color of light coming from the source")]
        public Color LightColor { set; get; }
        public override void Draw(Graphics g,bool highlighted)
        {
            Region reg = this.GetRegion();
            SolidBrush brushDefault = new SolidBrush(this.DefaultColor);
            SolidBrush brushFocus = new SolidBrush(this.FocusColor);
            if (highlighted) g.FillRegion(brushFocus, reg);
            else g.FillRegion(brushDefault, reg);
            g.DrawPolygon(Pens.Black, this.GetPoints());
        }
        public virtual List<Light> EmitLight(PointF start, double angle)
        {
            List<Light> ray = new List<Light>();
            Light light  = new Light(start, MathHelper.GetEndLight(start, angle));
            light.LightColor = this.LightColor;
            ray.Add(light);
            return ray;
        }

        public override String ToString()
        {
            return "";
        }
    }
}
