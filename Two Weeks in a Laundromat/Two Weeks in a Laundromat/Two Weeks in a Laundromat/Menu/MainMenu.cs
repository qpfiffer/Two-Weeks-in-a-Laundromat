using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delve_Engine.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Delve_Engine.DataTypes;
using Delve_Engine.Utilities;

namespace Two_Weeks_in_a_Laundromat
{
    public class MainMenu : Menu
    {
        #region BackgroundItems
        #endregion

        #region Constructors
        public MainMenu(GraphicsDevice gDevice, string title): base(gDevice, title)
        {
            cameraPos = new Vector3(0.8f, 2.2f, -5.4f);
            leftRightRot = MathHelper.ToRadians(180.0f);
            upDownRot = MathHelper.ToRadians(-10.0f);
            clearColor = Color.LightBlue;
        }
        #endregion

        private Vector3 defaultLightPos;

        public override void Update(GameTime gTime)
        {
            foreach (MetaModel m in models)
            {
                if (m.Shader != null)
                {
                    m.Shader.Parameters["World"].SetValue(globalEffect.World);
                    m.Shader.Parameters["View"].SetValue(globalEffect.View);
                    m.Shader.Parameters["Projection"].SetValue(globalEffect.Projection);
                }
            }
            base.Update(gTime);
        }

        public override void Load(ContentManager gManager)
        {
            defaultLightPos = new Vector3(0, 2, 0);

            MetaModel dryer = new MetaModel();
            dryer.Position = new Vector3(0.8f, 0.0f, -6.0f);
            dryer.Rotation = new Vector3(0, MathHelper.ToRadians(-90.0f), 0);
            dryer.model = gManager.Load<Model>("Models/Ghiblies/Dryer");
            dryer.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/Dryer");
            dryer.Shader = ModelUtil.CreateGlobalEffect(gDevice, gManager);
            this.models.Add(dryer);

            base.Load(gManager);
        }
    }
}
