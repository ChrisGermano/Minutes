using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinutesToMidnight
{

    public class Door
    {

        public Texture2D texture;
        public Vector2 position;

        public Boolean mouseOver;

        //Index of origin and destination rooms in the GameWorld
        public Room toRoom;
        public Room fromRoom;
        public int width = 30;
        public int height = 300;

        //Default constructor
        public Door(Room from, int dir, Room to = null)
        {
            texture = Textures.default_door;
            toRoom = to;
            fromRoom = from;
            mouseOver = false;

            switch (dir)
            {
                case 0:
                    position = new Vector2(0, 150);
                    break;
                case 1:
                    position = new Vector2(770, 150);
                    break;
                default:
                    if (from.bunkerIndex == 3 || from.bunkerIndex == 9)
                    {
                        position = new Vector2(140, 80);
                    }
                    else if (from.bunkerIndex == 6 || from.bunkerIndex == 7)
                    {
                        position = new Vector2(600, 80);
                    }
                    else if (from.bunkerIndex == 8)
                    {
                        position = new Vector2(150, 80);
                        //Make closed special doorway
                    }
                    else
                    {
                        position = new Vector2(340, 80);
                    }
                    width = 120;
                    break;
            }

        }

        //Param: Mouse location
        //Return: MouseType corresponding to position
        public MouseType CheckMouse(Vector2 pos)
        {
            if (CheckMouseOver(pos))
            {
                mouseOver = true;
                return MouseType.DOOR;
            }

            mouseOver = false;
            return MouseType.BACKGROUND;
        }


        //Param: Mouse location
        //Return: Whether the mouse is over the door
        public Boolean CheckMouseOver(Vector2 pos)
        {
            return (pos.X >= position.X &&
                pos.Y >= position.Y &&
                pos.X <= (position.X + width) &&
                pos.Y <= (position.Y + height));
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (position.X == 0)
            {
                spriteBatch.Draw(Textures.default_door, position, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.DOOR_LAYER);
            }
            else if (position.X == 770)
            {
                spriteBatch.Draw(Textures.default_door, position, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.FlipHorizontally, DrawConstants.DOOR_LAYER);
            }
            else
            {
                spriteBatch.Draw(Textures.z_door, position, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.ITEM_LAYER);
            }

            if (mouseOver)
            {
                spriteBatch.DrawString(Textures.item_font, "Go to " + toRoom.name, new Vector2(5, 0), Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.DOOR_LAYER);
            }
        }

    }
}
