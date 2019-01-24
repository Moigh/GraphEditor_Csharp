namespace Redactor_Vector_Graph
{
    partial class MainDrawForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDrawForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.fileDialogOpen = new System.Windows.Forms.OpenFileDialog();
            this.fileDialogSave = new System.Windows.Forms.SaveFileDialog();
            this.fileDialogExport = new System.Windows.Forms.SaveFileDialog();
            this.paintBox = new Redactor_Vector_Graph.PaintBox();
            this.btnToolPolyLine = new System.Windows.Forms.Button();
            this.btnToolLine = new System.Windows.Forms.Button();
            this.paintBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 604);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1085, 0);
            this.panel2.TabIndex = 4;
            // 
            // fileDialogOpen
            // 
            this.fileDialogOpen.Filter = "Project File(.svp)|*.svp";
            this.fileDialogOpen.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialogOpen_FileOk);
            // 
            // fileDialogSave
            // 
            this.fileDialogSave.DefaultExt = "json";
            this.fileDialogSave.FileName = "new.svp";
            this.fileDialogSave.Filter = "Project File(.svp)|*.svp";
            this.fileDialogSave.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialogSave_FileOk);
            // 
            // fileDialogExport
            // 
            this.fileDialogExport.Filter = "Vector(*.svg)|*.svg|Raster graphics(*.png)|*.png|Raster graphics(*.bmp)|*.bmp|Ras" +
    "ter graphics(*.jpg)|*.jpg";
            this.fileDialogExport.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialogExport_FileOk);
            // 
            // paintBox
            // 
            this.paintBox.AutoSize = true;
            this.paintBox.BackColor = System.Drawing.SystemColors.Window;
            this.paintBox.Controls.Add(this.btnToolPolyLine);
            this.paintBox.Controls.Add(this.btnToolLine);
            this.paintBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paintBox.Location = new System.Drawing.Point(0, 0);
            this.paintBox.MaximumSize = new System.Drawing.Size(2000, 2000);
            this.paintBox.MinimumSize = new System.Drawing.Size(960, 555);
            this.paintBox.Name = "paintBox";
            this.paintBox.Size = new System.Drawing.Size(1085, 604);
            this.paintBox.TabIndex = 0;
            this.paintBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PaintBox_MouseDown);
            this.paintBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PaintBox_MouseMove);
            this.paintBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PaintBox_MouseUp);
            // 
            // btnToolPolyLine
            // 
            this.btnToolPolyLine.Location = new System.Drawing.Point(12, 12);
            this.btnToolPolyLine.Name = "btnToolPolyLine";
            this.btnToolPolyLine.Size = new System.Drawing.Size(54, 54);
            this.btnToolPolyLine.TabIndex = 0;
            this.btnToolPolyLine.UseVisualStyleBackColor = true;
            // 
            // btnToolLine
            // 
            this.btnToolLine.Location = new System.Drawing.Point(82, 12);
            this.btnToolLine.Name = "btnToolLine";
            this.btnToolLine.Size = new System.Drawing.Size(54, 54);
            this.btnToolLine.TabIndex = 4;
            this.btnToolLine.UseVisualStyleBackColor = true;
            // 
            // MainDrawForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1085, 604);
            this.Controls.Add(this.paintBox);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainDrawForm";
            this.Text = "Super Vector Paint";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainDrawForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_Draw_Form_KeyDown);
            this.paintBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PaintBox paintBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.OpenFileDialog fileDialogOpen;
        private System.Windows.Forms.SaveFileDialog fileDialogSave;
        private System.Windows.Forms.SaveFileDialog fileDialogExport;
        private System.Windows.Forms.Button btnToolPolyLine;
        private System.Windows.Forms.Button btnToolLine;
    }
}

