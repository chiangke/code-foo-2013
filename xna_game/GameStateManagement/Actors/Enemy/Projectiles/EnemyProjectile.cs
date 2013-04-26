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
    public class EnemyProjectile : Enemy
    {
        const float DEFAULTSPEED = 0.5f;

        public float baseSpeed;     //Force multiplier basically
        public enum Type { NA, Normal, Homing, HorizontalSweep };
        public Type type;
        public Vector3 vTarget;
        public EnemyProjectile(Game game)
            : base(game)
        {
            baseSpeed = 0.5f;
            type = Type.NA;
            vTarget = Vector3.Zero;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void CheckCollision()
        {
            base.CheckCollision();
        }
    }
}
