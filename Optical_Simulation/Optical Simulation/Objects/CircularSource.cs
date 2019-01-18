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
    class CircularSource:Source
    {
        protected float radius,maxRadius=500;//radius of the circle which is the base from which the light come from
        /// <summary>
        /// Empty constructor for CircularSource, gives defult values.
        /// </summary>
        public CircularSource()
        {
            this.radius = 60;
        }
        public override object GetCopy()
        {
            CircularSource obj = new CircularSource();
            obj.Angle = this.angle;
            obj.defaultColor = this.defaultColor;
            obj.focusColor = this.focusColor;
            obj.height = this.height;
            obj.width = this.width;
            obj.x = this.x;
            obj.y = this.y;
            obj.numberOfSources = this.numberOfSources;
            obj.IsOn = this.IsOn;
            obj.radius = this.radius;
            return obj;
        }

        [Category("Source"), Description("Radius of the source shape")]
        public float Radius 
        { 
            set { if (value >= 0 && value <= maxRadius)this.radius = value; else UserValuesWarning(0, maxRadius); }             
            get { return this.radius; } 
        }
        public override PointF[] GetPoints()
        {
            PointF[] points = new PointF[1];
            points[0] = new PointF(this.X , this.Y);//change to Height/4
            return points;
        }
        public override Region GetRegion()
        {
            return new Region(this.GetPath());
        }
        public override GraphicsPath GetPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(this.X - (int)Radius / 2, this.Y - (int)Radius / 2, (int)Radius, (int)Radius);
            return path;
        }
        public override void Draw(Graphics g,bool highlighted)
        {
            SolidBrush brushDefault = new SolidBrush(this.DefaultColor);
            SolidBrush brushFocus = new SolidBrush(this.FocusColor);
            if (highlighted) g.FillEllipse(brushFocus, this.X - Radius / 2, this.Y - Radius / 2, Radius, Radius);
            else g.FillEllipse(brushDefault, this.X - Radius / 2, this.Y - Radius / 2, Radius, Radius);
            g.DrawEllipse(Pens.Black, this.X - Radius / 2, this.Y - Radius / 2, Radius, Radius);

            if(this.IsOn)
            {
                if (this.NumberOfSources == 1)
                {
                    PointF start = this.CenterPoint;
                    List<Light> ray = this.EmitLight(start,this.Angle);
                    DrawingHelper.DrawLightPath(g, ray);
                }
                else
                {
                    double diff = (360.0 / this.NumberOfSources);
                    for (int j = 0; j < this.NumberOfSources; j++)
                    {
                        PointF start = this.CenterPoint;
                        List<Light> ray = this.EmitLight(start, (diff * j + this.Angle) % 360);
                        DrawingHelper.DrawLightPath(g, ray);
                    }
                }
            }
        }
    }
}
