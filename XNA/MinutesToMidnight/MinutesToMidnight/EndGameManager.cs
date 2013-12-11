using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MinutesToMidnight.Animating;

namespace MinutesToMidnight
{
    public class EndGameManager
    {
        //Whether the player decided to launch
        private Boolean launched;

        //What the player learned
        private List<TextOverlay> pdaInfo;

        //All potential responses in the game
        List<Response> totalResponses;

        //Counter of correctly identified info
        int accuracy;

        //Percentage of obtained information
        float completion;

        float fadeout_counter;
        float fadeout_time;
        bool paper_draw;
        bool the_end;

        Dictionary<string, Texture2D> newspapers;
        Texture2D newspaper;

        //Final percentage of accurate completion

        Vector2 paper_position;
        Vector2 zoom_button_position;
        Button launch;
        Button DoNotLaunch;
        
        AnimationManager animator;
        bool Active;
        bool game_over;
        bool fade_in;

        public EndGameManager()
        {
            Active = false;
        }

        public void Initialize(List<TextOverlay> know, List<Response> allRes)
        {
            paper_position = new Vector2(25, Game1.screen_size.Y);
            pdaInfo = (know == null) ? new List<TextOverlay>() : know;
            totalResponses = allRes;
            getResults();
            zoom_button_position = new Vector2(Game1.screen_size.X / 2 - 100, 0);
            launch = new Button(new Vector2(zoom_button_position.X + 30, zoom_button_position.Y + 280), 160, 80, Launch, "launch");
            DoNotLaunch = new Button(new Vector2(zoom_button_position.X + 50, zoom_button_position.Y + 120), 120, 160, Close, "dontlaunch");
            game_over = false;
            fadeout_counter = 0;
            fadeout_time = 300;
            paper_draw = false;
            Active = true;
        }


        public void getResults()
        {
            completion = (pdaInfo.Count / totalResponses.Count);
            completion %= 0.1f;

            accuracy = 0;

            foreach (TextOverlay to in pdaInfo)
            {
                foreach (Response r in totalResponses)
                {
                    if (to.text == r.dialog)
                    {
                        if (to.isColored)
                        {
                            if (to.drawCol == Color.Green)
                            {
                                if (r.verity == VERITY.true_opinion)
                                {
                                    accuracy++;
                                }
                                else if (r.verity == VERITY.false_opinion)
                                {
                                    accuracy--;
                                }
                            }
                            else if (to.drawCol == Color.Red)
                            {
                                if (r.verity == VERITY.false_opinion)
                                {
                                    accuracy++;
                                }
                                else if (r.verity == VERITY.true_opinion)
                                {
                                    accuracy--;
                                }
                            }
                        }
                    }
                }
            }

        }
        //function called when button is closed
        public void Close()
        {
            animator.SetState(AnimationState.CLOSE);
            game_over = true;
            if (completion > 0.5 && (accuracy > (pdaInfo.Count/3)))
            {
                newspaper = newspapers["informedinaction"];
            }
            else
            {
                newspaper = newspapers["uninformedinaction"];
            }
        }

        //function called when button is launched
        public void Launch()
        {
            animator.SetState(AnimationState.PRESS);
            game_over = true;
            if (completion > 0.5 && (accuracy > (pdaInfo.Count / 3)))
            {
                newspaper = newspapers["informedlaunch"];
            }
            else
            {
                newspaper = newspapers["uninformedlaunch"];
            }
        }

        public void ClickCheck(int mouse_x, int mouse_y)
        {
            if(launch.mouseOver(mouse_x, mouse_y))
            {
                launch.ButtonAction();
            } else if(DoNotLaunch.mouseOver(mouse_x, mouse_y))
            {
                DoNotLaunch.ButtonAction();
            }
        }


