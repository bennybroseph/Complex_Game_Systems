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

    private const int SAMPLE_SIZE = 512;

    private const float INNER_MIN_RADIUS = 200f;
    private const float INNER_MAX_RADIUS = 50;

    private const float OUTER_MIN_RADIUS = 0f;
    private const float OUTER_MAX_RADIUS = 100f;

    private const float SPECTRUM_MIN = 0.0075f;

    private const float RADIUS_DECAY = 75f;
    private const float SPECTRUM_DECAY = 5f;

    private float m_Frequency;

    private float[][] m_Spectrum;
    private float[] m_SpectrumTotal;
    private float[] m_OldSpectrumTotal;
    private float[] m_CurrentBarValue;

    private float m_MaxValue;
    private float m_RadiusValue;

    private bool m_IsBeat;
    private float m_BeatVolume;

    private float m_BeatThresholdVolume = 0.15f;
    private float m_BeatMinContribution = 0.6f;
    private float m_BeatPostIgnore = 250f;

    private float m_FrequencyThreshold = 150f;
    private int m_BeatMaxIndex;

    private int m_LastBeat;

    public Equalizer(Channel channel)
    {
        m_SpectrumTotal = new float[SAMPLE_SIZE];
        m_OldSpectrumTotal = new float[SAMPLE_SIZE];
        m_CurrentBarValue = new float[SAMPLE_SIZE];

        LinkChannel(channel);
    }

    public void LinkChannel(Channel channel)
    {
        m_Channel = channel;

        Audio.CreateDSP(DSP_TYPE.FFT, out m_DSP);
        m_DSP.setParameterInt((int)DSP_FFT.WINDOWSIZE, SAMPLE_SIZE);
        m_Channel.addDSP(0, m_DSP);

        m_Channel.getFrequency(out m_Frequency);
        m_BeatMaxIndex = (int)Math.Ceiling(m_FrequencyThreshold / (m_Frequency / SAMPLE_SIZE));
    }

    public void Update()
    {
        IntPtr data;
        uint length;

        m_DSP.getParameterData((int)DSP_FFT.SPECTRUMDATA, out data, out length);

        var spectrumBuffer = (DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(DSP_PARAMETER_FFT));
        m_Spectrum = spectrumBuffer.spectrum;

        for (var i = 0; m_Spectrum.Length != 0 && i < SAMPLE_SIZE; ++i)
        {
            m_OldSpectrumTotal[i] = m_SpectrumTotal[i];
            m_SpectrumTotal[i] = (m_Spectrum[0][i] + m_Spectrum[1][i]) / 2f;
        }

        CalculateMaxValue();
        m_IsBeat = CalculateBeat();
        CalculateRadius();
    }

    public void Draw()
    {
        DrawCircleBar();
    }

    private void CalculateMaxValue()
    {
        foreach (var value in m_SpectrumTotal)
            if (value > m_MaxValue)
                m_MaxValue = value;
    }
    private bool CalculateBeat()
    {
        var beatAverage = 0f;
        for (var i = 0; i < m_BeatMaxIndex; ++i)
            beatAverage += m_SpectrumTotal[i] - m_OldSpectrumTotal[i];
        beatAverage /= m_BeatMaxIndex;

        if ((beatAverage / m_MaxValue >= m_BeatMinContribution || beatAverage >= m_BeatThresholdVolume) &&
            m_LastBeat == 0)
        {
            m_BeatVolume = beatAverage;
            m_LastBeat = (int)Time.time;

            return true;
        }

        if (Time.time - m_LastBeat >= m_BeatPostIgnore)
            m_LastBeat = 0;

        return false;
    }

    private void DrawCircleBar()
    {
        var screenOffset = new Vector2(MyGameWindow.main.Width / 2f, MyGameWindow.main.Height / 2f);
        for (var i = 0; i < SAMPLE_SIZE / 3f; ++i)
        {
            var theta = i / (SAMPLE_SIZE / 3f) * (2 * MathHelper.Pi);

            var start =
                CalculatePoint(i / (SAMPLE_SIZE / 3f), INNER_MIN_RADIUS + m_RadiusValue, screenOffset);

            var barValue = OUTER_MAX_RADIUS * (m_SpectrumTotal[i] / m_MaxValue);

            if (m_SpectrumTotal[i] < SPECTRUM_MIN)
                barValue = OUTER_MIN_RADIUS;

            if (barValue >= m_CurrentBarValue[i])
            {
                if (m_SpectrumTotal[i] - m_OldSpectrumTotal[i] > 0.0025f && m_OldSpectrumTotal[i] < 0.005f)
                    barValue *= 2f;
                m_CurrentBarValue[i] = barValue;
            }
            else
                m_CurrentBarValue[i] /= 1 + SPECTRUM_DECAY * Time.deltaTime;

            if (m_CurrentBarValue[i] < OUTER_MIN_RADIUS)
                m_CurrentBarValue[i] = OUTER_MIN_RADIUS;

            var matrix = Matrix4.CreateRotationZ(-theta + MathHelper.PiOver2);
            matrix.M41 = start.X;
            matrix.M42 = -start.Y;

            var endColor =
                new Color4(
                    Color4.White.R + m_CurrentBarValue[i] / 100f
                    * (Color4.BlueViolet.R - Color4.White.R),
                    Color4.White.G + m_CurrentBarValue[i] / 100f
                    * (Color4.BlueViolet.G - Color4.White.G),
                    Color4.White.B + m_CurrentBarValue[i] / 100f
                    * (Color4.BlueViolet.B - Color4.White.B),
                    1f);

            Gizmos.DrawRectangle(
                new Vector2(-2.5f, 0f),
                new Vector2(2.5f, m_CurrentBarValue[i]),
                Color4.White,
                endColor, false, matrix);
        }
    }

    private void DrawCircleSmooth()
    {
        var anchors = new List<Vector2>();

        var screenOffset = new Vector2(MyGameWindow.main.Width / 2f, MyGameWindow.main.Height / 2f);
        for (int i = 0; i < SAMPLE_SIZE / 3f; ++i)
        {
            var startPercent = i / (SAMPLE_SIZE / 3f);

            var start = CalculatePoint(startPercent, INNER_MIN_RADIUS + m_RadiusValue, screenOffset);

            var barValue = OUTER_MAX_RADIUS * (m_SpectrumTotal[i] / m_MaxValue);

            if (m_SpectrumTotal[i] <= SPECTRUM_MIN)
                barValue = OUTER_MIN_RADIUS;

            if (barValue >= m_CurrentBarValue[i])
            {
                if (m_SpectrumTotal[i] - m_OldSpectrumTotal[i] > 0.0025f && m_OldSpectrumTotal[i] < 0.005f)
                    barValue *= 2f;
                m_CurrentBarValue[i] = barValue;
            }
            else
                m_CurrentBarValue[i] -= SPECTRUM_DECAY * Time.deltaTime;

            if (m_CurrentBarValue[i] < OUTER_MIN_RADIUS)
                m_CurrentBarValue[i] = OUTER_MIN_RADIUS;

            var endPercent = (i + 0.5f) / (SAMPLE_SIZE / 3f);
            var end = CalculatePoint(
                endPercent, INNER_MIN_RADIUS + m_RadiusValue + m_CurrentBarValue[i], screenOffset);
            anchors.Add(end);
        }

        var vertexes = new List<Vector2>();
        var colors = new List<Color4>();
        for (var i = 0; i < anchors.Count; ++i)
        {
            var startAnchor = i - 1 >= 0 ? anchors[i - 1] : anchors.Last();

            var startBarValue = i - 1 >= 0 ? m_CurrentBarValue[i - 1] : m_CurrentBarValue.Last();
            var controlPoint =
                startBarValue > m_CurrentBarValue[i] ?
                CalculatePoint((float)i / anchors.Count,
                    INNER_MIN_RADIUS + m_RadiusValue + startBarValue, screenOffset) :
                CalculatePoint((float)i / anchors.Count,
                    INNER_MIN_RADIUS + m_RadiusValue + m_CurrentBarValue[i], screenOffset);

            var bezier = new BezierCurveQuadric(startAnchor, anchors[i], controlPoint);

            var endColor =
                new Color4(
                    Color4.White.R + m_CurrentBarValue[i] / 100f
                    * (Color4.BlueViolet.R - Color4.White.R),
                    Color4.White.G + m_CurrentBarValue[i] / 100f
                    * (Color4.BlueViolet.G - Color4.White.G),
                    Color4.White.B + m_CurrentBarValue[i] / 100f
                    * (Color4.BlueViolet.B - Color4.White.B),
                    1f);
            for (var j = 0f; j < 1f; j += 0.005f)
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

    private void CalculateRadius()
    {
        var tempRadius = m_BeatVolume * INNER_MAX_RADIUS;
        if (m_IsBeat && tempRadius > m_RadiusValue)
            m_RadiusValue = tempRadius;
        else
            m_RadiusValue -= RADIUS_DECAY * Time.deltaTime;

        if (m_RadiusValue < 0f)
            m_RadiusValue = 0f;
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