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
    public abstract class PhysicalObject:IDrawable
    {
        protected float width,height;//width and height of the object
        protected float x, y;//x and y coordinates of the object
        protected double angle;//angle of the object
        protected Color defaultColor, focusColor;//colors of the object

        /// <summary>
        /// Empty constructor for PhysicalObject, gives defult values.
        /// </summary>
        public PhysicalObject()
        {
        }
        public virtual Object GetCopy() //should be override in non abstract classes 
        {
            return null;
        }
        //abstract functions
        public abstract PointF[] GetPoints();
        public abstract Region GetRegion();
        public abstract GraphicsPath GetPath();
        public abstract void Draw(Graphics g, bool highlighted);
        //

        [Category("General"), Description("Default Color of the object")]
        public virtual Color DefaultColor 
        { 
            set { this.defaultColor = value; } 
            get { return this.defaultColor; } 
        }
        [Category("General"), Description("The color of the object when it's focused")]
        public virtual Color FocusColor 
        { 
            set { this.focusColor = value; } 
            get { return this.focusColor; } 
        }
        [Category("General"), Description("The X axis value")]
        public virtual float X
        {
            set { if (value >= 0 && value <= Main_Form.displayWidth)this.x = value; else UserValuesWarning(0, Main_Form.displayWidth); } 
            get { return this.x; } 
        }
        [Category("General"), Description("The Y axis value")]
        public virtual float Y 
        {
            set { if (value >= 0 && value <= Main_Form.displayHeight)this.y = value; else UserValuesWarning(0, Main_Form.displayHeight); } 
            get { return this.y; } 
        }
        [Category("General"), Description("Specify the Width of object")]
        public virtual float Width 
        {
            set { if (value >= 0 && value <= Main_Form.displayWidth)this.width = value; else UserValuesWarning(0, Main_Form.displayWidth); } 
            get { return this.width; } 
        }
        [Category("General"), Description("Specify the Height of object")]
        public virtual float Height 
        {
            set { if (value >= 0 && value <= Main_Form.displayHeight)this.height = value; else UserValuesWarning(0, Main_Form.displayHeight); } 
            get { return this.height; } 
        }
        [Category("General"), Description("Specify the angle rotations of object")]
        public virtual double Angle
        {
            get
            {
                return angle;
            }
            set
            {
                while (value < 0) value += 360;
                angle = value%360;
            }
        }
        [Browsable(false)]
        public virtual PointF CenterPoint
        {
            get
            {
                return new PointF(this.X,this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        public virtual void UserValuesWarning(double min,double max)
        {
            MessageBox.Show("Values should be between " + min + "-" + max);
        }
    }
}
