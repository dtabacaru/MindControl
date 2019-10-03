using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class FinishingSpell : Spell
    {
        public double TargetHealthPercentage = 25;

        public FinishingSpell(  VirtualKeyCode hotKey,
                                ushort comboPointsCost,
                                double targetHealthPercentage,
                                ushort manaCost = 0,
                                double cooldownTime = 0,
                                double healthPercentage = 100,
                                ushort level = 1,
                                bool useOnce = false) : base(hotKey, manaCost, cooldownTime, comboPointsCost, healthPercentage, level, useOnce)
        {
            TargetHealthPercentage = targetHealthPercentage;
        }

        public override bool CanCastSpell
        {
            get
            {
                if (WowApi.CurrentPlayerData.PlayerMana >= ManaCost &&
                   (WowApi.CurrentPlayerData.TargetComboPoints >= ComboPointsCost ||
                   (WowApi.CurrentPlayerData.TargetHealth < TargetHealthPercentage && WowApi.CurrentPlayerData.TargetComboPoints >= 1)) &&
                   !WowApi.CurrentPlayerData.Casting &&
                    WowApi.CurrentPlayerData.CanUseSkill &&
                    WowApi.CurrentPlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    WowApi.CurrentPlayerData.PlayerLevel >= Level &&
                    !Used &&
                    CooledDown)
                    return true;
                else
                    return false;
            }
        }
    }
}
