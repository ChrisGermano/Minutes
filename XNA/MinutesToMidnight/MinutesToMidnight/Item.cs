using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace MinutesToMidnight
{
    [DataContract]
	public class Item
	{
        [DataMember(Name = "texturename", IsRequired = true)]
        public string texturename;
        public Texture2D texture;

        [DataMember(Name = "overlayname", IsRequired = true)]
        public string overlayname;
        public Texture2D overlay_texture;

        [DataMember(Name = "name", IsRequired = true)]
        public string name;

        public bool mouseOver = false;

        [DataMember(Name = "type", IsRequired = true)]
        public String type;

        [DataMember(Name = "fact", IsRequired = true)]
        public String fact;

        public Vector2 position;
        public Vector2 overlayPosition;
		public int width;
		public int height;

        Rectangle drawRec = new Rectangle();

        public int overlayWidth;
        public int overlayHeight;
        public Boolean inspected;

        public Item()
        {
        }

        public Item(String itemName, Vector2 pos, String itemType, String fa)
        {
            name = itemName;
            position = pos;
            type = itemType;
            fact = fa;
            inspected = false;
            overlayPosition = new Vector2(150, 0);
            getDimensions();
        }

        public void getDimensions()
        {
            if (type == "newspaper")
            {
                width = 88;
                height = 188;
            }
            else if (type == "bulletin")
            {
                width = 184;
                height = 160;
            }
            else if (type == "computer")
            {
                width = 76;
                height = 72;
            }

            overlayWidth = 500;
            overlayHeight = 500;
        }

        //Made virtual for automatic overriding with drawing people
        public virtual void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            spritebatch.Draw(texture, position, drawRec, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.ITEM_LAYER);

            //Render different overlay based on item type
            if (inspected)
            {
                spritebatch.Draw(overlay_texture, new Vector2(150,0), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.ITEM_OVERLAY_LAYER);

                DrawOverlayText(spritebatch, gameTime);
            }
            else
            {
                if (mouseOver)
                {
                    //Render different text based on item type
                    if (type == "newspaper")
                    {
                        spritebatch.DrawString(Textures.item_font, "Read " + name, new Vector2(5, 0), Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.2f);
                    }
                    else if (type == "computer")
                    {
                        spritebatch.DrawString(Textures.item_font, "Use " + name, new Vector2(5, 0), Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.2f);
                    }
                    else if (type == "bulletin")
                    {
                        spritebatch.DrawString(Textures.item_font, "Check " + name, new Vector2(5, 0), Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.2f);
                    }

                }
            }
        }

        private void DrawOverlayText(SpriteBatch spritebatch, GameTime gameTime)
        {
            String[] lineOne = fact.Split(' ');
            String[] lineTwo = new String[lineOne.Length];
            int count = lineOne.Length - 1;

            while (Textures.item_font.MeasureString(String.Join(" ", lineOne)).X > 400)
            {
                lineTwo[count] = lineOne[count];
                lineOne[count] = "";
                count--;
            }

            spritebatch.DrawString(Textures.item_font, String.Join(" ", lineOne), new Vector2(210, 215), Color.Gray, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            spritebatch.DrawString(Textures.item_font, String.Join(" ", lineTwo), new Vector2(210, 235), Color.Gray, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);

        }
        public void Update()
        {

        }

        public void Interact()
        {

        }

        //Created for Person child to use
        public virtual List<Response> GetResponses()
        {
            return new List<Response>();
        }

        //Param: Location of user click
        //Return: If the user click falls on the item
        public Boolean CheckMouse(Vector2 point)
        {
            return (point.X >= position.X &&
                point.Y >= position.Y &&
                point.X <= (position.X + width) &&
                point.Y <= (position.Y + height));
        }

        //Param: Location of user click
        //Return: If the user click falls on the item
        public Boolean OverlayCheckMouse(Vector2 point)
        {
            return (point.X >= overlayPosition.X &&
                point.Y >= overlayPosition.Y &&
                point.X <= (overlayPosition.X + overlayWidth) &&
                point.Y <= (overlayPosition.Y + overlayHeight));
        }

        public virtual MouseType CheckMouseOver(Vector2 point)
        {
            if (this.inspected && this.OverlayCheckMouse(point))
            {
                this.mouseOver = true;
                return MouseType.ITEM_OVERLAY;
            }
            else if (this.CheckMouse(point))
            {
                this.mouseOver = true;
                return MouseType.ITEM;
            }
            else
            {
                this.mouseOver = false;
                return MouseType.BACKGROUND;
            }
        }

        public virtual void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            texture = Textures.interactive_items;

            if (type == "newspaper")
            {
                drawRec = new Rectangle(0, 0, 88, 188);
                overlay_texture = contentManager.Load<Texture2D>("Items//newspaperOverlay");
            }
            else if (type == "bulletin")
            {
                drawRec = new Rectangle(93, 0, 184, 160);
                overlay_texture = contentManager.Load<Texture2D>("Items//bulletinOverlay");
            }
            else if (type == "computer")
            {
                drawRec = new Rectangle(0, 193, 76, 72);
                overlay_texture = contentManager.Load<Texture2D>("Items//computerOverlay");
            }
        }
	}
}