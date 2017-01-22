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

        public Player(Vector2 position) : base(position, new Vector2(16), 100, EntityType.Player, 0)
        {
            canJumpTimer = Timer.CreateTimer(5, World.timeScale);
            moveTimer = Timer.CreateTimer(0, World.timeScale);
            dashTimer = Timer.CreateTimer(120, World.timeScale, true);
        }

        public override void Update(BaseWorld world)
        {
            if (trail == null)
            {
                trail = new VisualTrail(position, 1, 120, .1f, Color.White);
                world.entities.Add(trail);
            }
            velocity.Y += world.gravity * World.timeScale.scale;

            if (velocity.Y > 16)
                velocity.Y = 16;

            Move();

            base.Update(world);
            
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

            trail.position = position;
            trail.velocity = velocity;
        }

        private void Control()
        {

            //if (!canMove)
            //moveTimer--;

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
        {
            List<Vector2> resolvedVectors = hitpoly.CheckResolveCollision(world);
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
                        jumpDirection = 0;
                    }
                    else if ((angle >= 0 && angle < 45) || (angle > 315 && angle <= 360) || (angle > 135 || angle < 225))
                    {
                        velocity.X = 0;
                        resetXVel = true;
                    }

                    if (angle == 0)
                    {
                        canJumpTimer.Reset();
                        jumpDirection = -1;
                    }
                    else if (angle == 180)
                    {
                        canJumpTimer.Reset();
                        jumpDirection = 1;
                    }
                });
            }
        }

        public void ClearVisualTrail()
        {
            trail.ClearNodes();
        }

        public override void Draw(SpriteBatch batch)
        {
            DrawPrimitives.DrawRectangle(batch, new Rectangle(new Point((int)position.X, (int)position.Y) - new Point(8), new Point(16)), Color.Red);
            DrawPrimitives.DrawHollowRectangle(batch, new Rectangle(new Point((int)position.X, (int)position.Y) - new Point(8), new Point(16)), 1, Color.Black);

            DrawPrimitives.DrawHollowRectangle(batch, new Rectangle(Camera.ToWorldCoords(Main.mouse.position).ToPoint(), new Point(4)), 4, Color.Red);
        }
    }
}
