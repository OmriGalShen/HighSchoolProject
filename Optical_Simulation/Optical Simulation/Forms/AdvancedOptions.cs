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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            countTextBox.Text = "" + Main_Form.maxObjects;
            InterTextBox.Text = "" + Main_Form.maxInteractions;
            fontTextBox.Text = "" + Properties.Settings.Default.FontSize;
        }
        //cancel button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //confirm button
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bool keepOpen = false;
                string warning = "Must enter only numeric values";
                float fontSize;
                int maxObjects, maxInteractions;

                //change font size
                string value = fontTextBox.Text;
                bool result = float.TryParse(value, out fontSize);
                if (result)
                {
                    Properties.Settings.Default.FontSize = float.Parse(value);
                }
                else
                {
                    MessageBox.Show(warning);
                }

                //change max objects size
                value = countTextBox.Text;
                result = int.TryParse(value,out maxObjects);
                if (result)
                {
                    if(int.Parse(value)>=Main_Form.objectsList.Count)
                    Main_Form.maxObjects = int.Parse(value);
                    else
                    {
                        MessageBox.Show("Already passed over the limit");
                        countTextBox.Text = ""+Main_Form.maxObjects;
                        keepOpen = true;
                    }
                }
                else
                {
                    MessageBox.Show(warning);
                }

                //change max interaction size
                value = InterTextBox.Text;
                result = int.TryParse(value, out maxInteractions);
                if (result)
                {
                    Main_Form.maxInteractions = int.Parse(value);
                }
                else
                {
                    MessageBox.Show(warning);
                }
                //

                Form mainForm = Application.OpenForms["Main_Form"];
                mainForm.Font = new Font(FontFamily.GenericSansSerif, Properties.Settings.Default.FontSize, FontStyle.Regular);
                if (!keepOpen) this.Close();
            }
            catch { }
        }
    }
}
