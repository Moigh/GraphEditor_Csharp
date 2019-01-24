using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
namespace Redactor_Vector_Graph {
    [DataContract]
    public class PointW {
         public static double zoom = 1;
         public static Point offset = new Point(0, 0);
        [DataMember] public double X;
        [DataMember] public double Y;
        public PointW(double setX, double setY) {
            X = setX;
            Y = setY;
        }
        public PointW(int setX, int setY) {
            ToPointW(new Point(setX, setY));

        }
        public PointW() {
            X = 0;
            Y = 0;
        }
        public void ToPointW(Point point) {
            X = (point.X - offset.X) / zoom;
            Y = (point.Y - offset.Y) / zoom;
        }
        public static PointW ScrnToPointW(Point point) {
            return new PointW((point.X - offset.X) / zoom, (point.Y - offset.Y) / zoom);
        }
        public Point ToScrPnt() {
            return new Point((int)Math.Round(X * zoom) + offset.X, (int)Math.Round(Y * zoom) + offset.Y);
        }
        public PointW Clone() {
            return new PointW(X, Y);
        }
    }
    [KnownType(typeof(PolyLine))]
    [KnownType(typeof(Line))]
    [KnownType(typeof(RectangularFigure))]
    [KnownType(typeof(Rect))]
    [KnownType(typeof(RoundedRect))]
    [KnownType(typeof(Ellipse))]
    [DataContract]
    public abstract class Figure {
        public List<Anchor> anchorArray = new List<Anchor>(8);
        public Dictionary<string, Prop> propArray = new Dictionary<string, Prop>(5);
        [DataMember] public Color colorFill;
        [DataMember] public Color colorPen;
        [DataMember] public float widthPen;
        [DataMember] public PointW startPointW;
        [DataMember] public PointW endPointW;
        [DataMember] public bool isFill = false;
        [DataMember] public bool isSelected = false;
        public Rectangle rectColider;
        public virtual void Draw(Graphics graphics) { }
        public virtual void Move(PointW offset) { }
        public virtual void AddPoint(PointW pointW) { }
        public virtual bool SelectPoint(Point pntClick) { return false; }
        public virtual void DrawColider(Graphics graphics) { }
        public virtual bool SelectArea(Rectangle area) { return false; }
        public virtual void Load() { }
        public virtual string ToSvgFormat() { return ""; }
    }
    [DataContract]
    public class PolyLine : Figure {
        [DataMember] public List<PointW> pointsArray = new List<PointW>(10);
        public PolyLine(Pen setPen, PointW start) {
            colorPen = setPen.Color;
            widthPen = setPen.Width;
            pointsArray.Add(start);

            Load();
        }
        public override void Load() {
            anchorArray = new List<Anchor>(8);
            propArray = new Dictionary<string, Prop>(5);
            for (int i = 0; i <= pointsArray.Count - 1; i++) {
                anchorArray.Add(new Anchor(pointsArray, i, SetMaxMin));
            }
            startPointW = pointsArray[0].Clone();
            endPointW = new PointW(0.0, 0.0);
            foreach (var pointW in pointsArray) {
                startPointW.X = Math.Min(startPointW.X, pointW.X);
                startPointW.Y = Math.Min(startPointW.Y, pointW.Y);
                endPointW.X = Math.Max(endPointW.X, pointW.X);
                endPointW.Y = Math.Max(endPointW.Y, pointW.Y);
            }

            propArray.Add("PropColor", new PropColor(Color.Black));
            propArray.Add("PropPenWidth", new PropPenWidth());
            ((PropColor)propArray["PropColor"]).colorButton.color = colorPen;
            ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth = widthPen;
        }
        public override bool SelectPoint(Point pntClick) {
            const int dist = 20;
            foreach (PointW pointW in pointsArray) {
                if ((Math.Pow(pointW.ToScrPnt().X - pntClick.X, 2.0) + Math.Pow(pointW.ToScrPnt().Y - pntClick.Y, 2.0)) <= dist * dist) {
                    isSelected = true;
                    return true;
                }
            }
            isSelected = false;
            return false;
        }
        public override void Move(PointW offset) {
            foreach (PointW pointW in pointsArray) {
                pointW.X += offset.X;
                pointW.Y += offset.Y;
            }
        }
        public override bool SelectArea(Rectangle area) =>
            area.Contains(startPointW.ToScrPnt()) && area.Contains(endPointW.ToScrPnt());

