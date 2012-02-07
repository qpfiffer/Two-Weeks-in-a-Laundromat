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
        private Room currentRoom = null;

        public World(): base()
        {
            mainPlayer.setCameraPosition(new Vector3(10, Player.playerHeight, 10), Vector3.Zero);
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            if (currentRoom == null)
            {
                currentRoom = new Laundromat();
                currentRoom.Load(gManager, gDevice);
            }

            base.Load(gManager, gDevice);
        }

        public override void handleInput(ref InputInfo info)
        {
#if DEBUG
            // Toggles on brighter light settings.
            if (info.curKBDState.IsKeyDown(Keys.F) &&
                info.oldKBDState.IsKeyUp(Keys.F))
            {
                boundingBoxesDraw = !boundingBoxesDraw;
                currentRoom.ShouldDrawBoundingBoxes = !currentRoom.ShouldDrawBoundingBoxes;
            }
#endif

            base.handleInput(ref info);
        }

        public override void Draw()
        {
            clearBuffer();
            currentRoom.Draw(gDevice, globalEffect, this.mainPlayer.Position);

            base.Draw();
        }
    }
}
