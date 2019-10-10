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
                          double healthPercentage = 100,
                          ushort level = 1,
                          bool useOnce = false) : base(hotKey, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {
            Buff = buff;
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
                    !Used &&
                    CooledDown &&
                    ((int)WowApi.PlayerData.Buffs & (int)Buff) == 0)
                    return true;
                else
                    return false;
            }
        }

    }
}
