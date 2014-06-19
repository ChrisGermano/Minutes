using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinutesToMidnight.Animating;

namespace MinutesToMidnight
{
    class ConversationManager
    {
        Player player;
        Person person;
        private bool closeConvo;
        bool waitingforquestion;
        TextOverlay current_message;
        public Button continue_convo;
        public bool Active;
        PlayerQuestions questions;
        TextOverlay questionHeader;

        DialogInfo InfoToAdd;

        public ConversationManager(Player plyr, Person prsn)
        {
            if (plyr != null && prsn != null)
            {
                player = plyr;
                person = prsn;
                continue_convo = new Button(new Vector2(100, Game1.screen_size.Y - 50), 100, 50, ContinueConvo, "continue");
                continue_convo.SetTexture(Textures.convo_continue_button);
                continue_convo.InitAnimate();
                List<AnimationFrame> buttonframes = new List<AnimationFrame>();
                buttonframes.Add(new AnimationFrame(new Rectangle(0,0, 33, 30), 20));
                buttonframes.Add(new AnimationFrame(new Rectangle(33, 0, 33, 30), 20));
                continue_convo.SetAnimation(AnimationState.IDLE, buttonframes);
                current_message = person.greeting;
                questions = new PlayerQuestions(plyr.knowledge);
                questionHeader = new TextOverlay("Ask " + person.name, new Vector2(10, DrawConstants.CONVERSATION_OVERLAY_HEIGHT));
                Active = true;
                closeConvo = false;
            }
            else
            {
                Active = false;
            }
        }


        public void CheckClick(int x, int y)
        {
            if (!waitingforquestion)
            {
                if (continue_convo.mouseOver(x, y))
                {
                    continue_convo.ButtonAction();
                }
            }
            else
            {
                foreach (TextOverlay t in questions.getQuestions())
                {
                    if (t.isMouseOver(new Vector2(x, y)))
                    {
                        current_message = person.GetResponse(t.text.Substring(0,t.text.Length - 1));
                        waitingforquestion = false;
                    }
                }
            }
        }

        public void Update()
        {
            if (waitingforquestion)
            {
                person.StopSpeaking();
            }
            else
            {
                person.StartSpeaking();
            }
        }

        public bool HasInfo()
        {
            return InfoToAdd != null;
        }

        public DialogInfo GetInfo()
        {
            DialogInfo info = InfoToAdd;
            InfoToAdd = null;
            return info;
        }

        private void ContinueConvo()
        {
            if (!waitingforquestion && current_message != null)
            {
                if (current_message == person.GetResponse("goodbye"))
                {
                    this.Active = false;
                    this.closeConvo = true;

                }
                else
                {
                    if (current_message != person.greeting && current_message != person.generic && current_message != person.generic_answer)
                    {
                        if (current_message.response != null)
                        {
                            InfoToAdd = new DialogInfo(current_message.text, person.name, current_message.response);
                            if (current_message.response != "")
                            {
                                questions.AddQuestion(current_message.response);
                            }
                        }
                    }
                }
                waitingforquestion = true;
            }
            else 
            {
                current_message = null;

                person.StartSpeaking();
                if (player.knowledge.Count == 0)
                {
                    waitingforquestion = false;
                    if (current_message == person.greeting)
                    {
                        current_message = person.generic;
                    }
                    else if (current_message == person.generic)
                    {
                        this.Active = false;
                    }
                }
                if (current_message == person.GetResponse("goodbye"))
                {
                    this.Active = false;
                    person.StopSpeaking();
                }
            }
        }

        public void CloseConvo()
        {
            closeConvo = false;
            this.Active = false;
            person.StopSpeaking();
        }

        public bool ShouldClose()
        {
            return closeConvo;
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            float yval = questions.getYValue();
            float newyval = (Game1.screen_size.Y - (yval + Textures.text_background.Height + 20) + Textures.text_background.Height) / Textures.text_background.Height;
            sb.Draw(Textures.text_background, new Vector2(0, yval), null, Color.White, 0, new Vector2(0, 0), new Vector2(1, newyval), SpriteEffects.None, DrawConstants.CONVERSATION_BACKGROUND_LAYER);

            if (!waitingforquestion)
            {
                current_message.Draw(sb);
                continue_convo.Draw(sb);
            }
            else
            {
                questionHeader.position = new Vector2(questionHeader.position.X, yval + 1);
                questionHeader.Draw(sb);
                questions.Draw(sb, gt);
            }
        }


        internal bool CheckMouseOver(int mouse_x, int mouse_y)
        {
            if (waitingforquestion)
            {
                return questions.CheckMouseOver(mouse_x, mouse_y);
            }
            return false;
        }
    }

    class PlayerQuestions
    {
        List<TextOverlay> questions;
        float new_y_value;
        int y_mod;
        int y_diff;
        public PlayerQuestions(List<String> knwlge)
        {
            questions = new List<TextOverlay>();
            if (knwlge.Count > 0)
            {
                y_diff = 26;
                y_mod = y_diff;
                new_y_value = DrawConstants.CONVERSATION_OVERLAY_HEIGHT;

                if (y_diff * (knwlge.Count + 2) + DrawConstants.CONVERSATION_OVERLAY_HEIGHT > Game1.screen_size.Y)
                {
                    new_y_value = Game1.screen_size.Y - (y_diff * (knwlge.Count + 2)) - 20;
                }

                foreach (string k in knwlge)
                {
                    string question_mod = "?";
                    if (k == "goodbye")
                    {
                        question_mod = "!";
                    }
                    questions.Add(new TextOverlay(k + question_mod, new Vector2(40, y_mod + new_y_value)));
                    y_mod += y_diff;
                }
            }
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            foreach (TextOverlay t in questions)
            {
                t.Draw(sb);
            }
        }
        
        public List<TextOverlay> getQuestions()
        {
            return questions;
        }

        internal void AddQuestion(string p)
        {
            p += "?";
            foreach (TextOverlay t in questions)
            {
                if (t.text == p)
                {
                    return;
                }
            }
            TextOverlay goodbye = questions[questions.Count -1];
            questions.Insert(questions.Count - 1, new TextOverlay(p, new Vector2(40, goodbye.position.Y)));
            goodbye.position = new Vector2(goodbye.position.X, goodbye.position.Y + y_diff);
            questions[questions.Count - 1] = goodbye;
            if (y_diff * questions.Count + new_y_value > Game1.screen_size.Y - 40)
            {
                foreach (TextOverlay t in questions)
                {
                    t.position = new Vector2(t.position.X, t.position.Y - y_diff);
                }
                new_y_value = Game1.screen_size.Y - (y_diff * (questions.Count + 1));
            }
        }

        public float getYValue()
        {
            return new_y_value;
        }

        internal bool CheckMouseOver(int mouse_x, int mouse_y)
        {
            bool mouseover = false;
            foreach (TextOverlay t in questions)
            {
                if (t.isMouseOver(new Vector2(mouse_x, mouse_y)))
                {
                    t.drawCol = Color.Black;
                    mouseover = true;
                }
                else
                {
                    t.drawCol = Color.Gray;
                }
            }
            return mouseover;
        }
    }
}
