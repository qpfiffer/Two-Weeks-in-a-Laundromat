using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Delve_Engine.Utilities;
using Delve_Engine.Interfaces;
using Delve_Engine.DataTypes;

namespace Delve_Engine.Menu
{
    public class Menu : IInputHandler
    {
        #region MENU_STUFF
        string title;
        int selectedEntry;
        List<MenuItem> menuItems;
        SpriteFont mFont;

        public MenuFlags Flag { get; set; }
        #endregion

        #region SCENE_TO_DRAW
        MatrixDescriptor currentMatrices;
        SoundEffect bgSound;
        BasicEffect globalEffect;
        Vector3 cameraPos;
        float leftRightRot, upDownRot;
        RasterizerState rState;
        GraphicsDevice gDevice;
        #endregion

        #region Protected_Stuff
        protected List<MetaModel> models;
        protected Color clearColor;
        #endregion

        public Menu(GraphicsDevice gDevice, string title)
        {
            #region Menu_Stuff
            this.selectedEntry = 0;
            this.gDevice = gDevice;
            this.title = title;
            this.menuItems = new List<MenuItem>();
            this.Flag = MenuFlags.normal;
            this.clearColor = Color.LightCyan;
            #endregion
            #region Scenery
            cameraPos = new Vector3(0.0f, 1.0f, 3.0f);

            models = new List<MetaModel>();

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;

            leftRightRot = 0.0f;
            upDownRot = MathHelper.ToRadians(-10.0f);
            #endregion
        }

        public virtual void Load(ContentManager gManager)
        {
            #region Basic_Effect
            globalEffect = ModelUtil.CreateGlobalEffect(gDevice);
            #endregion
            #region Menu_Stuff
            mFont = gManager.Load<SpriteFont>("Fonts/mainFont");

            // Set up entries:
            MenuItem begin = new MenuItem("Begin");
            MenuItem quit = new MenuItem("Quit");

            begin.doWork += beginFunc;
            quit.doWork += quitFunc;

            menuItems.Add(begin);
            menuItems.Add(quit);
            #endregion
            #region Setup3D
            // Add scenery when you override this function.
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, cameraPos, ref currentMatrices);
            currentMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f),
                gDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
            currentMatrices.world = Matrix.CreateTranslation(Vector3.Zero);

            globalEffect.View = currentMatrices.view;
            globalEffect.Projection = currentMatrices.proj;
            globalEffect.World = currentMatrices.world;
            #endregion
        }

        public void Update(GameTime gTime)
        {
        }

        public void handleInput(ref InputInfo info)
        {
            #region ENTRY_SELECTION
            if (info.curKBDState.IsKeyDown(Keys.Down) &&
                info.oldKBDState.IsKeyUp(Keys.Down))
            {
                selectedEntry++;
            }

            if (info.curKBDState.IsKeyDown(Keys.Up) &&
                info.oldKBDState.IsKeyUp(Keys.Up))
            {
                selectedEntry--;
            }

            if (selectedEntry < 0)
                selectedEntry = menuItems.Count - 1;
            else if (selectedEntry >= menuItems.Count)
                selectedEntry = 0;
            #endregion

            if (info.curKBDState.IsKeyDown(Keys.Enter) &&
                info.oldKBDState.IsKeyUp(Keys.Enter))
            {
                menuItems[selectedEntry].GetItDone(this);
            }
        }

        public virtual void Draw(SpriteBatch sBatch)
        {
            gDevice.DepthStencilState = DepthStencilState.Default;
            gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColor, 1.0f, 0);
            gDevice.RasterizerState = rState;

            foreach (MetaModel mModel in models)
            {
                ModelUtil.DrawModel(mModel, globalEffect);
            }

            // Draw the title of the menu:
            sBatch.Begin();
            Vector2 stringSize = mFont.MeasureString(this.title);
            Vector2 menuTitleCenter = new Vector2((gDevice.Viewport.Width / 2) - (stringSize.X / 2), (gDevice.Viewport.Height / 4) - (stringSize.Y / 2));
            sBatch.DrawString(mFont, this.title, menuTitleCenter, Color.DarkBlue);

            // Draw the menu items:
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedEntry)
                    sBatch.DrawString(mFont, menuItems[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 16.0f)), Color.Blue);
                else
                    sBatch.DrawString(mFont, menuItems[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 16.0f)), Color.DarkBlue);
            }
            sBatch.End();
        }

        void beginFunc(object o, EventArgs e)
        {
            this.Flag = MenuFlags.startGame;
        }

        void quitFunc(object o, EventArgs e)
        {
            this.Flag = MenuFlags.quit;
        }
    }
}
