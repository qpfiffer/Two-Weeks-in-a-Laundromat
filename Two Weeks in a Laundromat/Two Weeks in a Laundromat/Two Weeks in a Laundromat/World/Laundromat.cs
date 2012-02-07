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
                dryer.Position = new Vector3(roomCenter.X, 0.0f, roomCenter.Z + 1.0f + (i * 5.5f));
                dryer.Rotation = new Vector3(0, 0, 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                ModelUtil.UpdateBoundingBoxes(ref dryer);
                this.pieces.Add(dryer);

                dryer = new MetaModel();
                dryer.Position = new Vector3(roomCenter.X + ((dimensions.X-1)*4), 0.0f, roomCenter.Z + 1.0f + (i * 5.5f));
                dryer.Rotation = new Vector3(0, MathHelper.ToRadians(180.0f), 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                ModelUtil.UpdateBoundingBoxes(ref dryer);
                this.pieces.Add(dryer);
            }

            base.Load(gManager, gDevice);
        }
    }
}
