using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public class SealSpell : Spell
    {
        public SealSpell(VirtualKeyCode hotKey,
                     uint manaCost = 0,
                     double cooldownTime = 0,
                     double healthPercentage = 100,
                     uint level = 1,
                     bool useOnce = false) : base( hotKey, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {

        }

        public override bool CanCastSpell
        {
            get
            {
                if (Api.PlayerData.PlayerMana >= ManaCost &&
                    !Api.PlayerData.Casting &&
                    Api.PlayerData.CanUseSkill &&
                    Api.PlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    Api.PlayerData.PlayerLevel >= Level &&
                    (((int)Api.PlayerData.Buffs & (int)BuffType.SealOfCommand) > 0 ||
                      ((int)Api.PlayerData.Buffs & (int)BuffType.SealOfTheCrusader) > 0) &&
                    !Used &&
                    CooledDown)
                    return true;
                else
                    return false;
            }
        }


}
}
