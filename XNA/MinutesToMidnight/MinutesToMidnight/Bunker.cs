using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MinutesToMidnight
{
    public class Bunker
    {
        public Room[] rooms;
        Boolean full;
        public int activeRoom;

        public Bunker(List<Item> items, List<Person> people)
        {
            full = false;
            //Demonstration of bunker room relations
            //Int values correspond to index in bunker array

            //0 = entrance
            //1 = visitor room
            //2 = cafeteria
            //3 = lounge
            //4 = living quarters
            //5 = storage
            //6 = tactical meeting room
            //7 = armory/communications
            //8 = presidential office
            //9 = button room
            rooms = new Room[10];
            int[] blankNeighs = { -1, -1, -1 };

            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i] = new Room("", i, blankNeighs, null, null);
            }

            loadBunker(items, people);

            rooms[0].name = "Entrance";
            rooms[0].leftNeigh = 5;
            rooms[0].rightNeigh = 1;
            rooms[0].zNeigh = -1;
            activeRoom = 9;
            rooms[1].name = "Sitting Room";
            rooms[1].leftNeigh = 0;
            rooms[1].rightNeigh = 2;
            rooms[1].zNeigh = -1;

            rooms[2].name = "Cafeteria";
            rooms[2].leftNeigh = 1;
            rooms[2].rightNeigh = 3;
            rooms[2].zNeigh = -1;

            rooms[3].name = "Lounge";
            rooms[3].leftNeigh = 2;
            rooms[3].rightNeigh = 4;
            rooms[3].zNeigh = 7;

            rooms[4].name = "Bunks";
            rooms[4].leftNeigh = 3;
            rooms[4].rightNeigh = 5;
            rooms[4].zNeigh = 6;

            rooms[5].name = "Storage";
            rooms[5].leftNeigh = 4;
            rooms[5].rightNeigh = 0;
            rooms[5].zNeigh = -1;

            rooms[6].name = "Meeting";
            rooms[6].leftNeigh = 8;
            rooms[6].rightNeigh = 7;
            rooms[6].zNeigh = 4;
            
            rooms[7].name = "Armory";
            rooms[7].leftNeigh = 6;
            rooms[7].rightNeigh = 8;
            rooms[7].zNeigh = 3;

            rooms[8].name = "Office";
            rooms[8].leftNeigh = 7;
            rooms[8].rightNeigh = 6;
            rooms[8].zNeigh = 9;

            rooms[9].name = "Button Room";
            rooms[9].leftNeigh = -1;
            rooms[9].rightNeigh = -1;
            rooms[9].zNeigh = 8;

            //Initialize Door connections
            for (int i = 0; i < rooms.Length; i++)
            {
                Room temp = rooms[i];

                if (temp.leftNeigh != -1)
                {
                    temp.doors[0] = new Door(temp, 0, rooms[temp.leftNeigh]);
                }
                else
                {
                    temp.doors[0] = new Door(temp, 0, null);
                }

                if (temp.rightNeigh != -1)
                {
                    temp.doors[1] = new Door(temp, 1, rooms[temp.rightNeigh]);
                }
                else
                {
                    temp.doors[1] = new Door(temp, 1, null);
                }

                if (temp.zNeigh != -1)
                {
                    temp.doors[2] = new Door(temp, 2, rooms[temp.zNeigh]);
                }
                else
                {
                    temp.doors[2] = new Door(temp, 2, null);
                }

                foreach (Door d in temp.doors)
                {
                    if (d.toRoom == null)
                    {
                        d.height = 0;
                        d.width = 0;
                    }
                }
            }
        }

        //Generates a bunker with the loaded items and people
        private void loadBunker(List<Item> items, List<Person> people)
        {
            //Rooms with newspaper: 0, 1, 2, 3, 6, 8
            //Rooms with computer: 5, 6, 7
            //Rooms with memo: 0, 1, 2, 4, 5, 6, 7, 8
            //Rooms with bulletin board: 1, 3

            //Rooms with civilian: 0, 1, 2, 3, 4, 5
            //Rooms with politician: 1, 2, 3, 4, 6, 7
            //Rooms with military: 2, 3, 4, 5, 6, 7
            //Rooms with unique person: 0, 8, 9

            int[] newsRooms = { 0, 6, 8 };
            int[] compRooms = { 5, 6 };
            int[] bullRooms = { 1, 3 };

            int[] civRooms = { 0, 1, 2, 3, 4, 5 };
            int[] polRooms = { 1, 2, 3, 4, 6 };
            int[] milRooms = { 2, 3, 4, 5, 6, 7 };

            //Where items can spawn based on room index
            List<int>[] itemSlots = new List<int>[10];
            itemSlots[0] = new List<int> { 240, 350, 460 };
            itemSlots[1] = new List<int> { 40, 130, 425 };
            itemSlots[2] = new List<int> { 20, 50, 80, 120 };
            itemSlots[3] = new List<int> { 280, 380, 470, 600 };
            itemSlots[4] = new List<int> { 100, 200, 300, 400, 500, 600 };
            itemSlots[5] = new List<int> { 300, 390, 480 };
            itemSlots[6] = new List<int> { 500 };
            itemSlots[7] = new List<int> { };
            itemSlots[8] = new List<int> { };
            itemSlots[9] = new List<int> { };

            //Where people can spawn based on room index
            List<int>[] personSlots = new List<int>[10];
            personSlots[0] = new List<int> { 250, 350, 420, 550 };
            personSlots[1] = new List<int> { 40, 100, 425 };
            personSlots[2] = new List<int> { 20, 80, 120, 200, 260, 420 };
            personSlots[3] = new List<int> { 280, 330, 380, 420, 470, 510, 560, 600 };
            personSlots[4] = new List<int> { 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600 };
            personSlots[5] = new List<int> { 300, 320, 350, 375, 420, 450, 460, 480 };
            personSlots[6] = new List<int> { 500 };
            personSlots[7] = new List<int> { };
            personSlots[8] = new List<int> { 630 };
            personSlots[9] = new List<int> { };

            Random r = new Random();

            Person guard1 = null;
            Person guard2 = null;

            foreach (Item i in items)
            {
                if (i.type == "newspaper")
                {
                    int tempIndex = r.Next(0, newsRooms.Length);
                    while (rooms[newsRooms[tempIndex]].items.Count >= itemSlots[newsRooms[tempIndex]].Count)
                    {
                        tempIndex = r.Next(0, newsRooms.Length);
                    }
                    rooms[newsRooms[tempIndex]].items.Add(i);
                }
                else if (i.type == "computer")
                {
                    int tempIndex = r.Next(0, compRooms.Length);
                    while (rooms[compRooms[tempIndex]].items.Count >= itemSlots[compRooms[tempIndex]].Count)
                    {
                        tempIndex = r.Next(0, compRooms.Length);
                    }
                    rooms[compRooms[tempIndex]].items.Add(i);
                }
                else if (i.type == "bulletin")
                {
                    int tempIndex = r.Next(0, bullRooms.Length);
                    while (rooms[bullRooms[tempIndex]].items.Count >= itemSlots[bullRooms[tempIndex]].Count)
                    {
                        tempIndex = r.Next(0, bullRooms.Length);
                    }
                    rooms[bullRooms[tempIndex]].items.Add(i);
                }
            }

            foreach (Person p in people)
            {
                if (p.isGuard)
                {
                    if (guard1 == null) {
                        guard1 = p;
                    } else {
                        guard2 = p;
                    }
                } 
                else if (p.role == ROLE.civilian)
                {
                    int tempIndex = r.Next(0, civRooms.Length);
                    while (rooms[civRooms[tempIndex]].people.Count >= personSlots[civRooms[tempIndex]].Count)
                    {
                        tempIndex = r.Next(0, civRooms.Length);
                    }
                    rooms[civRooms[tempIndex]].people.Add(p);
                }
                else if (p.role == ROLE.politician)
                {
                    int tempIndex = r.Next(0, polRooms.Length);
                    while (rooms[polRooms[tempIndex]].people.Count >= personSlots[polRooms[tempIndex]].Count)
                    {
                        tempIndex = r.Next(0, polRooms.Length);
                    }
                    rooms[polRooms[tempIndex]].people.Add(p);
                }
                else if (p.role == ROLE.military)
                {
                    int tempIndex = r.Next(0, milRooms.Length);
                    while (rooms[milRooms[tempIndex]].people.Count >= personSlots[milRooms[tempIndex]].Count)
                    {
                        tempIndex = r.Next(0, milRooms.Length);
                    }
                    rooms[milRooms[tempIndex]].people.Add(p);
                }
            }
            guard1.position = new Vector2(85,200);
            rooms[0].people.Add(guard1);
            guard2.position = new Vector2(75,200);
            rooms[8].people.Add(guard2);

            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i].refreshContents(itemSlots[i], personSlots[i]);
            }
        }

        public List<Item> GetActiveItems()
        {
            return rooms[activeRoom].GetItems();
        }

        internal Room getActiveRoom(int ActiveRoom)
        {
            if (ActiveRoom <= -1)
            {
                return null;
            }
            return rooms[ActiveRoom];
        }

        public void Update()
        {
            rooms[activeRoom].Update();
        }
        public void setActiveRoom(int room)
        {
            activeRoom = room;
        }

        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            Random rand = new Random();
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + "roomAssets");
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            FileInfo[] files = dir.GetFiles("*_bg.*");
            Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            foreach (FileInfo file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file.Name);
                textures.Add(filename, contentManager.Load<Texture2D>("roomAssets/" + filename));
            }

            rooms[0].SetBackground(textures["start_room_bg"]);
            textures.Remove("start_room_bg");
            rooms[0].LoadContent(contentManager);
            rooms[1].SetBackground(textures["visitor_bg"]);
            textures.Remove("visitor_bg");
            rooms[1].LoadContent(contentManager);
            rooms[2].SetBackground(textures["cafeteria_bg"]);
            textures.Remove("cafeteria_bg");
            rooms[2].LoadContent(contentManager);
            rooms[3].SetBackground(textures["lounge_bg"]);
            textures.Remove("lounge_bg");
            rooms[3].LoadContent(contentManager);
            rooms[4].SetBackground(textures["bunks_bg"]);
            textures.Remove("bunks_bg");
            rooms[4].LoadContent(contentManager);
            rooms[5].SetBackground(textures["storage_bg"]);
            textures.Remove("storage_bg");
            rooms[5].LoadContent(contentManager);
            rooms[6].SetBackground(textures["meeting_bg"]);
            textures.Remove("meeting_bg");
            rooms[6].LoadContent(contentManager);
            rooms[7].SetBackground(textures["armory_bg"]);
            textures.Remove("armory_bg");
            rooms[7].LoadContent(contentManager);
            rooms[8].SetBackground(textures["president_office_bg"]);
            textures.Remove("president_office_bg");
            rooms[8].LoadContent(contentManager);
            rooms[9].SetBackground(textures["button_room_bg"]);
            textures.Remove("button_room_bg");
            rooms[9].LoadContent(contentManager);
        }
    
        internal void Draw(SpriteBatch spritebatch,GameTime gametime)
        {
            rooms[activeRoom].Draw(spritebatch, gametime);
        }

        internal int activeDoorClicked(Vector2 vector2)
        {
            return rooms[activeRoom].doorClicked(vector2);
        }

        internal Door getActiveRoomDoors(int doorTarget)
        {
            return rooms[activeRoom].doors[doorTarget];
        }

        internal int getActiveRoomItemClicked(Vector2 vector2)
        {
            return rooms[activeRoom].itemClicked(vector2);
        }

        internal int getActiveRoomPersonClicked(Vector2 vector2)
        {
            return rooms[activeRoom].personClicked(vector2);
        }

        internal MouseType CheckMouseOver(int mouse_x, int mouse_y)
        {
            return rooms[activeRoom].CheckMouseOver(mouse_x, mouse_y);
        }

        internal List<Person> getActiveRoomPeople()
        {
            return rooms[activeRoom].GetPeople();
        }
    }
}
