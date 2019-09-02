using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

using WindowsInput;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public class RecordPathEventArgs : EventArgs
    {
        public double X;
        public double Y;

        public RecordPathEventArgs(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class AutomaterStatusEventArgs : EventArgs
    {
        public string Status;

        public AutomaterStatusEventArgs(string status)
        {
            Status = status;
        }
    }

    public enum ActionMode
    {
        AutoAttack,
        FindTarget,
        KillTarget,
        LootTarget,
        RegenerateVitals,
        Revive,
        SellItems
    }

    public class WowAutomater
    {
        #region API

        private PlayerData m_CurrentPlayerDataContainer = new PlayerData();
        private object m_CurrentPlayerDataLock = new object();
        private PlayerData m_CurrentPlayerData
        {
            get { lock (m_CurrentPlayerDataLock) { return m_CurrentPlayerDataContainer; } }
            set { lock (m_CurrentPlayerDataLock) { m_CurrentPlayerDataContainer = value; } }
        }

        private EventWaitHandle m_ApiEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private void WoWAPIUpdateEvent(object sender, WowApiUpdateEventArguments wea)
        {
            m_CurrentPlayerData = wea.PlayerData;
            m_ApiEventWaitHandle.Set();
        }
        
        #endregion

        #region Input

        private InputSimulator m_InputSimulator = new InputSimulator();

        private Random m_Random = new Random();

        private void KeyPress(VirtualKeyCode vk)
        {
            m_InputSimulator.Keyboard.KeyPress(vk);
            InputSleep();
        }

        private void KeyDown(VirtualKeyCode vk)
        {
            m_InputSimulator.Keyboard.KeyDown(vk);
            InputSleep();
        }

        private void KeyUp(VirtualKeyCode vk)
        {
            m_InputSimulator.Keyboard.KeyUp(vk);
            InputSleep();
        }

        private void RightClick()
        {
            m_InputSimulator.Mouse.RightButtonClick();
            InputSleep();
        }

        private void RightClickDown()
        {
            m_InputSimulator.Mouse.RightButtonDown();
            InputSleep();
        }

        private void RightClickUp()
        {
            m_InputSimulator.Mouse.RightButtonUp();
            InputSleep();
        }

        private void LeftClick()
        {
            m_InputSimulator.Mouse.LeftButtonClick();
            InputSleep();
        }

        private void BottomClick()
        {
            m_InputSimulator.Mouse.XButtonClick(3);
            InputSleep();
        }

        private void TopClick()
        {
            m_InputSimulator.Mouse.XButtonClick(4);
            InputSleep();
        }

        private void LeftClickDown()
        {
            m_InputSimulator.Mouse.LeftButtonDown();
            InputSleep();
        }

        private void LeftClickUp()
        {
            m_InputSimulator.Mouse.LeftButtonUp();
            InputSleep();
        }

        private void InputSleep()
        {
            double rand = m_Random.NextDouble() * 0.001;
            Block(0.001 + rand);
        }

        private static void Block(double seconds)
        {
            double durationTicks = Math.Round(seconds * Stopwatch.Frequency);

            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks) { };
        }

        #endregion

        #region Run

        public WowAutomater()
        {
            WowApi.UpdateEvent += WoWAPIUpdateEvent;

            Task.Run(() =>
            {
                WowApi.Run();
            });
        }

        private volatile ActionMode m_CurrentActionMode = ActionMode.FindTarget;

        public delegate void AutomaterStatusEventHandler(object sender, AutomaterStatusEventArgs wea);
        public event AutomaterStatusEventHandler AutomaterStatusEvent;

        public bool AutoAttackMode
        {
            set
            {
                if (value)
                    m_CurrentActionMode = ActionMode.AutoAttack;
                else
                    m_CurrentActionMode = ActionMode.FindTarget;
            }
        }

        private List<double> m_XCoordinates = null;
        private List<double> m_YCoordinates = null;

        private List<double> m_PathXCoordinates = null;
        private List<double> m_PathYCoordinates = null;
        private List<double> m_ReviveXCoordinates = null;
        private List<double> m_ReviveYCoordinates = null;
        private List<double> m_ShopXCoordinates = null;
        private List<double> m_ShopYCoordinates = null;

        private EventWaitHandle m_StopEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private volatile bool m_Run = true;

        private bool m_NoDead = false;
        private bool m_NoShop = false;
        private bool m_Initialized = false;

        private void Idle()
        {
            StopFollowingWaypoints();

            m_CurrentActionMode = m_CurrentActionMode == ActionMode.AutoAttack ? ActionMode.AutoAttack : ActionMode.FindTarget;

            if (!m_CurrentPlayerData.Found)
            {
                AutomaterStatusEvent?.Invoke(this, new AutomaterStatusEventArgs("API not found"));
            }
            else if (!m_CurrentPlayerData.IsWowForeground)
            {
                AutomaterStatusEvent?.Invoke(this, new AutomaterStatusEventArgs("Wow not selected"));
            }
            else if (!m_CurrentPlayerData.Start)
            {
                AutomaterStatusEvent?.Invoke(this, new AutomaterStatusEventArgs("Ready"));
            }
        }

        public void SetPathCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_PathXCoordinates = new List<double>(xCoordinates);
            m_PathYCoordinates = new List<double>(yCoordinates);

            m_XCoordinates = new List<double>(m_PathXCoordinates);
            m_YCoordinates = new List<double>(m_PathYCoordinates);
        }

        public void SetReviveCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ReviveXCoordinates = new List<double>(xCoordinates);
            m_ReviveYCoordinates = new List<double>(yCoordinates);

            if (m_ReviveXCoordinates.Count == 0)
                m_NoDead = true;
        }

        public void SetShopCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ShopXCoordinates = new List<double>(xCoordinates);
            m_ShopYCoordinates = new List<double>(yCoordinates);

            if (m_ShopXCoordinates.Count == 0)
                m_NoShop = true;
        }

        public void Stop()
        {
            m_Run = false;
            m_StopEventWaitHandle.WaitOne();
            WowApi.UpdateEvent -= WoWAPIUpdateEvent;
        }

        public void Run()
        {
            while (m_Run)
            {
                // Sync with API
                m_ApiEventWaitHandle.WaitOne();

                bool canRun = m_CurrentPlayerData.Start &&
                              m_CurrentPlayerData.Found &&
                              m_CurrentPlayerData.IsWowForeground &&
                              !m_RecordPath;

                if (canRun)
                {
                    if(!m_Initialized)
                    {
                        InitClasses();
                        m_Initialized = true;
                    }

                    switch (m_CurrentActionMode)
                    {
                        case ActionMode.AutoAttack:
                            AutoAttackTarget();
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
                            RunFromGraveToBody();
                            break;
                        case ActionMode.SellItems:
                            SellItems();
                            break;
                        default:
                            break;
                    }

                    AutomaterStatusEvent?.Invoke(this, new AutomaterStatusEventArgs(m_CurrentActionMode.ToString()));
                }
                else
                {
                    Idle();
                }

                m_StopEventWaitHandle.Set();
            }
        }

        #endregion

        #region Actions

        #region Generic

        public double RegisterDelay = 0.1;

        private bool m_Ghosted = false;
        private bool m_Fighting = false;
        private bool m_FarTarget = false;
        private bool m_Potion = false;
        private bool m_StartedEating = false;
        private bool m_Melee = false;
        private Stopwatch m_ReviveSw = new Stopwatch();

        private void InitClasses()
        {
            switch (m_CurrentPlayerData.Class)
            {
                case PlayerClass.Warrior:
                    InitWarrior();
                    break;
                case PlayerClass.Paladin:
                    InitPaladin();
                    break;
                case PlayerClass.Rogue:
                    InitRogue();
                    break;
                case PlayerClass.Priest:
                    InitPriest();
                    break;
                case PlayerClass.Mage:
                    InitMage();
                    break;
                case PlayerClass.Warlock:
                    InitWarlock();
                    break;
                case PlayerClass.Hunter:
                    InitHunter();
                    break;
                case PlayerClass.Shaman:
                    InitShaman();
                    break;
                case PlayerClass.Druid:
                    InitDruid();
                    break;
                default:
                    break;
            }

            m_CurrentActionMode = ActionMode.FindTarget;
        }

        private void RunFromGraveToBody()
        {
            if (m_NoDead)
                return;

            if (!m_Ghosted)
            {
                Block(5.0);
                m_InputSimulator.Mouse.MoveMouseTo(27300, 13000);
                Block(0.1);
                LeftClick();
                m_Ghosted = true;

                m_XCoordinates.Clear();
                m_XCoordinates.AddRange(m_ReviveXCoordinates);
                m_XCoordinates.AddRange(m_PathXCoordinates);

                m_YCoordinates.Clear();
                m_YCoordinates.AddRange(m_ReviveYCoordinates);
                m_YCoordinates.AddRange(m_PathYCoordinates);

                m_WaypointIndex = 0;

                Block(1.0);
                m_ReviveSw.Start();
            }

            FollowWaypoints();

            if (m_ReviveSw.ElapsedMilliseconds > 1000)
            {
                LeftClick();
                m_ReviveSw.Restart();
            }

            if (m_CurrentPlayerData.PlayerHealth > 1)
            {
                m_Ghosted = false;
                m_CurrentActionMode = ActionMode.RegenerateVitals;
                m_ReviveSw.Stop();

                KeyUp(VirtualKeyCode.VK_W);
                KeyUp(VirtualKeyCode.VK_A);
                KeyUp(VirtualKeyCode.VK_D);

                m_FollowingWaypoints = false;

                m_XCoordinates.Clear();
                m_XCoordinates.AddRange(m_PathXCoordinates);

                m_YCoordinates.Clear();
                m_YCoordinates.AddRange(m_PathYCoordinates);
            }
        }

        private void SellItems()
        {
            if (m_NoShop)
                return;
        }

        private void LootTarget()
        {
            Block(0.500);
            KeyDown(VirtualKeyCode.LSHIFT);

            m_InputSimulator.Mouse.MoveMouseTo(33300, 40000);
            RightClick();
            Block(0.500);

            m_InputSimulator.Mouse.MoveMouseTo(43300, 40000);
            RightClick();
            Block(0.500);

            m_InputSimulator.Mouse.MoveMouseTo(23300, 40000);
            RightClick();
            Block(0.500);

            m_InputSimulator.Mouse.MoveMouseTo(33300, 50000);
            RightClick();
            Block(0.500);

            m_InputSimulator.Mouse.MoveMouseTo(43300, 50000);
            RightClick();
            Block(0.500);

            m_InputSimulator.Mouse.MoveMouseTo(23300, 50000);
            RightClick();
            Block(0.500);

            KeyUp(VirtualKeyCode.LSHIFT);

            if (m_CurrentPlayerData.PlayerInCombat)
                m_CurrentActionMode = ActionMode.KillTarget;
            else if ((double)m_CurrentPlayerData.PlayerHealth / m_CurrentPlayerData.MaxPlayerHealth < 0.6)
                m_CurrentActionMode = ActionMode.RegenerateVitals;
            else
                m_CurrentActionMode = ActionMode.FindTarget;
        }

        private void RegenerateVitals()
        {
            if (!m_StartedEating)
            {
                Block(1.500);
                KeyPress(VirtualKeyCode.VK_8);
                Block(RegisterDelay);
                KeyPress(VirtualKeyCode.VK_9);
                m_StartedEating = true;
            }
            else if (m_CurrentPlayerData.PlayerInCombat)
            {
                m_StartedEating = false;
                m_CurrentActionMode = ActionMode.KillTarget;
            }
            else if (m_CurrentPlayerData.PlayerHealth == m_CurrentPlayerData.MaxPlayerHealth)
            {
                m_CurrentActionMode = ActionMode.FindTarget;
                m_StartedEating = false;
            }
        }

        private void FindTarget()
        {
            switch(m_CurrentPlayerData.Class)
            {
                case PlayerClass.Warrior:
                    FindTargetWarrior();
                    break;
                case PlayerClass.Paladin:
                    FindTargetPaladin();
                    break;
                case PlayerClass.Rogue:
                    FindTargetRogue();
                    break;
                case PlayerClass.Priest:
                    FindTargetPriest();
                    break;
                case PlayerClass.Mage:
                    FindTargetMage();
                    break;
                case PlayerClass.Warlock:
                    FindTargetWarlock();
                    break;
                case PlayerClass.Hunter:
                    FindTargetHunter();
                    break;
                case PlayerClass.Shaman:
                    FindTargetShaman();
                    break;
                case PlayerClass.Druid:
                    FindTargetDruid();
                    break;
                default:
                    break;
            }
        }

        private void AutoAttackTarget()
        {
            switch (m_CurrentPlayerData.Class)
            {
                case PlayerClass.Warrior:
                    AutoAttackTargetWarrior();
                    break;
                case PlayerClass.Paladin:
                    AutoAttackTargetPaladin();
                    break;
                case PlayerClass.Rogue:
                    AutoAttackTargetRogue();
                    break;
                case PlayerClass.Priest:
                    AutoAttackTargetPriest();
                    break;
                case PlayerClass.Mage:
                    AutoAttackTargetMage();
                    break;
                case PlayerClass.Warlock:
                    AutoAttackTargetWarlock();
                    break;
                case PlayerClass.Hunter:
                    AutoAttackTargetHunter();
                    break;
                case PlayerClass.Shaman:
                    AutoAttackTargetShaman();
                    break;
                case PlayerClass.Druid:
                    AutoAttackTargetDruid();
                    break;
                default:
                    break;
            }
        }

        private void KillTarget()
        {
            double healthRatio = (double)m_CurrentPlayerData.PlayerHealth / m_CurrentPlayerData.MaxPlayerHealth;

            if (m_CurrentPlayerData.IsPlayerDead)
            {
                m_SlicedAndDiced = false;
                m_Potion = false;
                m_Fighting = false;
                m_CurrentActionMode = ActionMode.Revive;
            }
            else if (!m_CurrentPlayerData.PlayerInCombat)
            {
                m_SlicedAndDiced = false;
                m_Potion = false;
                m_Fighting = false;
                m_CurrentActionMode = ActionMode.LootTarget;
            }
            // Wrong target
            else if (!m_CurrentPlayerData.PlayerHasTarget ||
                      !m_CurrentPlayerData.TargetInCombat ||
                      m_CurrentPlayerData.IsTargetPlayer)
            {
                if (!m_Fighting)
                {
                    KeyDown(VirtualKeyCode.VK_S);
                    Block(0.25);
                    KeyUp(VirtualKeyCode.VK_S);
                    KeyPress(VirtualKeyCode.TAB);
                    Block(RegisterDelay);
                }
            }
            // Wait for enemy to be close
            else if (!m_CurrentPlayerData.IsInCloseRange)
            {
                if (!m_FarTarget)
                {
                    KeyDown(VirtualKeyCode.VK_S);
                    Block(0.25);
                    KeyUp(VirtualKeyCode.VK_S);
                }

                m_FarTarget = true;
            }
            else if (m_FarTarget && m_CurrentPlayerData.IsInCloseRange)
            {
                KeyDown(VirtualKeyCode.VK_S);
                Block(0.25);
                KeyUp(VirtualKeyCode.VK_S);
                Block(0.75);
                m_FarTarget = false;
            }
            else if (WowApi.ActionErrorNeedsResolution)
            {
                KeyDown(VirtualKeyCode.VK_S);
                Block(0.25);
                KeyUp(VirtualKeyCode.VK_S);
                Block(RegisterDelay);
                WowApi.ActionErrorNeedsResolution = false;
            }
            else if (!m_CurrentPlayerData.PlayerIsAttacking)
            {
                KeyPress(VirtualKeyCode.VK_1);
                Block(RegisterDelay);
            }
            else if (healthRatio < 0.1 && !m_Potion)
            {
                KeyPress(VirtualKeyCode.VK_0);
                m_Potion = true;
            }
            else if (m_CurrentPlayerData.CanUseSkill)
            {
                m_Fighting = true;

                switch (m_CurrentPlayerData.Class)
                {
                    case PlayerClass.Warrior:
                        KillTargetWarrior();
                        break;
                    case PlayerClass.Paladin:
                        KillTargetPaladin();
                        break;
                    case PlayerClass.Rogue:
                        KillTargetRogue();
                        break;
                    case PlayerClass.Priest:
                        KillTargetPriest();
                        break;
                    case PlayerClass.Mage:
                        KillTargetMage();
                        break;
                    case PlayerClass.Warlock:
                        KillTargetWarlock();
                        break;
                    case PlayerClass.Hunter:
                        KillTargetHunter();
                        break;
                    case PlayerClass.Shaman:
                        KillTargetShaman();
                        break;
                    case PlayerClass.Druid:
                        KillTargetDruid();
                        break;
                    default:
                        break;
                }

            }

            // Strafe randomly

            if ((m_Random.NextDouble() <= 0.005) && m_Melee)
            {
                if (m_Random.NextDouble() >= 0.5)
                {
                    Task.Run(() =>
                    {
                        KeyDown(VirtualKeyCode.LEFT);
                        Block(0.075);
                        KeyUp(VirtualKeyCode.LEFT);
                        KeyDown(VirtualKeyCode.RIGHT);
                        Block(0.075);
                        KeyUp(VirtualKeyCode.RIGHT);
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        KeyDown(VirtualKeyCode.UP);
                        Block(0.075);
                        KeyUp(VirtualKeyCode.UP);
                        KeyDown(VirtualKeyCode.DOWN);
                        Block(0.075);
                        KeyUp(VirtualKeyCode.DOWN);
                    });
                }
            }

        }

        #endregion

        #region Warrior

        private void InitWarrior()
        {
            m_Melee = true;
        }

        bool ToggleAttackFlag = true;

        private void FindTargetWarrior()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                ToggleAttackFlag = true;
                WowApi.ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints();

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            if (ToggleAttackFlag && m_CurrentPlayerData.PlayerHasTarget) 
            {
                KeyPress(VirtualKeyCode.VK_1);
                ToggleAttackFlag = false;
            }

        }

        private void AutoAttackTargetWarrior()
        {

        }

        private void KillTargetWarrior()
        {
            if(m_CurrentPlayerData.PlayerMana > 15)
            {
                KeyPress(VirtualKeyCode.VK_2);
            }
        }

        #endregion

        #region Paladin

        private void InitPaladin()
        {

        }

        private void FindTargetPaladin()
        {

        }

        private void AutoAttackTargetPaladin()
        {

        }

        private void KillTargetPaladin()
        {

        }

        #endregion

        #region Rogue

        public int SliceAndDiceComboPoints = 1;
        public int RuptureComboPoints = 3;
        public int EvisceratePercentage = 25;
        public int StealthLevel = 10;

        private const int STEALTH_COOLDOWN_TIME_SEC = 10;

        private volatile bool m_CanStealth = true;
        private bool m_Stealthed = false;
        private bool m_SlicedAndDiced = false;
        private System.Timers.Timer m_CanStealthTimer = new System.Timers.Timer();
        public System.Timers.Timer StaleStealthTimer = new System.Timers.Timer();
        private Stopwatch m_RuptureSW = new Stopwatch();
        private Stopwatch m_KidneyShotSW = new Stopwatch();

        private void InitRogue()
        {
            m_Melee = true;

            m_CanStealthTimer.Interval = STEALTH_COOLDOWN_TIME_SEC * 1000;
            m_CanStealthTimer.Elapsed += CanStealthTimer_Elapsed;

            StaleStealthTimer.Interval = STEALTH_COOLDOWN_TIME_SEC * 1000;
            StaleStealthTimer.Elapsed += StaleStealthTimer_Elapsed;
        }

        private void StaleStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Stealthed)
            {
                KeyPress(VirtualKeyCode.VK_T);
                m_Stealthed = false;
                m_CanStealthTimer.Start();
            }

            StaleStealthTimer.Stop();
        }

        private void CanStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanStealth = true;

            m_CanStealthTimer.Stop();
        }

        private void FindTargetRogue()
        {
            if (m_CurrentPlayerData.PlayerLevel < 5)
            {
                FindTargetStartRogue();
            }
            else if (m_CurrentPlayerData.PlayerLevel < StealthLevel)
            {
                FindTargetNoStealthRogue();
            }
            else
            {
                FindTargetStealthRogue();
            }
        }

        private void FindTargetStealthRogue()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_Stealthed = false;
                StaleStealthTimer.Stop();
                m_CanStealthTimer.Start();

                m_CurrentActionMode = ActionMode.KillTarget;
                WowApi.ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints();

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            if (m_CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
                                 !m_CurrentPlayerData.TargetInCombat &&
                                 !m_CurrentPlayerData.IsTargetPlayer &&
                                  m_CurrentPlayerData.IsInFarRange;

                if (validEnemy && m_CurrentPlayerData.CanUseSkill)
                {
                    if (!m_Stealthed && m_CanStealth)
                    {
                        KeyPress(VirtualKeyCode.VK_T);

                        StaleStealthTimer.Start();
                        m_Stealthed = true;
                        m_CanStealth = false;
                    }

                    KeyPress(VirtualKeyCode.VK_2);
                }
            }
        }

        private void FindTargetStartRogue()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                WowApi.ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints();

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            // Found a target
            if (m_CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
                                  !m_CurrentPlayerData.TargetInCombat &&
                                  !m_CurrentPlayerData.IsTargetPlayer &&
                                   m_CurrentPlayerData.IsInFarRange &&
                                  !m_CurrentPlayerData.IsInCloseRange;

                if (validEnemy)
                {
                    StopFollowingWaypoints();

                    // Throw dagger
                    Block(1);
                    KeyPress(VirtualKeyCode.VK_4);
                    Block(3);
                }
            }
        }

        private void FindTargetNoStealthRogue()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                WowApi.ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints();

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            if (m_CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
                                  !m_CurrentPlayerData.TargetInCombat &&
                                  !m_CurrentPlayerData.IsTargetPlayer &&
                                  m_CurrentPlayerData.IsInCloseRange;

                if (validEnemy)
                {
                    KeyPress(VirtualKeyCode.VK_2);
                }
            }

        }

        private void AutoAttackTargetRogue()
        {
            if (!m_CurrentPlayerData.PlayerInCombat)
            {
                m_SlicedAndDiced = false;
                return;
            }
            else if (!m_CurrentPlayerData.PlayerHasTarget)
            {
                m_SlicedAndDiced = false;
                KeyPress(VirtualKeyCode.TAB);
            }
            else if (!m_CurrentPlayerData.PlayerIsAttacking)
            {
                KeyPress(VirtualKeyCode.VK_1);
                Block(0.05);
                return;
            }
            else if (m_CurrentPlayerData.CanUseSkill)
            {
                if (!m_SlicedAndDiced && m_CurrentPlayerData.TargetComboPoints == 1 && m_CurrentPlayerData.PlayerLevel > 9)
                {
                    if (m_CurrentPlayerData.PlayerMana >= 25)
                    {
                        KeyPress(VirtualKeyCode.VK_5);
                        m_SlicedAndDiced = true;
                    }
                }
                else if ((m_CurrentPlayerData.TargetHealth < 20 && m_CurrentPlayerData.TargetComboPoints > 0) ||
                    m_CurrentPlayerData.TargetComboPoints == 5)
                {
                    if (m_CurrentPlayerData.PlayerMana >= 35)
                    {
                        KeyPress(VirtualKeyCode.VK_3);
                    }
                }
                else if (m_CurrentPlayerData.TargetComboPoints >= 3 && m_CurrentPlayerData.PlayerLevel > 29 && (m_KidneyShotSW.ElapsedMilliseconds > 20000 || !m_KidneyShotSW.IsRunning))
                {
                    if (m_CurrentPlayerData.PlayerMana >= 25)
                    {
                        m_KidneyShotSW.Restart();
                        KeyPress(VirtualKeyCode.VK_7);
                    }
                }
                else if (m_CurrentPlayerData.TargetComboPoints >= 3 && m_CurrentPlayerData.PlayerLevel > 19 && (m_RuptureSW.ElapsedMilliseconds > 12000 || !m_RuptureSW.IsRunning))
                {
                    if (m_CurrentPlayerData.PlayerMana >= 25)
                    {
                        KeyPress(VirtualKeyCode.VK_6);
                        m_RuptureSW.Restart();
                    }
                }
                else
                {
                    if (m_CurrentPlayerData.PlayerLevel > 10)
                    {
                        if (m_CurrentPlayerData.PlayerMana >= 40)
                            KeyPress(VirtualKeyCode.VK_2);
                    }
                    else if (m_CurrentPlayerData.PlayerLevel > 9)
                    {
                        if (m_CurrentPlayerData.PlayerMana >= 43)
                            KeyPress(VirtualKeyCode.VK_2);
                    }
                    else
                    {
                        if (m_CurrentPlayerData.PlayerMana >= 45)
                            KeyPress(VirtualKeyCode.VK_2);
                    }

                }
            }
        }

        private void KillTargetRogue()
        {
            if (!m_SlicedAndDiced &&
                 m_CurrentPlayerData.TargetComboPoints == SliceAndDiceComboPoints &&
                 m_CurrentPlayerData.PlayerLevel > 9 &&
                 m_CurrentPlayerData.PlayerMana >= 25)
            {
                KeyPress(VirtualKeyCode.VK_5);
                m_SlicedAndDiced = true;
            }
            else if (m_CurrentPlayerData.TargetComboPoints == RuptureComboPoints &&
                 m_CurrentPlayerData.PlayerLevel > 19 &&
                 m_CurrentPlayerData.PlayerMana >= 25)
            {
                KeyPress(VirtualKeyCode.VK_6);
            }
            else if ((m_CurrentPlayerData.TargetHealth < 25 ||
                      m_CurrentPlayerData.TargetComboPoints == 5) &&
                      m_CurrentPlayerData.PlayerMana >= 35)
            {
                KeyPress(VirtualKeyCode.VK_3);
            }
            else
            {
                if (m_CurrentPlayerData.PlayerLevel > 10 &&
                    m_CurrentPlayerData.PlayerMana >= 40)
                {
                    KeyPress(VirtualKeyCode.VK_2);
                }
                else if (m_CurrentPlayerData.PlayerLevel > 9 &&
                         m_CurrentPlayerData.PlayerMana >= 43)
                {
                    KeyPress(VirtualKeyCode.VK_2);

                }
                else if (m_CurrentPlayerData.PlayerMana >= 45)
                {

                    KeyPress(VirtualKeyCode.VK_2);
                }
            }
        }

        #endregion

        #region Priest

        private void InitPriest()
        {

        }

        private void FindTargetPriest()
        {

        }

        private void AutoAttackTargetPriest()
        {
            
        }

        private void KillTargetPriest()
        {
            
        }

        #endregion

        #region Mage

        private void InitMage()
        {

        }

        private void FindTargetMage()
        {

        }

        private void AutoAttackTargetMage()
        {

        }

        private void KillTargetMage()
        {

        }

        #endregion

        #region Warlock

        private void InitWarlock()
        {

        }

        private void FindTargetWarlock()
        {

        }

        private void AutoAttackTargetWarlock()
        {

        }

        private void KillTargetWarlock()
        {

        }

        #endregion

        #region Hunter

        private System.Timers.Timer m_RaptorTimer = new System.Timers.Timer();
        bool RapFlag = true;
        private void InitHunter()
        {
            m_RaptorTimer.Interval = 6.5 * 1000;
            m_RaptorTimer.Elapsed += M_RaptorTimer_Elapsed;
        }

        private void M_RaptorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RapFlag = true;
            m_RaptorTimer.Stop();
        }

        private void FindTargetHunter()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                WowApi.ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints();

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            // Found a target
            if (m_CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
                                    !m_CurrentPlayerData.TargetInCombat &&
                                    !m_CurrentPlayerData.IsTargetPlayer &&
                                    m_CurrentPlayerData.IsInFarRange &&
                                    !m_CurrentPlayerData.IsInCloseRange;

                if (validEnemy)
                {
                    StopFollowingWaypoints();

                    // PewPew Bow
                    Block(1);
                    KeyPress(VirtualKeyCode.VK_3);
                    Block(4);
                }
            }
        }

        private void AutoAttackTargetHunter()
        {

        }

        private void KillTargetHunter()
        {

            if (m_CurrentPlayerData.PlayerMana >= 15 && RapFlag)
            {
                KeyPress(VirtualKeyCode.VK_2);
                Console.WriteLine("**FUCKAYU");
                RapFlag = false;
                m_RaptorTimer.Start();
            }


        }

        #endregion

        #region Shaman

        private void InitShaman()
        {

        }

        private void FindTargetShaman()
        {

        }

        private void AutoAttackTargetShaman()
        {

        }

        private void KillTargetShaman()
        {

        }

        #endregion

        #region Druid

        private void InitDruid()
        {

        }

        private void FindTargetDruid()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                WowApi.ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints();

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            // Found a target
            if (m_CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
                                    !m_CurrentPlayerData.TargetInCombat &&
                                    !m_CurrentPlayerData.IsTargetPlayer &&
                                    m_CurrentPlayerData.IsInFarRange &&
                                    !m_CurrentPlayerData.IsInCloseRange;

                if (validEnemy && m_CurrentPlayerData.PlayerMana >= 20)
                {
                    StopFollowingWaypoints();

                    // PewPew Wrath
                    Block(1);
                    KeyPress(VirtualKeyCode.VK_2);
                    Block(1.75);
                    KeyPress(VirtualKeyCode.VK_2);
                    Block(1.75);
                }
            }
        }

        private void AutoAttackTargetDruid()
        {

        }

        private void KillTargetDruid()
        {
            if(m_CurrentPlayerData.PlayerMana >= 20)
            {
                KeyPress(VirtualKeyCode.VK_2);
                Block(0.5);
            }
        }

        #endregion

        #endregion

        #region Record Path

        public double SplitDistance = 0.25;
        private volatile bool m_RecordPath = false;

        public void RecordPath()
        {
            m_RecordPath = true;

            double lastSplitX = -1;
            double lastSplitY = -1;
            while (m_RecordPath)
            {
                m_ApiEventWaitHandle.WaitOne();

                if (m_CurrentPlayerData.Found)
                {
                    double currentX = m_CurrentPlayerData.PlayerXPosition;
                    double currentY = m_CurrentPlayerData.PlayerYPosition;

                    if (lastSplitX == -1)
                    {
                        lastSplitX = currentX;
                        lastSplitY = currentY;

                        RecordPathEvent?.Invoke(this, new RecordPathEventArgs(currentX, currentY));

                        continue;
                    }

                    double splitDistance = Math.Sqrt((currentX - lastSplitX) * (currentX - lastSplitX) + (currentY - lastSplitY) * (currentY - lastSplitY));

                    if (splitDistance < SplitDistance)
                        continue;

                    lastSplitX = currentX;
                    lastSplitY = currentY;

                    RecordPathEvent?.Invoke(this, new RecordPathEventArgs(currentX, currentY));
                }
            }

            StopEvent?.Invoke(this, new EventArgs());
        }

        public void StopRecordingPath()
        {
            m_RecordPath = false;
        }

        public delegate void RecordPathEventHandler(object sender, RecordPathEventArgs wea);
        public event RecordPathEventHandler RecordPathEvent;

        public delegate void StopRecordingHandler(object sender, EventArgs ea);
        public event StopRecordingHandler StopEvent;

        #endregion

        #region Waypoints

        public double TurnToleranceRad  = 0.07; 
        public double PositionTolerance = 0.05;
        public double ClosestPointDistance = 1.00;

        private int m_WaypointIndex = 0;
        private bool m_FollowingWaypoints = false;
        private EventWaitHandle m_StopWaypointEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private TurningDirection m_TurningDirection = TurningDirection.None;

        private void FindClosestWaypoint()
        {
            double closestDistance = double.MaxValue;
            int closestWaypointIndex = 0;

            for (int i = 0; i < m_XCoordinates.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow((m_XCoordinates[i] - m_CurrentPlayerData.PlayerXPosition), 2) +
                                            Math.Pow((m_YCoordinates[i] - m_CurrentPlayerData.PlayerYPosition), 2));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointIndex = i;
                }
            }

            m_WaypointIndex = closestWaypointIndex;
        }

        private void StopFollowingWaypoints()
        {
            if (m_FollowingWaypoints)
            {
                m_FollowingWaypoints = false;
                m_StopWaypointEventWaitHandle.WaitOne();

                KeyUp(VirtualKeyCode.VK_W);
                KeyUp(VirtualKeyCode.VK_A);
                KeyUp(VirtualKeyCode.VK_D);

                m_TurningDirection = TurningDirection.None;
            }
        }

        enum TurningDirection
        {
            None,
            Left,
            Right
        }

        private void FollowWaypoints()
        {
            if (!m_FollowingWaypoints)
            {
                m_FollowingWaypoints = true;

                Block(1);
                KeyDown(VirtualKeyCode.VK_W);

                Task.Run(() =>
                {
                    while (m_FollowingWaypoints)
                    {
                        m_ApiEventWaitHandle.WaitOne();

                        double distanceToWaypoint = Math.Sqrt(Math.Pow(m_XCoordinates[m_WaypointIndex] - m_CurrentPlayerData.PlayerXPosition, 2) +
                                                           Math.Pow(m_YCoordinates[m_WaypointIndex] - m_CurrentPlayerData.PlayerYPosition, 2));

                        if (distanceToWaypoint > ClosestPointDistance)
                            FindClosestWaypoint();

                        double actualHeading_x = (m_CurrentPlayerData.PlayerXPosition - Math.Sin(m_CurrentPlayerData.PlayerHeading)) -
                                                 m_CurrentPlayerData.PlayerXPosition;

                        double actualHeading_y = (m_CurrentPlayerData.PlayerYPosition - Math.Cos(m_CurrentPlayerData.PlayerHeading)) -
                                                 m_CurrentPlayerData.PlayerYPosition;

                        double desiredHeading_x = m_XCoordinates[m_WaypointIndex] - m_CurrentPlayerData.PlayerXPosition;
                        double desiredHeading_y = m_YCoordinates[m_WaypointIndex] - m_CurrentPlayerData.PlayerYPosition;

                        double requriedTurn = Math.Atan2(actualHeading_x * desiredHeading_y - actualHeading_y * desiredHeading_x,
                                                         actualHeading_x * desiredHeading_x + actualHeading_y * desiredHeading_y);

                        if (Math.Abs(requriedTurn) < TurnToleranceRad)
                        {
                            if(m_TurningDirection != TurningDirection.None)
                            {
                                KeyUp(VirtualKeyCode.VK_D);
                                KeyUp(VirtualKeyCode.VK_A);
                            }

                            m_TurningDirection = TurningDirection.None;
                        }
                        else if (requriedTurn > 0)
                        {
                            if(m_TurningDirection != TurningDirection.Right)
                            {
                                KeyUp(VirtualKeyCode.VK_A);
                                KeyDown(VirtualKeyCode.VK_D);
                            }

                            m_TurningDirection = TurningDirection.Right;
                        }
                        else if (requriedTurn < 0)
                        {
                            if (m_TurningDirection != TurningDirection.Left)
                            {
                                KeyUp(VirtualKeyCode.VK_D);
                                KeyDown(VirtualKeyCode.VK_A);
                            }

                            m_TurningDirection = TurningDirection.Left;
                        }

                        if (m_Random.NextDouble() <= 0.01)
                        {
                            double rand = m_Random.NextDouble();
                            if (rand < 0.33)
                            {
                                KeyPress(VirtualKeyCode.SPACE);
                            }
                            else if (rand >= 0.33 && rand < 0.66)
                            {
                                Task.Run(() =>
                                {
                                    KeyDown(VirtualKeyCode.LEFT);
                                    Block(0.100);
                                    KeyUp(VirtualKeyCode.LEFT);
                                });
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    KeyDown(VirtualKeyCode.RIGHT);
                                    Block(0.100);
                                    KeyUp(VirtualKeyCode.RIGHT);
                                });
                            }
                        }

                        bool xReached = Math.Abs(m_CurrentPlayerData.PlayerXPosition - m_XCoordinates[m_WaypointIndex]) < PositionTolerance;
                        bool yReached = Math.Abs(m_CurrentPlayerData.PlayerYPosition - m_YCoordinates[m_WaypointIndex]) < PositionTolerance;

                        if (xReached && yReached)
                        {
                            m_WaypointIndex++;

                            if (m_WaypointIndex == m_XCoordinates.Count)
                            {
                                m_XCoordinates.Reverse();
                                m_YCoordinates.Reverse();

                                m_WaypointIndex = 0;
                            }
                        }
                    }
                    m_StopWaypointEventWaitHandle.Set();
                });
            }
        }
        
        #endregion
    }
}
