using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public enum FirstSealType
    {
        None,
        Crusader,
        Justice
    }

    public class PaladinAutomater : WowClassAutomater
    {
        public Action Attack;
        public Action Target;

        public FirstSealType FirstSeal = FirstSealType.None;

        public BuffSpell SealOfTheCrusader;
        public BuffSpell SealOfCommand;
        public BuffSpell SealOfJustice;

        public BuffSpell BlessingOfWisdom;

        public SealSpell Judgement;

        public Spell HolyLight;
        public Spell HammerOfJustice;

        public PaladinAutomater()
        {
            Attack = new Action(VirtualKeyCode.VK_1);
            Target = new Action(VirtualKeyCode.TAB);

            SealOfTheCrusader = new BuffSpell(VirtualKeyCode.VK_2, BuffType.SealOfTheCrusader, 34, level: 6, useOnce: true);
            SealOfCommand = new BuffSpell(VirtualKeyCode.VK_4, BuffType.SealOfCommand, 55, level: 20);
            Judgement = new SealSpell(VirtualKeyCode.VK_3, 37, 8);
            BlessingOfWisdom = new BuffSpell(VirtualKeyCode.VK_T, BuffType.BlessingOfWisdom, 45);
            SealOfJustice = new BuffSpell(VirtualKeyCode.VK_L, BuffType.SealOfJustice, 81, useOnce: true );

            HammerOfJustice = new Spell(VirtualKeyCode.VK_5, 50, 60, 40);
            HolyLight = new Spell(VirtualKeyCode.VK_6, 275, healthPercentage: 40);
        }

        public override bool IsMelee => true;

        public override void AutoAttackTarget()
        {
            if (BlessingOfWisdom.CanCastSpell)
                BlessingOfWisdom.CastSpell();
            else if (!Api.PlayerData.PlayerInCombat)
                return;
            else if (!Api.PlayerData.PlayerHasTarget)
                Target.Act();
            else if (!Api.PlayerData.PlayerIsAttacking)
                Attack.Act();
            else if (FirstSeal != FirstSealType.None)
            {
                switch (FirstSeal)
                {
                    case FirstSealType.Crusader:
                        if (SealOfTheCrusader.CanCastSpell)
                            SealOfTheCrusader.CastSpell();
                        break;
                    case FirstSealType.Justice:
                        if (SealOfJustice.CanCastSpell)
                            SealOfJustice.CastSpell();
                        break;
                }
            }
            else if (Judgement.CanCastSpell)
                Judgement.CastSpell();
            else if (SealOfCommand.CanCastSpell)
                SealOfCommand.CastSpell();
        }

        public override void FindTarget()
        {
            WaypointFollower.FollowWaypoints(true);

            if (FirstSeal != FirstSealType.None)
            {
                switch (FirstSeal)
                {
                    case FirstSealType.Crusader:
                        if (SealOfTheCrusader.CanCastSpell)
                            SealOfTheCrusader.CastSpell();
                        break;
                    case FirstSealType.Justice:
                        if (SealOfJustice.CanCastSpell)
                            SealOfJustice.CastSpell();
                        break;
                }
            }
            else if (SealOfCommand.CanCastSpell)
                SealOfCommand.CastSpell();

            bool validTarget = false;

            validTarget = Api.PlayerData.PlayerHasTarget &&
                            Api.PlayerData.TargetHealth == 100 &&
                            !Api.PlayerData.TargetInCombat &&
                            Api.PlayerData.TargetFaction == 0 &&
                            Api.PlayerData.IsInCloseRange;

            if (validTarget)
            {
                if (Api.PlayerData.IsInCloseRange)
                {
                    Input.KeyPress(VirtualKeyCode.VK_3);
                    Helper.WaitSeconds(0.5);
                }
            }
            else
                Target.Act();
        }

        public override void KillTarget()
        {
            if (BlessingOfWisdom.CanCastSpell)
                BlessingOfWisdom.CastSpell();
            else if (HammerOfJustice.CanCastSpell)
                HammerOfJustice.CastSpell();
            else if (HolyLight.CanCastSpell)
                HolyLight.CastSpell();
            else if (Judgement.CanCastSpell)
                Judgement.CastSpell();
            //else if (FirstSeal == FirstSealType.Crusader && SealOfTheCrusader.CanCastSpell)
                //SealOfTheCrusader.CastSpell();
            //else if (FirstSeal == FirstSealType.Justice && SealOfJustice.CanCastSpell)
                //SealOfJustice.CastSpell();
            else if (SealOfCommand.CanCastSpell)
                SealOfCommand.CastSpell();
        }

        public override void RegenerateVitals()
        {
            // Do nothing
        }
    }
}
