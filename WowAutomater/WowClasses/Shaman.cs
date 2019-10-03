using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicWowNeuralParasite
{
    public class ShamanAutomater : WowClassAutomater
    {

        public override bool IsMelee => false;
        public override void AutoAttackTarget()
        {
            throw new NotImplementedException();
        }

        public override void FindTarget()
        {
            throw new NotImplementedException();
        }

        public override void KillTarget()
        {
            throw new NotImplementedException();
        }

        public override void RegenerateVitals()
        {
            throw new NotImplementedException();
        }
    }
}
