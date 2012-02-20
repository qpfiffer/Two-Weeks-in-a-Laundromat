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
        #region misc
        Random wololo;
        #endregion
        #region MetaData
        private const float FuckingDoorOffset = tileHalf + tileSize;
        private Vector3 entranceDoorDirection;
        #endregion

        public Hallway(ref Vector3 startPos, ref Vector3 doorDirection, string theme)
            : base(theme)
        {
            wololo = new Random();

            #region CentralVoodoo
            this.roomCenter = startPos;
            Vector3 offsetVector = doorDirection * new Vector3(tileHalf, 0.0f, tileHalf);
            this.roomCenter += offsetVector;
            #endregion
            #region DimensionalFuckery
            // Figure out a random straight hallway in the direction we want
            this.dimensions = new Vector3(wololo.Next(4, 10), 1.0f, wololo.Next(4, 10));
            // Make one dimensional:
            this.dimensions *= doorDirection;
            // Make sure the other dimension is at least one:
            if (dimensions.X == 0f)
            {
                dimensions.X = 1.0f;
            }
            if (dimensions.Z == 0f)
            {
                dimensions.Z = 1.0f;
            }

            // Make sure we have a height of at least 1
            dimensions.Y = 1.0f;
            #endregion
            this.entranceDoorDirection = doorDirection;
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            this.gManager = gManager;
            this.gDevice = gDevice;
            
            addRandomDoor(wololo);
            addRandomDoor(wololo);
            addRandomDoor(wololo);

            preloadPieces();
            setupPieces();            
            loaded = true;
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            this.Load(gManager, gDevice);
        }
    }
}
