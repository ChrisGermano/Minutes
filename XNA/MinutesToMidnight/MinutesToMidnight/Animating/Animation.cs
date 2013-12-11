using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MinutesToMidnight.Animating
{
    public class Animation
    {
        List<AnimationFrame> Frames;
        Texture2D Spritesheet;
        bool OneOff;
        bool finished;
        bool interruptable;
        int CurrentFrame;
        int frame_ticker;

        public Animation(Texture2D sheet, List<AnimationFrame> frames, bool one_off = false, bool _interruptable = true)
        {
            Spritesheet = sheet;
            Frames = frames;
            CurrentFrame = 0;
            frame_ticker = 0;
            OneOff = one_off;
            finished = true;
            interruptable = _interruptable;
        }

        public void UpdateFrame()
        {
            AnimationFrame curr = Frames[CurrentFrame];

            if (frame_ticker >= curr.FrameLength)
            {
                if (CurrentFrame + 1 >= Frames.Count)
                {
                    if (OneOff)
                    {
                        frame_ticker = 0;
                        return;
                    }
                    finished = true;
                    CurrentFrame = 0;
                }
                else
                {
                    finished = false;
                    CurrentFrame++;
                }

                frame_ticker = 0;
            }
            frame_ticker++;
        }

        public void DrawAnimation(SpriteBatch spritebatch, Vector2 position, float depth, bool flip, float scale, Color color)
        {
            SpriteEffects eff = SpriteEffects.None;
            if (flip)
            {
                eff = SpriteEffects.FlipHorizontally;
            }
            spritebatch.Draw(Spritesheet, position, Frames[CurrentFrame].Frame, color, 0f, new Vector2(0, 0), scale, eff, depth);
        }

        internal bool IsFinished()
        {
            if (interruptable)
            {
                return true;
            }
            return finished;
        }
    }
}
