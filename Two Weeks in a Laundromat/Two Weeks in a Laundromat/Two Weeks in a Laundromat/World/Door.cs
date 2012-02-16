﻿using System;
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
        public Door(ref MetaModel newObject, GraphicsDevice gDevice)
            : base(ref newObject, gDevice) 
        {
        }

        public override void interactedWith()
        {
            System.Diagnostics.Debug.WriteLine("Clicked on a fuckin door!");
            base.interactedWith();
        }
    }
}
