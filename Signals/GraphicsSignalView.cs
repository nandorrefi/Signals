using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Signals
{

    public partial class GraphicsSignalView : ViewBase
    {
        IReadOnlyList<SignalValue> signals;
        int prevSignalCount = 0;

        float scale = 1.0f;
        const float pixelPerSec = 120f;
        const float pixelPerValue = 3.0f;
        const float dotSizeVal = 3.0f;

        public GraphicsSignalView(SignalDocument document)
        {
            InitializeComponent();
            InitTimer();
            this.document = document;
            signals = document.Signals;
            prevSignalCount = signals.Count;
        }

        private void InitTimer()
        {
            drawingTimer.Interval = 1000;
            drawingTimer.Start();
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            scale *= 1.2f;
            GraphPanel.Invalidate();
        }

        private void MinusButton_Click(object sender, EventArgs e)
        {
            scale /= 1.2f;
            GraphPanel.Invalidate();
        }

        private void DrawingTimer_Tick(object sender, EventArgs e)
        {
            bool isLive = ((SignalDocument)document).DataIsLive;
            int signalCount = ((SignalDocument)document).Signals.Count;

            if ( isLive && prevSignalCount != signalCount)
            {
                GraphPanel.Invalidate();
            }
        }

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            signals = ((SignalDocument)document).Signals;

            InitAutoScroll(e);
            DrawTheAxes(e);
            DrawTheSignals(e);
            
            base.OnPaint(e);
        }

        private void InitAutoScroll(PaintEventArgs e)
        {
            int canvasWidth = GetCanvasWidth();

            GraphPanel.AutoScrollMinSize = new Size(canvasWidth, this.Height);

            Point ScrollPosition = GraphPanel.AutoScrollPosition;
            Matrix transform = new Matrix(1, 0, 0, 1, ScrollPosition.X, ScrollPosition.Y);
            e.Graphics.Transform = transform;
        }

        private void DrawTheAxes(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Green, 2);
            pen.DashStyle = DashStyle.Dot;
            pen.EndCap = LineCap.ArrowAnchor;

            e.Graphics.DrawLine(pen, 1.0f, ClientSize.Height, 1.0f, 0.0f);
            e.Graphics.DrawLine(pen, 0.0f, ClientSize.Height / 2, GetCanvasWidth(), ClientSize.Height / 2);
        }

        private int GetCanvasWidth()
        {
            DateTime newest = signals.Last<SignalValue>().TimeStamp;
            float t = GetTimeElapsedInSeconds(signals[0].TimeStamp, newest);
            return (int)Math.Ceiling(t * pixelPerSec * scale);
        }

        private void DrawTheSignals(PaintEventArgs e)
        {
            Color dataColor = Color.Blue;

            Pen signalPen = new Pen(dataColor, 2);
            SolidBrush dotBrush = new SolidBrush(dataColor);

            SizeF dotSize = new SizeF(dotSizeVal, dotSizeVal);
            SizeF halfDotSize = new SizeF(dotSizeVal / 2, dotSizeVal / 2);

            PointF lastSignalPosition = new PointF();
            for (int i = 0; i < signals.Count(); ++i)
            {
                SignalValue currentSignal = signals[i];
                
                float t = GetTimeElapsedInSeconds(signals[0].TimeStamp, currentSignal.TimeStamp);
                float x = t * pixelPerSec * scale;
                float y = ClientSize.Height / 2 - (float)currentSignal.Value * pixelPerValue * scale;

                PointF currentPosition = new PointF(x, y);
                RectangleF dot = new RectangleF(currentPosition - halfDotSize, dotSize);

                e.Graphics.FillRectangle(dotBrush, dot);

                if (i > 0)
                    e.Graphics.DrawLine(signalPen, lastSignalPosition, currentPosition);

                lastSignalPosition = currentPosition;

            }
        }

        private float GetTimeElapsedInSeconds(DateTime from, DateTime to)
        {
            long elapsedTicks = to.Ticks - from.Ticks;
            TimeSpan span = new TimeSpan(elapsedTicks);
            return (float)span.TotalSeconds;
        }

        
    }
}
