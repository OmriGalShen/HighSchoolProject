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
    class Medium:PhysicalObject,IReflect
    {
        public Medium()
        {
            this.DefaultColor = Color.Transparent;
            this.FocusColor = Color.Transparent;
            this.Height = 150;
            this.Width = 50;
            this.Index = 1.5;
        }
        public override object GetCopy()
        {
            Medium obj = new Medium();
            obj.Angle = this.angle;
            obj.defaultColor = this.defaultColor;
            obj.focusColor = this.focusColor;
            obj.height = this.height;
            obj.width = this.width;
            obj.x = this.x;
            obj.y = this.y;
            return obj;
        }
        [Category("Medium"), Description("Index of the medium. Determine the curvature of light")]
        public double Index { set; get; }
        public override PointF[] GetPoints()
        {
            PointF[] points = new PointF[2];
            points[0] = new PointF(this.X, this.Y + this.Height / 2);
            points[1] = new PointF(this.X , this.Y - this.Height / 2);
            points = MathHelper.RotatePointFArray(this, points);
            return points;
        }
        public override Region GetRegion()
        {
            Light light = new Light(this.GetPoints()[0], this.GetPoints()[1]);
            return light.GetRegion();
        }
        public override GraphicsPath GetPath()
        {
            GraphicsPath path = new GraphicsPath();
            PointF[] p = this.GetPoints();
            //path.AddEllipse(this.X - this.Height / 2, this.Y - this.Height / 2, this.Height, this.Height);
            path.AddPie(this.X - this.Height / 2, this.Y - this.Height / 2, this.Height, this.Height, 360-(float)this.angle-90, 180); 
            //path.AddLine(this.GetPoints()[0],this.GetPoints()[1]);
            return path;
        }
        public bool IsIntersect(Light ray, Graphics g)
        {
            Region ObjReg = this.GetRegion();
            Region rayReg = ray.GetRegion();
            ObjReg.Intersect(rayReg);
            if (!ObjReg.IsEmpty(g)) return true;
            return false;
        }
        public void Interact(List<Light> lightList, Graphics g)
        {
            Light last = lightList[lightList.Count - 1];
            PointF pi = last.pi;
            PointF pf = last.pf;
            Color lightColor = last.LightColor;
            Region mediumReg = this.GetRegion();
            Region rayReg = last.GetRegion();
            double angle = last.GetAngle();


            mediumReg.Intersect(rayReg); //Intersect area of lens and the last light
            if (!mediumReg.IsEmpty(g))
            {

                double n1 = AdvanceInformation.screenIndex;
                double n2 = this.Index;

                if (n1 == n2) //act as nothing
                {
                    //
                }
                else if (n1 > n2) //act like a mirror
                {
                    AlgoritemHelper.MirrorAlgoritem(lightList, g, this);
                }
                else //act like medium
                {
                    AlgoritemHelper.MediumAlgoritem(lightList, g, this, n1, n2);
                }
            }
        }
        public override void Draw(Graphics g, bool highlighted)
        {
            Region reg = this.GetRegion();
            SolidBrush brushDefault = new SolidBrush(this.DefaultColor);
            SolidBrush brushFocus = new SolidBrush(this.FocusColor);
            
            
            if (highlighted) g.FillRegion(brushFocus, reg);
            else g.FillRegion(brushDefault, reg);

            g.DrawPath(Pens.Black,this.GetPath());
            //g.DrawArc(Pens.Black,new RectangleF(this.X,this.Y,this.Width,this.Height),90,180);
        }
    }
}
