using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BBoxImporter
{
    [ContentProcessor(DisplayName = "Bounding Box Generator")]
    public class Importer : ModelProcessor
    {
        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double minZ = double.MaxValue;
        double maxX = double.MinValue;
        double maxY = double.MinValue;
        double maxZ = double.MinValue;

        List<BoundingBox> boxes = new List<BoundingBox>();
        List<List<Vector3>> MeshVerts = new List<List<Vector3>>();

        object[] ModelData = new object[2];

        // Bounding Box's
        private void CheckNode(NodeContent content)
        {
            foreach (NodeContent o in content.Children)
            {
                if (o is MeshContent && o.Name.Contains("bounding"))
                {
                    // Get VertData
                    GetAllVerticies((MeshContent)o);
                    BoundingBox bb = new BoundingBox();

                    minX = double.MaxValue;
                    minY = double.MaxValue;
                    minZ = double.MaxValue;
                    maxX = double.MinValue;
                    maxY = double.MinValue;
                    maxZ = double.MinValue;

                    MeshContent mesh = (MeshContent)o;
                    foreach (Vector3 basev in mesh.Positions)
                    {
                        Vector3 v = basev;

                        if (v.X < minX)
                            minX = v.X;

                        if (v.Y < minY)
                            minY = v.Y;

                        if (v.Z < minZ)
                            minZ = v.Z;

                        if (v.X > maxX)
                            maxX = v.X;

                        if (v.Y > maxY)
                            maxY = v.Y;

                        if (v.Z > maxZ)
                            maxZ = v.Z;

                    }

                    double lenX = maxX - minX;
                    double lenZ = maxZ - minZ;
                    double lenY = maxY - minY;

                    #region Matrix_Scale_Code
                    Matrix scaleMatrix = Matrix.CreateScale(base.Scale);

                    // We need to make sure that the BoundingBoxes are scaled properly:
                    bb.Min = Vector3.Transform(new Vector3((float)minX, (float)minY, (float)minZ), scaleMatrix);
                    bb.Max = Vector3.Transform(new Vector3((float)maxX, (float)maxY, (float)maxZ), scaleMatrix);
                    #endregion
                    #region Float_Scale
                    //bb.Min = new Vector3((float)minX, (float)minY, (float)minZ);
                    //bb.Max = new Vector3((float)maxX, (float)maxY, (float)maxZ);
                    #endregion
                    #region Nonscale
                    //bb.Min = new Vector3((float)minX, (float)minY, (float)minZ);
                    //bb.Max = new Vector3((float)maxX, (float)maxY, (float)maxZ);
                    #endregion
                    boxes.Add(bb);
                }
                else
                    CheckNode(o);
            }
        }

        // Vertex positions
        private void GetAllVerticies(MeshContent mesh)
        {
            for (int g = 0; g < mesh.Geometry.Count; g++)
            {
                GeometryContent geometry = mesh.Geometry[g];

                List<Vector3> temp = new List<Vector3>();

                for (int ind = 0; ind < geometry.Indices.Count; ind++)
                {
                    // Transforms all of my verticies to local space.
                    Vector3 position = Vector3.Transform(geometry.Vertices.Positions[geometry.Indices[ind]], mesh.AbsoluteTransform);
                    temp.Add(position);
                }
                MeshVerts.Add(temp);
            }
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            // Setup bounding box data.
            CheckNode(input);

            ModelData[0] = boxes;
            ModelData[1] = MeshVerts;

            ModelContent basemodel = base.Process(input, context);
            basemodel.Tag = ModelData;
            return basemodel;
        }
    }
}