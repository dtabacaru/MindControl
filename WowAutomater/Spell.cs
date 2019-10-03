using System.Timers;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class Spell
    {
        public Timer CooldownTimer = new Timer();
        public VirtualKeyCode HotKey;
        public ushort ManaCost;
        public ushort ComboPointsCost;
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
                if (WowApi.CurrentPlayerData.PlayerMana >= ManaCost &&
                    WowApi.CurrentPlayerData.TargetComboPoints >= ComboPointsCost &&
                    !WowApi.CurrentPlayerData.Casting &&
                    WowApi.CurrentPlayerData.CanUseSkill &&
                    WowApi.CurrentPlayerData.PlayerHealthPercentage <= PlayerHealthPercentage &&
                    WowApi.CurrentPlayerData.PlayerLevel >= Level &&
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
                     ushort comboPointsCost = 0, 
                     double healthPercentage = 100,
                     ushort level = 1,
                     bool useOnce = false)
        {
            HotKey = hotKey;
            ManaCost = manaCost;
            ComboPointsCost = comboPointsCost;
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
            if (!WowApi.CurrentPlayerData.PlayerInCombat || !WowApi.CurrentPlayerData.PlayerHasTarget)
                Used = false;
        }
    }
}
