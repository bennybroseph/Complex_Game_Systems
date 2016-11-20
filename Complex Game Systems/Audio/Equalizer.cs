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

    private const int SAMPLE_SIZE = 128;

    private const float INNER_MIN_RADIUS = 200f;
    private const float INNER_MAX_RADIUS = 50;

    private const float OUTER_MIN_RADIUS = 0f;
    private const float OUTER_MAX_RADIUS = 75f;

    private const float SPECTRUM_MIN = 0.0075f;

    private const float RADIUS_DECAY = 0.5f;
    private const float SPECTRUM_DECAY = 1.5f;

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

        for (var i = 0; spectrum.Length != 0 && i < SAMPLE_SIZE; ++i)
        {
            oldSpectrumTotal[i] = spectrumTotal[i];
            spectrumTotal[i] = (spectrum[0][i] + spectrum[1][i]) / 2f;
        }
    }

    public void Draw()
    {
        DrawCircleSmooth();
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
            var theta = i / (SAMPLE_SIZE / 3f) * (2 * MathHelper.Pi);

            var tempRadius = spectrumTotal[0] * 200f;
            if (tempRadius > radiusValue)
                radiusValue = tempRadius;
            else
                radiusValue -= 0.01f;

            var start =
                new Vector2(
                    MyGameWindow.main.Width / 2f + (float)Math.Cos(theta) * (100f + radiusValue),
                    MyGameWindow.main.Height / 2f + (float)Math.Sin(theta) * (100f + radiusValue));

            var barValue = 100f * (spectrumTotal[i] / maxValue);

            if (spectrumTotal[i] < 0.0075f)
                barValue = 0f;

            if (barValue >= currentBarValue[i])
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

        var anchors = new List<Vector2>();

        var tempRadius = spectrumTotal[0] * INNER_MAX_RADIUS;
        if (tempRadius > radiusValue)
            radiusValue = tempRadius;
        else
            radiusValue -= RADIUS_DECAY;

        if (radiusValue < 0f)
            radiusValue = 0f;

        var screenOffset = new Vector2(MyGameWindow.main.Width / 2f, MyGameWindow.main.Height / 2f);
        for (int i = 0; i < SAMPLE_SIZE / 3f; ++i)
        {
            var startPercent = i / (SAMPLE_SIZE / 3f);

            var start = CalculatePoint(startPercent, INNER_MIN_RADIUS + radiusValue, screenOffset);

            var barValue = OUTER_MAX_RADIUS * (spectrumTotal[i] / maxValue);

            if (spectrumTotal[i] <= SPECTRUM_MIN)
                barValue = OUTER_MIN_RADIUS;

            if (barValue >= currentBarValue[i])
            {
                if (spectrumTotal[i] - oldSpectrumTotal[i] > 0.0025f && oldSpectrumTotal[i] < 0.005f)
                    barValue *= 2f;
                currentBarValue[i] = barValue;
            }
            else
                currentBarValue[i] -= SPECTRUM_DECAY;

            if (currentBarValue[i] < OUTER_MIN_RADIUS)
                currentBarValue[i] = OUTER_MIN_RADIUS;

            var endPercent = (i + 0.5f) / (SAMPLE_SIZE / 3f);
            var end = CalculatePoint(
                endPercent, INNER_MIN_RADIUS + radiusValue + currentBarValue[i], screenOffset);
            anchors.Add(end);
        }

        var vertexes = new List<Vector2>();
        var colors = new List<Color4>();
        for (var i = 0; i < anchors.Count; ++i)
        {
            var startAnchor = i - 1 >= 0 ? anchors[i - 1] : anchors.Last();

            var startBarValue = i - 1 >= 0 ? currentBarValue[i - 1] : currentBarValue.Last();
            var controlPoint =
                startBarValue > currentBarValue[i] ?
                CalculatePoint((float)i / anchors.Count,
                    INNER_MIN_RADIUS + radiusValue + startBarValue, screenOffset) :
                CalculatePoint((float)i / anchors.Count,
                    INNER_MIN_RADIUS + radiusValue + currentBarValue[i], screenOffset);

            var bezier = new BezierCurveQuadric(startAnchor, anchors[i], controlPoint);

            var endColor =
                new Color4(
                    Color4.White.R + currentBarValue[i] / 100f
                    * (Color4.BlueViolet.R - Color4.White.R),
                    Color4.White.G + currentBarValue[i] / 100f
                    * (Color4.BlueViolet.G - Color4.White.G),
                    Color4.White.B + currentBarValue[i] / 100f
                    * (Color4.BlueViolet.B - Color4.White.B),
                    1f);
            for (var j = 0f; j < 1f; j += 0.05f)
            {
                colors.Add(endColor);
                vertexes.Add(bezier.CalculatePoint(j));
                colors.Add(Color4.White);
                vertexes.Add(screenOffset);
            }
        }
        colors.Add(colors.First());
        vertexes.Add(vertexes.First());

        Gizmos.DrawCustomShape(vertexes, colors, PrimitiveType.TriangleStrip);
    }

    private static Vector2 CalculatePoint(float percent, float radius, Vector2 offset = new Vector2())
    {
        var theta = percent * MathHelper.TwoPi;
        return
            new Vector2(
                offset.X + (float)Math.Cos(theta) * radius,
                offset.Y + (float)Math.Sin(theta) * radius);
    }
}