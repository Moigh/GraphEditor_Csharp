using System.Windows.Forms;

namespace Redactor_Vector_Graph {
    public class PaintBox : Panel {
        public PaintBox() {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
    }
}
