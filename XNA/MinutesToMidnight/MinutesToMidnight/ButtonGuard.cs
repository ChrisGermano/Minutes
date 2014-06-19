using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinutesToMidnight.Animating;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    public class ButtonGuard
    {

        public AnimationManager animator;
        public Texture2D sheet;

        public String name;
        public Vector2 position;
        public Boolean inspected;
        public int width;
        public int height;

        //0 = prompt, 1 = yes, 2 = no
        public TextOverlay[] overlays = new TextOverlay[3];

        public ButtonGuard(Vector2 pos)
        {
            name = "Button Guard";
            position = pos;
            width = 50;
            height = 200;

            overlays[0] = new TextOverlay("Sir, have you decided what we should do about the Mannage?", new Vector2(10, Game1.screen_size.Y - 110));
            overlays[1] = new TextOverlay(" - I'm ready.", new Vector2(40, Game1.screen_size.Y - 90));
            overlays[2] = new TextOverlay(" - I'm not ready.", new Vector2(40, Game1.screen_size.Y - 70));
        }

        public Boolean CheckMouseOver(Vector2 mPos)
        {
            return (mPos.X >= position.X &&
                mPos.Y >= position.Y &&
                mPos.X <= (position.X + width) &&
                mPos.Y <= (position.Y + height));
        }

        public MouseType GetMouseType(Vector2 mPos)
        {
            if (CheckMouseOver(mPos))
            {
                return MouseType.BUTTON_GUARD;
            }

            if (GetOverOverlay(mPos) != null)
            {
                return MouseType.BUTTON_GUARD_OVERLAY;
            }

            return MouseType.BACKGROUND;
        }

        public TextOverlay GetOverOverlay(Vector2 mPos)
        {
            TextOverlay t = null;
            foreach (TextOverlay teOv in overlays)
            {
                if (teOv.isMouseOver(mPos))
                {
                    if (overlays[0] != teOv)
                    {
                        teOv.drawCol = Color.Black;
                    }
                    t = teOv;
                }
                else
                {
                    if (overlays[0] != teOv)
                    {
                        teOv.drawCol = Color.Gray;
                    }
                }
            }
            return t;
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            //To be replaced with animation
            animator.Draw(sb, position, DrawConstants.PERSON_LAYER);
            if (inspected)
            {
                sb.Draw(Textures.text_background, new Vector2(0, Game1.screen_size.Y - Textures.text_background.Height - 20), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.CONVERSATION_BACKGROUND_LAYER);

                foreach (TextOverlay t in overlays)
                {
                    t.Draw(sb);
                }
            }
        }


        internal void LoadContent(ContentManager contentManager)
        {
            animator = new AnimationManager();
            List<AnimationFrame> frames = new List<AnimationFrame>();
            int idleframes = 55;
            frames.Add(new AnimationFrame(new Rectangle(52, 0, 50, 210), idleframes));
            frames.Add(new AnimationFrame(new Rectangle(156, 0, 48, 210), idleframes));

            animator.Set(AnimationState.IDLE, new Animation(Textures.default_person, frames));
            animator.SetState(AnimationState.IDLE);
        }
    }
}
