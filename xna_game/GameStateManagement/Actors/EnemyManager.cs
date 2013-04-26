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
    public class EnemyManager : Microsoft.Xna.Framework.GameComponent
    {
        protected Utils.Timer timer = new Utils.Timer();
        private static int max;
        public static int MAX { get { return max; } }
        private Random random = new Random();
        private Ship ship;
        private FollowMe followme;
        private Skybox skybox;

        public static System.Collections.Generic.List<EnemyActor> enemyList {get; set;}
        public static System.Collections.Generic.List<EnemyProjectile> enemyProjectileList;

        public EnemyManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            max = 12;

            enemyList = new List<EnemyActor>();
            enemyProjectileList = new List<EnemyProjectile>();
            timer.AddTimer("SpawnTimer", 1.0f, new Utils.TimerDelegate(SpawnAsteroids), true);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            timer.Update(gameTime);
            base.Update(gameTime);
        }

        public void SpawnAsteroids()
        {
            if (enemyList.Count() < max)
            {
                int rand = random.Next(3);
                EnemyActor m_enemy;
                if (rand == 0) 
                    m_enemy = new EnemyShipBottom(Game);
                //else if (rand == 1)
                //    m_enemy = new EnemyShipSide(Game);
                else
                    m_enemy = new EnemyShipBehind(Game);
                m_enemy.setShip(ship);
                enemyList.Add(m_enemy);
                Game.Components.Add(m_enemy);
                //Asteroids m_Asteroid = new Asteroids(Game);
                //m_Asteroid.setShip(ship);
                //enemyList.Add(m_Asteroid);
                //Game.Components.Add(m_Asteroid);
            }
        }
        public void setShip(Ship s)
        {
            ship = s;
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].setShip(ship);
            }
        }

        public void setFollowMe(FollowMe f)
        {
            followme = f;
        }

        public void setSkybox(Skybox s)
        {
            skybox = s;
        }
    }
}