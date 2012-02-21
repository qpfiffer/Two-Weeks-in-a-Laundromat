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
    class Door: GameObject
    {
        private bool open = false;
        private DoorData metaDoor;
        private Room parentRoom, childRoom;

        public DoorData MetaDoor
        {
            get { return metaDoor; }
        }

        public bool IsOpen
        {
            get { return open; }
        }

        public Room ParentRoom
        {
            get { return parentRoom; }
        }

        public Room ChildRoom
        {
            get { return childRoom; }
            set { childRoom = value; }
        }

        public Door(ref MetaModel newObject, DoorData metaData, GraphicsDevice gDevice, Room parentRoom)
            : base(ref newObject, gDevice) 
        {
            this.metaDoor = metaData;
            this.parentRoom = parentRoom;
        }

        public override void interactedWith()
        {
            if (open)
            {
                this.model.Rotation = new Vector3(0, model.Rotation.Y + MathHelper.ToRadians(-90.0f), 0);
                //this.model.model.Bones[0].Transform = Matrix.CreateRotationY(model.Rotation.Y + MathHelper.ToRadians(-90.0f));
                open = false;
            }
            else
            {
                this.model.Rotation = new Vector3(0, model.Rotation.Y + MathHelper.ToRadians(90.0f), 0);
                //this.model.model.Bones[0].Transform = Matrix.CreateRotationY(model.Rotation.Y);
                open = true;
            }

            ModelUtil.UpdateBoundingBoxes(ref model);
            base.interactedWith();
        }
    }
}
