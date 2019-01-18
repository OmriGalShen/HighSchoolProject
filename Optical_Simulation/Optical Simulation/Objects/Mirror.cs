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
    class Mirror : PhysicalObject, IInteract
    {
        //constructor
        public Mirror()
        {
            //defult values 
            this.DefaultColor = Color.Aquamarine;
            this.FocusColor = Color.Aqua;
            this.Height = 100;
            this.Width = 30;
            this.ShowImage = false;
            this.ImageColor = Color.Blue; 
        }
        public override object GetCopy()
        {
            Mirror obj = new Mirror();
            obj.Angle = this.angle;
            obj.defaultColor = this.defaultColor;
            obj.focusColor = this.focusColor;
            obj.height = this.height;
            obj.width = this.width;
            obj.x = this.x;
            obj.y = this.y;
            return obj;
        }
        public override PointF[] GetPoints()
        {
            PointF[] points = new PointF[4];
            points[0] = new PointF(this.X - Width / 2, this.Y - Height / 2);
            points[1] = new PointF(this.X - Width / 2, this.Y + Height / 2);
            points[2] = new PointF(this.X + Width / 2, this.Y + Height / 2);
            points[3] = new PointF(this.X + Width / 2, this.Y - Height / 2);
            points = MathHelper.RotatePointFArray(this, points);
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

        [Category("Mirror"), Description("Determine whether or not to show the image created by the mirror or not")]
        public bool ShowImage { set; get; }
        [Category("Mirror"), Description("Determine the color of the image created by the mirror")]
        public Color ImageColor { set; get; }
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
            AlgoritemHelper.MirrorAlgoritem(lightList, g, this);
        }
        public override void Draw(Graphics g, bool highlighted)
        {
            Region reg = this.GetRegion();
            SolidBrush brushDefault = new SolidBrush(this.DefaultColor);
            SolidBrush brushFocus = new SolidBrush(this.FocusColor);
            if (highlighted) g.FillRegion(brushFocus, reg);
            else g.FillRegion(brushDefault, reg);
            g.DrawPolygon(Pens.Black, this.GetPoints());
        }
    }
}
