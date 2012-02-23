using System;
using System.Collections.Generic;
using System.Text;
using Delve_Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Delve_Engine.DataTypes;
using SkinnedModel;

namespace Delve_Engine.World
{
    public class GameObject
    {
        private const bool ShouldDrawBBoxesDefault = false;
        #region Fields
        protected Vector3 position;
        protected float leftRightRot, upDownRot;
        protected MetaModel metaModel;
        //protected BasicEffect material;
        protected GraphicsDevice gDevice;
        protected List<Vector3> boundingOffsets;
        protected AnimationPlayer animationPlayer;
        protected SkinningData skinningData;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get { return position; }
            set
            {
                metaModel.Position = value;

                if (metaModel.model != null)
                    ModelUtil.UpdateBoundingBoxes(ref metaModel);

                this.position = value;
            }
        }
        public GraphicsDevice GDevice
        {
            get { return gDevice; }
            set { gDevice = value; }
        }
        public float LeftRightRot
        {
            get { return leftRightRot; }
        }
        public float UpDownRot
        {
            get { return upDownRot; }
        }
        public MetaModel Model
        {
            get { return metaModel; }
        }
        //public BasicEffect Material
        //{
        //    get { return material; }
        //}
        public bool ShouldDrawBoundingBoxes
        {
            get;
            set;
        }
        #endregion

        // Commenting this out becuase it is confusing. You should be using one of the other
        // constructors.
        /// <summary>
        /// Creates a blank game object.
        /// </summary>
        //public GameObject(GraphicsDevice gDevice)
        //{
        //    position = Vector3.Zero;
        //    boundingSpheres = new List<BoundingSphere>();
        //    leftRightRot = 0.0f;
        //    upDownRot = 0.0f;
        //}

        /// <summary>
        /// Creates a GameObject that is useful for cameras.
        /// </summary>
        /// <param name="position">Where it should go.</param>
        /// <param name="rotation">How it should be rotated.</param>
        public GameObject(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice)
        {
            this.position = position;
            this.gDevice = gDevice;
            leftRightRot = rotation.X;
            upDownRot = rotation.Y;
            boundingOffsets = new List<Vector3>();
            ShouldDrawBoundingBoxes = ShouldDrawBBoxesDefault;
        }

        /// <summary>
        /// Creates a GameObject that will eventually have a model mesh.
        /// </summary>
        /// <param name="position">Where it should go.</param>
        /// <param name="rotation">How it will be rotated when displayed.</param>
        public GameObject(ref MetaModel newObject, GraphicsDevice gDevice)
        {
            this.metaModel = newObject;
            this.gDevice = gDevice;
            ShouldDrawBoundingBoxes = ShouldDrawBBoxesDefault;
            
            if (((object[])metaModel.model.Tag)[2] is SkinningData)
            {
                skinningData = ((object[])metaModel.model.Tag)[2] as SkinningData;
                animationPlayer = new AnimationPlayer(skinningData);
            }
        }

        public virtual void Load(ContentManager gManager)
        {
            throw new NotImplementedException("This probably shouldn't be called directly.");
        }

        //public void ToggleFog(bool fogEnabled)
        //{
        //    material.FogEnabled = fogEnabled;
        //}

        //public void ToggleFog()
        //{
        //    material.FogEnabled = !material.FogEnabled;
        //}

        public virtual void interactedWith()
        {
            // Do nothing by default.
        }

        public void Draw(ref MatrixDescriptor cMatrices, ref Vector3 playerPos)
        {
            //if (model.Shader != null)
            {
                metaModel.Shader.Parameters["World"].SetValue(cMatrices.world);
                metaModel.Shader.Parameters["View"].SetValue(cMatrices.view);
                metaModel.Shader.Parameters["Projection"].SetValue(cMatrices.proj);
                metaModel.Shader.Parameters["LightPos"].SetValue(playerPos);

                if (animationPlayer != null)
                {
                    ModelUtil.DrawModel(metaModel, animationPlayer);
                }
                else
                {
                    ModelUtil.DrawModel(metaModel);
                }
#if DEBUG
                if (metaModel.BBoxes != null && ShouldDrawBoundingBoxes)
                {
                    foreach (BoundingBox bBox in metaModel.BBoxes)
                    {
                        BoundingBoxRenderer.Render(bBox,
                            gDevice,
                            cMatrices.view,
                            cMatrices.proj,
                            Color.Blue);
                    }
                }
#endif
            }
        }

        public virtual void Update(GameTime gTime)
        {
            if (animationPlayer != null)
            {
                MetaModel m = metaModel;
                Matrix translationMatrix = Matrix.CreateRotationX(m.Rotation.X) * Matrix.CreateRotationY(m.Rotation.Y)
                * Matrix.CreateRotationZ(m.Rotation.Z) * Matrix.CreateTranslation(m.Position);

                animationPlayer.Update(gTime.ElapsedGameTime, true, translationMatrix);
                // This is fucked up. Don't use it yet.
                //ModelUtil.UpdateBoundingBoxes(ref metaModel, animationPlayer);
            }
        }
    }
}
