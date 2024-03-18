using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public class ContinuousProgressBar : Element
{
    private float _value = 0.5f;
    private float _minValue = 0;
    private float _maxValue = 1;

/// <summary>
/// Current value of the bar
/// </summary>
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_value < _minValue)
            {
                throw new ArgumentException("Value cannot be less than min value");
            }
            if (_value > _maxValue)
            {
                throw new ArgumentException("Value cannot be greater than max value");
            }
        }
    }
    /// <summary>
    /// Minimum value of the bar
    /// </summary>
    public float MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;
            if (_minValue > _maxValue)
            {
                throw new ArgumentException("Min value cannot be greater than max value");
            }
            if (_value < _minValue)
            {
                throw new ArgumentException("Min value cannot be greater than value");
            }
        }
    }
    /// <summary>
    /// Maximum value of the bar
    /// </summary>
    public float MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            if (_maxValue < _minValue)
            {
                throw new ArgumentException("Max value cannot be less than min value");
            }
            if (_value > _maxValue)
            {
                throw new ArgumentException("Max value cannot be less than value");
            }
        }
    }

    /// <summary>
    /// Progress of the bar from 0 to 1
    /// </summary>
    public float Progress {
        get => (Value - MinValue) / (MaxValue - MinValue);
        set => Value = MinValue + (MaxValue - MinValue) * value;
    }

    public Color ProgressColor { get; set; }
    public Image ProgressImage { get; set; }

    public ContinuousProgressBar()
    {
        BackgroundColor = new(171, 125, 101);
        Background = AssetManager.Instance.GetImage("ui/blank");

        ProgressColor = new (54, 120, 33);
        ProgressImage = AssetManager.Instance.GetImage("ui/blank");
    }

    protected override void DrawBackground(SpriteBatch spriteBatch)
    {
        Rectangle progressRect =
            new(
                Transform.Bounds.X,
                Transform.Bounds.Y,
                (int)(Transform.Bounds.Width * Progress),
                Transform.Bounds.Height
            );

        Background.Draw(spriteBatch, Transform.Bounds, BackgroundColor, LayerDepth);
        ProgressImage.Draw(spriteBatch, progressRect, ProgressColor, LayerDepth + 0.01f);
    }
}
