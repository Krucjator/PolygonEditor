using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditorCS
{
    public static class GP
    {
        public const int distance = 6;
        public const int pointSize = 10;
        public const int offset = pointSize / 2;
        public const int iconOffset = 10;

    }

    public class PictureBoxManager
    {
        PictureBox pictureBox;
        public readonly int width;
        public readonly int height;
        List<Point> points; 
        List<TextBox> angles;       //kąty przechowywane jako wartość textboxu, jeżeli null to ograniczenie nie jest ustawione
        List<(Edge e, string rstr, Label label)> edgeRestrictions;
        public Point activePoint;
        public Edge activeEdge;
        public bool moving;
        public int vertexCount;
        public ContextMenuStrip edgeMenuStrip;           //horiznotal lub vertical
        Point LowestPoint;                               //do algorytmu Grahama

        public PictureBoxManager(PictureBox pictureBox1, int vCount)
        {
            pictureBox = pictureBox1;
            width = pictureBox.Width;
            height = pictureBox.Height;
            moving = false;
            pictureBox.Image = new Bitmap(width, height);
            InitEdgeMenuStrip();
            InitPictureBox(vCount);
            Paint();
        }

        private void InitEdgeMenuStrip()
        {
            edgeMenuStrip = new ContextMenuStrip();
            edgeMenuStrip.Items.Add("Horizontal");
            edgeMenuStrip.Items.Add("Vertical");
            edgeMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(edgeMenuStripItemClicked);
        }

        private void edgeMenuStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            edgeRestrictions.RemoveAll(w => w.e.IsEqual(activeEdge));
            switch (item.Text)
            {
                case "Horizontal":
                    edgeRestrictions.Add((activeEdge, "Horizontal", CreateLabel("H")));

                    break;
                case "Vertical":
                    edgeRestrictions.Add((activeEdge, "Vertical", CreateLabel("V")));
                    break;
            }
            Paint();
        }

        private Label CreateLabel(string icon)
        {
            Label V = new Label();
            V.Text = icon;
            V.Font = new Font("Arial", 12);
            V.Size = new Size(14, 14);
            V.ForeColor = Color.OrangeRed;
            V.Padding = new Padding(0);
            V.Margin = new Padding(0);
            V.Parent = pictureBox;

            return V;
        }

        public void Paint()
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics e = Graphics.FromImage(bitmap))
            {
                ApplySetAngles();
                ApplyRestrictions();
                DrawPoints(e);
                DrawLines(bitmap);
                ChangeLabelLocations();
                ChangeTextBoxLocations();
            }
            pictureBox.Image = bitmap;

        }

        private void ApplySetAngles()
        {
            for (int i = 0; i < angles.Count; i++)
            {
                TextBox item = angles[i];
                if (item != null)
                {
                    if (double.TryParse(item.Text, out double angle) && angle > 0 && angle < 360)
                    {

                        Point a = new Point(points[(i - 1) < 0 ? points.Count - 1 : i - 1].x, points[(i - 1) < 0 ? points.Count - 1 : i - 1].y);
                        Point b = new Point(points[i].x, points[i].y);
                        double dist = Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
                        a.x = a.x - b.x;
                        a.y = a.y - b.y;
                        angle = (Math.PI / 180) * angle;
                        (double re, double im) = (Math.Cos(angle), Math.Sin(angle));
                        double x = a.x * re - a.y * im;
                        double y = a.x * im + a.y * re;
                        double d = (x * x * dist * dist) / (x * x + y * y);
                        //double x1 = Math.Sqrt(d);
                        //double y1 = Math.Sqrt((dist * dist) - d);
                        x = x + b.x;
                        y = y + b.y;
                        if (x >= width) x = width - 1;
                        if (x < 0) x = 0;
                        if (y >= height) y = height - 1;
                        if (y < 0) y = 0;
                        Point c = new Point((int)x, (int)y);
                        points[(i + 1) % points.Count] = c;
                        //x1 = x1 + b.x;
                        //y1 = y1 + b.y;
                        //if (x1 >= width) x1 = width - 1;
                        //if (x1 < 0) x1 = 0;
                        //if (y1 >= height) y1 = height - 1;
                        //if (y1 < 0) y1 = 0;
                        //Point c = new Point((int)x1, (int)y1);
                        //points[(i + 1) % points.Count] = c;
                    }
                }
            }
        }

        private void ChangeTextBoxLocations()
        {
            for (int i = 0; i < angles.Count; i++)
            {
                if (angles[i] != null)
                    angles[i].Location = new System.Drawing.Point(points[i].x + GP.offset, points[i].y + GP.offset);
            }
        }

        private void ChangeLabelLocations()
        {
            foreach (var (e, rstr, label) in edgeRestrictions)
            {
                label.Location = new System.Drawing.Point((e.a.x + e.b.x) / 2 + GP.offset, (e.a.y + e.b.y) / 2 + GP.offset);
                label.Show();
            }
        }

        private void ApplyRestrictions()
        {
            {   //(Edge e, string rstr) first;
                //int j = 0;
                //if ((first = edgeRestrictions.FirstOrDefault(w => w.e.a == activePoint || w.e.b == activePoint)).e != null)
                //{
                //    j = edgeRestrictions.IndexOf(first);
                //}
                //for (int i = 0; i < edgeRestrictions.Count; i++)
                //{
                //    Edge edge = edgeRestrictions[(j + i) % edgeRestrictions.Count].e;
                //    switch (edgeRestrictions[(j + i) % edgeRestrictions.Count].rstr)
                //    {
                //        case "H":
                //            if (activePoint == edge.a)
                //                edge.b.y = edge.a.y;
                //            else
                //                edge.a.y = edge.b.y;
                //            break;
                //    }
                //}}
            } //order of resolving stuff

            foreach (var tuple in edgeRestrictions)
            {
                switch (tuple.rstr)
                {
                    case "Horizontal":
                        if (activePoint == tuple.e.a)
                            tuple.e.b.y = tuple.e.a.y;
                        else
                            tuple.e.a.y = tuple.e.b.y;
                        break;
                    case "Vertical":
                        if (activePoint == tuple.e.a)
                            tuple.e.b.x = tuple.e.a.x;
                        else
                            tuple.e.a.x = tuple.e.b.x;
                        break;
                }

            }

        }

        private double DistanceSq(Point a, Point b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
        }

        private int Orientation(Point p, Point q, Point r)
        {
            int val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
            if (val == 0) return 0;  // colinear 
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        private int CompareByPolarAngle(Point a, Point b)
        {
            int o = Orientation(LowestPoint, a, b);
            if (o == 0)
                return (DistanceSq(LowestPoint, b) >= DistanceSq(LowestPoint, a)) ? -1 : 1;
            return (o == 2) ? -1 : 1;
        }

        internal void ConvexHull() //lab task
        {
            int index = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[index].y > points[i].y)
                {
                    index = i;
                }
                if (points[index].y == points[i].y && points[index].x > points[i].x) index = i;
            }
            LowestPoint = points[index];
            List<Point> list = points.ToList();
            list.Sort(CompareByPolarAngle);
            Stack<Point> stack = new Stack<Point>();
            stack.Push(list[0]);
            stack.Push(list[1]);
            stack.Push(list[2]);
            for (int i = 3; i < list.Count; i++)
            {
                // Keep removing top while the angle formed by 
                // points next-to-top, top, and points[i] makes 
                // a non-left turn 
                while (Orientation(stack.Skip(1).First(), stack.Peek(), list[i]) != 2)
                    stack.Pop();
                stack.Push(list[i]);
            }
 
            points = list.ToList();
            List<Point> finalPoints = stack.ToList();
            foreach (var item in list)
            {
                if (!finalPoints.Contains(item))
                {
                    DeleteVertex(item);
                }
            }
            Paint();
        }

        private void DeleteVertex(Point item)
        {
            vertexCount--;
            int ind = points.IndexOf(item);
            if (angles[ind] != null) angles[ind].Visible = false;
            angles.RemoveAt(ind);
            points.Remove(item);
            edgeRestrictions.ForEach(x => x.label.Visible = false);
            edgeRestrictions.RemoveAll(w => w.e.a == item || w.e.b == item);
        }

        internal void LockAngle()
        {
            foreach (var (edge, rstr, label) in edgeRestrictions)
            {
                if (edge.a == activePoint || edge.b == activePoint)
                {
                    return;
                }
            }

            int index = points.IndexOf(activePoint);
            int a = (index - 1) < 0 ? angles.Count - 1 : index - 1;
            int b = (index + 1) == angles.Count ? 0 : index + 1;
            if (angles[a] != null || angles[b] != null) return;
            if (angles[index] == null)
            {
                angles[index] = new TextBox
                {
                    Font = new Font("Arial", 12),
                    Enabled = true,
                    Text = "90",
                    Parent = pictureBox
                };
            }
            Paint();
        }

        private void DrawLines(Bitmap bitmap)
        {
            for (int i = 0; i < points.Count; i++)
            {
                DrawLine(points[i].x, points[i].y, points[(i + 1) % points.Count].x, points[(i + 1) % points.Count].y, bitmap);
            }
        }

        private void DrawPoints(Graphics e)
        {
            e.FillRectangle(Brushes.White, 0, 0, width, height);
            for (int i = 0; i < points.Count; i++)
            {
                e.FillRectangle(Brushes.Black, points[i].x - GP.offset, points[i].y - GP.offset, GP.pointSize, GP.pointSize);
            }
        }

        public void DrawLine(int x, int y, int x2, int y2, Bitmap bitmap) //brezenham
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (longest <= shortest)
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                bitmap.SetPixel(x, y, Color.Black);
                numerator += shortest;
                if (numerator >= longest)
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        internal void AdjustMenuStrip()
        {
            foreach (ToolStripItem item in edgeMenuStrip.Items)
            {
                item.Enabled = true;
            }
            int a = points.IndexOf(activeEdge.a);
            int b = points.IndexOf(activeEdge.b);

            if (angles[a] != null || angles[b] != null)
            {
                foreach (ToolStripItem item in edgeMenuStrip.Items)
                {
                    item.Enabled = false;
                }
                return;
            }



            foreach (var etup in edgeRestrictions)
            {
                if (etup.e.b == activeEdge.a || etup.e.a == activeEdge.b || etup.e.a == activeEdge.a)
                {
                    switch (etup.rstr)
                    {
                        case "Horizontal":
                            edgeMenuStrip.Items[0].Enabled = false;
                            break;
                        case "Vertical":
                            edgeMenuStrip.Items[1].Enabled = false;
                            break;

                    }
                }
            }
        }

        private void InitPictureBox(int x)
        {
            Random rnd = new Random();
            points = new List<Point>();
            angles = new List<TextBox>();
            while (points.Count < x)
            {
                AddPoint(rnd.Next(GP.offset, width), rnd.Next(GP.offset, height));
            }
            for (int i = 0; i < x; i++)
            {
                angles.Add(null);
            }
            edgeRestrictions = new List<(Edge, string, Label)>();
        }

        public bool IsBlack(int x, int y)
        {
            return ((Bitmap)pictureBox.Image).GetPixel(x, y).ToArgb() == Color.Black.ToArgb();
        }

        public Color GetColor(int x, int y)
        {
            return ((Bitmap)pictureBox.Image).GetPixel(x, y);
        }

        internal Point GetNearbyPoint(int x, int y)
        {
            Point p = new Point(x, y);
            foreach (var point in points)
            {
                if (!p.FurtherThanDistance(point))
                    return point;
            }
            return null;
        }

        internal Edge GetNearbyEdge(int x, int y) //creates new Edge
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (DistanceFromSection(points[i], points[(i + 1) % points.Count], x, y) <= GP.distance)
                    return new Edge(points[i], points[(i + 1) % points.Count]);
            }

            return null;
        }

        internal void DeleteActiveVertex()
        {
            if (vertexCount > 3)
            {
                vertexCount--;
                int index = points.IndexOf(activePoint);
                if (angles[index] != null) angles[index].Visible = false;
                angles.RemoveAt(index);
                points.Remove(activePoint);
                edgeRestrictions.ForEach(x => x.label.Visible = false);
                edgeRestrictions.RemoveAll(w => w.e.a == activePoint || w.e.b == activePoint);
                Paint();
            }
        }

        private bool AddPoint(int x, int y)
        {
            Point point = new Point(x, y);
            foreach (var a in points)
            {
                if (!point.FurtherThanDistance(a))
                    return false;
            }
            points.Add(point);
            vertexCount++;
            return true;
        }

        public void Split(int x, int y)
        {
            RemoveRestrictionsOnActive();
            int i = points.IndexOf(activeEdge.b);
            Point point = new Point(x, y);
            activePoint = point;
            points.Insert(i, point);
            angles.Insert(i, null);
            moving = true;
            vertexCount++;
            Paint();
        }

        public void RemoveRestrictionsOnActive()
        {
            edgeRestrictions.ForEach(e => e.label.Visible = false);
            edgeRestrictions.RemoveAll(w => w.e.IsEqual(activeEdge));
        }

        private double DistanceFromSection(Point p1, Point p2, int x, int y)
        {
            {
                double dx = p2.x - p1.x;
                double dy = p2.y - p1.y;


                // Calculate the t that minimizes the distance.
                double t = ((x - p1.x) * dx + (y - p1.y) * dy) /
                    (dx * dx + dy * dy);

                // See if this represents one of the segment's
                // end points or a point in the middle.
                switch (t)
                {
                    case double _t when _t < 0:
                        dx = x - p1.x;
                        dy = y - p1.y;
                        break;
                    case double _t when _t > 1:
                        dx = x - p2.x;
                        dy = y - p2.y;
                        break;
                    default:
                        dx = x - (p1.x + t * dx);
                        dy = y - (p1.y + t * dy);
                        break;
                }
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

    }


    public static class PointExtensions
    {
        ///<summary>
        ///is distance grater than GP.distance
        ///</summary>
        public static bool FurtherThanDistance(this Point a, Point b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y)) >= GP.distance;
        }
    }

    public class Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Edge //compare edges using isEqual
    {
        public Point a, b;

        public Edge(Point point1, Point point2)
        {
            a = point1;
            b = point2;
        }
        /// <summary>
        /// edge comparision
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsEqual(Edge f)
        {
            return a == f.a && b == f.b;
        }

    }
}
