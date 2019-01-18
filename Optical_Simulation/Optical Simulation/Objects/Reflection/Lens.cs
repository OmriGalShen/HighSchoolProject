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
    public class Lens : PhysicalObject,IReflect
    {
        protected double radius1, radius2;//radii of the lens
        //Constructor
        public Lens(int type)
        {
            this.Type = type; //The type of lens is accourding to the pictures seleced in the toolbox
            SetDefaults(); //defualt values for all lenses
        }
        public override object GetCopy()
        {
            Lens obj = new Lens(this.Type);
            obj.Angle = this.angle;
            obj.defaultColor = this.defaultColor;
            obj.focusColor = this.focusColor;
            obj.height = this.height;
            obj.width = this.width;
            obj.x = this.x;
            obj.y = this.y;
            obj.radius1 = this.radius1;
            obj.radius2 = this.radius2;
            return obj;
        }
        public void SetDefaults()
        {           
            this.Index = 1.5;
            this.Height = 150;
            this.Width = 20;
            this.Thickness = 20;
            this.DefaultColor = Color.LightBlue;
            this.FocusColor = Color.CornflowerBlue;
            this.ShowImage = true;
            this.ShowOpticalAxis = false;
            this.ShowFocalPoints = true;
            this.RealImageColor = Color.Blue;
            this.ImaginaryImageColor = Color.Green;
            SetDefultsForEachType();
        }
        //defualt values for each type of lens
        public void SetDefultsForEachType()
        {           
            if(this.Type== 1)
            {
                this.FocalPoint = 150;
                this.Rad1 = 200;
                this.Rad2 = 200;
                this.Thickness = 10;
            }
            if (this.Type == 2)
            {
                this.FocalPoint = 150;
                this.Rad1 = 1000;
                this.Rad2 = 150;
                this.Thickness = 10;
            }
            if (this.Type == 3)
            {
                this.FocalPoint = 150;
                this.Rad1 = -250;
                this.Rad2 = 250;
                this.Thickness = 10;
            }
            if (this.Type == 4)
            {
                this.FocalPoint = -150;
                this.Rad1 = -250;
                this.Rad2 = 1000;
                this.Thickness = 20;
            }
            if (this.Type == 5)
            {
                this.FocalPoint = -150;
                this.Rad1 = -250;
                this.Rad2 = -250;
                this.Thickness = 20;
            }
        }
        //return array of points describing the lens
        public override PointF[] GetPoints()
        {
            float halfHeight =this.Height/2,halfWidth=this.Width/2;
            double r1 = this.Rad1, r2 = this.Rad2;

            PointF[] pointsArr = new PointF[6];
            PointF[] radius1Points = PointsByRadius(r1, this.height / 2,true);
            PointF[] radius2Points = PointsByRadius(r2, this.height / 2,false);
            pointsArr[0] = radius1Points[0];
            pointsArr[1] = radius1Points[1];
            pointsArr[2] = radius1Points[2];
            pointsArr[3] = radius2Points[0];
            pointsArr[4] = radius2Points[1];
            pointsArr[5] = radius2Points[2];


            float width = (float)MathHelper.DistanceBetweenPointF(pointsArr[1], pointsArr[4]);
            float lowWidth =(float)MathHelper.DistanceBetweenPointF(pointsArr[0], pointsArr[5]);
            if (width > lowWidth)
                this.FocalPoint = Math.Abs(this.FocalPoint);
            else this.FocalPoint = -1*Math.Abs(this.FocalPoint);
            
            this.Width = width;
            pointsArr = MathHelper.RotatePointFArray(this,pointsArr);
            return pointsArr;
        }
        //using math the function returnes array of 3 points representing half ellipse which is half the lens
        private PointF[] PointsByRadius(double radius,float height,bool isR1)
        {
            PointF[] arr = new PointF[3]; //array of 3 points which describe half ellipse according to the radius

            float distance = (float)(Math.Abs(radius) - Math.Sqrt(radius * radius - height * height)); //width of the half ellipse using math
            float thick = (float)(this.Thickness / 2); //thickness added to each side of the half ellipse

            if (isR1) //radius 1
            {
                if (radius < 0)
                {
                    arr[0] = new PointF(this.X + distance + thick, this.Y - height);
                    arr[1] = new PointF(this.X + thick, this.Y);
                    arr[2] = new PointF(this.X + distance + thick, this.Y + height);
                }
                else
                {
                    arr[0] = new PointF(this.X + thick, this.Y - height);
                    arr[1] = new PointF(this.X + thick + distance, this.Y);
                    arr[2] = new PointF(this.X + thick, this.Y + height);
                }
            }
            else //radius 2
            {
                if (radius < 0)
                {
                    arr[2] = new PointF(this.X - distance - thick, this.Y - height);
                    arr[1] = new PointF(this.X - thick, this.Y);
                    arr[0] = new PointF(this.X - distance - thick, this.Y + height);
                }
                else
                {
                    arr[2] = new PointF(this.X - thick, this.Y - height);
                    arr[1] = new PointF(this.X - distance - thick, this.Y);
                    arr[0] = new PointF(this.X - thick, this.Y + height);
                }
            }
            return arr;
        }
        //returnes the lens Region
        public override Region GetRegion()
        {
            return new Region(this.GetPath());
        }
        //return the lens GraphicsPath
        public override GraphicsPath GetPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddClosedCurve(this.GetPoints());            
            return path;
        }
        //Draw the lens using the colors in the variables and also a black line around the object
        public override void Draw(Graphics g, bool highlighted)
        {
            Region reg = this.GetRegion();
            SolidBrush brushDefault = new SolidBrush(this.DefaultColor);
            SolidBrush brushFocus = new SolidBrush(this.FocusColor);
            if (highlighted) g.FillRegion(brushFocus, reg);
            else g.FillRegion(brushDefault, reg);
            g.DrawPath(Pens.Black, this.GetPath());
        }
        //override the Height property in order to change the radii together with the height
        [Category("General"), Description("Specify the Height of object")]
        public override float Height
        {
            set
            {
                radius1 += radius1 / Math.Abs(radius1)*(value - height) / 2;
                radius2 += radius2 / Math.Abs(radius2) * (value - height) / 2;
                height = value;
            }
            get
            {
                return height;
            }
        }
        //override the Width property in order to keep the user from changing it
        [Category("General"), Description("Specify the Width of object"),ReadOnly(true)]
        public override float Width { set { this.width = value; } get { return this.width; } }
        [Browsable(false)]
        //the type of lens
        public int Type { set; get; }

        [Category("Lens"), Description("Determine whether or not to show the image created by the lens or not")]
        public bool ShowImage { set; get; }

        [Category("Lens"), Description("Determine whether or not to show the focal points of the lens when it is hitted")]
        public bool ShowFocalPoints { set; get; }

        [Category("Lens"), Description("Determine whether or not to show the optical axis of the lens")]
        public bool ShowOpticalAxis { set; get; }

        [Category("Lens"), Description("Determine the color of the real image created by the lens")]
        public Color RealImageColor { set; get; }

        [Category("Lens"), Description("Determine the color of the imaginary image created by the lens")]
        public Color ImaginaryImageColor { set; get; }

        [Category("Lens"), Description("Focal point of the lens")]
        public double FocalPoint { set; get; }
        [Category("Lens"), Description("Radius number 1 of the lens")]
        public double Rad1
        {
            get
            {
                return radius1;
            }
            set
            {
                if (Math.Abs(value) > this.Height / 2) //keep the user from giving impossible values
                radius1 = value;
            }
        }
        [Category("Lens"), Description("Radius number 2 of the lens")]
        public double Rad2
        {
            get
            {
                return radius2;
            }
            set
            {
                if (Math.Abs(value) > this.Height / 2)//keep the user from giving impossible values
                    radius2 = value;
            }
        }
        [Category("Lens"), Description("Index of the lens")]
        public double Index { set; get; }
        [Category("Lens"), Description("Thickness added to the lens")]
        public double Thickness { set; get; }
        [Browsable(false)]

        public override String ToString()
        {
            return "";
        }
        //The function check if a light ray hit the lens using the region of both
        public bool IsIntersect(Light ray, Graphics g)
        {
            Region lensReg = this.GetRegion();
            Region rayReg = ray.GetRegion();
            lensReg.Intersect(rayReg);
            if (!lensReg.IsEmpty(g)) return true;
            return false;
        }
        //The function will make the light bend accordingly if the light hit it
        public void Interact(List<Light> lightList, Graphics g)
        {
            AlgoritemHelper.LensAlgoritem(lightList, g, this);
        }
    }
}
