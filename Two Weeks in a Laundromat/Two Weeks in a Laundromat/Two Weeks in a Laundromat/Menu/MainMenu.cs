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
        private Laundromat laundro;
        private Vector3 defaultLightPos;
        #endregion

        #region Constructors
        public MainMenu(GraphicsDevice gDevice, string title): base(gDevice, title)
        {
            cameraPos = new Vector3(1.5f, 6.0f, 12.0f);
            leftRightRot = MathHelper.ToRadians(-90.0f);
            upDownRot = MathHelper.ToRadians(-10.0f);
            clearColor = Color.LightBlue;
        }
        #endregion

        public override void Update(GameTime gTime)
        {            
            base.Update(gTime);
        }

        public override void Load(ContentManager gManager)
        {
            laundro = new Laundromat();
            laundro.Load(gManager, gDevice);
            laundro.ShouldDrawBoundingBoxes = false;
            foreach (MetaModel m in laundro.AllMetas)
            {
                m.Shader.Parameters["lightRadius"].SetValue(14.0f);
            }

            defaultLightPos = cameraPos;

            base.Load(gManager);
        }

        public override void Draw(SpriteBatch sBatch)
        {
            if (!bufferCleared)
                clearBuffer();
            
            laundro.Draw(gDevice, ref cMatrices, defaultLightPos);
 
            base.Draw(sBatch);
        }
    }
}
