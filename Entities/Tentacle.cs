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
    public class Tentacle : Entity
    {
        public PrimitivePolygon[] polys;
        public Vector2[] positions, origins;
        public readonly Vector2[] constOrigins;

        public float height;

        private readonly bool reverse;

        public Tentacle(bool reverse) : base(Vector2.Zero, new Vector2(8), EntityType.Tentacle, 0)
        {
            polys = new PrimitivePolygon[2];
            positions = new Vector2[3];
            origins = new Vector2[3];
            constOrigins = new Vector2[3];

            float x = Main.rand.Next(0, 800);

            positions[1] = new Vector2(x, 632);
            positions[1] = new Vector2(x, 474);
            positions[2] = new Vector2(x, 316);

            origins[0] = new Vector2(x, 632);
            origins[1] = new Vector2(x, 474);
            origins[2] = new Vector2(x, 316);

            constOrigins[0] = new Vector2(x, 632);
            constOrigins[1] = new Vector2(x, 474);
            constOrigins[2] = new Vector2(x, 316);

            height = 328;
            if (reverse)
            {
                height = -632;
                Array.Reverse(positions);
                Array.Reverse(origins);
                Array.Reverse(constOrigins);
            }

            polys[0] = new PrimitivePolygon(
                new Vector2[] { positions[0] + new Vector2(64, 0), positions[0] - new Vector2(64, 0), positions[1] - new Vector2(32, 0), positions[1] + new Vector2(32, 0) },
                new Color[] { Color.White, Color.White, Color.White, Color.White });

            polys[1] = new PrimitivePolygon(
                new Vector2[] { positions[1] - new Vector2(32, 0), positions[1] + new Vector2(32, 0), positions[2] },
                new Color[] { Color.White, Color.White, Color.White });

            solid = false;

            this.reverse = reverse;
        }

        public override void Update(BaseWorld world)
        {
            base.Update(world);

            for (int i = 0; i < 3; i++)
            {
                while (true)
                {
                    origins[i].X += Main.rand.Next(0, 2) * (Main.rand.NextCoinFlip() ? 1 : -1); //random value between [0, 2), 1/2 chance of being [0, -2)

                    if (origins[i].X > constOrigins[i].X + 16)
                        origins[i].X = constOrigins[i].X + 16;
                    else if (origins[i].X < constOrigins[i].X - 16)
                        origins[i].X = constOrigins[i].X - 16;
                    else
                        break;
                }

                if (!reverse)
                {
                    if (height > 0)
                        origins[i].Y = constOrigins[i].Y + height;
                }
                else
                {
                    if (height < -328)
                        origins[i].Y = constOrigins[i].Y + height;
                }

                positions[i] = Camera.ToWorldCoords(origins[i]);
            }

            polys[0] = new PrimitivePolygon(
                new Vector2[] { positions[0] + new Vector2(48, 0), positions[0] - new Vector2(48, 0), positions[1] - new Vector2(24, 0), positions[1] + new Vector2(24, 0) },
                new Color[] { Color.White, Color.White, Color.White, Color.White });

            polys[1] = new PrimitivePolygon(
                new Vector2[] { positions[1] - new Vector2(24, 0), positions[1] + new Vector2(24, 0), positions[2] },
                new Color[] { Color.White, Color.White, Color.White });

            if (reverse)
                height += .25f;
            else
                height -= .25f;
        }

        public override void Draw(SpriteBatch batch)
        {
            if (alive > 1)
            {
                foreach (PrimitivePolygon poly in polys)
                {
                    poly.Draw(batch, new EffectInstance[] { new EffectInstance(BaseMain.assets.GetEffectContainer("basicEffect")) }.ToList());
                }
            }
        }
    }
}
