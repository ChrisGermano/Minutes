#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.Generic;

#endregion

namespace MinutesToMidnight
{
	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;		
		GameWorld game_world;
        public static Vector2 screen_size;
        public static bool EXIT = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";	            
			
            IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
          

            graphics.PreferredBackBufferHeight = 500;
			graphics.PreferredBackBufferWidth = 800;

            screen_size = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

			graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            // -2- Generate People/items to stuff them into
			// -3- Lock/Modify some Responses, add the "key" responses into item/people pool
			// -4- generate bunker
			// -5- put people in bunker graphics.PreferredBackBufferWidth = 500;

            game_world = new GameWorld();
            base.Initialize();
            
            
            
				
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures.default_room_back = this.Content.Load<Texture2D>("Room//mtmBunkBack");
            Textures.default_player = this.Content.Load<Texture2D>("Characters//mtmDefaultPlayer");
            Textures.default_person = this.Content.Load<Texture2D>("Characters//Agent_Orville");
            Textures.default_pointer = this.Content.Load<Texture2D>("Pointers//mtmDefaultPointer");
            Textures.item_pointer = this.Content.Load<Texture2D>("Pointers//mtmItemPointer");
            Textures.button_pointer = this.Content.Load<Texture2D>("Pointers//mtmLaunchPointer");
            Textures.case_pointer = this.Content.Load<Texture2D>("Pointers//mtmNoLaunchPointer");
            Textures.pda_pointer = this.Content.Load<Texture2D>("Pointers//mtmPDA");

            Textures.interactive_items = this.Content.Load<Texture2D>("Items//interactiveItems");

            Textures.studio_logo = this.Content.Load<Texture2D>("Logos//Potoo_logo");
            Textures.game_logo = this.Content.Load<Texture2D>("Logos//Minutes_logo");

            Textures.pda_map = this.Content.Load<Texture2D>("PDA//mtmPDAMapBG");
            Textures.pda_options = this.Content.Load<Texture2D>("PDA//mtmPDAOptionsBG");
            Textures.pda_timeline = this.Content.Load<Texture2D>("PDA//mtmPDATimelineBG");
            Textures.z_door = this.Content.Load<Texture2D>("roomAssets//mtmZDoor");
            Textures.default_door = this.Content.Load<Texture2D>("roomAssets//mtmDefaultDoor");
            Textures.item_font = this.Content.Load<SpriteFont>("tempFont");
            Textures.pda_font = this.Content.Load<SpriteFont>("PDAFont");
            Textures.text_background = this.Content.Load<Texture2D>("gameAssets//convoBackground");
            Textures.pda_map_button = this.Content.Load<Texture2D>("PDA//mtmPDAMapButton");
            Textures.pda_timeline_button = this.Content.Load<Texture2D>("PDA//mtmPDATimelineButton");
            Textures.pda_bios_button = this.Content.Load<Texture2D>("PDA//mtmPDABiosButton");
            Textures.pda_close_button = this.Content.Load<Texture2D>("PDA//mtmPDACloseButton");
            Textures.convo_continue_button = this.Content.Load<Texture2D>("continue_button");
            game_world.LoadContent(this.Content);

            //TODO: use this.Content to load your game content here 
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || EXIT)
			{
				Exit();
			}
            // TODO: Add your update logic here		
			game_world.Update ();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            //Begin with parameters to make layers work
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            this.game_world.Draw(spriteBatch, gameTime);
            //TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}

