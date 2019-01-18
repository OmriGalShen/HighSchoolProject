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
    //A class with static functions to help drawing 
    class DrawingHelper
    {
        //Input: Graphics g and list of Light
        //Output: Draw stright line which alter with impact with IReflect objects
        public static void DrawLightPath(Graphics g, List<Light> ray)
        {
            DrawLightPathHelper(g, ray, 0, -1);
        }
        //Input: Graphics g,list of light, number of times the function was repeated and index of object to ignore
        //Output: Draw stright line which alter with impact with IReflect objects
        public static void DrawLightPathHelper(Graphics g, List<Light> ray, int repeat, int ignore)
        {
            try
            {
                Light last = ray[ray.Count - 1];
                int objIndex = MathHelper.ClosestReflectionObject(last, g, ignore);
                if (objIndex != -1)
                {
                    if (Main_Form.objectsList[objIndex] is Lens)
                    {
                        if (((Lens)Main_Form.objectsList[objIndex]).ShowFocalPoints || ((Lens)Main_Form.objectsList[objIndex]).ShowOpticalAxis)
                        DrawFocalPointsAndOpticalAxis(g, (Lens)Main_Form.objectsList[objIndex]);
                    }

                    ((IInteract)Main_Form.objectsList[objIndex]).Interact(ray, g); //change the course of the light

                    //this part check if the light will hit another lens, if so it will recall this function (recursively)
                    bool check=true;
                    if (Main_Form.objectsList[objIndex] is Block) check=false; 
                    int temp = objIndex; //which object to ignore 
                    last = ray[ray.Count - 1];
                    objIndex = MathHelper.ClosestReflectionObject(last, g,temp);
                    if (objIndex != -1 && repeat<Main_Form.maxInteractions-1 &&check)
                    {
                        repeat++;
                        DrawLightPathHelper(g, ray, repeat, temp);
                    }
                    else
                    {
                        for (int h = 0; h < ray.Count; h++)
                        {
                            g.DrawLine(new Pen(ray[h].LightColor), ray[h].pi, ray[h].pf);
                        }
                    }

                }
                else
                {
                    for (int h = 0; h < ray.Count; h++)
                    {
                        g.DrawLine(new Pen(ray[h].LightColor), ray[h].pi, ray[h].pf);
                    }
                }
            }
            catch 
            {
                //MessageBox.Show(ex.Message);
            }
        }
        //Input: Graphics g and a lens
        //Output: Draw the either the focal points of the lens or/and the lens optical axis
        public static void DrawFocalPointsAndOpticalAxis(Graphics g, Lens lens)
        {
            //draw the focal points of the lens who got hit
            PointF focal1 = new PointF(lens.X + (float)lens.FocalPoint - 5, lens.Y - 5);
            PointF focal2 = new PointF(lens.X - (float)lens.FocalPoint - 5, lens.Y - 5);
            focal1 = MathHelper.RotatePointF(lens,focal1 );
            focal2 = MathHelper.RotatePointF(lens, focal2);
            if (lens.ShowFocalPoints)
            {
                g.FillEllipse(Brushes.BlueViolet, focal1.X, focal1.Y, 10, 10);
                g.FillEllipse(Brushes.BlueViolet, focal2.X, focal2.Y, 10, 10);
            }
            if(lens.ShowOpticalAxis)
            {
                Light l1 = new Light(lens.CenterPoint, MathHelper.GetEndLight(lens.CenterPoint, lens.Angle));
                Light l2 = new Light(lens.CenterPoint, MathHelper.GetEndLight(lens.CenterPoint, lens.Angle+180));
                g.DrawLine(Pens.DarkBlue, l1.pi, l1.pf);
                g.DrawLine(Pens.DarkBlue, l2.pi, l2.pf);
            }
        }
    }
}
