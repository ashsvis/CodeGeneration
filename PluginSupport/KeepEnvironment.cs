

namespace PluginSupport
{
    public struct KeepEnvironment
    {
        public KeepEnvironment()
        {
            Matrix = new float[6];
            Matrix[0] = 1f;
            Matrix[3] = 1f;
        }

        public KeepEnvironment(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            Matrix = new float[6];
            Matrix[0] = m11;
            Matrix[1] = m12;
            Matrix[2] = m21;
            Matrix[3] = m22;
            Matrix[4] = dx;
            Matrix[5] = dy;
        }

        public float[] Matrix { get; set; }

        public void Reset()
        {
            Matrix = new float[6];
            Matrix[0] = 1f;
            Matrix[3] = 1f;
        }

        public void SetValues(KeepEnvironment value)
        {
            Matrix = value.Matrix;
        }
    }

}
