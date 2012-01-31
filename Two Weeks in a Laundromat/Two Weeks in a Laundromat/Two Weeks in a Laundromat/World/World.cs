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

        public override void Update(GameTime gTime)
        {            
            base.Update(gTime);
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            MetaModel Scale = new MetaModel();
            Scale.Position = Vector3.Zero;
            Scale.Rotation = Vector3.Zero;
            Scale.model = gManager.Load<Model>("Models/Ghiblies/Scale");
            Scale.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            Scale.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            this.modelsToDraw.Add(Scale);

            for (int i = 0; i < 3; i++)
            {
                MetaModel dryer = new MetaModel();
                dryer.Position = new Vector3(3.0f - (i*2.2f), 0.0f, -6.0f);
                dryer.Rotation = new Vector3(0, MathHelper.ToRadians(-90.0f), 0);
                dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
                dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
                dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
                this.modelsToDraw.Add(dryer);
            }

            base.Load(gManager, gDevice);
        }

        public override void Draw()
        {
            // Update the shaders in each model
            foreach (MetaModel m in modelsToDraw)
            {
                if (m.Shader != null)
                {
                    m.Shader.Parameters["World"].SetValue(globalEffect.World);
                    m.Shader.Parameters["View"].SetValue(globalEffect.View);
                    m.Shader.Parameters["Projection"].SetValue(globalEffect.Projection);
                    m.Shader.Parameters["LightPos"].SetValue(this.mainPlayer.Position);
                }
            }
            base.Draw();
        }
    }
}
