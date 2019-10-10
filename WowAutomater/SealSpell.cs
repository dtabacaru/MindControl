using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class SealSpell : Spell
    {
        public SealSpell(VirtualKeyCode hotKey,
                     ushort manaCost = 0,
                     double cooldownTime = 0,
                     double healthPercentage = 100,
                     ushort level = 1,
                     bool useOnce = false) : base( hotKey, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {

        }

        public override bool CanCastSpell
        {
            get
            {
                if (WowApi.PlayerData.PlayerMana >= ManaCost &&
                    !WowApi.PlayerData.Casting &&
                    WowApi.PlayerData.CanUseSkill &&
                    WowApi.PlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    WowApi.PlayerData.PlayerLevel >= Level &&
                    (((int)WowApi.PlayerData.Buffs & (int)BuffType.SealOfCommand) > 0 ||
                      ((int)WowApi.PlayerData.Buffs & (int)BuffType.SealOfTheCrusader) > 0) &&
                    !Used &&
                    CooledDown)
                    return true;
                else
                    return false;
            }
        }


}
}
