using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace zlib;

public sealed class InputManager
{
    private static readonly Lazy<InputManager> lazy = new(() => new InputManager());
    public static InputManager Instance
    {
        get { return lazy.Value; }
    }

    private InputManager() { }

    private List<IClickable> clickables;
    private List<IClickable> click;
    private List<IClickable> hover;

    public MouseHandler Mouse { get; private set; }

    public event EventHandler OnClickStartMiss;
    public event EventHandler OnClickMiss;
    public event EventHandler OnClickEndMiss;

    public void Initialize()
    {
        Mouse = new MouseHandler();
        clickables = new List<IClickable>();
        click = new List<IClickable>();
        hover = new List<IClickable>();
    }

    public void RegisterClickable(IClickable clickable)
    {
        clickables.Add(clickable);
    }

    public void UnregisterClickable(IClickable clickable)
    {
        clickables.Remove(clickable);
    }

    private IEnumerable<IClickable> GetHovered()
    {
        return clickables.Where(c => c.Interactable && c.ContainsScreenPoint(Mouse.ScreenPosition));
    }

    private IEnumerable<IClickable> GetClicked()
    {
        return clickables.Where(c => c.Interactable && c.ContainsScreenPoint(Mouse.ScreenPosition));
    }

    public void Update(GameTime gameTime)
    {
        Mouse.Update();

        IEnumerable<IClickable> hovered = GetHovered();

        IEnumerable<IClickable> hoverLeft = hover.Except(hovered);
        IEnumerable<IClickable> hoverEntered = hovered.Except(hover);
        IEnumerable<IClickable> justHover = hover.Except(hoverLeft).Except(hoverEntered);

        hover = hovered.ToList();

        foreach (IClickable clickable in hoverLeft)
        {
            if (click.Contains(clickable))
            {
                clickable.OnClickCanceled();
                click.Remove(clickable);
            }

            clickable.OnHoverEnd();
        }
        foreach (IClickable clickable in justHover)
        {
            clickable.OnHover();
        }
        foreach (IClickable clickable in hoverEntered)
        {
            clickable.OnHoverStart();
        }

        if (Mouse.LeftButtonClickStart)
        {
            IEnumerable<IClickable> clicked = GetClicked();

            if (clicked.Any())
            {
                float maxLayerDepth = clicked.Max(c =>
                    c.LayerDepthAtScreenPoint(Mouse.ScreenPosition)
                );

                clicked = clicked.Where(c =>
                    c.LayerDepthAtScreenPoint(Mouse.ScreenPosition) == maxLayerDepth
                );

                foreach (IClickable clickable in clicked)
                {
                    clickable.OnClickStart();
                    click.Add(clickable);
                }
            }
            else
            {
                OnClickStartMiss?.Invoke(this, EventArgs.Empty);
                clickables.ForEach(c => c.OnClickStartMissAll());
            }
        }
        else if (Mouse.LeftButtonClickEnd)
        {
            if (click.Any())
            {
                foreach (IClickable clickable in click)
                {
                    if (clickable.ContainsScreenPoint(Mouse.ScreenPosition))
                    {
                        clickable.OnClickEnd();
                    }
                    else
                    {
                        throw new Exception("This should not happen");
                        // clickable.OnClickCanceled();
                    }
                }
                click.Clear();
            }
            else
            {
                OnClickEndMiss?.Invoke(this, EventArgs.Empty);
                clickables.ForEach(c => c.OnClickEndMissAll());
            }
        }
        else if (Mouse.LeftButtonDown)
        {
            if (click.Any())
            {
                foreach (IClickable clickable in click)
                {
                    clickable.OnClick();
                }
            }
            else
            {
                OnClickMiss?.Invoke(this, EventArgs.Empty);
                clickables.ForEach(c => c.OnClickMissAll());
            }
        }
    }
}
