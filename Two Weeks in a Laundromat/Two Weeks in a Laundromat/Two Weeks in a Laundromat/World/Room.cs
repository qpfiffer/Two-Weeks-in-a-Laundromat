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
        private List<MetaModel> pieces;

        private Vector3 dimensions;
        private Vector3 roomCenter;

        public Room(ref Vector3 dimensions, ref Vector3 roomCenter)
        {
            pieces = new List<MetaModel>();
            this.dimensions = dimensions;
            this.roomCenter = roomCenter;
        }

        public void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            MetaModel floor = new MetaModel();
            floor.Position = Vector3.Zero;
            floor.Rotation = Vector3.Zero;
            floor.model = gManager.Load<Model>("Models/Segments/floor");
            floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            floor.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            MetaModel wall = new MetaModel();
            floor.Position = Vector3.Zero;
            floor.Rotation = Vector3.Zero;
            floor.model = gManager.Load<Model>("Models/Segments/wall");
            floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            floor.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            MetaModel ceiling = new MetaModel();
            //floor.Position = Vector3.Zero;
            //floor.Rotation = Vector3.Zero;
            //floor.model = gManager.Load<Model>("Models/Segments/floor");
            //floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            //floor.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);

            setupPieces(ref floor, ref wall, ref ceiling);
        }

        /// <summary>
        /// Call this to create the room.
        /// </summary>
        private void setupPieces(ref MetaModel floor, ref MetaModel wall, ref MetaModel ceiling)
        {
            // Find the top left portion of the room:
            Vector3 topLeft = new Vector3(roomCenter.X - (dimensions.X / 2.0f), roomCenter.Y - (dimensions.Y / 2.0f), roomCenter.Z - (dimensions.Z / 2.0f));
            Vector3 worldSpaceSize = new Vector3(roomCenter.X + dimensions.X, roomCenter.Y + dimensions.Y, roomCenter.Z + dimensions.Z);

            for (float z = topLeft.Z; z < (topLeft.Z + worldSpaceSize.Z); z++)
            {
                for (float x = topLeft.X; x < (topLeft.X + worldSpaceSize.X); x++)
                {
                    if (x == topLeft.X)
                    {
                        MetaModel newWall = new MetaModel();
                        newWall.Position = new Vector3(x-2.0f, topLeft.Y, z);
                        newWall.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
                        newWall.model = wall.model;
                        newWall.Texture = wall.Texture;
                        newWall.Shader = wall.Shader;
                        ModelUtil.UpdateBoundingBoxes(ref newWall);
                        pieces.Add(newWall);
                    }
                    else if (x == ((topLeft.X + worldSpaceSize.X) - 1))
                    {
                        MetaModel newWall = new MetaModel();
                        newWall.Position = new Vector3(x+2.0f, topLeft.Y, z);
                        newWall.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
                        newWall.model = wall.model;
                        newWall.Texture = wall.Texture;
                        newWall.Shader = wall.Shader;
                        ModelUtil.UpdateBoundingBoxes(ref newWall);
                        pieces.Add(newWall);
                    }

                    if (z == topLeft.Z)
                    {
                        MetaModel newWall = new MetaModel();
                        newWall.Position = new Vector3(x, topLeft.Y, z-2.0f);
                        newWall.Rotation = Vector3.Zero;
                        newWall.model = wall.model;
                        newWall.Texture = wall.Texture;
                        newWall.Shader = wall.Shader;
                        ModelUtil.UpdateBoundingBoxes(ref newWall);
                        pieces.Add(newWall);
                    }
                    else if (z == ((topLeft.Z + worldSpaceSize.Y) - 1))
                    {
                        MetaModel newWall = new MetaModel();
                        newWall.Position = new Vector3(x, topLeft.Y, z+2.0f);
                        newWall.Rotation = Vector3.Zero;
                        newWall.model = wall.model;
                        newWall.Texture = wall.Texture;
                        newWall.Shader = wall.Shader;
                        ModelUtil.UpdateBoundingBoxes(ref newWall);
                        pieces.Add(newWall);
                    }
                }
            }
        }
    }
}
