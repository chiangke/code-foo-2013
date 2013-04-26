using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class LookAtCamera : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Constant//
        const float  VERTICALFOLLOWDIST = 200.0f;
        const float  HORIZONTALFOLLOWDIST = 300.0f;
        const float  SPRINGCONST = 128.0f;

        const float MAXSHAKEAMMOUNT = 100.0f;
        const float SHAKESWITCHTIME = 75f;     //In milliseconds
        const float SHAKELERPACCELERATION = 1.0f;
        
        //Basic Members//
        public Vector3 cameraPosition;
        public Vector3 cameraForward;
        public Vector3 cameraLeft;
        public Vector3 cameraUp;
        public Vector3 cameraDisplacement;
        public Vector3 springAccel;
        public Vector3 cameraVelocity;
        public Vector3 FollowPosition;
        public Vector3 Target;

        public enum lockOnTarget { Ship, FollowMe, ChangeA, ChangeB }; // ChangeA -> Camera rotates to face the Ship
                                                                       // ChangeB -> Camera rotates to face the FollowMe
        public lockOnTarget target;

        //Shake use//
        public enum ShakeState { One, Two, Three, Four, Five, Six, None }; //Five shake states. One is entry
        public static ShakeState shakeState;
        private Vector3 prevShakeTarget;
        private double shakeUpdate;
        private float shakeLerp;

        private float dampConstant = (float)(2.0f * Math.Sqrt(SPRINGCONST));

        public Matrix LookAtMatrix;
        //Rotation use//
        private float lerpValue;

        public LookAtCamera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here

            //Initalizing Vectors//
            cameraPosition = Vector3.Zero;
            cameraForward = Vector3.Zero;
            cameraLeft = Vector3.Zero;
            cameraUp = Vector3.Zero;
            cameraDisplacement = Vector3.Zero;
            springAccel = Vector3.Zero;
            cameraVelocity = Vector3.Zero;
            FollowPosition = Vector3.Zero;
            Target = Vector3.Zero;
            target = lockOnTarget.FollowMe;

            //Shake variables//
            shakeState = ShakeState.None;
            prevShakeTarget = Vector3.Zero;
            shakeUpdate = 0;
            shakeLerp = 0;

            //Initialize matrix//
            LookAtMatrix = Matrix.Identity;
            lerpValue = 0f;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Matrix FollowWorldMatrix = Matrix.Identity;
            Vector3 FollowUp;
            Vector3 FollowForward;

            if (target != lockOnTarget.Ship)
            {
                FollowWorldMatrix = FollowMe.FollowMeWorldMatrix;
                FollowPosition = FollowMe.FollowMePosition;
            }

            else if (target == lockOnTarget.Ship)
            {
                FollowWorldMatrix = Ship.ShipWorldMatrix;
                FollowPosition = Ship.ShipPosition;
            }
            FollowUp = FollowWorldMatrix.Up;
            FollowForward = FollowWorldMatrix.Forward;
            Vector3 cameraIdealPosition;

            FollowUp.Normalize();
            FollowForward.Normalize();

            cameraIdealPosition = FollowPosition - FollowForward * HORIZONTALFOLLOWDIST
                             + FollowUp * VERTICALFOLLOWDIST;
            cameraDisplacement = cameraPosition - cameraIdealPosition;
            springAccel = (-SPRINGCONST * cameraDisplacement) - (dampConstant * cameraVelocity);
            cameraVelocity += springAccel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            cameraPosition += cameraVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            RotationCheck(gameTime);

            ShakeCheck(gameTime);
            //cameraForward = FollowPosition - cameraPosition;
            cameraForward = Target - cameraPosition;
            cameraForward.Normalize();
            cameraLeft = Vector3.Cross(FollowUp, cameraForward);
            cameraUp = Vector3.Cross(cameraForward, cameraLeft);

            LookAtMatrix = Matrix.CreateLookAt(cameraPosition, Target, cameraUp);

            base.Update(gameTime);
        }

        //Function to rotate to ship or back to FollowMe. Doesn't work yet!//
        public void RotationCheck(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (target == lockOnTarget.ChangeA)
            {
                if (lerpValue < 0.5f)
                    lerpValue += 0.6f * deltaTime;
                else
                    lerpValue = 0.5f; 
            }
            else
            {
                if (lerpValue > 0)
                    lerpValue -= 1.1f * deltaTime;
                else
                    lerpValue = 0;
            }
            Target = Vector3.Lerp(FollowPosition, Ship.ShipPosition, lerpValue);
            
        }

        #region Shake
        public void ShakeCheck(GameTime gameTime)
        {
            if (shakeState != ShakeState.None)  //What to do to shake
            {
                Vector3 shakeTarget = Vector3.Zero;
                Matrix targetMatrix;

                //Set target matrix//
                if (target != lockOnTarget.Ship)
                    targetMatrix = FollowMe.FollowMeWorldMatrix;
                else
                    targetMatrix = Ship.ShipWorldMatrix;

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                //Update lerp//
                if (shakeState != ShakeState.Six)
                {
                    if (shakeLerp < 1)
                        shakeLerp += SHAKELERPACCELERATION * deltaTime;
                    else
                        shakeLerp = 1f;
                }
                else
                {
                    if (shakeLerp < 1)
                        shakeLerp += 20 * SHAKELERPACCELERATION * deltaTime;
                    else
                        shakeLerp = 1f;
                }


                switch (shakeState) //Different vibration
                {
                    case ShakeState.One: shakeUpdate = gameTime.TotalGameTime.TotalMilliseconds + SHAKESWITCHTIME;
                        ShakeStateUpdate();
                        break;

                    case ShakeState.Two: shakeTarget = Target /*targetMatrix.Left * MAXSHAKEAMMOUNT * 1.5f*/ - targetMatrix.Up * MAXSHAKEAMMOUNT;
                                         Target = Vector3.Lerp(prevShakeTarget, shakeTarget, shakeLerp);  //Use previous target here for smooth interpolation
                                         break;

                    case ShakeState.Three: shakeTarget = Target /*+ targetMatrix.Left * MAXSHAKEAMMOUNT * 2.0f*/ + targetMatrix.Up * MAXSHAKEAMMOUNT;
                                           Target = Vector3.Lerp(prevShakeTarget, shakeTarget, shakeLerp);  //Use previous target here for smooth interpolation
                                           break;

                    case ShakeState.Four: shakeTarget = Target /*- targetMatrix.Left * MAXSHAKEAMMOUNT */ + targetMatrix.Up * MAXSHAKEAMMOUNT;
                                          Target = Vector3.Lerp(prevShakeTarget, shakeTarget, shakeLerp);
                                          break;

                    case ShakeState.Five: shakeTarget = Target /*+ targetMatrix.Left * MAXSHAKEAMMOUNT */ - targetMatrix.Up * MAXSHAKEAMMOUNT;
                                          Target = Vector3.Lerp(prevShakeTarget, shakeTarget, shakeLerp);
                                          break;
                    
                    case ShakeState.Six: shakeTarget = Target; //Return to normal state
                                          Target = Vector3.Lerp(prevShakeTarget, shakeTarget, shakeLerp);
                                          break;
                    default: break;
                }

                if (shakeUpdate < gameTime.TotalGameTime.TotalMilliseconds) //When to next change vibration direction
                {
                    shakeUpdate = gameTime.TotalGameTime.TotalMilliseconds + SHAKESWITCHTIME;
                    ShakeStateUpdate();
                }
            }

            else  //Check for Ship state whether it's damaged
                if (Ship.state == Ship.State.Damaged)
                    shakeState = ShakeState.One;
                
        }

        private void ShakeStateUpdate()  //Increment states
        {
            if (shakeState == ShakeState.One)
                shakeState = ShakeState.Two;
            else if (shakeState == ShakeState.Two)
                shakeState = ShakeState.Three;
            else if (shakeState == ShakeState.Three)
                shakeState = ShakeState.Four;
            else if (shakeState == ShakeState.Four)
                shakeState = ShakeState.Five;
            else if (shakeState == ShakeState.Five)
                shakeState = ShakeState.Six;
            else if (shakeState == ShakeState.Six)
                shakeState = ShakeState.None;

            shakeLerp = 0;
            prevShakeTarget = Target;   //Save the lastest aim target for smooth interpolation later
        }
        #endregion
    }
}
