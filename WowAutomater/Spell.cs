using System.Timers;
using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public class Spell
    {
        public Timer CooldownTimer = new Timer();
        public VirtualKeyCode HotKey;
        public uint ManaCost;
        public double CooldownTime;
        public double PlayerHealthPercentage;
        public uint Level;
        public volatile bool CooledDown = true;
        public bool UseOnce = false;
        public bool Used = false;

        public virtual bool CanCastSpell
        {
            get
            {
                if (Api.PlayerData.PlayerMana >= ManaCost &&
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

        public Spell(VirtualKeyCode hotKey, 
                     uint manaCost = 0, 
                     double cooldownTime = 0, 
                     double healthPercentage = 100,
                     uint level = 1,
                     bool useOnce = false)
        {
            HotKey = hotKey;
            ManaCost = manaCost;
            CooldownTime = cooldownTime;
            PlayerHealthPercentage = healthPercentage;
            Level = level;
            UseOnce = useOnce;

            if(UseOnce)
            {
                Api.UpdateEvent += Api_UpdateEvent;
            }

            if(CooldownTime > 0)
            {
                CooldownTimer.Interval = CooldownTime * 1000;
                CooldownTimer.Elapsed += CooldownTimer_Elapsed;
            }

        }

        public virtual void CastSpell()
        {
            Input.KeyPress(HotKey);
            Helper.WaitSeconds(Automater.RegisterDelay);

            if (CooldownTime > 0)
            {
                CooledDown = false;
                CooldownTimer.Start();
            }

            if(UseOnce)
            {
                Used = true;
            }
        }

        private void CooldownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CooledDown = true;
            CooldownTimer.Stop();
        }

        private void Api_UpdateEvent(object sender, System.EventArgs ea)
        {
            if (!Api.PlayerData.PlayerInCombat || !Api.PlayerData.PlayerHasTarget)
                Used = false;
        }
    }
}
