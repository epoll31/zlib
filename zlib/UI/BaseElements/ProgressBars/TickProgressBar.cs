using System;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public class TickProgressBar : Element
{
    private int _value = 3;
    private int _minValue = 0;
    private int _maxValue = 6;

    /// <summary>
    /// Current value of the bar
    /// </summary>
    public int Value
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
    public int MinValue
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
    public int MaxValue
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
    public float Progress
    {
        get => (Value - MinValue) / (float)(MaxValue - MinValue);
        set => Value = (int)(MinValue + (MaxValue - MinValue) * value);
    }

    public NineSlice Tick { get; set; }
    public int AssetScale { get; set; } = 3;
    public int TickMargin { get; set; } = 3;
    public int TickSpacing { get; set; } = 3;

    public TickProgressBar()
    {
        Background = AssetManager.Instance.GetImage<NineSlice>("ui/progress-bar/bg");
        BackgroundColor = Color.White;

        // BorderColor = Color.Pink;
        BorderWidth = 1;

        // Background = AssetManager.Instance.GetImage("ui/blank");
        // BackgroundColor = new(171, 125, 101);

        Tick = AssetManager.Instance.GetImage<NineSlice>("ui/progress-bar/tick");
    }

    protected override void DrawBackground(SpriteBatch spriteBatch)
    {
        NineSlice bg = Background as NineSlice;
        int ticks = MaxValue - MinValue;

        int tickWidth =
            (Transform.Bounds.Width - (TickMargin + TickSpacing) * 2 - TickSpacing * (ticks - 1))
            / ticks;
        int tickHeight = Transform.Bounds.Height - (TickMargin + TickSpacing) * 2;

        int offsetWidth =
            Transform.Bounds.Width
            - ((tickWidth + TickSpacing) * ticks + (TickMargin) * 2 + TickSpacing);
        int offsetHeight = Transform.Bounds.Height - (tickHeight + (TickMargin + TickSpacing) * 2);

        Rectangle bounds =
            new(
                Transform.Bounds.X + offsetWidth / 2,
                Transform.Bounds.Y + offsetHeight / 2,
                Transform.Bounds.Width - offsetWidth,
                Transform.Bounds.Height - offsetHeight
            );

        bg.Draw(spriteBatch, bounds, BackgroundColor, LayerDepth, AssetScale);

        for (int i = 0; i < Value; i++)
        {
            Rectangle tickRect =
                new(
                    Transform.Bounds.X
                        + TickMargin
                        + TickSpacing
                        + i * (tickWidth + TickSpacing)
                        + offsetWidth / 2,
                    Transform.Bounds.Y + TickMargin + TickSpacing + offsetHeight / 2,
                    tickWidth,
                    tickHeight
                );
            Tick.Draw(spriteBatch, tickRect, Color.White, LayerDepth + 0.01f, AssetScale);
        }
    }
}
