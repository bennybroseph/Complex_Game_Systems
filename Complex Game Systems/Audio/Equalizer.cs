namespace ComplexGameSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    using FMOD;

    using Geometry.Shapes;

    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

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

        private float radiusValue;

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
                catch (Exception e)
                {
                    Debug.Log("Error: " + e.Message);
                    break;
                }

            }
        }

        public void Draw()
        {
            DrawCircleBar();
        }

        private void DrawCircleBar()
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

                var tempRadius = spectrumTotal[0] * 200f;
                if (tempRadius > radiusValue)
                    radiusValue = tempRadius;
                else
                    radiusValue -= 0.01f;

                var start =
                    new Vector2(
                        GameWindow.main.Width / 2f + (float)Math.Cos(theta) * (100f + radiusValue),
                        GameWindow.main.Height / 2f + (float)Math.Sin(theta) * (100f + radiusValue));

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
                    currentBarValue[i] -= 1.5f;

                if (currentBarValue[i] < 0f)
                    currentBarValue[i] = 0f;

                var matrix = Matrix4.CreateRotationZ(-theta + MathHelper.Pi / 2f);
                matrix.M41 = start.X;
                matrix.M42 = -start.Y;

                var endColor =
                    new Color4(
                        Color4.White.R + currentBarValue[i] / 100f
                        * (Color4.BlueViolet.R - Color4.White.R),
                        Color4.White.G + currentBarValue[i] / 100f
                        * (Color4.BlueViolet.G - Color4.White.G),
                        Color4.White.B + currentBarValue[i] / 100f
                        * (Color4.BlueViolet.B - Color4.White.B),
                        1f);

                Gizmos.DrawRectangle(
                    new Vector2(-2.5f, 0f),
                    new Vector2(2.5f, currentBarValue[i]),
                    Color4.White,
                    endColor, false, matrix);
            }
        }

        private void DrawCircleSmooth()
        {
            var maxValue = 0f;
            foreach (var value in spectrumTotal)
            {
                if (value > maxValue)
                    maxValue = value;
            }

            var vertextes = new List<Vector2>();
            var colors = new List<Color4>();
            for (var i = 0; i < SAMPLE_SIZE / 3f; ++i)
            {
                var theta = (float)i / (SAMPLE_SIZE / 3f) * (2 * MathHelper.Pi);

                var tempRadius = spectrumTotal[0] * 200f;
                if (tempRadius > radiusValue)
                    radiusValue = tempRadius;
                else
                    radiusValue -= 0.01f;

                var start =
                    new Vector2(
                        GameWindow.main.Width / 2f + (float)Math.Cos(theta) * (100f + radiusValue),
                        GameWindow.main.Height / 2f + (float)Math.Sin(theta) * (100f + radiusValue));

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
                    currentBarValue[i] -= 1.5f;

                if (currentBarValue[i] < 0f)
                    currentBarValue[i] = 0f;

                var endColor =
                    new Color4(
                        Color4.White.R + currentBarValue[i] / 100f
                        * (Color4.BlueViolet.R - Color4.White.R),
                        Color4.White.G + currentBarValue[i] / 100f
                        * (Color4.BlueViolet.G - Color4.White.G),
                        Color4.White.B + currentBarValue[i] / 100f
                        * (Color4.BlueViolet.B - Color4.White.B),
                        1f);

                colors.Add(Color4.White);
                vertextes.Add(new Vector2(start.X, start.Y));

                colors.Add(endColor);
                vertextes.Add(
                    new Vector2(
                        start.X + currentBarValue[i] * (float)Math.Cos(theta),
                        start.Y + currentBarValue[i] * (float)Math.Sin(theta)));
            }
            Gizmos.DrawCustomShape(vertextes, colors, PrimitiveType.TriangleStrip);
        }
    }
}
