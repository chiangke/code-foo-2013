using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

namespace Utils
{
    public class HUD : DrawableGameComponent
    {
        private const int x = 1000;

        private SpriteBatch m_kSpriteBatch;
        private SpriteFont m_kFont;
        private SpriteBatch mBatch;
        private Texture2D mHealthBar;
        private Texture2D mBomb;
        private Texture2D mShip;
        private int mCurrentHealth;
        private int numLives;
        private int numBombs;
        private int bossHealth;
        private String lstate;
        bool bossSpawn = true;

        private Vector2 m_vPosition;

        private float m_fHighestFrameRate = 0;
		private float m_fCurrentFrameRate;
        private float m_fLowestFrameRate = x;

        private float averageFrameRate;

        private Queue<float> frameRates = new Queue<float>();
        
        public HUD(Game game, Vector2 vPosition, Ship s)
            : base(game)
        {
            mCurrentHealth = Ship.health;
            numLives = Ship.lives;
            numBombs = Ship.bombs;
            lstate = Ship.lazorState.ToString();
            m_vPosition = vPosition;
            DrawOrder = 1000;    //Ensures that the counter will be at the top of the screen
        }

        protected override void LoadContent()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));

            m_kSpriteBatch = new SpriteBatch(graphicsService.GraphicsDevice);
            m_kFont = Game.Content.Load<SpriteFont>("fpsfont");
            mHealthBar = Game.Content.Load<Texture2D>("Pictures/healthbar") as Texture2D;
            mBomb = Game.Content.Load<Texture2D>("Pictures/bombicon") as Texture2D;
            mShip = Game.Content.Load<Texture2D>("Pictures/shipicon") as Texture2D;

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
            mCurrentHealth = Ship.health;
            numBombs = Ship.bombs;
            numLives = Ship.lives;
            lstate = Ship.lazorState.ToString();
            mCurrentHealth = (int)MathHelper.Clamp(mCurrentHealth, 0, 100);
            if (bossSpawn)
                bossHealth = Boss.Bosshealth;
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
            /*
			// Color this based on the framerate
            Color DrawColor = Color.Green;
			if (m_fCurrentFrameRate < 15.0f)
                DrawColor = Color.Red;
			else if (m_fCurrentFrameRate < 30.0f)
                DrawColor = Color.Yellow;

            //Declare new Vector2//
            Vector2 correction = new Vector2(0, 40);

            
			m_kSpriteBatch.DrawString(m_kFont, "FPS: " + averageFrameRate.ToString("f3"), m_vPosition, DrawColor);
            m_kSpriteBatch.DrawString(m_kFont, "Lowest FPS: " + m_fLowestFrameRate.ToString("f3"), m_vPosition + correction, DrawColor);
             * */
            m_kSpriteBatch.DrawString(m_kFont, "Health: " + mCurrentHealth.ToString("f2"), m_vPosition, Color.Red);
            m_kSpriteBatch.DrawString(m_kFont, lstate, m_vPosition + new Vector2(0, 50), Color.Red);
            m_kSpriteBatch.Draw(mHealthBar, new Rectangle(30, 30, mHealthBar.Width, 50), new Rectangle(0, 50, mHealthBar.Width, 50), Color.Transparent);
            m_kSpriteBatch.Draw(mHealthBar, new Rectangle(30, 30, (int)(mHealthBar.Width * ((double)mCurrentHealth / 100)), 50), new Rectangle(0, 50, mHealthBar.Width, 50), Color.Red);
            m_kSpriteBatch.Draw(mHealthBar, new Rectangle(30, 30, mHealthBar.Width, 50), new Rectangle(0, 0, mHealthBar.Width, 50), Color.White);

            if (bossSpawn)
            {
                m_kSpriteBatch.Draw(mHealthBar, new Rectangle(0, 0, 1024, 10), new Rectangle(0, 50, mHealthBar.Width, 50), Color.Transparent);
                m_kSpriteBatch.Draw(mHealthBar, new Rectangle(0, 0, (int)(1024 * ((double)bossHealth / 1000)), 10), new Rectangle(0, 50, mHealthBar.Width, 50), Color.Red);
            }

            for (int i = 0; i < numLives; i++)
            {
                m_kSpriteBatch.Draw(mShip, new Rectangle((900+25*i), 20, mShip.Width, mShip.Height), new Rectangle(0, 0, mShip.Width, mShip.Height), Color.White);
            }

            for (int i = 0; i < numBombs; i++)
            {
                m_kSpriteBatch.Draw(mBomb, new Rectangle((900 + 25 * i), 50, mBomb.Width, mBomb.Height), new Rectangle(0, 0, mBomb.Width, mBomb.Height), Color.White);
            }

                m_kSpriteBatch.End();
            
            base.Draw(gameTime);
        }

        public void addBoss(Boss b)
        {
            bossHealth = Boss.Bosshealth;
            bossSpawn = true;
        }
    }
}
