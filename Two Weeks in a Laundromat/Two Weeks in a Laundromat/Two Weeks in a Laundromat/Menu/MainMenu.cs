using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delve_Engine.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Delve_Engine.DataTypes;

namespace Two_Weeks_in_a_Laundromat
{
    public class MainMenu : Menu
    {
        #region BackgroundItems
        #endregion

        #region Constructors
        public MainMenu(GraphicsDevice gDevice, string title): base(gDevice, title)
        {
            clearColor = Color.LightBlue;
        }
        #endregion

        public override void Load(ContentManager gManager)
        {
            MetaModel test = new MetaModel();
            test.Position = new Vector3(0,0,1.0f);
            test.Rotation = new Vector3(0.0f, MathHelper.ToRadians(45.0f), 0.0f);
            test.model = gManager.Load<Model>("Models/Ghiblies/cart");
            test.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/cart");
            this.models.Add(test);

            test = new MetaModel();
            test.Position = new Vector3(0, 0, -5.0f);
            test.Rotation = Vector3.Zero;
            test.model = gManager.Load<Model>("Models/Ghiblies/Tired_and_Things");
            test.Texture = gManager.Load<Texture2D>("Textures/Ghiblies/textureless");
            this.models.Add(test);

            base.Load(gManager);
        }
    }
}
