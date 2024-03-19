using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public enum Position
{
    Relative,
    Absolute
}

public abstract class Element : IClickable
{
    public Element Parent { get; set; }
    public List<Element> Children { get; private set; }
    public string ID { get; set; }
    public float LayerDepth => SortingLayers.Get(this);

    public bool Visible { get; set; } = true;

    private bool _interactable = true;
    public bool Interactable
    {
        get
        {
            if (Parent != null)
            {
                return _interactable && Parent.Interactable;
            }
            return _interactable;
        }
        set => _interactable = value;
    }

    private Transform _transform;
    public Transform Transform
    {
        get { return _transform; }
        set
        {
            _transform = value;
            _transform.Host = this;
        }
    }

    protected NineSlice Border { get; set; }
    public Color BorderColor { get; set; } = Color.Transparent;
    public int BorderWidth { get; set; } = 0;
    public Image Background { get; set; }
    public Color BackgroundColor { get; set; } = Color.Transparent;

    public Element Root
    {
        get
        {
            Element root = this;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            return root;
        }
    }

    // Rectangle IClickable.Bounds => throw new NotImplementedException();

    protected Element(params Element[] children)
    {
        Transform = new Transform() { Host = this };

        Children = children.ToList();
        foreach (var child in Children)
        {
            child.Parent = this;
        }

        Background = AssetManager.Instance.GetImage<Image>("blank");
        Border = AssetManager.Instance.GetImage<NineSlice>("border");

        (this as IClickable).RegisterClickable();
    }

    /// <summary>
    /// Get the element with the specified ID from the element tree.
    /// </summary>
    /// <param name="id">ID to search for</param>
    /// <returns></returns>
    public Element GetElementByID(string id)
    {
        if (ID == id)
            return this;

        foreach (var child in Children)
        {
            var element = child.GetElementByID(id);
            if (element != null)
                return element;
        }

        return null;
    }

    public T GetElementByID<T>(string id)
        where T : Element
    {
        Element e = GetElementByID(id);
        if (e is T t)
            return t;
        throw new ArgumentException($"Element with ID {id} is not of type {typeof(T)}");
    }

    public void AddChild(Element child)
    {
        child.Parent = this;
        // (child as IClickable).RegisterClickable();
        Children.Add(child);
    }

    public void RemoveChild(Element child)
    {
        child.Parent = null;
        // (child as IClickable).UnregisterClickable();
        Children.Remove(child);
    }

    public void RemoveChild(int index)
    {
        Children[index].Parent = null;
        // (Children[index] as IClickable).UnregisterClickable();
        Children.RemoveAt(index);
    }

    public void ClearChildren()
    {
        foreach (var child in Children)
        {
            child.Parent = null;
            // (child as IClickable).UnregisterClickable();
        }
        Children.Clear();
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var child in Children)
        {
            child.Update(gameTime);
        }
    }

    protected virtual void DrawBorder(SpriteBatch spriteBatch)
    {
        Border?.Draw(spriteBatch, Transform.Bounds, BorderColor, LayerDepth + 0.01f, BorderWidth);
    }

    protected virtual void DrawBackground(SpriteBatch spriteBatch)
    {
        Background?.Draw(spriteBatch, Transform.Bounds, BackgroundColor, LayerDepth);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!Visible)
            return;

        DrawBackground(spriteBatch);
        DrawBorder(spriteBatch);

        foreach (var child in Children)
        {
            child.Draw(spriteBatch);
        }
    }

    public bool ContainsScreenPoint(Vector2 position)
    {
        return Transform.Bounds.Contains(position);
    }

    public float LayerDepthAtScreenPoint(Vector2 position)
    {
        return LayerDepth;
    }
}
