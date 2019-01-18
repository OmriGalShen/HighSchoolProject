using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Optical_Simulation
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            // Define the border style of the form to a dialog box.
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Set the MaximizeBox to false to remove the maximize box.
            this.MaximizeBox = false;

            // Set the MinimizeBox to false to remove the minimize box.
            this.MinimizeBox = false;

            aboutLabel.Text = "\nHello user,\n\n";
            aboutLabel.Text += "My name is Omri Elhi Shenhav, I live in Israel and in high school at the moment.\n";
            aboutLabel.Text += "This is my computer science project about simulating a very basic Geometrical optics system. I had enjoy making this project and determinted to improve it.";
            aboutLabel.Text+="\n\nFor any kind of questions, suggestions and requests please contact me through e-mail:";
            InfoLabel.Text = "Appliction Version " + GeneralInfo.appVersion +", 2016";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:shenhav.omri@gmail.com?subject=Optical Simulation Contact");
        }

        private void aboutLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
