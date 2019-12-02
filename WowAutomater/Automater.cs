using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public enum ActionMode
    {
        AutoAttack,
        AutoWalk,
        FindTarget,
        KillTarget,
        LootTarget,
        RegenerateVitals,
        Revive,
        SellItems,
        ReadyToStart,
        WaitingForWow,
        WaitingForAddon,
        RemoteStop
    }

    public class AutomaterActionEventArgs : EventArgs
    {
        public ActionMode CurrentAction;

        public AutomaterActionEventArgs(ActionMode currentAction)
        {
            CurrentAction = currentAction;
        }
    }

    public static class Automater
    {
        public static ActionMode AutomaterActionMode
        {
            set
            {
                m_ResetCoordinates = true;
                m_InitializeAction = true;
                m_CurrentActionMode = value;
                m_PreviousActionMode = value;
            }
            get
            {
                return m_CurrentActionMode;
            }
        }

        public static void TransitionState(ActionMode action)
        {
            m_CurrentActionMode = action;
            m_InitializeAction = true;
        }

        private static volatile bool m_InitializeAction = true;
        private static volatile ActionMode m_PreviousActionMode = ActionMode.FindTarget;
        private static volatile ActionMode m_CurrentActionMode = ActionMode.FindTarget;

        public delegate void AutomaterActionEventHandler(object sender, AutomaterActionEventArgs wea);
        public static event AutomaterActionEventHandler AutomaterStatusEvent;

        private static List<double> m_PathXCoordinates = new List<double>();
        private static List<double> m_PathYCoordinates = new List<double>();
        private static List<double> m_ReviveXCoordinates = new List<double>();
        private static List<double> m_ReviveYCoordinates = new List<double>();
        private static List<double> m_ShopXCoordinates = new List<double>();
        private static List<double> m_ShopYCoordinates = new List<double>();
        private static List<double> m_WalkXCoordinates = new List<double>();
        private static List<double> m_WalkYCoordinates = new List<double>();

        private static EventWaitHandle m_ActionEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static volatile bool m_Run = true;

        private static bool m_NoDead = false;
        private static bool m_NoShop = false;
        private static bool m_NoWalk = false;
        private static bool m_ResetCoordinates = true;

        private static PlayerClassType m_CurrentClass = PlayerClassType.None;

        public static double RegisterDelay = 0.1;
        public static double XReviveButtonLocation = 32500;
        public static double YReviveButtonLocation = 14000;
        public static volatile bool SkinLoot = true;

        public static double RegenerateVitalsHealthPercentage = 60;

        private static bool m_WaddleDirection = true;
        private static volatile bool m_Waddling = false;

        private static bool m_Potion = false;

        private static bool m_Idle = false;
        
        private static volatile bool m_RelayFlag = false;
        private static volatile bool m_RemoteStopFlag = false;

        private static volatile string m_RelayString = string.Empty;

        public static volatile bool AutoLoot = false;

        private static Stopwatch m_ReviveSw = new Stopwatch();

        private static WowClassAutomater m_WowClassAutomater = null;

        public static WarriorAutomater Warrior = new WarriorAutomater();
        public static PaladinAutomater Paladin = new PaladinAutomater();
        public static RogueAutomater   Rogue =   new RogueAutomater();
        public static PriestAutomater  Priest =  new PriestAutomater();
        public static MageAutomater    Mage =    new MageAutomater();
        public static WarlockAutomater Warlock = new WarlockAutomater();
        public static HunterAutomater  Hunter =  new HunterAutomater();
        public static ShamanAutomater  Shaman =  new ShamanAutomater();
        public static DruidAutomater   Druid =   new DruidAutomater();

        public static Jitterizer Jitterizer = new Jitterizer();

        public static void SetRelayString(string relayString)
        {
            m_RelayString = relayString.Trim().ToLower();
            m_RelayFlag = true;
        }

        public static void RemoteStop()
        {
            m_RemoteStopFlag = true;
        }

        public static void RemoteStart()
        {
            m_RemoteStopFlag = false;
        }

        private static void TypeMessage(string message)
        {
            foreach (char c in message)
            {
                switch(c)
                {
                    case '0':
                        Input.KeyPress(VirtualKeyCode.VK_0);
                        break;
                    case '1':
                        Input.KeyPress(VirtualKeyCode.VK_1);
                        break;
                    case '2':
                        Input.KeyPress(VirtualKeyCode.VK_2);
                        break;
                    case '3':
                        Input.KeyPress(VirtualKeyCode.VK_3);
                        break;
                    case '4':
                        Input.KeyPress(VirtualKeyCode.VK_4);
                        break;
                    case '5':
                        Input.KeyPress(VirtualKeyCode.VK_5);
                        break;
                    case '6':
                        Input.KeyPress(VirtualKeyCode.VK_6);
                        break;
                    case '7':
                        Input.KeyPress(VirtualKeyCode.VK_7);
                        break;
                    case '8':
                        Input.KeyPress(VirtualKeyCode.VK_8);
                        break;
                    case '9':
                        Input.KeyPress(VirtualKeyCode.VK_9);
                        break;
                    case 'a':
                        Input.KeyPress(VirtualKeyCode.VK_A);
                        break;
                    case 'b':
                        Input.KeyPress(VirtualKeyCode.VK_B);
                        break;
                    case 'c':
                        Input.KeyPress(VirtualKeyCode.VK_C);
                        break;
                    case 'd':
                        Input.KeyPress(VirtualKeyCode.VK_D);
                        break;
                    case 'e':
                        Input.KeyPress(VirtualKeyCode.VK_E);
                        break;
                    case 'f':
                        Input.KeyPress(VirtualKeyCode.VK_F);
                        break;
                    case 'g':
                        Input.KeyPress(VirtualKeyCode.VK_G);
                        break;
                    case 'h':
                        Input.KeyPress(VirtualKeyCode.VK_H);
                        break;
                    case 'i':
                        Input.KeyPress(VirtualKeyCode.VK_I);
                        break;
                    case 'j':
                        Input.KeyPress(VirtualKeyCode.VK_J);
                        break;
                    case 'k':
                        Input.KeyPress(VirtualKeyCode.VK_K);
                        break;
                    case 'l':
                        Input.KeyPress(VirtualKeyCode.VK_L);
                        break;
                    case 'm':
                        Input.KeyPress(VirtualKeyCode.VK_M);
                        break;
                    case 'n':
                        Input.KeyPress(VirtualKeyCode.VK_N);
                        break;
                    case 'o':
                        Input.KeyPress(VirtualKeyCode.VK_O);
                        break;
                    case 'p':
                        Input.KeyPress(VirtualKeyCode.VK_P);
                        break;
                    case 'q':
                        Input.KeyPress(VirtualKeyCode.VK_Q);
                        break;
                    case 'r':
                        Input.KeyPress(VirtualKeyCode.VK_R);
                        break;
                    case 's':
                        Input.KeyPress(VirtualKeyCode.VK_S);
                        break;
                    case 't':
                        Input.KeyPress(VirtualKeyCode.VK_T);
                        break;
                    case 'u':
                        Input.KeyPress(VirtualKeyCode.VK_U);
                        break;
                    case 'v':
                        Input.KeyPress(VirtualKeyCode.VK_V);
                        break;
                    case 'w':
                        Input.KeyPress(VirtualKeyCode.VK_W);
                        break;
                    case 'x':
                        Input.KeyPress(VirtualKeyCode.VK_X);
                        break;
                    case 'y':
                        Input.KeyPress(VirtualKeyCode.VK_Y);
                        break;
                    case 'z':
                        Input.KeyPress(VirtualKeyCode.VK_Z);
                        break;
                    case ' ':
                        Input.KeyPress(VirtualKeyCode.SPACE);
                        break;
                    case '/':
                        Input.KeyPress(VirtualKeyCode.DIVIDE);
                        break;
                    default:
                        break;
                }

                Helper.WaitSeconds(0.025);
            }
        }

        private static void Relay()
        {
            Helper.WaitSeconds(0.5);

            Input.KeyPress(VirtualKeyCode.RETURN);
            Helper.WaitSeconds(RegisterDelay);
            TypeMessage(m_RelayString);
            Input.KeyPress(VirtualKeyCode.RETURN);
            Helper.WaitSeconds(RegisterDelay);

            Helper.WaitSeconds(0.5);
        }

        private static void ApiUpdateEvent(object sender, EventArgs wea)
        {
            CheckClass();
        }

        private static bool CheckCombat()
        {
            if (Api.PlayerData.PlayerInCombat)
            {
                TransitionState(ActionMode.KillTarget);
                return true;
            }

            return false;
        }

        private static bool CheckDead()
        {
            if (Api.PlayerData.IsPlayerDead)
            {
                TransitionState(ActionMode.Revive);
                return true;
            }

            return false;
        }

        private static void CheckClass()
        {
            if(Api.PlayerData.Found && m_CurrentClass != Api.PlayerData.Class)
            {
                m_CurrentClass = Api.PlayerData.Class;
                switch (Api.PlayerData.Class)
                {
                    case PlayerClassType.Warrior:
                        m_WowClassAutomater = Warrior;
                        break;
                    case PlayerClassType.Paladin:
                        m_WowClassAutomater = Paladin;
                        break;
                    case PlayerClassType.Rogue:
                        m_WowClassAutomater = Rogue;
                        break;
                    case PlayerClassType.Priest:
                        m_WowClassAutomater = Priest;
                        break;
                    case PlayerClassType.Mage:
                        m_WowClassAutomater = Mage;
                        break;
                    case PlayerClassType.Warlock:
                        m_WowClassAutomater = Warlock;
                        break;
                    case PlayerClassType.Hunter:
                        m_WowClassAutomater = Hunter;
                        break;
                    case PlayerClassType.Shaman:
                        m_WowClassAutomater = Shaman;
                        break;
                    case PlayerClassType.Druid:
                        m_WowClassAutomater = Druid;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SetPathCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count )
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_PathXCoordinates.Clear();
            m_PathXCoordinates.AddRange(xCoordinates);

            m_PathYCoordinates.Clear();
            m_PathYCoordinates.AddRange(yCoordinates);
        }

        public static void SetReviveCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_ReviveXCoordinates.Clear();
            m_ReviveXCoordinates.AddRange(xCoordinates);

            m_ReviveYCoordinates.Clear();
            m_ReviveYCoordinates.AddRange(yCoordinates);

            if (m_ReviveXCoordinates.Count == 0)
                m_NoDead = true;
            else
                m_NoDead = false;
        }

        public static void SetShopCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_ShopXCoordinates.Clear();
            m_ShopXCoordinates.AddRange(xCoordinates);

            m_ShopYCoordinates.Clear();
            m_ShopYCoordinates.AddRange(yCoordinates);

            if (m_ShopXCoordinates.Count == 0)
                m_NoShop = true;
            else
                m_NoShop = false;
        }

        public static void SetWalkCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_WalkXCoordinates.Clear();
            m_WalkXCoordinates.AddRange(xCoordinates);

            m_WalkYCoordinates.Clear();
            m_WalkYCoordinates.AddRange(yCoordinates);

            if (m_WalkXCoordinates.Count == 0)
                m_NoWalk = true;
            else
                m_NoWalk = false;
        }

        public static void Stop()
        {
            m_Run = false;
            m_ActionEventWaitHandle.WaitOne();
            Api.UpdateEvent -= ApiUpdateEvent;
        }

        private static void CheckIdle()
        {
            if(m_RelayFlag)
            {
                WaypointFollower.StopFollowingWaypoints();
                Relay();
                m_RelayFlag = false;
            }
            else if (m_RemoteStopFlag)
            {
                if (!m_Idle)
                {
                    WaypointFollower.StopFollowingWaypoints();
                    m_PreviousActionMode = m_CurrentActionMode;
                }

                m_CurrentActionMode = ActionMode.RemoteStop;
                m_Idle = true;
            }
            else if (!Api.PlayerData.IsWowForeground)
            {
                if(!m_Idle)
                {
                    WaypointFollower.StopFollowingWaypoints();
                    m_PreviousActionMode = m_CurrentActionMode;
                }

                m_CurrentActionMode = ActionMode.WaitingForWow;
                m_Idle = true;
            }
            else if (!Api.PlayerData.Found)
            {
                if (!m_Idle)
                {
                    WaypointFollower.StopFollowingWaypoints();
                    m_PreviousActionMode = m_CurrentActionMode;
                }

                m_CurrentActionMode = ActionMode.WaitingForAddon;
                m_Idle = true;
            }
            else if (!Api.PlayerData.Start)
            {
                if (!m_Idle)
                {
                    WaypointFollower.StopFollowingWaypoints();
                    m_PreviousActionMode = m_CurrentActionMode;
                }

                m_CurrentActionMode = ActionMode.ReadyToStart;
                m_Idle = true;
            }
            else if (m_Idle)
            {
                m_CurrentActionMode = m_PreviousActionMode;
                m_Idle = false;
            }
                
        }

        public static void Run()
        {
            Api.UpdateEvent += ApiUpdateEvent;

            while (m_Run)
            {
                Api.Sync.WaitOne();

                CheckIdle();

                switch (m_CurrentActionMode)
                {
                    case ActionMode.AutoAttack:
                        AutoAttackTarget();
                        break;
                    case ActionMode.AutoWalk:
                        AutoWalk();
                        break;
                    case ActionMode.FindTarget:
                        FindTarget();
                        break;
                    case ActionMode.KillTarget:
                        KillTarget();
                        break;
                    case ActionMode.LootTarget:
                        LootTarget();
                        break;
                    case ActionMode.RegenerateVitals:
                        RegenerateVitals();
                        break;
                    case ActionMode.Revive:
                        Revive();
                        break;
                    case ActionMode.SellItems:
                        SellItems();
                        break;
                    default:
                        break;
                }

                AutomaterStatusEvent?.Invoke(null, new AutomaterActionEventArgs(m_CurrentActionMode));
                m_ActionEventWaitHandle.Set();
            }
        }

        private static void AutoWalk()
        {
            if (m_NoWalk)
                return;

            if(m_ResetCoordinates)
            {
                WaypointFollower.SetWaypoints(m_WalkXCoordinates, m_WalkYCoordinates);

                m_ResetCoordinates = false;
            }

            WaypointFollower.FollowWaypoints(false);
        }

        private static void Revive()
        {
            if (m_NoDead)
                return;

            if (m_InitializeAction)
            {
                List<double> ghostXCoordinates = new List<double>();
                List<double> ghostYCoordinates = new List<double>();

                ghostXCoordinates.AddRange(m_ReviveXCoordinates);
                ghostXCoordinates.AddRange(m_PathXCoordinates);

                ghostYCoordinates.AddRange(m_ReviveYCoordinates);
                ghostYCoordinates.AddRange(m_PathYCoordinates);

                WaypointFollower.SetWaypoints(ghostXCoordinates, ghostYCoordinates);

                Helper.WaitSeconds(3.5);
                Input.MoveMouseTo(XReviveButtonLocation, YReviveButtonLocation);
                Helper.WaitSeconds(RegisterDelay);
                Input.LeftClick();

                Helper.WaitSeconds(1.0);
                m_ReviveSw.Start();

                m_InitializeAction = false;
            }

            WaypointFollower.FollowWaypoints(true);

            if (m_ReviveSw.ElapsedMilliseconds > 1000)
            {
                Input.LeftClick();
                m_ReviveSw.Restart();
            }

            if (Api.PlayerData.PlayerHealth > 1)
            {
                WaypointFollower.StopFollowingWaypoints();
                TransitionState(ActionMode.RegenerateVitals);
                m_ReviveSw.Stop();
                m_ResetCoordinates = true;

                Helper.WaitSeconds(1);
            }
        }

        private static void SellItems()
        {
            if (m_NoShop)
                return;
        }

        private static void LootTarget()
        {
            if (CheckCombat())
                return;

            if (CheckDead())
                return;

            Helper.WaitSeconds(1.0);

            if (!AutoLoot)
                Input.KeyDown(VirtualKeyCode.LSHIFT);

            for (int x = 25000; x < 41000; x += 1000)
            {
                for (int y = 25000; y < 49000; y += 1000)
                {
                    Input.MoveMouseTo(x, y);
                    Input.RightClick();
                    Helper.WaitSeconds(0.001);
                }
            }

            Helper.WaitSeconds(2.5);

            if (!AutoLoot)
                Input.KeyUp(VirtualKeyCode.LSHIFT);

            if (SkinLoot)
            {
                if (CheckCombat())
                    return;

                if (CheckDead())
                    return;

                if (!AutoLoot)
                    Input.KeyDown(VirtualKeyCode.LSHIFT);

                for (int x = 25000; x < 41000; x += 1000)
                {
                    for (int y = 25000; y < 49000; y += 1000)
                    {
                        Input.MoveMouseTo(x, y);
                        Input.RightClick();
                        Helper.WaitSeconds(0.001);
                    }
                }

                Helper.WaitSeconds(4);

                if (!AutoLoot)
                    Input.KeyUp(VirtualKeyCode.LSHIFT);
            }

            if (CheckCombat())
                return;

            if (CheckDead())
                return;

            if (Api.PlayerData.PlayerHealthPercentage <= RegenerateVitalsHealthPercentage)
                TransitionState(ActionMode.RegenerateVitals);
            else
                TransitionState(ActionMode.FindTarget);

        }

        private static void RegenerateVitals()
        {
            if (CheckCombat())
                return;

            if (CheckDead())
                return;

            if (m_InitializeAction)
            {
                WaypointFollower.StopFollowingWaypoints();
                Helper.WaitSeconds(1);
                Input.KeyPress(VirtualKeyCode.VK_8);
                Helper.WaitSeconds(RegisterDelay);
                Input.KeyPress(VirtualKeyCode.VK_9);
                Helper.WaitSeconds(RegisterDelay);

                m_WowClassAutomater.RegenerateVitals();

                m_InitializeAction = false;
            }

            else if (Api.PlayerData.PlayerHealth == Api.PlayerData.MaxPlayerHealth)
            {
                TransitionState(ActionMode.FindTarget);
            }
        }

        private static void FindTarget()
        {
            if (m_ResetCoordinates)
            {
                WaypointFollower.SetWaypoints(m_PathXCoordinates, m_PathYCoordinates);

                m_ResetCoordinates = false;
            }

            if (CheckCombat())
                return;

            if (CheckDead())
                return;

            m_WowClassAutomater.FindTarget();
        }

        private static void AutoAttackTarget()
        {
            m_WowClassAutomater.AutoAttackTarget();
        }

        private static void Waddle()
        {
            if (m_Waddling)
                return;

            m_Waddling = true;

            Task.Run(() =>
            {
                Input.KeyDown(VirtualKeyCode.VK_S);

                if (m_WaddleDirection)
                    Input.KeyDown(VirtualKeyCode.VK_D);
                else
                    Input.KeyDown(VirtualKeyCode.VK_A);

                Helper.WaitSeconds(0.25);

                Input.KeyUp(VirtualKeyCode.VK_S);

                if (m_WaddleDirection)
                    Input.KeyUp(VirtualKeyCode.VK_D);
                else
                    Input.KeyUp(VirtualKeyCode.VK_A);

                m_WaddleDirection = !m_WaddleDirection;

                Helper.WaitSeconds(0.1);

                m_Waddling = false;
            });
                
        }

        private static void KillTarget()
        {
            if (CheckDead())
                return;

            if (m_InitializeAction)
            {
                WaypointFollower.StopFollowingWaypoints();
                Helper.WaitSeconds(0.5);
                m_InitializeAction = false;
            }
            else if (!Api.PlayerData.PlayerInCombat)
            {
                Helper.WaitSeconds(RegisterDelay);
                m_Potion = false;
                TransitionState(ActionMode.LootTarget);
            }
            else if (!Api.PlayerData.PlayerHasTarget)
            {
                // Wait to be attacked
            }
            else if (!Api.PlayerData.TargetInCombat ||
                     Api.PlayerData.TargetFaction > 0)
            {
                Input.KeyPress(VirtualKeyCode.VK_F);
                Helper.WaitSeconds(RegisterDelay);
            }
            else if (Api.PlayerData.PlayerActionError > ActionErrorType.None &&
                    Api.PlayerData.PlayerActionError < ActionErrorType.OutOfRange)
            {
                Waddle();
                Helper.WaitSeconds(0.35);
            }
            else if (!Api.PlayerData.IsInCloseRange &&
                     m_WowClassAutomater.IsMelee)
            {
                Jitterizer.Jitter();
                Helper.WaitSeconds(0.5);
            }
            else if (!Api.PlayerData.PlayerIsAttacking)
            {
                Input.KeyPress(VirtualKeyCode.VK_1);
                Helper.WaitSeconds(RegisterDelay);
            }
            else if (Api.PlayerData.PlayerHealthPercentage < 10 && !m_Potion)
            {
                Input.KeyPress(VirtualKeyCode.VK_0);
                m_Potion = true;
            }
            else
            {
                m_WowClassAutomater.KillTarget();

                if (m_WowClassAutomater.IsMelee)
                    Jitterizer.RandomJitter();
            }

        }

    }
}
