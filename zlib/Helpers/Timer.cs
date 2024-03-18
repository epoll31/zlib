using Microsoft.Xna.Framework;

namespace zlib
{
    public class Timer
    {
        private float time;
        public float Duration { get; set; }
        private bool on;

        public Timer()
        {
            on = false;
        }

        public Timer(float duration, bool on = true)
        {
            this.Duration = duration;
            this.on = on;
        }

        public void Stop()
        {
            on = false;
        }

        public void Start()
        {
            on = true;
        }

        public void Reset()
        {
            time = 0;
        }

        public bool Tick(GameTime gameTime)
        {
            if (!on)
                return false;

            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time >= Duration)
            {
                time = 0;
                return true;
            }
            return false;
        }
    }
}
