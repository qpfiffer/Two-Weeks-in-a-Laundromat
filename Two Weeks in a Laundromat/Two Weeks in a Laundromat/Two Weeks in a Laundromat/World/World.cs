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

            MetaModel wall = new MetaModel();
            wall.Position = new Vector3(-5, 0, 0);
            wall.Rotation = Vector3.Zero;
            wall.model = gManager.Load<Model>("Models/Segments/wall");
            wall.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            wall.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            addNewModel(ref wall);

            for (int i = 0; i < 3; i++)
            {
                MetaModel dryer = new MetaModel();
                dryer.Position = new Vector3(6.0f - (i*6.2f), 0.0f, -6.0f - (i*6.0f));
                dryer.Rotation = new Vector3(0, MathHelper.ToRadians(-90.0f), 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                addNewModel(ref dryer);
            }

            base.Load(gManager, gDevice);
        }
    }
}
