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
    public class Projectile : Entity, IDamageDealer
    {
        private int _damage;
        public int damage { get { return _damage; } set { } }

        TextureContainer container;

        private readonly float rotationOffset;

        private bool bounces;

        public bool diesOnCollision = true;

        public readonly bool destroyedOnDeal = true;  //if the entity is destroyed upon dealing its damage.
        public readonly int damageEntityType;   //The type of entity to damage. -1 = all enemies. -2 = all enemies AND player.
                                                //Defaults to 0, so it will damage the player by default.

        public Projectile(Vector2 position, Vector2 size, TextureContainer container, float initialRotation, float rotationOffset, float speed, int damage) : base(position, size, EntityType.Projectile, 0)
        {
            this.container = container;

            container.rotation = initialRotation + rotationOffset;
            hitpoly.RotateTo(initialRotation);
            this.rotationOffset = rotationOffset;

            velocity = VectorHelper.GetVectorAtAngle(initialRotation) * speed;

            this._damage = damage;
        }

        public Projectile SetBounces()
        {
            this.bounces = true;

            return this;
        }

        public override void Update(BaseWorld world)
        {
            base.Update(world);
            RotateTo(VectorHelper.GetVectorAngle(velocity));
            Move();

            foreach(EntityLiving e in world.entities.OfType<EntityLiving>().ToList())
            {
                bool hit = false;
                if (damageEntityType == -1 || damageEntityType == -2)
                {
                    if (hitpoly.Intersects(e.hitpoly))
                    {
                        if (damageEntityType == -1 && e.entityType == 0)    //if it's a friendly projectile (-1, doesn't damage the player) and the current entity is the player
                            continue;                                       //then continue; we don't want to hurt the player.

                        e.TakeDamage(world, this, damage);                  //otherwise, the entity takes damage.
                        hit = true;
                    }
                }
                else if (e.entityType == damageEntityType)
                {
                    e.TakeDamage(world, this, damage);
                    hit = true;
                }

                if (hit && destroyedOnDeal)
                {
                    Die(world);
                    break;      //we break since it's gonna be dead.
                }
            }
        }

        public override void PostResolve(BaseWorld world)
        {
            List<Vector2> resolvedVectors = hitpoly.CheckResolveCollisionWorldLines(world);
            if (resolvedVectors.Count > 0)
            {
                if (diesOnCollision)
                    Die(world);

                collided = true;

                resolvedVectors.ForEach(vector =>
                {
                    float angle = vector.GetVectorAngle();
                    if ((angle >= 45 && angle <= 135) || (angle >= 225 && angle <= 315))
                    {
                        velocity.Y = 0;
                        resetYVel = true;
                    }
                    else if ((angle >= 0 && angle < 45) || (angle > 315 && angle <= 360) || (angle > 135 || angle < 225))
                    {
                        velocity.X = 0;
                        resetXVel = true;
                    }
                });
            }
        }

        private void RotateTo(float value)
        {
            container.rotation = value + rotationOffset;
            hitpoly.RotateTo(value);
        }

        private void Rotate(float value)
        {
            container.rotation += value;
            hitpoly.Rotate(value);
        }

        public override void Draw(SpriteBatch batch)
        {
            hitpoly.Draw(batch);

            //batch.Draw(container.texture, position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            batch.Draw(container.texture, position, null, container.color, MathHelper.ToRadians(container.rotation), container.texture.Bounds.Size.ToVector2() / 2, container.scale, SpriteEffects.None, 0);
            //DrawHelper.Draw(batch, position, Vector2.Zero, container);   
        }
    }
}
