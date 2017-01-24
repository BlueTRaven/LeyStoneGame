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
using LeyStoneEngine.Collision;
using LeyStoneEngine.Graphics;

namespace LeyStoneGame.Entities
{
    public class Player : EntityLiving
    {
        private bool canJump = true;
        private Timer canJumpTimer;
        private int jumpDirection = 0;

        public bool canMove = true;
        public Timer moveTimer;

        private VisualTrail trail;

        private bool canDash;
        private Timer dashTimer;

        private float speed = 2.5f;

        //private Polygon2 test;

        public Player(Vector2 position) : base(position, new Vector2(16), 100, EntityType.Player, 0)
        {
            canJumpTimer = Timer.CreateTimer(5, World.timeScale);
            moveTimer = Timer.CreateTimer(0, World.timeScale);
            dashTimer = Timer.CreateTimer(120, World.timeScale, true);
            solid = true;
        }

        public override void Update(BaseWorld world)
        {
            base.Update(world);

            if (trail == null)
            {
                trail = new VisualTrail(position, 1, 120, .1f, Color.White);
                world.entities.Add(trail);
            }
            velocity.Y += world.gravity * World.timeScale.scale;

            if (velocity.Y > 16)
                velocity.Y = 16;
            
            if (resetYVel)
            {
                canJumpTimer.Reset();
                //canJumpTimer = 5;
            }

            if (!canJumpTimer.done)
            {
                canJump = true;
            }
            else canJump = false;

            Control();

            Move();

            trail.position = position;
            trail.velocity = velocity;
        }

        private void Control()
        {
            if (moveTimer.done)
                canMove = true;
            else canMove = false;

            if (canMove)
            {
                if (Main.keyboard.KeyPressedContinuous(Keys.A))
                    velocity.X += -speed;

                if (Main.keyboard.KeyPressedContinuous(Keys.D))
                    velocity.X += speed;

                if (Main.keyboard.KeyPressed(Keys.Space) && canJump)
                {
                    velocity.Y = -12;

                    velocity.X += jumpDirection * 12;

                    if (jumpDirection != 0)
                        moveTimer.Reset(5);

                    canJump = false;
                    canJumpTimer.Reset();
                }
            }

            if (moveTimer.done || moveTimer.paused)
                velocity.X = MathHelper.Clamp(velocity.X, -6, 6);

            velocity.X *= .88f * World.timeScale.scale;
        }

        public override void PostResolve(BaseWorld world)
        {   //PostResolve is overwritten for the walljump code.
            List<Vector2> resolvedVectors = hitpoly.CheckResolveCollisionWorldLines(world);
            if (resolvedVectors.Count > 0)
            {
                collided = true;

                resolvedVectors.ForEach(vector =>
                {
                    float angle = vector.GetVectorAngle();
                    if ((angle >= 45 && angle <= 135) || (angle >= 225 && angle <= 315))
                    {
                        velocity.Y = 0;
                        resetYVel = true;
                        jumpDirection = 0;  //overwritten addition:
                    }                       //if the player jumps after being resolved from a nearly flat surface, they will jump straight up.
                    else if ((angle >= 0 && angle < 45) || (angle > 315 && angle <= 360) || (angle > 135 || angle < 225))
                    {
                        velocity.X = 0;
                        resetXVel = true;
                    }

                    if (angle == 0)
                    {
                        canJumpTimer.Reset();
                        jumpDirection = -1; //overwritten addition:
                    }                       //if the player jumps on a wall facing either directly left or right, i.e. 0 or 180 degrees, they will walljump, launching back in the appropriate direction.
                    else if (angle == 180)
                    {
                        canJumpTimer.Reset();
                        jumpDirection = 1;
                    }
                });
            }
        }

        public override void Die(BaseWorld world)
        {
            base.Die(world);

            for (int i = 0; i < Main.rand.Next(15, 30); i++)
            {
                float size = (float)Main.rand.NextDouble(6, 12);
                bool left = Main.rand.NextCoinFlip();

                PrimitivePolygon poly = new PrimitivePolygon(
                            new Vector2[] { new Vector2(0), new Vector2(size * 2, 0), new Vector2(size * 2), new Vector2(0, size * 2) },
                            Color.White);
                world.entities.Add(new VisualPolygonParticle(position, poly, (float)Main.rand.NextDouble(.8f, 2.3f))
                                .SetSpin(new Vector2(size / 2), (float)Main.rand.NextDouble(0, 360), 2.5f + (float)Main.rand.NextDouble(-.7f, 1f) * (left ? -1 : 1))
                                .SetFade(60, 90 + Main.rand.Next(60), Color.Transparent)
                                .SetDelay(Main.rand.Next(0, 20)));
            }

            Main.camera.SetFade(Color.White, false, 240);
        }

        public void ClearVisualTrail()
        {
            trail.ClearNodes();
        }

        public override void Draw(SpriteBatch batch)
        {
            //hitpoly.Draw(batch);
            DrawPrimitives.DrawRectangle(batch, new Rectangle(new Point((int)position.X, (int)position.Y) - new Point(8), new Point(16)), Color.Red);
            DrawPrimitives.DrawHollowRectangle(batch, new Rectangle(new Point((int)position.X, (int)position.Y) - new Point(8), new Point(16)), 1, Color.Black);
            //DrawPrimitives.DrawCircle(batch, position, 8, Color.Blue, 4, 32);
            DrawPrimitives.DrawHollowRectangle(batch, new Rectangle(Camera.ToWorldCoords(Main.mouse.position).ToPoint(), new Point(4)), 4, Color.Red);

            batch.DrawString(Main.assets.GetFont("bitfontMunro12"), String.Format("<{0,4}> <{1,4}> \n{2,4} / {3,4}", position.X, position.Y, health, maxHealth), Main.camera.Position, Color.White);
        }
    }
}