        public override void AddPoint(PointW pointW) {
            pointsArray.Add(pointW);
            startPointW.X = Math.Min(startPointW.X, pointW.X);
            startPointW.Y = Math.Min(startPointW.Y, pointW.Y);
            endPointW.X = Math.Max(endPointW.X, pointW.X);
            endPointW.Y = Math.Max(endPointW.Y, pointW.Y);
            anchorArray.Add(new Anchor(pointsArray, pointsArray.Count - 1, SetMaxMin));
        }
        public void SetMaxMin(PointW pointW) {
            startPointW.X = Math.Min(startPointW.X, pointW.X);
            startPointW.Y = Math.Min(startPointW.Y, pointW.Y);
            endPointW.X = Math.Max(endPointW.X, pointW.X);
            endPointW.Y = Math.Max(endPointW.Y, pointW.Y);
        }
        public override void Draw(Graphics graphics) {
            colorPen = ((PropColor)propArray["PropColor"]).colorButton.color;
            widthPen = ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth;
            Pen pen = new Pen(colorPen);
            pen.Width = widthPen;
            PointW lastPointW = pointsArray[0];
            foreach (PointW pointW in pointsArray) {
                graphics.DrawLine(pen, pointW.ToScrPnt(), lastPointW.ToScrPnt());
                lastPointW = pointW;
            }
        }
        public override void DrawColider(Graphics graphics) {
            if (isSelected) {
                foreach (Anchor anchor in anchorArray) {
                    anchor.Draw(graphics);
                }
            }
        }
        public override string ToSvgFormat() {
            string str = "<polyline points='";
            foreach(var PointW in pointsArray) {
                str += PointW.ToScrPnt().X+","+PointW.ToScrPnt().Y+" ";
            }
            str += "' stroke-width='" + widthPen + "' stroke='" + ColorTranslator.ToHtml(colorPen) + "' fill = 'none'/>  ";
            return str;
        }
    }
    [Serializable]
    public class Line : Figure {
        public Line(Pen setPen, PointW start) {
            colorPen = setPen.Color;
            widthPen = setPen.Width;
            startPointW = start;
            Load();
        }
        public override void Load() {
            anchorArray = new List<Anchor>(8);
            propArray = new Dictionary<string, Prop>(5);
            anchorArray.Add(new Anchor(ref startPointW));
            anchorArray.Add(new Anchor(ref endPointW));
            propArray.Add("PropColor", new PropColor(Color.Black));
            propArray.Add("PropPenWidth", new PropPenWidth());
            ((PropColor)propArray["PropColor"]).colorButton.color = colorPen;
            ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth = widthPen;
        }

