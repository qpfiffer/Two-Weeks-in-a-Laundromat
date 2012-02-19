using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Delve_Engine.Utilities;
using Delve_Engine.DataTypes;
using Delve_Engine.World;

namespace Two_Weeks_in_a_Laundromat
{
    public class World: Delve_Engine.World.World
    {
        private List<Room> liveRooms;

        public World(): base()
        {
            mainPlayer.setCameraPosition(new Vector3(10, Player.playerHeight, 15), Vector3.Zero);
            liveRooms = new List<Room>();
        }

        public override void Update(GameTime gTime)
        {
            foreach (Room room in liveRooms)
            {
                room.Update(gTime);
            }
            base.Update(gTime);
        }

        public override void Load(ContentManager gManager, GraphicsDevice gDevice)
        {
            if (liveRooms.Count == 0)
            {
                Room laundromat = new Laundromat(string.Empty);
                laundromat.Load(gManager, gDevice);
                //foreach (MetaModel m in laundromat.AllMetas)
                //{
                //    this.collisionBoxes.AddRange(m.BBoxes);
                //}

                //foreach (GameObject go in laundromat.AllGOs)
                //{
                //    MetaModel m = go.Model;
                //    this.collisionBoxes.AddRange(m.BBoxes);
                //}

                //liveRooms.Add(laundromat);

                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        Vector3 testRoomDim = new Vector3(WOLOLO.Next(2, 10), WOLOLO.Next(1, 6), WOLOLO.Next(2, 10));
                        Vector3 testRoomStart = new Vector3(x + (x * 30), 0f, y + (y * 30));
                        Room testRoom = new Room(ref testRoomDim, ref testRoomStart, string.Empty);
                        testRoom.addRandomDoor(WOLOLO);
                        testRoom.addRandomDoor(WOLOLO);
                        testRoom.addRandomDoor(WOLOLO);
                        testRoom.Load(gManager, gDevice, gManager.Load<Effect>("Shaders/rbShift"));
                        //testRoom.Load(gManager, gDevice);
                        liveRooms.Add(testRoom);
                    }
                }
            }

            //bgMusic = gManager.Load<Song>("Sounds/Music/Headache");
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(bgMusic);

            base.Load(gManager, gDevice);
        }

        public override void handleInput(ref InputInfo info)
        {
            if (info.curKBDState.IsKeyDown(Keys.E) &&
                info.oldKBDState.IsKeyUp(Keys.E))
            {
                List<GameObject> objects = new List<GameObject>();
                foreach (Room room in liveRooms)
                {
                    objects.AddRange(room.AllGOs);
                }

                #region RetardedShit
                // Get the object the player might have clicked on:
                GameObject clickedOn = this.mainPlayer.clickOnSomething(ref objects);
                // If we actually clicked on something:
                if (clickedOn != null)
                {
                    // Remove the non-transformed bounding boxes from the collision list:
                    foreach (BoundingBox bbox in clickedOn.Model.BBoxes)
                    {
                        collisionBoxes.Remove(bbox);
                    }
                    // Interact with the object
                    clickedOn.interactedWith();
                    // Add the newly transformed boxes back to the list:
                    collisionBoxes.AddRange(clickedOn.Model.BBoxes);
                }
                #endregion
            }                
#if DEBUG
            // Toggles on brighter light settings.
            if (info.curKBDState.IsKeyDown(Keys.F) &&
                info.oldKBDState.IsKeyUp(Keys.F))
            {
                boundingBoxesDraw = !boundingBoxesDraw;
                foreach (Room room in liveRooms)
                {
                    room.ShouldDrawBoundingBoxes = !room.ShouldDrawBoundingBoxes;
                }
            }
#endif

            base.handleInput(ref info);
        }

        public override void Draw()
        {
            clearBuffer();
            foreach (Room room in liveRooms)
            {                
                room.Draw(gDevice, ref cMatrices, this.mainPlayer.Position);
            }

            base.Draw();
        }
    }
}
