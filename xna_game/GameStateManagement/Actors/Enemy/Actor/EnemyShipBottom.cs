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
    public class EnemyShipBottom : EnemyActor
    {
        private Random rand;
        static float zcoord;
        public Utils.Timer shipBottomTimer;

        public EnemyShipBottom(Game game)
            : base(game)
        {
            sMeshName = "Ship";
            zcoord = Ship.ShipPosition.Z - 2000;
            Position = new Vector3(100, 0, zcoord);
            shipBottomTimer = new Utils.Timer();
            rand = new Random();
        }

        public override void Initialize()
        {
            Position.X = rand.Next(-480, 480);
            Position.Y = rand.Next(-320, 100);
            shipBottomTimer.AddTimer("LaserTimer", 2.0f, FireLaser, true);
            shipBottomTimer.AddTimer("RemoveTimer", 10.0f, StopLaser, false);
            shipBottomTimer.AddTimer("DestroyTimer", 12.0f, DestroyShip, false);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            DeathCheck();
            shipBottomTimer.Update(gameTime);
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
            shipBottomTimer.RemoveTimer("LaserTimer");
        }

        public void DestroyShip()
        {
            this.Damage(10);
        }
    }
}