        public override void Move(PointW offset) {
            startPointW.X += offset.X;
            startPointW.Y += offset.Y;
            endPointW.X += offset.X;
            endPointW.Y += offset.Y;
        }
        public override bool SelectPoint(Point pntClick) {
            const int dist = 20;
            double A, B, C, distToline, distToEndPoint, distToSatrtPoint, lineLength;
            A = startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y;
            B = endPointW.ToScrPnt().X - startPointW.ToScrPnt().X;
            C = -(startPointW.ToScrPnt().X * A + startPointW.ToScrPnt().Y * B);
            lineLength = Math.Sqrt(Math.Pow(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X, 2.0) + Math.Pow(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y, 2.0));
            distToline = (Math.Abs(A * pntClick.X + B * pntClick.Y + C) / (Math.Sqrt(A * A + B * B)));
            distToSatrtPoint = Math.Sqrt(Math.Pow(startPointW.ToScrPnt().X - pntClick.X, 2.0) + Math.Pow(startPointW.ToScrPnt().Y - pntClick.Y, 2.0)) - lineLength;
            distToEndPoint = Math.Sqrt(Math.Pow(endPointW.ToScrPnt().X - pntClick.X, 2.0) + Math.Pow(endPointW.ToScrPnt().Y - pntClick.Y, 2.0)) - lineLength;
            if (distToline <= dist && distToSatrtPoint < dist && distToEndPoint < dist) {
                isSelected = true;
                return true;
            }
            isSelected = false;
            return false;
        }
        public override bool SelectArea(Rectangle area) =>
            area.Contains(startPointW.ToScrPnt()) && area.Contains(endPointW.ToScrPnt());
        public override void AddPoint(PointW pointW) {
            endPointW = pointW;
            anchorArray[0] = new Anchor(ref startPointW);
            anchorArray[1] = new Anchor(ref endPointW);
        }
        public override void Draw(Graphics graphics) {
            colorPen = ((PropColor)propArray["PropColor"]).colorButton.color;
            widthPen = ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth;
            Pen pen = new Pen(colorPen);
            pen.Width = widthPen;
            graphics.DrawLine(pen, startPointW.ToScrPnt(), endPointW.ToScrPnt());
        }
        public override void DrawColider(Graphics graphics) {
            if (isSelected) {
                anchorArray[0].Draw(graphics);
                anchorArray[1].Draw(graphics);
            }
        }
        public override string ToSvgFormat() {
       
            return "<line x1='"+startPointW.ToScrPnt().X+ "' y1='" + startPointW.ToScrPnt().Y + "' x2='" + endPointW.ToScrPnt().X + "' y2='" + endPointW.ToScrPnt().Y + "' stroke-width='"+widthPen+ "' stroke='"+ ColorTranslator.ToHtml(colorPen) + "'/>  ";
        }
    }
    [DataContract]
    public class RectangularFigure : Figure {
        public override void Load() {
            anchorArray = new List<Anchor>(8);
            propArray = new Dictionary<string, Prop>(5);
            anchorArray.Add(new Anchor(ref startPointW));
            anchorArray.Add(new Anchor(ref endPointW));
            propArray.Add("PropColor", new PropColor(Color.Black));
            propArray.Add("PropPenWidth", new PropPenWidth());
            propArray.Add("PropFill", new PropFill(Color.Black));
            ((PropColor)propArray["PropColor"]).colorButton.color = colorPen;
            ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth = widthPen;
            ((PropFill)propArray["PropFill"]).propColor.colorButton.color = colorFill;
            ((PropFill)propArray["PropFill"]).checkBox.Checked = isFill;
        }
        public override void Move(PointW offset) {
            startPointW.X += offset.X;
            startPointW.Y += offset.Y;
            endPointW.X += offset.X;
            endPointW.Y += offset.Y;
        }
        public override void AddPoint(PointW pointW) {
            endPointW = pointW;
            anchorArray[0] = new Anchor(ref startPointW);
            anchorArray[1] = new Anchor(ref endPointW);
        }
        protected void DrawColiderRect(Graphics g, Rectangle rect) {
            rectColider = rect;
            Pen penColider = new Pen(Color.Gray);
            penColider.Width = 3;
            penColider.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            g.DrawRectangle(penColider, rect);
            anchorArray[0].Draw(g);
            anchorArray[1].Draw(g);
        }

    }
    [DataContract]
    public class Rect : RectangularFigure {
        int x, y, width, height;
        public Rect(Pen pen, PointW start, Color? setColorFill = null) {

            colorPen = pen.Color;
            widthPen = pen.Width;
            startPointW = start;
            endPointW = start;
            if (setColorFill.HasValue) {
                colorFill = (Color)setColorFill;
                isFill = true;
            }
            Load();


        }
        public override bool SelectPoint(Point pntClick) {
            if (rectColider.Contains(pntClick)) {
                isSelected = true;
                return true;
            }
            isSelected = false;
            return false;
        }
        public override bool SelectArea(Rectangle area) =>
            area.Contains(startPointW.ToScrPnt()) && area.Contains(endPointW.ToScrPnt());

