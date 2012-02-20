using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Delve_Engine.DataTypes;

namespace Two_Weeks_in_a_Laundromat
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region CONSTANTS
        const int DEFAULT_WINDOW_WIDTH = 1280;
        const int DEFAULT_WINDOW_HEIGHT = 720;
        const bool AA_DEFAULT_ON = true;
        const bool FULLSCREEN_DEFAULT_ON = false;
        #endregion

        #region INPUT
        InputInfo inputInfo;
        #endregion

        #region GAMESTATE_MANAGEMENT
        GameStates gameState;
        #endregion

        #region MENU
        MainMenu menu;
        #endregion

        #region WORLD
        World world;
        #endregion

        #region FONTS
        SpriteFont mainFont;
        #endregion

        #region MISC
        Random WOLOLO;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            if (FULLSCREEN_DEFAULT_ON)
            {
                graphics.IsFullScreen = FULLSCREEN_DEFAULT_ON;
                graphics.PreferredBackBufferWidth =
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight =
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
                graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
            }

            graphics.PreferMultiSampling = AA_DEFAULT_ON;
            graphics.ApplyChanges();

            gameState = GameStates.menu;

            Window.Title = "Two Weeks in a Laundromat";

            WOLOLO = new Random();
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainFont = Content.Load<SpriteFont>("Fonts/mainFont");

            menu = new MainMenu(GraphicsDevice, "TWO WEEKS");
            menu.Load(Content);

            inputInfo = new InputInfo();
            inputInfo.oldKBDState = Keyboard.GetState();
            inputInfo.oldMouseState = Mouse.GetState();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void loadWorld()
        {
            gameState = GameStates.loading;
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);
            inputInfo.oldMouseState = Mouse.GetState();
            world = new World();
            world.Load(Content, GraphicsDevice);
            gameState = GameStates.game;
        }

        protected override void Update(GameTime gameTime)
        {
            #region INPUT_UPDATE
            inputInfo.curKBDState = Keyboard.GetState();
            inputInfo.curMouseState = Mouse.GetState();
            inputInfo.timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            #endregion

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case GameStates.menu:
                    menu.handleInput(ref inputInfo);
                    menu.Update(gameTime);

                    if (menu.Flag == MenuFlags.quit)
                        this.Exit();
                    else if (menu.Flag == MenuFlags.startGame)
                        loadWorld();

                    break;
                case GameStates.loading:
                    break;
                case GameStates.game:
                    world.handleInput(ref inputInfo);
                    world.Update(gameTime);
                    break;
            }

            #region INPUT_UPDATE
            // Do not update the mouse state. The one we have here is the center of the screen.
            //inputInfo.oldMouseState = inputInfo.curMouseState;
            inputInfo.oldKBDState = inputInfo.curKBDState;
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightCyan);

            switch (gameState)
            {
                case GameStates.menu:
                    menu.Draw(spriteBatch);
                    break;
                case GameStates.loading:
                    spriteBatch.Begin();
                    Vector2 stringSize = mainFont.MeasureString("Loading...");
                    spriteBatch.DrawString(mainFont, "Loading...", new Vector2(GraphicsDevice.Viewport.Width / 2 - stringSize.X,
                        GraphicsDevice.Viewport.Height / 2 - stringSize.Y), Color.Black);
                    spriteBatch.End();
                    break;
                case GameStates.game:
                    world.Draw();
#if DEBUG
                    spriteBatch.Begin();
                    spriteBatch.DrawString(mainFont, "LR Rot: " + world.MPlayer.LeftRightRot, new Vector2(1, 0), Color.White);
                    spriteBatch.DrawString(mainFont, "UD Rot: " + world.MPlayer.UpDownRot, new Vector2(1, 20), Color.White);
                    spriteBatch.DrawString(mainFont, " X Pos: " + world.MPlayer.Position.X, new Vector2(1, 40), Color.White);
                    spriteBatch.DrawString(mainFont, " Z Pos: " + world.MPlayer.Position.Z, new Vector2(1, 60), Color.White);
                    spriteBatch.DrawString(mainFont, "  Door: " + world.lastOpened, new Vector2(1, 80), Color.White);
                    spriteBatch.End();
#endif
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
