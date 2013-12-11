using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MinutesToMidnight
{

    public class BunkerButton
    {
        Vector2 position;
        int width;
        int height;
        public Boolean used;

        public BunkerButton(Vector2 pos, int w, int h)
        {
            position = pos;
            width = w;
            height = h;
            used = false;
        }

        public Boolean isOverButton(int mX, int mY)
        {
            return (mX >= position.X && mY >= position.Y &&
                mX <= (position.X + width) && mY <= (position.Y + height));
        }

        public MouseType getMouse(int mX, int mY)
        {
            if (isOverButton(mX, mY))
            {
                Console.WriteLine("case");
                return MouseType.BUTTON_ROOM;
            }
            else 
            {
                Console.WriteLine("back");
                return MouseType.BACKGROUND;
            }
        }
    }
}
