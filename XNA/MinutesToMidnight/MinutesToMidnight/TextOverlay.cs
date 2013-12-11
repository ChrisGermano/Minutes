using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    public class TextOverlay
    {

        public String text;
        public Vector2 position;
        public Vector2 source_position;
        public int width;
        public int height;
        public Boolean isVisible;
        public Color drawCol;
        public Boolean isColored;
        public string source;
        private int pda_width;
        private SpriteFont sprite_font;
        bool in_pda;
        public TextOverlay(String t, Vector2 pos, string _source = "", bool pda = false, int pdawidth = 800)
        {
            text = t;
            position = pos;
            isVisible = false;
            drawCol = Color.Black;
            isColored = false;
            source = _source;
            in_pda = pda;
            pda_width = pdawidth;
        }

        //For PDA text altering
        public void makeColorable()
        {
            isColored = true;
        }

        public void getDimensions()
        {
            String[] lineOne = text.Split(' ');
            String[] lineTwo = new String[lineOne.Length];
            string new_lineOne;
            int count = lineOne.Length - 1;
            if (in_pda)
            {
                sprite_font = Textures.pda_font;
            }
            else
            {
                sprite_font = Textures.item_font;
            }
            while (sprite_font.MeasureString(String.Join(" ", lineOne)).X > pda_width)
            {
                lineTwo[count] = lineOne[count];
                lineOne[count] = "";
                count--;
            }

            new_lineOne = String.Join(" ", lineOne);
            Vector2 size = sprite_font.MeasureString(new_lineOne);
            float base_displacement = size.Y;
            float displacement = base_displacement;
            if (lineTwo[lineOne.Length - 1] != null)
            {
                displacement += (base_displacement + 5);
            }
            if (in_pda)
            {
                source_position = new Vector2(position.X + pda_width / 2, position.Y + displacement);
                displacement += base_displacement;
            }

            height = (int)displacement;
            width = (int)size.X;
        }
        public int getHeight()
        {
            return height;
        }
        public MouseType setMouseType(Vector2 mousePos)
        {
            if (isMouseOver(mousePos))
            {
                return MouseType.TEXT_OVERLAY;
            }
            return MouseType.BACKGROUND;
        }

        public Boolean isMouseOver(Vector2 mousePos)
        {
            return (mousePos.X >= position.X &&
                mousePos.Y >= position.Y &&
                mousePos.X <= (position.X + width) &&
                mousePos.Y <= (position.Y + height));
        }

        public void CycleColor()
        {
            if (isColored)
            {
                if (drawCol == Color.Black)
                {
                    drawCol = Color.Red;
                }
                else if (drawCol == Color.Red)
                {
                    drawCol = new Color(40,200,60);
                }
                else if (drawCol == new Color(40, 200, 60))
                {
                    drawCol = Color.Black;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D separator = null, float scalar = 0f)
        {
            getDimensions();
            if (in_pda)
            {
                DrawTimelineText(spriteBatch, separator, scalar);
            }
            else
            {
                spriteBatch.DrawString(sprite_font, text, position, drawCol, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
        }

        //Taken from Item class shh don't tell
        private void DrawTimelineText(SpriteBatch spritebatch, Texture2D separator, float scalar)
        {
            String[] lineOne = text.Split(' ');
            String[] lineTwo = new String[lineOne.Length];
            string new_lineOne;
            int count = lineOne.Length - 1;

            while (sprite_font.MeasureString(String.Join(" ", lineOne)).X > pda_width)
            {
                lineTwo[count] = lineOne[count];
                lineOne[count] = "";
                count--;
            }
            
            new_lineOne =  String.Join(" ", lineOne);

            spritebatch.DrawString(sprite_font, new_lineOne, position, drawCol, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.1f);
            float base_displacement = sprite_font.MeasureString(new_lineOne).Y;
            float displacement = base_displacement;
            if (lineTwo[lineOne.Length - 1] != null)
            {

                spritebatch.DrawString(sprite_font, String.Join(" ", lineTwo).Trim(), new Vector2(position.X, position.Y + displacement), drawCol, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.1f);
                displacement += (base_displacement + 5);
            }
            source_position = new Vector2(position.X + pda_width/2, position.Y + displacement);
            spritebatch.DrawString(sprite_font, "-- " + source, source_position, drawCol, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.1f);

            displacement += base_displacement;
            spritebatch.Draw(separator, new Vector2(position.X, position.Y + displacement), null, Color.White, 0f, new Vector2(0, 0), scalar, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);

            height = (int)displacement;

        }
    }
}
