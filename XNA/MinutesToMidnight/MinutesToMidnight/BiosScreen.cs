using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MinutesToMidnight.Animating;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    class BiosScreen : PDAScreen
    {
        Vector2 position;
        public List<TextOverlay> knowledge;
        Texture2D separator;
        List<Person> people;
        int height;
        int width;
        float scaling = 0.5f;
        float screen_scale;
        public BiosScreen(Vector2 pos, int hght, int wdth, List<Person> ppl, float scale)
        {
            position = pos;
            knowledge = new List<TextOverlay>();
            height = hght;
            width = wdth;
            people = new List<Person>();
            screen_scale = scale;
            foreach (Person p in ppl)
            {
                if (!p.isGuard)
                {
                    people.Add(p);
                }
            }
        }


        public override void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            //spritebatch.Draw(Textures.pda_timeline, position, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.PDA_SCREEN_LAYER);
            Vector2 head_psn = new Vector2(position.X + 10, position.Y + 10);
            Vector2 orig_head_psn = new Vector2(head_psn.X, head_psn.Y);
            foreach (Person p in people)
            {
                p.DrawHeadshot(spritebatch, gameTime, head_psn, scaling);
                Vector2 size = Textures.pda_font.MeasureString(p.name);
                spritebatch.DrawString(Textures.pda_font, p.name, new Vector2(head_psn.X + p.width * scaling + 10, head_psn.Y + (p.head_height * scaling)/2), Color.Black);

                head_psn = new Vector2(head_psn.X, head_psn.Y);  
                head_psn.Y = head_psn.Y + p.head_height * scaling + 10;
                spritebatch.Draw(separator, new Vector2(position.X, head_psn.Y -5), null, Color.White, 0f, new Vector2(0,0), screen_scale, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);
            }
        }

        public override void LoadContent(ContentManager cm)
        {
            separator = cm.Load<Texture2D>("PDA//Contacts//Separator");
        }
    }
}