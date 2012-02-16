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
    class Laundromat:Room
    {
        const int width = 7;
        const int height = 1;
        const int length = 10;

        public Laundromat()
        {
            this.dimensions = new Vector3(width, height, length);
            this.roomCenter = Vector3.Zero;
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gManager = gManager;
            this.gDevice = gDevice;

            Effect shaderToLoad = null;

            if (alternateShader == null)
                shaderToLoad = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            else
                shaderToLoad = alternateShader;

            #region RoomBuilding
            floor = new MetaModel();
            floor.Position = Vector3.Zero;
            floor.Rotation = Vector3.Zero;
            floor.model = gManager.Load<Model>("Models/Segments/floor");
            floor.Texture = gManager.Load<Texture2D>("Textures/Laundromat/floor");
            floor.Shader = shaderToLoad;

            wall = new MetaModel();
            wall.Position = Vector3.Zero;
            wall.Rotation = Vector3.Zero;
            wall.model = gManager.Load<Model>("Models/Segments/wall");
            wall.Texture = gManager.Load<Texture2D>("Textures/Laundromat/wall");
            wall.Shader = shaderToLoad;

            ceiling = new MetaModel();
            ceiling.Position = Vector3.Zero;
            ceiling.Rotation = Vector3.Zero;
            ceiling.model = gManager.Load<Model>("Models/Segments/ceiling");
            ceiling.Texture = gManager.Load<Texture2D>("Textures/Laundromat/ceiling");
            ceiling.Shader = shaderToLoad;

            doorframe = new MetaModel();
            doorframe.Position = Vector3.Zero;
            doorframe.Rotation = Vector3.Zero;
            doorframe.model = gManager.Load<Model>("Models/Segments/doorframe");
            doorframe.Texture = gManager.Load<Texture2D>("Textures/Laundromat/doorframe");
            doorframe.Shader = shaderToLoad;

            door = new MetaModel();
            door.model = gManager.Load<Model>("Models/segments/door");
            door.Texture = gManager.Load<Texture2D>("Textures/Laundromat/door");
            door.Shader = shaderToLoad;


            this.doors.Add(new DoorData(WallSide.North, 3));

            setupPieces();
            #endregion
            #region GhibliesBuilding
            for (int i = 0; i < 7; i++)
            {
                MetaModel dryer = new MetaModel();
                dryer.Position = new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z + 1.0f + (i * 5.5f));
                dryer.Rotation = Vector3.Zero;
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = shaderToLoad;
                ModelUtil.UpdateBoundingBoxes(ref dryer);
                this.pieces.Add(dryer);

                dryer = new MetaModel();
                dryer.Position = new Vector3(roomCenter.X + ((dimensions.X - 1) * tileSize), roomCenter.Y, roomCenter.Z + 1.0f + (i * 5.5f));
                dryer.Rotation = new Vector3(0, MathHelper.ToRadians(180.0f), 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = shaderToLoad;
                ModelUtil.UpdateBoundingBoxes(ref dryer);
                this.pieces.Add(dryer);
            }

            MetaModel pillar = new MetaModel();
            pillar.Position = new Vector3(roomCenter.X + (tileSize * dimensions.X / 2.0f) - tileHalf, roomCenter.Y, roomCenter.Z + dimensions.Z - tileHalf);
            pillar.Rotation = new Vector3(0, 0, 0);
            pillar.model = gManager.Load<Model>("Models/Ghiblies/pillar");
            pillar.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/pillar");
            pillar.Shader = shaderToLoad;
            ModelUtil.UpdateBoundingBoxes(ref pillar);
            this.pieces.Add(pillar);

            MetaModel pillar2 = new MetaModel();
            pillar2.Position = new Vector3(pillar.Position.X, pillar.Position.Y,
                roomCenter.Z + (0.75f * dimensions.Z * tileSize) - tileHalf);
            pillar2.Rotation = new Vector3(0, 0, 0);
            pillar2.model = pillar.model;
            pillar2.Texture = pillar.Texture;
            pillar2.Shader = shaderToLoad;
            ModelUtil.UpdateBoundingBoxes(ref pillar2);
            this.pieces.Add(pillar2);

            MetaModel box = new MetaModel();
            box.Position = new Vector3(13, 3.5f, 13);
            box.Rotation = Vector3.Zero;
            box.model = gManager.Load<Model>("Models/Ghiblies/box");
            box.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            box.Shader = shaderToLoad;
            ModelUtil.UpdateBoundingBoxes(ref box);
            this.pieces.Add(box);

            //MetaModel testFrame = new MetaModel();
            //testFrame.Position = new Vector3(roomCenter.X + dimensions.X * 2.0f, roomCenter.Y, roomCenter.Z + dimensions.Z * 2.0f);
            //testFrame.Rotation = Vector3.Zero;
            //testFrame.model = gManager.Load<Model>("Models/Segments/doorframe");
            //testFrame.Texture = gManager.Load<Texture2D>("Textures/Laundromat/doorframe");
            //testFrame.Shader = shaderToLoad;
            //ModelUtil.UpdateBoundingBoxes(ref testFrame);
            //this.pieces.Add(testFrame);

            //MetaModel doorMeta = new MetaModel();
            //doorMeta.Position = new Vector3(roomCenter.X + dimensions.X*2.0f, roomCenter.Y, roomCenter.Z + dimensions.Z*2.0f);
            //doorMeta.Rotation = Vector3.Zero;
            //doorMeta.model = gManager.Load<Model>("Models/Segments/door");
            //doorMeta.Texture = gManager.Load<Texture2D>("Textures/Laundromat/door");
            //doorMeta.Shader = shaderToLoad;
            //ModelUtil.UpdateBoundingBoxes(ref doorMeta);

            //Door testDoor = new Door(ref doorMeta, gDevice);
            //this.things.Add(testDoor);
            #endregion



        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            base.Load(gManager, gDevice, alternateShader);
        }
    }
}
