using Microsoft.VisualBasic.Devices;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Threading;

namespace CodeGenerator
{
    public partial class DrawPanel : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float ZoomSensitivity { get; set; } = 0.1f; /// Controls how much zoom per zoom mouse scroll
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float MaxZoom { get; set; } = 100f;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float MinZoom { get; set; } = 0.01f;

        public event EventHandler<DrawEventArgs>? OnDraw; 
        public event EventHandler<PanOrZoomEventArgs>? OnPanOrZoom;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ZoomScale { get; set; } = 1; /// Current Zoom value
        public PointF Origin = new(0, 0); /// Origin is the left most point of a viewport

        private PointF LastMousePosition = new(-1, -1);
        private MouseWheelEvent? MouseWheelData { get; set; }
        private MouseMoveEvent? MouseMoveData { get; set; }
        private Matrix? transformation; /// Keeps track of zoom transformations

        public Matrix? Transformation => transformation;

        public DrawPanel()
        {
            InitializeComponent();
            MouseWheel += DrawPanel_MouseWheel;
            MouseDown += DrawPanel_MouseDown;
            MouseMove += DrawPanel_MouseMove;
            MouseDoubleClick += DrawPanel_MouseDoubleClick;
            Paint += DrawPanel_Paint;
            // Utilize double buffer to smooth out flickering when re-painting
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        private void DrawPanel_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && e.Clicks > 1)
            {
                Reset();
                OnPanOrZoom?.Invoke(this, new PanOrZoomEventArgs()
                {
                    Zoom = ZoomScale,
                    ViewPort = Origin
                });
                return;
            }
        }

        /// <summary>
        /// Resets all transformations and zoom for this control
        /// </summary>
        public void Reset()
        {
            Origin = new PointF(0, 0);
            ZoomScale = 1;
            transformation = new Matrix();
            Invalidate();
        }

        public void RestoreWheelData(float tk0, float tk1, float tk2, float tk3, float tk4, float tk5)
        {
            transformation = new Matrix(tk0, tk1, tk2, tk3, tk4, tk5);
            Origin = new PointF(tk4, tk5);
            ZoomScale = tk0;
            Invalidate();
        }

        private class MouseData
        {
            public Point Position { get; set; }
        }

        private class MouseMoveEvent : MouseData
        {

        }

        private class MouseWheelEvent : MouseData
        {
            public double Zoom { get; set; }
        }

        private void DrawPanel_Paint(object? sender, PaintEventArgs e)
        {
            // Get control size
            var width = this.Width;
            var height = this.Height;
            var g = e.Graphics;

            try
            {
                g.Transform = transformation ?? new Matrix();
            }
            catch { }

            // Handle mouse wheel events before drawing
            if (MouseWheelData != null)
            {
                var mouseX = MouseWheelData.Position.X;
                var mouseY = MouseWheelData.Position.Y;
                var zoom = (float)MouseWheelData.Zoom;

                var newZoom = zoom * ZoomScale;
                if (MinZoom <= newZoom && newZoom <= MaxZoom)
                {
                    g.TranslateTransform(Origin.X, Origin.Y);
                    Origin.X -= (float)(mouseX / (ZoomScale * zoom) - mouseX / ZoomScale);
                    Origin.Y -= (float)(mouseY / (ZoomScale * zoom) - mouseY / ZoomScale);

                    g.ScaleTransform(zoom, zoom);
                    g.TranslateTransform(-Origin.X, -Origin.Y);

                    ZoomScale *= zoom;
                }

                // Nullify to prevent unecessary calculations
                MouseWheelData = null;
            }
            else if (MouseMoveData != null)
            {
                var mouseX = MouseMoveData.Position.X;
                var mouseY = MouseMoveData.Position.Y;
                var dx = (float)((mouseX - LastMousePosition.X) / ZoomScale);
                var dy = (float)((mouseY - LastMousePosition.Y) / ZoomScale);
                g.TranslateTransform(dx, dy);
                Origin.X -= dx;
                Origin.Y -= dy;

                LastMousePosition = MouseMoveData.Position;
                MouseMoveData = null;
            }

            // Keep transofrmation record
            transformation = g.Transform;

            OnDraw?.Invoke(this, new DrawEventArgs()
            {
                Graphics = g,
                Zoom = ZoomScale,
                ViewPort = Origin
            });
        }

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            var mouseX = e.Location.X;
            var mouseY = e.Location.Y;
            LastMousePosition = new PointF(mouseX, mouseY);
        }

        private void DrawPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {

                if (LastMousePosition.X == -1 && LastMousePosition.Y == -1)
                {
                    LastMousePosition = new PointF(e.Location.X, e.Location.Y);
                }
                else if (e.Location.X == LastMousePosition.X && e.Location.Y == LastMousePosition.Y)
                {
                    return;
                }

                var mouseX = e.Location.X;
                var mouseY = e.Location.Y;

                this.MouseMoveData = new MouseMoveEvent()
                {
                    Position = new Point(mouseX, mouseY),
                };
                OnPanOrZoom?.Invoke(this, new PanOrZoomEventArgs()
                {
                    Zoom = ZoomScale,
                    ViewPort = Origin
                });
                Invalidate();
            }
        }

        private void DrawPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                if (ModifierKeys == Keys.None)
                {
                    var mouseX = e.Location.X;
                    var mouseY = e.Location.Y;

                    var wheel = e.Delta < 0 ? -1 : 1;
                    var zoom = Math.Exp(wheel * ZoomSensitivity);

                    this.MouseWheelData = new MouseWheelEvent()
                    {
                        Position = new Point(mouseX, mouseY),
                        Zoom = zoom,
                    };
                    OnPanOrZoom?.Invoke(this, new PanOrZoomEventArgs()
                    {
                        Zoom = ZoomScale,
                        ViewPort = Origin
                    });
                }
                Invalidate();
            }
        }

        public class DrawEventArgs : EventArgs
        {
            public Graphics? Graphics { get; set; }
            public double Zoom { get; set; }
            public PointF ViewPort { get; set; }
        }

        public class PanOrZoomEventArgs : EventArgs
        {
            public double Zoom { get; set; }
            public PointF ViewPort { get; set; }
        }

    }
}
