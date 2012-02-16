using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delve_Engine.DataTypes
{
    public struct MetaModel
    {
        public Model model { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Texture2D Texture { get; set; }
        public List<BoundingBox> BBoxes { get; set; }
        public Effect Shader { get; set; }
    }

    public enum WallSide {
        North = 1,
        East = 2, 
        South = 4,
        West = 8 
    };

    /// <summary>
    /// The way this works is you specify a wall,
    /// and then how many tiles in the wall is.
    /// </summary>
    public struct DoorData
    {
        public DoorData(WallSide myWall, float myTile)
        {
            this.myWall = myWall;
            this.myTilePos = myTile;
        }

        public WallSide myWall;
        public float myTilePos;        
    }

    public struct TexturedPlane
    {
        public Texture2D texture;
        public GraphicsDevice gDevice;
        public VertexPositionTexture[] vertices;
        public short[] indices;
        public VertexBuffer vBuffer;
        public IndexBuffer iBuffer;
    }

    public struct ColoredPlane
    {
        public GraphicsDevice gDevice;
        public Color color;
        public VertexPositionColor[] vertices;
        public short[] indices;
        public VertexBuffer vBuffer;
        public IndexBuffer iBuffer;
    }

    public struct MatrixDescriptor
    {
        public Matrix view { get; set; }
        public Matrix proj { get; set; }
        public Matrix world { get; set; }
    }
}
