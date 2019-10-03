using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicWowNeuralParasite
{
    public abstract class WowClassAutomater
    {
        public abstract bool IsMelee
        {
            get;
        }
        public abstract void RegenerateVitals();

        public abstract void FindTarget();

        public abstract void KillTarget();

        public abstract void AutoAttackTarget();
    }
}
