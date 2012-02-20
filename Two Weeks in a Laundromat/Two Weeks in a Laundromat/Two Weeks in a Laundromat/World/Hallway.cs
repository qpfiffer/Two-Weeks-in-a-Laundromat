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
            //this.dimensions = new Vector3(4, 1.0f, 4);
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

            sanitiseHallway();

            preloadPieces();
            setupPieces();            
            loaded = true;
        }

        /// <summary>
        /// Makes sure there is no wall and no door at the entrance to the hallway.
        /// </summary>
        private void sanitiseHallway()
        {
            //wallToDirectionMap.Add(WallSide.East, new Vector3(1.0f, 0f, 0f));
            //wallToDirectionMap.Add(WallSide.West, new Vector3(-1.0f, 0f, 0f));
            //wallToDirectionMap.Add(WallSide.North, new Vector3(0.0f, 0f, -1.0f));
            //wallToDirectionMap.Add(WallSide.South, new Vector3(0.0f, 0f, 1.0f));

            DoorData wallToRemove = new DoorData();
            if (entranceDoorDirection.X == 1.0f)
            {
                // East
                wallToRemove.myWall = WallSide.West;
            }
            else if (entranceDoorDirection.X == -1.0f) 
            { 
                // West
                wallToRemove.myWall = WallSide.East;
            }

            if (entranceDoorDirection.Z == 1.0f)
            {
                // South
                wallToRemove.myWall = WallSide.North;
            }
            else if (entranceDoorDirection.Z == -1.0f)
            {
                // North
                wallToRemove.myWall = WallSide.South;
            }

            removeWallSection(ref wallToRemove);

            if (doors.Contains(wallToRemove))
            {
                doors.Remove(wallToRemove);
            }
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            this.Load(gManager, gDevice);
        }
    }
}
