using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class PaladinAutomater : WowClassAutomater
    {
        public Action Attack;
        public Action Target;

        public BuffSpell SealOfTheCrusader;
        public BuffSpell SealOfCommand;
        public BuffSpell BlessingOfWisdom;

        public SealSpell Judgement;

        public PaladinAutomater()
        {
            Attack = new Action(VirtualKeyCode.VK_1);
            Target = new Action(VirtualKeyCode.TAB);

            SealOfTheCrusader = new BuffSpell(VirtualKeyCode.VK_2, BuffType.SealOfTheCrusader, 34, level: 6, useOnce: true);
            SealOfCommand = new BuffSpell(VirtualKeyCode.VK_4, BuffType.SealOfCommand, 55, level: 20);
            Judgement = new SealSpell(VirtualKeyCode.VK_3, 37, 8);
            BlessingOfWisdom = new BuffSpell(VirtualKeyCode.VK_9, BuffType.BlessingOfWisdom, 45);
        }

        public override bool IsMelee => true;

        public override void AutoAttackTarget()
        {
            if (BlessingOfWisdom.CanCastSpell)
                BlessingOfWisdom.CastSpell();
            else if (!WowApi.CurrentPlayerData.PlayerInCombat)
                return;
            else if (!WowApi.CurrentPlayerData.PlayerHasTarget)
                Target.Act();
            else if (!WowApi.CurrentPlayerData.PlayerIsAttacking)
                Attack.Act();
            else if (SealOfTheCrusader.CanCastSpell)
                SealOfTheCrusader.CastSpell();
            else if (Judgement.CanCastSpell)
                Judgement.CastSpell();
            else if (SealOfCommand.CanCastSpell)
                SealOfCommand.CastSpell();
        }

        public override void FindTarget()
        {
            
        }

        public override void KillTarget()
        {

        }

        public override void RegenerateVitals()
        {
            throw new NotImplementedException();
        }
    }
}
