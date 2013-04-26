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
    public class FollowMe : Actor
    {
        public static Vector3 FollowMePosition;
        public static Matrix FollowMeWorldMatrix;
        private EnemyManager EnemyManager;
        const float forwardVelocity = -2.1f;

        public FollowMe(Game game)
            : base(game)
        {
            sMeshName = "Ship";
            RotationAxis = Vector3.UnitY;
            RotationAngle = 0.0f;

            TerminalVelocity = 5.0f;
            bPhysicsDriven = false;
            timer = new Utils.Timer();

            FollowMePosition = new Vector3(0,0,200);
            //vShipVelocity = Vector3.Zero;
            Velocity = new Vector3(0, 0, forwardVelocity);
            //Velocity = Vector3.Zero;
            FollowMeWorldMatrix = Matrix.Identity;
            
            // TODO: Construct any child components here
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
            FollowMePosition = Position;
            FollowMeWorldMatrix = worldMatrix;
            base.Update(gameTime);
        }

        //Firing a missile//

        public void setEnemyManager(EnemyManager s)
        {
            EnemyManager = s;
            EnemyManager.setFollowMe(this);
        }
    }
}
