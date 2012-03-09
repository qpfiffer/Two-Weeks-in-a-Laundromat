#region FileDescription
// Some of this was taken from the SkinnedModelSample by microsoft.
// I basically copy-pasted it in chunks, and add my own comments too see
// if I understood what was going on.
//
// Because of this, this is all licensed under the Microsoft Permissive License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;
using SkinnedModel;

namespace BBox_Animated_Importer
{
    [ContentProcessor(DisplayName = "Animated BoundingBox Generator")]
    public class AnimatedImporter : ModelProcessor
    {
        #region BoundingBox

        [DisplayName("Blender Export")]
        [Description("Toggles whether or not this model should be treated as exported from Blender.")]
        [DefaultValue(false)]
        public bool BlenderExport
        {
            get;
            set; 
        }

        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double minZ = double.MaxValue;
        double maxX = double.MinValue;
        double maxY = double.MinValue;
        double maxZ = double.MinValue;

        List<BoundingBox> boxes = new List<BoundingBox>();
        List<List<Vector3>> MeshVerts = new List<List<Vector3>>();

        // Holds the calculated bounding boxes and the skinningData.
        object[] ModelData = new object[3];
        #endregion
        #region BoundingBoxProcessing
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
                    if (BlenderExport)
                    {
                        for (int i = 0; i < mesh.Positions.Count; i++)
                        {
                            Vector3 v = mesh.Positions[i];
                            v = Vector3.Transform(v, Matrix.CreateRotationX(RotationX));
                            v = Vector3.Transform(v, Matrix.CreateRotationY(RotationY));
                            v = Vector3.Transform(v, Matrix.CreateRotationZ(RotationZ));

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
                    }
                    else
                    {
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
                    Vector3 position = Vector3.Transform(geometry.Vertices.Positions[geometry.Indices[ind]],
                        mesh.AbsoluteTransform);
                    temp.Add(position);
                }
                MeshVerts.Add(temp);
            }
        }

        // Normals
        private void GenerateNormals(NodeContent input, ContentProcessorContext context)
        {
            MeshContent mesh = input as MeshContent;

            if (mesh != null)
            {
                MeshHelper.CalculateNormals(mesh, true);
            }

            foreach (NodeContent child in input.Children)
            {
                GenerateNormals(child, context);
            }
        }
        #endregion
        #region AnimationProcessing
        