        public void Update()
        {
            if (game_over)
            {
                if (the_end)
                {
                    if (fade_in)
                    {
                        fadeout_counter--;

                        if (fadeout_counter <= 0)
                        {
                            fade_in = false;
                        }
                    }
                    else
                    {
                        fadeout_counter++;
                        if (fadeout_counter == fadeout_time)
                        {
                            Game1.EXIT = true;
                        }
                    }
                }
                else if (!paper_draw)
                {
                    fadeout_counter++;
                    if (fadeout_counter >= fadeout_time)
                    {
                        paper_draw = true;
                    }
                }
                else
                {
                    if (paper_position.Y + newspaper.Height * 0.75f > Game1.screen_size.Y / 2)
                    {
                        fadeout_counter--;
                        if (fadeout_counter <= 0)
                        {
                            fadeout_counter = 0;
                        }
                    } 
                    else if(paper_position.Y + newspaper.Height * 0.75f < 0)
                    {
                        the_end = true;
                        fade_in = true;
                        fadeout_counter = fadeout_time;
                    }
                    else
                    {
                        fadeout_counter++;
                    }

                    paper_position = new Vector2(paper_position.X, paper_position.Y - 0.7f);
                }
            }
            
        }

        public void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            if (game_over)
            {
                float x = fadeout_counter / fadeout_time;
                if (the_end)
                {
                    Vector2 size = Textures.item_font.MeasureString("the end");
                    spritebatch.DrawString(Textures.item_font, "The End", new Vector2(Game1.screen_size.X / 2 - size.X / 2, Game1.screen_size.Y / 2 - size.Y / 2), Color.White * (1 - x));
                } 
                else if (paper_draw)
                {
                    spritebatch.Draw(newspaper, paper_position, null, Color.White * (1 - x), 0f, new Vector2(0,0), .75f, SpriteEffects.None, 0f);
                }
                else
                {
                    
                    animator.Draw(spritebatch, zoom_button_position, 1f, false, 1f, Color.White * (1-x));
                }
            }
            else
            {
                animator.Draw(spritebatch, zoom_button_position, 1f);
            }
        }

        public void LoadContent(ContentManager cm)
        {
            animator = new AnimationManager();
            Texture2D sheet = cm.Load<Texture2D>("roomAssets//button_seq");
            Texture2D push_sheet = cm.Load<Texture2D>("roomAssets//button_seq_push");
            List<AnimationFrame> frames = new List<AnimationFrame>();

            frames.Add(new AnimationFrame(new Rectangle(0, 0, 230, 500), 1000));
            animator.Set(AnimationState.IDLE, new Animation(sheet, frames));

            frames = new List<AnimationFrame>();
            frames.Add(new AnimationFrame(new Rectangle(0, 0, 230, 500), 20));
            frames.Add(new AnimationFrame(new Rectangle(230, 0, 230, 500), 20));
            frames.Add(new AnimationFrame(new Rectangle(460, 0, 230, 500), 20));
            animator.Set(AnimationState.CLOSE, new Animation(sheet, frames, true));

            frames = new List<AnimationFrame>();
            frames.Add(new AnimationFrame(new Rectangle(230, 0, 230, 500), 30));
            frames.Add(new AnimationFrame(new Rectangle(0, 0, 230, 500), 30));
            animator.Set(AnimationState.PRESS, new Animation(push_sheet, frames, true)); 
            animator.SetState(AnimationState.IDLE);

            newspapers = new Dictionary<string, Texture2D>();
            newspapers.Add("informedlaunch", cm.Load<Texture2D>("gameAssets//Informed_Launch"));
            newspapers.Add("uninformedlaunch", cm.Load<Texture2D>("gameAssets//Uninformed_Launch"));
            newspapers.Add("informedinaction", cm.Load<Texture2D>("gameAssets//Informed_Inaction"));
            newspapers.Add("uninformedinaction", cm.Load<Texture2D>("gameAssets//Uninformed_Inaction"));
        }


        internal string mouseOver(int mouse_x, int mouse_y)
        {
            if (!game_over)
            {
                if (launch.mouseOver(mouse_x, mouse_y))
                {
                    return "launch";
                }
                if (DoNotLaunch.mouseOver(mouse_x, mouse_y))
                {
                    return "dontlaunch";
                }
            }
            
            return "";
        }

        internal bool IsActive()
        {
            return Active;
        }
    }
}
