using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace WowAutomater
{
    public class CompoundSpell : Spell
    {
        public VirtualKeyCode SecondHotKey;

        public CompoundSpell(VirtualKeyCode firstHotKey,
                        VirtualKeyCode secondHotKey,
                        uint manaCost = 0,
                        double cooldownTime = 0,
                        double healthPercentage = 100,
                        uint level = 1,
                        bool useOnce = false) : base(firstHotKey, manaCost, cooldownTime, healthPercentage, level, useOnce)
        {
            SecondHotKey = secondHotKey;
        }

        public override void CastSpell()
        {
            Input.KeyPress(HotKey);
            Helper.WaitSeconds(Automater.RegisterDelay);
            Input.KeyPress(SecondHotKey);
            Helper.WaitSeconds(Automater.RegisterDelay);

            if (CooldownTime > 0)
            {
                CooledDown = false;
                CooldownTimer.Start();
            }

            if (UseOnce)
            {
                Used = true;
            }
        }
    }
}
