using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace MinutesToMidnight
{
    public abstract class PDAScreen
    {
        public List<TextOverlay> knowledge;
        abstract public void Draw(SpriteBatch spritebatch, GameTime gameTime);
        public virtual void AddKnowledge(PersonInfo s, Boolean isFact)
        {

        }
        public virtual void CheckClick(int x, int y)
        {
        }
        public virtual void LoadContent(ContentManager contentmanager)
        {

        }

        public virtual void setActiveRoom(int p)
        {
            
        }
    }
}
