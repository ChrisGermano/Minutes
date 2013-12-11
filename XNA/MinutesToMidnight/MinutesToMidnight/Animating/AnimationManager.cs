using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MinutesToMidnight.Animating
{
    public class AnimationManager
    {
        Dictionary<AnimationState, Animation> animations;
        private AnimationState active;
        private AnimationState next;

        public AnimationManager()
        {
            animations = new Dictionary<AnimationState, Animation>();
            active = AnimationState.IDLE;
            next = AnimationState.NONE;
            //instance = this;
        }

        public void Set(AnimationState state, Animation anim)
        {
            animations.Add(state, anim);
        }

        public void Draw(SpriteBatch spritebatch, Vector2 posn, float depth, bool flip = false, float scale = 1f, Color? color = null)
        {
            if (next != AnimationState.NONE && animations[active].IsFinished())
            {
                active = next;
                next = AnimationState.NONE;
            }
            Color c = color.HasValue ? color.Value : Color.White;    
            animations[active].DrawAnimation(spritebatch, posn, depth, flip, scale, c);
            animations[active].UpdateFrame();
        }

        internal void SetState(AnimationState animationState)
        {
            if (animations.ContainsKey(active) && animations[active].IsFinished())
            {
                active = animationState;
            }
            else
            {
                next = animationState;
            }
        }
    }
}
