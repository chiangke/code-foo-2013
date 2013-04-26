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
    public class EnemyActor : Enemy
    {
        const int DEFAULTHEALTH = 10;
        private Ship ship;
        
        public int health;

        public EnemyActor(Game game)
            : base(game)
        {
            ship = null;
            health = DEFAULTHEALTH;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            DeathCheck();
            base.Update(gameTime);
        }

        protected override void CheckCollision()
        {
            base.CheckCollision();
        }

        protected void DeathCheck()
        {
            if (health <= 0)
            {
                EnemyManager.enemyList.Remove(this);
                selfDestroy();
            }
        }

        //set the ship to aim at here
        public void setShip(Ship s)
        {
            ship = s;
        }

        public void Damage(int damage)
        {
            health -= damage;
        }
    }
}
