#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace SkinnedModel
{
    /// <summary>
    /// The animation player is in charge of decoding bone position
    /// matrices from an animation clip.
    /// </summary>
    public class AnimationPlayer
    {
        #region Fields


        // Information about the currently playing animation clip.
        AnimationClip currentClipValue;
        TimeSpan currentTimeValue;
        int currentKeyframe;


        // Current animation transform matrices.
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransforms;
        Matrix[] initialBonePositions;


        // Backlink to the bind pose and skeleton hierarchy data.
        SkinningData skinningDataValue;

        private bool loop, lastFrame, playing, idle, reverse;

        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public bool Reverse
        {
            get { return reverse; }
            set { reverse = value; }
        }

        public bool IsPlaying
        {
            get { return playing; }
            set { playing = value; }
        }

        public bool IsIdle
        {
            get { return idle; }
            set { idle = value; }
        }

        public bool SitOnLastFrame 
        {
            get { return lastFrame; }
            set { lastFrame = value; } 
        }


        #endregion


        /// <summary>
        /// Constructs a new animation player.
        /// </summary>
        public AnimationPlayer(SkinningData skinningData)
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            loop = false;
            lastFrame = false;
            playing = false;
            reverse = false;

            skinningDataValue = skinningData;

            boneTransforms = new Matrix[skinningData.BindPose.Count];

            initialBonePositions = new Matrix[skinningData.BindPose.Count];
            skinningDataValue.BindPose.CopyTo(initialBonePositions, 0);

            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];
        }

        public Matrix[] GetStartingBonePositions()
        {
            return initialBonePositions;
        }


        /// <summary>
        /// Starts decoding the specified animation clip.
        /// </summary>
        public void StartClip(AnimationClip clip)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");

            idle = false;
            playing = true;

            if (!reverse)
            {
                currentClipValue = clip;
                currentTimeValue = TimeSpan.Zero;
                currentKeyframe = 0;
            }
            else
            {
                currentClipValue = clip;
                currentTimeValue = TimeSpan.Zero;
                currentKeyframe = clip.Keyframes.Count;
            }

            // Initialize bone transforms to the bind pose.
            skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
        }

        public void StartClipIdle(AnimationClip clip)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");

            idle = true;
            playing = true;

            currentClipValue = clip;
            currentTimeValue = TimeSpan.Zero;
            currentKeyframe = 0;

            // Initialize bone transforms to the bind pose.
            skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
        }


        /// <summary>
        /// Advances the current animation position.
        /// </summary>
        public void Update(TimeSpan time, bool relativeToCurrentTime,
                           Matrix rootTransform)
        {
            // This may look kind of weird, but we do it this way so that
            // the bone transforms get set only once.
            if (playing == true)
            {
                UpdateBoneTransforms(time, relativeToCurrentTime);
                UpdateWorldTransforms(rootTransform);
                UpdateSkinTransforms();
            }

            if (idle == true)
                playing = false;
        }


        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            if (currentClipValue == null)
                throw new InvalidOperationException(
                            "AnimationPlayer.Update was called before StartClip");

            #region Forward
            if (!reverse)
            {
                // Update the animation position.
                if (relativeToCurrentTime)
                {
                    time += currentTimeValue;


                    if (loop == false && time >= currentClipValue.Duration)
                        return;

                    // If we reached the end, loop back to the start.
                    while (time >= currentClipValue.Duration)
                        time -= currentClipValue.Duration;
                }

                if ((time < TimeSpan.Zero) || (time >= currentClipValue.Duration))
                    throw new ArgumentOutOfRangeException("time");

                // If the position moved backwards, reset the keyframe index.
                if (time < currentTimeValue)
                {
                    currentKeyframe = 0;
                    skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
                }

                currentTimeValue = time;

                // Read keyframe matrices.
                IList<Keyframe> keyframes = currentClipValue.Keyframes;

                while (currentKeyframe < keyframes.Count)
                {
                    Keyframe keyframe = keyframes[currentKeyframe];

                    // Stop when we've read up to the current time position.
                    if (keyframe.Time > currentTimeValue)
                        break;

                    // Use this keyframe.
                    boneTransforms[keyframe.Bone] = keyframe.Transform;

                    currentKeyframe++;
                }
            }
            #endregion
            #region Reverse
            else
            {
                // Update the animation position.
                if (relativeToCurrentTime)
                {
                    time += currentTimeValue;


                    if (loop == false && time >= currentClipValue.Duration)
                        return;

                    // If we reached the end, loop back to the start.
                    while (time >= currentClipValue.Duration)
                        time -= currentClipValue.Duration;
                }

                if ((time < TimeSpan.Zero) || (time >= currentClipValue.Duration))
                    throw new ArgumentOutOfRangeException("time");

                // If the position moved backwards, reset the keyframe index.
                if (time < currentTimeValue)
                {
                    currentKeyframe = 0;
                    skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
                }

                currentTimeValue = time;

                // Read keyframe matrices.
                IList<Keyframe> keyframes = currentClipValue.Keyframes;

                while (currentKeyframe < keyframes.Count)
                {
                    Keyframe keyframe = keyframes[currentKeyframe];

                    // Stop when we've read up to the current time position.
                    if (keyframe.Time > currentTimeValue)
                        break;

                    // Use this keyframe.
                    boneTransforms[keyframe.Bone] = keyframe.Transform;

                    currentKeyframe++;
                }
            }
            #endregion
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningDataValue.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] *
                                             worldTransforms[parentBone];
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningDataValue.InverseBindPose[bone] *
                                            worldTransforms[bone];
            }
        }


        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }


        /// <summary>
        /// Gets the clip currently being decoded.
        /// </summary>
        public AnimationClip CurrentClip
        {
            get { return currentClipValue; }
        }


        /// <summary>
        /// Gets the current play position.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return currentTimeValue; }
        }
    }
}
