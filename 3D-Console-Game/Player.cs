using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using _3D_Console_Game.Hudstuff;

namespace _3D_Console_Game
{
    internal class Player : IUpdatable
    {
        float width = 0.8f;
        float height = 2;
        float depth = 0.8f;
        public Prism Hitbox
        {
            get
            {
                Vector3 pos = playerPos - new Vector3(width / 2, 0, depth / 2);
                return new Prism(pos, width, height, depth);
            }
        }
        public Vector3 Forward { get { return Vector3.Transform(-Vector3.UnitZ, view); } }
        public Vector3 Left { get { return Vector3.Transform(-Vector3.UnitX, view); } }
        public Vector3 nearestLookCollision = Vector3.Zero;

        List<object> collidedObjects = new();
        public Inventory inventory { get; private set; } = new();
        public Quaternion view { get; private set; } = Quaternion.Identity;
        float yaw = 0f;
        float pitch = 0f;
        public Vector3 CamPos { get; private set; } = Vector3.Zero;
        Vector3 playerPos = new Vector3(0,0,0); 
        public Matrix4x4 ViewMatrix { 
            get {
                // View matrix = inverse of camera transform
                // First undo rotation (conjugate), then undo translation (negate)
                Matrix4x4 rot = Matrix4x4.CreateFromQuaternion(Quaternion.Conjugate(view));
                Matrix4x4 trans = Matrix4x4.CreateTranslation(-CamPos);
                Matrix4x4 vertInvert = Matrix4x4.Identity;
                vertInvert.M22 = -1;
                return trans * rot * vertInvert;
            } 
        }

        Box? heldBox;

        float moveSpeed;
        float rotSpeed;

        Vector3 playerVel;

        public float health { get; private set; }

        public Player(float health, float width = 0.5f, float height = 1.5f, float depth = 0.5f, Vector3 playerPos = default, float moveSpeed = 9f, float rotSpeed = 2f)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.playerPos = playerPos;
            this.moveSpeed = moveSpeed;
            this.rotSpeed = rotSpeed;
            this.health = health;
        }

        float timeSinceDamageTaken = 10;
        public void TakeDamage(float damage, Vector3 sourcePos)
        {
            if (timeSinceDamageTaken > 0.5f)
            {
                health -= damage;
                Vector3 knockback = sourcePos - playerPos;
                if (knockback.LengthSquared() > 0.0001f)
                {
                    playerVel -= Vector3.Normalize(knockback) * 4;
                }
                timeSinceDamageTaken = 0;
            }
        }

        public bool ObjectCollidedWithPlayer(object obj)
        {
            return collidedObjects.Contains(obj);
        }

