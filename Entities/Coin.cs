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
    public class Coin : Entity
    {
        private PrimitivePolygon polygon;

        private Vector2 sideMin, sideMax;

        bool collected = false;

        public Coin(Vector2 position) : base(position, new Vector2(32, 32), EntityType.Coin, 0)
        {
            polygon = new PrimitivePolygon(position, 24, 64, Color.Blue);
        }

        Vector2 axis;
        public override void Update(BaseWorld world)
        {
            base.Update(world);

            axis = VectorHelper.GetPerpendicular(position - Camera.ToWorldCoords(Main.camera.center));

            if ((((World)world).player.position - position).Length() < 24 && !collected)
            {
                Vector2 axis = VectorHelper.GetPerpendicular(position - Camera.ToWorldCoords(Main.camera.center));

                sideMax = position + axis * 24;
                sideMin = position - axis * 24;

                collected = true;

                for (int i = 0; i < Main.rand.Next(25, 40); i++)
                {
                    float size = (float)Main.rand.NextDouble(4, 10);
                    bool left = Main.rand.NextCoinFlip();

                    /*PrimitivePolygon poly = new PrimitivePolygon(
                    new Vector2[] { new Vector2(-size), new Vector2(size, -size), new Vector2(size), new Vector2(-size, size) },
                    new Color[] { Color.White, Color.White, Color.White, Color.White }, new TextureContainer(Main.assets.GetTexture("glowMask"), .25f));*/

                    PrimitivePolygon poly = new PrimitivePolygon(
                        new Vector2[] { new Vector2(0), new Vector2(size * 2, 0), new Vector2(size * 2), new Vector2(0, size * 2) },
                        Color.White);

                    world.entities.Add(new VisualPolygonParticle(position - new Vector2(32), poly, (float)Main.rand.NextDouble(.8f, 2.3f))
                            .SetSpin(new Vector2(size / 2), (float)Main.rand.NextDouble(0, 360), 2.5f + (float)Main.rand.NextDouble(-.7f, 1f) * (left ? -1 : 1))
                            .SetFade(60, 90 + Main.rand.Next(60), Color.Transparent)
                            .SetDelay(Main.rand.Next(0, 15)));
                }

                foreach (Entity e in world.entities)
                {
                    if (e.entityType == (int)EntityType.TentacleSpawner)
                    {
                        if (((TentacleSpawner)e).deathTimer <= 1280)
                        {
                            e.Die(world);

                            Main.camera.SetFade(Color.White, true, 30);
                        }
                        else collected = false;
                    }
                }
            }

            if (collected)
            {
                World w = ((World)world);
                w.player.moveTimer.Reset(15);
                w.player.ClearVisualTrail();
                w.player.TakeDamage(world, this, 30);
                polygon = new PrimitivePolygon(new Vector2[] { Camera.ToWorldCoords(Main.camera.center), sideMax, sideMin }, new Color[] { Color.White, Color.White, Color.White });

                sideMax = Vector2.Lerp(sideMax, Camera.ToWorldCoords(Main.camera.center), .175f);
                sideMin = Vector2.Lerp(sideMin, Camera.ToWorldCoords(Main.camera.center), .175f);

                if ((sideMax - Camera.ToWorldCoords(Main.camera.center)).Length() < 2 || (sideMin - Camera.ToWorldCoords(Main.camera.center)).Length() < 2)
                    Die(world);
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (!collected)
                batch.Draw(Main.assets.GetTexture("glowMask"), position - new Vector2(64), null, Color.White, 0, Vector2.Zero, .5f, SpriteEffects.None, 0);
            //batch.Draw(Main.assets.GetTexture("glowMask"), new Rectangle((position - new Vector2(64)).ToPoint(), new Point(128)), Color.White);
            polygon.Draw(batch, new EffectInstance[] { new EffectInstance(BaseMain.assets.GetEffectContainer("basicEffect")) }.ToList());

            //DrawPrimitives.DrawCircle(batch, position, 8, Color.Blue, 4, 64);
        }
    }
}
