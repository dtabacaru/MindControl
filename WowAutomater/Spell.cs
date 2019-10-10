using System.Timers;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class Spell
    {
        public Timer CooldownTimer = new Timer();
        public VirtualKeyCode HotKey;
        public ushort ManaCost;
        public double CooldownTime;
        public double PlayerHealthPercentage;
        public ushort Level;
        public volatile bool CooledDown = true;
        public bool UseOnce = false;
        public bool Used = false;

        public virtual bool CanCastSpell
        {
            get
            {
                if (WowApi.PlayerData.PlayerMana >= ManaCost &&
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

        public Spell(VirtualKeyCode hotKey, 
                     ushort manaCost = 0, 
                     double cooldownTime = 0, 
                     double healthPercentage = 100,
                     ushort level = 1,
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
                WowApi.UpdateEvent += WowApi_UpdateEvent;
            }

            if(CooldownTime > 0)
            {
                CooldownTimer.Interval = CooldownTime * 1000;
                CooldownTimer.Elapsed += CooldownTimer_Elapsed;
            }

        }

        public void CastSpell()
        {
            Input.KeyPress(HotKey);
            Helper.WaitSeconds(WowAutomater.RegisterDelay);

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

        private void WowApi_UpdateEvent(object sender, System.EventArgs ea)
        {
            if (!WowApi.PlayerData.PlayerInCombat || !WowApi.PlayerData.PlayerHasTarget)
                Used = false;
        }
    }
}
