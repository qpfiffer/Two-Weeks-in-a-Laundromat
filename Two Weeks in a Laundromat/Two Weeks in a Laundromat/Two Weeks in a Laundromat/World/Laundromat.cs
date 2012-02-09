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
            for (int i = 0; i < 7; i++)
            {
                MetaModel dryer = new MetaModel();
                dryer.Position = new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z + 1.0f + (i * 5.5f));
                dryer.Rotation = new Vector3(0, 0, 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                ModelUtil.UpdateBoundingBoxes(ref dryer);
                this.pieces.Add(dryer);

                dryer = new MetaModel();
                dryer.Position = new Vector3(roomCenter.X + ((dimensions.X-1)*tileSize), roomCenter.Y, roomCenter.Z + 1.0f + (i * 5.5f));
                dryer.Rotation = new Vector3(0, MathHelper.ToRadians(180.0f), 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                ModelUtil.UpdateBoundingBoxes(ref dryer);
                this.pieces.Add(dryer);
            }

            MetaModel oddDryer = new MetaModel();
            oddDryer.Position = new Vector3(roomCenter.X + 6.0f, roomCenter.Y, roomCenter.Z + 15.0f);
            oddDryer.Rotation = new Vector3(MathHelper.ToRadians(90.0f), 0, 0);
            oddDryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
            oddDryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
            oddDryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            ModelUtil.UpdateBoundingBoxes(ref oddDryer);
            this.pieces.Add(oddDryer);

            MetaModel pillar = new MetaModel();
            pillar.Position = new Vector3(roomCenter.X + (tileSize * dimensions.X / 2.0f) - tileHalf, roomCenter.Y, roomCenter.Z + dimensions.Z - tileHalf);
            pillar.Rotation = new Vector3(0, 0, 0);
            pillar.model = gManager.Load<Model>("Models/Ghiblies/pillar");
            pillar.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/pillar");
            pillar.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            ModelUtil.UpdateBoundingBoxes(ref pillar);
            this.pieces.Add(pillar);

            MetaModel pillar2 = new MetaModel();
            pillar2.Position = new Vector3(pillar.Position.X, pillar.Position.Y,
                roomCenter.Z + (0.75f * dimensions.Z * tileSize) - tileHalf);
            pillar2.Rotation = new Vector3(0, 0, 0);
            pillar2.model = pillar.model;
            pillar2.Texture = pillar.Texture;
            pillar2.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            ModelUtil.UpdateBoundingBoxes(ref pillar2);
            this.pieces.Add(pillar2);

            MetaModel cabinetModel = new MetaModel();
            cabinetModel.Position = new Vector3(roomCenter.X + 6.0f, roomCenter.Y, roomCenter.Z + 6.0f);
            cabinetModel.Rotation = new Vector3(0, MathHelper.ToRadians(45.0f), 0);
            cabinetModel.model = gManager.Load<Model>("Models/Ghiblies/cabinet");
            cabinetModel.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            cabinetModel.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            ModelUtil.UpdateBoundingBoxes(ref cabinetModel);

            GameObject cabinet = new GameObject(ref cabinetModel, gDevice);
            this.things.Add(cabinet);

            base.Load(gManager, gDevice);
        }
    }
}
