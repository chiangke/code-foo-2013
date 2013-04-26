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
    public class Laser : EnemyProjectile
    {
        Random rand = new Random();
        Utils.Timer selfDestroy = new Utils.Timer();

        private int index;

        public Laser(Game game)
            : base(game)
        {
            sMeshName = "Missile";
            // TODO: Construct any child components here
        }

        public Laser(Game game, Actor Firee, int i)
            : base(game)
        {
            sMeshName = "Missile";
            Vector3 Direction = new Vector3(1,0,0);

            index = i;

            this.worldMatrix.Forward = new Vector3(0,0,1);
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)MathHelper.Pi);
            Force = Vector3.Multiply(Direction, 5.0f);
            Velocity = new Vector3(0,0,-2.1f);
            Position = Vector3.Add(Firee.Position+new Vector3(-400, -600, 0), Vector3.Multiply(Force, 35.0f));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
            uniformScale = 200.0f;
            bPhysicsDriven = true;
            Mass = 1.0f;
            selfDestroy.AddTimer("Self-Destroy", 5.0f, new Utils.TimerDelegate(SelfDestroy), false);
            TerminalVelocity = 5.0f;
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
