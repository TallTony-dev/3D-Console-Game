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

    }
}
