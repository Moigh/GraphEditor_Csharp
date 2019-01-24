using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Redactor_Vector_Graph {
    class Tool {
        public static PointW pntwMinReact = new PointW(Double.MaxValue, Double.MaxValue);
        public static PointW pntwMaxReact = new PointW(0.0, 0.0);
        public static List<Figure> figureSelectionArray;
        public static Tool ActiveTool { get; set; }
        public Button toolButton;
        public List<Figure> figureArray;
        public PaintBox paintBox;
        public Cursor cursor = Cursors.Default;

        protected bool flagLeftMouseClick = false;
        protected bool flagRightMouseClick = false;
        protected PropColor propColor;
        protected PropPenWidth propPenWidth;
        protected PanelProp panelProp;
        public void ToolButtonClick(object sender, EventArgs e) {
            ActiveTool.HidePanelProp();
            ActiveTool = this;
            ActiveTool.ShowPanelProp();
            paintBox.Cursor = this.cursor;
        }
        public virtual void HidePanelProp() { }
        public virtual void ShowPanelProp() { }
        public virtual void MouseMove(object sender, MouseEventArgs e) { }
        public virtual void MouseDown(object sender, MouseEventArgs e) { }
        public virtual void MouseUp(object sender, MouseEventArgs e) { }
        public static void SetResetReact(int X, int Y) {
            PointW pointW = new PointW(X, Y);
            pntwMinReact.X = Math.Min(pointW.X, pntwMinReact.X);
            pntwMinReact.Y = Math.Min(pointW.Y, pntwMinReact.Y);
            pntwMaxReact.X = Math.Max(pointW.X, pntwMaxReact.X);
            pntwMaxReact.Y = Math.Max(pointW.Y, pntwMaxReact.Y);
        }
        public static void ResetReactUpdate(List<Figure> figureArray) {
            if(figureArray[0].startPointW !=null)
                 pntwMinReact = figureArray[0].startPointW;
            if (figureArray[0].endPointW != null)
                 pntwMaxReact = figureArray[0].endPointW;
            foreach (var figure in figureArray) {
                pntwMinReact.X = Math.Min(figure.startPointW.X, pntwMinReact.X);
                pntwMinReact.Y = Math.Min(figure.startPointW.Y, pntwMinReact.Y);
                pntwMinReact.X = Math.Min(figure.endPointW.X, pntwMinReact.X);
                pntwMinReact.Y = Math.Min(figure.endPointW.Y, pntwMinReact.Y);
                pntwMaxReact.X = Math.Max(figure.startPointW.X, pntwMaxReact.X);
                pntwMaxReact.Y = Math.Max(figure.startPointW.Y, pntwMaxReact.Y);
                pntwMaxReact.X = Math.Max(figure.endPointW.X, pntwMaxReact.X);
                pntwMaxReact.Y = Math.Max(figure.endPointW.Y, pntwMaxReact.Y);
            }
        }
        public static void UpdateSelectionArray(List<Figure> figureArray) {
            figureSelectionArray = new List<Figure>();
            foreach (var figure in figureArray) {   
                if (figure.isSelected) {
                    figureSelectionArray.Add(figure);
                }
            }
        }
    }

    class ToolRect : Tool {
        PropFill propFill;

        public ToolRect(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            panelProp = new PanelProp();
            panelProp.Text = "Rect";
            propColor = new PropColor(Color.Black);
            propColor.Draw(new Point(5, 20), panelProp, "Color:");
            propPenWidth = new PropPenWidth();
            propPenWidth.Draw(new Point(5, 50), panelProp, "Width:");
            propFill = new PropFill(Color.Black);
            propFill.Draw(new Point(5, 80), panelProp, "Fill:");
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                paintBox.Invalidate();
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = true;
                if (propFill.GetCheked())
                    figureArray.Add(new Rect(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y), propFill.GetColor()));
                else
                    figureArray.Add(new Rect(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y)));
                SetResetReact(e.X, e.Y);
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
                SetResetReact(e.X, e.Y);
                UndoRedo.SaveState();
            }
        }
        public override void HidePanelProp() {
            panelProp.Visible = false;
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
        }
    }
    class ToolRoundedRect : Tool {
        PropFill propFill;
        PropRadius propRadius;

        public ToolRoundedRect(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            panelProp = new PanelProp();
            panelProp.Text = "Rounded Rect";
            propColor = new PropColor(Color.Black);
            propColor.Draw(new Point(5, 20), panelProp, "Color:");
            propPenWidth = new PropPenWidth();
            propPenWidth.Draw(new Point(5, 50), panelProp, "Width:");
            propFill = new PropFill(Color.Black);
            propFill.Draw(new Point(5, 80), panelProp, "Fill:");
            propRadius = new PropRadius(25);
            propRadius.Draw(new Point(5, 140), panelProp, "Radius:");
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                paintBox.Invalidate();
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = true;
                if (propFill.GetCheked())
                    figureArray.Add(new RoundedRect(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y), propRadius.GetRadius(), propFill.GetColor()));
                else
                    figureArray.Add(new RoundedRect(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y), propRadius.GetRadius()));
                SetResetReact(e.X, e.Y);
                paintBox.Invalidate();
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
                SetResetReact(e.X, e.Y);
                UndoRedo.SaveState();
            }
           
        }
        public override void HidePanelProp() {
            panelProp.Visible = false;
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
        }
    }
    class ToolPolyLine : Tool {
        public ToolPolyLine(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            panelProp = new PanelProp();
            panelProp.Text = "Poly line";
            propColor = new PropColor(Color.Black);
            propColor.Draw(new Point(5, 20), panelProp, "Color:");
            propPenWidth = new PropPenWidth();
            propPenWidth.Draw(new Point(5, 50), panelProp, "Width:");
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                paintBox.Invalidate();
                SetResetReact(e.X, e.Y);
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = true;
                figureArray.Add(new PolyLine(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y)));
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                SetResetReact(e.X, e.Y);
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
                SetResetReact(e.X, e.Y);
                UndoRedo.SaveState();
            }

        }
        public override void HidePanelProp() {
            panelProp.Visible = false;
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
        }

    }
    class ToolLine : Tool {

        public ToolLine(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            panelProp = new PanelProp();
            panelProp.Text = "Line";
            propColor = new PropColor(Color.Black);
            propColor.Draw(new Point(5, 20), panelProp, "Color:");
            propPenWidth = new PropPenWidth();
            propPenWidth.Draw(new Point(5, 50), panelProp, "Width:");
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                paintBox.Invalidate();
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = true;
                figureArray.Add(new Line(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y)));
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                SetResetReact(e.X, e.Y);
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
                SetResetReact(e.X, e.Y);
                UndoRedo.SaveState();
            }
        }
        public override void HidePanelProp() {
            panelProp.Visible = false;
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
        }
    }
    class ToolEllipse : Tool {
        PropFill propFill;
        public ToolEllipse(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            panelProp = new PanelProp();
            panelProp.Text = "Ellipse";
            propColor = new PropColor(Color.Black);
            propColor.Draw(new Point(5, 20), panelProp, "Color:");
            propPenWidth = new PropPenWidth();
            propPenWidth.Draw(new Point(5, 50), panelProp, "Width:");
            propFill = new PropFill(Color.Black);
            propFill.Draw(new Point(5, 80), panelProp, "Fill:");
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                paintBox.Invalidate();
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = true;
                if (propFill.GetCheked())
                    figureArray.Add(new Ellipse(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y), propFill.GetColor()));
                else
                    figureArray.Add(new Ellipse(new Pen(propColor.GetColor(), propPenWidth.GetPenWidth()), new PointW(e.X, e.Y)));
                figureArray.Last().AddPoint(new PointW(e.X, e.Y));
                SetResetReact(e.X, e.Y);
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
                SetResetReact(e.X, e.Y);
                UndoRedo.SaveState();
            }
        }
        public override void HidePanelProp() {
            panelProp.Visible = false;
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
        }
    }
    class ToolZoom : Tool {
        NumericUpDown stepZoom;
        Label labelStepZoom;
        PointW pointWStart;
        PointW pointWEnd;
        Point pointStart;
        Point pointEnd;
        NumericUpDown numZoom;
        public ToolZoom(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set, NumericUpDown numZoomSet) {
            numZoom = numZoomSet;
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            CreatePanelProp();
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                pointWEnd = new PointW(e.X, e.Y);
                pointEnd = new Point(e.X, e.Y);
                figureArray.Last().AddPoint(pointWEnd);
                paintBox.Invalidate();
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = true;
                pointStart = new Point(e.X, e.Y);
                pointEnd = new Point(e.X + 1, e.Y + 1);
                pointWStart = new PointW(e.X, e.Y);
                pointWEnd = new PointW(e.X + 1, e.Y + 1);
                figureArray.Add(new Rect(new Pen(Color.Gray), pointWStart));
                figureArray.Last().AddPoint(pointWStart);
                paintBox.Invalidate();
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            int stepZoomValue = (int)stepZoom.Value;
            if (e.Button == MouseButtons.Left) {
                if (Math.Abs(pointEnd.X - pointStart.X + pointEnd.Y - pointStart.Y) > 10) {
                    decimal zoom = (decimal)(Math.Min((paintBox.Width - 200) / Math.Abs(pointWEnd.X - pointWStart.X),
                                            (paintBox.Height - 50) / Math.Abs(pointWEnd.Y - pointWStart.Y)) * 100);
                    if (Math.Min(zoom, numZoom.Maximum) > numZoom.Minimum)
                        numZoom.Value = Math.Min(zoom, numZoom.Maximum);
                    else
                        numZoom.Value = numZoom.Minimum;
                    PointW.offset = new Point((int)Math.Round(-Math.Min(pointWEnd.X, pointWStart.X) * (double)(numZoom.Value / 100) + 150),
                                              (int)Math.Round(-Math.Min(pointWEnd.Y, pointWStart.Y) * (double)(numZoom.Value / 100)) + 10);
                }
                else {
                    if ((numZoom.Value + stepZoom.Value) >= 1000)
                        stepZoomValue = 1000 - (int)numZoom.Value;
                    else
                        stepZoomValue = (int)stepZoom.Value;
                    numZoom.Value += stepZoomValue;
                    PointW.offset = new Point(
                        (int)Math.Round((PointW.offset.X - e.X) * PointW.zoom / (PointW.zoom - (stepZoomValue / 100.0))) + e.X,
                        (int)Math.Round((PointW.offset.Y - e.Y) * PointW.zoom / (PointW.zoom - (stepZoomValue / 100.0))) + e.Y);
                }
                figureArray.RemoveAt(figureArray.Count - 1);
                flagLeftMouseClick = false;
                paintBox.Invalidate();
            }
            if (e.Button == MouseButtons.Right && (pointEnd.X - pointStart.X + pointEnd.Y - pointStart.Y) < 10) {
                if (numZoom.Value <= stepZoom.Value)
                    stepZoomValue = (int)numZoom.Value - 1;
                else
                    stepZoomValue = (int)stepZoom.Value;
                numZoom.Value -= stepZoomValue;
                PointW.offset = new Point(
                      (int)Math.Round((PointW.offset.X - e.X) * PointW.zoom / (PointW.zoom + (stepZoomValue / 100.0))) + e.X,
                      (int)Math.Round((PointW.offset.Y - e.Y) * PointW.zoom / (PointW.zoom + (stepZoomValue / 100.0))) + e.Y);

                paintBox.Invalidate();
            }
        }
        private void CreatePanelProp() {
            panelProp = new PanelProp();
            panelProp.Text = "Zoom";
            stepZoom = new NumericUpDown();
            stepZoom.Location = new Point(65, 15);
            stepZoom.Size = new Size(48, 24);
            stepZoom.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            stepZoom.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            stepZoom.Value = new decimal(new int[] { 20, 0, 0, 0 });
            stepZoom.Increment = 10;
            panelProp.Controls.Add(stepZoom);
            labelStepZoom = new Label();
            labelStepZoom.Location = new Point(5, 20);
            labelStepZoom.Text = "Step:";
            panelProp.Controls.Add(labelStepZoom);
        }
        public override void HidePanelProp() {
            panelProp.Visible = false;
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
        }
    }
    class ToolHand : Tool {
        Point pntLastMause = new Point(0, 0);
        public ToolHand(Button button, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            cursor = Cursors.Hand;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                PointW.offset.X += e.X - pntLastMause.X;
                PointW.offset.Y += e.Y - pntLastMause.Y;
                paintBox.Invalidate();
                pntLastMause.X = e.X;
                pntLastMause.Y = e.Y;
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {

                flagLeftMouseClick = true;
                pntLastMause.X = e.X;
                pntLastMause.Y = e.Y;
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
            }
        }
    }
    class ToolMoveFigure : Tool {
        PointW pntLastMause = new PointW(0, 0);
        public ToolMoveFigure(Button button, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            cursor = Cursors.Hand;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
        }
        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                PointW clickW = PointW.ScrnToPointW(e.Location);
                if (figureSelectionArray != null) {
                    foreach (Figure primitiv in figureSelectionArray) {
                        primitiv.Move(new PointW(clickW.X - pntLastMause.X, clickW.Y - pntLastMause.Y));
                    }
                }
                paintBox.Invalidate();

                pntLastMause.X = clickW.X;
                pntLastMause.Y = clickW.Y;
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                PointW clickW = PointW.ScrnToPointW(e.Location);
                flagLeftMouseClick = true;
                pntLastMause.X = clickW.X;
                pntLastMause.Y = clickW.Y;
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                flagLeftMouseClick = false;
                UndoRedo.SaveState();
            }
        }
    }
    class ToolSelection : Tool {
        PointW pointWStart;
        PointW pointWEnd;
        Point pointStart;
        Point pointEnd;
        Anchor anchorSelected;
        Dictionary<string, Prop> intersectProps = new Dictionary<string, Prop>(5);
        bool flagDragPoint = false;
        public ToolSelection(Button button, ref List<Figure> figureArrayFrom, PaintBox paintBox_set) {
            paintBox = paintBox_set;
            figureArray = figureArrayFrom;
            toolButton = button;
            toolButton.Click += new EventHandler(ToolButtonClick);
            panelProp = new PanelProp();
            panelProp.Text = "Selection";
        }

        public override void MouseMove(object sender, MouseEventArgs e) {
            if (flagLeftMouseClick) {
                if (flagDragPoint) {
                    anchorSelected.EditPoint(new Point(e.X, e.Y));
                }
                else {
                    pointEnd = new Point(e.X, e.Y);
                    pointWEnd = new PointW(e.X, e.Y);
                    figureArray.Last().AddPoint(pointWEnd);
                }
                paintBox.Invalidate();
            }
        }
        public override void MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                if (figureSelectionArray != null) {
                    foreach (Figure primitiv in figureSelectionArray) {
                        foreach (Anchor anchor in primitiv.anchorArray) {
                            if (anchor.rect.Contains(e.Location)) {
                                anchorSelected = anchor;
                                flagDragPoint = true;
                            }
                        }
                    }
                }
                if (!flagDragPoint) {
                    pointStart = new Point(e.X, e.Y);
                    pointEnd = new Point(e.X + 1, e.Y + 1);
                    Pen pen = new Pen(Color.Gray);
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    pen.DashOffset = 15;
                    pen.Width = 2;
                    pointWStart = new PointW(e.X, e.Y);
                    pointWEnd = new PointW(e.X + 1, e.Y + 1);
                    figureArray.Add(new Rect(pen, pointWStart));
                    paintBox.Invalidate();
                }
                flagLeftMouseClick = true;
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                
                if (flagDragPoint) {
                    flagDragPoint = false;
                }
                else {
                    figureArray.RemoveAt(figureArray.Count - 1);
                    figureSelectionArray = new List<Figure>();
                    if (Math.Abs(pointEnd.X - pointStart.X + pointEnd.Y - pointStart.Y) < 10) {
                        var hasSelected = false;
                        for (int i = figureArray.Count - 1; i >= 0; --i) {
                            figureArray[i].isSelected = !hasSelected && figureArray[i].SelectPoint(e.Location);
                            hasSelected |= figureArray[i].isSelected;
                            if (figureArray[i].isSelected) {
                                figureSelectionArray.Add(figureArray[i]);
                            }
                        }

                    }
                    else {

                        foreach (Figure primitiv in figureArray) {
                            var a = pointWStart.ToScrPnt();
                            primitiv.isSelected = primitiv.SelectArea(
                                new Rectangle(
                                    Math.Min(a.X, pointWEnd.ToScrPnt().X), Math.Min(a.Y, pointWEnd.ToScrPnt().Y),
                                    Math.Abs(a.X - pointWEnd.ToScrPnt().X), Math.Abs(a.Y - pointWEnd.ToScrPnt().Y)));
                            if (primitiv.isSelected) {
                                figureSelectionArray.Add(primitiv);
                            }
                        }
                    }
                    if (figureSelectionArray.Count == 0) {
                        figureSelectionArray = null;
                        intersectProps = null;
                    }
                }
                paintBox.Invalidate();
                DrawPanel();
                flagLeftMouseClick = false;
                UndoRedo.SaveState();
            }

        }
        void DrawPanel() {
            int offset = 0;
            intersectProps = new Dictionary<string, Prop>(5);
            panelProp.Visible = false;
            panelProp = new PanelProp();
            panelProp.Visible = true;
            panelProp.Text = "Selection";
            if (figureSelectionArray != null) {
                foreach (Figure primitiv in figureSelectionArray)
                    foreach (var prop in primitiv.propArray)
                        if (figureSelectionArray[0].propArray.ContainsKey(prop.Key)) {
                            if (!intersectProps.ContainsKey(prop.Key)) {
                                intersectProps.Add(prop.Key, prop.Value);
                            }
                        }
                        else
                            intersectProps.Remove(prop.Key);
                foreach (var prop in intersectProps) {
                    prop.Value.Draw(new Point(5, 20 + offset), panelProp, null, paintBox);
                    prop.Value.changeAll = changeAll;
                    if (prop.Value.GetType() == typeof(PropFill))
                        offset += 50;
                    else
                        offset += 30;
                }
            }
        }
        void changeAll(object sender) {
            foreach (Figure primitiv in figureSelectionArray) {
                primitiv.propArray[sender.GetType().Name] = intersectProps[sender.GetType().Name].Clone();
            }
        }
        public override void HidePanelProp() {
            paintBox.Invalidate();
            panelProp.Visible = false;
            panelProp = new PanelProp();
            panelProp.Text = "Selection";
        }
        public override void ShowPanelProp() {
            panelProp.Visible = true;
            DrawPanel();
        }
    }
    public class NumWidthPen : NumericUpDown {
        public float penWidth;
        public NumWidthPen(decimal val) {
            Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            //      Value = new decimal(new int[] { 1, 0, 0, 0 });
            Value = val;
            ValueChanged += new EventHandler(NumWidthPen_ValueChanged);
            penWidth = (float)Value;
        }
        private void NumWidthPen_ValueChanged(object sender, EventArgs e) {
            penWidth = (float)Value;
        }
    }
    public class ColorButton : Button {
        public Color color;
        private ColorDialog colorDialog = new ColorDialog();
        public ColorButton(Color setColor) {
            this.Click += new EventHandler(ColorButton_Click);
            color = setColor;
            Size = new Size(48, 24);
            SetButtonColor(setColor);
        }
        private void ColorButton_Click(object sender, EventArgs e) {
            colorDialog.ShowDialog();
            color = colorDialog.Color;
            SetButtonColor(colorDialog.Color);
        }
        public void SetButtonColor(Color color) {
            Graphics bitmapGBtnMainColor;
            Bitmap bitmapBtnMainColor;
            bitmapBtnMainColor = new Bitmap(this.Width, this.Height);
            bitmapGBtnMainColor = Graphics.FromImage(bitmapBtnMainColor);
            bitmapGBtnMainColor.Clear(color);
            this.Image = bitmapBtnMainColor;
        }
    }
    public class PanelProp : GroupBox {
        public static Panel toolPanel;
        public PanelProp() {
            BackColor = SystemColors.ControlDark;
            Location = new Point(3, 322);
            Size = new Size(120, 180);
            TabIndex = 12;
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            toolPanel.Controls.Add(this);
            Visible = false;
        }

    }
    public class Prop {
        protected Control control;
        protected PaintBox paintBox;
        protected Label label;
        public delegate void ChangeAll(object sender);
        public ChangeAll changeAll = null;
        public virtual Prop Clone() { return new Prop(); }
        public virtual void Draw(Point position, PanelProp panelProp, String text = null, PaintBox paintBox = null) { }
        protected void ValChanged(object sender, EventArgs e) {
            if (paintBox != null)
                paintBox.Invalidate();
            if (changeAll != null) {
                changeAll(this);
            }
        }
    }
    public class PropColor : Prop {
        public ColorButton colorButton;
        public PropColor(Color col) {
            colorButton = new ColorButton(col);
        }
        public override void Draw(Point position, PanelProp panelProp, String text, PaintBox paintBox = null) {
            colorButton.SetButtonColor(colorButton.color);
            this.paintBox = paintBox;
            colorButton.Click += ValChanged;
            if (text == null)
                text = "Color:";
            colorButton.Location = new Point(position.X + 60, position.Y);
            panelProp.Controls.Add(colorButton);
            label = new Label {
                Location = position,
                Text = text
            };
            panelProp.Controls.Add(label);
        }
        public Color GetColor() {
            return colorButton.color;
        }
        public override Prop Clone() {
            return new PropColor(colorButton.color);
        }
    }
    public class PropPenWidth : Prop {
        public NumWidthPen numWidthPen;
        public PropPenWidth(int val = 1) {
            numWidthPen = new NumWidthPen(val);
        }
        public override void Draw(Point position, PanelProp panelProp, String text, PaintBox paintBox = null) {
            numWidthPen.Value = (decimal)numWidthPen.penWidth;
            this.paintBox = paintBox;
            numWidthPen.ValueChanged += ValChanged;
            if (text == null)
                text = "Width:";
            numWidthPen.Location = new Point(position.X + 60, position.Y);
            numWidthPen.Size = new Size(48, 26);
            panelProp.Controls.Add(numWidthPen);
            label = new Label {
                Location = position,
                Text = text
            };
            panelProp.Controls.Add(label);
        }
        public float GetPenWidth() {
            return ((NumWidthPen)numWidthPen).penWidth;
        }
        public override Prop Clone() {
            return new PropPenWidth((int)numWidthPen.Value);
        }
    }
    public class PropFill : Prop {
        public CheckBox checkBox;
        public PropColor propColor;
        public PropFill(Color col, bool chek = false) {
            propColor = new PropColor(col);
            checkBox = new CheckBox();
            checkBox.Checked = chek;
        }
        public override void Draw(Point position, PanelProp panelProp, String text, PaintBox paintBox = null) {
            this.paintBox = paintBox;
            if (text == null)
                text = "Fill:";
            checkBox.Click += ValChanged;
            propColor.colorButton.Click += ValChanged;
            checkBox.Location = new Point(position.X + 60, position.Y);
            checkBox.Size = new Size(48, 26);
            checkBox.Text = "";
            panelProp.Controls.Add(checkBox);
            label = new Label {
                Location = position,
                Text = text
            };
            panelProp.Controls.Add(label);
            propColor.Draw(new Point(position.X, position.Y + 25), panelProp, "Clr fill:");

        }
        public bool GetCheked() {
            return ((CheckBox)checkBox).Checked;
        }
        public Color color => propColor.GetColor();
        public Color GetColor() {
            return propColor.GetColor();
        }
        public override Prop Clone() {
            return new PropFill(propColor.colorButton.color, checkBox.Checked);
        }
    }
    public class PropRadius : Prop {
        public NumericUpDown numeric;

        public PropRadius(int radius) {
            numeric = new NumericUpDown();
            numeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numeric.Value = new decimal(radius);
        }
        public override void Draw(Point position, PanelProp panelProp, String text, PaintBox paintBox = null) {
            this.paintBox = paintBox;
            if (text == null)
                text = "Radius:";
            numeric.Location = new Point(position.X + 60, position.Y);
            numeric.Size = new Size(48, 26);
            numeric.ValueChanged += ValChanged;
            panelProp.Controls.Add(numeric);
            label = new Label {
                Location = position,
                Text = text
            };
            panelProp.Controls.Add(label);
        }
        public int GetRadius() {
            return (int)numeric.Value;
        }
        public override Prop Clone() {
            return new PropRadius((int)numeric.Value);
        }
    }

}