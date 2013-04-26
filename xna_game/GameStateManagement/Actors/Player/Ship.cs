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
        //Shake use//
        const float SHAKESWITCHTIME = .10f;     //In seconds
        const float SHAKEANGLEACCELERATION = 5.0f;

        private Utils.Timer shipTimer;
        private bool canFire;
        public bool destroyed;
        private bool invincible;
        private bool locktoTarget;
        private bool charging;
        private bool charged;
        
        public static int health;
        public static int lives;
        public static int bombs;
        public bool isInvincibile { get { return invincible; } }
        public static Vector3 ShipPosition;
        public static Vector3 vShipVelocity;  //This'll get updated so that other classes that use this
        public static Matrix ShipWorldMatrix;

        public enum State { Normal1, Normal2, Damaged, Invincible1, Invincible2, Destroyed, BarrelRolling }; //Normal1 = idle. Normal2 = during barrel roll cooldown. Same thing for Invincible
        public static State state;
        public enum ShakeState { Enter, One, Two, Three, Exit, None };
        public static ShakeState shakeState;
        private float shakeTotalAngle;

        public enum LazorState {uncharged, charging, charged };
        public static LazorState lazorState; 

        private EnemyManager EnemyManager;
        private Boss boss;
        public static float forwardVelocity =-2.1f;

        //Barrel Roll use//
        //private bool barrelRollable;
        //public bool barrelRolling;
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

            health = 100;
            lives = 3;
            bombs = 3;

            shipTimer = new Utils.Timer();

            shipTimer.AddTimer("CameraLock", 3.0f, new Utils.TimerDelegate(enableTargetTracking), false);

            ShipPosition = Vector3.Zero;

            Velocity = new Vector3(0, 0, forwardVelocity);

            ShipWorldMatrix = Matrix.Identity;

            state = State.Normal1;
            shakeState = ShakeState.None;
            shakeTotalAngle = 0;
            lazorState = LazorState.uncharged;
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
            //GameplayScreen.soundbank.PlayCue("Ship_Spawn");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            EnemyManager.setShip(this);

            if (boss != null)
                boss.setShip(this);

            shipTimer.Update(gameTime);
            CheckCollision();
            CheckDeath();
            ShipPosition = Position;
            //vShipVelocity = Velocity;
            ShipWorldMatrix = worldMatrix;
            base.Update(gameTime);
            rotationCheck(gameTime);
            ShakeCheck(gameTime);
        }

        //Firing a missile//
        public void ShipFire()
        {
            if (state != State.Destroyed && canFire)
            {
                canFire = false;
                #region Create Missile
                PlayerMissile m_Missile = new PlayerMissile(Game, this);
                Game.Components.Add(m_Missile);
                //GameplayScreen.soundbank.PlayCue("Ship_Missile");
                #endregion Create Missile
                shipTimer.AddTimer("Missile Cooldown", 0.2f, new Utils.TimerDelegate(FireEnable), false); //Reset canFire after 0.2 sec
            }

            else
                return;
        }

        public void ChargeLazor()
        {
            if (state != State.Destroyed && lazorState == LazorState.uncharged)
            {
                lazorState = LazorState.charging;
                shipTimer.AddTimer("Lazor Charge", 1.0f, new Utils.TimerDelegate(LazorCharged), false); //Fully charged after held for 1s
            }
        }
        public void UnchargeLazor()
        {
            if (lazorState == LazorState.charging)
            {
                lazorState = LazorState.uncharged;
                shipTimer.RemoveTimer("Lazor Charge");
            }
            else if (lazorState == LazorState.charged)
            {
                fireChargedLazor();
                lazorState = LazorState.uncharged;
            }
        }
        public void fireChargedLazor()
        {
            ChargedShot chargeShot = new ChargedShot(Game, this);
            Game.Components.Add(chargeShot);
        }
        public void ShootBomb()
        {
            if (bombs > 0 && state != State.Destroyed)
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

        private void LazorCharged()
        {
            lazorState = LazorState.charged;
        }

        private void TurnOffInvincibility()
        {
            //invincible = false;
            //destroyed = false;
            state = State.Normal1;
        }
        //Collision Check//
        protected override void CheckCollision()
        {
            if (state == State.Normal1 || state == State.Normal2 )//&& !barrelRolling)
            {
                foreach (EnemyActor E in EnemyManager.enemyList)
                    if (WorldBounds.Intersects(E.WorldBounds))
                    {
                        damaged(10);
                        return;
                    }


                //Josh's new and improved missle ship collision detector
                foreach (EnemyProjectile E in EnemyManager.enemyProjectileList) {
                    foreach (BoundingSphere OwnBounding in BoneSpheres) {
                        foreach (BoundingSphere EnemyBounding in E.BoneSpheres) {
                            if (OwnBounding.Intersects(EnemyBounding)) {
                                damaged(10);
                                return;
                            }
                        }
                    }
                }
//                    if(WorldBounds.Intersects(E.WorldBounds))
//                    {
//                        damaged(10);
//                        return;
//                    }
            }
            base.CheckCollision();
        }

        //Death Check//
        void CheckDeath()
        {
            if (health <= 0)
            {
                state = State.Destroyed;
                //destroyed = true;
                this.Visible = false;
                shipTimer.AddTimer("Respawn Ship", 3.0f, new Utils.TimerDelegate(respawnShip), false);
            }
        }

        public void setEnemyManager(EnemyManager s)
        {
            EnemyManager = s;
            EnemyManager.setShip(this);
        }

        public void setBoss(Boss b)
        {
            boss = b;
            boss.setShip(this);
        }

        //default function in taking damage
        public void damaged(int damage = 10)
        {
            state = State.Damaged;
            LookAtCamera.shakeState = LookAtCamera.ShakeState.One;
            shakeState = ShakeState.Enter;
            health -= damage;
            setInvincible();
        }

        //Set the ship into invincible state//
        public void setInvincible()
        {
            state = State.Invincible1;
            //invincible = true;
            shipTimer.AddTimer("Turn Off Invincibility", 3.0f, new Utils.TimerDelegate(TurnOffInvincibility), false);
        }

        public void respawnShip()
        {
            this.Visible = true;
            //destroyed = false;
            //invincible = true;
            state = State.Invincible1;
            lives--;
            health = 100;
            shipTimer.AddTimer("Turn Off Invincibility", 3.0f, new Utils.TimerDelegate(TurnOffInvincibility), false);
        }

        #region Rotation & Barrel Roll
        private void enableBarrelRoll()
        {
            state = State.Normal1;
            //barrelRollable = true;
        }

        private void enableTargetTracking()
        {
            locktoTarget = true;
        }

        //Excecute Barrel roll
        public void BarrelRoll()
        {
            if (state == State.Normal1 || state == State.Invincible1)
            {
                rollAngle = 2 * MathHelper.Pi;
                //barrelRollable = false;
                //barrelRolling = true;
                if (state != State.Invincible1)
                {
                    state = State.BarrelRolling;
                    shipTimer.AddTimer("BarrelRoll cooldown", 2.0f, new Utils.TimerDelegate(enableBarrelRoll), false);
                }
                else
                {
                    state = State.Invincible2;
                }
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
            yawRotation(gameTime);
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
                state = State.Normal2;
                //barrelRolling = false;
            }
            Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle);
        }
        //Rotate towards the reticle//
        private void rotateToReticle(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 vTargetDir = Reticule.vTargetPosition - Position;
            vTargetDir.Normalize();
            float dotProduct = Vector3.Dot(worldMatrix.Forward, vTargetDir);
            if (dotProduct < 1 && dotProduct > -1)
            {
                float rotationAngle = (float)Math.Acos(dotProduct / (worldMatrix.Forward.Length() * vTargetDir.Length()));
                Vector3 rotationAxis = Vector3.Cross(worldMatrix.Forward, vTargetDir);
                rotationAxis.Normalize();
                if (locktoTarget)
                    Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle * deltaTime * 8);
            }
        }

        //Rotate so that it will tend to stay upright//
        private void rotateUpright(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float dotProduct = Vector3.Dot(worldMatrix.Forward, FollowMe.FollowMeWorldMatrix.Up);
            if (dotProduct < 1 && dotProduct > -1)
            {
                float rotationAngle = (float)Math.Acos(dotProduct / (worldMatrix.Forward.Length() * FollowMe.FollowMeWorldMatrix.Up.Length()));
                Vector3 rotationAxis = Vector3.Cross(worldMatrix.Forward, FollowMe.FollowMeWorldMatrix.Up);
                rotationAxis.Normalize();
                if (locktoTarget)
                    Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle * deltaTime * 0.5f);
            }
        }

        //Rotate to fix yaw//
        private void yawRotation(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float dotProduct = Vector3.Dot(worldMatrix.Left, FollowMe.FollowMeWorldMatrix.Left);
            if(dotProduct<1 && dotProduct>-1)
            {
                 float rotationAngle = (float)Math.Acos(dotProduct / (worldMatrix.Left.Length() * FollowMe.FollowMeWorldMatrix.Left.Length()));
                Vector3 rotationAxis = Vector3.Cross(worldMatrix.Left, FollowMe.FollowMeWorldMatrix.Left);
                rotationAxis.Normalize();
                if (locktoTarget)
                    Rotation *= Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle * deltaTime * 0.5f);
            }

        }
        #endregion

        #region Shake

        private void ShakeCheck(GameTime gameTime)
        {
            if (shakeState != ShakeState.None)  //What to do to shake
            {  
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float shakeAngle = 0;

                //Update Angle//
                if (shakeTotalAngle > 0)
                {
                    shakeAngle = SHAKEANGLEACCELERATION * deltaTime;
                    shakeTotalAngle -= shakeAngle;
                }
                //else
                //    shakeAngle = SHAKEANGLEACCELERATION;

                switch (shakeState) //Different vibration
                {
                    case ShakeState.Enter: ShakeStateUpdate();
                                           break;

                    case ShakeState.One: Rotation *= Quaternion.CreateFromAxisAngle(worldMatrix.Forward, shakeAngle);
                                         break;

                    case ShakeState.Two: Rotation *= Quaternion.CreateFromAxisAngle(worldMatrix.Forward, -shakeAngle);
                                         break;

                    case ShakeState.Three: Rotation *= Quaternion.CreateFromAxisAngle(worldMatrix.Forward, shakeAngle);
                                           break;

                    case ShakeState.Exit: Rotation *= Quaternion.CreateFromAxisAngle(worldMatrix.Forward, -shakeAngle);
                                          break;

                    default: break;
                }

            }
                
        }

        private void ShakeStateUpdate()
        {
            if (shakeState != ShakeState.None)
            {
                if (shakeState == ShakeState.Enter)
                {
                    shakeState = ShakeState.One;
                    shakeTotalAngle = MathHelper.PiOver4;
                }
                else if (shakeState == ShakeState.One)
                {
                    shakeState = ShakeState.Two;
                    shakeTotalAngle = MathHelper.PiOver2;
                }
                else if (shakeState == ShakeState.Two)
                {
                    shakeState = ShakeState.Three;
                    shakeTotalAngle = MathHelper.PiOver2;
                }
                else if (shakeState == ShakeState.Three)
                {
                    shakeState = ShakeState.Exit;
                    shakeTotalAngle = MathHelper.PiOver4;
                }
                else if (shakeState == ShakeState.Exit)
                    shakeState = ShakeState.None;

                //shakeAngle = MathHelper.PiOver4;
                shipTimer.AddTimer("ShakeStateUpdateTimer", SHAKESWITCHTIME, ShakeStateUpdate, true);
            }
            else
                shipTimer.RemoveTimer("ShakeStateUpdateTimer");
        }

        #endregion
    }
}
