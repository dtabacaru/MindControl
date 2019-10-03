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
        public bool Stealth = true;
        public bool ThrowWeapon = false;
        public int StealthCooldown = 10;

        private volatile bool m_CanStealth = true;
        private bool m_Stealthed = false;

        public Timer StealthTimer = new Timer();
        public Timer StaleStealthTimer = new Timer();

        public Action Attack;
        public Action Target;
        public Action Throw;

        public Spell SinisterStrike;
        public FinishingSpell Eviscerate;
        public Spell SliceAndDice;
        public Spell Rupture;
        public Spell KidneyShot;
        public Spell Evasion;

        public override bool IsMelee => true;
        public RogueAutomater()
        {
            Attack = new Action(VirtualKeyCode.VK_1);
            Target = new Action(VirtualKeyCode.TAB);
            Throw = new Action(VirtualKeyCode.VK_4);

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
            if (m_Stealthed)
            {
                Input.KeyPress(VirtualKeyCode.VK_T);
                m_Stealthed = false;
                StealthTimer.Start();
            }

            StaleStealthTimer.Stop();
        }

        private void CanStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanStealth = true;
            StealthTimer.Stop();
        }

        public override void FindTarget()
        {
            WaypointFollower.FollowWaypoints(true);

            // Look for target
            Input.KeyPress(VirtualKeyCode.TAB);
            Helper.WaitSeconds(0.1);

            if (WowApi.CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = false;

                if ((WowApi.CurrentPlayerData.PlayerLevel < 5 || ThrowWeapon) && WowApi.CurrentPlayerData.AmmoCount > 0)
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
                        Input.KeyPress(VirtualKeyCode.VK_4);
                        Helper.WaitSeconds(3);
                    }
                }
                else if (WowApi.CurrentPlayerData.PlayerLevel > StealthLevel && Stealth)
                {
                    validEnemy = WowApi.CurrentPlayerData.TargetHealth == 100 &&
                                !WowApi.CurrentPlayerData.TargetInCombat &&
                                !WowApi.CurrentPlayerData.IsTargetPlayer &&
                                WowApi.CurrentPlayerData.IsInFarRange;

                    if (validEnemy && WowApi.CurrentPlayerData.CanUseSkill)
                    {
                        if (!m_Stealthed && m_CanStealth)
                        {
                            Input.KeyPress(VirtualKeyCode.VK_T);

                            StaleStealthTimer.Start();
                            m_Stealthed = true;
                            m_CanStealth = false;
                        }

                        Input.KeyPress(VirtualKeyCode.VK_2);
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
            
        }
    }
}
