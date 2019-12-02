using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public class ComboPointSpell : Spell
    {
        public uint MinimumComboPointsCost;
        public uint MaximumComboPointsCost;

        public ComboPointSpell(VirtualKeyCode hotKey,
                                uint minimumComboPointsCost,
                                uint maximumComboPointsCost,
                                uint manaCost = 0,
                                double cooldownTime = 0,
                                double healthPercentage = 100,
                                uint level = 1,
                                bool useOnce = false) : base(hotKey, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {
            MinimumComboPointsCost = minimumComboPointsCost;
            MaximumComboPointsCost = maximumComboPointsCost;
        }

        public override bool CanCastSpell
        {
            get
            {
                if (Api.PlayerData.PlayerMana >= ManaCost &&
                   (Api.PlayerData.TargetComboPoints >= MinimumComboPointsCost && Api.PlayerData.TargetComboPoints <= MaximumComboPointsCost) &&
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