        public override void Draw(Graphics graphics) {
            colorPen = ((PropColor)propArray["PropColor"]).colorButton.color;
            widthPen = ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth;
            colorFill = ((PropFill)propArray["PropFill"]).propColor.colorButton.color;
            isFill = ((PropFill)propArray["PropFill"]).checkBox.Checked;
            Pen pen = new Pen(colorPen);
            pen.Width = widthPen;
            x = Math.Min(startPointW.ToScrPnt().X, endPointW.ToScrPnt().X);
            y = Math.Min(startPointW.ToScrPnt().Y, endPointW.ToScrPnt().Y);
            width = Math.Abs(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X);
            height = Math.Abs(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y);
            graphics.DrawRectangle(pen, new Rectangle(x, y, width, height));
            if (isFill)
                graphics.FillRectangle(new SolidBrush(colorFill), new Rectangle(x + (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero),
                                          y + (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero), width - (int)(widthPen), height - (int)(widthPen)));

            rectColider = new Rectangle(x - (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero), y - (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero),
                             width + (int)(widthPen), height + (int)(widthPen));
        }
        public override void DrawColider(Graphics graphics) {
            if (isSelected)
                DrawColiderRect(graphics, new Rectangle(x, y, width, height));
        }
        public override string ToSvgFormat() {
            string strColorFill = "rgba(255, 255, 255, 0)";
            if (isFill)
                 strColorFill = ColorTranslator.ToHtml(colorFill);
            x = Math.Min(startPointW.ToScrPnt().X, endPointW.ToScrPnt().X);
            y = Math.Min(startPointW.ToScrPnt().Y, endPointW.ToScrPnt().Y);
            width = Math.Abs(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X);
            height = Math.Abs(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y);
            return "<rect x=\""+x+"\" y=\"" +y+ "\" width=\"" + width + "\" height=\"" + height + "\" " +
                   "fill=\""+ strColorFill + "\" stroke-width=\""+widthPen+"\" stroke=\""+ ColorTranslator.ToHtml(colorPen) + "\"/>";
        }
    }
    [DataContract]
    public class RoundedRect : RectangularFigure {
        [DataMember] public int radius = 25;
        int x, y, width, height;
        public RoundedRect(Pen setPen, PointW start, int setRadius, Color? setColorFill = null) {
            colorPen = setPen.Color;
            widthPen = setPen.Width;
            startPointW = start;
            endPointW = start;
            radius = setRadius;
            if (setColorFill.HasValue) {
                colorFill = (Color)setColorFill;
                isFill = true;
            }
            Load();
            
        }
        public override void Load() {
            anchorArray = new List<Anchor>(8);
            propArray = new Dictionary<string, Prop>(5);
            anchorArray.Add(new Anchor(ref startPointW));
            anchorArray.Add(new Anchor(ref endPointW));
            propArray.Add("PropColor", new PropColor(Color.Black));
            propArray.Add("PropPenWidth", new PropPenWidth());
            propArray.Add("PropFill", new PropFill(Color.Black));
            ((PropColor)propArray["PropColor"]).colorButton.color = colorPen;
            ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth = widthPen;
            ((PropFill)propArray["PropFill"]).propColor.colorButton.color = colorFill;
            ((PropFill)propArray["PropFill"]).checkBox.Checked = isFill;
            propArray.Add("PropRadius", new PropRadius(25));
            ((PropRadius)propArray["PropRadius"]).numeric.Value = radius;
        }
        public override bool SelectPoint(Point pntClick) {
            if (rectColider.Contains(pntClick)) {
                isSelected = true;
                return true;
            }
            isSelected = false;
            return false;
        }
        public override bool SelectArea(Rectangle area) =>
            area.Contains(startPointW.ToScrPnt()) && area.Contains(endPointW.ToScrPnt());

