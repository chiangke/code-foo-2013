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
    public class Boss : EnemyActor
    {
        private Utils.Timer bossTimer;
        private bool canFire;
        private bool destroyed;
        private bool invincible;
        public static int Bosshealth;
        private bool isInvincible;

        private static Vector3 bossPosition;
        private static Vector3 vBossVelocity;
        private static Matrix BossWorldMatrix;
        const float backwardVelocity = -2.1f;

        private Random rand;
        private Vector3 ForceNormalized;
        static float zcoord = -1000;

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
            Bosshealth = health;

            bossTimer = new Utils.Timer();

            bossPosition = Vector3.Zero;
            Velocity = new Vector3(0, 0, backwardVelocity);
            BossWorldMatrix = Matrix.Identity;

            zcoord = Ship.ShipPosition.Z - 1000;
            Position = new Vector3(0, 0, zcoord);
            ForceNormalized = new Vector3();
            worldMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
            rand = new Random();
            canCollide = true;
        }

        public override void Initialize()
        {
            bossTimer.AddTimer("BossStateTimer", 3.0f, ChooseState, false);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            bossTimer.Update(gameTime);
            bossPosition = Position;
            BossWorldMatrix = worldMatrix;
            Bosshealth = health;
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
            bossTimer.AddTimer("BossStateTimer", 3.0f, ChooseState, false);
        }

        public void ChooseState()
        {
            int x = 0;
            x = rand.Next(0, 3);
            if (x < 2)
            {
                bossTimer.AddTimer("LaserTimer", 0.2f, FireLaser, true);
                bossTimer.AddTimer("RemoveLaserTimer", 5.0f, stopFiring, false);
            }
            else if (x < 3)
            {
                bossTimer.AddTimer("SweepTimer", 0.1f, FireLazor, false);
            }
        }

        //fires this object's laser at the ship's current position
        public void FireLaser()
        {
            NormalMissile m_Missile = new NormalMissile(Game, this, 1);
            EnemyManager.enemyProjectileList.Add(m_Missile);
            Game.Components.Add(m_Missile);
        }
        public void FireLazor()
        {
            Laser m_Laser = new Laser(Game, this, 2);
            EnemyManager.enemyProjectileList.Add(m_Laser);
            Game.Components.Add(m_Laser);
            resetBossTimer();
        }

        public void resetBossTimer()
        {
            bossTimer.AddTimer("BossStateTimer", 3.0f, ChooseState, false);
        }
    }
}
