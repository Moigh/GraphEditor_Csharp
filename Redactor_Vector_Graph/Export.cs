using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace Redactor_Vector_Graph {
    class Export {
        private static double zoomTemp = PointW.zoom;
        private static int offsetTempX = PointW.offset.X;
        private static int offsetTempY = PointW.offset.Y;
        public static void ToSvg(string fileName, int width, int height, List<Figure> figureArray) {
            string text = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"" + width + "\" height=\"" + height + "\">";
            Resize(width, height);
            foreach (var figure in figureArray)
                text += figure.ToSvgFormat();
            text += "</svg>";
            ResetSize();
            using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.WriteLine(text);
                }
            }
        }
        public static void ToBitmap(string fileName, int width, int height, List<Figure> figureArray) {

            Resize(width, height);
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            foreach (var figure in figureArray)
                figure.Draw(g);
            ResetSize();
            bitmap.Save(fileName);
        }
        private static void Resize(int width, int height) {
            const int border = 10;
            zoomTemp = PointW.zoom;
            offsetTempX = PointW.offset.X;
            offsetTempY = PointW.offset.Y;
            PointW.zoom = Math.Min((width - border * 2) / (Tool.pntwMaxReact.X - Tool.pntwMinReact.X), (height - border * 2) / (Tool.pntwMaxReact.Y - Tool.pntwMinReact.Y));
            PointW.offset = new Point((int)(Math.Round(-Tool.pntwMinReact.X * PointW.zoom) + border*PointW.zoom), (int)(Math.Round(-Tool.pntwMinReact.Y * PointW.zoom) + border * PointW.zoom));
        }
        private static void ResetSize() {
            PointW.zoom = zoomTemp;
            PointW.offset.X = offsetTempX;
            PointW.offset.Y = offsetTempY;
        }
    }
}
