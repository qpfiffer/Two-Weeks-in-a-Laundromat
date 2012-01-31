using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Delve_Engine.DataTypes;
using Delve_Engine.Utilities;

namespace Delve_Engine.World
{
    public class Player : GameObject
    {
        #region Fields
        private MatrixDescriptor matrices;
        private float mils;
        public bool HeadBobbing { get; set; }
        #endregion
        #region Properties
        public Vector2 rotationTarget { get; set; }
        public bool rotateEnabled { get; set; }
        /// <summary>
        /// Controls whether noclip is enabled or not.
        /// </summary>
        public bool NoClip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current matrices that this instance of player is using.
        /// </summary>
        public MatrixDescriptor Matrices
        {
            get { return matrices; }
            set { matrices = value; }
        }
        #endregion
        #region Constants
        // Default constants.
        public const float rotationSpeed = 0.3f;
        public const float moveSpeed = 4.0f;
        // Top of the head
        public const float playerHeight = 5.0f;
        // 0.5f up from the bottom of the floor (has to do with the radius
        // of the sphere we use)
        public const float floorBoxHeight = 0.5f;
        public const float gravity = 0.35f;
        public const float rightAngleRadians = 1.57079633f;
        #endregion

        public Player(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice) :
            base(ref position, ref rotation, gDevice)
        {
            this.rotateEnabled = false;
            HeadBobbing = false;
            matrices = new MatrixDescriptor();
            // A bounding sphere right at the chest
            BoundingSphere chestSphere = new BoundingSphere(position, 0.25f);
            base.addNewBounding(chestSphere, Vector3.Zero);
        }

        public void rotateCamera(ref Point mouseDifference, float timeDifference)
        {
            leftRightRot -= rotationSpeed * mouseDifference.X * timeDifference;
            if (leftRightRot > (2 * Math.PI))
            {
                leftRightRot -= (float)(2 * Math.PI);
            }
            else if (leftRightRot < (-2 * Math.PI))
            {
                leftRightRot += (float)(2 * Math.PI);
            }

            float upDownTemp = upDownRot - (rotationSpeed * mouseDifference.Y * timeDifference);
            if (upDownTemp < rightAngleRadians &&
                upDownTemp > -rightAngleRadians)
            {
                upDownRot = upDownTemp;
            }
        }

        /// <summary>
        /// Rotates the camera.
        /// </summary>
        /// <param name="point">The point about which to rotate.</param>
        /// <param name="degrees">How far to rotate.</param>
        public void rotateCameraAboutYAxisPoint(Vector2 point, float degrees)
        {
            if (degrees == 0.0f)
                return;

            if (degrees < 0.0f)
            {
                degrees = 360.0f + degrees;
            }

            Vector3 newPosition = position;
            float radians = MathHelper.ToRadians(degrees);

            float xAddition = (float)(Math.Cos(radians) * (newPosition.X - point.X) -
                Math.Sin(radians) * (newPosition.Z - point.Y));
            float yAddition = (float)(Math.Sin(radians) * (newPosition.X - point.X) +
                Math.Cos(radians) * (newPosition.Z - point.Y));

            System.Diagnostics.Debug.WriteLine("XAddition: " + xAddition + " YAddition: " + yAddition);

            newPosition.X = point.X + xAddition;
            newPosition.Z = point.Y + yAddition;

            position = newPosition;

            leftRightRot -= radians;

            while (leftRightRot > (2 * Math.PI))
            {
                leftRightRot -= (float)(2 * Math.PI);
            }

            while (leftRightRot < (-2 * Math.PI))
            {
                leftRightRot += (float)(2 * Math.PI);
            }

            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
        }

        public void addToCameraPosition(ref Vector3 toAdd)
        {
            float localMoveSpeed = moveSpeed;
            Vector3 oldPosition = position;
            Matrix cameraRotation = Matrix.Identity;
            if (NoClip)
            {
                cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);
                localMoveSpeed *= 2;
            }
            else
            {
                // We don't move in the direction we're looking if we are looking up. Discard the upDown
                // rotation.
                cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            }

            Vector3 rotatedVector = Vector3.Transform(toAdd, cameraRotation);
            oldPosition += localMoveSpeed * rotatedVector;

            position = oldPosition;
            if (HeadBobbing)
            {
                Vector3 headBobPos = position + (new Vector3(0, (float)(Math.Cos(mils*2.0f)/5.0f), 0));
                ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, headBobPos, ref matrices);
            }
            else
            {
                ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
            }
        }

        public void setCameraPosition(Vector3 newPosition, Vector3 offset)
        {
            Matrix cameraRotation = Matrix.Identity;
            cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            Vector3 afterOffset = newPosition - offset;
            position = afterOffset;
            ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
        }

        public override void Update(GameTime gTime)
        {
            mils += 0.1f;

            if (mils > 100.0f)
                mils = 0.0f;

            if (rotateEnabled)
                rotateCameraAboutYAxisPoint(new Vector2(rotationTarget.X, rotationTarget.Y), -1.0f);

            if (!HeadBobbing)
                ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);

            base.Update(gTime);
        }
    }
}
