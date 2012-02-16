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
        public BoundingSphere chestSphere
        {
            get;
            set;
        }
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
        public const float maxReachDistance = 2.5f;
        public const float rotationSpeed = 0.3f;
        public const float moveSpeed = 5.0f;
        // Top of the head
        public const float playerHeight = 4.75f;
        // 0.5f up from the bottom of the floor (has to do with the radius
        // of the sphere we use)
        public const float floorBoxHeight = 0.5f;
        public const float gravity = 0.35f;
        public const float rightAngleRadians = 1.57079633f;
        public const float chestSphereRadius = 0.75f;
        #endregion

        public Player(ref Vector3 position, ref Vector2 rotation, GraphicsDevice gDevice) :
            base(ref position, ref rotation, gDevice)
        {
            this.rotateEnabled = false;
            HeadBobbing = false;
            matrices = new MatrixDescriptor();
            // A bounding sphere right at the chest
            BoundingSphere chestSphere = new BoundingSphere(position, chestSphereRadius);
        }

        public void clickOnSomething(ref List<GameObject> toCheck)
        {
            Ray lookRay;
#region SetupTehRei
            int width = gDevice.Viewport.Width / 2;
            int height = gDevice.Viewport.Height / 2;
            Viewport vp = gDevice.Viewport;

            Vector3 pos1 = vp.Unproject(new Vector3(width, height / 2, 0),
                matrices.proj,
                matrices.view,
                matrices.world);
            Vector3 pos2 = vp.Unproject(new Vector3(width, height / 2, 1),
                matrices.proj,
                matrices.view,
                matrices.world);

            Vector3 dir = Vector3.Normalize(pos2 - pos1);
#endregion
            lookRay = new Ray(position, dir);
            float distanceToClosest = float.MaxValue;
            GameObject closest = null;

            foreach (GameObject go in toCheck)
            {
                foreach (BoundingBox bbox in go.Model.BBoxes)
                {
                    float? distanceToObj = lookRay.Intersects(bbox);

                    if (distanceToObj != null &&
                        distanceToObj < distanceToClosest &&
                        distanceToObj < Player.maxReachDistance)
                    {
                        closest = go;
                        distanceToClosest = (float)distanceToObj;
                        break;
                    }
                }
            }

            if (closest != null)
            {
                closest.interactedWith();
            }

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
            chestSphere = new BoundingSphere(position, chestSphereRadius);

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

            addToCameraPosPrecomputed(ref oldPosition);
        }

        public void addToCameraPosPrecomputed(ref Vector3 toSet)
        {
            position = toSet;
            chestSphere = new BoundingSphere(position, chestSphereRadius);
            if (HeadBobbing)
            {
                Vector3 headBobPos = position + (new Vector3(0, (float)(Math.Cos(mils * 2.0f) / 6.0f), 0));
                ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, headBobPos, ref matrices);
            }
            else
            {
                ModelUtil.UpdateViewMatrix(upDownRot, leftRightRot, position, ref matrices);
            }
        }

        public Vector3 examineFuturePos(ref Vector3 toAdd) 
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

            return oldPosition;
        }

        public void setCameraPosition(Vector3 newPosition, Vector3 offset)
        {
            Matrix cameraRotation = Matrix.Identity;
            cameraRotation = Matrix.CreateRotationX(0.0f) * Matrix.CreateRotationY(leftRightRot);
            Vector3 afterOffset = newPosition - offset;
            position = afterOffset;
            chestSphere = new BoundingSphere(position, chestSphereRadius);
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
