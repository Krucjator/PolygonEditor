using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditorCS
{
    public partial class PolygonEditor : Form
    {
        private SplitContainer splitContainer1;
        public PictureBox pictureBox1;
        private Button button1;
        public PictureBoxManager boxManager;
        public PolygonEditor()
        {
            InitializeComponent();
            boxManager = new PictureBoxManager(pictureBox1, 3);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }



        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Black;
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Size = new System.Drawing.Size(882, 553);
            this.splitContainer1.SplitterDistance = 738;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(732, 547);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(41, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "convex hull";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PolygonEditor
            // 
            this.ClientSize = new System.Drawing.Size(882, 553);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PolygonEditor";
            this.Text = "PolygonEditor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(boxManager.GetColor(e.X, e.Y).ToString()); //poor man's debugger
            //MessageBox.Show(boxManager.GetKnownColor(e.X, e.Y).ToString());
            if ((boxManager.activePoint = boxManager.GetNearbyPoint(e.X,e.Y))!=null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    boxManager.moving = true;
                    return;
                }
                if (e.Button == MouseButtons.Right)
                {
                    boxManager.DeleteActiveVertex();
                }
                if(e.Button == MouseButtons.Middle)
                {
                    boxManager.LockAngle();
                }

            }
            if ((boxManager.activeEdge=boxManager.GetNearbyEdge(e.X, e.Y))!=null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    boxManager.Split(e.X, e.Y);
                }
                if (e.Button == MouseButtons.Middle)
                {
                    boxManager.RemoveRestrictionsOnActive();
                    boxManager.Paint();
                }
                return;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            boxManager.moving = false;
            if (e.Button == MouseButtons.Right)
            {
                if ((boxManager.activeEdge = boxManager.GetNearbyEdge(e.X, e.Y)) != null)
                {
                    boxManager.AdjustMenuStrip();
                    boxManager.edgeMenuStrip.Show(pictureBox1,new System.Drawing.Point(e.X, e.Y));
                }
            }
            return;
            
        }



        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (boxManager.moving)
            {
                if(e.X>= 0 && e.X<boxManager.width && e.Y>=0 && e.Y < boxManager.height)
                {
                    boxManager.activePoint.x = e.X;
                    boxManager.activePoint.y = e.Y;
                    boxManager.Paint();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            boxManager.ConvexHull();
        }
    }
}