        public override void Draw(Graphics graphics) {
            colorPen = ((PropColor)propArray["PropColor"]).colorButton.color;
            widthPen = ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth;
            colorFill = ((PropFill)propArray["PropFill"]).propColor.colorButton.color;
            isFill = ((PropFill)propArray["PropFill"]).checkBox.Checked;
            radius = (int)((PropRadius)propArray["PropRadius"]).numeric.Value;
            Pen pen = new Pen(colorPen);
            pen.Width = widthPen;
            x = Math.Min(startPointW.ToScrPnt().X, endPointW.ToScrPnt().X);
            y = Math.Min(startPointW.ToScrPnt().Y, endPointW.ToScrPnt().Y);
            width = Math.Abs(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X);
            height = Math.Abs(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y);
            rectColider = new Rectangle(x - (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero), y - (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero),
                        width + (int)(widthPen), height + (int)(widthPen));

            int diameter = radius * 2;
            if (diameter * 2 > rectColider.Width & rectColider.Width <= rectColider.Height) {
                diameter = rectColider.Width + 1;
            }
            if (diameter * 2 > rectColider.Height & rectColider.Width >= rectColider.Height) {
                diameter = rectColider.Height + 1;
            }
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rectColider.Location, size);
            GraphicsPath path = new GraphicsPath();
            if (radius == 0) {
                graphics.DrawRectangle(pen, new Rectangle(x, y, width, height));
                if (isFill)
                    graphics.FillRectangle(new SolidBrush(colorFill), new Rectangle(x + (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero), y + (int)Math.Round(widthPen / 2, MidpointRounding.AwayFromZero), width - (int)(widthPen), height - (int)(widthPen)));
                return;
            }
            path.AddArc(arc, 180, 90);
            arc.X = rectColider.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rectColider.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rectColider.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            graphics.DrawPath(pen, path);
            if (isFill)
                graphics.FillPath(new SolidBrush(colorFill), path);
        }
        public override void DrawColider(Graphics graphics) {
            if (isSelected)
                DrawColiderRect(graphics, rectColider);
        }
        public override string ToSvgFormat() {
            string strColorFill = "rgba(255, 255, 255, 0)";
            if (isFill)
                strColorFill = ColorTranslator.ToHtml(colorFill);
            x = Math.Min(startPointW.ToScrPnt().X, endPointW.ToScrPnt().X);
            y = Math.Min(startPointW.ToScrPnt().Y, endPointW.ToScrPnt().Y);
            width = Math.Abs(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X);
            height = Math.Abs(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y);
            return "<rect x=\"" + x + "\" y=\"" + y + "\" rx=\"" + radius + "\" ry=\"" + radius + "\" width=\"" + width + "\" height=\"" + height + "\" " +
                   "fill=\"" + strColorFill + "\" stroke-width=\"" + widthPen + "\" stroke=\"" + ColorTranslator.ToHtml(colorPen) + "\"/>";
        }
    
}
    [DataContract]
    public class Ellipse : RectangularFigure {
        public Ellipse(Pen setPen, PointW start, Color? setColorFill = null) {
            colorPen = setPen.Color;
            widthPen = setPen.Width;
            startPointW = start;
            endPointW = start;
            if (setColorFill.HasValue) {
                colorFill = (Color)setColorFill;
                isFill = true;
            }
            Load();
        }

        public override bool SelectPoint(Point pntClick) {
            double A = (endPointW.ToScrPnt().X - startPointW.ToScrPnt().X) / 2;
            double B = (endPointW.ToScrPnt().Y - startPointW.ToScrPnt().Y) / 2;
            if (((Math.Pow((pntClick.X - startPointW.ToScrPnt().X - A) / A, 2) + (Math.Pow((pntClick.Y - startPointW.ToScrPnt().Y - B) / B, 2))) <= 1)) {
                isSelected = true;
                return true;
            }
            isSelected = false;
            return false;
        }
        public override bool SelectArea(Rectangle area) =>
            area.Contains(startPointW.ToScrPnt()) && area.Contains(endPointW.ToScrPnt());

        public override void Draw(Graphics graphics) {
            colorPen = ((PropColor)propArray["PropColor"]).colorButton.color;
            widthPen = ((PropPenWidth)propArray["PropPenWidth"]).numWidthPen.penWidth;
            colorFill = ((PropFill)propArray["PropFill"]).propColor.colorButton.color;
            rectColider = new Rectangle(startPointW.ToScrPnt(),
                    new Size(endPointW.ToScrPnt().X - startPointW.ToScrPnt().X, endPointW.ToScrPnt().Y - startPointW.ToScrPnt().Y));
            Pen pen = new Pen(colorPen);
            pen.Width = widthPen;
            graphics.DrawEllipse(pen, rectColider);
            if (isFill)
                graphics.FillEllipse(new SolidBrush(colorFill), rectColider);
        }
        public override void DrawColider(Graphics graphics) {
            if (isSelected)
                DrawColiderRect(graphics, new Rectangle(Math.Min(startPointW.ToScrPnt().X, endPointW.ToScrPnt().X), Math.Min(startPointW.ToScrPnt().Y, endPointW.ToScrPnt().Y),
                  Math.Abs(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X), Math.Abs(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y)));
        }
        public override string ToSvgFormat() {
            string strColorFill = "rgba(255, 255, 255, 0)";
            if (isFill)
                strColorFill = ColorTranslator.ToHtml(colorFill);
            int x = Math.Min(startPointW.ToScrPnt().X, endPointW.ToScrPnt().X);
            int y = Math.Min(startPointW.ToScrPnt().Y, endPointW.ToScrPnt().Y);
            int width = Math.Abs(startPointW.ToScrPnt().X - endPointW.ToScrPnt().X)/2;
            int height = Math.Abs(startPointW.ToScrPnt().Y - endPointW.ToScrPnt().Y)/2;
            return "<ellipse cx='"+ (x + width) + "' cy='"+(y+height)+"' rx='"+width+"' ry='"+height+ "' fill =\"" + strColorFill + "\" stroke-width=\"" + widthPen + "\" stroke=\"" + ColorTranslator.ToHtml(colorPen) + "\"/>";
        }
    }

    public class Anchor {
        public PointW editedPoint;
        public PointW anchorPoint;
        public delegate void EditCallBack(PointW editedPoint);
        public EditCallBack editCallBack;
        public Rectangle rect;
        public Anchor(ref PointW editedPointSet) {
            editedPoint = editedPointSet;
        }
        public Anchor(List<PointW> editedPointSet, int index, EditCallBack editCallBack) {
            editedPoint = editedPointSet[index];
            this.editCallBack = editCallBack;
        }
        public void EditPoint(Point pointTo) {
            PointW pointW = PointW.ScrnToPointW(pointTo);
            editedPoint.X = pointW.X;
            editedPoint.Y = pointW.Y;
            if (editCallBack != null) {
                editCallBack.Invoke(editedPoint);
            }
        }
        public void Draw(Graphics g) {
            const int width = 4;
            Point point = editedPoint.ToScrPnt();
            Pen pen = new Pen(Color.Black);
            pen.Width = 2;
            rect = new Rectangle(point.X - width, point.Y - width, width * 2, width * 2);
            g.DrawRectangle(pen, rect);
            g.FillRectangle(new SolidBrush(Color.White), rect);

        }

    }
}