        void ValidateMesh(NodeContent node, ContentProcessorContext context,
                                 string parentBoneName)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Validate the mesh.
                if (parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} has no skinning information, so it has been deleted.",
                        mesh.Name);

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
            }
            else if (node is BoneContent)
            {
                // If this is a bone, remember that we are now looking inside it.
                parentBoneName = node.Name;
            }

            // Recurse (iterating over a copy of the child collection,
            // because validating children may delete some of them).
            foreach (NodeContent child in new List<NodeContent>(node.Children))
                ValidateMesh(child, context, parentBoneName);
        }

        /// <summary>
        /// Used by ValidateMesh.
        /// </summary>
        bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }

            return true;
        }

        void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (NodeContent child in node.Children)
            {
                // Blender fucks everything up, so make sure that we transform the skeleton.
                if (child == skeleton)
                {
                    //MeshHelper.TransformScene(child, child.Transform + Matrix.CreateRotationX(MathHelper.ToRadians(RotationX)));
                    continue;
                }

                // Bake the local transform into the actual geometry.
                MeshHelper.TransformScene(child, child.Transform);

                // Having baked it, we can now set the local
                // coordinate system back to identity.
                child.Transform = Matrix.Identity;

                // Recurse.
                FlattenTransforms(child, skeleton);
            }
        }

        Dictionary<string, AnimationClip> ProcessAnimations(
            AnimationContentDictionary animations, IList<BoneContent> bones)
        {
            // Build up a table mapping bone names to indices.
            Dictionary<string, int> boneMap = new Dictionary<string, int>();           

            for (int i = 0; i < bones.Count; i++)
            {
                string boneName = bones[i].Name;

                //if (!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            // Convert each animation in turn.
            Dictionary<string, AnimationClip> animationClips;
            animationClips = new Dictionary<string, AnimationClip>();

            foreach (KeyValuePair<string, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap);

                animationClips.Add(animation.Key, processed);
            }

            if (animationClips.Count == 0)
            {
                throw new InvalidContentException(
                            "Input file does not contain any animations.");
            }

            return animationClips;
        }

        AnimationClip ProcessAnimation(AnimationContent animation,
                                              Dictionary<string, int> boneMap)
        {
            List<Keyframe> keyframes = new List<Keyframe>();

            // For each input animation channel.
            foreach (KeyValuePair<string, AnimationChannel> channel in
                animation.Channels)
            {
                // Look up what bone this channel is controlling.
                int boneIndex;

                if (!boneMap.TryGetValue(channel.Key, out boneIndex))
                {
                    throw new InvalidContentException(string.Format(
                        "Found animation for bone '{0}', " +
                        "which is not part of the skeleton.", channel.Key));
                }

                // Convert the keyframe data.
                foreach (AnimationKeyframe keyframe in channel.Value)
                {
                    //if (BlenderExport == true)
                    //{
                    //    Matrix fixTheFuckup = keyframe.Transform + Matrix.CreateRotationX(MathHelper.ToRadians(RotationX));
                    //    keyframes.Add(new Keyframe(boneIndex, keyframe.Time,
                    //                               keyframe.Transform));
                    //}
                    //else
                    //{
                    Matrix transform = keyframe.Transform;
                    //transform *= Matrix.CreateRotationX(MathHelper.ToRadians(RotationX));
                        keyframes.Add(new Keyframe(boneIndex, keyframe.Time,
                                                   transform));
                    //}
                }
            }

            // Sort the merged keyframes by time.
            keyframes.Sort(CompareKeyframeTimes);

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            return new AnimationClip(animation.Duration, keyframes);
        }

        int CompareKeyframeTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }

        #endregion
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            //Why.
            if (BlenderExport)
            {
                RotationX -= 90.0f;
            }

            #region BoundingBox
            //GenerateNormals(input, context);
            // Setup bounding box data.
            CheckNode(input);

            ModelData[0] = boxes;
            #endregion  
            #region Animation
            ValidateMesh(input, context, null);

            // Find the skeleton.
            BoneContent skeleton = MeshHelper.FindSkeleton(input);

            // Make sure we have one.
            if (skeleton == null)
                throw new InvalidContentException("Input skeleton not found.");

            // Make sure everything is in the same coordinate system.
            //MeshHelper.TransformScene(skeleton, skeleton.Transform + Matrix.CreateRotationX(MathHelper.ToRadians(RotationX)));
            FlattenTransforms(input, skeleton);

            // Read the bind pose and skeleton hierarchy data.
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

            // Too many bones. MSDN says the max is 72. Good to know.
            if (bones.Count > SkinnedEffect.MaxBones)
            {
                throw new InvalidContentException(string.Format(
                    "Skeleton has {0} bones, but the maximum supported is {1}.",
                    bones.Count, SkinnedEffect.MaxBones));
            }

            // Bookkeeping:
            List<Matrix> bindPose = new List<Matrix>();
            List<Matrix> inverseBindPose = new List<Matrix>();
            List<int> skeletonHierarchy = new List<int>();

            // Put everything where it needs to be:
            foreach (BoneContent bone in bones)
            {
                Vector3 scale, trans;
                Quaternion rot;
                bone.Transform.Decompose(out scale, out rot, out trans);
                Matrix what = Matrix.CreateTranslation(trans); 
                what *= Matrix.CreateFromQuaternion(rot) + Matrix.CreateRotationX(MathHelper.ToRadians(RotationX));
                //what *= Matrix.CreateScale(scale);


                MeshHelper.TransformScene(bone, what);
                //bone.Transform += Matrix.CreateRotationX(MathHelper.ToRadians(RotationX));
                //if (BlenderExport)
                //{
                //    Matrix toAdd = Matrix.CreateRotationX(MathHelper.ToRadians(90.0f));
                //    bindPose.Add(bone.Transform + toAdd);
                //    inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform + toAdd));
                //    skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                //}
                //else
                //{
                    bindPose.Add(what);
                    inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                    skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                //}
            }

            // Convert animation data to our runtime format.
            Dictionary<string, AnimationClip> animationClips;

            context.Logger.LogWarning(null, null, "Bone count is: {0}", bones.Count);

            animationClips = ProcessAnimations(skeleton.Animations, bones);

            // Store our custom animation data in the Tag property of the model.
            SkinningData temp = new SkinningData(animationClips, bindPose,
                                         inverseBindPose, skeletonHierarchy);
            ModelData[2] = temp;
            #endregion            

            ModelContent basemodel = base.Process(input, context);
            basemodel.Tag = ModelData;
            return basemodel;
        }
    }
}