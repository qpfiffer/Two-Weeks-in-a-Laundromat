using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Delve_Engine.Interfaces;
using Delve_Engine.DataTypes;
using Delve_Engine.Utilities;

namespace Delve_Engine.World
{
    public class World : IInputHandler
    {
        #region Player
        protected Player mainPlayer;
        public Player MPlayer { get { return mainPlayer; } }
        #endregion

        #region GameStuff
        protected ContentManager gManager;
        protected GraphicsDevice gDevice;
        protected BasicEffect globalEffect;
        protected RasterizerState rState;
#if DEBUG
        bool releaseMouseToggle; // Used to release the mouse from the window
#endif
        #endregion

        #region World
        protected Song bgMusic;
        // Any random models that are needed:
        protected List<MetaModel> modelsToDraw;
        #endregion

        public World()
        {
            Vector3 playerPos = new Vector3(0, Player.chestHeight, 3.0f);
            Vector2 playerRot = new Vector2(0.0f, 0.0f);

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;

            mainPlayer = new Player(ref playerPos, ref playerRot, gDevice);
            modelsToDraw = new List<MetaModel>();
            releaseMouseToggle = false;
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
            this.gManager = gManager;

            mainPlayer.Position = new Vector3(0, Player.chestHeight, 0);

            Setup3D(gDevice);
        }

        private void Setup3D(GraphicsDevice gDevice)
        {
            // Get a unified global effect from the model util thing:
            globalEffect = ModelUtil.CreateGlobalEffect(gDevice);
            // We create a matrixDescriptor here becuase we can't set properties of properties:
            MatrixDescriptor cMatrices = mainPlayer.Matrices;
            ModelUtil.UpdateViewMatrix(mainPlayer.UpDownRot, mainPlayer.LeftRightRot, mainPlayer.Position,
                ref cMatrices);
            // Set the pieces of out matrix descriptor:
            cMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75.0f),
                gDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
            cMatrices.world = Matrix.CreateTranslation(Vector3.Zero);

            // Update the global effect:
            globalEffect.View = cMatrices.view;
            globalEffect.World = cMatrices.world;
            globalEffect.Projection = cMatrices.proj;

            // Finally place it where it needs to be:
            mainPlayer.Matrices = cMatrices;
        }

        private void collideMove(float amount, Vector3 moveVector)
        {
            // TODO: Collision, with octrees
            Vector3 finalVector = moveVector * amount;
            mainPlayer.addToCameraPosition(ref finalVector);
        }

        public virtual void Update(GameTime gTime)
        {
            globalEffect.View = mainPlayer.Matrices.view;
            globalEffect.World = mainPlayer.Matrices.world;
            globalEffect.Projection = mainPlayer.Matrices.proj;

            mainPlayer.Update(gTime);
        }

        public void handleInput(ref InputInfo info)
        {
            if (info.curKBDState.IsKeyDown(Keys.E) &&
                info.oldKBDState.IsKeyUp(Keys.E))
            {
                // TODO: Object interaction.
            }

#if DEBUG
            if (info.curKBDState.IsKeyDown(Keys.N) &&
                info.oldKBDState.IsKeyUp(Keys.N))
            {
                mainPlayer.NoClip = !mainPlayer.NoClip;
            }

            if (info.curKBDState.IsKeyDown(Keys.M) &&
                info.oldKBDState.IsKeyUp(Keys.M))
            {
                releaseMouseToggle = !releaseMouseToggle;
            }

            if (info.curKBDState.IsKeyDown(Keys.G) &&
                info.oldKBDState.IsKeyUp(Keys.G))
            {
                mainPlayer.rotateCameraAboutYAxisPoint(mainPlayer.rotationTarget, 90.0f);
            }

            if (info.curKBDState.IsKeyDown(Keys.R) &&
                info.oldKBDState.IsKeyUp(Keys.R))
            {
                mainPlayer.rotateEnabled = !mainPlayer.rotateEnabled;
            }

            if (info.curKBDState.IsKeyDown(Keys.F) &&
                info.oldKBDState.IsKeyUp(Keys.F))
            {
                // TODO: Toggle fog on objects that have it enabled.
            }
#endif


            if (info.curMouseState != info.oldMouseState && !releaseMouseToggle)
            {
                int xDelta = info.curMouseState.X - info.oldMouseState.X;
                int yDelta = info.curMouseState.Y - info.oldMouseState.Y;

                Point deltas = new Point(xDelta, yDelta);
                mainPlayer.rotateCamera(ref deltas, info.timeDifference);

                Mouse.SetPosition(gDevice.Viewport.Width / 2, gDevice.Viewport.Height / 2);
                MatrixDescriptor cMatrices = mainPlayer.Matrices;
                ModelUtil.UpdateViewMatrix(mainPlayer.UpDownRot, mainPlayer.LeftRightRot,
                    mainPlayer.Position, ref cMatrices);
                mainPlayer.Matrices = cMatrices;
            }

            Vector3 moveVector = Vector3.Zero;
            if (info.curKBDState.IsKeyDown(Keys.W))
            {
                moveVector.Z -= 1;
            }
            else if (info.curKBDState.IsKeyDown(Keys.S))
            {
                moveVector.Z += 1;
            }

            if (info.curKBDState.IsKeyDown(Keys.A))
            {
                moveVector.X -= 1;
            }
            else if (info.curKBDState.IsKeyDown(Keys.D))
            {
                moveVector.X += 1;
            }

            if (moveVector != Vector3.Zero)
            {
                collideMove(info.timeDifference, moveVector);
            }
        }

        public void Draw()
        {
            gDevice.DepthStencilState = DepthStencilState.Default;
            gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.LightCyan, 1.0f, 0);
            gDevice.RasterizerState = rState;

            foreach (MetaModel model in modelsToDraw)
            {
                if (model.Shader == null)
                {
                    ModelUtil.DrawModel(model, globalEffect);
                }
                else
                {
                    ModelUtil.DrawModel(model);
                }
            }
        }
    }
}
