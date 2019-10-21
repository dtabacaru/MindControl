using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class FinishingSpell : ComboPointSpell
    {
        public double TargetHealthPercentage = 25;

        public FinishingSpell(  VirtualKeyCode hotKey,
                                double targetHealthPercentage,
                                uint minimumComboPointsCost,
                                uint maximumComboPointsCost,
                                uint manaCost = 0,
                                double cooldownTime = 0,
                                double healthPercentage = 100,
                                uint level = 1,
                                bool useOnce = false) : base(hotKey, minimumComboPointsCost, maximumComboPointsCost, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {
            TargetHealthPercentage = targetHealthPercentage;
        }

        public override bool CanCastSpell
        {
            get
            {
                if (WowApi.PlayerData.PlayerMana >= ManaCost &&
                   ((WowApi.PlayerData.TargetComboPoints >= MinimumComboPointsCost && WowApi.PlayerData.TargetComboPoints <= MaximumComboPointsCost) ||
                   (WowApi.PlayerData.TargetHealth < TargetHealthPercentage && WowApi.PlayerData.TargetComboPoints >= 1)) &&
                   !WowApi.PlayerData.Casting &&
                    WowApi.PlayerData.CanUseSkill &&
                    WowApi.PlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    WowApi.PlayerData.PlayerLevel >= Level &&
                    !Used &&
                    CooledDown)
                    return true;
                else
                    return false;
            }
        }

    }
}
