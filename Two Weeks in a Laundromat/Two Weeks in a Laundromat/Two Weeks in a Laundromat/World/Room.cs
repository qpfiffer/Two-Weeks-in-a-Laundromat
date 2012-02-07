using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Delve_Engine.Utilities;
using Delve_Engine.DataTypes;
using Delve_Engine.World;

namespace Two_Weeks_in_a_Laundromat
{
    class Room
    {
        protected List<MetaModel> pieces;

        protected Vector3 dimensions;
        protected Vector3 roomCenter;

        /// <summary>
        /// Use this one only if you're a subclass doing some dirty shit.
        /// </summary>
        protected Room()
        {
            pieces = new List<MetaModel>();
        }

        public Room(ref Vector3 dimensions, ref Vector3 roomCenter)
        {
            pieces = new List<MetaModel>();
            this.dimensions = dimensions;
            this.roomCenter = roomCenter;
        }

        public void Draw(GraphicsDevice gDevice, BasicEffect gEffect, Vector3 playerPos)
        {
            foreach (MetaModel m in pieces)
            {
                m.Shader.Parameters["World"].SetValue(gEffect.World);
                m.Shader.Parameters["View"].SetValue(gEffect.View);
                m.Shader.Parameters["Projection"].SetValue(gEffect.Projection);
                m.Shader.Parameters["LightPos"].SetValue(playerPos);

                ModelUtil.DrawModel(m);
#if DEBUG
                if (m.BBoxes != null)
                {
                    foreach (BoundingBox bBox in m.BBoxes)
                    {
                        BoundingBoxRenderer.Render(bBox,
                            gDevice,
                            m.Shader.Parameters["View"].GetValueMatrix(),
                            m.Shader.Parameters["Projection"].GetValueMatrix(),
                            Color.Red);
                    }
                }
#endif
            }
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            MetaModel floor = new MetaModel();
            floor.Position = Vector3.Zero;
            floor.Rotation = Vector3.Zero;
            floor.model = gManager.Load<Model>("Models/Segments/floor");
            floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            floor.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            MetaModel wall = new MetaModel();
            wall.Position = Vector3.Zero;
            wall.Rotation = Vector3.Zero;
            wall.model = gManager.Load<Model>("Models/Segments/wall");
            wall.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            wall.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            MetaModel ceiling = new MetaModel();
            ceiling.Position = Vector3.Zero;
            ceiling.Rotation = Vector3.Zero;
            ceiling.model = gManager.Load<Model>("Models/Segments/ceiling");
            ceiling.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            ceiling.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            setupPieces(ref floor, ref wall, ref ceiling);
        }

        /// <summary>
        /// Call this to create the room.
        /// </summary>
        private void setupPieces(ref MetaModel floor, ref MetaModel wall, ref MetaModel ceiling)
        {
            // Find the top left portion of the room:
            Vector3 topLeft = new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z);
            Vector3 worldSpaceSize = new Vector3(roomCenter.X + dimensions.X, roomCenter.Y + dimensions.Y, roomCenter.Z + dimensions.Z);

            // TODO: Make it handle multilevel buildings. Should be as easy as adding another loop.
            for (float z = topLeft.Z; z < (topLeft.Z + dimensions.Z); z++)
            {
                for (float x = topLeft.X; x < (topLeft.X + dimensions.X); x++)
                {
                    #region XWalls
                    if (x == topLeft.X)
                    {
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3((x * 4) - 2.0f, topLeft.Y + (i*8), (z * 4));
                            newWall.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
                            newWall.model = wall.model;
                            newWall.Texture = wall.Texture;
                            newWall.Shader = wall.Shader;
                            ModelUtil.UpdateBoundingBoxes(ref newWall);
                            pieces.Add(newWall);
                        }
                    }
                    else if (x == worldSpaceSize.X - 1)
                    {
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3((x * 4) + 2.0f, topLeft.Y + (i * 8), (z * 4));
                            newWall.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
                            newWall.model = wall.model;
                            newWall.Texture = wall.Texture;
                            newWall.Shader = wall.Shader;
                            ModelUtil.UpdateBoundingBoxes(ref newWall);
                            pieces.Add(newWall);
                        }
                    }
                    #endregion

                    #region ZWalls
                    if (z == topLeft.Z)
                    {
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3((x * 4), topLeft.Y + (i * 8), (z * 4) - 2.0f);
                            newWall.Rotation = Vector3.Zero;
                            newWall.model = wall.model;
                            newWall.Texture = wall.Texture;
                            newWall.Shader = wall.Shader;
                            ModelUtil.UpdateBoundingBoxes(ref newWall);
                            pieces.Add(newWall);
                        }
                    }
                    else if (z == worldSpaceSize.Z - 1)
                    {
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3((x * 4), topLeft.Y + (i * 8), (z * 4) + 2.0f);
                            newWall.Rotation = Vector3.Zero;
                            newWall.model = wall.model;
                            newWall.Texture = wall.Texture;
                            newWall.Shader = wall.Shader;
                            ModelUtil.UpdateBoundingBoxes(ref newWall);
                            pieces.Add(newWall);
                        }
                    }
                    #endregion

                    MetaModel floorPiece = new MetaModel();
                    floorPiece.Position = new Vector3((x * 4), topLeft.Y, (z * 4));
                    floorPiece.Rotation = Vector3.Zero;
                    floorPiece.model = floor.model;
                    floorPiece.Texture = floor.Texture;
                    floorPiece.Shader = floor.Shader;
                    ModelUtil.UpdateBoundingBoxes(ref floorPiece);
                    pieces.Add(floorPiece);

                    MetaModel ceilingPiece = new MetaModel();
                    ceilingPiece.Position = new Vector3((x * 4), topLeft.Y + ((dimensions.Y-1) * 8), (z * 4));
                    ceilingPiece.Rotation = Vector3.Zero;
                    ceilingPiece.model = ceiling.model;
                    ceilingPiece.Texture = ceiling.Texture;
                    ceilingPiece.Shader = ceiling.Shader;
                    ModelUtil.UpdateBoundingBoxes(ref ceilingPiece);
                    pieces.Add(ceilingPiece);
                }
            }
        }
    }
}
