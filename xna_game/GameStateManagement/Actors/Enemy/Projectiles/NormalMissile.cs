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
    public class NormalMissile : EnemyProjectile
    {
        Random rand = new Random();
        Utils.Timer selfDestroy = new Utils.Timer();

        private int index;

        public NormalMissile(Game game)
            : base(game)
        {
            sMeshName = "Missile";
            // TODO: Construct any child components here
        }

        public NormalMissile(Game game, Actor Firee, int i)
            : base(game)
        {
            sMeshName = "Missile";
            Vector3 Direction = Vector3.Zero;

            index = i;

            Direction = Ship.ShipPosition - Firee.Position;
            if (index == 1)
                Direction.Z -= 450;
            Direction.Normalize();

            this.worldMatrix.Forward = Direction;
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)MathHelper.Pi);
            if (index == 1)
                Force = Vector3.Multiply(Direction, 2.0f);
            else
                Force = Vector3.Multiply(Direction, 5.0f);
            Velocity = Force;
            Position = Vector3.Add(Firee.Position, Vector3.Multiply(Force, 35.0f));
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
            bPhysicsDriven = true;
            Mass = 1.0f;
            if (index == 1)
            {
                selfDestroy.AddTimer("Self-Destroy", 7.0f, new Utils.TimerDelegate(SelfDestroy), false);
                TerminalVelocity = 20.0f;
            }
            else
            {
                selfDestroy.AddTimer("Self-Destroy", 5.0f, new Utils.TimerDelegate(SelfDestroy), false);
                TerminalVelocity = 12.0f;
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

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
    }
}
