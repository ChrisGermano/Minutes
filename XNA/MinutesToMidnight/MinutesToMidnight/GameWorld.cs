using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace MinutesToMidnight
{
	public class GameWorld
	{
        //The player, bunker, and (index of) active bunker room
		private Player myPlayer;

        private List<Person> People;
        //Mouse attributes
		private MouseState m_state;
		private MouseState previous_m_state;
		private	int mouse_x;
		private int mouse_y;
        private MouseType mouse_type;
        private ConversationManager convo_manager;
        //The player's PDA
        public PDA pda;

        //Random for game generation
		private Random rnd = new Random ();
        public Bunker bunker;
        //Placeholder Rooms for bunker generation/relations
		public Room entrance = new Room("", -1, null, null, null);
        public Room visitor = new Room("", -1, null, null, null);
        public Room storage = new Room("", -1, null, null, null);
        public Room cafeteria = new Room("", -1, null, null, null);
        public Room living = new Room("", -1, null, null, null);
        public Room tactMeet = new Room("", -1, null, null, null);
        public Room armorComms = new Room("", -1, null, null, null);
        public Room presOffice = new Room("", -1, null, null, null);
        public Room button = new Room("", -1, null, null, null);

		//Available foreign country names
		private String[] countryNames = {
			"Derpania",
			"Derpistan",
			"Derpopolis"
		};

        //Whether the game is running, either on start menu or end game
        public Boolean gameRunning;
        public Boolean start_screen;
        private Button start_button;
        //A manager to handle end game processes
        public EndGameManager egm;

		//Number of facts in this game
		static private int numPerFacts = 2;
		static private int numItmFacts = 2;
			
		//Chosen names/facts
		private String interCountry;
		private String[] chosenPersonFacts = new String[numPerFacts];
		private String[] chosenItemFacts = new String[numItmFacts];

		public GameWorld ()
		{		
            //Set up data before bunker initialization
            SetPlayer();

            SetBunker();
            bunker.setActiveRoom(0);
            SetPDA();
            SetTime();
            egm = new EndGameManager();
            Random r = new Random();
            //When start screen developed this begins false

            gameRunning = true;
            start_screen = true;
            int button_width = 200;
            int button_height = 50;
            convo_manager = new ConversationManager(null, null);
            start_button = new Button(new Vector2(Game1.screen_size.X / 2 - button_width / 2, 400), button_width, button_height, ChangeToMain, "start");

		}

        private void start_update()
        {
            this.clickListen();
            this.checkMouseOver();
        }
        private void ChangeToMain()
        {
            start_screen = false;
        }
        //Updates the game
        public void Update()
        {
            previous_m_state = m_state;
            m_state = Mouse.GetState();
            mouse_x = m_state.X;
            mouse_y = m_state.Y;
            bunker.Update();
            if (start_screen)
            {
                start_update();
            }
            else if (egm.IsActive())
            {
                egm.Update();
                this.clickListen();
                this.checkMouseOver();
            }
            else if (gameRunning)
            {
                this.clickListen();
                this.checkMouseOver();

                Boolean guardTalk = bunker.activeRoom == 9 && bunker.rooms[bunker.activeRoom].bg.inspected;

                //If there isn't an inspected Item, update the Player
                if (noOverlay() && !guardTalk)
                {
                    myPlayer.Update();
                    if (myPlayer.hasTargetPerson())
                    {
                        if (myPlayer.targetPerson.inspected && !convo_manager.Active)
                        {
                            if (convo_manager.ShouldClose())
                            {
                                convo_manager.CloseConvo();
                                myPlayer.targetPerson.inspected = false;
                                myPlayer.targetPerson = null;

                            }
                            else
                            {
                                convo_manager = new ConversationManager(myPlayer, myPlayer.targetPerson);
                            }
                        }
                        else
                        {
                            if (convo_manager.Active)
                            {

                                if (convo_manager.continue_convo.mouseOver(mouse_x, mouse_y))
                                {
                                    mouse_type = MouseType.CONVO_ARROW;
                                }

                                if (convo_manager.HasInfo())
                                {
                                    DialogInfo info = convo_manager.GetInfo();
                                    myPlayer.AddKnowledge(info);
                                    pda.AddKnowledge(info, false);
                                }
                                convo_manager.Update();
                                
                            }
                        }
                    }
                    //If the Player is at a targeted Door, load a new Room
                    if (myPlayer.atDoor)
                    {
                        bunker.setActiveRoom(myPlayer.targetDoor.toRoom.bunkerIndex);
                        pda.setActiveRoom(myPlayer.targetDoor.toRoom.bunkerIndex);
                        myPlayer.RoomReset(myPlayer.targetDoor);
                        myPlayer.atDoor = false;
                    }
                }

                if (myPlayer.timeLeft <= 0 && !myPlayer.ending)
                {
                    bunker.setActiveRoom(9);
                    myPlayer.RoomReset(bunker.rooms[8].doors[2]);
                    Door d = bunker.rooms[9].doors[2];
                    d.width = 0;
                    d.height = 0;
                    d.toRoom = null;
                    myPlayer.ending = true;
                }
            }
            else
            {
                egm.Update();
            }
		}

        //Checks if any overlays are active in the player's room
        private Boolean noOverlay()
        {
            foreach (Item i in bunker.GetActiveItems())
            {
                if (i.inspected)
                {
                    return false;
                }
            }
            return true;
        }

        //Instantiates a player for this game
        private void SetPlayer()
        {
            myPlayer = new Player();
        }

        //Initialize the PDA
        private void SetPDA()
        {
            pda = new PDA(this.People);
        }

        private void SetBunker()
        {
            List<Item> defaultItms = new List<Item>();
            List<Person> defaultPeople = new List<Person>();
            ObjLoader<Person> people_loader = new ObjLoader<Person>();
            ObjLoader<Response> response_loader = new ObjLoader<Response>();
            ObjLoader<Item> item_loader = new ObjLoader<Item>();
            string pre = "Information\\";
          //  if (Directory.Exists("..\\Debug"))
          //  {
          //      pre = "..\\Debug\\";
          //  }
            List<Person> loaded_people = people_loader.Load(pre + "people.xml");
            List<Response> loaded_responses = response_loader.Load( pre + "opinions.xml");
            List<Item> loaded_items = item_loader.Load(pre + "items.xml");

            List<Response> Specifics = new List<Response>();
            List<Response> templist = new List<Response>();
            foreach (Response r in loaded_responses)
            {
                if (r.specificPerson != "")
                {
                    Specifics.Add(r);
                    templist.Add(r);
                }
            }

            foreach (Response r in templist)
            {
                loaded_responses.Remove(r);
            }

            templist = null;
            foreach (Person p in loaded_people)
            {
                p.Initialize();
                p.addSpecifics(ref Specifics);
                p.addResponse(ref loaded_responses);
                p.createResponseOverlays();
            }

            People = new List<Person>(loaded_people);
            bunker = new Bunker(loaded_items, loaded_people);
        }
		//Sets the foreign country name as one from the
		//array of choices
		private void SetCountry() {
			int cInt = rnd.Next (0, countryNames.Length);
			interCountry = countryNames [cInt];
		}

        private void SetTime()
        {
            int timeleft = 0;
            foreach (Room r in bunker.rooms) {
                foreach (Item i in r.items)
                {
                    timeleft++;
                }
                foreach (Person p in r.people)
                {
                    timeleft++;
                }
                timeleft ++;
            }
            myPlayer.timeLeft = timeleft;
        }

		//Helper function for String array randomization
		//Implements Fisher-Yates shuffle O(n)
		private String[] arrayShuffle(String[] ary) {
			for (int x = 0; x < ary.Length; x++) {
				int tmpVal = rnd.Next (0, x);
				String tmp = ary [tmpVal];
				ary [tmpVal] = ary [x];
				ary [x] = tmp;
			}
			return ary;
		}

        //Handle mouse clicks on all Items, People, overlays, and the Player
        private void clickListen()
        {

            if (m_state.LeftButton == ButtonState.Pressed)
            {
                //Click this frame
                if (previous_m_state.LeftButton != ButtonState.Pressed)
                {
                    if (start_screen)
                    {
                        if (start_button.mouseOver(mouse_x, mouse_y))
                        {
                            start_button.ButtonAction();
                        }
                    }
                    else if (egm.IsActive())
                    {
                        egm.ClickCheck(mouse_x, mouse_y);
                    }//If the mouse is in the window
                    else if (inRoom(mouse_x, mouse_y))
                    {

                        if (pda.Active)
                        {
                            pda.CheckMousePositionClick(mouse_x, mouse_y);
                            if (pda.active_screen == "timeline")
                            {
                                pda.screens["timeline"].CheckClick(mouse_x, mouse_y);
                            }
                        }
                        else
                        {
                            if (convo_manager.Active)
                            {
                                convo_manager.CheckClick(mouse_x, mouse_y);
                            }
                            else if (myPlayer.hasTargetItem() && myPlayer.targetItem.inspected)
                            {
                                if (mouse_type == MouseType.ITEM_OVERLAY)
                                {
                                    if (!myPlayer.hasInfo(myPlayer.targetItem.fact))
                                    {
                                        DialogInfo info = new DialogInfo(myPlayer.targetItem.fact, myPlayer.targetItem.name, myPlayer.targetItem.question);
                                        myPlayer.AddKnowledge(info);
                                        pda.AddKnowledge(info, true);
                                        myPlayer.timeLeft--;
                                    }

                                    myPlayer.targetItem.inspected = false;
                                    myPlayer.setTargetItem(null);
                                }
                            }
                            else if (myPlayer.CheckMousePosition(mouse_x, mouse_y))
                            {
                                myPlayer.targetItem = null;
                                myPlayer.targetPerson = null;
                                myPlayer.targetDoor = null;
                                myPlayer.setDestination(myPlayer.position);
                                pda.Open();
                            }
                            else if (myPlayer.hasTargetGuard() && myPlayer.targetGuard.inspected)
                            {
                                ButtonGuard tempBG = myPlayer.targetGuard;

                                if (tempBG.overlays[1].isMouseOver(new Vector2(mouse_x,mouse_y)))
                                {
                                    bunker.rooms[9].bb.used = true;
                                    gameRunning = false;
                                    egm.Initialize(pda.screens["timeline"].knowledge, getAllResponses());
                                }
                                else if (tempBG.overlays[2].isMouseOver(new Vector2(mouse_x, mouse_y)))
                                {
                                    myPlayer.targetGuard.inspected = false;
                                    myPlayer.setTargetGuard(null);
                                }
                            }
                            else
                            {
                                int target;
                                if (mouse_type == MouseType.DOOR)
                                {
                                    target = bunker.activeDoorClicked(new Vector2(mouse_x, mouse_y));
                                    if (target == -1)
                                    {
                                        return;
                                    }
                                    myPlayer.setTargetDoor(bunker.getActiveRoomDoors(target));
                                }
                                else if (mouse_type == MouseType.ITEM)
                                {
                                    target = bunker.getActiveRoomItemClicked(new Vector2(mouse_x, mouse_y));
                                    if (target == -1)
                                    {
                                        return;
                                    }
                                    myPlayer.setTargetItem(bunker.GetActiveItems()[target]);
                                }
                                else if (mouse_type == MouseType.CHARACTER)
                                {
                                    target = bunker.getActiveRoomPersonClicked(new Vector2(mouse_x, mouse_y));
                                    if (target == -1)
                                    {
                                        return;
                                    }
                                    myPlayer.setTargetPerson(bunker.getActiveRoomPeople()[target]);
                                }
                                else if (mouse_type == MouseType.BUTTON_GUARD)
                                {
                                    myPlayer.setTargetGuard(bunker.rooms[bunker.activeRoom].bg);
                                }
                                else if (mouse_type == MouseType.BUNKER_BUTTON_CASE || mouse_type == MouseType.BUNKER_BUTTON_BUTTON || mouse_type == MouseType.BUTTON_ROOM)
                                {
                                    //End no launch
                                    if (!bunker.rooms[9].bb.used)
                                    {
                                        bunker.rooms[9].bb.used = true;
                                        gameRunning = false;
                                        Console.WriteLine("screens: " + pda.screens["timeline"].knowledge);
                                        egm.Initialize(pda.screens["timeline"].knowledge, getAllResponses());
                                    }
                                }
                                else if (mouse_type == MouseType.BUNKER_BUTTON_BUTTON)
                                {
                                    //End launch
                                    if (!bunker.rooms[9].bb.used)
                                    {
                                        bunker.rooms[9].bb.used = true;
                                        gameRunning = false;
                                        Console.WriteLine("screens: " + pda.screens["timeline"].knowledge);
                                        egm.Initialize(pda.screens["timeline"].knowledge, getAllResponses());
                                    }
                                }
                                else
                                {
                                    myPlayer.targetItem = null;
                                    myPlayer.targetPerson = null;
                                    myPlayer.targetDoor = null;
                                }

                                myPlayer.setDestination(new Vector2(mouse_x, mouse_y));
                            }
                        }
                    }
                }
            } 
        }

        //Get all game responses
        private List<Response> getAllResponses()
        {
            List<Response> allRes = new List<Response>();

            foreach (Room r in bunker.rooms) {
                foreach (Person p in r.people)
                {
                    foreach (Response re in p.responses)
                    {
                        allRes.Add(re);
                    }
                }
                foreach (Item i in r.items)
                {
                    Response newRes = new Response(i.fact);
                    newRes.verity = VERITY.fact;
                    allRes.Add(newRes);
                }
            }

            return allRes;
        }

        //Param: x and y coordinates of a location
        //Return: Whether the given location is in the visible window
        private Boolean inRoom(int x, int y)
        {
            return (x >= 0 && x <= 800 && y >= 0 && y <= 500);
        }

		//Determines which item (if any) is currently moused over
		//And changes display/behavior accordingly
		private void checkMouseOver () {
            if (start_screen)
            {
                this.mouse_type = MouseType.BACKGROUND;
                if (start_button.mouseOver(mouse_x, mouse_y))
                {
                    mouse_type = MouseType.CHARACTER;
                }
            }
            else if (egm.IsActive())
            {
                string launch_p = egm.mouseOver(mouse_x, mouse_y);
                if (launch_p == "launch")
                {
                    mouse_type = MouseType.BUNKER_BUTTON_BUTTON;
                }
                else if (launch_p == "dontlaunch")
                {
                    mouse_type = MouseType.BUNKER_BUTTON_CASE;
                }
                else
                {
                    mouse_type = MouseType.BACKGROUND;
                }
            }
            else if (convo_manager.Active)
            {
                if (convo_manager.CheckMouseOver(mouse_x, mouse_y))
                {
                    mouse_type = MouseType.ITEM;
                }
                else
                {
                    mouse_type = MouseType.BACKGROUND;
                }
            }
            else if (pda.Active)
            {
                this.mouse_type = MouseType.BACKGROUND;
                if (pda.isMouseOver(mouse_x, mouse_y))
                {
                    mouse_type = MouseType.ITEM;
                }
                pda.CheckMousePosition(mouse_x, mouse_y);
            }
            else
            {
                this.mouse_type = myPlayer.CheckMouseOver(mouse_x, mouse_y);
                if (this.mouse_type == MouseType.BACKGROUND)
                {
                    this.mouse_type = bunker.CheckMouseOver(mouse_x, mouse_y);
                }
            }

		}

        //Draw the scene
        public void Draw(SpriteBatch spritebatch, GameTime gametime)
        {
            if (start_screen)
            {
                spritebatch.Draw(Textures.game_logo, new Vector2(230, 20), null, Color.White, 0, new Vector2(0, 0), new Vector2(1,1), SpriteEffects.None, 0f);
                spritebatch.Draw(Textures.studio_logo, new Vector2(0, 450), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);

                bunker.Draw(spritebatch, gametime);
                myPlayer.Draw(spritebatch, gametime);
                start_button.Draw(spritebatch);
            }
            else if (gameRunning)
            {
                bunker.Draw(spritebatch, gametime);
                myPlayer.Draw(spritebatch, gametime);
                pda.Draw(spritebatch, gametime, mouse_x, mouse_y);
                if (convo_manager.Active)
                {
                    convo_manager.Draw(spritebatch, gametime);
                }
            }
            else
            {
                egm.Draw(spritebatch, gametime);
            }
            DrawPointer(spritebatch);
        }


        private void DrawPointer(SpriteBatch spritebatch)
        {
            Vector2 pointerposition = new Vector2(mouse_x - 10, mouse_y);
            
            if (this.mouse_type == MouseType.ITEM ||
                    this.mouse_type == MouseType.CHARACTER ||
                    this.mouse_type == MouseType.ITEM_OVERLAY ||
                    this.mouse_type == MouseType.TEXT_OVERLAY ||
                    this.mouse_type == MouseType.DOOR ||
                    this.mouse_type == MouseType.BUTTON_GUARD ||
                    this.mouse_type == MouseType.BUTTON_GUARD_OVERLAY ||
                    this.mouse_type == MouseType.CONVO_ARROW)
            {
                spritebatch.Draw(Textures.item_pointer, pointerposition, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
            else if (this.mouse_type == MouseType.PLAYER && !(myPlayer.hasTargetItem() && myPlayer.targetItem.inspected))
            {
                spritebatch.Draw(Textures.pda_pointer, new Vector2(mouse_x, mouse_y), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
            else if (this.mouse_type == MouseType.BUNKER_BUTTON_CASE)
            {
                spritebatch.Draw(Textures.case_pointer, new Vector2(mouse_x - Textures.case_pointer.Width / 2, mouse_y - Textures.case_pointer.Height / 2), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
            else if (this.mouse_type == MouseType.BUNKER_BUTTON_BUTTON)
            {
                spritebatch.Draw(Textures.button_pointer, new Vector2(mouse_x - Textures.case_pointer.Width / 2, mouse_y - Textures.case_pointer.Height / 2), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
            else
            {
                spritebatch.Draw(Textures.default_pointer, new Vector2(mouse_x, mouse_y), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
        }
        internal void LoadContent(ContentManager contentManager)
        {
            pda.LoadContent(contentManager);
            myPlayer.LoadContent(contentManager);
            bunker.LoadContent(contentManager);
            start_button.LoadContent(contentManager);
            egm.LoadContent(contentManager);
            SoundPlayer.Instance.LoadContent(contentManager);
        }
    }
}
