using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace Optical_Simulation
{
    public partial class Feedback : Form
    {
        public Feedback()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string moreInformation = moreInfo.Text;


                var checkedRadio = new[] { performanceGroup, algorithmsGroup,interfaceGroup }
                   .SelectMany(g => g.Controls.OfType<RadioButton>()
                                            .Where(r => r.Checked));
                int performanceRate = 0, algorithmsRate = 0, interfaceRate = 0;
                int i = 0;
                //print name
                foreach(var rate in checkedRadio)
                {
                    if(i==0)
                    {
                        performanceRate = int.Parse(""+rate.Text);
                        
                    }
                    if(i==1)
                    {
                        algorithmsRate = int.Parse("" + rate.Text);
                    }
                    if(i==2)
                    {
                        interfaceRate = int.Parse("" + rate.Text);
                    }
                    i++;
                }



                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("optical.simulation@outlook.com", "Pi31415926");//optical.simulation@outlook.com
                MailMessage msg = new MailMessage();
                msg.To.Add("shenhav.omri@gmail.com");
                msg.From = new MailAddress("optical.simulation@outlook.com");
                msg.Subject = "Optical Simulation Feedback";
                string body = "";
                body += "Date:" + DateTime.Now.ToString()+"\n";
                body += "Performance Rate:" + performanceRate + "\n";
                body += "Algorithms Rate:" + algorithmsRate + "\n";
                body += "Interface Rate:" + interfaceRate + "\n";
                body += "More Text:" + moreInformation + "\n";
                msg.Body = body;
                client.Send(msg);
                MessageBox.Show("Successfully Sent Message. \nThank you very Much!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
