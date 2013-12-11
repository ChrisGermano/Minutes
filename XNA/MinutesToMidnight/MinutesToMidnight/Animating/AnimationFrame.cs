using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MinutesToMidnight.Animating
{
    public class AnimationFrame
    {
        public Rectangle Frame;
        public int FrameLength;

        public AnimationFrame(Rectangle frame, int framelength)
        {
            Frame = frame;
            FrameLength = framelength;
        }
    }
}
