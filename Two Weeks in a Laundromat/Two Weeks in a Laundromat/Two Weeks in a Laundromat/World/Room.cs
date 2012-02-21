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
        #region Misc
        protected ContentManager gManager;
        protected GraphicsDevice gDevice;
        protected bool loaded = false;
        //private float frames = 0f;
        #endregion
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
        protected List<DoorData> absentWalls;
        protected string roomTheme = string.Empty;
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

        /// <summary>
        /// What kind of textures to use for the room.
        /// </summary>
        public string Theme
        {
            get { return roomTheme; }
            set { roomTheme = value; }
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
        protected Room(string theme)
        {
            pieces = new List<MetaModel>();
            things = new List<GameObject>();
            doors = new List<DoorData>();
            absentWalls = new List<DoorData>();
            this.roomTheme = theme;
        }

        public Room(ref Vector3 dimensions, ref Vector3 roomCenter, string theme)
        {
            pieces = new List<MetaModel>();
            things = new List<GameObject>();
            doors = new List<DoorData>();
            absentWalls = new List<DoorData>();
            this.dimensions = dimensions;
            this.roomCenter = roomCenter;
            this.roomTheme = theme;
        }

        public void addDoor(ref DoorData newDoor)
        {
            this.doors.Add(newDoor);
        }

        public void removeWallSection(ref DoorData toRemove)
        {
            this.absentWalls.Add(toRemove);
        }

        public void addRandomDoor(Random wololo)
        {
            if (loaded)
                throw new Exception("Room already loaded. This won't do anything.");

            DoorData toAdd;

            Vector3 checkVector = dimensions;
            if (checkVector.X < 0f)
            {
                checkVector.X *= -1.0f;
            }
            if (checkVector.Z < 0f)
            {
                checkVector.Z *= -1.0f;
            }

            switch (wololo.Next(4))
            {
                case 0:
                    toAdd = new DoorData(WallSide.North, wololo.Next((int)checkVector.X));
                    break;
                case 1:
                    toAdd = new DoorData(WallSide.West, wololo.Next((int)checkVector.Z));
                    break;
                case 2:
                    toAdd = new DoorData(WallSide.East, wololo.Next((int)checkVector.Z));
                    break;
                case 3:
                    toAdd = new DoorData(WallSide.South, wololo.Next((int)checkVector.X));
                    break;
                default:
                    toAdd = new DoorData(WallSide.West, wololo.Next((int)checkVector.Z));
                    break;
            }

            this.doors.Add(toAdd);
        }

        public void Draw(GraphicsDevice gDevice, ref MatrixDescriptor cMatrices, Vector3 playerPos)
        {
            //frames += 0.1f;
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

                //m.Shader.Parameters["screenVector"].SetValue(new Vector2((float)Math.Cos(frames) / 2,
                //    (float)Math.Sin(frames) / 2));

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

        protected void preloadPieces()
        {
            Effect shaderToLoad = null;

            if (alternateShader == null)
                shaderToLoad = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            else
                shaderToLoad = alternateShader;

            if (roomTheme == string.Empty)
                roomTheme = "Concrete";

            floor = new MetaModel();
            floor.model = gManager.Load<Model>("Models/Segments/floor");
            floor.Texture = gManager.Load<Texture2D>("Textures/Segments/" + roomTheme + "/floor");
            floor.Shader = shaderToLoad;

            wall = new MetaModel();
            wall.model = gManager.Load<Model>("Models/Segments/wall");
            wall.Texture = gManager.Load<Texture2D>("Textures/Segments/" + roomTheme + "/wall");
            wall.Shader = shaderToLoad;

            ceiling = new MetaModel();
            ceiling.model = gManager.Load<Model>("Models/Segments/ceiling");
            ceiling.Texture = gManager.Load<Texture2D>("Textures/Segments/" + roomTheme + "/ceiling");
            ceiling.Shader = shaderToLoad;

            doorframe = new MetaModel();
            doorframe.model = gManager.Load<Model>("Models/Segments/doorframe");
            doorframe.Texture = gManager.Load<Texture2D>("Textures/Segments/" + roomTheme + "/doorframe");
            doorframe.Shader = shaderToLoad;

            door = new MetaModel();
            door.model = gManager.Load<Model>("Models/Segments/bone_door");
            door.Texture = gManager.Load<Texture2D>("Textures/Segments/" + roomTheme + "/door");
            door.Shader = shaderToLoad;
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gManager = gManager;
            this.gDevice = gDevice;
            preloadPieces();
            setupPieces();
            loaded = true;
        }

        public virtual void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            this.Load(gManager, gDevice);
        }

        /// <summary>
        /// Call this to create the room.
        /// </summary>
        protected virtual void setupPieces()
        {
            // Find the top left portion of the room:
            Vector3 topLeft = new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z);

            #region DirectionToCreate
            if (dimensions.X < 0f)
            {
                // Make positive
                dimensions.X *= -1.0f;
                topLeft.X -= dimensions.X * tileSize - tileSize;
            }
            if (dimensions.Y <= 0f)
            {
                // Why the fuck do we have a negative or zero height?
                dimensions.Y = 1.0f;
            }
            if (dimensions.Z < 0f)
            {
                // Make positive
                dimensions.Z *= -1.0f;
                topLeft.Z -= dimensions.Z * tileSize - tileSize;
            }
            #endregion

            Vector3 worldSpaceSize = new Vector3(topLeft.X + (dimensions.X * tileSize),
                topLeft.Y + dimensions.Y, topLeft.Z + (dimensions.Z * tileSize));

            int zIncrement = 0;
            int xIncrement = 0;
            for (float z = topLeft.Z; z < worldSpaceSize.Z; z += tileSize)
            {
                xIncrement = 0;
                for (float x = topLeft.X; x < worldSpaceSize.X; x += tileSize)
                {
                    #region XWalls
                    #region Wall8
                    if (x == topLeft.X)
                    {
                        bool doorHere = false;
                        bool wallAbsenceHere = false;
                        DoorData foundADoor = new DoorData(WallSide.West, zIncrement);
                        // See if we have a door here
                        if (doors.Contains(foundADoor)) 
                        {
                            doorHere = true;
                        }

                        if (absentWalls.Contains(foundADoor))
                        {
                            doorHere = false;
                            wallAbsenceHere = true;
                        }
                        // Loop through all heights as well.
                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            // Only place the door on the ground level.
                            if (doorHere)
                            {
                                doorHere = false;

                                Vector3 rotVector = Vector3.Zero;
                                Vector3 posVector = new Vector3(x - tileHalf, topLeft.Y + (i * 8), z);
                                addNewDoor(ref rotVector, ref posVector, ref foundADoor);

                                continue;
                            }
                            else if (wallAbsenceHere)
                            {
                                continue;
                            }

                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3(x - tileHalf, topLeft.Y + (i * 8), z);
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
                    if (x == worldSpaceSize.X - tileSize)
                    {
                        bool doorHere = false;
                        bool wallAbsenceHere = false;
                        
                        DoorData foundADoor = new DoorData(WallSide.East, zIncrement);
                        if (doors.Contains(foundADoor))
                        {
                            doorHere = true;
                        }

                        if (absentWalls.Contains(foundADoor))
                        {
                            doorHere = false;
                            wallAbsenceHere = true;
                        }

                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            // Only place the door on the ground level.
                            if (doorHere)
                            {
                                doorHere = false;

                                Vector3 rotVector = new Vector3(0, MathHelper.ToRadians(180.0f), 0f);
                                Vector3 posVector = new Vector3(x + tileHalf, topLeft.Y + (i * 8), z);
                                addNewDoor(ref rotVector, ref posVector, ref foundADoor);

                                continue;
                            }
                            else if (wallAbsenceHere)
                            {
                                continue;
                            }

                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3(x + tileHalf, topLeft.Y + (i * 8), z);
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
                        bool wallAbsenceHere = false;

                        DoorData foundADoor = new DoorData(WallSide.North, xIncrement);
                        if (doors.Contains(foundADoor))
                        {
                            doorHere = true;
                        }

                        if (absentWalls.Contains(foundADoor))
                        {
                            doorHere = false;
                            wallAbsenceHere = true;
                        }

                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            if (doorHere)
                            {
                                doorHere = false;

                                Vector3 rotVector = new Vector3(0, MathHelper.ToRadians(-90.0f), 0f);
                                Vector3 posVector = new Vector3(x, topLeft.Y + (i * 8), z - tileHalf);
                                addNewDoor(ref rotVector, ref posVector, ref foundADoor);

                                continue;
                            }
                            else if (wallAbsenceHere)
                            {
                                continue;
                            }
                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3(x, topLeft.Y + (i * 8), z - tileHalf);
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
                    if (z == worldSpaceSize.Z - tileSize)
                    {
                        bool doorHere = false;
                        bool wallAbsenceHere = false;

                        DoorData foundADoor = new DoorData(WallSide.South, xIncrement);
                        if (doors.Contains(foundADoor))
                        {
                            doorHere = true;
                        }
                        
                        if (absentWalls.Contains(foundADoor))
                        {
                            doorHere = false;
                            wallAbsenceHere = true;
                        }

                        for (int i = 0; i < dimensions.Y; i++)
                        {
                            if (doorHere)
                            {
                                doorHere = false;

                                Vector3 rotVector = new Vector3(0, MathHelper.ToRadians(90.0f), 0f);
                                Vector3 posVector = new Vector3(x, topLeft.Y + (i * 8), z + tileHalf);
                                addNewDoor(ref rotVector, ref posVector, ref foundADoor);

                                continue;
                            }
                            else if (wallAbsenceHere)
                            {
                                continue;
                            }

                            MetaModel newWall = new MetaModel();
                            newWall.Position = new Vector3(x, topLeft.Y + (i * 8), z + tileHalf);
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
                    floorPiece.Position = new Vector3(x, topLeft.Y, z);
                    floorPiece.Rotation = Vector3.Zero;
                    floorPiece.model = floor.model;
                    floorPiece.Texture = floor.Texture;
                    floorPiece.Shader = floor.Shader;
                    ModelUtil.UpdateBoundingBoxes(ref floorPiece);
                    pieces.Add(floorPiece);

                    MetaModel ceilingPiece = new MetaModel();
                    ceilingPiece.Position = new Vector3(x, topLeft.Y + ((dimensions.Y-1) * 8), z);
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

        /// <summary>
        /// If a door is discovered you can create a new GameObject pretty easily with this.
        /// </summary>
        /// <param name="rotVector">How the door is rotated.</param>
        /// <param name="posVector">Where the door is</param>
        /// <param name="foundADoor">The MetaData to associate with the door.</param>
        private void addNewDoor(ref Vector3 rotVector, ref Vector3 posVector, ref DoorData foundADoor)
        {
            MetaModel newDoorFrame = new MetaModel();
            newDoorFrame.Position = posVector;
            newDoorFrame.Rotation = rotVector;
            newDoorFrame.model = this.doorframe.model;
            newDoorFrame.Texture = doorframe.Texture;
            newDoorFrame.Shader = doorframe.Shader;
            ModelUtil.UpdateBoundingBoxes(ref newDoorFrame);
            pieces.Add(newDoorFrame);

            MetaModel newDoorMeta = new MetaModel();
            newDoorMeta.Position = posVector;
            newDoorMeta.Rotation = rotVector;
            newDoorMeta.model = door.model;
            newDoorMeta.Texture = door.Texture;
            newDoorMeta.Shader = door.Shader;
            ModelUtil.UpdateBoundingBoxes(ref newDoorMeta);

            Door newDoor = new Door(ref newDoorMeta, foundADoor, gDevice, this);
            this.things.Add(newDoor);
        }
    }
}
