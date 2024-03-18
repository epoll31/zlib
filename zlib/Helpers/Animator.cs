using System.Collections.Generic;
using System.Linq;

namespace zlib;

public interface IAnimatable { }

public struct AnimationFrame
{
    public string Name { get; set; }

    /// <summary>
    /// Duration in milliseconds
    /// </summary>
    public float Duration { get; set; }
}

public class Animator : Timer
{
    private AnimationFrame[] frames;
    private int[] frameOrder;

    private int currentFrameIndex;
    public int CurrentFrameIndex
    {
        get => currentFrameIndex;
        set
        {
            while (value < 0)
            {
                value += frameOrder.Length;
            }
            currentFrameIndex = value % frameOrder.Length;
            currentFrame = frames[frameOrder[currentFrameIndex]];
            currentFrameName = currentFrame.Name;
        }
    }
    private string currentFrameName;
    public string CurrentFrameName
    {
        get => currentFrameName;
        set
        {
            for (int i = 0; i < frameOrder.Length; i++)
            {
                if (frames[frameOrder[i]].Name == value)
                {
                    currentFrameIndex = i;
                    currentFrameName = value;
                    currentFrame = frames[frameOrder[i]];

                    Duration = frames[frameOrder[i]].Duration;
                    Reset();
                    return;
                }
            }
            throw new KeyNotFoundException($"Frame with name {value} not found");
        }
    }
    private AnimationFrame currentFrame;
    public AnimationFrame CurrentFrame
    {
        get => currentFrame;
        set { CurrentFrameName = value.Name; }
    }

    public Animator(AnimationFrame[] frames)
        : this(frames, Enumerable.Range(0, frames.Length).ToArray()) { }

    public Animator(AnimationFrame[] frames, int[] frameOrder)
    {
        this.frames = frames;
        this.frameOrder = frameOrder;
        currentFrameIndex = 0;
    }

    public Animator(AnimationFrame[] frames, string[] frameOrder)
        : this(
            frames,
            frameOrder
                .Select(f => frames.ToList().IndexOf(frames.First(frame => frame.Name == f)))
                .ToArray()
        ) { }
    // {
    //     this.frameOrder = new int[frameOrder.Length];
    //     for (int i = 0; i < frameOrder.Length; i++)
    //     {
    //         for (int j = 0; j < this.frames.Length; j++)
    //         {
    //             if (this.frames[j].Name == frameOrder[i])
    //             {
    //                 this.frameOrder[i] = j;
    //                 break;
    //             }
    //         }
    //     }
    // }
}
