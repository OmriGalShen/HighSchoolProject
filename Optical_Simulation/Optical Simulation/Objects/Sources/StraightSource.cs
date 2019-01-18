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
    class StraightSource:Source
    {
        public StraightSource()
        {

        }
        public override object GetCopy()
        {
            StraightSource obj = new StraightSource();
            obj.Angle = this.angle;
            obj.defaultColor = this.defaultColor;
            obj.focusColor = this.focusColor;
            obj.height = this.height;
            obj.width = this.width;
            obj.x = this.x;
            obj.y = this.y;
            obj.numberOfSources = this.numberOfSources;
            obj.IsOn = this.IsOn;
            return obj;
        }
        public override PointF[] GetPoints()
        {
            PointF[] points = new PointF[4];
            points[0] = new PointF(this.X - Width / 2, this.Y - Height / 4);
            points[1] = new PointF(this.X - Width / 2, this.Y + Height / 4);
            points[2] = new PointF(this.X + Width / 2, this.Y + Height / 2);
            points[3] = new PointF(this.X + Width / 2, this.Y - Height / 2); 
            points = MathHelper.RotatePointFArray(this,points);
            return points;
        }
        public PointF GetHatchPoint()
        {
            PointF[] p = this.GetPoints();
            PointF np = new PointF();
            np.X = (p[2].X + p[3].X) / 2;
            np.Y = (p[2].Y + p[3].Y) / 2;
            return np;
        }
        public override List<Light> EmitLight(PointF start,double angle)
        {
            List<Light> ray = new List<Light>();
            Light light = new Light(start, MathHelper.GetEndLight(start, angle));
            light.LightColor = this.LightColor;
            ray.Add(light);
            return ray;
        }
        public override void Draw(Graphics g,bool highlighted)
        {
            Region reg = this.GetRegion();
            SolidBrush brushDefault = new SolidBrush(this.DefaultColor);
            SolidBrush brushFocus = new SolidBrush(this.FocusColor);
            if (highlighted) g.FillRegion(brushFocus, reg);
            else g.FillRegion(brushDefault, reg);
            g.DrawPolygon(Pens.Black, this.GetPoints());

            if (this.IsOn)
            {
                if (this.NumberOfSources == 1)
                {
                    PointF start = this.GetHatchPoint();
                    List<Light> ray = this.EmitLight(start,this.Angle);
                    DrawingHelper.DrawSrightLine(g, ray);
                }
                else
                {
                    float diffHeight = (float)MathHelper.DistanceBetweenPointF(this.GetPoints()[2], this.GetPoints()[3]) / (float)(this.NumberOfSources - 1);
                    for (int j = 0; j < this.NumberOfSources; j++)
                    {
                        PointF start = new PointF(this.GetPoints()[3].X + (float)(diffHeight * j * Math.Sin(this.Angle * Math.PI / 180)), this.GetPoints()[3].Y + (float)(diffHeight * j * Math.Cos(this.Angle * Math.PI / 180)));
                        List<Light> ray = this.EmitLight(start,this.Angle);
                        DrawingHelper.DrawSrightLine(g, ray);
                    }
                }
            }
        }
    }
}
