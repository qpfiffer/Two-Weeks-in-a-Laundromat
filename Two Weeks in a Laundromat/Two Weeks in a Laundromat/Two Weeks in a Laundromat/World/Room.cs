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
        protected bool shouldDrawBoundingBoxes = true;
        protected List<MetaModel> pieces;
        protected List<GameObject> things;
        protected const float tileSize = 4.0f;
        protected const float tileHalf = tileSize / 2.0f;
        protected Vector3 dimensions;
        // Remember, roomCenter is actually the top left!
        protected Vector3 roomCenter;

        protected MetaModel doorframe, wall, ceiling, floor;

        public bool ShouldDrawBoundingBoxes {
            get
            {
                return shouldDrawBoundingBoxes;
            }

            set
            {
                this.shouldDrawBoundingBoxes = value;
                if (things != null)
                {
                    foreach (GameObject thing in things)
                    {
                        thing.ShouldDrawBoundingBoxes = shouldDrawBoundingBoxes;
                    }
                }
            }
        }
        public List<MetaModel> AllMetas
        {
            get { return pieces; }
        }

        public List<GameObject> AllGOs
        {
            get { return things; }
        }

        /// <summary>
        /// Use this one only if you're a subclass doing some dirty shit.
        /// </summary>
        protected Room()
        {
            pieces = new List<MetaModel>();
            things = new List<GameObject>();
            ShouldDrawBoundingBoxes = true;
        }

        public Room(ref Vector3 dimensions, ref Vector3 roomCenter)
        {
            pieces = new List<MetaModel>();
            things = new List<GameObject>();
            ShouldDrawBoundingBoxes = true;
            this.dimensions = dimensions;
            this.roomCenter = roomCenter;
        }

        public void Draw(GraphicsDevice gDevice, ref MatrixDescriptor cMatrices, Vector3 playerPos)
        {
            foreach (GameObject g in things)
            {
                g.Draw(ref cMatrices, ref playerPos);
            }

            foreach (MetaModel m in pieces)
            {
                m.Shader.Parameters["World"].SetValue(cMatrices.world);
                m.Shader.Parameters["View"].SetValue(cMatrices.view);
                m.Shader.Parameters["Projection"].SetValue(cMatrices.proj);
                m.Shader.Parameters["LightPos"].SetValue(playerPos);

                ModelUtil.DrawModel(m);
#if DEBUG
                if (m.BBoxes != null && shouldDrawBoundingBoxes)
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

        public virtual void Update(GameTime gTime)
        {
            foreach (GameObject g in things)
            {
                g.Update(gTime);
            }
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            floor = new MetaModel();
            floor.Position = Vector3.Zero;
            floor.Rotation = Vector3.Zero;
            floor.model = gManager.Load<Model>("Models/Segments/floor");
            floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            floor.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            wall = new MetaModel();
            wall.Position = Vector3.Zero;
            wall.Rotation = Vector3.Zero;
            wall.model = gManager.Load<Model>("Models/Segments/wall");
            wall.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            wall.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            ceiling = new MetaModel();
            ceiling.Position = Vector3.Zero;
            ceiling.Rotation = Vector3.Zero;
            ceiling.model = gManager.Load<Model>("Models/Segments/ceiling");
            ceiling.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            ceiling.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            doorframe = new MetaModel();
            doorframe.Position = Vector3.Zero;
            doorframe.Rotation = Vector3.Zero;
            doorframe.model = gManager.Load<Model>("Models/Segments/doorframe");
            doorframe.Texture = gManager.Load<Texture2D>("Textures/Segments/textureless");
            doorframe.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            setupPieces();
        }

        /// <summary>
        /// Call this to create the room.
        /// </summary>
        protected void setupPieces()
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
                            newWall.Position = new Vector3((x * tileSize) - tileHalf, topLeft.Y + (i * 8), (z * tileSize));
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
                            newWall.Position = new Vector3((x * tileSize) + tileHalf, topLeft.Y + (i * 8), (z * tileSize));
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
                            newWall.Position = new Vector3((x * tileSize), topLeft.Y + (i * 8), (z * tileSize) - tileHalf);
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
                            newWall.Position = new Vector3((x * tileSize), topLeft.Y + (i * 8), (z * tileSize) + tileHalf);
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
