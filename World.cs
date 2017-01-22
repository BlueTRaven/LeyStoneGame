using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LeyStoneEngine;
using LeyStoneEngine.Utility;
using LeyStoneEngine.Interface;
using LeyStoneEngine.Collision;
using LeyStoneEngine.Entities;
using LeyStoneEngine.Graphics;
using LeyStoneEngine.Serialization;
using LeyStoneEngine.Triggers;

using LeyStoneGame.Entities;

namespace LeyStoneGame
{
    public enum EntityType
    {
        Player,
        Visual,
        Coin,
        Tentacle,
        TentacleSpawner
    }

    public class World : BaseWorld
    {   //Name is porto.
        bool cameraFollowsPlayer = true;
        private Vector2 cameraUnlockedPos;

        public Player player;

        public Color colorStart, colorEnd;

        PrimitivePolygon backPlate;
        public World(Vector2 size) : base(size)
        {
            lines.Add(new Line(new Node(new Vector2(64, 256)), new Node(new Vector2(256, 256)), true));
            lines.Add(new Line(new Node(new Vector2(256, 256)), new Node(new Vector2(512, 320)), true));
            lines.Add(new Line(new Node(new Vector2(512, 320)), new Node(new Vector2(768, 320)), true));

            lines.Add(new Line(new Node(new Vector2(256, 128)), new Node(new Vector2(64, 128)), true));
            lines.Add(new Line(new Node(new Vector2(64, 128)), new Node(new Vector2(64, 256)), true));
            player = new Player(new Vector2(65, 16));
            entities.Add(player);

            /*triggers.Add(new TriggerContinuous(new Vector2(512, 192), new Vector2(256, 128), 0, new Action<BaseWorld, Trigger>((world, trigger) =>
            {
                World w = (World)world;
                Player player = ((World)world).player;
                float x = 0;

                x = (player.position.X - trigger.bounds.Location.X);
                w.colorStart = Color.Lerp(Color.LightGreen, new Color(84, 0, 0), ((float)x / (float)trigger.bounds.Size.X));

                ChangeBackplateColor();
            })));*/

            Line.DEBUGDrawLines = true;
        }

        public override void Update(BaseMain main)
        {
            base.Update(main);

            if (counter == 1)
            {
                BackgroundPreset1();
            }

            if (Main.keyboard.KeyPressed(Keys.R))
            {
                cameraFollowsPlayer = !cameraFollowsPlayer;
                cameraUnlockedPos = player.position;
                /*polygons.Add(new PrimitivePolygon(
                    new Vector2[] { new Vector2(128, 0), new Vector2(256, 0), new Vector2(128, 128) }, 
                    new Color[] { Color.White, Color.White, Color.White }));*/
            }

            if (cameraFollowsPlayer)
            {
                Main.camera.target = player.position + new Vector2(0, -64);
            }
            else
            {
                player.moveTimer.Reset(2);

                Main.camera.target = cameraUnlockedPos;

                if (Main.keyboard.KeyPressedContinuous(Keys.W))
                    cameraUnlockedPos.Y -= 8;
                if (Main.keyboard.KeyPressedContinuous(Keys.A))
                    cameraUnlockedPos.X -= 8;
                if (Main.keyboard.KeyPressedContinuous(Keys.S))
                    cameraUnlockedPos.Y += 8;
                if (Main.keyboard.KeyPressedContinuous(Keys.D))
                    cameraUnlockedPos.X += 8;
            }

            if (Main.keyboard.KeyPressed(Keys.F))
            {

                for (int i = 0; i < 30; i++)
                {
                    PrimitivePolygon poly = new PrimitivePolygon(
                    new Vector2[] { new Vector2(-8), new Vector2(8, -8), new Vector2(8), new Vector2(-8, 8) },
                    new Color[] { Color.Pink, Color.Pink, Color.Pink, Color.Pink });

                    entities.Add(new VisualPolygonParticle(new Vector2(320, 128), poly, Main.rand.Next(2, 5))
                            .SetSpin(new Vector2(), (float)Main.rand.NextDouble(0, 360), 2.5f + (float)Main.rand.NextDouble(-.7f, 1f))
                            .SetFade(60, 90 + Main.rand.Next(60), Color.Transparent)
                            .SetSolid(new PolyRectangle(new Vector2(320, 128), new Vector2(320 + 16, 128 + 16)))
                            .SetGravityAffected(.1f));
                            //.SetWander((float)Main.rand.NextDouble(.9f, 1.5f), 0, -1, 30, new Vector2(0, 120)));
                }
                entities.Add(new Coin(new Vector2(512 + 64, 128 + 64)));
                //entities.Add(new VisualRaySpawner(player.position, 360, new Vector2(5, 15), new Vector2(1, 3), new Vector2(128), new Vector2(2), new Vector2(10, 60), Color.White));
                //entities.Add(new TentacleSpawner());
            }

            if (Main.keyboard.KeyPressed(Keys.G))
                this.Deserialize("map1.lsmf");
        }

        public override void Deserialize(string input)
        {
            player = null;
            base.Deserialize(input);

            foreach(Entity entity in entities)
            {
                if (entity.entityType == (int)EntityType.Player)
                {
                    player = (Player)entity;
                    break;
                }
            }


            if (player == null)
                entities.Add(player = new Player(Vector2.Zero));
        }

