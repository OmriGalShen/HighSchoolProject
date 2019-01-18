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
    //A class with static functions to help with Math and calculations
    class MathHelper
    {
        //Input: Physical Object ref and PointF to rotate around the object center point
        //Output: PointF with the new coordinates after the point has rotated around the object center point
        public static PointF RotatePointF(PhysicalObject ob, PointF pointToRotate)
        {
            double angleInRadians = -ob.Angle * (Math.PI / 180);
            PointF centerPoint = new PointF(ob.X, ob.Y);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new PointF
            {
                X =
                    (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
        //Input: Physical Object ref and array of PointF to rotate around the object center point
        //Output: PointF array each with the new coordinates after the point has rotated around the object center point
        public static PointF[] RotatePointFArray(PhysicalObject ob, PointF[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = RotatePointF(ob, p[i]);
            }
            return p;
        }
        //Input: PointF to rotate around center point, center point to rotate around and angle or rotation
        //Output: PointF with the new coordinates after the point has rotated around the object center point
        public static PointF RotatePointF(PointF pointToRotate, PointF centerPoint, double angle)
        {
            double angleInRadians = -angle * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new PointF
            {
                X =
                    (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
        //Input: Two PointF 
        //Output: The distance in double between the two points
        public static double DistanceBetweenPointF(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        //Input: Light and Graphics in which the light and the lens found at and index to ignore 
        //Output: return the closest object which is IReflect in the screen
        public static int ClosestReflectionObject(Light light, Graphics g, int ignore)
        {
            int closesObj = -1;
            double minDistance = Main_Form.displayWidth, temp;
            for (int i = 0; i < Main_Form.objectsList.Count; i++)
            {
                if (Main_Form.objectsList[i] is IInteract && i != ignore)
                {
                    PhysicalObject obj = Main_Form.objectsList[i];
                    if (((IInteract)obj).IsIntersect(light, g))
                    {
                        temp = MathHelper.DistanceBetweenPointF(light.pi, obj.CenterPoint);
                        if (temp < minDistance)
                        {
                            minDistance = temp;
                            closesObj = i;
                        }
                    }
                }
            }
            return closesObj;
        }
        //Input: Light and Graphics in which the light and the lens found at
        //Output: return the closest lens which the light hit or -1 if none were hit
        public static int ClosestLens(Light light, Graphics g)
        {
            return ClosestReflectionObject(light, g, -1);
        }
        //Input: PointF in which the light start from and the angle or rotation of the light
        //Output: PointF with the coordinate of the edge of the screen which this light hit
        public static PointF GetEndLight(PointF start, double angle)
        {
            PointF end = new PointF(start.X, start.Y);
            if (angle < 0) angle += 360;
            angle = angle % 360;
            if (angle == 0 || angle == 360) end.X = Main_Form.displayWidth;
            else if (angle == 90) end.Y = 0;
            else if (angle == 180) end.X = 0;
            else if (angle == 270) end.Y = Main_Form.displayHeight;
            else
            {
                double radius = Main_Form.displayWidth + 100;
                double y = radius * Math.Sin(-angle * Math.PI / 180) + start.Y;
                double x = radius * Math.Cos(-angle * Math.PI / 180) + start.X;

                end = new PointF((float)x, (float)y);
            }
            return end;
        }
        //Input: PointF to check if it's on an object in the objectList
        //Output: If the PointF is on an object in the objectList return it's index if isn't one return -1
        public static int DetectCollision(PointF point)
        {
            GraphicsPath path = new GraphicsPath();
            try
            {
                if (Main_Form.highlightedIndex != -1)
                {
                    path = Main_Form.objectsList[Main_Form.highlightedIndex].GetPath();
                    if (path.IsVisible(point)) return Main_Form.highlightedIndex;
                }

                for (int i = 0; i < Main_Form.objectsList.Count; i++)
                {
                    path = Main_Form.objectsList[i].GetPath();
                    if (path.IsVisible(point)) return i;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }
        //Input: Two lights
        //Output: Return the acute angle between the two lights
        public static double AngleBetweenLights(Light a, Light b)
        {
            PointF v = new PointF(a.pf.X - a.pi.X, a.pf.Y - a.pi.Y);
            PointF u = new PointF(b.pf.X - b.pi.X, b.pf.Y - b.pi.Y);
            double num = Math.Abs(v.X * u.X + v.Y * u.Y);
            num = num / ((Math.Sqrt(v.X * v.X + v.Y * v.Y) * (Math.Sqrt(u.X * u.X + u.Y * u.Y))));
            double angle = Math.Acos(num) * 180 / Math.PI;
            double correction = -0.3;
            return Math.Round(angle + correction);
        }
        //Input: Two points from two Parallel Lines and the lines angle
        //Output: Return distance between the lines
        public static double DistanceBetweenParallelLines(PointF a, PointF b, double angle)
        {
            try
            {
                double slope = Math.Atan(angle);
                double n1 = a.Y - slope * a.X;
                double n2 = b.Y - slope * b.X;
                double distance = Math.Abs(n1 - n2);
                distance = distance / Math.Sqrt(1 + slope * slope);

                return distance;
            }
            catch { return 0; }
        }
    }
}