        bool wasTouchingGround = true;
        double timeSinceWalkParticle = 0;
        public void Update(double deltaTime)
        {
            collidedObjects.Clear();
            float dt = (float)deltaTime;

            Vector3 xzForwardRaw = Forward * new Vector3(1, 0, 1);
            Vector3 xzForward = xzForwardRaw.LengthSquared() > 0.0001f ? Vector3.Normalize(xzForwardRaw) : Vector3.UnitZ;
            Vector3 xzLeftRaw = Left * new Vector3(1, 0, 1);
            Vector3 xzLeft = xzLeftRaw.LengthSquared() > 0.0001f ? Vector3.Normalize(xzLeftRaw) : Vector3.UnitX;


            bool isTouchingGround = playerPos.Y == 0;
            timeSinceDamageTaken += dt;
            

            if (InputManager.IsCharPressedAsync('W'))
            {
                playerVel += xzForward * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzForward)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('S'))
            {
                playerVel -= xzForward * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzForward)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('A'))
            {
                playerVel += xzLeft * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzLeft)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('D'))
            {
                playerVel -= xzLeft * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzLeft)) + 0.1f);
            }

            if (InputManager.IsCharPressedAsync('R'))
            {
                Vector3 camPoss = CamPos + Forward;
                ParticleManager.AddParticle(new Bullet(6f, 1, 4, camPoss, Forward, ConsoleColor.Green, false, true));
                //for (int i = 0; i < 3; i++)
                //{
                //    ParticleManager.AddParticle(new SplortParticle(1, CamPos + Forward, ConsoleColor.DarkGreen, 0.1f, 0.1f, 1.4f, 1.4f, 0f, Forward, 5, 0));
                //}
            }

            if (InputManager.IsKeyPressed(ConsoleKey.E))
            {
                if (heldBox == null) { 
                    ICollidable? c = Raycast.GetFirstObject(CamPos, Forward, 1);
                    if (c is Box b && b.isPickable)
                    {
                        heldBox = b;
                    }
                }
                else
                {
                    heldBox = null;
                }
            }

            playerVel -= new Vector3(Math.Sign(playerVel.X) * 2f * dt, dt * 8, Math.Sign(playerVel.Z) * 2f * dt);
            playerPos += Vector3.Multiply((float)deltaTime, playerVel);

            if (playerPos.Y < 0)
            {
                playerPos.Y = 0;
                playerVel.Y = 0;
            }

            if (heldBox != null)
            {
                heldBox.SetPos(CamPos + (heldBox.Pos - heldBox.MidPoint) + Forward, -MathF.Asin(Forward.Y), 0, MathF.Atan2(Forward.X, Forward.Z));
            }

            Prism hitbox = Hitbox;

            foreach (object obj in Game.activeObjects)
            {
                if (obj is ICollidable collidable && obj != heldBox)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = collidable.CollidesWith(Hitbox);
                    if (collides)
                    {
                        collidedObjects.Add(obj);

                        if (obj is PhysicsBox phys)
                        {
                            phys.CollideWithPhysics(-dirOut, penetration * 100, collisionPoint);
                        }

                        if (dirOut.Y > 0.7f)
                        {

                            isTouchingGround = true;
                        }

                        if (penetration > 0)
                        {
                            playerPos += dirOut * (penetration);
                        }

                        // Remove velocity component along the resolution direction
                        float velAlongNormal = Vector3.Dot(playerVel, dirOut);
                        if (velAlongNormal < 0)
                        {
                            playerVel -= velAlongNormal * dirOut;
                        }
                    }
                }
            }
            nearestLookCollision = Raycast.GetFirstObjectCollisionPos(CamPos, Forward, 3f) ?? nearestLookCollision;

            if (InputManager.IsCharPressedAsync(0x20) && isTouchingGround)
            {
                playerVel.Y = 5f;
                for (int i = 0; i < 15; i++)
                {
                    ParticleManager.AddParticle(new SplortParticle(1, playerPos + new Vector3(width / 2, 0, depth / 2), ConsoleColor.DarkBlue, 0.3f, 0.3f, 1, MathF.PI, 10, Vector3.UnitY, 6.5f, 15));
                }
            }

            if (isTouchingGround)
            {
                if (!wasTouchingGround)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        ParticleManager.AddParticle(new SplortParticle(1, playerPos + new Vector3(width / 2, 0, depth / 2), ConsoleColor.DarkBlue, 0.3f, 0.3f, 1, MathF.PI, 10, Vector3.UnitY, 6.5f, 15));
                    }
                }
                timeSinceWalkParticle += deltaTime;
                if (timeSinceWalkParticle > 0.2)
                {
                    for (int i = 0; i < playerVel.Length() - 0.6f; i++)
                    {
                        ParticleManager.AddParticle(new SplortParticle(1, playerPos + new Vector3(width / 2, 0, depth / 2), ConsoleColor.Yellow, 0.1f, 0.1f, 0.3f, 0.3f, 10, Vector3.Normalize(-playerVel + new Vector3(0, 2f, 0)), 4f, 15));
                    }
                    timeSinceWalkParticle = 0;
                }
            }
            wasTouchingGround = isTouchingGround;

            CamPos = playerPos + new Vector3(0, 1.3f, 0);

            Vector2 delta = InputManager.GetMouseDelta();
            
            yaw += rotSpeed * dt * delta.X / 20;
            pitch += rotSpeed * dt * delta.Y / 20;
            pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

            view = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch);


        }

        
    }
}