        public override Entity GetEntityFromType(int type, int subType, Vector2 position, params int[] additionalInformation)
        {
            if (position == null) position = Vector2.Zero;

            switch (type)
            {
                case 0: return new Player(position);
                case 1:
                    {
                        switch (subType)
                        {
                            case 0: return null;
                            case 1: return null;//return new VisualPolygonParticle(position, ); //TODO PolygonParticle presets
                            case 2: return null;//new VisualRay(position, additionalInformation[0], additionalInformation[1], additionalInformation[2], additionalInformation[3], )
                            case 3: return null;//new VisualRaySpawner()
                            default: return null;
                        }
                    }
                    case 2: return new Coin(position);
                            case 3: return new Tentacle(Convert.ToBoolean(additionalInformation[0]));
                            case 4: return new TentacleSpawner();
                default: return new Player(position);
            }
        }

        public override void Draw(GraphicsDevice device, SpriteBatch batch)
        {
            foreach (Background b in backgrounds)
                if (b.counter == 0)
                    b.DrawRenderTarget(batch);

            //backPlate.Draw(batch, new EffectInstance[] { new EffectInstance(BaseMain.assets.GetEffectContainer("basicEffect")) }.ToList());

            foreach (Background b in backgrounds)
                b.Draw(this, batch);

            foreach (Trigger t in triggers)
                t.DrawDebug(batch);

            ((BasicEffect)(Main.assets.GetEffectContainer("basicEffect")).effect).World = BaseMain.camera.GetViewMatrix();
            foreach (PrimitivePolygon p in polygons)
                p.Draw(batch, new EffectInstance[] { new EffectInstance(BaseMain.assets.GetEffectContainer("basicEffect")) }.ToList());

            foreach (Line l in lines)
                l.DrawDebug(batch);

            foreach (Entity e in entities)
                e.Draw(batch);

            //DrawPrimitives.DrawRectangle(batch, bounds, Color.Blue);
            //DrawPrimitives.DrawLine(batch, Vector2.Zero, new Vector2(64, 128), Color.Red);
        }

        public void ChangeBackplateColor()
        {
            Background b = (BackgroundDynamic)backgrounds[0];
            b.polygons.Clear();
            b.polygons.Add(new PrimitivePolygon(new Vector2[] {
                    new Vector2(0, Main.HEIGHT), Vector2.Zero, new Vector2(Main.WIDTH, 0), new Vector2(Main.WIDTH, Main.HEIGHT) },
                new Color[] { colorStart, colorEnd, colorEnd, colorStart }));

            //((BackgroundDynamic)backgrounds[0]).SetPolygons();

            backgrounds[0].counter = 0;
        }

        public void BackgroundPreset1()
        {
            colorStart = Color.LightGreen;//new Color(84, 0, 0);
            colorEnd = Color.Black;
            List<PrimitivePolygon> poly = new List<PrimitivePolygon>();
            poly.Add(new PrimitivePolygon(new Vector2[] {
                new Vector2(0, Main.HEIGHT), Vector2.Zero, new Vector2(Main.WIDTH, 0), new Vector2(Main.WIDTH, Main.HEIGHT) },
                    new Color[] { colorStart, colorEnd, colorEnd, colorStart }));
            //new Color[] { new Color(84, 0, 0), Color.Black, Color.Black, new Color(84, 0, 0) }));
            backgrounds.Add(new BackgroundDynamic(null, Vector2.Zero, poly, new Action<SpriteBatch, Background>((batch, background) => {
            })));
            backPlate = poly[0];

            poly = new List<PrimitivePolygon>();
            for (int i = 0; i < 32; i++)
            {
                float randX = Main.rand.Next(0, Main.WIDTH);
                float randY = Main.rand.Next(0, Main.HEIGHT);

                float randW = (float)Main.rand.NextDouble(1.5f, 4);
                //float randH = (float)Main.rand.NextDouble(6, 18);

                float randH = (float)Main.rand.NextDouble(4, 12);

                poly.Add(new PrimitivePolygon(new Vector2(randX, randY), randH, 4, Color.White));
            }
            
            backgrounds.Add(new Background(null, new Vector2(-.05f, 0), poly, new Action<SpriteBatch, Background>((batch, background) => { })));

            poly = new List<PrimitivePolygon>();
            poly.Add(new PrimitivePolygon(Vector2.Zero - new Vector2(128, 128 + 16), 512, 25, Color.Orange).SetRandomColors(Color.OrangeRed, Color.DarkOrange));
            backgrounds.Add(new Background(null, Vector2.Zero, poly, new Action<SpriteBatch, Background>((batch, background) =>
            {
                batch.Draw(Main.assets.GetTexture("porto"), Main.camera.center - new Vector2(16, 16), null, Color.White, 0, Vector2.Zero, .5f, SpriteEffects.None, 0);
            })));

            /*poly = new List<PrimitivePolygon>();
            for (int i = 0; i < Main.rand.Next(3, 8); i++)
                poly.Add(new PrimitivePolygon(new Vector2((float)Main.rand.Next(800 * 4), 1400), 1500, 4, Color.Gray));
            backgrounds.Add(new Background(null, new Vector2(-.5f, 0), poly, new Action<SpriteBatch, Background>((batch, background) => { })).SetSize(new Vector2(800 * 4, 600)));*/

            poly = new List<PrimitivePolygon>();
            poly.Add(new PrimitivePolygon(new Vector2(400, 1400), 1000, 256, Color.IndianRed));
            backgrounds.Add(new Background(null, new Vector2(-.5f, 0), poly, new Action<SpriteBatch, Background>((batch, background) => { })));

            poly = new List<PrimitivePolygon>();
            poly.Add(new PrimitivePolygon(new Vector2(400, 1300), 800, 256, Color.OrangeRed));
            backgrounds.Add(new Background(null, new Vector2(-1, 0), poly, new Action<SpriteBatch, Background>((batch, background) => { })));
        }
    }
}
