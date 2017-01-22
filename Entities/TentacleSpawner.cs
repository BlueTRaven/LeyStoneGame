using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LeyStoneEngine;
using LeyStoneEngine.Utility;
using LeyStoneEngine.Interface;
using LeyStoneEngine.Entities;
using LeyStoneEngine.Graphics;

namespace LeyStoneGame.Entities
{
    public class TentacleSpawner : Entity
    {
        List<Tentacle> tentacles = new List<Tentacle>();

        public int deathTimer;

        public TentacleSpawner() : base(Vector2.Zero, new Vector2(8), EntityType.TentacleSpawner, 0)
        {
            for (int i = 0; i < 16; i++)
            {
                tentacles.Add(new Tentacle(Main.rand.NextCoinFlip()));
            }
        }

        public override void Update(BaseWorld world)
        {
            base.Update(world);

            foreach (Tentacle t in tentacles)
                t.Update(world);

            deathTimer++;
            if (deathTimer > 1280)
            {
                if (deathTimer == 1281)
                    Main.camera.SetFade(Color.White, false, 120);

                if (deathTimer > 1400)
                    Die(world);
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            foreach (Tentacle t in tentacles)
                t.Draw(batch);
        }
    }
}
