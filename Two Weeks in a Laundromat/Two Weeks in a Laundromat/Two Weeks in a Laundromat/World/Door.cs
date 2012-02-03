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
        public Door(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice)
            : base(ref position, ref rotation, gDevice)
        {
        }
    }
}
