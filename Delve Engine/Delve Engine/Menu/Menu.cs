using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
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

        #region Protected_Stuff
        protected RasterizerState rState;
        protected List<MetaModel> models;
        protected Color clearColor;
        protected GraphicsDevice gDevice;
        protected Vector3 cameraPos = Vector3.Zero;
        protected float leftRightRot, upDownRot;
        protected bool bufferCleared = false;
        protected MatrixDescriptor cMatrices;
        protected Song bgMusic;
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
            models = new List<MetaModel>();

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;         
            #endregion
        }

        public virtual void Load(ContentManager gManager)
        {
            #region Menu_Stuff
            mFont = gManager.Load<SpriteFont>("Fonts/softly_now");

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
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, cameraPos, ref cMatrices);
            cMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f),
                gDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
            cMatrices.world = Matrix.CreateTranslation(Vector3.Zero);
            #endregion
        }

        public virtual void Update(GameTime gTime)
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

        protected void clearBuffer()
        {
            gDevice.DepthStencilState = DepthStencilState.Default;
            gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColor, 1.0f, 0);
            gDevice.RasterizerState = rState;

            bufferCleared = true;
        }

        public virtual void Draw(SpriteBatch sBatch)
        {
            if (!bufferCleared)
                clearBuffer();

            // TODO: Draw GameObjects. Probably not necessary.
            foreach (MetaModel m in models)
            {
                // Assuming the shader is not null here.
                // Not going to update these because the camera never moves.
                //m.Shader.Parameters["World"].SetValue(cMatrices.world);
                //m.Shader.Parameters["View"].SetValue(cMatrices.view);
                //m.Shader.Parameters["Projection"].SetValue(cMatrices.proj);
                ModelUtil.DrawModel(m);
            }

            // Draw the title of the menu:
            sBatch.Begin();
            Vector2 stringSize = mFont.MeasureString(this.title);
            Vector2 menuTitleCenter = new Vector2((gDevice.Viewport.Width / 2) - (stringSize.X / 2), (gDevice.Viewport.Height / 4) - (stringSize.Y / 2));
            sBatch.DrawString(mFont, this.title, menuTitleCenter, Color.Gray);

            // Draw the menu items:
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedEntry)
                    sBatch.DrawString(mFont, menuItems[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 26.0f)), Color.White);
                else
                    sBatch.DrawString(mFont, menuItems[i].Text, new Vector2(menuTitleCenter.X, menuTitleCenter.Y + 35.0f + (i * 26.0f)), Color.Gray);
            }
            sBatch.End();

            bufferCleared = false;
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
