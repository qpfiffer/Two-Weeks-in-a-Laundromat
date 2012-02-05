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
    public class World: Delve_Engine.World.World
    {
        private Room currentRoom = null;

        public World(): base()
        {
            mainPlayer.setCameraPosition(new Vector3(0, Player.playerHeight, 10), Vector3.Zero);
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            MetaModel Scale = new MetaModel();
            Scale.Position = Vector3.Zero;
            Scale.Rotation = Vector3.Zero;
            Scale.model = gManager.Load<Model>("Models/Ghiblies/Scale");
            Scale.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            Scale.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            addNewModel(ref Scale);

            //for (int i = 0; i < 10; i++)
            //{
            //    MetaModel wall = new MetaModel();
            //    wall.Position = new Vector3(-5, 0, (i * 4));
            //    wall.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
            //    wall.model = gManager.Load<Model>("Models/Segments/wall");
            //    wall.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            //    wall.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            //    addNewModel(ref wall);

            //    MetaModel floor = new MetaModel();
            //    floor.Position = new Vector3(-7, 0, (i * 4));
            //    floor.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
            //    floor.model = gManager.Load<Model>("Models/Segments/floor");
            //    floor.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            //    floor.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            //    addNewModel(ref floor);

            //    wall = new MetaModel();
            //    wall.Position = new Vector3(-9, 0, (i * 4));
            //    wall.Rotation = new Vector3(0, MathHelper.ToRadians(90.0f), 0);
            //    wall.model = gManager.Load<Model>("Models/Segments/wall");
            //    wall.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            //    wall.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            //    addNewModel(ref wall);
            //}

            for (int i = 0; i < 3; i++)
            {
                MetaModel dryer = new MetaModel();
                dryer.Position = new Vector3(6.0f - (i*6.2f), 0.0f, -6);
                dryer.Rotation = new Vector3(0, MathHelper.ToRadians(-90.0f), 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                addNewModel(ref dryer);
            }

            if (currentRoom == null)
            {
                Vector3 dimensions = new Vector3(10, 8, 10);
                Vector3 pos = new Vector3(-10, 0, -10);
                currentRoom = new Room(ref dimensions, ref pos);
                currentRoom.Load(gManager, gDevice);
            }
            base.Load(gManager, gDevice);
        }

        public override void Draw()
        {
            currentRoom.Draw(gDevice);

            base.Draw();
        }
    }
}
