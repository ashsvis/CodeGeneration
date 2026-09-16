using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public partial class DrawPanel : UserControl
    {
        private float ZoomSensitivity { get; set; } = 0.1f; /// Controls how much zoom per zoom mouse scroll
        private float MaxZoom { get; set; } = 100f;
        private float MinZoom { get; set; } = 0.01f;

        public event EventHandler<DrawEventArgs>? OnDraw; 
        public event EventHandler<PanOrZoomEventArgs>? OnPanOrZoom;

        private double zoomScale = 1; /// Current Zoom value
        private PointF origin = new(0, 0); /// Origin is the left most point of a viewport

        private PointF LastMousePosition = new(-1, -1);
        private MouseWheelEvent? MouseWheelData { get; set; }
        private MouseMoveEvent? MouseMoveData { get; set; }
        private Matrix? transformation; /// Keeps track of zoom transformations

        public Matrix? Transformation => transformation;
        public Point Origin => Point.Ceiling(origin);
        public Double Zoom => zoomScale;

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
                    Zoom = zoomScale,
                    Origin = origin
                });
                return;
            }
        }

        /// <summary>
        /// Resets all transformations and zoom for this control
        /// </summary>
        public void Reset()
        {
            origin = new PointF(0, 0);
            zoomScale = 1;
            transformation = new Matrix();
            Invalidate();
        }

        public void RestoreWheelData(float m11, float m12, float m21, float m22, float dx, float dy, Point point, double zoom)
        {
            transformation = new Matrix(m11, m12, m21, m22, dx, dy);
            origin = point;
            zoomScale = zoom;
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

                var newZoom = zoom * zoomScale;
                if (MinZoom <= newZoom && newZoom <= MaxZoom)
                {
                    g.TranslateTransform(origin.X, origin.Y);
                    origin.X -= (float)(mouseX / (zoomScale * zoom) - mouseX / zoomScale);
                    origin.Y -= (float)(mouseY / (zoomScale * zoom) - mouseY / zoomScale);

                    g.ScaleTransform(zoom, zoom);
                    g.TranslateTransform(-origin.X, -origin.Y);

                    zoomScale *= zoom;
                }

                // Nullify to prevent unecessary calculations
                MouseWheelData = null;
            }
            else if (MouseMoveData != null)
            {
                var mouseX = MouseMoveData.Position.X;
                var mouseY = MouseMoveData.Position.Y;
                var dx = (float)((mouseX - LastMousePosition.X) / zoomScale);
                var dy = (float)((mouseY - LastMousePosition.Y) / zoomScale);
                g.TranslateTransform(dx, dy);
                origin.X -= dx;
                origin.Y -= dy;

                LastMousePosition = MouseMoveData.Position;
                MouseMoveData = null;
            }

            // Keep transofrmation record
            transformation = g.Transform;

            OnDraw?.Invoke(this, new DrawEventArgs()
            {
                Graphics = g,
                Zoom = zoomScale,
                ViewPort = origin
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
                    Zoom = zoomScale,
                    Origin = origin
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
                        Zoom = zoomScale,
                        Origin = origin
                    });
                }
                Invalidate();
            }
        }

    }
}
