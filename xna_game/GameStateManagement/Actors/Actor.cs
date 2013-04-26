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
    public class Actor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected Model ActorModel;
        public Matrix worldMatrix;
        public string sMeshName;
        protected Utils.Timer timer;
        protected Matrix[] actorBones;

        //Transformation properties//
        public float uniformScale;
        public Vector3 Position;
        public Quaternion Rotation;
        public float RotationAngle;
        private Matrix AllTransform;

        //Movement//
        public Vector3 Velocity;
        public Vector3 RotationAxis;

        //Basic Properties//
        public float Mass;
        public float TerminalVelocity;
        public Vector3 Force;
        public Vector3 Acceleration;
        public Vector3 Drag;
        public bool bPhysicsDriven;

        //Collision Properties//
        public BoundingSphere ModelBounds;
        public BoundingSphere WorldBounds;

        //Josh's new and improved collisions!!
        public int AllBonesCount;               //This is just a count of all the bones contained in the model.
        public List<BoundingSphere> BoneSpheres;//You'll want to use this list of bounding spheres to check collision if that wasn't already aparent.





        private Random rand = new Random();
        public Actor(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            worldMatrix = Matrix.Identity;
            sMeshName = "Actor";
            timer = new Utils.Timer();

            uniformScale = 1.0f;
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;

            Velocity = Vector3.Zero;
            RotationAxis = Vector3.Zero;

            //Initialize basic Physics Properties//
            Mass = 1.0f;
            TerminalVelocity = 1.0f;
            Force = Vector3.Zero;
            Acceleration = Vector3.Zero;
            Drag = Vector3.One;
            Drag.Z = 0;
            bPhysicsDriven = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            worldMatrix = Matrix.CreateScale(uniformScale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            WorldBounds.Center = Position;
            WorldBounds.Radius = ModelBounds.Radius * uniformScale;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        protected override void LoadContent()
        {
            sMeshName = "Models/" + sMeshName;
            ActorModel = Game.Content.Load<Model>(sMeshName);
            actorBones = new Matrix[ActorModel.Bones.Count];


            //Josh's new and improved collisions!!
            AllBonesCount = ActorModel.Bones.Count;

            BoneSpheres = new List<BoundingSphere>();
            for (int i = 0; i < ActorModel.Meshes.Count; i++)
            {
                if (ActorModel.Meshes.ElementAt(i).Name == "Bounding_Sphere")
                {
                    BoneSpheres.Add(new BoundingSphere(Position, ActorModel.Meshes.ElementAt(i).BoundingSphere.Radius));
                }
            }
 


            //Setup ModelBounds//
            foreach (ModelMesh mesh in ActorModel.Meshes)
            {
                ModelBounds = BoundingSphere.CreateMerged(ModelBounds, mesh.BoundingSphere);
            }
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
		{
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ActorModel.CopyAbsoluteBoneTransformsTo(actorBones);
            foreach(ModelMesh mesh in ActorModel.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.World = actorBones[mesh.ParentBone.Index]*worldMatrix;
                    effect.View = GameplayScreen.Camera1.LookAtMatrix;
                    effect.Projection = GameplayScreen.ProjectionMatrix;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.1f);
                    effect.SpecularColor = GameplayScreen.ColorLerp;//new Vector3(0.5f, 0.5f, 0.1f);
                    //effect.SpecularPower = 0.1f;
                    effect.DirectionalLight0.Direction = GameplayScreen.DirectionalLightPos;//new Vector3(0.0f, 0.0f, -0.1f);
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                }
                mesh.Draw();
            } 
			base.Draw(gameTime);
        }

        public Vector3 GetWorldFacing()
        {
            return worldMatrix.Forward;
        }

        public Vector3 GetWorldPosition()
        {
            return worldMatrix.Translation;
        }
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
            timer.Update(gameTime);

            float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region Physics Driven Calculation
            if (bPhysicsDriven)
            {
                Velocity += Acceleration * fDeltaTime / 2.0f;
                Position += Velocity * fDeltaTime * 100;
                Acceleration = Force / Mass;
                Velocity += Acceleration * fDeltaTime / 2.0f;

                //Diminishing Velocity due to "friction"
                //Velocity -= Drag * fDeltaTime;

                //Setting Max Velocity//
                if (Velocity.LengthSquared() > (TerminalVelocity * TerminalVelocity))
                {
                    Velocity.Normalize();
                    Velocity = Vector3.Multiply(Velocity, TerminalVelocity);
                }

                //Setting Min Velocity//
                if (Velocity.LengthSquared() < 0.0f)
                    Velocity = Vector3.Zero;          
            }
            #endregion
            #region Non-Physics Speed Calculation
            else
            {
                //Velocity Update:

                Position += Vector3.Multiply(Velocity, (float)gameTime.ElapsedGameTime.TotalSeconds) * 100;
            }
            #endregion
            //WorldBounds Update//
            WorldBounds.Center = Position;
            WorldBounds.Radius = ModelBounds.Radius * uniformScale;


            //Josh's new and improved collisions!!!
            int BoneCounter = 0;
            for (int i = 0; i < AllBonesCount; i++ )
            {
                if (ActorModel.Bones.ElementAt(i).Name == "Bounding_Sphere")  //I named the Bones while making the model in Autodesk Maya.
                {
                    Vector3 bonePosition = Position + ActorModel.Bones.ElementAt(i).Transform.Translation;
                    BoundingSphere temp = BoneSpheres[BoneCounter];
                    temp.Center = bonePosition;
                    BoneSpheres[BoneCounter] = temp;
 
                    BoneCounter++;
                }
            }


            
            //Rotation//
            Rotation *= Quaternion.CreateFromAxisAngle(RotationAxis, (float)(RotationAngle * gameTime.ElapsedGameTime.TotalSeconds));
        
            //Final Transformation Matrix//
            AllTransform = Matrix.CreateScale(uniformScale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            worldMatrix = Matrix.Identity * AllTransform;

            //WrapAround();
        }

        /************Obsolete WrapAround()************************
        protected void WrapAround()
        {
            if (Position.X < -600)
                Position.X = 600;
            else if (Position.X > 600)
                Position.X = -600;

            if (Position.Y < -450)
                Position.Y = 450;
            else if (Position.Y > 450)
                Position.Y = -450;
        }
        ************************************************************/
        protected virtual void CheckCollision() { return; }

        //Function to delete the enemy object from the game//
        protected void selfDestroy()
        {
            UnloadContent();
            Game.Components.Remove(this);
        }
    }
}
