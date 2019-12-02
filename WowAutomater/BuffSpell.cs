using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public class BuffSpell : Spell
    {
        public BuffType Buff = BuffType.None;

        public BuffSpell (VirtualKeyCode hotKey,
                          BuffType buff,
                          uint manaCost = 0,
                          double cooldownTime = 0,
                          double healthPercentage = 100,
                          uint level = 1,
                          bool useOnce = false) : base(hotKey, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {
            Buff = buff;
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
                    !Used &&
                    CooledDown &&
                    ((int)Api.PlayerData.Buffs & (int)Buff) == 0)
                    return true;
                else
                    return false;
            }
        }

    }
}
