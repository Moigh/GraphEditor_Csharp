using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Text;

namespace Redactor_Vector_Graph {
    public partial class MainDrawForm : Form {
        public const string Signature = "<SVP>";
        List<Figure> figureArray = new List<Figure>(20);
        ToolTip toolTipMain = new ToolTip();
        Pen penMain = new Pen(Color.Black);
        ToolPolyLine toolPolyLine;
        ToolLine toolLine;
        ToolRect toolRect;
        ToolEllipse toolCircle;
        ToolHand toolHand;
        ToolMoveFigure toolMoveFigure;
        ToolZoom toolZoom;
        ToolRoundedRect toolRoundedRect;
        ToolSelection toolSelection;
        bool isFirstSave = true;
        DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Figure[]));
        string openedFileName = " ";
        string OpenedFileName {
            get => openedFileName;
            set {
                Text = winName + " | " + value;
                openedFileName = value;
            }
        }
        bool isChanged;
        public bool IsChanged {
            get => isChanged;
            set {
                isChanged = value;
                if (OpenedFileName.Last() == '*')
                    OpenedFileName = OpenedFileName.Remove(OpenedFileName.Length - 1);
                if (value)
                    OpenedFileName += "*";
            }
        }
        string winName;
        public MainDrawForm() {
            InitializeComponent();
            PanelProp.toolPanel = toolPanel;
            toolTipMain.SetToolTip(btnToolPolyLine, "Pencil");
            toolTipMain.SetToolTip(btnToolLine, "Line");
            toolTipMain.SetToolTip(btnToolRect, "Rectangle");
            toolTipMain.SetToolTip(btnToolEllipse, "Ellipse");
            toolTipMain.SetToolTip(btnToolZoom, "Zoom");
            toolTipMain.SetToolTip(btnToolHand, "Hand");
            toolTipMain.SetToolTip(btnToolRoundedRect, "Rounded Rect");
            toolTipMain.SetToolTip(btnToolSelection, "Selection");
            toolTipMain.SetToolTip(btnToolMoveFigure, "Move Figure");

            toolPolyLine = new ToolPolyLine(btnToolPolyLine, ref figureArray, paintBox);
            toolLine = new ToolLine(btnToolLine, ref figureArray, paintBox);
            toolRect = new ToolRect(btnToolRect, ref figureArray, paintBox);
            toolRoundedRect = new ToolRoundedRect(btnToolRoundedRect, ref figureArray, paintBox);
            toolCircle = new ToolEllipse(btnToolEllipse, ref figureArray, paintBox);
            toolZoom = new ToolZoom(btnToolZoom, ref figureArray, paintBox, numZoom);
            toolHand = new ToolHand(btnToolHand, paintBox);
            toolSelection = new ToolSelection(btnToolSelection, ref figureArray, paintBox);
            toolMoveFigure = new ToolMoveFigure(btnToolMoveFigure, paintBox);
            Tool.ActiveTool = toolPolyLine;
            toolPolyLine.ToolButtonClick(null, null);
            paintBox.Paint += PaintBox_Paint;
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Super Vector Paint\\Projects");
            fileDialogSave.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Super Vector Paint\\Projects";
            fileDialogOpen.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Super Vector Paint\\Projects";
            fileDialogExport.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Super Vector Paint\\Export";
            UndoRedo.Init(btnUndo, btnRedo, paintBox, ref figureArray, this);
            winName = Text;
            OpenedFileName = " ";
        }
        private void PaintBox_Paint(object sender, PaintEventArgs e) {
            foreach (Figure primitiv in figureArray) {
                primitiv.Draw(e.Graphics);
            }
            foreach (Figure primitiv in figureArray) {
                primitiv.DrawColider(e.Graphics);
            }
        }

        private void PaintBox_MouseDown(object sender, MouseEventArgs e) =>
            Tool.ActiveTool.MouseDown(sender, e);

        private void PaintBox_MouseUp(object sender, MouseEventArgs e) =>
            Tool.ActiveTool.MouseUp(sender, e);

        private void PaintBox_MouseMove(object sender, MouseEventArgs e) =>
            Tool.ActiveTool.MouseMove(sender, e);

        private void ToolStripExit_Click(object sender, EventArgs e) =>
            Application.Exit();

        private void ToolStripAbout_Click(object sender, EventArgs e) =>
            MessageBox.Show("Vector graph \nVersion: Alpha v0.1 \nMade by kenny5660(Liamaev Mikhail)");

        private void Main_Draw_Form_KeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.Z)
                UndoRedo.Undo(null, null);
            if (e.Control && e.KeyCode == Keys.Y) 
                UndoRedo.Redo(null, null);
            if (e.Control && e.KeyCode == Keys.C)
                CopyPaste.Copy(Tool.figureSelectionArray);
            if (e.Control && e.KeyCode == Keys.V) {
                CopyPaste.Paste(ref figureArray);
                Tool.UpdateSelectionArray(figureArray);
                UndoRedo.SaveState();
            }
            if (e.Control && e.KeyCode == Keys.X) {
                CopyPaste.Copy(Tool.figureSelectionArray);
                toolStripDelSelected_Click(null, null);
            }
            paintBox.Invalidate();
        }
        private void numZoom_ValueChanged(object sender, EventArgs e) {
            PointW.zoom = (double)(numZoom.Value / 100);
            paintBox.Invalidate();
        }

        private void btnResetZoom_Click(object sender, EventArgs e) {
            if (Tool.pntwMaxReact.X > 0) {
                numZoom.Value = (decimal)(Math.Min((paintBox.Width - 200) / (Tool.pntwMaxReact.X - Tool.pntwMinReact.X), (paintBox.Height - 50) / (Tool.pntwMaxReact.Y - Tool.pntwMinReact.Y)) * 100);
                PointW.offset = new Point((int)Math.Round(-Tool.pntwMinReact.X * (double)(numZoom.Value / 100) + 150), (int)Math.Round(-Tool.pntwMinReact.Y * (double)(numZoom.Value / 100)) + 10);
            }
            paintBox.Invalidate();
        }

        private void toolStripDelSelected_Click(object sender, EventArgs e) {
            figureArray.RemoveAll(FindFigureIsSelected);
            Tool.figureSelectionArray = null;
            UndoRedo.SaveState();
            paintBox.Invalidate();
        }

        private void toolStripUpLayer_Click(object sender, EventArgs e) {
            List<Figure> figureArrayTemp = figureArray.FindAll(FindFigureIsSelected);
            figureArray.RemoveAll(FindFigureIsSelected);
            figureArray.AddRange(figureArrayTemp);
            UndoRedo.SaveState();
            paintBox.Invalidate();
        }
        private bool FindFigureIsSelected(Figure figure) {
            return figure.isSelected;
        }

        private void toolStripDownLayer_Click(object sender, EventArgs e) {
            List<Figure> figureArrayTemp = figureArray.FindAll(FindFigureIsSelected);
            figureArray.RemoveAll(FindFigureIsSelected);
            figureArray.InsertRange(0, figureArrayTemp);
            UndoRedo.SaveState();
            paintBox.Invalidate();
        }

        private void ToolStripSave_Click(object sender, EventArgs e) {
            if (!isFirstSave) {
                using (FileStream fs = new FileStream(fileDialogSave.FileName, FileMode.Create)) {
                    using (StreamWriter sw = new StreamWriter(fs)) {
                        sw.WriteLine(Signature + SerializerFigure.SerializeAllFigures(ref figureArray));
                    }
                }
                UndoRedo.Saved();
            }
            else
                toolStripSaveAs_Click(null, null);
        }

        private void ToolStripOpen_Click(object sender, EventArgs e) {
            fileDialogOpen.ShowDialog();
        }

        private void fileDialogOpen_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
            using (FileStream fs = new FileStream(fileDialogOpen.FileName, FileMode.Open)) {
                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8)) {
                    string str = reader.ReadLine();
                    figureArray.Clear();
                    for (int i = figureArray.Count - 1; i >= 0; i--) {
                        figureArray.Remove(figureArray[i]);
                    }
                    if (str.Substring(0, Signature.Length) == Signature) {
                        str = str.Remove(0, Signature.Length);
                    }
                    else {
                        MessageBox.Show("Error, file is corrupted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UndoRedo.Reset();
                        return;
                    }
                    try {
                        figureArray.AddRange(SerializerFigure.Parse(str));
                        fileDialogSave.FileName = fileDialogOpen.FileName;
                        
                        isFirstSave = false;
                        OpenedFileName = fileDialogOpen.FileName;
                      
                    }
                    catch {
                        MessageBox.Show("Error, file is corrupted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Tool.ResetReactUpdate(figureArray);
                }
            }
            UndoRedo.Reset();
            paintBox.Invalidate();
        }

        private void fileDialogSave_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
            isFirstSave = false;
            OpenedFileName = fileDialogSave.FileName;
            ToolStripSave_Click(null, null);
            UndoRedo.Reset();
        }

        private void toolStripSaveAs_Click(object sender, EventArgs e) {
            fileDialogSave.ShowDialog();
        }

        private void toolStripNew_Click(object sender, EventArgs e) {
            for (int i = figureArray.Count - 1; i >= 0; i--) {
                figureArray.Remove(figureArray[i]);
            }
            isFirstSave = true;
            UndoRedo.Reset();
            paintBox.Invalidate();
        }

        private void MainDrawForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (isChanged) {
                DialogResult result = MessageBox.Show("Save file \"" + OpenedFileName + "\"?", "Save?", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                    ToolStripSave_Click(null, null);
                if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void toolStripExit_Click_1(object sender, EventArgs e) {
            Application.Exit();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e) {
           DialogResult dialogResult = fileDialogExport.ShowDialog();
           
        }

        private void fileDialogExport_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
            ExportWizard exportWizard = new ExportWizard();
            switch (fileDialogExport.FilterIndex) {
                case 1: exportWizard.ShowDialog(); Export.ToSvg(fileDialogExport.FileName,(int)(exportWizard.numWidth.Value), (int)(exportWizard.numHeight.Value), figureArray); break;
                case 2:
                case 3:
                case 4: exportWizard.ShowDialog(); Export.ToBitmap(fileDialogExport.FileName, (int)(exportWizard.numWidth.Value), (int)(exportWizard.numHeight.Value), figureArray); break;
            }
            paintBox.Invalidate();
        }

    }
}