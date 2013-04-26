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
    public class SpawnManager : Microsoft.Xna.Framework.GameComponent
    {
        protected Utils.Timer timer = new Utils.Timer();
        private static int max;
        public static int MAX { get { return max; } }
        private Random random = new Random();
        private Ship ship;
        private FollowMe followme;

        public static System.Collections.Generic.List<Asteroids> asteroidList {get; set;}

        public SpawnManager(Game game)
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
            max = 10;

            asteroidList = new List<Asteroids>();
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
            if (asteroidList.Count() < max)
            {
                Asteroids m_Asteroid = new Asteroids(Game);
                m_Asteroid.setShip(ship);
                asteroidList.Add(m_Asteroid);
                Game.Components.Add(m_Asteroid);
            }
        }
        public void setShip(Ship s)
        {
            ship = s;
            for (int i = 0; i < asteroidList.Count; i++)
            {
                asteroidList[i].setShip(ship);
            }
        }

        public void setFollowMe(FollowMe f)
        {
            followme = f;
        }
    }
}