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
        private const float FuckingDoorOffset = tileHalf + tileSize;
        private Vector3 entranceDoorDirection;
        #endregion

        public Hallway(string theme)
            : base(theme)
        {
            this.dimensions = Vector3.One;
            this.roomCenter = Vector3.Zero;
        }

        public Hallway(ref Vector3 startPos, ref Vector3 doorDirection, string theme)
            : base(theme)
        {
            this.dimensions = Vector3.One;
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
                Vector3 movVector = new Vector3(roomCenter.X + FuckingDoorOffset + (tileSize * i), roomCenter.Y,
                    roomCenter.Z + FuckingDoorOffset + (tileSize * i));
                movVector *= entranceDoorDirection;

                if (movVector.X == 0f)
                {
                    movVector.X = roomCenter.X;
                }
                if (movVector.Y == 0f)
                {
                    movVector.Y = roomCenter.Y;
                }
                if (movVector.Z == 0f)
                {
                    movVector.Z = roomCenter.Z;
                }

                MetaModel floorPiece = new MetaModel();
                floorPiece.Position = movVector;
                floorPiece.Rotation = Vector3.Zero;
                floorPiece.model = floor.model;
                floorPiece.Texture = floor.Texture;
                floorPiece.Shader = floor.Shader;
                ModelUtil.UpdateBoundingBoxes(ref floorPiece);
                pieces.Add(floorPiece);

                MetaModel ceilingPiece = new MetaModel();
                ceilingPiece.Position = new Vector3(movVector.X, roomCenter.Y + ((dimensions.Y - 1) * 8), movVector.Z);
                ceilingPiece.Rotation = Vector3.Zero;
                ceilingPiece.model = ceiling.model;
                ceilingPiece.Texture = ceiling.Texture;
                ceilingPiece.Shader = ceiling.Shader;
                ModelUtil.UpdateBoundingBoxes(ref ceilingPiece);
                pieces.Add(ceilingPiece);
            }
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice, Effect alternateShader)
        {
            this.alternateShader = alternateShader;
            this.Load(gManager, gDevice);
        }
    }
}
