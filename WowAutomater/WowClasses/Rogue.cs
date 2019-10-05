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

        private volatile bool m_CanStealth = true;

        public Timer StealthTimer = new Timer();
        public Timer StaleStealthTimer = new Timer();

        public Action Attack;
        public Action Target;
        public Action Throw;

        public BuffSpell Stealth;

        public Spell SinisterStrike;
        public Spell SliceAndDice;
        public Spell Rupture;
        public Spell KidneyShot;
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
            SliceAndDice = new Spell(VirtualKeyCode.VK_5, 25, comboPointsCost: 1, level: 10, useOnce: true);
            Rupture = new Spell(VirtualKeyCode.VK_6, 25, 6 + 3 * 2, comboPointsCost: 3, level: 20);
            KidneyShot = new Spell(VirtualKeyCode.VK_7, 25, 20, 3, level: 30);
            Evasion = new Spell(VirtualKeyCode.VK_L, cooldownTime: 5 * 60, healthPercentage: 40, level: 8);

            StealthTimer.Interval = StealthCooldown * 1000;
            StealthTimer.Elapsed += CanStealthTimer_Elapsed;

            StaleStealthTimer.Interval = StealthCooldown * 1000;
            StaleStealthTimer.Elapsed += StaleStealthTimer_Elapsed;
        }

        private void StaleStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((WowApi.CurrentPlayerData.Buffs & (int)BuffType.Stealth) > 0)
            {
                Input.KeyPress(VirtualKeyCode.VK_T);
            }

            StaleStealthTimer.Stop();
        }

        private void CanStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanStealth = true;
            StealthTimer.Stop();
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
                      (ThrowFlag && WowApi.CurrentPlayerData.AmmoCount > 0) )
            {
                FindTargetMode = RogueFindTargetMode.StealthAndThrow;
            }
            else if (StealthFlag && WowApi.CurrentPlayerData.PlayerLevel > StealthLevel)
            {
                FindTargetMode = RogueFindTargetMode.Stealth;
            }
            else if (ThrowFlag && WowApi.CurrentPlayerData.AmmoCount > 0)
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

            Target.Act();

            if (WowApi.CurrentPlayerData.PlayerHasTarget)
            {
                CheckFindMode();

                bool validEnemy = false;

                if (FindTargetMode == RogueFindTargetMode.Throw)
                {
                    validEnemy = WowApi.CurrentPlayerData.TargetHealth == 100 &&
                                !WowApi.CurrentPlayerData.TargetInCombat &&
                                !WowApi.CurrentPlayerData.IsTargetPlayer &&
                                WowApi.CurrentPlayerData.IsInFarRange &&
                                !WowApi.CurrentPlayerData.IsInCloseRange;

                    if (validEnemy)
                    {
                        WaypointFollower.StopFollowingWaypoints();

                        // Throw dagger
                        Helper.WaitSeconds(1);
                        Throw.Act();
                        Helper.WaitSeconds(3);
                    }
                }
                else if (FindTargetMode == RogueFindTargetMode.Stealth)
                {
                    validEnemy = WowApi.CurrentPlayerData.TargetHealth == 100 &&
                                !WowApi.CurrentPlayerData.TargetInCombat &&
                                !WowApi.CurrentPlayerData.IsTargetPlayer &&
                                WowApi.CurrentPlayerData.IsInFarRange;

                    if (validEnemy && WowApi.CurrentPlayerData.CanUseSkill)
                    {
                        if (Stealth.CanCastSpell)
                        {
                            Stealth.CastSpell();
                            StaleStealthTimer.Start();
                        }

                        if (WowApi.CurrentPlayerData.IsInCloseRange)
                        {
                            Input.KeyPress(VirtualKeyCode.VK_2);
                            Helper.WaitSeconds(0.1);
                        }

                    }
                }
                else if (FindTargetMode == RogueFindTargetMode.StealthAndThrow)
                {
                    if (Stealth.CanCastSpell)
                        Stealth.CastSpell();

                    validEnemy = WowApi.CurrentPlayerData.TargetHealth == 100 &&
                                !WowApi.CurrentPlayerData.TargetInCombat &&
                                !WowApi.CurrentPlayerData.IsTargetPlayer &&
                                WowApi.CurrentPlayerData.IsInFarRange &&
                                !WowApi.CurrentPlayerData.IsInCloseRange;

                    if (validEnemy)
                    {
                        WaypointFollower.StopFollowingWaypoints();

                        // Throw dagger
                        Helper.WaitSeconds(1);
                        Throw.Act();
                        Helper.WaitSeconds(3);
                    }
                }
                else
                {
                    validEnemy = WowApi.CurrentPlayerData.TargetHealth == 100 &&
                                !WowApi.CurrentPlayerData.TargetInCombat &&
                                !WowApi.CurrentPlayerData.IsTargetPlayer &&
                                WowApi.CurrentPlayerData.IsInCloseRange;

                    if (validEnemy)
                    {
                        Input.KeyPress(VirtualKeyCode.VK_2);
                        Helper.WaitSeconds(0.1);
                    }
                }


            }
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
