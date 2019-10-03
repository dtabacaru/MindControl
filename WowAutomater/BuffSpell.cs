using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class BuffSpell : Spell
    {
        public BuffType Buff = BuffType.None;

        public BuffSpell (VirtualKeyCode hotKey,
                          BuffType buff,
                          ushort manaCost = 0,
                          double cooldownTime = 0,
                          ushort comboPointsCost = 0,
                          double healthPercentage = 100,
                          ushort level = 1,
                          bool useOnce = false) : base(hotKey, manaCost, cooldownTime, comboPointsCost, healthPercentage, level, useOnce)
        {
            Buff = buff;
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
                    !Used &&
                    CooledDown &&
                    ((int)WowApi.CurrentPlayerData.Buffs & (int)Buff) == 0)
                    return true;
                else
                    return false;
            }
        }

    }
}
