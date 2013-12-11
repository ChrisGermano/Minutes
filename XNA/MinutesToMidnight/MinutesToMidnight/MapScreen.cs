using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    class MapScreen : PDAScreen
    {
        Vector2 position;
        int height;
        int width;
        float scalar;
        Texture2D texture;
        Texture2D posnpointer;
        Vector2[] room_position;
        int active_room;

        public MapScreen(Vector2 pos, int hght, int wdth, float scale)
        {
            height = hght;
            width = wdth;
            position = pos;
            scalar = scale;
            room_position = new Vector2[10];
            active_room = 0;
        }


        public override void Draw(SpriteBatch spritebatch, GameTime gametime)
        {
            spritebatch.Draw(texture, position, null, Color.White, 0, new Vector2(0, 0), scalar, SpriteEffects.None, DrawConstants.PDA_SCREEN_LAYER);
            Vector2 psn = room_position[active_room];
            psn = position + psn;

            spritebatch.Draw(posnpointer, psn, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);
            Vector2 newpsn = new Vector2(position.X + 40, position.Y + height - 10);
            spritebatch.Draw(posnpointer, newpsn, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);
            newpsn.X = newpsn.X + 20;
            spritebatch.DrawString(Textures.pda_font, " = You Are Here", newpsn, Color.Black);

        }

        public override void LoadContent(ContentManager cm)
        {
            texture = cm.Load<Texture2D>("PDA//Map//Map_Main_Asset");

            posnpointer = cm.Load<Texture2D>("PDA//Map//locator");
            height = (int)(texture.Height * scalar);
            width = (int)(texture.Width * scalar);

            //room_position[0] = new Vector2(width / 2, height / 10);
            //room_position[1] = new Vector2(9 * (width / 10), height / 3);
            //room_position[2] = new Vector2(.9f * width, 2 * (height / 3));
            //room_position[3] = new Vector2(width / 2, .92f * height);
            //room_position[4] = new Vector2(width / 6, 2*(height / 3));
            //room_position[5] = new Vector2(width / 6, height / 3);
            //room_position[6] = new Vector2(width / 3, height / 2);
            //room_position[7] = new Vector2(.55f * width, 2 * (height / 3));
            //room_position[8] = new Vector2(.65f *width, .3f * height );
            //room_position[9] = new Vector2(width / 2, height / 2);            
            room_position[0] = new Vector2(width / 2, 20);
            room_position[1] = new Vector2(width - 40, 60);
            room_position[2] = new Vector2(width - 40, height - 60);
            room_position[3] = new Vector2(width / 2, height - 20);
            room_position[4] = new Vector2(40, height - 60);
            room_position[5] = new Vector2(40, 60);
            room_position[6] = new Vector2(60, height / 2);
            room_position[7] = new Vector2(width/2 + 30, height - 60);
            room_position[8] = new Vector2(width/2 + 30, 60);
            room_position[9] = new Vector2(110, 110);
 
            
            position = new Vector2(position.X + 5, Game1.screen_size.Y / 2 - height/2 - 20);
        }


        public override void setActiveRoom(int p)
        {
            active_room = p;
        }
    }
}