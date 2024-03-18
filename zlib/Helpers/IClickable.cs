using Microsoft.Xna.Framework;

namespace zlib;

public interface IClickable
{
    public bool ContainsScreenPoint(Vector2 position);
    public float LayerDepthAtScreenPoint(Vector2 position);
    public bool Interactable { get; }

    virtual void OnClickStart() { }
    virtual void OnClick() { }
    virtual void OnClickEnd() { }
    virtual void OnClickCanceled() { }
    virtual void OnHoverStart() { }
    virtual void OnHover() { }
    virtual void OnHoverEnd() { }
    virtual void OnClickStartMissAll() { }
    virtual void OnClickMissAll() { }
    virtual void OnClickEndMissAll() { }

    public void RegisterClickable()
    {
        InputManager.Instance.RegisterClickable(this);
    }
    public void UnregisterClickable()
    {
        InputManager.Instance.UnregisterClickable(this);
    }
}
