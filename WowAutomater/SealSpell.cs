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
                     ushort comboPointsCost = 0,
                     double healthPercentage = 100,
                     ushort level = 1,
                     bool useOnce = false) : base( hotKey, manaCost, cooldownTime, comboPointsCost, healthPercentage, level, useOnce)
        {

        }

        public override bool CanCastSpell
        {
            get
            {
                if (WowApi.CurrentPlayerData.PlayerMana >= ManaCost &&
                    WowApi.CurrentPlayerData.TargetComboPoints >= ComboPointsCost &&
                    !WowApi.CurrentPlayerData.Casting &&
                    WowApi.CurrentPlayerData.CanUseSkill &&
                    WowApi.CurrentPlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    WowApi.CurrentPlayerData.PlayerLevel >= Level &&
                    (((int)WowApi.CurrentPlayerData.Buffs & (int)BuffType.SealOfCommand) > 0 ||
                      ((int)WowApi.CurrentPlayerData.Buffs & (int)BuffType.SealOfTheCrusader) > 0) &&
                    !Used &&
                    CooledDown)
                    return true;
                else
                    return false;
            }
        }


}
}
