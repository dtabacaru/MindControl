using System;
using System.Timers;
using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public class RogueAutomater : WowClassAutomater
    {
        public bool StealthFlag = true;
        public bool ThrowFlag = true;
        public int StealthCooldown = 10;
        public bool RuptureFirst = true;
        public bool AlwaysStealth = true;
        public volatile bool ApplyPoison = true;
        public volatile bool DontThrow = true;
        public volatile bool StopAroundPlayers = false;

        public Timer StaleStealthTimer = new Timer();
        public Timer FriendlyTimer = new Timer();
        public volatile bool FriendlyFlag = false;

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
        public Spell AdrenalineRush;
        public Spell BladeFlurry;

        public CompoundSpell PoisonMain;
        public CompoundSpell PoisonOff;

        public FinishingSpell Eviscerate;

        public override bool IsMelee => true;
        public RogueAutomater()
        {
            Attack = new Action(VirtualKeyCode.VK_1);
            Target = new Action(VirtualKeyCode.TAB);
            Throw = new Spell(VirtualKeyCode.VK_4, cooldownTime: 7.5);

            Stealth = new BuffSpell(VirtualKeyCode.VK_T, BuffType.Stealth, cooldownTime: 10);
            SinisterStrike = new Spell(VirtualKeyCode.VK_2, 45);
            Eviscerate = new FinishingSpell(VirtualKeyCode.VK_3, 25, 5, 5, 35);
            SliceAndDice = new ComboPointSpell(VirtualKeyCode.VK_5, 1, 1, 25, level: 10, useOnce: true);
            Rupture = new ComboPointSpell(VirtualKeyCode.VK_6, 3, 5, 25, 6 + 3 * 2, level: 20);
            KidneyShot = new ComboPointSpell(VirtualKeyCode.VK_7, 3, 5, 25, 20, level: 30);
            Evasion = new Spell(VirtualKeyCode.VK_L, cooldownTime: 5 * 60, healthPercentage: 25, level: 8);
            AdrenalineRush = new Spell(VirtualKeyCode.VK_J, cooldownTime: 5 * 60, healthPercentage: 35, level: 40);
            BladeFlurry = new Spell(VirtualKeyCode.VK_N, cooldownTime: 2 * 60, healthPercentage: 35, level: 31);
            EquipAmmo = new Spell(VirtualKeyCode.VK_Z, cooldownTime: 10);

            PoisonMain = new CompoundSpell(VirtualKeyCode.VK_G, VirtualKeyCode.VK_Y, cooldownTime: 31 * 60, level: 20);
            PoisonOff = new CompoundSpell(VirtualKeyCode.VK_G, VirtualKeyCode.VK_H, cooldownTime: 31 * 60, level: 20);

            StaleStealthTimer.Interval = StealthCooldown * 1000;
            StaleStealthTimer.Elapsed += StaleStealthTimer_Elapsed;
            
            FriendlyTimer.Interval = 30000;
            FriendlyTimer.Elapsed += FriendlyTimer_Elapsed;

            Api.UpdateEvent += Api_UpdateEvent;
        }

        private void FriendlyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Input.KeyPress(VirtualKeyCode.VK_F);

            FriendlyFlag = false;
            FriendlyTimer.Stop();
        }

        private void Api_UpdateEvent(object sender, EventArgs ea)
        {
            if (Automater.AutomaterActionMode == ActionMode.KillTarget)
                StaleStealthTimer.Stop();
        }

        private void StaleStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((Api.PlayerData.Buffs & (int)BuffType.Stealth) > 0)
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
            if (Api.PlayerData.AmmoCount == 1 && EquipAmmo.CanCastSpell && ThrowFlag)
                EquipAmmo.CastSpell();

            if (StealthFlag && ThrowFlag && !FriendlyFlag)
            {
                FindTargetMode = RogueFindTargetMode.StealthAndThrow;
            }
            else if (StealthFlag)
            {
                FindTargetMode = RogueFindTargetMode.Stealth;
            }
            else if (ThrowFlag && !FriendlyFlag)
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

            if (PoisonMain.CanCastSpell && ApplyPoison)
            {
                WaypointFollower.StopFollowingWaypoints();
                Helper.WaitSeconds(1.0);
                PoisonMain.CastSpell();
                Helper.WaitSeconds(4.5);
                PoisonOff.CastSpell();
                Helper.WaitSeconds(4.5);
                Input.RightClick();
                return;
            }

            if(!FriendlyFlag && DontThrow)
            {
                Input.KeyPress(VirtualKeyCode.VK_P);
                Helper.WaitSeconds(Automater.RegisterDelay);
            }

            if (Api.PlayerData.IsTargetPlayer && !FriendlyFlag && DontThrow)
            {
                FriendlyTimer.Start();
                FriendlyFlag = true;
            }

            CheckFindMode();
            bool validTarget;

            if (FindTargetMode == RogueFindTargetMode.StealthAndThrow)
            {
                if (Stealth.CanCastSpell)
                {
                    Stealth.CastSpell();

                    if (!AlwaysStealth)
                        StaleStealthTimer.Start();
                }

                validTarget = Api.PlayerData.PlayerHasTarget &&
                            Api.PlayerData.TargetHealth == 100 &&
                            !Api.PlayerData.TargetInCombat &&
                            Api.PlayerData.TargetFaction == 0 &&
                            Api.PlayerData.IsInFarRange &&
                            !Api.PlayerData.IsInCloseRange &&
                            Throw.CanCastSpell;

                if (validTarget)
                {
                    WaypointFollower.StopFollowingWaypoints();

                    Helper.WaitSeconds(1);
                    Throw.CastSpell();
                    Helper.WaitSeconds(2.5);
                }
            }
            else if (FindTargetMode == RogueFindTargetMode.Throw)
            {
                validTarget = Api.PlayerData.PlayerHasTarget &&
                            Api.PlayerData.TargetHealth == 100 &&
                            !Api.PlayerData.TargetInCombat &&
                            Api.PlayerData.TargetFaction == 0 &&
                            Api.PlayerData.IsInFarRange &&
                            !Api.PlayerData.IsInCloseRange &&
                            Throw.CanCastSpell;

                if (validTarget)
                {
                    WaypointFollower.StopFollowingWaypoints();

                    Helper.WaitSeconds(1);
                    Throw.CastSpell();
                    Helper.WaitSeconds(2.5);
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

                validTarget = Api.PlayerData.PlayerHasTarget &&
                            Api.PlayerData.TargetHealth == 100 &&
                            !Api.PlayerData.TargetInCombat &&
                            Api.PlayerData.TargetFaction == 0 &&
                            Api.PlayerData.IsInCloseRange;

                if (validTarget)
                {
                    if (Api.PlayerData.IsInCloseRange)
                    {
                        Input.KeyPress(VirtualKeyCode.VK_2);
                        Helper.WaitSeconds(0.1);
                    }
                }
            }
            else
            {
                validTarget = Api.PlayerData.PlayerHasTarget &&
                            Api.PlayerData.TargetHealth == 100 &&
                            !Api.PlayerData.TargetInCombat &&
                            Api.PlayerData.TargetFaction == 0 &&
                            Api.PlayerData.IsInCloseRange;

                if (validTarget)
                {
                    Input.KeyPress(VirtualKeyCode.VK_2);
                    Helper.WaitSeconds(0.1);
                }
            }

            if (!validTarget)
                Target.Act();
        }

        public override void AutoAttackTarget()
        {
            if (!Api.PlayerData.PlayerInCombat)
                return;
            else if (!Api.PlayerData.PlayerHasTarget)
                Target.Act();
            else if (!Api.PlayerData.PlayerIsAttacking)
                Attack.Act();
            else if (SliceAndDice.CanCastSpell)
                SliceAndDice.CastSpell();
            else if (Eviscerate.CanCastSpell)
                Eviscerate.CastSpell();
            else if (KidneyShot.CanCastSpell || Rupture.CanCastSpell)
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
            if (AdrenalineRush.CanCastSpell)
                AdrenalineRush.CastSpell();
            if (BladeFlurry.CanCastSpell)
                BladeFlurry.CastSpell();
            else if (Evasion.CanCastSpell)
                Evasion.CastSpell();
            else if (SliceAndDice.CanCastSpell)
                SliceAndDice.CastSpell();
            else if (Eviscerate.CanCastSpell)
                Eviscerate.CastSpell();
            else if (KidneyShot.CanCastSpell || Rupture.CanCastSpell)
            {
                if (RuptureFirst)
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

        public override void RegenerateVitals()
        {
            Input.KeyPress(VirtualKeyCode.VK_T);
            Helper.WaitSeconds(Automater.RegisterDelay);
        }
    }
}
