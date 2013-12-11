using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinutesToMidnight.Animating;
using Microsoft.Xna.Framework.Content;

namespace MinutesToMidnight
{
    public class Player
    {
        private static Player instance;

        //Ommitted until animations are created
        //private List<List<Texture2D>> textures;
        private List<Texture2D> textures;
        private AnimationManager animator;

        public Vector2 position;
        private float walkspeed;
        private float destinationX;
        private bool faceLeft;
        private float width;
        private float height;

        public Item targetItem;
        public Door targetDoor;
        public Person targetPerson;
        public ButtonGuard targetGuard;

        public Boolean atDoor;

        //Compiled information from Items
        //Item.fact and Person.responses.dialog
        public List<String> knowledge;

        public int timeLeft;
        public Boolean ending;

        public Player()
        {
            position = new Vector2(500, 220);
            walkspeed = 2.5f;
            destinationX = -1;
            targetItem = null;
            targetDoor = null;
            targetGuard = null;
            faceLeft = true;
            atDoor = false;
            width = 50;
            height = 200;
            knowledge = new List<String>();
            knowledge.Add("What do you know");
            knowledge.Add("goodbye");
            ending = false;
        }

        public static Player Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Player();
                }
                return instance;
            }
        }

        public void Update()
        {
            moveToDest();
        }

        //Param: Point where user clicked
        public void setDestination(Vector2 dest)
        {
            destinationX = dest.X;
            faceLeft = destinationX < position.X;
        }

        //If a player is at most one body width away from an object
        //they're considered "at" it
        private void moveToDest()
        {
            if (destinationX >= 0)
            {
                int threshold = 50;
                if (hasTargetDoor()) threshold = 25;

                if (Math.Abs((position.X + (width / 2)) - destinationX) <= threshold)
                {
                    animator.SetState(AnimationState.IDLE);
                    if (hasTargetItem())
                    {
                        targetItem.inspected = true;
                    }
                    else if (hasTargetPerson() && !targetPerson.inspected)
                    {
                        targetPerson.inspected = true;
                        targetPerson.greeting.isVisible = true;
                    }
                    else if (hasTargetDoor())
                    {
                        atDoor = true;
                        timeLeft--;
                    }
                    else if (hasTargetGuard())
                    {
                        targetGuard.inspected = true;
                    }
                }
                else
                {
                    animator.SetState(AnimationState.WALK);
                    SoundPlayer.Instance.playWalkSound();

                    if (destinationX < position.X + width / 2)
                    {
                        position.X -= walkspeed;
                    }
                    else
                    {
                        position.X += walkspeed;
                    }
                }
            }
        }

        //Param: Location of a mouse click
        //Return: Did the given click fall on the player?
        //Note: To be used with PDA functionality
        public void CheckMouseClick(int x, int y)
        {
            if (x >= this.position.X &&
                    y >= this.position.Y &&
                    x <= this.position.X + this.width &&
                    y <= this.position.Y + this.height)
            {
                Console.WriteLine("PLAYER CLICKED");
            }
        }

        //Param: A String to be checked against what's in knowledge
        //Return: Whether the Response has already been added
        public Boolean hasInfo(String r)
        {
            foreach (String str in knowledge)
            {
                if (r == str)
                {
                    return true;
                }
            }
            return false;
        }

        //Param: Person, to be moved to and interacted with,
        //or null to remove existing target person
        public void setTargetPerson(Person per)
        {
            targetPerson = per;
            if (targetPerson != null)
            {
                setTargetItem(null);
                setTargetDoor(null);
                setTargetGuard(null);
                setDestination(targetPerson.position);
            }
        }

        //Param: Item, to be moved to and interacted with,
        //or null to remove existing target item
        public void setTargetItem(Item itm)
        {
            targetItem = itm;
            if (targetItem != null)
            {
                setTargetPerson(null);
                setTargetDoor(null);
                setTargetGuard(null);
                setDestination(targetItem.position);
            }
        }

        public void setTargetGuard(ButtonGuard bg)
        {
            targetGuard = bg;
            if (targetGuard != null)
            {
                setTargetItem(null);
                setTargetPerson(null);
                setTargetDoor(null);
                setDestination(targetGuard.position);
            }
        }

        //Param: Door to travel through
        //or null to remove existing target Door
        public void setTargetDoor(Door d)
        {
            targetDoor = d;
            if (targetDoor != null)
            {
                setTargetItem(null);
                setTargetPerson(null);
                setTargetGuard(null);
                setDestination(targetDoor.position);
            }
        }


        public MouseType CheckMouseOver(int x, int y)
        {
            if (CheckMousePosition(x, y))
            {
                return MouseType.PLAYER;
            }
            return MouseType.BACKGROUND;
        }

        public Boolean CheckMousePosition(int x, int y)
        {
            return (x >= this.position.X &&
                    y >= this.position.Y &&
                    x <= this.position.X + this.width &&
                    y <= this.position.Y + this.height);
        }

        //Return: Is targetItem defined?
        public Boolean hasTargetItem()
        {
            return (targetItem != null);
        }

        //Currently working if a person only has one fact/response
        public void AddKnowledge(String source)
        {
            if (source == "item")
            {
                knowledge.Insert(knowledge.Count - 1, targetItem.fact);
            }
            else if (source == "person")
            {
                knowledge.Insert(knowledge.Count - 1,targetPerson.responses[0].dialog);
            }
            else
            {
                foreach (string s in knowledge)
                {
                    if (s == source)
                    {
                        return;
                    }
                }
                knowledge.Insert(knowledge.Count - 1, source);
            }
        }

        //Return: Is targetDoor defined?
        public Boolean hasTargetDoor()
        {
            return (targetDoor != null);
        }

        //Return: Is targetPerson defined?
        public Boolean hasTargetPerson()
        {
            return (targetPerson != null);
        }

        public Boolean hasTargetGuard()
        {
            return (targetGuard != null);
        }

        //Called when the Player enters a new room through a Door
        //Note: Questionable logic, to be revisited
        public void RoomReset(Door d)
        {

            if ((d.fromRoom.bunkerIndex == 0 && d.toRoom.bunkerIndex == 5) ||
                (d.fromRoom.bunkerIndex == 1 && d.toRoom.bunkerIndex == 0) ||
                (d.fromRoom.bunkerIndex == 2 && d.toRoom.bunkerIndex == 1) ||
                (d.fromRoom.bunkerIndex == 3 && d.toRoom.bunkerIndex == 2) ||
                (d.fromRoom.bunkerIndex == 4 && d.toRoom.bunkerIndex == 3) ||
                (d.fromRoom.bunkerIndex == 5 && d.toRoom.bunkerIndex == 4) ||
                (d.fromRoom.bunkerIndex == 6 && d.toRoom.bunkerIndex == 8) ||
                (d.fromRoom.bunkerIndex == 7 && d.toRoom.bunkerIndex == 6) ||
                (d.fromRoom.bunkerIndex == 8 && d.toRoom.bunkerIndex == 7))
            {
                position.X = d.toRoom.doors[1].position.X-50;
                faceLeft = true;
                destinationX = position.X;
                targetItem = null;
                targetDoor = null;
            }
            else if ((d.fromRoom.bunkerIndex == 0 && d.toRoom.bunkerIndex == 1) ||
                      (d.fromRoom.bunkerIndex == 1 && d.toRoom.bunkerIndex == 2) ||
                      (d.fromRoom.bunkerIndex == 2 && d.toRoom.bunkerIndex == 3) ||
                      (d.fromRoom.bunkerIndex == 3 && d.toRoom.bunkerIndex == 4) ||
                      (d.fromRoom.bunkerIndex == 4 && d.toRoom.bunkerIndex == 5) ||
                      (d.fromRoom.bunkerIndex == 5 && d.toRoom.bunkerIndex == 0) ||
                      (d.fromRoom.bunkerIndex == 6 && d.toRoom.bunkerIndex == 7) ||
                      (d.fromRoom.bunkerIndex == 7 && d.toRoom.bunkerIndex == 8) ||
                      (d.fromRoom.bunkerIndex == 8 && d.toRoom.bunkerIndex == 6))
            {
                position.X = d.toRoom.doors[0].position.X;
                faceLeft = false;
                destinationX = position.X;
                targetItem = null;
                targetDoor = null;
            }
            else
            {
                position.X = d.toRoom.doors[2].position.X;
                faceLeft = true;
                destinationX = position.X;
                targetItem = null;
                targetDoor = null;
            }

        }


        public void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            animator.Draw(spritebatch, position, DrawConstants.PLAYER_LAYER, faceLeft);

            if (!ending)
            {
                spritebatch.DrawString(Textures.item_font, timeLeft + " minutes to midnight", new Vector2(600, 0), Color.Red, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
        }

        internal void LoadContent(ContentManager contentManager)
        {
            animator = new AnimationManager();
            Texture2D texture = contentManager.Load<Texture2D>("Characters//president");
            List<AnimationFrame> frames = new List<AnimationFrame>();
            frames.Add(new AnimationFrame(new Rectangle(50, 0, 50, 210), 60));
            frames.Add(new AnimationFrame(new Rectangle(154, 0, 51, 210), 60));
            animator.Set(AnimationState.IDLE, new Animation(texture, frames));
            frames = new List<AnimationFrame>();
            frames.Add(new AnimationFrame(new Rectangle(285, 225, 60, 210), 8));
            frames.Add(new AnimationFrame(new Rectangle(345, 225, 60, 210), 8));
            frames.Add(new AnimationFrame(new Rectangle(405, 225, 55, 210), 8));
            frames.Add(new AnimationFrame(new Rectangle(230, 225, 50, 210), 6));
            animator.Set(AnimationState.WALK, new Animation(texture, frames, false, false));
            animator.SetState(AnimationState.IDLE);
        }
    }
}
