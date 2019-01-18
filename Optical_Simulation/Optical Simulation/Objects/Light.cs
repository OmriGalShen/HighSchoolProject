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
using System.Numerics;
using System.Windows;

namespace Optical_Simulation
{
    [Serializable]
    public sealed class Light
    {
        public PointF pi, pf;//two points which define the line 
        private Color lightColor;//the light color

        /// <summary>
        /// Light constrctor with color
        /// Input: initial PointF of light, finle PoinF of light and Light Color
        /// </summary>
        public Light(PointF pi, PointF pf, Color lightColor)
        {
            this.pi = pi;
            this.pf = pf;
            this.LightColor = lightColor;
        }
        /// <summary>
        /// Light constrctor without color (gives defult color)
        /// Input: initial PointF of light, finle PoinF of light
        /// </summary>
        public Light(PointF pi, PointF pf) : this(pi, pf, Color.Black) { }
        public Color LightColor { set { this.lightColor = value; } get { return this.lightColor; } }
        /// <summary>
        /// return the area near the light for calculations
        /// </summary>
        public Region GetRegion()
        {
            GraphicsPath path = new GraphicsPath();
            PointF p1, p2, p3, p4;
            if (this.GetAngle() >= 315 && this.GetAngle() <= 0 || this.GetAngle() >= 0 && this.GetAngle() <= 45 || this.GetAngle() >= 135 && this.GetAngle() <= 225)
            {
                p1 = new PointF(pi.X, pi.Y + 1);
                p2 = new PointF(pf.X, pf.Y + 1);
                p3 = new PointF(pi.X, pi.Y - 1);
                p4 = new PointF(pf.X, pf.Y - 1);
            }
            else
            {
                p1 = new PointF(pi.X + 1, pi.Y);
                p2 = new PointF(pf.X + 1, pf.Y);
                p3 = new PointF(pi.X - 1, pi.Y);
                p4 = new PointF(pf.X - 1, pf.Y);
            }
            path.AddLine(p1, p2);
            path.AddLine(p2, p4);
            path.AddLine(p4, p3);
            path.AddLine(p3, p1);
            path.CloseAllFigures();
            return new Region(path);
        }
        /// <summary>
        /// return the angle of the light in double
        /// </summary>
        public double GetAngle()
        {
            double angle = 0;
            double xDiff = pf.X - pi.X;
            double yDiff = pi.Y - pf.Y;
            if (Math.Abs(xDiff) < 0.000001) xDiff = xDiff/Math.Abs(xDiff)*0.1;
            if (Math.Abs(yDiff) < 0.000001) yDiff = yDiff / Math.Abs(yDiff) * 0.1;
            double slope = yDiff / xDiff;
            if (pf.Y == pi.Y && pi.X < pf.X) angle = 0;
            else if (pf.X == pi.X && pf.Y < pi.Y) angle = 90;
            else if (pf.Y == pi.Y && pi.X > pf.X) angle = 180;
            else if (pf.X == pi.X && pf.Y > pi.Y) angle = 270;
            else angle = Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
            if (angle < 0) angle += 360;
            return angle % 360;
        }
    }
}
