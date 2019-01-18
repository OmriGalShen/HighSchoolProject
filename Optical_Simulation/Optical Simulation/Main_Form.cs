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
using System.Drawing.Printing;
using System.Drawing.Imaging; 
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Optical_Simulation
{
    public partial class Main_Form : Form
    {
        //public static variables
        public static int highlightedIndex = -1; //index of highlighted object
        public static List<PhysicalObject> objectsList = new List<PhysicalObject>();//list of objects
        public static float displayWidth, displayHeight; //share values of screen sizes
        public static bool isFullScreen = false; //bool if the screen is full screen
        public static int maxObjects = 15;//max amount of objects in the screen
        public static int maxInteractions = 20;//max light interctions

        //private variables
        private PointF mouseDownLocation; //save the mouse down location
        private int selectedPic = -1; // selected pic from the tool box
        private bool isMouseDown; //bool if the mouse is down
        private static Stack<List<PhysicalObject>> undoStack = new Stack<List<PhysicalObject>>();//stack for undo
        private static Stack<List<PhysicalObject>> restoreStack = new Stack<List<PhysicalObject>>();//stack for redo
        private string fileName = "";//save file name
        private PhysicalObject saveObjectsList;//save of a specific Physical object
        //

        //instances of data classes
        GeneralInfo generalInfo = new GeneralInfo();
        LensInfo lensInfo = new LensInfo();
        MirrorInfo mirrorInfo = new MirrorInfo();
        MediumInfo mediumInfo = new MediumInfo();
        ScreenProp screenProp=new ScreenProp();
        GeneralProp generalProp = new GeneralProp();
        //
         

        public Main_Form()
        {
            InitializeComponent();

            SaveObjectsState();

            //Initialize display width and height
            displayWidth = displayScreen.Width;
            displayHeight = displayScreen.Height;

            //Initialize timerMove Interval
            ScreenProp.screenSpeed=timerMove.Interval;

            infoStrip.Text = "Please Notice this application is currently in a very early stage of development. Current Version : Alpha "+generalInfo.AppVersion;

            propTab.SelectedIndex = 2;

            this.Font = new Font(FontFamily.GenericSansSerif,Properties.Settings.Default.FontSize,FontStyle.Regular);

            //DoubleBuffered set to true: help rendering
            typeof(Panel).InvokeMember("DoubleBuffered",BindingFlags.SetProperty | BindingFlags.Instance 
                | BindingFlags.NonPublic,null, this.displayScreen, new object[] { true });
            //

            //set console for develpment helper
            
            //AllocConsole();
            //Console.WriteLine("InitializeComponent()");
            //Console.SetWindowSize(52, 20);
        }

        //allow console
        // give the use of cmd if needed
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        //
     
        // ----- Main Receiving Function -----

        // This is the main function, which handle the events after the user click on the Display
        // The function, depending on the selected picturebox, will create a new object (lens, source or other)
        // while creating the new object it will give it some initial data depending on which object is it
        // such as the curser loctation on the display and the object type

        private void Display_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {

                if (objectsList.Count < maxObjects && selectedPic != -1)
                {
                    // setting new point, which is where the cursor is on the display
                    Point currentLocation = this.displayScreen.PointToClient(Cursor.Position);

                    //creating objects corresponding to the dropped picturebox

                    if (selectedPic == 1 || selectedPic == 2 || selectedPic == 3 || selectedPic == 4 || selectedPic == 5)
                    {
                        objectsList.Add(new Lens(selectedPic));
                    }

                    else if (selectedPic == 6)
                    {
                        objectsList.Add(new StraightSource());
                    }
                    else if (selectedPic == 7)
                    {
                        objectsList.Add(new CircularSource());
                    }
                    else if (selectedPic == 8)
                    {
                        objectsList.Add(new Block());
                    }
                    else if (selectedPic == 9)
                    {
                        objectsList.Add(new Medium());
                    }
                    else if (selectedPic == 10)
                    {
                        objectsList.Add(new Mirror());
                    }
                    PhysicalObject ob = objectsList[objectsList.Count - 1];
                    ob.X = currentLocation.X;
                    ob.Y = currentLocation.Y;
                    highlightedIndex = objectsList.Count - 1;
                    SaveObjectsState();
                    propTab.SelectedIndex =0;
                    UpdateInfo();

                    selectedPic = -1;
                }
                else if (objectsList.Count >= maxObjects && selectedPic != -1)
                {
                    MessageBox.Show("Too many obejcts on screen");
                }
                selectedPic = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        // ----- Main Graphics Function -----
        private void Display_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            try
            {
                // Loop draw every object (polymorphism)
                for (int i = 0; i < objectsList.Count; i++)
                {
                    objectsList[i].Draw(g, i == highlightedIndex);
                }

                //
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        // ----- Main Timer Function -----
        // This is the timer which update the display, therefore is need to be used whenever the user making changes
        private void timerMove_Tick(object sender, EventArgs e)
        {
            try
            {
                //update values
                displayWidth = displayScreen.Width;
                displayHeight = displayScreen.Height;
                timerMove.Interval = ScreenProp.screenSpeed;

                //If the mouse is down on the display, the display will be updated
                if (isMouseDown)
                {
                    int index = MathHelper.DetectCollision(mouseDownLocation);//object index or -1 if none
                    if (index != -1)
                    {
                        mouseDownLocation = this.displayScreen.PointToClient(Cursor.Position);
                        if (mouseDownLocation.X < 0 || mouseDownLocation.X > displayScreen.Width || mouseDownLocation.Y < 0 || mouseDownLocation.Y > displayScreen.Height)
                        {
                            DeleteObject(index);//if the object get out of the display it will be deleted
                        }
                        else
                        {
                            //update object info
                            objectsList[index].X = mouseDownLocation.X;
                            objectsList[index].Y = mouseDownLocation.Y;
                            highlightedIndex = index;
                        }
                        propTab.SelectedIndex = 0;//set the propTab to the object
                        UpdateInfo();
                    }
                }
                else
                {
                    SaveObjectsState();
                    timerMove.Enabled = false;
                }
            }
            catch { }

        }

        // ----- Set the Properties for display, lens and source and others--- 
        public void SetProperties()
        {
            try
            {
                //set properties grids
                infoPropGrid.SelectedObject = generalInfo;
                lensPropGrid.SelectedObject = lensInfo;
                mirrorPropGrid.SelectedObject = mirrorInfo;
                mediumPropGrid.SelectedObject = mediumInfo;
                screenPropGrid.SelectedObject = screenProp;
                generalPropGrid.SelectedObject = generalProp;

                //focus on object
                if (highlightedIndex!=-1 && highlightedIndex < objectsList.Count && highlightedIndex >= 0)
                {                   
                    if (objectsList[highlightedIndex] is Lens) //focus on lenses
                    {
                        objectPropGrid.SelectedObject = ((Lens)objectsList[highlightedIndex]);
                        infoTab.SelectedIndex = 1;
                    }
                    else if (objectsList[highlightedIndex] is Source) //focus on sources
                    {
                        objectPropGrid.SelectedObject = ((Source)objectsList[highlightedIndex]);
                        if (objectsList[highlightedIndex] is StraightSource) objectPropGrid.SelectedObject = ((StraightSource)objectsList[highlightedIndex]);
                        if (objectsList[highlightedIndex] is CircularSource) objectPropGrid.SelectedObject = ((CircularSource)objectsList[highlightedIndex]);
                    }
                    else
                    {
                        if (objectsList[highlightedIndex] is Block) { objectPropGrid.SelectedObject = ((Block)objectsList[highlightedIndex]); }
                        if (objectsList[highlightedIndex] is Medium) { objectPropGrid.SelectedObject = ((Medium)objectsList[highlightedIndex]); infoTab.SelectedIndex = 3; };
                        if (objectsList[highlightedIndex] is Mirror) {objectPropGrid.SelectedObject = ((Mirror)objectsList[highlightedIndex]);infoTab.SelectedIndex = 2;}
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //when propertyGridObject changed values update values
        private void propertyGridObject_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateInfo();            
        }
        //Update informtion throught the system
        public void UpdateInfo()
        {
            UpdateScreenValues(); //update screen info
            displayScreen.Invalidate();   //redraw everthing         
            SetProperties(); //refresh Properties info
        }
        //Update screen values
        public void UpdateScreenValues()
        {
            displayWidth = displayScreen.Width;
            displayHeight = displayScreen.Height;
            displayScreen.BackColor = ScreenProp.screenColor;
        }
        //delete an object from the list by his index
        public void DeleteObject(int index)
        {
            try
            {
                if (objectsList.Count > 0 && index >= 0)
                {
                    objectsList.RemoveAt(index);
                    highlightedIndex = 0;
                    UpdateInfo();
                    SaveObjectsState();
                    Console.WriteLine("Deleted");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //gives a copy of list of objects
        public List<PhysicalObject> CopyObjects(List<PhysicalObject> list)
        {
            try
            {
                List<PhysicalObject> listCopy = new List<PhysicalObject>();
                for (int i = 0; i < list.Count; i++)
                {
                    listCopy.Add((PhysicalObject)list[i].GetCopy());
                }
                return listCopy;
            }
            catch
            { return null; }
        }
        //undo the last action
        public void UndoObjects()
        {
            try
            {
                if (undoStack.Count > 1)
                {
                    restoreStack.Push(undoStack.Pop());
                    objectsList = CopyObjects(undoStack.Peek());
                }
                UpdateInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //redo the last action
        public void RestoreObjects()
        {
            try
            {
                if (restoreStack.Count > 0)
                {
                    objectsList = restoreStack.Pop();
                }
                UpdateInfo();
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
        }
        //save the object list current state 
        public void SaveObjectsState()
        {
            undoStack.Push(CopyObjects(objectsList));
            UpdateInfo(); 
        }
        //Mouse leaves display panel
        private void Display_MouseLeave(object sender, EventArgs e)
        {
            isMouseDown = false;
        }
        private void Main_Form_Load(object sender, EventArgs e)
        {
            UpdateInfo();
        }
        //redo nutton
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreObjects();
        }
        //undo button
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoObjects();
        }
        //clean all button
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to clean all items?",
                                     "Confirm Action",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                highlightedIndex = -1;
                objectsList = new List<PhysicalObject>();
                UpdateInfo();
            }
            else
            {
                // If 'No', do something here.
            }
        }


        private void propTab_MouseDown(object sender, MouseEventArgs e)
        {
            SaveObjectsState();
        }
        //save button
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "bin files (*.bin)|*.bin";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;

                if (this.fileName!=""||saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    IFormatter formatter = new BinaryFormatter();
                    string path = saveFileDialog1.FileName;
                    if (this.fileName != "") path = this.fileName;
                    Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    formatter.Serialize(stream, objectsList);
                    stream.Close();
                    this.fileName = path;
                }
                saveFileDialog1.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //save as button
        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "bin files (*.bin)|*.bin";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    IFormatter formatter = new BinaryFormatter();
                    string path = saveFileDialog1.FileName;
                    Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    formatter.Serialize(stream, objectsList);
                    stream.Close();
                    this.fileName = path;
                }
                saveFileDialog1.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //open button
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "bin files (*.bin)|*.bin";
            openFileDialog1.RestoreDirectory = true;

            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    if (openFileDialog1.OpenFile() != null)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        string path = openFileDialog1.FileName;
                        myStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                        objectsList = (List<PhysicalObject>)formatter.Deserialize(myStream);
                        myStream.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            UpdateInfo();
        }
        // new project button
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to open new project?\nMake sure you saved all you need",
                         "Confirm Action",
                         MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                objectsList = new List<PhysicalObject>();
                UpdateInfo();
            }
            else
            {
                // If 'No', do something here.
            }
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        //print button
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.AllowSomePages = true;
            printDialog1.Document = printDocument1;
            
            if(printDialog1.ShowDialog()==DialogResult.OK)
            {
                printDocument1.DefaultPageSettings.Landscape = true;
                printDocument1.Print();
            }          
        }
        //print page action
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            try
            {
                for (int i = 0; i < objectsList.Count; i++)
                {
                    objectsList[i].Draw(g, i == highlightedIndex);
                }

                //
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //print preview button
        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument1.DefaultPageSettings.Landscape = true;
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }
        //copy button
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (highlightedIndex != -1)
                {
                    saveObjectsList = (PhysicalObject)objectsList[highlightedIndex].GetCopy();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //paste button
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsList.Count < maxObjects)
            {
                if(saveObjectsList!=null)
                {
                    if (objectsList.Count < maxObjects)
                    {
                        objectsList.Add(saveObjectsList);
                        UpdateInfo();
                    }
                }                
            }
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (highlightedIndex != -1)
                {
                    saveObjectsList = (PhysicalObject)objectsList[highlightedIndex].GetCopy();
                    DeleteObject(highlightedIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // A couple of function used for style
        private void pictureBox4_MouseDown_1(object sender, MouseEventArgs e)
        {
            selectedPic = 4;
        }

        private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 5;
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 1;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 2;
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 3;
        }

        private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 6;
        }

        private void pictureBox7_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 7;
        }

        private void pictureBox8_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 8;
        }

        private void pictureBox9_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 9;
        }

        private void pictureBox10_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPic = 10;
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isFullScreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                isFullScreen = true;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Maximized;
                isFullScreen = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options settings = new Options();
            settings.Show();
        }
        private void propBar_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is the properties panel.\nIt displays information about the objects";
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a Plano-Convex lens.\nIt can focus light.";
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a Biconvex lens.\nIt can focus light.";
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a Positive Meniscus lens.\nIt can focus light.";
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a Plano-Concave lens.\nIt can disperse light.";
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a Biconcave lens.\nIt can disperse light.";
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a straight light source.\nIt emits straight light from a square base.";
        }

        private void pictureBox7_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a circle light source.\nIt emits straight light from a circle base.";
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a block object.\nIt can block light .";
        }

        private void pictureBox9_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a medium object.\nIt curves light .";
        }

        private void pictureBox10_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is a mirror object.\nIt reflects light"; 
        }

        private void propertyGridDisplay_MouseHover(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is the properties panel.\nIt displays information about the objects";
        }

        private void infoBar_MouseEnter(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is the information panel.\nIt displays general basic information";
        }

        private void propertyGrid1_MouseHover(object sender, EventArgs e)
        {
            descriptionLabel.Text = "This is the information panel.\nIt displays general basic information";
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Feedback feedback = new Feedback();
            feedback.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            toolBar.BackColor = Properties.Settings.Default.DefultBackground;
            descriptionPanel.BackColor = Properties.Settings.Default.DefultBackground;
            infoBar.BackColor = Properties.Settings.Default.DefultBackground;
            propBar.BackColor = Properties.Settings.Default.DefultBackground;
            menuStrip.BackColor = SystemColors.Control;
            menuStrip.ForeColor = Color.Black;
            informationTitle.ForeColor = Color.Black;
            descriptionTitle.ForeColor = Color.Black;
            PropertiesTitle.ForeColor = Color.Black;
            toolBoxTitle.ForeColor = Color.Black;
            descriptionLabel.ForeColor = Color.Black;
            UpdateInfo();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            toolBar.BackColor = Properties.Settings.Default.DarkBackground;
            descriptionPanel.BackColor = Properties.Settings.Default.DarkBackground;
            infoBar.BackColor = Properties.Settings.Default.DarkBackground;
            propBar.BackColor = Properties.Settings.Default.DarkBackground;
            menuStrip.BackColor = Properties.Settings.Default.DarkBackground;
            menuStrip.ForeColor = Color.White;
            informationTitle.ForeColor = Color.White;
            descriptionTitle.ForeColor = Color.White;
            PropertiesTitle.ForeColor = Color.White;
            toolBoxTitle.ForeColor = Color.White;
            descriptionLabel.ForeColor = Color.White;
            UpdateInfo();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            toolBar.BackColor = Properties.Settings.Default.GreenBackground;
            descriptionPanel.BackColor = Properties.Settings.Default.GreenBackground;
            infoBar.BackColor = Properties.Settings.Default.GreenBackground;
            propBar.BackColor = Properties.Settings.Default.GreenBackground;
            menuStrip.BackColor = Properties.Settings.Default.GreenBackground;
            menuStrip.ForeColor = Color.White;
            informationTitle.ForeColor = Color.White;
            descriptionTitle.ForeColor = Color.White;
            PropertiesTitle.ForeColor = Color.White;
            toolBoxTitle.ForeColor = Color.White;
            descriptionLabel.ForeColor = Color.White;
            UpdateInfo();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            toolBar.BackColor = Properties.Settings.Default.RedBackground;
            descriptionPanel.BackColor = Properties.Settings.Default.RedBackground;
            infoBar.BackColor = Properties.Settings.Default.RedBackground;
            propBar.BackColor = Properties.Settings.Default.RedBackground;
            menuStrip.BackColor = Properties.Settings.Default.RedBackground;
            menuStrip.ForeColor = Color.White;
            informationTitle.ForeColor = Color.White;
            descriptionTitle.ForeColor = Color.White;
            PropertiesTitle.ForeColor = Color.White;
            toolBoxTitle.ForeColor = Color.White;
            descriptionLabel.ForeColor = Color.White;
            UpdateInfo();
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolBar.BackColor = Properties.Settings.Default.BlueBackground;
            descriptionPanel.BackColor = Properties.Settings.Default.BlueBackground;
            infoBar.BackColor = Properties.Settings.Default.BlueBackground;
            propBar.BackColor = Properties.Settings.Default.BlueBackground;
            menuStrip.BackColor = Properties.Settings.Default.BlueBackground;
            menuStrip.ForeColor = Color.White;
            informationTitle.ForeColor = Color.White;
            descriptionTitle.ForeColor = Color.White;
            PropertiesTitle.ForeColor = Color.White;
            toolBoxTitle.ForeColor = Color.White;
            descriptionLabel.ForeColor = Color.White;
            UpdateInfo();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolBar.BackColor = Properties.Settings.Default.LightBackground;
            descriptionPanel.BackColor = Properties.Settings.Default.LightBackground;
            infoBar.BackColor = Properties.Settings.Default.LightBackground;
            propBar.BackColor = Properties.Settings.Default.LightBackground;
            menuStrip.BackColor = Properties.Settings.Default.LightBackground;
            menuStrip.ForeColor = Color.Black;
            informationTitle.ForeColor = Color.Black;
            descriptionTitle.ForeColor = Color.Black;
            PropertiesTitle.ForeColor = Color.Black;
            toolBoxTitle.ForeColor = Color.Black;
            descriptionLabel.ForeColor = Color.Black;
            UpdateInfo();
        }

        private void propertyGridDisplay_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateInfo();
        }
        private void Display_DragOver(object sender, DragEventArgs e)
        {
            Point screenCoords = Cursor.Position;
            mouseDownLocation = this.displayScreen.PointToClient(screenCoords);
        }

        private void Display_MouseDown(object sender, MouseEventArgs e)
        {
            timerMove.Enabled = true;
            isMouseDown = true;
            mouseDownLocation = this.displayScreen.PointToClient(Cursor.Position);
        }

        private void Display_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            timerMove.Enabled = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectsList.Count >= 1 && highlightedIndex >= 0)
            {
                DeleteObject(highlightedIndex);
            }
        }

    }
}
