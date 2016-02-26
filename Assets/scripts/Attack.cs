using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public abstract class Attack
    {
        public abstract bool effectPlayer(Player target);
        
        public abstract bool effectEnemy(Enemy target);
    }
}
