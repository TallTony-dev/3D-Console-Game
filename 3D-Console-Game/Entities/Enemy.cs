using _3D_Console_Game.Engine;
using _3D_Console_Game.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Entities
{
    internal abstract class Enemy : Entity
    {
       
        public Enemy(PhysicsBox body, float health) : base(health, body)
        {

        }

        public override void TakeDamage(float damage, Vector3 sourcePos)
        {
            if (timeSinceTakenDamage > 0.1)
            {
                SoundPlayer.PlaySound("smallhurt1.wav", 0.4f);
            }
            base.TakeDamage(damage, sourcePos);
        }

    }
}
