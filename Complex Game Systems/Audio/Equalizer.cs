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

        private int SAMPLE_SIZE = 1024;
        private float[][] spectrum;
        private float[] spectrumTotal;
        private float[] oldSpectrumTotal;
        private float[] currentBarValue;

        private float maxValue;

        public Equalizer(Channel channel)
        {
            spectrumTotal = new float[SAMPLE_SIZE];
            oldSpectrumTotal = new float[SAMPLE_SIZE];
            currentBarValue = new float[SAMPLE_SIZE];

            LinkChannel(channel);
        }

        public void LinkChannel(Channel channel)
        {
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

            for (var i = 0; i < spectrumTotal.Length && i < SAMPLE_SIZE; ++i)
            {
                try
                {
                    oldSpectrumTotal[i] = spectrumTotal[i];
                    spectrumTotal[i] = (spectrum[0][i] + spectrum[1][i]) / 2f;
                }
                catch (Exception)
                {

                }

            }
        }

        public void Draw()
        {
            var maxValue = 0f;
            foreach (var value in spectrumTotal)
            {
                if (value > maxValue)
                    maxValue = value;
            }

            for (var i = 0; i < SAMPLE_SIZE / 3f; ++i)
            {
                var theta = (float)i / (SAMPLE_SIZE / 3f) * (2 * MathHelper.Pi);

                var start =
                    new Vector2(
                        GameWindow.main.Width / 2f + (float)Math.Cos(theta) * 350f,
                        GameWindow.main.Height / 2f + (float)Math.Sin(theta) * 350f);

                var barValue = 100f * (spectrumTotal[i] / maxValue);

                if (spectrumTotal[i] < 0.0075f)
                    barValue = 0f;

                if (barValue > currentBarValue[i])
                {
                    if (spectrumTotal[i] - oldSpectrumTotal[i] > 0.0025f && oldSpectrumTotal[i] < 0.005f)
                        barValue *= 2f;
                    currentBarValue[i] = barValue;
                }
                else
                    currentBarValue[i] /= 1.1f;

                if (currentBarValue[i] < 0f)
                    currentBarValue[i] = 0f;

                Gizmos.DrawRectangle(
                    start,
                    new Vector2(5f + start.X, start.Y + currentBarValue[i]),
                    Color4.LightBlue,
                    Color4.Blue, false, Matrix4.CreateRotationZ(-theta + MathHelper.Pi / 2f));
            }
        }
    }
}
