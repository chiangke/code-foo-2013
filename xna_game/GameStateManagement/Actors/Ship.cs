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
    public class Ship : Actor
    {
        private Utils.Timer shipTimer;
        private bool canFire;
        public bool destroyed;
        private bool invincible;
        private bool locktoTarget;
        
        public static int health;
        public static int lives;
        public static int bombs;
        public bool isInvincibile { get { return invincible; } }
        public static Vector3 ShipPosition;
        public static Vector3 vShipVelocity;  //This'll get updated so that other classes that use this
        public static Matrix ShipWorldMatrix;
        private SpawnManager spawnManager;
        private Boss boss;
        const float forwardVelocity =-2.1f;

        //Barrel Roll use//
        private bool barrelRollable;
        public bool barrelRolling;
        private float rollAngle;

        public Ship(Game game)
            : base(game)
        {
            sMeshName = "Starship";        
            RotationAxis = Vector3.UnitY;
            RotationAngle = 0.0f;
            
            TerminalVelocity = 5.0f;
            bPhysicsDriven = false;
            timer = new Utils.Timer();
            canFire = true;
            invincible = false;
            destroyed = false;
            locktoTarget = false;
            barrelRollable = true;
            barrelRolling = false;
            health = 20;
            lives = 3;
            bombs = 99;

            shipTimer = new Utils.Timer();

            shipTimer.AddTimer("CameraLock", 3.0f, new Utils.TimerDelegate(enableTargetTracking), false);

            ShipPosition = Vector3.Zero;
            //vShipVelocity = Vector3.Zero;
            Velocity = new Vector3(0, 0, forwardVelocity);
            //Velocity = Vector3.Zero;
            ShipWorldMatrix = Matrix.Identity;
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
            GameplayScreen.soundbank.PlayCue("Ship_Spawn");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            spawnManager.setShip(this);
            boss.setShip(this);

            shipTimer.Update(gameTime);
            CheckCollision();
            ShipPosition = Position;
            //vShipVelocity = Velocity;
            ShipWorldMatrix = worldMatrix;
            base.Update(gameTime);
            rotationCheck(gameTime);
        }

        //Firing a missile//
        public void ShipFire()
        {
            if (canFire && !destroyed)
            {
                canFire = false;
                #region Create Missile
                PlayerMissile m_Missile = new PlayerMissile(Game, this);
                Game.Components.Add(m_Missile);
                GameplayScreen.soundbank.PlayCue("Ship_Missile");
                #endregion Create Missile
                shipTimer.AddTimer("Missile Cooldown", 0.2f, new Utils.TimerDelegate(FireEnable), false); //Reset canFire after 1 sec
            }

            else
                return;
        }

        public void ShootBomb()
        {
            if (bombs > 0)
            {
                Bomb m_Bomb = new Bomb(Game, this);
                Game.Components.Add(m_Bomb);
                bombs--;
            }
        }


        //Called every second to enable firing.
        private void FireEnable()
        {
            canFire = true;
        }

        private void TurnOffInvincibility()
        {
            invincible = false;
            //destroyed = false;
        }
        //Collision Check//
        protected override void CheckCollision()
        {
            if (!invincible && !destroyed && !barrelRolling)
            {
                foreach (Asteroids A in SpawnManager.asteroidList)
                    if (WorldBounds.Intersects(A.WorldBounds))
                    {
                        setInvincible();
                        base.CheckCollision();
                    }
            }
        }

        public void setSpawnManager(SpawnManager s)
        {
            spawnManager = s;
            spawnManager.setShip(this);
        }

        public void setBoss(Boss b)
        {
            boss = b;
            boss.setShip(this);
        }

        public void setInvincible()
        {
            health -= 10;
            if (health <= 0) {
                destroyed = true;
                this.Visible = false;
                shipTimer.AddTimer("Respawn Ship", 3.0f, new Utils.TimerDelegate(respawnShip), false);
            }
            else
            {
                invincible = true;
                shipTimer.AddTimer("Turn Off Invincibility", 3.0f, new Utils.TimerDelegate(TurnOffInvincibility), false);
            }
        }

        public void respawnShip()
        {
            this.Visible = true;
            destroyed = false;
            invincible = true;
            lives--;
            health = 100;
            shipTimer.AddTimer("Turn Off Invincibility", 3.0f, new Utils.TimerDelegate(TurnOffInvincibility), false);
        }

        private void enableBarrelRoll()
        {
            barrelRollable = true;
        }

        private void enableTargetTracking()
        {
            locktoTarget = true;
        }

        public void BarrelRoll()
        {
            if (barrelRollable)
            {
                rollAngle = 2 * MathHelper.Pi;
                barrelRollable = false;
                barrelRolling = true;
                shipTimer.AddTimer("BarrelRoll cooldown", 2.0f, new Utils.TimerDelegate(enableBarrelRoll), false);
            }
            else
                return;
        }

        //Rotate such that it will always face the reticule
        public void rotationCheck(GameTime gameTime)
        {
            if (rollAngle != 0)
                barrelRotation(gameTime);
            else
            {
                rotateToReticle(gameTime);
                rotateUpright(gameTime);
            }
        }
        
        //Barrel Roll rotation//
        private void barrelRotation(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float rotationAngle;
            Vector3 rotationAxis = FollowMe.FollowMeWorldMatrix.Forward;
            if (rollAngle > MathHelper.PiOver4 / 64)
            {
                rotationAngle = rollAngle * deltaTime * 8;
                rollAngle -= rotationAngle;
            }
            else
            {
                rotationAngle = rollAngle;
                rollAngle = 0;
                barrelRolling = false;
            }
            Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle);
        }
        //Rotate towards the reticle//
        private void rotateToReticle(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 vTargetDir = Reticule.vTargetPosition - Position;
            vTargetDir.Normalize();
            float rotationAngle = (float)Math.Acos(Vector3.Dot(worldMatrix.Forward, vTargetDir) / (worldMatrix.Forward.Length() * vTargetDir.Length()));
            Vector3 rotationAxis = Vector3.Cross(worldMatrix.Forward, vTargetDir);
            rotationAxis.Normalize();
            if (locktoTarget)
                Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle * deltaTime * 8);
        }

        //Rotate so that it will tend to stay upright//
        private void rotateUpright(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float rotationAngle = (float)Math.Acos(Vector3.Dot(worldMatrix.Forward, FollowMe.FollowMeWorldMatrix.Up) / (worldMatrix.Forward.Length() * FollowMe.FollowMeWorldMatrix.Up.Length()));
            Vector3 rotationAxis = Vector3.Cross(worldMatrix.Forward, FollowMe.FollowMeWorldMatrix.Up);
            rotationAxis.Normalize();
            if (locktoTarget)
                Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle * deltaTime * 0.5f);
        }
    }
}
