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
    //A class with static functions to help with the main optical algoritems
    class AlgoritemHelper
    {
        //Input: Light list, graphics g and a Lens
        //Output: Change the Light list to the path casued by the lens
        public static void LensAlgoritem(List<Light> lightList, Graphics g, Lens obj)
        {
            try
            {
                Light last = lightList[lightList.Count - 1];
                PointF pi = last.pi;
                PointF pf = last.pf;
                PointF firstPoint = new PointF(pi.X, pi.Y);
                Color lightColor = last.LightColor;
                double initialAngle = last.GetAngle();

                Region lensReg = obj.GetRegion();
                Region rayReg = last.GetRegion();


                lensReg.Intersect(rayReg); //Intersect area of lens and the last light
                if (!lensReg.IsEmpty(g))
                {
                    //getting the hit point
                    RectangleF boundsRect = lensReg.GetBounds(g);
                    PointF hit = new PointF((boundsRect.Right + boundsRect.Left) / 2, (boundsRect.Top + boundsRect.Bottom) / 2);
                    pf = hit;

                    //creating a light bewtween initial point and hit point
                    lightList[lightList.Count - 1] = new Light(pi, pf, lightColor);

                    pi = pf;

                    Light light = new Light(pi,pf);
                    double beta = 0;

                    beta = GetImagePointAngle(g, firstPoint, obj, initialAngle, hit);

                    //creating a light between the hit point and on
                    light = new Light(hit, MathHelper.GetEndLight(hit, beta),lightColor);
                    lightList.Add(light);
                }
            }
            catch
            {

            }
        }
        //Input: Graphics g, point which the last light start from, Lens object, angle of last light and the hit point
        //Output: calculate the angle in which the light comes out of the lens
        public static double GetImagePointAngle(Graphics g, PointF firstPoint, Lens obj, double angle, PointF hit)
        {
            bool imaginary = false;

            float f = (float)obj.FocalPoint;
            float u = (float)Math.Abs(obj.X - firstPoint.X);
            float ho = firstPoint.Y - obj.Y;
            float v = 1 / ((1 / f) - (1 / u));
            float hi = (v * ho / u);

            PointF imagePoint = new PointF();

            bool isVertical = obj.Angle > 225 && obj.Angle < 315 || obj.Angle > 45 && obj.Angle < 135;

            if (!isVertical)
            {
                Color imageColor = obj.RealImageColor;
                if (v * u < 0)
                {
                    imageColor = obj.ImaginaryImageColor;
                    imaginary = true;
                }
                if (firstPoint.X > obj.X) v *= -1;

                imagePoint = new PointF(obj.X + (float)(v), obj.Y - (float)hi);
                /*double angleToRotate = obj.Angle;
                if (angleToRotate >= 90 && angleToRotate <= 270) angleToRotate = angleToRotate + 180;
                imagePoint = MathHelper.RotatePointF(imagePoint, obj.CenterPoint, 0*angleToRotate);*/

                //drawing image
                if (obj.ShowImage) g.FillEllipse(new SolidBrush(imageColor), imagePoint.X - 5, imagePoint.Y - 5, 10, 10);
            }
            else
            {
                u = (float)Math.Abs(obj.Y - firstPoint.Y);
                ho = -firstPoint.X + obj.X;
                v = 1 / ((1 / f) - (1 / u));
                hi = (v * ho / u);

                Color imageColor = obj.RealImageColor;
                if (v * u < 0)
                {
                    imageColor = obj.ImaginaryImageColor;
                    imaginary = true;
                }
                if (firstPoint.Y > obj.Y) v *= -1;

                imagePoint = new PointF(obj.X + (float)(v), obj.Y - (float)hi);
                /*double angleToRotate = obj.Angle;
                if (angleToRotate >= 0 && angleToRotate <= 180) angleToRotate = angleToRotate + 180;
                imagePoint = MathHelper.RotatePointF(imagePoint, obj.CenterPoint, angleToRotate);*/

                //drawing image
                if (obj.ShowImage) g.FillEllipse(new SolidBrush(imageColor), imagePoint.X - 5, imagePoint.Y - 5, 10, 10);

            }

            //give information to data class
            if (Main_Form.highlightedIndex != -1)
            {
                PhysicalObject highlighted = Main_Form.objectsList[Main_Form.highlightedIndex];
                if (highlighted == obj)
                {
                    LensInfo.focal = Math.Round(f, 3);
                    LensInfo.u = Math.Round(u, 3);
                    LensInfo.v = Math.Round(-v, 3);
                    LensInfo.ho = Math.Round(-ho, 3);
                    LensInfo.hi = Math.Round(hi, 3);
                }
            }

            double angleToReturn = new Light(hit, imagePoint).GetAngle();
            if (imaginary) angleToReturn = new Light(hit, imagePoint).GetAngle() + 180;

            return angleToReturn;
        }
        //Input: Light list, graphics g and a Physical object
        //Output: Change the Light list to the path casued by the Mirror
        public static void MirrorAlgoritem(List<Light> lightList, Graphics g, PhysicalObject obj)
        {
            Light last = lightList[lightList.Count - 1];
            PointF pi = last.pi;
            PointF pf = last.pf;
            PointF firstPoint = new PointF(pi.X, pi.Y);
            Color lightColor = last.LightColor;
            Region mirrorReg = obj.GetRegion();
            Region rayReg = last.GetRegion();
            double angle = last.GetAngle();
            bool vertical = false;


            mirrorReg.Intersect(rayReg); //Intersect area of lens and the last light
            if (!mirrorReg.IsEmpty(g))
            {
                PointF hit = GetHitPoint(pf, mirrorReg, g);

                lightList[lightList.Count - 1] = new Light(pi, hit, lightColor); //light from the start to the hit point

                double alpha = 180 - (angle - 2 * obj.Angle);

                //check if the light hiting the vertical part of the mirror 
                PointF[] points = obj.GetPoints();
                if (points.Length == 4)
                {
                    PointF p1 = points[0];
                    PointF p2 = points[1];
                    PointF p3 = points[2];
                    PointF p4 = points[3];
                    float x = hit.X;
                    float y = hit.Y;
                    bool check1 = x >= Math.Min(p1.X, p4.X) && x <= Math.Max(p1.X, p4.X) && y >= Math.Min(p1.Y, p4.Y) && y <= Math.Max(p1.Y, p4.Y);
                    bool check2 = x >= Math.Min(p2.X, p3.X) && x <= Math.Max(p2.X, p3.X) && y >= Math.Min(p2.Y, p3.Y) && y <= Math.Max(p2.Y, p3.Y);
                    vertical = check1 || check2;
                    if (vertical)
                    {
                        alpha += 180;
                    }
                }
                lightList.Add(new Light(hit, MathHelper.GetEndLight(hit, alpha), lightColor)); //light from the hit point farther

                //draw image and give info
                Color imageColor = Color.Blue;
                Mirror mirror = obj as Mirror;
                if (mirror != null&&mirror.ShowImage)
                {

                    //calc image
                    imageColor = mirror.ImageColor;
                    double beta = alpha + 180;
                    double radius = MathHelper.DistanceBetweenPointF(hit, firstPoint);
                    float height = (float)(radius * Math.Sin(beta * Math.PI / 180));
                    float width = (float)(radius * Math.Cos(beta * Math.PI / 180));
                    float addWidth = obj.X - hit.X;
                    float addHeight = obj.Y - hit.Y;
                    //

                    //draw image
                    PointF imagePoint = new PointF(obj.X + width + addWidth, obj.Y - height + addHeight);
                    double angleToRotate = obj.Angle;
                    if (angleToRotate >= 180 && angleToRotate <= 360) angleToRotate = angleToRotate + 180;
                    imagePoint = MathHelper.RotatePointF(imagePoint, obj.CenterPoint, angleToRotate);
                    g.FillEllipse(new SolidBrush(imageColor), imagePoint.X - 5, imagePoint.Y - 5, 10, 10);
                    //

                    //give information to data class
                    if (Main_Form.highlightedIndex != -1)
                    {
                        PhysicalObject highlighted = Main_Form.objectsList[Main_Form.highlightedIndex];
                        if (highlighted == obj)
                        {
                            MirrorInfo.startPoint = firstPoint;
                            //MirrorInfo.imagePoint = imagePoint;
                        }
                    }
                }
                //
            }
        }
        //Input: Light list, graphics g, Physical object, screen index, medium index
        //Output: Change the Light list to the path casued by the Medium
        public static void MediumAlgoritem(List<Light> lightList, Graphics g, PhysicalObject obj,double n1,double n2)
        {
            Light last = lightList[lightList.Count - 1];
            PointF pi = last.pi;
            PointF pf = last.pf;
            PointF firstPoint = new PointF(pi.X, pi.Y);
            Color lightColor = last.LightColor;
            Region mediumReg = obj.GetRegion();
            Region rayReg = last.GetRegion();
            double angle = last.GetAngle();

            mediumReg.Intersect(rayReg); //Intersect area of lens and the last light
            if (!mediumReg.IsEmpty(g))
            {
                PointF hit = GetHitPoint(pf, mediumReg, g);
                bool oppSide = false, isCritical=false;

                double alpha = 0,beta=0,tempBeta=0,criticalAngle=0;//angles

                //getting hiting angle using math
                PointF[] points = obj.GetPoints();
                Light first = new Light(firstPoint, hit);
                Light second = new Light(firstPoint, hit);//need to modified
                if (points.Length == 2) second = new Light(points[0], points[1]);
                alpha = 90 - MathHelper.AngleBetweenLights(first, second);
                //

                if ((firstPoint.X - hit.X) * (obj.X - hit.X) > 0) oppSide = true;

                //calculate critical angle
                if (!oppSide)
                {
                    if (n1 > n2) criticalAngle = Math.Round(Math.Asin(n2 / n1) * 180 / Math.PI);
                    isCritical = criticalAngle != 0 && alpha >= criticalAngle;
                }
                else
                {
                    if (n2 > n1) criticalAngle = Math.Round(Math.Asin(n1 / n2) * 180 / Math.PI);
                    isCritical = criticalAngle != 0 && alpha >= criticalAngle;
                }
                //

                if (isCritical)//act like a mirror
                {

                    MirrorAlgoritem(lightList, g, obj);
                }
                else
                {                    

                    if (!oppSide)
                    {
                        beta = (Math.Asin(n1 / n2 * Math.Sin(alpha * Math.PI / 180)) * 180 / Math.PI);//snal law
                        beta = Math.Round(beta);
                        tempBeta = beta;//save beta value 
                    }
                    else //opposite side
                    {
                        beta = (Math.Asin(n2 / n1 * Math.Sin(alpha * Math.PI / 180)) * 180 / Math.PI);//snal law
                        tempBeta = Math.Round(beta);//save beta value 
                        beta = 180 - Math.Round(beta);
                    }
                    //fix light direction
                    float dirction = (firstPoint.Y - hit.Y) / (firstPoint.X - hit.X);
                    dirction = Math.Abs(dirction) / dirction;
                    if (oppSide) dirction *= -1;
                    if (dirction == 1) beta = 360 - beta;
                    //

                    if (alpha % 90 != 0)
                    {
                        lightList[lightList.Count - 1] = new Light(firstPoint, hit, lightColor);//light from the start to the hit point
                        lightList.Add(new Light(hit, MathHelper.GetEndLight(hit, beta + obj.Angle), lightColor));
                    }
                }
                //give information to data class
                if (Main_Form.highlightedIndex != -1)
                {
                    PhysicalObject highlighted = Main_Form.objectsList[Main_Form.highlightedIndex];
                    if (highlighted == obj)
                    {
                        MediumInfo.alphaAngle = alpha;
                        MediumInfo.betaAngle = tempBeta;
                        MediumInfo.mediumIndex = n2;
                        MediumInfo.criticalAngle = criticalAngle;
                    }
                }
            }

        }
        //Input: the end PointF of a light, reg of the Physical object, Graphics of screen 
        //Output: Get a hit point in the center of the object (if hit)
        public static PointF GetHitPoint(PointF pf, Region reg, Graphics g)
        {
            //getting the heat point
            RectangleF boundsRect = reg.GetBounds(g);
            PointF hit = new PointF();
            if (Math.Abs(boundsRect.Top - pf.Y) > Math.Abs(boundsRect.Bottom - pf.Y)) hit.Y = boundsRect.Top;
            else hit.Y = boundsRect.Bottom;
            if (Math.Abs(boundsRect.Right - pf.X) > Math.Abs(boundsRect.Left - pf.X)) hit.X = boundsRect.Right;
            else hit.X = boundsRect.Left;
            return hit;
        }
        //Input: Light list, graphics g, Physical object
        //Output: Change the Light list to the path casued by the Block
        public static void BlockAlgoritem(List<Light> lightList, Graphics g, PhysicalObject obj)
        {
            Light last = lightList[lightList.Count - 1];
            PointF pi = last.pi;
            PointF pf = last.pf;
            Color lightColor = last.LightColor;
            Region BlockReg = obj.GetRegion();
            Region rayReg = last.GetRegion();


            BlockReg.Intersect(rayReg); //Intersect area of lens and the last light
            if (!BlockReg.IsEmpty(g))
            {
                //getting the heat point
                RectangleF boundsRect = BlockReg.GetBounds(g);
                PointF hit = new PointF((boundsRect.Right + boundsRect.Left) / 2, (boundsRect.Top + boundsRect.Bottom) / 2);

                lightList[lightList.Count - 1] = new Light(pi, hit, lightColor); //light from start to hit point
            }
        }
    }
}
