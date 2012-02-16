﻿using System;
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
        #region 3dStuff
        protected Effect alternateShader = null;
        protected bool shouldDrawBoundingBoxes = false;
        protected MetaModel doorframe, wall, ceiling, floor, door;
        #endregion
        #region WorldStuff
        protected List<MetaModel> pieces;
        protected List<GameObject> things;
        #endregion
        #region Constants
        protected const float tileSize = 4.0f;
        protected const float tileHalf = tileSize / 2.0f;
        #endregion
        #region RoomShit
        protected Vector3 dimensions;
        // Remember, roomCenter is actually the top left!
        protected Vector3 roomCenter;
        protected List<DoorData> doors;
        #endregion        
        #region Properties
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
        #endregion

        /// <summary>
        /// Use this one only if you're a subclass doing some dirty shit.
        /// </summary>
        protected Room()
        {
            pieces = new List<MetaModel>();
            things = new List<GameObject>();
            doors = new List<DoorData>();
        }

        public Room(ref Vector3 dimensions, ref Vector3 roomCenter)
        {
            pieces = new List<MetaModel>();
            things = new List<GameObject>();
            doors = new List<DoorData>();
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
            Effect shaderToLoad = null;

            if (alternateShader == null)
                shaderToLoad = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            else
                shaderToLoad = alternateShader;

            floor = new MetaModel();
            floor.model = gManager.Load<Model>("Models/Segments/floor");
            floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            floor.Shader = shaderToLoad;

            wall = new MetaModel();
            wall.model = gManager.Load<Model>("Models/Segments/wall");
            wall.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            wall.Shader = shaderToLoad;

            ceiling = new MetaModel();
            ceiling.model = gManager.Load<Model>("Models/Segments/ceiling");
            ceiling.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            ceiling.Shader = shaderToLoad;

            doorframe = new MetaModel();
            doorframe.model = gManager.Load<Model>("Models/Segments/doorframe");
            doorframe.Texture = gManager.Load<Texture2D>("Textures/Segments/textureless");
            doorframe.Shader = shaderToLoad;

            door = new MetaModel();
            door.model = gManager.Load<Model>("Models/segments/door");
            door.Texture = gManager.Load<Texture2D>("Textures/Segments/textureless");
            door.Shader = shaderToLoad;

            setupPieces();
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            this.Load(gManager, gDevice);
        }

        /// <summary>
        /// Call this to create the room.
        /// </summary>
        protected void setupPieces()
        {
            // Find the top left portion of the room:
            Vector3 topLeft = new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z);
            Vector3 worldSpaceSize = new Vector3(roomCenter.X + dimensions.X, roomCenter.Y + dimensions.Y, roomCenter.Z + dimensions.Z);

            int zIncrement = 0;
            int xIncrement = 0;
            for (float z = topLeft.Z; z < (topLeft.Z + dimensions.Z); z++)
            {
                xIncrement = 0;
                for (float x = topLeft.X; x < (topLeft.X + dimensions.X); x++)
                {
                    #region XWalls
                    #region Wall8
                    if (x == topLeft.X)
                    {
                        bool doorHere = false;
                        // Loop through all the doors we have in this place
                        foreach (DoorData dd in doors)
                        {
                            // If this isn't my wall skip it
                            if (dd.myWall != WallSide.West)
                                continue;
                            else if (dd.myTilePos == zIncrement)
                            {
                                // Is this where the door is?
                                doorHere = true;
                            }
                        }
                        // Loop through all heights as well.
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            // Only place the door on the ground level.
                            if (doorHere)
                            {
                                doorHere = false;
                                continue;
                            }

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
                    #endregion
                    #region Wall2
                    else if (x == worldSpaceSize.X - 1)
                    {
                        bool doorHere = false;
                        // Loop through all the doors we have in this place
                        foreach (DoorData dd in doors)
                        {
                            // If this isn't my wall skip it
                            if (dd.myWall != WallSide.East)
                                continue;
                            else if (dd.myTilePos == zIncrement)
                            {
                                // Is this where the door is?
                                doorHere = true;
                            }
                        }
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            // Only place the door on the ground level.
                            if (doorHere)
                            {
                                doorHere = false;
                                continue;
                            }

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
                    #endregion

                    #region ZWalls
                    #region Wall1
                    if (z == topLeft.Z)
                    {
                        bool doorHere = false;
                        // Loop through all the doors we have in this place
                        foreach (DoorData dd in doors)
                        {
                            // If this isn't my wall skip it
                            if (dd.myWall != WallSide.North)
                                continue;
                            else if (dd.myTilePos == xIncrement)
                            {
                                // Is this where the door is?
                                doorHere = true;
                            }
                        }

                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            if (doorHere)
                            {
                                doorHere = false;
                                continue;
                            }
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
                    #endregion
                    #region Wall4
                    else if (z == worldSpaceSize.Z - 1)
                    {
                        bool doorHere = false;
                        // Loop through all the doors we have in this place
                        foreach (DoorData dd in doors)
                        {
                            // If this isn't my wall skip it
                            if (dd.myWall != WallSide.South)
                                continue;
                            else if (dd.myTilePos == xIncrement)
                            {
                                // Is this where the door is?
                                doorHere = true;
                            }
                        }
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            if (doorHere)
                            {
                                doorHere = false;
                                continue;
                            }

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

                    xIncrement++;
                }
                zIncrement++;
            }
        }
    }
}
