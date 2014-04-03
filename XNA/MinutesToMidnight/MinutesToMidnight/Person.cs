using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MinutesToMidnight.Animating;
using System.Runtime.Serialization;

namespace MinutesToMidnight
{

    [DataContract]
    [Serializable]
    public class Person
    {
        public AnimationManager animator;


        //List of facts/opinions useful to the Player
        public List<Response> responses;
        public Dictionary<string, TextOverlay> responseOverlays;
        
        [DataMember(Name = "texturename", IsRequired = true)]
        public string texturename;
        public Texture2D sheet;

        [DataMember(Name = "overlayname", IsRequired = true)]
        public string overlayname;
        public Texture2D overlay_texture;

        [DataMember(Name = "name", IsRequired = true)]
        public string name;

        public bool mouseOver = false;

        [DataMember(Name = "role", IsRequired = true)]
        public ROLE role = ROLE.civilian;

        [DataMember(Name = "greetingString", IsRequired = true)]
        private String greetingString;
        public TextOverlay greeting;

        [DataMember(Name = "genericString", IsRequired = true)]
        private String genericString;
        public TextOverlay generic;

        public  TextOverlay generic_answer;
        [DataMember(Name = "guard", IsRequired = true)]
        private int guard;
        public Boolean isGuard;

        public Vector2 position;
        public Vector2 overlayPosition;
        public int width;
        public int height;
        public int head_height;


        public int overlayWidth;
        public int overlayHeight;
        public Boolean inspected;
        public Boolean speaking;

        public String fact;

        //Construct with the Person's name, position, greeting text, generic text, and
        //list of String pieces of information
        public Person()
        {
            width = 50;
            height = 250;
        }

        public Person(String personName, Vector2 pos, String greet, String gen, String[] facts)
        {
            name = personName;
            position = pos;

            responses = new List<Response>();
            responseOverlays = new Dictionary<string, TextOverlay>();
            createResponses(facts);
            createResponseOverlays();
            setDimensions();
            speaking = false;
            inspected = false;

        }

        private void createResponses(String[] facts)
        {
            foreach (String s in facts)
            {
                responses.Add(new Response(s));
            }
        }

        private void createResponseOverlays()
        {
            foreach (Response r in responses)
            {
                responseOverlays.Add(r.prompt, new TextOverlay(r.dialog, new Vector2(20, 400 + (18 * responses.IndexOf(r))), "person", r.responsePrompt));
            }
            responseOverlays.Add("goodbye", new TextOverlay(generic.text,new Vector2(20, 400)));
            Console.WriteLine(name + ": " + responseOverlays.Count);
        }


        private void setDimensions()
        {
            this.width = 50;
            this.height = 200;
        }
        public void Initialize()
        {
            this.width = 50;
            this.height = 200;
            greeting = new TextOverlay(greetingString, new Vector2(20, 400));
            generic = new TextOverlay(genericString, new Vector2(20, 400));
            generic_answer = new TextOverlay("I'm sorry, I don't know anything about that", new Vector2(20, 400));
            responses = new List<Response>();
            responseOverlays = new Dictionary<string, TextOverlay>();
            isGuard = (guard == 1);
            head_height = 50;
        }
        public void Update()
        {
            if (speaking)
            {
          //      SoundPlayer.Instance.StartTalking();
            }
        }

        public void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            
                if (mouseOver)
                {
                    spritebatch.DrawString(Textures.item_font, "Talk to " + name, new Vector2(5, 0), Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0.1f);
                }
            animator.Draw(spritebatch, position, DrawConstants.PERSON_LAYER);
        }

        public void DrawHeadshot(SpriteBatch spritebatch, GameTime gameTime, Vector2 head_position, float scale)
        {
            spritebatch.Draw(sheet, head_position, new Rectangle(0, 0, 50, 50), Color.White, 0f, new Vector2(0,0), scale, SpriteEffects.None, DrawConstants.PDA_BUTTON_LAYER);
        }
        public List<Response> GetResponses()
        {
            return responses;
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

        public MouseType CheckMouseOver(Vector2 point)
        {
            if (this.CheckMouse(point))
            {
                this.mouseOver = true;
                return MouseType.CHARACTER;
            }
            else
            {
                this.mouseOver = false;
                return MouseType.BACKGROUND;
            }
        }


        public void LoadContent(ContentManager contentManager)
        {
            Random r = new Random();
            animator = new AnimationManager();
            sheet = (name != "") ? contentManager.Load<Texture2D>("Characters//" + texturename) : contentManager.Load<Texture2D>("Characters//" + name);
            List<AnimationFrame> frames = new List<AnimationFrame>();
            int idleframes = r.Next(50, 70);
            frames.Add(new AnimationFrame(new Rectangle(52, 0, 50, 210), idleframes));
            frames.Add(new AnimationFrame(new Rectangle(156, 0, 48, 210), idleframes));

            animator.Set(AnimationState.IDLE, new Animation(sheet, frames));
            frames = new List<AnimationFrame>();
            frames.Add(new AnimationFrame(new Rectangle(0, 0, 51, 210), 10));
            frames.Add(new AnimationFrame(new Rectangle(52, 0, 51, 210), 10));
            animator.Set(AnimationState.SPEAK, new Animation(sheet, frames));

            frames = new List<AnimationFrame>();
            frames.Add(new AnimationFrame(new Rectangle(0, 0, 51, 51), idleframes));
            frames.Add(new AnimationFrame(new Rectangle(104, 0, 51, 51), idleframes));
            animator.Set(AnimationState.HEADSHOT, new Animation(sheet, frames));
            animator.SetState(AnimationState.IDLE);
        }


        internal void addResponse(ref List<Response> loaded_responses)
        {
            List<Response> temp_list = new List<Response>();
            Response rsp;
            foreach (Response r in loaded_responses)
            {
                if (r.getRole() == this.role)
                {
                    temp_list.Add(r);
                }
            }
            if (temp_list.Count > 0)
            {
                Random rand = new Random();
                int index = rand.Next(0, temp_list.Count - 1 );
                rsp = temp_list[index];
                List<Response> next_responses = new List<Response>(); 
                if(index + 1 < temp_list.Count){
                    Response next;
                    for (int i = index + 1; i < temp_list.Count; i++)
                    {
                        next = temp_list[i];
                        if (next.tietoprevious)
                        {
                            next_responses.Add(next);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
          
                this.responses.Add(rsp);                
                loaded_responses.Remove(rsp);
                foreach (Response r in next_responses)
                {
                    this.responses.Add(r);
                    loaded_responses.Remove(r);
                }                
            }
            this.createResponseOverlays();
        }
        
        internal TextOverlay GetResponse(string prompt)
        {
            if (responseOverlays.ContainsKey(prompt))
            {
                return responseOverlays[prompt];
            }
            else return generic_answer;
        }

        internal void StopSpeaking()
        {
            animator.SetState(AnimationState.IDLE);
            speaking = false;
            SoundPlayer.Instance.StopTalking();
        }

        internal void StartSpeaking()
        {
            animator.SetState(AnimationState.SPEAK);
            speaking = true;
            SoundPlayer.Instance.StartTalking();
        }
    }
}
