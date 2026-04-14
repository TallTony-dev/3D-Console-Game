using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Items
{
    internal class GrapplingItem : BoxHeldItem
    {
        static readonly char[,] texture =
        {
            { 'G', '▓', '▓', },
            { 'R', '▓', '▓', },
            { 'A', '▓', '▓', },
            { 'P', '▓', '▓', }
        };

        public GrapplingItem(Player player, float duration, float velocity = 40f) : base(player, ConsoleColor.Black, Vector3.Zero, texture)
        {
            this.duration = duration;
            this.velocity = velocity;
        }
        private float velocity;
        private float duration;
        private Vector3 grapplePoint = Vector3.Zero;
        private object? grappledObject;
        private bool isGrappled = false;
        private bool isGrappleShot = false;
        private float timeSinceGrappleShot = 10;
        private Box grappleBox = new Box(Vector3.Zero, 0, 0, 0, ConsoleColor.White);
        private Bullet grappleBullet = new(0, 0, 0, Vector3.Zero, Vector3.Zero, ConsoleColor.Blue, false, false);


        private void UpdateGrappleBox()
        {
            if (grappledObject != null && grappledObject is ICollidable c && grappledObject is IPushable)
            {
                grapplePoint = c.MidPoint;
            }

            Vector3 start = box.MidBack;
            grappleBox.SetPos(box.MidBack, Vector3.Normalize(grapplePoint - start));
            grappleBox.SetSize(0.2f, 0.2f, (grapplePoint - start).Length());
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (isSelected)
            {
                Vector3 adjGun = Vector3.Transform(new Vector3(0, 0.4f / (timeSinceGrappleShot * 4 + 0.3f), 0), owner.view);
                dirOffset = adjGun * 3;
                if (!isGrappled)
                {
                    timeSinceGrappleShot += dt;

                    if (!isGrappleShot)
                    {
                        if (InputManager.IsLeftMousePressed())
                        {
                            timeSinceGrappleShot = 0;
                            isGrappleShot = true;
                            Vector3 shotPos = box.MidBack * 0.3f + owner.CamPos * 0.7f;
                            Vector3 recoil = Vector3.Transform(new Vector3(-0.2f, 0, 0f), owner.view) + adjGun;
                            Vector3 shotDir = Vector3.Normalize(owner.Forward + 0.3f * recoil);
                            grappleBullet = new Bullet(duration, 0, velocity, shotPos, shotDir, ConsoleColor.Cyan, false, false, 0.3f, 0.3f, 0.1f);
                        }
                    }
                    if (isGrappleShot)
                    {
                        float prevTime = grappleBullet.timeLeft;
                        grappleBullet.Update(dt);
                        if (grappleBullet.timeLeft <= prevTime - dt - 0.0001f)
                        {
                            isGrappled = true;
                            isGrappleShot = false;
                            grapplePoint = grappleBullet.box.MidPoint;
                            grappledObject = grappleBullet.LastCollidedObj;
                        }
                        if (grappleBullet.timeLeft < 0 || !InputManager.IsLeftMouseDown())
                        {
                            isGrappleShot = false;
                        }
                    }
                }
                if (isGrappled)
                {
                    owner.ApplyForce(Vector3.Normalize(grapplePoint - owner.MidPoint), (10 + (grapplePoint - owner.MidPoint).Length()) * dt, grapplePoint);
                    UpdateGrappleBox();
                    if (grappledObject != null && grappledObject is IPushable p)
                    {
                        p.ApplyForce(Vector3.Normalize(owner.MidPoint - grapplePoint), (10 + (grapplePoint - owner.MidPoint).Length()) * dt, grapplePoint);
                    }
                    if (!InputManager.IsLeftMouseDown())
                    {
                        isGrappled = false;
                    }
                }
            }
            
        }

        public override void Draw(Display display)
        {
            base.Draw(display);
            if (isSelected)
            {
                if (isGrappled)
                {
                    grappleBox?.Draw(display);
                }
                else if (isGrappleShot)
                {
                    grappleBullet.Draw(display);
                }
            }
        }

    }
}
