using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Utils
{
    public class FrameRateCounter : DrawableGameComponent
    {
        private const int x = 1000;

        private SpriteBatch m_kSpriteBatch;
        private SpriteFont m_kFont;

        private Vector2 m_vPosition;

        private float m_fHighestFrameRate = 0;
		private float m_fCurrentFrameRate;
        private float m_fLowestFrameRate = x;

        private float averageFrameRate;

        private Queue<float> frameRates = new Queue<float>();
        
        public FrameRateCounter(Game game, Vector2 vPosition)
            : base(game)
        {
            m_vPosition = vPosition;
            DrawOrder = 1000;    //Ensures that the counter will be at the top of the screen
        }

        protected override void LoadContent()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));

            m_kSpriteBatch = new SpriteBatch(graphicsService.GraphicsDevice);
            m_kFont = Game.Content.Load<SpriteFont>("fpsfont");

            base.LoadContent();
        }
        
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        
        public override void Update(GameTime gameTime)
        {
			m_fCurrentFrameRate = 1 / (float) gameTime.ElapsedGameTime.TotalSeconds;

            if(m_fCurrentFrameRate < 10000)
                if (m_fCurrentFrameRate > m_fHighestFrameRate)      //Update Highest//
                    m_fHighestFrameRate = m_fCurrentFrameRate;

            if (m_fLowestFrameRate > m_fCurrentFrameRate)           //Update lowest//
                m_fLowestFrameRate = m_fCurrentFrameRate;

            //Calculate Ave. frame rates//
            float totalFrameRate = 0;
            if(frameRates.Count < 1000)
            {
                frameRates.Enqueue(m_fCurrentFrameRate);
                foreach (float i in frameRates)
                    totalFrameRate += i;
                averageFrameRate = totalFrameRate / frameRates.Count;
            }
            else
            {
                frameRates.Dequeue();
                frameRates.Enqueue(m_fCurrentFrameRate);
                foreach (float i in frameRates)
                    totalFrameRate += i;
                averageFrameRate = totalFrameRate / frameRates.Count;
            }

			base.Update(gameTime);
        }

        //Clear FrameRateCounter//
        public void ResetFPSCount()
        {
            if(frameRates.Count > 0)
                 frameRates.Clear();
        }
        
        public override void Draw(GameTime gameTime)
        {
            m_kSpriteBatch.Begin();
            
			// Color this based on the framerate
            Color DrawColor = Color.Green;
			if (m_fCurrentFrameRate < 15.0f)
                DrawColor = Color.Red;
			else if (m_fCurrentFrameRate < 30.0f)
                DrawColor = Color.Yellow;

            //Declare new Vector2//
            Vector2 correction = new Vector2(0, 40);

            m_kSpriteBatch.DrawString(m_kFont, "Highest FPS: " + m_fHighestFrameRate.ToString("f2"), m_vPosition - correction, DrawColor);
			m_kSpriteBatch.DrawString(m_kFont, "FPS: " + averageFrameRate.ToString("f3"), m_vPosition, DrawColor);
            m_kSpriteBatch.DrawString(m_kFont, "Lowest FPS: " + m_fLowestFrameRate.ToString("f3"), m_vPosition + correction, DrawColor);
            m_kSpriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
