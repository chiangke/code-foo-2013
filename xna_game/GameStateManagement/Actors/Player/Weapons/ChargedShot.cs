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
    public class ChargedShot : Actor
    {
        Actor enemyPointer;
        Vector3 rotationAxis, targetPos;
        double rotationAnglerad, rotationAngledeg;
        float num, denum;   //test var
        Ship Firer;
        Random rand = new Random();
        Utils.Timer selfDestroy = new Utils.Timer();
        private Vector3 fireDirection;

        public ChargedShot(Game game)
            : base(game)
        {
            sMeshName = "missile";
            // TODO: Construct any child components here
        }

        public ChargedShot(Game game, Ship Firee)
            : base(game)
        {
            Firer = Firee;
            sMeshName = "Missile";
            #region Orientation of Missile
            Rotation = Firee.Rotation;
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)MathHelper.Pi);
            /*
            //Initial Rotation (so that it's aligned with the player's ship)//
            

            //Rotation to face the target//
           

            num = Vector3.Dot(Firee.worldMatrix.Forward, fireDirection);
            denum = Firee.worldMatrix.Forward.Length() * fireDirection.Length();
            rotationAnglerad = Math.Acos(num/denum);
            //rotationAngledeg = MathHelper.ToDegrees((float)rotationAnglerad);
            rotationAxis = Vector3.Cross(Firee.worldMatrix.Forward, fireDirection);
            rotationAxis.Normalize();
            //Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, (float)-rotationAnglerad);*/
            #endregion
            //Force = Vector3.Multiply(Firee.worldMatrix.Forward, 2.0f);
            //Velocity = Force + Ship.vShipVelocity;
            targetPos = Reticule.vTargetPosition;
            enemyPointer = (Actor)Reticule.enemyPointer;
            fireDirection = targetPos - Firee.Position;
            fireDirection.Normalize();
            Velocity = fireDirection*15;
            
            Position = Vector3.Add(Firee.Position, Vector3.Multiply(Force, 30.0f));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
            uniformScale = 2.0f;
            TerminalVelocity = 5.0f;
            bPhysicsDriven = false;
            Mass = 1.0f;
            selfDestroy.AddTimer("Self-Destroy", 5.0f, new Utils.TimerDelegate(SelfDestroy), false);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (enemyPointer!=null)
                 targetPos = enemyPointer.Position;
            fireDirection = targetPos - Firer.Position;
            fireDirection.Normalize();
            Velocity = fireDirection * 15;
            selfDestroy.Update(gameTime);
            base.Update(gameTime);
            CheckCollision();
        }

        //Will be called after the Missile lived for 5s
        public void SelfDestroy()
        {
            UnloadContent();
            Game.Components.Remove(this);
        }

        //Collision Behaviour//
        protected override void CheckCollision()
        {
            foreach (EnemyActor A in EnemyManager.enemyList)
            {
                if (WorldBounds.Intersects(A.WorldBounds))
                {
                    A.Damage(10);
                    SelfDestroy();
                    return;
                }
            }
        }
    }
}
