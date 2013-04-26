using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameStateManagement
{
    public class EnemyShipBehind : EnemyActor
    {
        private Random rand;
        static float zcoord;
        float forwardVelocity;
        public Utils.Timer shipBehindTimer;
        public EnemyShipBehind(Game game)
            : base(game)
        {
            sMeshName = "Ship";
            zcoord = Ship.ShipPosition.Z + 200;
            Position = new Vector3(0, 0, zcoord);
            shipBehindTimer = new Utils.Timer();
            rand = new Random();
        }

        public override void Initialize()
        {
            Position.X = rand.Next(-400, 400);
            Position.Y = rand.Next(-100, 100);
            shipBehindTimer.AddTimer("SlowDown Timer", 2.5f, SlowDown, false);
            shipBehindTimer.AddTimer("LaserTimer", 4.0f, FireLaser, true);
            shipBehindTimer.AddTimer("MoveTimer", 8.0f, GoAway, false);
            shipBehindTimer.AddTimer("DestroyTimer", 12.0f, DestroyShip, false);
            forwardVelocity = -5.0f;
            Velocity = new Vector3 (0,0,forwardVelocity);
            base.Initialize();
        }
        public void SlowDown()
        {
            forwardVelocity = Ship.forwardVelocity;
            Velocity = new Vector3(0, 0, forwardVelocity);
        }
        public void GoAway()
        {
            Velocity = new Vector3(3*rand.Next(-1, 1), 3*rand.Next(-1, 1), 3*rand.Next(-1, 1));
        }
        public override void Update(GameTime gameTime)
        {
            DeathCheck();
            //Position.Z += forwardVelocity;
           // if (Position.Z < Ship.ShipPosition.Z - 1000)
            //    forwardVelocity = Ship.forwardVelocity;
            shipBehindTimer.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void CheckCollision()
        {
            base.CheckCollision();
        }
        public void FireLaser()
        {
            NormalMissile m_Missile = new NormalMissile(Game, this, 0);
            EnemyManager.enemyProjectileList.Add(m_Missile);
            Game.Components.Add(m_Missile);
        }

        public void StopLaser()
        {
            shipBehindTimer.RemoveTimer("LaserTimer");
        }

        public void DestroyShip()
        {
            this.Damage(10);
        }
    }
}
