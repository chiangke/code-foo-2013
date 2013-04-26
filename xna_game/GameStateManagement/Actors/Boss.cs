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
    public class Boss : Actor
    {
        private Utils.Timer bossTimer;
        private bool canFire;
        private bool destroyed;
        private bool invincible;
        private int health;
        private bool isInvincible;

        private static Vector3 bossPosition;
        private static Vector3 vBossVelocity;
        private static Matrix BossWorldMatrix;
        const float backwardVelocity = -2.1f;

        private Random rand;
        private Vector3 ForceNormalized;
        static int zcoord = -1000;

        public bool canCollide;

        private Ship ship;

        public Boss(Game game)
            : base(game)
        {
            sMeshName = "Starship";
            RotationAxis = Vector3.UnitY;
            RotationAngle = 0.0f;
            uniformScale = 4.0f;

            TerminalVelocity = 5.0f;
            bPhysicsDriven = false;
            canFire = true;
            invincible = false;
            destroyed = false;
            health = 1000;

            bossTimer = new Utils.Timer();

            bossPosition = Vector3.Zero;
            Velocity = new Vector3(0, 0, backwardVelocity);
            BossWorldMatrix = Matrix.Identity;

            Position = new Vector3(0, 0, zcoord);
            ForceNormalized = new Vector3();
            worldMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
            rand = new Random();
            canCollide = true;
        }

        public override void Initialize()
        {
            //pew pew to the doo doo
            bossTimer.AddTimer("LaserTimer", 0.2f, FireLaser, true);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            bossTimer.Update(gameTime);
            bossPosition = Position;
            BossWorldMatrix = worldMatrix;
            base.Update(gameTime);
        }

        //set the ship to aim at here
        public void setShip(Ship s)
        {
            ship = s;
        }

        public void stopFiring()
        {
            bossTimer.RemoveTimer("LaserTimer");
        }

        //fires this object's laser at the ship's current position
        public void FireLaser()
        {
            EnemyMissile m_Missile = new EnemyMissile(Game, this, ship, 1);
            Game.Components.Add(m_Missile);
        }
    }
}
