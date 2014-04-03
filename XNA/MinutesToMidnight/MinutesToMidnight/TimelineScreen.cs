using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    class TimelineScreen : PDAScreen
    {
        Vector2 position;
        Vector2 scroll_bar_posn;
        public List<TextOverlay> knowledge;
        Texture2D separator;
        Texture2D scroll_bar;
        int height;
        int width;
        int absolute_total;
        int starting_index;
        float scroll_scale;
        Button scroll_up;
        Button scroll_down;
        private float scalar;
        public TimelineScreen(Vector2 pos, int hght, int wdth, float scale)
        {
            position = pos;
            knowledge = new List<TextOverlay>();
            height = hght;
            width = wdth;
            scalar = scale;

            starting_index = 0;
            absolute_total = 0;
            scroll_up = new Button(new Vector2(pos.X + wdth - 10, pos.Y), 20, 20, ScrollUp, "scroll up");
            scroll_bar_posn = scroll_up.getPosition();
            scroll_bar_posn.Y = scroll_bar_posn.Y + 10;
            scroll_up.Disable();
            scroll_down = new Button(new Vector2(pos.X + wdth - 10, pos.Y+ height - 10), 20, 20, ScrollDown, "scroll down");
            scroll_down.Disable();
        }
        public override void CheckClick(int x, int y)
        {
            if(scroll_up.mouseOver(x, y))
            {
                scroll_up.ButtonAction();
            }
            if (scroll_down.mouseOver(x, y))
            {
                scroll_down.ButtonAction();
            }   
            CheckTextClick(x, y);
        }
        public void CheckTextClick(int mouse_x, int mouse_y)
        {
            foreach (TextOverlay t in knowledge)
            {
                if (t.isMouseOver(new Vector2(mouse_x, mouse_y)))
                {
                    t.CycleColor();
                }
            }
        }

        public override void Draw(SpriteBatch spritebatch, GameTime gametime)
        {
            int total_height = 0;
            
            TextOverlay t;
            for(int i = starting_index; i < knowledge.Count; i++)
            {
                t = knowledge[i];
                total_height += t.height;
                if (total_height > height)
                {
                    break;
                }
                else
                {
                    t.Draw(spritebatch, separator, scalar);
                }
                //spritebatch.Draw(separator, new Vector2(t.position.X, t.position.Y + t.height), null, Color.White, 0f, new Vector2(0, 0), scalar, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);
            }
            if (absolute_total > height)
            {
                scroll_scale = (((float)height / scroll_bar.Height) / ((float)absolute_total / scroll_bar.Height)) * ((float)height / scroll_bar.Height);
            }
            float scroll_height = scroll_bar.Height * scroll_scale;
            float scroll_diff = height - scroll_height - 40;
            float above_text_height = 0;
            for (int i = 0; i < starting_index; i++)
            {
                above_text_height += knowledge[i].height;
            }
            scroll_diff = above_text_height / total_height;
            scroll_diff = height * scroll_diff;
            spritebatch.Draw(scroll_bar, new Vector2(scroll_bar_posn.X, scroll_bar_posn.Y + scroll_diff), null, Color.White, 0f, new Vector2(0, 0), new Vector2(1, scroll_scale), SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER + .01f);
            scroll_up.Draw(spritebatch);
            scroll_down.Draw(spritebatch);
        }

        public void ScrollUp()
        {
            if (starting_index <= 0)
            {
                scroll_up.Disable();
                return;
            }
            else
            {
                int new_index = starting_index - 1;
                int old_value_height = knowledge[starting_index].height;
                foreach (TextOverlay t in knowledge)
                {
                    t.position.Y = t.position.Y + old_value_height;
                }
                starting_index = new_index;
                if (getCurrentHeight() > height)
                {
                    scroll_down.Enable();
                }
                if (starting_index == 0) scroll_up.Disable();
            }

        }
        public void ScrollDown()
        {
            int new_index = starting_index + 1;
            if (starting_index >= knowledge.Count)
            {
                return;
            }
            int old_value_height = knowledge[starting_index].height;
            foreach (TextOverlay t in knowledge)
            {
                t.position.Y = t.position.Y - old_value_height;
            }
            starting_index = new_index;
            if (getCurrentHeight() < height)
            {
                scroll_down.Disable();
                scroll_up.Enable();
            }
        }

        public float getCurrentHeight()
        {
            int total_height = 0;
            TextOverlay t;
            for (int i = starting_index; i < knowledge.Count; i++)
            {
                t = knowledge[i];
                total_height += t.height;
            }
            return total_height;
        }
        public override void AddKnowledge(DialogInfo s, Boolean isFact)
        {
            int y_pos = (int)(position.Y + 3);
            int total_height = 0;
            foreach (TextOverlay t in knowledge)
            {
                int t_height = t.getHeight() + 3;
                y_pos += t_height;
                total_height += t_height;
            }

            Vector2 newPos = new Vector2(position.X + 5, y_pos);
            TextOverlay newFact = new TextOverlay(s.Info, newPos, s.Source, s.ResponsePrompt, true, width - 60);
            if (!isFact)
            {
                newFact.isColored = true;
            }
            else
            {
                newFact.drawCol = new Color(40, 200, 60);
            }
            newFact.getDimensions();
            total_height += newFact.height + 3;
            absolute_total = total_height;
            if (total_height > height)
            {
                scroll_down.Enable();
            }
            knowledge.Add(newFact);
        }

        public List<TextOverlay> GetKnowledge()
        {
            return knowledge;
        }

        public override void LoadContent(ContentManager cm)
        {
            separator = cm.Load<Texture2D>("PDA//Notes//Separator");
            scroll_bar = cm.Load<Texture2D>("PDA//Notes//scroll_bar");
            scroll_down.SetTexture(Textures.pda_pointer);
            scroll_up.SetTexture(Textures.pda_pointer);
            scroll_scale = height / scroll_bar.Height;

        }
    }
}