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
        public bool ThrowFlag = true;
        public int StealthCooldown = 10;
        public bool RuptureFirst = true;
        public bool AlwaysStealth = true;

        public Timer StaleStealthTimer = new Timer();

        public Action Attack;
        public Action Target;
        public Spell Throw;

        public BuffSpell Stealth;

        public Spell EquipAmmo;

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
            Throw = new Spell(VirtualKeyCode.VK_4, cooldownTime: 5);

            Stealth = new BuffSpell(VirtualKeyCode.VK_T, BuffType.Stealth, cooldownTime: 10);
            SinisterStrike = new Spell(VirtualKeyCode.VK_2, 45);
            Eviscerate = new FinishingSpell(VirtualKeyCode.VK_3, 25, 5, 5, 35);
            SliceAndDice = new ComboPointSpell(VirtualKeyCode.VK_5, 1, 1, 25, level: 10, useOnce: true);
            Rupture = new ComboPointSpell(VirtualKeyCode.VK_6, 3, 5, 25, 6 + 3 * 2, level: 20);
            KidneyShot = new ComboPointSpell(VirtualKeyCode.VK_7, 3, 5, 25, 20, level: 30);
            Evasion = new Spell(VirtualKeyCode.VK_L, cooldownTime: 5 * 60, healthPercentage: 40, level: 8);
            EquipAmmo = new Spell(VirtualKeyCode.VK_Z, cooldownTime: 10);

            StaleStealthTimer.Interval = StealthCooldown * 1000;
            StaleStealthTimer.Elapsed += StaleStealthTimer_Elapsed;

            WowApi.UpdateEvent += WowApi_UpdateEvent;
        }

        private void WowApi_UpdateEvent(object sender, EventArgs ea)
        {
            if (WowAutomater.CurrentActionMode == ActionMode.KillTarget)
                StaleStealthTimer.Stop();
        }

        private void StaleStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((WowApi.PlayerData.Buffs & (int)BuffType.Stealth) > 0)
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
            if (WowApi.PlayerData.AmmoCount == 1 && EquipAmmo.CanCastSpell && ThrowFlag)
                EquipAmmo.CastSpell();

            if ( (StealthFlag && WowApi.PlayerData.PlayerLevel > StealthLevel) && 
                      ThrowFlag)
            {
                FindTargetMode = RogueFindTargetMode.StealthAndThrow;
            }
            else if (StealthFlag && WowApi.PlayerData.PlayerLevel > StealthLevel)
            {
                FindTargetMode = RogueFindTargetMode.Stealth;
            }
            else if (ThrowFlag)
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
                {
                    Stealth.CastSpell();

                    if (!AlwaysStealth)
                        StaleStealthTimer.Start();
                }

                validTarget = WowApi.PlayerData.PlayerHasTarget &&
                            WowApi.PlayerData.TargetHealth == 100 &&
                            !WowApi.PlayerData.TargetInCombat &&
                            WowApi.PlayerData.TargetFaction == 0 &&
                            WowApi.PlayerData.IsInFarRange &&
                            !WowApi.PlayerData.IsInCloseRange &&
                            Throw.CanCastSpell;

                if (validTarget)
                {
                    WaypointFollower.StopFollowingWaypoints();

                    Helper.WaitSeconds(1);
                    Throw.CastSpell();
                    Helper.WaitSeconds(2);
                }
            }
            else if (FindTargetMode == RogueFindTargetMode.Throw)
            {
                validTarget = WowApi.PlayerData.PlayerHasTarget &&
                            WowApi.PlayerData.TargetHealth == 100 &&
                            !WowApi.PlayerData.TargetInCombat &&
                            WowApi.PlayerData.TargetFaction == 0 &&
                            WowApi.PlayerData.IsInFarRange &&
                            !WowApi.PlayerData.IsInCloseRange &&
                            Throw.CanCastSpell;

                if (validTarget)
                {
                    WaypointFollower.StopFollowingWaypoints();

                    Helper.WaitSeconds(1);
                    Throw.CastSpell();
                    Helper.WaitSeconds(2);
                }
            }
            else if (FindTargetMode == RogueFindTargetMode.Stealth)
            {
                if (Stealth.CanCastSpell)
                {
                    Stealth.CastSpell();

                    if(!AlwaysStealth)
                        StaleStealthTimer.Start();
                }

                validTarget = WowApi.PlayerData.PlayerHasTarget &&
                            WowApi.PlayerData.TargetHealth == 100 &&
                            !WowApi.PlayerData.TargetInCombat &&
                            WowApi.PlayerData.TargetFaction == 0 &&
                            WowApi.PlayerData.IsInCloseRange;

                if (validTarget)
                {
                    if (WowApi.PlayerData.IsInCloseRange)
                    {
                        Input.KeyPress(VirtualKeyCode.VK_2);
                        Helper.WaitSeconds(0.1);
                    }
                }
            }
            else
            {
                validTarget = WowApi.PlayerData.PlayerHasTarget &&
                            WowApi.PlayerData.TargetHealth == 100 &&
                            !WowApi.PlayerData.TargetInCombat &&
                            WowApi.PlayerData.TargetFaction == 0 &&
                            WowApi.PlayerData.IsInCloseRange;

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
            if (!WowApi.PlayerData.PlayerInCombat)
                return;
            else if (!WowApi.PlayerData.PlayerHasTarget)
                Target.Act();
            else if (!WowApi.PlayerData.PlayerIsAttacking)
                Attack.Act();
            else if (SliceAndDice.CanCastSpell)
                SliceAndDice.CastSpell();
            else if (Eviscerate.CanCastSpell)
                Eviscerate.CastSpell();
            else if (Eviscerate.CanCastSpell || Rupture.CanCastSpell)
            {
                if(RuptureFirst)
                {
                    if (Rupture.CanCastSpell)
                        Rupture.CastSpell();
                    else if (KidneyShot.CanCastSpell)
                        KidneyShot.CastSpell();
                }
                else
                {
                    if (KidneyShot.CanCastSpell)
                        KidneyShot.CastSpell();
                    else if (Rupture.CanCastSpell)
                        Rupture.CastSpell();
                }
            }
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
