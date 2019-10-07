using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class RogueAutomater : WowClassAutomater
    {
        public int StealthLevel = 10;
        public bool StealthFlag = true;
        public bool ThrowFlag = false;
        public int StealthCooldown = 10;

        public Timer StaleStealthTimer = new Timer();

        public Action Attack;
        public Action Target;
        public Action Throw;

        public BuffSpell Stealth;

        public Spell SinisterStrike;
        public ComboPointSpell SliceAndDice;
        public ComboPointSpell Rupture;
        public ComboPointSpell KidneyShot;
        public Spell Evasion;

        public FinishingSpell Eviscerate;

        public override bool IsMelee => true;
        public RogueAutomater()
        {
            Attack = new Action(VirtualKeyCode.VK_1);
            Target = new Action(VirtualKeyCode.TAB);
            Throw = new Action(VirtualKeyCode.VK_4);

            Stealth = new BuffSpell(VirtualKeyCode.VK_T, BuffType.Stealth, cooldownTime: 10);
            SinisterStrike = new Spell(VirtualKeyCode.VK_2,45);
            Eviscerate = new FinishingSpell(VirtualKeyCode.VK_3, 5, 25, 35);
            SliceAndDice = new ComboPointSpell(VirtualKeyCode.VK_5, 1, 1, 25, level: 10, useOnce: true);
            Rupture = new ComboPointSpell(VirtualKeyCode.VK_6,3,5, 25, 6 + 3 * 2, level: 20);
            KidneyShot = new ComboPointSpell(VirtualKeyCode.VK_7,3,5, 25, 20, level: 30);
            Evasion = new Spell(VirtualKeyCode.VK_L, cooldownTime: 5 * 60, healthPercentage: 40, level: 8);

            StaleStealthTimer.Interval = StealthCooldown * 1000;
            StaleStealthTimer.Elapsed += StaleStealthTimer_Elapsed;

            WowApi.UpdateEvent += WowApi_UpdateEvent;
        }

        private void WowApi_UpdateEvent(object sender, EventArgs ea)
        {
            if (WowApi.CurrentPlayerData.PlayerActionError == ActionErrorType.FacingWrongWay)
                Target.Act();
        }

        private void StaleStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((WowApi.CurrentPlayerData.Buffs & (int)BuffType.Stealth) > 0)
            {
                Input.KeyPress(VirtualKeyCode.VK_T);
            }

            StaleStealthTimer.Stop();
            Stealth.CooldownTimer.Stop();
            Stealth.CooldownTimer.Start();
        }

        public enum RogueFindTargetMode
        {
            Normal,
            Throw,
            Stealth,
            StealthAndThrow
        }

        public RogueFindTargetMode FindTargetMode = RogueFindTargetMode.Normal;

        private void CheckFindMode()
        {
            if (WowApi.CurrentPlayerData.PlayerLevel < 5)
            {
                FindTargetMode = RogueFindTargetMode.Throw;
            }
            else if ( (StealthFlag && WowApi.CurrentPlayerData.PlayerLevel > StealthLevel) && 
                      (ThrowFlag && WowApi.CurrentPlayerData.AmmoCount > 1) )
            {
                FindTargetMode = RogueFindTargetMode.StealthAndThrow;
            }
            else if (StealthFlag && WowApi.CurrentPlayerData.PlayerLevel > StealthLevel)
            {
                FindTargetMode = RogueFindTargetMode.Stealth;
            }
            else if (ThrowFlag && WowApi.CurrentPlayerData.AmmoCount > 1)
            {
                FindTargetMode = RogueFindTargetMode.Throw;
            }
            else
            {
                FindTargetMode = RogueFindTargetMode.Normal;
            }
        }

        public override void FindTarget()
        {
            WaypointFollower.FollowWaypoints(true);

            CheckFindMode();
            bool validTarget = false;

            if (FindTargetMode == RogueFindTargetMode.StealthAndThrow)
            {
                if (Stealth.CanCastSpell)
                    Stealth.CastSpell();

                validTarget = WowApi.CurrentPlayerData.PlayerHasTarget &&
                            WowApi.CurrentPlayerData.TargetHealth == 100 &&
                            !WowApi.CurrentPlayerData.TargetInCombat &&
                            WowApi.CurrentPlayerData.TargetFaction == 0 &&
                            WowApi.CurrentPlayerData.IsInFarRange &&
                            !WowApi.CurrentPlayerData.IsInCloseRange;

                if (validTarget)
                {
                    WaypointFollower.StopFollowingWaypoints();

                    Helper.WaitSeconds(1);
                    Throw.Act();
                    Helper.WaitSeconds(3);
                }
            }
            else if (FindTargetMode == RogueFindTargetMode.Throw)
            {
                validTarget = WowApi.CurrentPlayerData.PlayerHasTarget &&
                            WowApi.CurrentPlayerData.TargetHealth == 100 &&
                            !WowApi.CurrentPlayerData.TargetInCombat &&
                            WowApi.CurrentPlayerData.TargetFaction == 0 &&
                            WowApi.CurrentPlayerData.IsInFarRange &&
                            !WowApi.CurrentPlayerData.IsInCloseRange;

                if (validTarget)
                {
                    WaypointFollower.StopFollowingWaypoints();

                    Helper.WaitSeconds(1);
                    Throw.Act();
                    Helper.WaitSeconds(3);
                }
            }
            else if (FindTargetMode == RogueFindTargetMode.Stealth)
            {
                if (Stealth.CanCastSpell)
                {
                    Stealth.CastSpell();
                    StaleStealthTimer.Start();
                }

                validTarget = WowApi.CurrentPlayerData.PlayerHasTarget &&
                            WowApi.CurrentPlayerData.TargetHealth == 100 &&
                            !WowApi.CurrentPlayerData.TargetInCombat &&
                            WowApi.CurrentPlayerData.TargetFaction == 0 &&
                            WowApi.CurrentPlayerData.IsInCloseRange;

                if (validTarget)
                {
                    if (WowApi.CurrentPlayerData.IsInCloseRange)
                    {
                        Input.KeyPress(VirtualKeyCode.VK_2);
                        Helper.WaitSeconds(0.1);
                    }
                }
            }
            else
            {
                validTarget = WowApi.CurrentPlayerData.PlayerHasTarget &&
                            WowApi.CurrentPlayerData.TargetHealth == 100 &&
                            !WowApi.CurrentPlayerData.TargetInCombat &&
                            WowApi.CurrentPlayerData.TargetFaction == 0 &&
                            WowApi.CurrentPlayerData.IsInCloseRange;

                if (validTarget)
                {
                    Input.KeyPress(VirtualKeyCode.VK_2);
                    Helper.WaitSeconds(0.1);
                }
            }

            if(!validTarget)
                Target.Act();
        }

        public override void AutoAttackTarget()
        {
            if (!WowApi.CurrentPlayerData.PlayerInCombat)
                return;
            else if (!WowApi.CurrentPlayerData.PlayerHasTarget)
                Target.Act();
            else if (!WowApi.CurrentPlayerData.PlayerIsAttacking)
                Attack.Act();
            else if (SliceAndDice.CanCastSpell)
                SliceAndDice.CastSpell();
            else if (Eviscerate.CanCastSpell)
                Eviscerate.CastSpell();
            else if (KidneyShot.CanCastSpell)
                KidneyShot.CastSpell();
            else if (Rupture.CanCastSpell)
                Rupture.CastSpell();
            else if (SinisterStrike.CanCastSpell)
                SinisterStrike.CastSpell();
        }

        public override void KillTarget()
        {
            if (Evasion.CanCastSpell)
                Evasion.CastSpell();
            else if (SliceAndDice.CanCastSpell)
                SliceAndDice.CastSpell();
            else if (Rupture.CanCastSpell)
                Rupture.CastSpell();
            else if (Eviscerate.CanCastSpell)
                Eviscerate.CastSpell();
            else if (SinisterStrike.CanCastSpell)
                SinisterStrike.CastSpell();
        }

        public override void RegenerateVitals()
        {
            Input.KeyPress(VirtualKeyCode.VK_T);
            Helper.WaitSeconds(WowAutomater.RegisterDelay);
        }
    }
}
