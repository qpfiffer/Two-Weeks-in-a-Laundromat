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
    /// <summary>
    /// Special motherfucking room. Surprise! Its a hallway.
    /// </summary>
    class Hallway : Room
    {
        #region MetaData
        private Vector3 entranceDoorDirection;
        #endregion

        public Hallway(string theme)
            : base(theme)
        {
            this.dimensions = new Vector3(-1.0f);
            this.roomCenter = Vector3.Zero;
        }

        public Hallway(ref Vector3 startPos, ref Vector3 doorDirection, string theme)
            : base(theme)
        {
            this.dimensions = new Vector3(-1.0f);
            this.roomCenter = startPos;
            this.entranceDoorDirection = doorDirection;
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gManager = gManager;
            this.gDevice = gDevice;
            preloadPieces();
            this.setupPieces();
            loaded = true;
        }

        protected override void setupPieces()
        {
            for (int i = 0; i < 10; i++)
            {
            }
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            this.Load(gManager, gDevice);
        }
    }
}
