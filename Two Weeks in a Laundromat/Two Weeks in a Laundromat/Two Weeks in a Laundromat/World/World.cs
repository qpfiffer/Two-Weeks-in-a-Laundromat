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
        private Dictionary<WallSide, Vector3> wallToDirectionMap;
        
        #region DebugShit
        public WallSide lastOpened { get; set; }
        #endregion

        public World(): base()
        {
            mainPlayer.setCameraPosition(new Vector3(10, Player.playerHeight, 15), Vector3.Zero);
            liveRooms = new List<Room>();
            
            // AREN'T I JUST THE FUCKING CLEVEREST
            wallToDirectionMap = new Dictionary<WallSide,Vector3>();
            wallToDirectionMap.Add(WallSide.East, new Vector3(1.0f,0f,0f));
            wallToDirectionMap.Add(WallSide.West, new Vector3(-1.0f,0f,0f));
            wallToDirectionMap.Add(WallSide.North, new Vector3(0.0f,0f,-1.0f));
            wallToDirectionMap.Add(WallSide.South, new Vector3(0.0f,0f,1.0f));

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
            this.gManager = gManager;
            this.gDevice = gDevice;

            if (liveRooms.Count == 0)
            {
                Room laundromat = new Laundromat(string.Empty);
                addNewRoom(laundromat);

                //createTestRooms();
            }

            //bgMusic = gManager.Load<Song>("Sounds/Music/Headache");
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(bgMusic);

            base.Load(gManager, gDevice);
        }

        private void createTestRooms()
        {
            // To generated a bunch of random rooms:
            //for (int x = 0; x < 5; x++)
            //{
            //    for (int y = 0; y < 5; y++)
            //    {
            //        Vector3 testRoomDim = new Vector3(WOLOLO.Next(2, 10), WOLOLO.Next(1, 6), WOLOLO.Next(2, 10));
            //        Vector3 testRoomStart = new Vector3(x + (x * 30), 0f, y + (y * 30));
            //        Room testRoom = new Room(ref testRoomDim, ref testRoomStart, string.Empty);
            //        testRoom.addRandomDoor(WOLOLO);
            //        testRoom.addRandomDoor(WOLOLO);
            //        testRoom.addRandomDoor(WOLOLO);
            //        addNewRoom(testRoom);
            //    }
            //}

            Vector3 testRoomDim = new Vector3(3, 2, 3);
            Vector3 testRoomStart = new Vector3(5, 0, 10);
            Room testRoom = new Room(ref testRoomDim, ref testRoomStart, string.Empty);
            DoorData newDoor = new DoorData(WallSide.North, 1.0f);
            testRoom.addDoor(ref newDoor);
            newDoor = new DoorData(WallSide.West, 1.0f);
            testRoom.addDoor(ref newDoor);
            newDoor = new DoorData(WallSide.East, 1.0f);
            testRoom.addDoor(ref newDoor);
            newDoor = new DoorData(WallSide.South, 1.0f);
            testRoom.addDoor(ref newDoor);
            //testRoom.addRandomDoor(WOLOLO);
            //testRoom.addRandomDoor(WOLOLO);
            //testRoom.addRandomDoor(WOLOLO);
            addNewRoom(testRoom);
        }

        private void addNewRoom(Room toAdd)
        {
            toAdd.Load(gManager, gDevice);

            foreach (MetaModel m in toAdd.AllMetas)
            {
                this.collisionBoxes.AddRange(m.BBoxes);
            }

            foreach (GameObject go in toAdd.AllGOs)
            {
                MetaModel m = go.Model;
                this.collisionBoxes.AddRange(m.BBoxes);
            }

            liveRooms.Add(toAdd);
        }

        private void removeRoom(Room toRemove)
        {
            foreach (MetaModel m in toRemove.AllMetas)
            {
                foreach (BoundingBox b in m.BBoxes) 
                {
                    this.collisionBoxes.Remove(b);
                }
            }

            foreach (GameObject go in toRemove.AllGOs)
            {
                MetaModel m = go.Model;
                foreach (BoundingBox b in m.BBoxes)
                {
                    this.collisionBoxes.Remove(b);
                }
            }

            liveRooms.Remove(toRemove);
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

                    // WALK THROUGH DOORS, YEAH?
                    // Add the newly transformed boxes back to the list:
                    collisionBoxes.AddRange(clickedOn.Model.BBoxes);

                    Door clickedDoor = clickedOn as Door;
                    lastOpened = clickedDoor.MetaDoor.myWall;
                    if (clickedDoor != null)
                    {
                        // We have to flip these because of the "interactedWith" call slightly
                        // above. Whatever. Just note that open means closed and vice versa.
                        if (!clickedDoor.IsOpen)
                        {
                            removeRoom(clickedDoor.ChildRoom);
                        }
                        else
                        {
                            Vector3 startPos = clickedDoor.Model.Position;
                            Vector3 direction = wallToDirectionMap[clickedDoor.MetaDoor.myWall];
                            Hallway newHallway = new Hallway(ref startPos, ref direction, string.Empty);
                            addNewRoom(newHallway as Room);
                            clickedDoor.ChildRoom = newHallway;
                        }
                    }
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
