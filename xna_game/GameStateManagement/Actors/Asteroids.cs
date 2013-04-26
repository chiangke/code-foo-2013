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
    public class Asteroids : Actor
    {
        private Random rand;
        private Vector3 ForceNormalized;
        static int zcoord = -1000;

        public bool canCollide;
        public Utils.Timer CollisionToggle;

        private Ship ship;

        public Asteroids(Game game)
            : base(game)
        {
            Position = new Vector3(100, 0, zcoord);
            zcoord -=300;
            sMeshName = "Asteroid";
            ForceNormalized = new Vector3();
            worldMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
            rand = new Random();
            canCollide = true;
            CollisionToggle = new Utils.Timer();
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            bPhysicsDriven = false;
            Mass = 100f;
            TerminalVelocity = 1.5f;
            Position.X = rand.Next(-512, 512);
            Position.Y = rand.Next(-384, 384);
            /*} while (Vector3.DistanceSquared(Position, Ship.ShipPosition) < 40000);

            do
            {                
                Force.X = rand.Next(-10, 10);
                Velocity.X = Force.X;
                Force.Y = rand.Next(-10, 10);
                Velocity.Y = Force.Y;
                Velocity.Normalize();
                Velocity = Vector3.Zero; //test
            } while (Force.LengthSquared() < 25); */
            //Force.Normalize();   //Set equal speed to the Asteroids.
            //Set rotational Angle
            RotationAngle = MathHelper.PiOver4;


            //pew pew to the doo doo
            CollisionToggle.AddTimer("LaserTimer", 4.0f, FireLaser, true);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            CollisionToggle.Update(gameTime);
            base.Update(gameTime);
            RotationAxis = Vector3.UnitZ;
            RotationAxis.Normalize();
            //CheckCollision();
        }

        //Collison? Reflect: continue;//
        protected override void CheckCollision()
        {
            foreach(Asteroids A in SpawnManager.asteroidList)
                if(this != A && canCollide)
                    if (WorldBounds.Intersects(A.WorldBounds))
                    {
                        Force = A.Velocity;
                        A.Force = Velocity;
                        Velocity = Force;
                        Velocity.Normalize();
                        A.Velocity = A.Force;
                        A.Velocity.Normalize();

                        canCollide = false;
                        CollisionToggle.AddTimer("Collision Reset Timer", 1.0f, CollisionReset, false);
                        A.canCollide = false;
                        A.CollisionToggle.AddTimer("Collision Reset Timer", 1.0f, A.CollisionReset, false);
                        break;
                    }
            base.CheckCollision();
        }

        //Reset Collison bool//
        public void CollisionReset()
        {
            canCollide = true;
        }

        //set the ship to aim at here
        public void setShip(Ship s)
        {
            ship = s;
        }

        public void stopFiring()
        {
            CollisionToggle.RemoveTimer("LaserTimer");
        }

        //fires this object's laser at the ship's current position
        public void FireLaser()
        {
            EnemyMissile m_Missile = new EnemyMissile(Game, this, ship, 0);
            Game.Components.Add(m_Missile);
        }
    }
}
