namespace ComplexGameSystems
{
    using System;
    using System.Runtime.InteropServices;

    using FMOD;

    using Geometry.Shapes;

    using OpenTK;
    using OpenTK.Graphics;

    using Debug = Utility.Debug;

    public class Equalizer
    {
        private Channel m_Channel;
        private DSP m_DSP;

        private int SAMPLE_SIZE = 64;
        private float[][] spectrum;
        private float[] spectrumTotal;

        public Equalizer(Channel channel)
        {
            spectrum = new float[2][];
            spectrum[0] = new float[SAMPLE_SIZE];
            spectrum[1] = new float[SAMPLE_SIZE];
            spectrumTotal = new float[SAMPLE_SIZE];

            m_Channel = channel;

            Audio.CreateDSP(DSP_TYPE.FFT, out m_DSP);
            m_DSP.setParameterInt((int)DSP_FFT.WINDOWSIZE, SAMPLE_SIZE);
            m_Channel.addDSP(0, m_DSP);
        }

        public void Update()
        {
            IntPtr data;
            uint length;

            m_DSP.getParameterData((int)DSP_FFT.SPECTRUMDATA, out data, out length);

            var spectrumBuffer = (DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(DSP_PARAMETER_FFT));

            spectrum = spectrumBuffer.spectrum;
        }

        public void Draw()
        {
            for (var i = 0; i < SAMPLE_SIZE; ++i)
            {
                try
                {
                    Gizmos.DrawRectangle(
                        new Vector2(i * 20f, 0f),
                        new Vector2(10f + i * 20f, spectrum[0][i] * 1000f),
                        Color4.LightBlue,
                        Color4.Blue);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

                //spectrumTotal[i] = (spectrum[0][i] + spectrum[1][i]) / 2f;
            }
        }
    }
}
