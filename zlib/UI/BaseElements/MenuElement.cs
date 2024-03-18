using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public enum Corner
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

public class MenuElement : Element
{
    private ButtonElement closeButton;

    private int _closeButtonCornerDistance = 24;
    public int CloseButtonCornerDistance
    {
        get { return _closeButtonCornerDistance; }
        set
        {
            _closeButtonCornerDistance = value;
            if (CloseButtonCorner != null)
            {
                Corner corner = CloseButtonCorner.Value;
                CloseButtonCorner = null;
                CloseButtonCorner = corner;
            }
        }
    }

    private int _closeButtonSize = 24;
    public int CloseButtonSize
    {
        get { return _closeButtonSize; }
        set
        {
            _closeButtonSize = value;
            closeButton.Transform.Width = _closeButtonSize;
            closeButton.Transform.Height = _closeButtonSize;
        }
    }

    private Corner? _closeButtonLocation;
    public Corner? CloseButtonCorner
    {
        get { return _closeButtonLocation; }
        set
        {
            if (_closeButtonLocation == value)
            {
                return;
            }
            _closeButtonLocation = value;
            if (_closeButtonLocation != null)
            {
                switch (_closeButtonLocation)
                {
                    case Corner.TopLeft:
                        closeButton.Transform.Top = CloseButtonCornerDistance;
                        closeButton.Transform.Left = CloseButtonCornerDistance;
                        closeButton.Transform.Right = null;
                        closeButton.Transform.Bottom = null;
                        break;
                    case Corner.TopRight:
                        closeButton.Transform.Top = CloseButtonCornerDistance;
                        closeButton.Transform.Right = CloseButtonCornerDistance;
                        closeButton.Transform.Left = null;
                        closeButton.Transform.Bottom = null;
                        break;
                    case Corner.BottomLeft:
                        closeButton.Transform.Bottom = CloseButtonCornerDistance;
                        closeButton.Transform.Left = CloseButtonCornerDistance;
                        closeButton.Transform.Right = null;
                        closeButton.Transform.Top = null;
                        break;
                    case Corner.BottomRight:
                        closeButton.Transform.Bottom = CloseButtonCornerDistance;
                        closeButton.Transform.Right = CloseButtonCornerDistance;
                        closeButton.Transform.Left = null;
                        closeButton.Transform.Top = null;
                        break;
                }
                AddChild(closeButton);
            }
            else
            {
                RemoveChild(closeButton);
            }
        }
    }

    public event EventHandler OnCloseRequested;

    public MenuElement(params Element[] children)
        : base(children)
    {
        Background = AssetManager.Instance.GetImage<NineSlice>("ui/menu-large");
        BackgroundColor = Color.White;

        closeButton = new ButtonElement(AssetManager.Instance.GetImage<Image>("ui/x"))
        {
            BackgroundColor = Color.White,
            ClickColor = Color.Gray,
            HoverColor = Color.LightGray,
        };
        CloseButtonCorner = Corner.TopRight;
        CloseButtonCornerDistance = 24;
        CloseButtonSize = 24;

        closeButton.OnClickedEvent += (sender, args) => Close();
    }

    public void Close()
    {
        OnCloseRequested?.Invoke(this, new EventArgs());
    }

    protected override void DrawBackground(SpriteBatch spriteBatch)
    {
        (Background as NineSlice).Draw(
            spriteBatch,
            Transform.Bounds,
            BackgroundColor,
            LayerDepth,
            2
        );
    }
}
