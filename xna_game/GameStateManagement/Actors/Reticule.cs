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
    public class Reticule : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private bool isTargeted;  //Reticule over target?
        private float fscale;
        Texture2D myTexture;
        public Vector2 vPosition = Vector2.Zero;
        public static Vector3 vTargetPosition;    //Target position.
        private SpriteBatch spriteBatch;
        public Viewport ScreenViewport;

        Ray raycast;

        public Reticule(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            raycast = new Ray(Vector3.Zero, Vector3.Zero);
            isTargeted = false;
            vTargetPosition = Vector3.Zero;
            ScreenViewport = Game.GraphicsDevice.Viewport;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
            DrawOrder = 510;
            fscale = 0.4f;
        }


        protected override void LoadContent()
        {
            base.LoadContent();
            myTexture = Game.Content.Load<Texture2D>("Pictures/reticule");
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            vPosition.X = InputState.CurrentMouseStates.X;
            vPosition.Y = InputState.CurrentMouseStates.Y;
            UpdateRay();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            Vector2 vDrawPosition = vPosition;
            //Set to the edge of the screen//
            /*
            if (vDrawPosition.X < -512)
                vDrawPosition.X = -512;
            else if (vDrawPosition.X > 512)
                vDrawPosition.X = 512;

            if (vDrawPosition.Y < -384)
                vDrawPosition.Y = -384;
            else if (vDrawPosition.Y > 384)
                vDrawPosition.Y = 384;
            Ship 
            *******************************/
            if (isTargeted)
            {
                spriteBatch.Draw(myTexture, vDrawPosition, null, Color.Red, 0, new Vector2(myTexture.Height / 2, myTexture.Width / 2), fscale, SpriteEffects.None, 0f);
            }
            else
                spriteBatch.Draw(myTexture, vDrawPosition, null, Color.White, 0, new Vector2(myTexture.Height / 2, myTexture.Width / 2), fscale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public void UpdateRay()
        {
            Vector3 nearPlane, farPlane;
           

            //Calculate unprojection for the far and near plane//
            #region Unproject
            nearPlane = ScreenViewport.Unproject(new Vector3(vPosition.X, vPosition.Y, 0), GameplayScreen.ProjectionMatrix, GameplayScreen.Camera1.LookAtMatrix, Matrix.Identity);
            farPlane = ScreenViewport.Unproject(new Vector3(vPosition.X, vPosition.Y, 1), GameplayScreen.ProjectionMatrix, GameplayScreen.Camera1.LookAtMatrix, Matrix.Identity);
         
            raycast.Direction = farPlane - nearPlane;
            raycast.Direction.Normalize();
            raycast.Position = GameplayScreen.Camera1.cameraPosition;

            if (!isTargeted)
                vTargetPosition = Vector3.Lerp(nearPlane, farPlane, 0.1f); //Temporarily set the target to the middle plane if nothing is targeted.

            #endregion

            isTargeted = false;

            foreach (Asteroids A in SpawnManager.asteroidList)  //Iterate through the Asteroids list and check whether ray is true.
            {
                if(raycast.Intersects(A.WorldBounds) != null)
                {
                    isTargeted = true;
                    vTargetPosition = A.Position;
                    break;
                }  
            }
        }
    }
}
