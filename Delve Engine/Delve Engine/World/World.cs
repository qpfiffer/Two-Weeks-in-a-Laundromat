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
        protected bool frameIsClear = false;
#if DEBUG
        bool releaseMouseToggle; // Used to release the mouse from the window
#endif
        #endregion

        #region World
        protected Song bgMusic;
        // Any random models that are needed:
        private List<MetaModel> modelsToDraw;
        protected Random WOLOLO;
        protected bool boundingBoxesDraw = false;
        protected MatrixDescriptor cMatrices;
        protected List<BoundingBox> collisionBoxes;
        #endregion

        public World()
        {
            Vector3 playerPos = new Vector3(0, Player.playerHeight, 0);
            Vector2 playerRot = Vector2.Zero;

            rState = new RasterizerState();
            rState.FillMode = FillMode.Solid;
            rState.CullMode = CullMode.CullCounterClockwiseFace;
            rState.ScissorTestEnable = true;

            mainPlayer = new Player(ref playerPos, ref playerRot, null);
            modelsToDraw = new List<MetaModel>();
#if DEBUG
            releaseMouseToggle = false;
#endif
            collisionBoxes = new List<BoundingBox>();
            WOLOLO = new Random();
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
            this.gManager = gManager;

            mainPlayer.GDevice = gDevice;

            Setup3D(gDevice);
        }

        /// <summary>
        /// To add a new model to the world, add it via this function.
        /// It performs some housekeeping to make sure everything is correct.
        /// </summary>
        /// <param name="toAdd">The model to add.</param>
        protected void addNewModel(ref MetaModel toAdd)
        {
            try
            {
                ModelUtil.UpdateBoundingBoxes(ref toAdd);
            }
            catch (Exception e)
            {
                // Probably no bounding box data. Thats okay.
                System.Diagnostics.Debug.WriteLine(e);
            }
            modelsToDraw.Add(toAdd);
        }

        private void Setup3D(GraphicsDevice gDevice)
        {
            // Get a unified global effect from the model util thing:
            globalEffect = ModelUtil.CreateGlobalEffect(gDevice);
            // We create a matrixDescriptor here becuase we can't set properties of properties:
            cMatrices = mainPlayer.Matrices;
            ModelUtil.UpdateViewMatrix(mainPlayer.UpDownRot, mainPlayer.LeftRightRot, mainPlayer.Position,
                ref cMatrices);
            // Set the pieces of out matrix descriptor:
            cMatrices.proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60.0f),
                gDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
            cMatrices.world = Matrix.CreateTranslation(Vector3.Zero);

            // Update the global effect:
            globalEffect.View = cMatrices.view;
            globalEffect.World = cMatrices.world;
            globalEffect.Projection = cMatrices.proj;

            // Finally place it where it needs to be:
            mainPlayer.Matrices = cMatrices;
        }

        private void collideMove(float amount, Vector3 moveVector, List<BoundingBox> bboxes)
        {
            Vector3 finalVector = moveVector * amount;
            Vector3 posToTest = mainPlayer.examineFuturePos(ref finalVector);

            BoundingSphere tempSphere = new BoundingSphere(posToTest, mainPlayer.chestSphere.Radius);

            if (!mainPlayer.NoClip)
            {
                foreach (BoundingBox bbox in bboxes)
                {
                    if (tempSphere.Intersects(bbox))
                    {
                        return;
                    }
                }
            }

            // If we got here then we didnt hit anything.
            mainPlayer.addToCameraPosPrecomputed(ref posToTest);
        }

        public virtual void Update(GameTime gTime)
        {
            mainPlayer.Update(gTime);
        }

        public virtual void handleInput(ref InputInfo info)
        {
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

            // Haha, totally forgot I put this in here. Does a nice little demo rotate think around
            // 0,0,0. Neat little thing.
            if (info.curKBDState.IsKeyDown(Keys.R) &&
                info.oldKBDState.IsKeyUp(Keys.R))
            {
                mainPlayer.rotateEnabled = !mainPlayer.rotateEnabled;
            }
#endif

#if DEBUG
            if (info.curMouseState != info.oldMouseState && !releaseMouseToggle)
#else
            if (info.curMouseState != info.oldMouseState)
#endif
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
                mainPlayer.HeadBobbing = true;
                collideMove(info.timeDifference, moveVector, collisionBoxes);
            }
            else
            {
                mainPlayer.HeadBobbing = false;
            }
        }

        protected void clearBuffer()
        {
            cMatrices.view = mainPlayer.Matrices.view;
            cMatrices.world = mainPlayer.Matrices.world;
            cMatrices.proj = mainPlayer.Matrices.proj;

            // Do this if some other function has already cleared the frame and drawn stuff. We don't want to
            // hide it.
            if (!frameIsClear)
            {
                gDevice.DepthStencilState = DepthStencilState.Default;
                gDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                gDevice.RasterizerState = rState;
                frameIsClear = true;
            }
        }

        public virtual void Draw()
        {            
            clearBuffer();

            foreach (MetaModel model in modelsToDraw)
            {
                if (model.Shader == null)
                {
                    ModelUtil.DrawModel(model, globalEffect);
                }
                else
                {
                    model.Shader.Parameters["World"].SetValue(cMatrices.world);
                    model.Shader.Parameters["View"].SetValue(cMatrices.view);
                    model.Shader.Parameters["Projection"].SetValue(cMatrices.proj);
                    model.Shader.Parameters["LightPos"].SetValue(this.mainPlayer.Position);
                    ModelUtil.DrawModel(model);
                }

#if DEBUG
                if (model.BBoxes != null && boundingBoxesDraw)
                {
                    foreach (BoundingBox bBox in model.BBoxes)
                    {
                        BoundingBoxRenderer.Render(bBox,
                            gDevice,
                            cMatrices.view,
                            cMatrices.proj,
                            Color.Red);
                    }
                }
#endif
            }
            frameIsClear = false;
        }
    }
}
