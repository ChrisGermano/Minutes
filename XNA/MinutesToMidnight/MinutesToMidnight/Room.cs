using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
	public class Room //: IDrawable
	{
        public String name;
        public int bunkerIndex;
		public int leftNeigh;
		public int rightNeigh;
		public int zNeigh;
		private Texture2D background;
        private Vector2 position;
        public List<Item> items;
        public List<Person> people;
		public int drawOrder;
		public bool visible;
        public Door[] doors;

        public BunkerButton bb;

        public ButtonGuard bg;

        private Random r;
        private int[] personSpawns = {50, 60, 70, 150, 160, 170, 250, 260, 270, 350, 360, 370, 450, 460, 470, 550, 560, 570, 650, 660, 670};


		//Param: Background image, neighbors, and a list of items
		public Room (String n, int bIndex, int[] neighbors = null, List<Item> itms = null, List<Person> ppl = null)
		{
            name = n;
            bunkerIndex = bIndex;
            position = new Vector2(0, 0);
			drawOrder = (int)e_DRAWORDER.Room;
			visible = false;
            doors = new Door[3];
            r = new Random();

            if (bunkerIndex == 9)
            {
                bb = new BunkerButton(new Vector2(350, 150), 95, 250);
            }
            else
            {
                bb = null;
            }

            if (itms == null)
            {
                items = new List<Item>();
            } else {
                items = itms;
            }
            if (ppl == null)
            {
                people = new List<Person>();
            } else {
                people = ppl;
            }

            if (itms != null)
            {
                for (int i = 0; i < itms.Count; i++)
                {
                    Item temp = itms[i];
                    if (temp.position == new Vector2(0, 0))
                    {
                        temp.position = new Vector2(r.Next(50, (int)(Game1.screen_size.X - temp.width - 50)), WorldConstants.ITEM_WALL_Y_POSITION);
                        itms[i] = new Item(temp.name, temp.position, temp.type, temp.fact);
                    }
                }
			    items = itms;
            }

            if (ppl != null)
            {
                foreach (Person p in ppl)
                {
                    if (p.position == new Vector2(0, 0))
                    {
                        p.position = new Vector2(personSpawns[r.Next(personSpawns.Length)], WorldConstants.PERSON_Y_POSITION);
                        System.Threading.Thread.Sleep(50); //Because C# random sux lol
                    }
                }
            people = ppl;
            }

            if (bunkerIndex == 9)
            {
                bg = new ButtonGuard(new Vector2(250, 210));
            }
		}

        //update room content
        public void Update()
        {
            foreach (Item i in items)
            {
                i.Update();
            }
            foreach (Person p in people)
            {
                p.Update();
            }
        }
        //Re-places people and items in the room
        public void refreshContents(List<int> itemLocs, List<int> personLocs)
        {
            foreach (Item i in items)
            {
                if (i.type == "bulletin")
                {
                    int tempIndex = r.Next(0, itemLocs.Count);
                    i.position = new Vector2(itemLocs[tempIndex], WorldConstants.BULLETIN_WALL_Y_POSITION);
                    itemLocs.Remove(itemLocs[tempIndex]);
                }
                else if (i.type == "computer")
                {
                    int tempIndex = r.Next(0, itemLocs.Count);
                    i.position = new Vector2(itemLocs[tempIndex], WorldConstants.COMPUTER_WALL_Y_POSITION);
                    itemLocs.Remove(itemLocs[tempIndex]);
                }
                else
                {
                    int tempIndex = r.Next(0, itemLocs.Count);
                    i.position = new Vector2(itemLocs[tempIndex], WorldConstants.ITEM_WALL_Y_POSITION);
                    itemLocs.Remove(itemLocs[tempIndex]);
                }
                i.getDimensions();
                System.Threading.Thread.Sleep(50); //Because C# random sux lol
            }

            foreach (Person p in people)
            {
                if (!p.isGuard)
                {
                    int tempIndex = r.Next(0, personLocs.Count);
                    p.position = new Vector2(personLocs[tempIndex], WorldConstants.PERSON_Y_POSITION);
                    personLocs.Remove(personLocs[tempIndex]);
                    System.Threading.Thread.Sleep(50); //Because C# random sux lol
                }
            }
        }

		public void Draw (SpriteBatch spritebatch, GameTime gameTime)
		{
            spritebatch.Draw(background, position, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 1f);

            Vector2 roomsize = Textures.item_font.MeasureString(name);

            spritebatch.DrawString(Textures.item_font, name, new Vector2((400-roomsize.X/2), 482), Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.01f);

            foreach (Item i in items) {
                i.Draw(spritebatch, gameTime);
            }
            foreach (Person p in people)
            {
                p.Draw(spritebatch, gameTime);
            }

            foreach (Door d in doors)
            {
                if (d.toRoom != null)
                {
                    d.Draw(spritebatch, gameTime);
                }
            }

            if (bunkerIndex == 9)
            {
                bg.Draw(spritebatch, gameTime);
            }
		}

		public MouseType CheckMouseOver (int mx, int my)
        {
            MouseType mtype = MouseType.BACKGROUND;

            if (bunkerIndex == 9)
            {
                mtype = bg.GetMouseType(new Vector2(mx, my));
                if (mtype == MouseType.BUTTON_GUARD || mtype == MouseType.BUTTON_GUARD_OVERLAY)
                {
                    return mtype;
                }
            }

            foreach (Person p in this.people)
            {
                mtype = p.CheckMouseOver(new Vector2(mx, my));
                if (mtype == MouseType.CHARACTER || mtype == MouseType.ITEM_OVERLAY)
                {
                    return mtype;
                }
            }

            foreach (Item i in this.items)
            {
                mtype = i.CheckMouseOver(new Vector2(mx, my));
                if (mtype == MouseType.ITEM || mtype == MouseType.CHARACTER || mtype == MouseType.ITEM_OVERLAY)
                {
                    return mtype;
                }
            }

            foreach (Door d in this.doors)
            {
                mtype = d.CheckMouse(new Vector2(mx, my));
                if (mtype == MouseType.DOOR)
                {
                    return mtype;
                }
            }

            return mtype;
		}

		//Param: Point in space where user clicked
		//Return: Index of Item in Room that is clicked, -1 if none
		public int itemClicked (Vector2 point)
		{
			for (int i = 0; i < items.Count; i++) {
				if (items[i].CheckMouse(point)) {
					return i;
				}
			}
			return -1;
		}

        public int personClicked(Vector2 point)
        {
            for (int i = 0; i < people.Count; i++)
            {
                if (people[i].CheckMouse(point))
                {
                    return i;
                }
            }
            return -1;
        }

        public List<Item> GetItems()
        {
            return items;
        }

        public List<Person> GetPeople()
        {
            return people;
        }


        //Param: Point in space where user clicked
        //Return Index of Door in the Room that is clicked, -1 if none
        public int doorClicked(Vector2 point)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].CheckMouseOver(point))
                {
                    return i;
                }
            }
            return -1;
        }

        internal void LoadContent(ContentManager contentManager)
        {
            foreach (Item i in items)
            {
                i.LoadContent(contentManager);
            }
            foreach (Person p in people)
            {
                p.LoadContent(contentManager);
            }
            if (bg != null)
            {
                bg.LoadContent(contentManager);
            }
        }

        internal void SetBackground(Texture2D texture2D)
        {
            background = texture2D;
        }
    }
}