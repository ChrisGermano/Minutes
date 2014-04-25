using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace MinutesToMidnight
{
    public class PDA
    {
        private Texture2D m_frame;
        public Dictionary<string, PDAScreen> screens;
        public String active_screen;
        private Vector2 screen_position;
        private Vector2 pda_position;
        private Vector2 button_location;
        public bool Active;
        private Texture2D Background;
        private Dictionary<string, Button> buttons;
        private Button close_button;
        private string active_button;
        private Dictionary<string, Action> button_actions;
        private int height;
        private int width;
        private float scalar;
        private string mouse_over = "";

        private List<Person> pda_people;
        //For TimelineScreen
        public List<TextOverlay> knowledge;

        private Vector2 m_leftButtonXY;

        private Vector2 m_right;
        
        public PDA(List<Person> people)
        {
            height = 400;
            width = 400;
            pda_position = new Vector2(Game1.screen_size.X/2 - width/2, 40);
            screen_position = pda_position;
            button_location = screen_position;
            pda_people = people;

        }

        public void Close()
        {
            this.Active = false;
        }

        public void Open()
        {
            this.Active = true;
        }

        private void SetScreenMap() {
            SetScreen("map");
        }

        private void SetScreenTimeline()
        {
            SetScreen("timeline");
        }

        private void SetScreenBios()
        {
            SetScreen("bios");
        }

        private void SetScreen(String s)
        {
            active_screen = s;
            active_button = s;
        }

        public void Draw(SpriteBatch spritebatch, GameTime gameTime, int mouse_x, int mouse_y)
        {
            if (this.Active)
            {
                spritebatch.Draw(Background, pda_position, null, Color.White, 0f, new Vector2(0,0), 1.1f, SpriteEffects.None, DrawConstants.PDA_BACKGROUND_LAYER);
               
                screens[active_screen].Draw(spritebatch, gameTime);
                close_button.Draw(spritebatch);
                foreach (string key in buttons.Keys)
                {
                    buttons[key].Draw(spritebatch);
                    
                }
                if (mouse_over == "close")
                {
                    Vector2 size = Textures.item_font.MeasureString(mouse_over);
                    spritebatch.DrawString(Textures.item_font, mouse_over, new Vector2(mouse_x - size.X, mouse_y - size.Y), Color.Black, 0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, DrawConstants.TEXT_LAYER);
                }
            }
        }

        public Boolean isMouseOver(int mouse_x, int mouse_y)
        {
            return (mouse_x >= screen_position.X &&
                    mouse_y >= screen_position.Y &&
                    mouse_x <= (screen_position.X + width) &&
                    mouse_y <= (screen_position.Y + height));
        }

        public void CheckMousePosition(int mouse_x, int mouse_y)
        {
            mouse_over = "";
            foreach (string key in buttons.Keys)
            {
                if (buttons[key].mouseOver(mouse_x, mouse_y))
                {
                    mouse_over = key;
                    return;
                }
            }
            if (close_button.mouseOver(mouse_x, mouse_y))
            {
                mouse_over = close_button.name;
            }
        }

        public void CheckMousePositionClick(int mouse_x, int mouse_y)
        {
            string buttonFound = "";
            foreach (string key in buttons.Keys)
            {
               if (buttons[key].mouseOver(mouse_x, mouse_y))
                {
                    buttonFound = key;
                    buttons[key].active = true;
                    buttons[key].ButtonAction();
                    break;
                }
            }
            if (buttonFound != "")
            {
                foreach (string key in buttons.Keys)
                {
                    if (key != buttonFound)
                    {
                        buttons[key].active = false;
                    }
                }
            }
            if (close_button.mouseOver(mouse_x, mouse_y))
            {
                close_button.ButtonAction();
            }

        }

        internal void AddKnowledge(DialogInfo p, Boolean isFact)
        {
            screens["timeline"].AddKnowledge(p, isFact);
        }

        public void LoadContent(ContentManager contentManager)
        {
            this.Background = contentManager.Load<Texture2D>("PDA//Frame");
            scalar = 1.1f;
            height = (int)(this.Background.Height * scalar);
            width = (int)(this.Background.Width * scalar);

            pda_position = new Vector2(Game1.screen_size.X / 2 - width / 2, Game1.screen_size.Y / 2 - height / 2);
            button_location = new Vector2(pda_position.X + width / 10 - 22, pda_position.Y + (height * 0.8f));
            screen_position = new Vector2(pda_position.X + width / 10, pda_position.Y + (height * 0.125f));

            screens = new Dictionary<string, PDAScreen>();
            // m_screens.Add(new OptionsScreen());
            int screenheight = (int)(0.65 * height);
            int screenwidth = (int)(0.78 * width);

            screens.Add("timeline", new TimelineScreen(screen_position, screenheight, screenwidth, scalar));
            screens.Add("map", new MapScreen(screen_position, screenheight, screenwidth, scalar));
            screens.Add("bios", new BiosScreen(screen_position, screenheight, screenwidth, pda_people, scalar));
            SetScreenMap();
            buttons = new Dictionary<string, Button>();
            button_actions = new Dictionary<string, Action>();
            int buttonwidth = (int)((screenwidth*scalar) / 3);
            close_button = new Button(pda_position, 20, 20, Close, "close");
            close_button.LoadContent(contentManager); 
            buttons.Add("map", new Button(button_location, buttonwidth, 35, SetScreenMap, "map", scalar, 0));
            buttons.Add("timeline", new Button(button_location, buttonwidth, 35, SetScreenTimeline, "timeline", scalar, 2));
            buttons.Add("bios", new Button(button_location, buttonwidth, 35, SetScreenBios, "bios", scalar, 1));
            foreach (string key in buttons.Keys)
            {
                buttons[key].LoadContent(contentManager);
                if (key != active_button && key != "close")
                {
                    buttons[key].active = false;
                }
            }
            foreach (string key in screens.Keys)
            {
                screens[key].LoadContent(contentManager);
            }
        }

        internal void setActiveRoom(int p)
        {
            screens["map"].setActiveRoom(p);
        }
    }
}
