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
    public class Bomb : Actor
    {
        Utils.Timer bombTimer = new Utils.Timer();
        public Bomb(Game game, Ship Firee)
            : base(game)
        {
            sMeshName = "Bomb";
           
            Force = Vector3.Multiply(Firee.worldMatrix.Forward, 2.0f);
            Velocity = Force + Ship.vShipVelocity;
            Position = Vector3.Add(Firee.Position, Vector3.Multiply(Force, 35.0f));
            Rotation = Firee.Rotation;
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)MathHelper.Pi);
            // TODO: Construct any child components here
        }
        /*
        public Missile(Game game, Ship Firee)
            : base(game)
        {
            sMeshName = "Missile";
            Rotation = Firee.Rotation;
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)MathHelper.Pi);
            Force = Vector3.Multiply(Firee.worldMatrix.Forward, 2.0f);
            Velocity = Force;
            Position = Vector3.Add(Firee.Position, Vector3.Multiply(Force, 35.0f));
        }
        */

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
            TerminalVelocity = 5.0f;
            bPhysicsDriven = true;
            Mass = 1.0f;
            /*
            uniformScale = 2.0f;
            TerminalVelocity = 5.0f;
            bPhysicsDriven = true;
            Mass = 1.0f;
             
            */
            uniformScale = 0.1f;
            bombTimer.AddTimer("Explode", 2.0f, new Utils.TimerDelegate(Explode), false);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            bombTimer.Update(gameTime);
            base.Update(gameTime);
            CheckCollision();

        }

        //Will be called after the Missile lived for 5s
        public void Explode()
        {
            bombTimer.AddTimer("Self-Remove", 2.0f, new Utils.TimerDelegate(SelfRemove), false);
            bombTimer.AddTimer("Expand", 0.05f, new Utils.TimerDelegate(Expand), true);
            
        }

        public void SelfRemove()
        {
            UnloadContent();
            Game.Components.Remove(this);
        }
        public void Expand()
        {
            //uniformScale = (float)Math.Log(uniformScale);
            uniformScale += 1.0f;
            //WorldBounds.Center = Position;
            WorldBounds.Radius = ModelBounds.Radius * uniformScale;
            //WorldBounds.Radius+= 1.0f;
        }
        //Collision Behaviour//
        protected override void CheckCollision()
        {
            //Destroy all enemy's bullet upon contact//
            foreach (EnemyProjectile A in EnemyManager.enemyProjectileList)
            {
                if(WorldBounds.Intersects(A.WorldBounds))
                {
                    EnemyManager.enemyProjectileList.Remove(A);
                    Game.Components.Remove(A);
                    return;
                }
            }
            foreach (EnemyActor A in EnemyManager.enemyList)
            {
                if (WorldBounds.Intersects(A.WorldBounds))
                {
                    //Game.Components.Remove(A);
                    //EnemyManager.enemyList.Remove(A);
                    A.Damage(1);
                    //A.stopFiring();
                }
            }
            return;
        }
    }
}
