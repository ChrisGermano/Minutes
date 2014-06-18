using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MinutesToMidnight.Animating;

namespace MinutesToMidnight
{
    class Button
    {
        Vector2 position;
        
        int width;
        int height;
        int button_number;
        public string name;
        public bool active;
        public Action ButtonAction;
        private float scalar;
        Texture2D texture;
        bool disabled;

        public bool animating;
        AnimationManager animator;

        public Button(Vector2 _position, int _width, int _height, Action action, string nm, float _scalar = 1.0f, int buttonnumber = 0)
        {
            position = _position;
            width = _width;
            height = _height;
            ButtonAction = action;
            name = nm;
            disabled = false;
            button_number = buttonnumber;
            scalar = _scalar;
            active = true;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (animating == null || !animating)
            {
                float alpha;
                if (disabled || !active)
                {
                    alpha = 0.5f;
                }
                else
                {
                    alpha = 1f;
                }
                spritebatch.Draw(texture, position, null, Color.White * alpha, 0f, new Vector2(0, 0), scalar, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);
            }
            else
            {
                animator.Draw(spritebatch, position, DrawConstants.PDA_BUTTON_LAYER);
            }
        }

        public bool mouseOver(int x, int y)
        {
            if (disabled)
            {
                return false;
            }
            return (x >= position.X &&
               y >= position.Y &&
               x <= (position.X + width) &&
               y <= (position.Y + height));
        }

        internal void LoadContent(ContentManager contentManager)
        {
            switch (name)
            {
                case "close":
                    texture = Textures.pda_close_button;
                    break;
                case "timeline":
                    texture = contentManager.Load<Texture2D>("PDA//Notes//Notes_Active_Button");
                    break;
                case "map":
                    texture = contentManager.Load<Texture2D>("PDA//Map//Map_Active_Button");
                    break;
                case "bios":
                    texture = contentManager.Load<Texture2D>("PDA//Contacts//Contacts_Active");
                    break;
                case "start":
                    texture = contentManager.Load<Texture2D>("start_button");
                    break;
                default:
                    texture = Textures.pda_map_button;
                    break;
            }

            width = (int)(texture.Width * scalar);
            height = (int)(texture.Height * scalar);
            int separator = button_number > 0 ? 2 : 0;
            position = new Vector2(position.X + width * button_number + separator*button_number, position.Y);
        }
        public void Disable()
        {
            disabled = true;
        }

        public void Enable()
        {
            disabled = false;
        }

        internal void SetTexture(Texture2D texture2D)
        {
            texture = texture2D;
        }

        internal Vector2 getPosition()
        {
            return position;
        }


        internal void InitAnimate()
        {
            animator = new AnimationManager();
            animator.SetState(AnimationState.IDLE);
            animating = true;
        }

        internal void SetAnimation(AnimationState animationState, List<AnimationFrame> buttonframes)
        {
            if (animator != null)
            {
                animator.Set(animationState, new Animation(texture, buttonframes));
            }
        }
    }
}
