using WindowsInput.Native;
using WowApi;

namespace WowAutomater
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
                if (Api.PlayerData.PlayerMana >= ManaCost &&
                   ((Api.PlayerData.TargetComboPoints >= MinimumComboPointsCost && Api.PlayerData.TargetComboPoints <= MaximumComboPointsCost) ||
                   (Api.PlayerData.TargetHealthPercentage <= TargetHealthPercentage && Api.PlayerData.TargetComboPoints >= 1)) &&
                   !Api.PlayerData.Casting &&
                    Api.PlayerData.CanUseSkill &&
                    Api.PlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    Api.PlayerData.PlayerLevel >= Level &&
                    !Used &&
                    CooledDown)
                    return true;
                else
                    return false;
            }
        }

    }
}
