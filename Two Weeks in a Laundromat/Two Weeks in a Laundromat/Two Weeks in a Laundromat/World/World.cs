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
        const float milsMultiple = 50 * (float)Math.PI;
        public override void Update(GameTime gTime)
        {
            float mils = gTime.TotalGameTime.Milliseconds / milsMultiple;
            // Update the shaders in each model
            foreach (MetaModel m in modelsToDraw)
            {
                if (m.Shader != null)
                {                   
                    m.Shader.Parameters["World"].SetValue(globalEffect.World);
                    m.Shader.Parameters["View"].SetValue(globalEffect.View);
                    m.Shader.Parameters["Projection"].SetValue(globalEffect.Projection);

                    //m.Shader.Parameters["LightPos"].SetValue(new Vector3(0 + (float)Math.Sin(mils) * 4,
                    //    2, 0 + (float)Math.Cos(mils) * 4));
                    m.Shader.Parameters["LightPos"].SetValue(this.mainPlayer.Position);
                }
            }

            base.Update(gTime);
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            MetaModel cart = new MetaModel();
            cart.Position = new Vector3(0, 0, 1.0f);
            cart.Rotation = new Vector3(0.0f, MathHelper.ToRadians(45.0f), 0.0f);
            cart.model = gManager.Load<Model>("Models/Ghiblies/cart");
            cart.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/cart");
            cart.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            this.modelsToDraw.Add(cart);

            MetaModel laundromat = new MetaModel();
            laundromat.Position = new Vector3(0, 0, 0.0f);
            laundromat.Rotation = Vector3.Zero;
            laundromat.model = gManager.Load<Model>("Models/Ghiblies/Tired_and_Things");
            laundromat.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            laundromat.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            this.modelsToDraw.Add(laundromat);

            MetaModel hallwayTest = new MetaModel();
            hallwayTest.Position = new Vector3(-10, 0, 0.0f);
            hallwayTest.Rotation = Vector3.Zero;
            hallwayTest.model = gManager.Load<Model>("Models/Ghiblies/hall_test");
            hallwayTest.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            //hallwayTest.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            this.modelsToDraw.Add(hallwayTest);

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
    }
}
