
namespace PluginSupport
{
    public struct KeepEnvironment
    {
        public KeepEnvironment()
        {
            Matrix = new float[6];
            Matrix[0] = 1f;
            Matrix[3] = 1f;
            Origin = Point.Empty;
            Zoom = 1.0;
        }

        public KeepEnvironment(float m11, float m12, float m21, float m22, float dx, float dy, Point point, double zoom)
        {
            Matrix = new float[6];
            Matrix[0] = m11;
            Matrix[1] = m12;
            Matrix[2] = m21;
            Matrix[3] = m22;
            Matrix[4] = dx;
            Matrix[5] = dy;
            Origin = point;
            Zoom = zoom;
        }

        public float[] Matrix { get; set; }
        public Point Origin { get; set; }
        public double Zoom { get; set; }

        public void SetValues(KeepEnvironment value)
        {
            Matrix = value.Matrix;
            Origin = value.Origin;
            Zoom = value.Zoom;
        }
    }

}
