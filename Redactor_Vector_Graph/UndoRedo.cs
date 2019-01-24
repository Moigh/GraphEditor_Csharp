using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Redactor_Vector_Graph {
   static class UndoRedo {
        const int bufferSize = 300000;
        static private bool firstSaveState = false;
        static private MainDrawForm _form;
        static private Button _btnUndo;
        static private Button _btnRedo;
        static private PaintBox _paintBox;
        static private List<Figure> _figureArray;
        static private RingBuffer<String> ringBuffer = new RingBuffer<string>(bufferSize);
        public static void Init(Button btnUndo, Button btnRedo, PaintBox paintBox, ref List<Figure> figureArray, MainDrawForm form) {
            _btnUndo = btnUndo;
            _btnUndo.Click += Undo;
            _btnRedo = btnRedo;
            _btnRedo.Click += Redo;
            _paintBox = paintBox;
            _figureArray = figureArray;
            _form = form;
            ringBuffer.Push(SerializerFigure.SerializeAllFigures(ref _figureArray));
        }

        public static void Redo(object sender, EventArgs e) {
            if (!firstSaveState) return;
            _btnUndo.Enabled = true;
            _btnRedo.Enabled = ringBuffer.Up(); ;
            for (int i = _figureArray.Count - 1; i >= 0; i--) {
                _figureArray.Remove(_figureArray[i]);
            }
            _figureArray.AddRange(SerializerFigure.Parse(ringBuffer.Value));
            _form.IsChanged = !ringBuffer.IsSaved;
            _paintBox.Invalidate();

        }

        public static void Undo(object sender, EventArgs e) {
            if (!firstSaveState) return;
            _btnRedo.Enabled = true;
            _btnUndo.Enabled = ringBuffer.Down();

            for (int i = _figureArray.Count - 1; i >= 0; i--) {
                _figureArray.Remove(_figureArray[i]);
            }
            _figureArray.AddRange(SerializerFigure.Parse(ringBuffer.Value));
            _form.IsChanged = !ringBuffer.IsSaved;
            _paintBox.Invalidate();
        }
        public static void SaveState() {

            firstSaveState = true;
            ringBuffer.Push(SerializerFigure.SerializeAllFigures(ref _figureArray));
            _form.IsChanged = !ringBuffer.IsSaved;
            _btnRedo.Enabled = false;
            _btnUndo.Enabled = true;
        }
        public static void Reset() {
            firstSaveState = false;
            _btnRedo.Enabled = false;
            _btnUndo.Enabled = false;
            ringBuffer = new RingBuffer<string>(bufferSize);
            ringBuffer.Push(SerializerFigure.SerializeAllFigures(ref _figureArray));
            ringBuffer.IsSaved = true;
            _form.IsChanged = !ringBuffer.IsSaved;
        }
        public static void Saved() {
            ringBuffer.IsSaved = true;
            _form.IsChanged = !ringBuffer.IsSaved;
        }

    }
}