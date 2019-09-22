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
        AutoWalk,
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

            if (m_CurrentPlayerData.CastingInterrupted || m_CurrentPlayerData.CastingSucceeded)
                m_Casting = false;

            if (m_CurrentPlayerData.PlayerActionError == ActionError.BehindTarget || m_CurrentPlayerData.PlayerActionError == ActionError.FacingWrongWay)
                m_ActionErrorNeedsResolution = true;

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

        public ActionMode CurrentActionMode
        {
            set
            {
                m_ResetCoordinates = true;
                m_CurrentActionMode = value;
            }
            get
            {
                return m_CurrentActionMode;
            }
        }

        private volatile ActionMode m_CurrentActionMode = ActionMode.FindTarget;

        public delegate void AutomaterStatusEventHandler(object sender, AutomaterStatusEventArgs wea);
        public event AutomaterStatusEventHandler AutomaterStatusEvent;

        private List<double> m_XCoordinates = new List<double>();
        private List<double> m_YCoordinates = new List<double>();

        private List<double> m_PathXCoordinates = new List<double>();
        private List<double> m_PathYCoordinates = new List<double>();
        private List<double> m_ReviveXCoordinates = new List<double>();
        private List<double> m_ReviveYCoordinates = new List<double>();
        private List<double> m_ShopXCoordinates = new List<double>();
        private List<double> m_ShopYCoordinates = new List<double>();
        private List<double> m_WalkXCoordinates = new List<double>();
        private List<double> m_WalkYCoordinates = new List<double>();

        private EventWaitHandle m_StopEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private volatile bool m_Run = true;

        private bool m_NoDead = false;
        private bool m_NoShop = false;
        private bool m_NoWalk = false;
        private bool m_Initialized = false;
        private bool m_ResetCoordinates = true;

        private void Idle()
        {
            StopFollowingWaypoints();

            if (m_CurrentActionMode == ActionMode.AutoAttack)
                m_CurrentActionMode = ActionMode.AutoAttack;
            else if (m_CurrentActionMode == ActionMode.AutoWalk)
                m_CurrentActionMode = ActionMode.AutoWalk;
            else
                m_CurrentActionMode = ActionMode.FindTarget;

            if (!m_CurrentPlayerData.IsWowForeground)
            {
                AutomaterStatusEvent?.Invoke(this, new AutomaterStatusEventArgs("Wow not selected"));
            }
            else if (!m_CurrentPlayerData.Found)
            {
                AutomaterStatusEvent?.Invoke(this, new AutomaterStatusEventArgs("Addon not found"));
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

            m_ResetCoordinates = true;

            m_PathXCoordinates = new List<double>(xCoordinates);
            m_PathYCoordinates = new List<double>(yCoordinates);
        }

        public void SetReviveCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_ReviveXCoordinates = new List<double>(xCoordinates);
            m_ReviveYCoordinates = new List<double>(yCoordinates);

            if (m_ReviveXCoordinates.Count == 0)
                m_NoDead = true;
        }

        public void SetShopCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_ShopXCoordinates = new List<double>(xCoordinates);
            m_ShopYCoordinates = new List<double>(yCoordinates);

            if (m_ShopXCoordinates.Count == 0)
                m_NoShop = true;
        }

        public void SetWalkCoordinates(List<double> xCoordinates, List<double> yCoordinates)
        {
            if (xCoordinates.Count != yCoordinates.Count)
                throw new Exception("The number of x and y coordinates must match.");

            m_ResetCoordinates = true;

            m_WalkXCoordinates = new List<double>(xCoordinates);
            m_WalkYCoordinates = new List<double>(yCoordinates);

            if (m_WalkXCoordinates.Count == 0)
                m_NoWalk = true;
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
        public double XReviveButtonLocation = 32500;
        public double YReviveButtonLocation = 14000;
        public volatile bool SkinLoot = false;

        private volatile bool m_ActionErrorNeedsResolution = false;

        private bool m_Ghosted = false;
        private bool m_Fighting = false;
        private bool m_FarTarget = false;
        private bool m_Potion = false;
        private bool m_StartedEating = false;
        private bool m_Melee = false;
        private bool m_MovedBack = false;
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

        }

        private void AutoWalk()
        {
            if (m_NoWalk)
                return;

            if(m_ResetCoordinates)
            {
                m_XCoordinates.Clear();
                m_YCoordinates.Clear();

                m_XCoordinates.AddRange(m_WalkXCoordinates);
                m_YCoordinates.AddRange(m_WalkYCoordinates);

                m_WaypointIndex = 0;

                m_ResetCoordinates = false;
            }

            FollowWaypoints(false);
        }

        private void RunFromGraveToBody()
        {
            if (m_NoDead)
                return;

            if (!m_Ghosted)
            {
                Block(5.0);
                m_InputSimulator.Mouse.MoveMouseTo(XReviveButtonLocation, YReviveButtonLocation);
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

            FollowWaypoints(true);

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
            Block(0.250);
            KeyDown(VirtualKeyCode.LSHIFT);

            m_InputSimulator.Mouse.MoveMouseTo(33300, 30000);
            RightClick();
            Block(0.250);

            m_InputSimulator.Mouse.MoveMouseTo(33300, 40000);
            RightClick();
            Block(0.250);

            m_InputSimulator.Mouse.MoveMouseTo(43300, 40000);
            RightClick();
            Block(0.250);

            m_InputSimulator.Mouse.MoveMouseTo(23300, 40000);
            RightClick();
            Block(0.250);

            m_InputSimulator.Mouse.MoveMouseTo(33300, 50000);
            RightClick();
            Block(0.250);

            m_InputSimulator.Mouse.MoveMouseTo(43300, 50000);
            RightClick();
            Block(0.250);

            m_InputSimulator.Mouse.MoveMouseTo(23300, 50000);
            RightClick();
            Block(0.250);

            KeyUp(VirtualKeyCode.LSHIFT);

            if(SkinLoot)
            {
                Block(0.250);
                KeyDown(VirtualKeyCode.LSHIFT);

                m_InputSimulator.Mouse.MoveMouseTo(33300, 30000);
                RightClick();
                Block(0.250);   

                m_InputSimulator.Mouse.MoveMouseTo(33300, 40000);
                RightClick();
                Block(0.250);

                m_InputSimulator.Mouse.MoveMouseTo(43300, 40000);
                RightClick();
                Block(0.250);

                m_InputSimulator.Mouse.MoveMouseTo(23300, 40000);
                RightClick();
                Block(0.250);

                m_InputSimulator.Mouse.MoveMouseTo(33300, 50000);
                RightClick();
                Block(0.250);

                m_InputSimulator.Mouse.MoveMouseTo(43300, 50000);
                RightClick();
                Block(0.250);

                m_InputSimulator.Mouse.MoveMouseTo(23300, 50000);
                RightClick();
                Block(0.250);

                Block(4.000);
                KeyUp(VirtualKeyCode.LSHIFT);
            }

            if (m_CurrentPlayerData.PlayerInCombat)
                m_CurrentActionMode = ActionMode.KillTarget;
            else if ((double)m_CurrentPlayerData.PlayerHealth / m_CurrentPlayerData.MaxPlayerHealth < 0.6)
                m_CurrentActionMode = ActionMode.RegenerateVitals;
            else
                m_CurrentActionMode = ActionMode.FindTarget;
        }

        private void RegenerateVitals()
        {
            switch (m_CurrentPlayerData.Class)
            {
                case PlayerClass.Warrior:
                    //RegenerateVitalsWarrior();
                    break;
                case PlayerClass.Paladin:
                    //RegenerateVitalsPaladin();
                    break;
                case PlayerClass.Rogue:
                    RegenerateVitalsRogue();
                    break;
                case PlayerClass.Priest:
                    //RegenerateVitalsPriest();
                    break;
                case PlayerClass.Mage:
                    //RegenerateVitalsMage();
                    break;
                case PlayerClass.Warlock:
                    //RegenerateVitalsWarlock();
                    break;
                case PlayerClass.Hunter:
                    //RegenerateVitalsHunter();
                    break;
                case PlayerClass.Shaman:
                    //RegenerateVitalsShaman();
                    break;
                case PlayerClass.Druid:
                    //RegenerateVitalsDruid();
                    break;
                default:
                    break;
            }

        }

        private void FindTarget()
        {
            if(m_ResetCoordinates)
            {
                m_XCoordinates.Clear();
                m_YCoordinates.Clear();

                m_XCoordinates.AddRange(m_PathXCoordinates);
                m_YCoordinates.AddRange(m_PathYCoordinates);

                m_WaypointIndex = 0;

                m_ResetCoordinates = false;
            }

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
                if (!m_FarTarget && m_Melee)
                {
                    KeyDown(VirtualKeyCode.VK_S);
                    Block(0.25);
                    KeyUp(VirtualKeyCode.VK_S);
                }

                m_FarTarget = true;
            }
            else if (m_FarTarget && m_CurrentPlayerData.IsInCloseRange)
            {
                if(m_Melee)
                {
                    KeyDown(VirtualKeyCode.VK_S);
                    Block(0.25);
                    KeyUp(VirtualKeyCode.VK_S);
                    Block(0.75);
                }

                m_FarTarget = false;
            }
            else if (m_ActionErrorNeedsResolution && m_CurrentPlayerData.PlayerIsAttacking)
            {
                KeyDown(VirtualKeyCode.VK_S);
                Block(0.25);
                KeyUp(VirtualKeyCode.VK_S);
                Block(RegisterDelay);
                m_ActionErrorNeedsResolution = false;
            }
            else if (!m_CurrentPlayerData.PlayerIsAttacking)
            {
                KeyPress(VirtualKeyCode.VK_1);
                Block(RegisterDelay);
                m_ActionErrorNeedsResolution = false;
            }
            else if (m_CurrentPlayerData.PlayerHealthPercentage < 10 && !m_Potion)
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
                m_ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints(true);

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
        public int EvasionPercentage = 40;
        public int StealthLevel = 10;
        public bool Stealth = true;
        public bool ThrowWeapon = false;

        public int StealthCooldown = 10;

        private const int KIDNEY_SHOT_COOLDOWN = 20;
        private const int EVASION_COOLDOWN = 5* 60;

        private volatile bool m_CanStealth = true;

        private bool m_Stealthed = false;
        private bool m_SlicedAndDiced = false;
        private bool m_CanRupture = true;
        private bool m_CanKidneyShot = true;
        private bool m_CanEvasion = true;

        public System.Timers.Timer StealthTimer = new System.Timers.Timer();
        public System.Timers.Timer StaleStealthTimer = new System.Timers.Timer();
        public System.Timers.Timer RuptureTimer = new System.Timers.Timer();
        private System.Timers.Timer m_KidneyShotTimer = new System.Timers.Timer();
        private System.Timers.Timer m_EvasionTimer = new System.Timers.Timer();

        private void InitRogue()
        {
            m_Melee = true;

            StealthTimer.Interval = StealthCooldown * 1000;
            StealthTimer.Elapsed += CanStealthTimer_Elapsed;

            StaleStealthTimer.Interval = StealthCooldown * 1000;
            StaleStealthTimer.Elapsed += StaleStealthTimer_Elapsed;

            RuptureTimer.Interval = (6 + RuptureComboPoints * 2) * 1000;
            RuptureTimer.Elapsed += RuptureTimer_Elapsed;

            m_KidneyShotTimer.Interval = KIDNEY_SHOT_COOLDOWN * 1000;
            m_KidneyShotTimer.Elapsed += KidneyShotTimer_Elapsed;

            m_EvasionTimer.Interval = EVASION_COOLDOWN * 1000;
            m_EvasionTimer.Elapsed += EvasionTimer_Elapsed; ;
        }

        private void EvasionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanEvasion = true;
            m_EvasionTimer.Stop();
        }

        private void KidneyShotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanKidneyShot = true;
            m_KidneyShotTimer.Stop();
        }

        private void RuptureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanRupture = true;
            RuptureTimer.Stop();
        }

        private void StaleStealthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Stealthed)
            {
                KeyPress(VirtualKeyCode.VK_T);
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

        private void FindTargetRogue()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_Stealthed = false;
                StaleStealthTimer.Stop();
                StealthTimer.Start();

                m_CurrentActionMode = ActionMode.KillTarget;
                m_ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints(true);

            // Look for target
            KeyPress(VirtualKeyCode.TAB);
            Block(RegisterDelay);

            if (m_CurrentPlayerData.PlayerHasTarget)
            {
                bool validEnemy = false;

                if ((m_CurrentPlayerData.PlayerLevel < 5 || ThrowWeapon) && m_CurrentPlayerData.AmmoCount > 0)
                {
                    validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
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
                else if (m_CurrentPlayerData.PlayerLevel > StealthLevel && Stealth)
                {
                    validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
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
                else
                {
                    validEnemy = m_CurrentPlayerData.TargetHealth == 100 &&
                                !m_CurrentPlayerData.TargetInCombat &&
                                !m_CurrentPlayerData.IsTargetPlayer &&
                                m_CurrentPlayerData.IsInCloseRange;

                    if (validEnemy)
                    {
                        KeyPress(VirtualKeyCode.VK_2);
                    }
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
                else if (m_CurrentPlayerData.TargetComboPoints >= 3 && m_CurrentPlayerData.PlayerLevel > 29 && m_CanKidneyShot)
                {
                    if (m_CurrentPlayerData.PlayerMana >= 25)
                    {
                        KeyPress(VirtualKeyCode.VK_7);
                        m_CanKidneyShot = false;
                        m_KidneyShotTimer.Start();
                    }
                }
                else if (m_CurrentPlayerData.TargetComboPoints >= 3 && m_CurrentPlayerData.PlayerLevel > 19 && m_CanRupture)
                {
                    if (m_CurrentPlayerData.PlayerMana >= 25)
                    {
                        KeyPress(VirtualKeyCode.VK_6);
                        m_CanRupture = false;
                        RuptureTimer.Start();
                    }
                }
                else
                {
                    if (m_CurrentPlayerData.PlayerLevel > 10)
                    {
                        if (m_CurrentPlayerData.PlayerMana >= 45)
                            KeyPress(VirtualKeyCode.VK_2);
                    }
                    else if (m_CurrentPlayerData.PlayerLevel > 9)
                    {
                        if (m_CurrentPlayerData.PlayerMana >= 45)
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
            if( (m_CurrentPlayerData.PlayerHealthPercentage <= EvasionPercentage) && m_CanEvasion)
            {
                KeyPress(VirtualKeyCode.VK_L);
                m_CanEvasion = false;
                m_EvasionTimer.Start();
            }
            else if (!m_SlicedAndDiced &&
                 m_CurrentPlayerData.TargetComboPoints == SliceAndDiceComboPoints &&
                 m_CurrentPlayerData.PlayerLevel > 9 &&
                 m_CurrentPlayerData.PlayerMana >= 25)
            {
                KeyPress(VirtualKeyCode.VK_5);
                m_SlicedAndDiced = true;
            }
            else if (m_CurrentPlayerData.TargetComboPoints == RuptureComboPoints &&
                 m_CurrentPlayerData.PlayerLevel > 19 &&
                 m_CurrentPlayerData.PlayerMana >= 25 &&
                 m_CanRupture)
            {
                KeyPress(VirtualKeyCode.VK_6);
                m_CanRupture = false;
                RuptureTimer.Start();
            }
            else if (( (m_CurrentPlayerData.TargetHealth < 25 && m_CurrentPlayerData.TargetComboPoints > 0) ||
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

        private void RegenerateVitalsRogue()
        {
            if (!m_StartedEating)
            {
                Block(1.500);
                KeyPress(VirtualKeyCode.VK_X);
                Block(RegisterDelay);
                KeyPress(VirtualKeyCode.VK_8);
                Block(RegisterDelay);
                KeyPress(VirtualKeyCode.VK_T);
                Block(RegisterDelay);
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
                KeyPress(VirtualKeyCode.VK_T);
                Block(RegisterDelay);
                m_CanStealth = false;
                StealthTimer.Start();
                m_StartedEating = false;
            }
        }

        #endregion

        #region Priest

        private void InitPriest()
        {

        }

        private void FindTargetPriest()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                m_ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints(true);

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
                                    !m_CurrentPlayerData.IsInMediumRange;

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

        private void AutoAttackTargetPriest()
        {
            
        }

        bool m_Casting = false;

        private void KillTargetPriest()
        {
            if (m_CurrentPlayerData.PlayerMana >= 20 && !m_Casting)
            {
                KeyPress(VirtualKeyCode.VK_2);
                m_Casting = true;
            }
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
                m_ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints(true);

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

        private const int MOTW_COOLDOWN_TIME_SEC = 60 * 30;
        private const int THORNS_COOLDOWN_TIME_SEC = 60 * 10;

        private System.Timers.Timer m_ThornsTimer = new System.Timers.Timer();
        private System.Timers.Timer m_MotwTimer = new System.Timers.Timer();

        private volatile bool m_CanThorns = true;
        private volatile bool m_CanMotw = true;

        private void InitDruid()
        {
            m_MotwTimer.Interval = MOTW_COOLDOWN_TIME_SEC * 1000;
            m_MotwTimer.Elapsed += MotwTimer_Elapsed; ;

            m_ThornsTimer.Interval = THORNS_COOLDOWN_TIME_SEC * 1000;
            m_ThornsTimer.Elapsed += ThornsTimer_Elapsed; ;
        }

        private void ThornsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanThorns = true;
            m_ThornsTimer.Stop();
        }

        private void MotwTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CanMotw = true;
            m_MotwTimer.Stop();
        }

        private void FindTargetDruid()
        {
            if (m_CurrentPlayerData.PlayerInCombat)
            {
                StopFollowingWaypoints();

                m_CurrentActionMode = ActionMode.KillTarget;
                m_ActionErrorNeedsResolution = false;
                return;
            }

            FollowWaypoints(true);

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
            if(m_CanThorns && m_CurrentPlayerData.CanUseSkill && m_CurrentPlayerData.PlayerMana >= 35)
            {
                KeyPress(VirtualKeyCode.VK_L);
                m_CanThorns = false;
                Block(0.1);
                m_ThornsTimer.Start();
            }
            else if(m_CanMotw && m_CurrentPlayerData.CanUseSkill && m_CurrentPlayerData.PlayerMana >= 20)
            {
                KeyPress(VirtualKeyCode.VK_P);
                m_CanMotw = false;
                Block(0.1);
                m_MotwTimer.Start();
            }
            else if (!m_CurrentPlayerData.PlayerInCombat)
            {
                return;
            }
            else if (!m_CurrentPlayerData.PlayerHasTarget)
            {
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
                if (m_CurrentPlayerData.PlayerMana >= 25 && m_CurrentPlayerData.PlayerHealthPercentage < 40 && !m_Casting)
                {
                    KeyPress(VirtualKeyCode.VK_3);
                    m_Casting = true;
                }
                else if (m_CurrentPlayerData.PlayerMana >= 35 && !m_Casting)
                {
                    KeyPress(VirtualKeyCode.VK_2);
                    m_Casting = true;
                }
            }
        }

        private void KillTargetDruid()
        {
            if (m_CurrentPlayerData.PlayerMana >= 20 && !m_Casting)
            {
                KeyPress(VirtualKeyCode.VK_2);
                m_Casting = true;
            }
        }

        #endregion

        #endregion

        #region Record Path

        public double SplitDistance = 0.5;
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

        public double TurnToleranceRad  = 0.08; 
        public double PositionTolerance = 0.08;
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

                m_TurningDirection = TurningDirection.None;
            }
        }

        enum TurningDirection
        {
            None,
            Left,
            Right
        }

        private void FollowWaypoints(bool loop)
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
                            if (m_TurningDirection != TurningDirection.None)
                            {
                                KeyUp(VirtualKeyCode.VK_D);
                                KeyUp(VirtualKeyCode.VK_A);
                            }

                            m_TurningDirection = TurningDirection.None;
                        }
                        else if (requriedTurn > 0)
                        {
                            if (m_TurningDirection != TurningDirection.Right)
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
                            if (m_WaypointIndex == (m_XCoordinates.Count - 1))
                            {
                                if (!loop)
                                {
                                    break;
                                }

                                m_XCoordinates.Reverse();
                                m_YCoordinates.Reverse();

                                m_WaypointIndex = 0;
                            }

                            m_WaypointIndex++;
                        }
                    }

                    KeyUp(VirtualKeyCode.VK_W);
                    KeyUp(VirtualKeyCode.VK_A);
                    KeyUp(VirtualKeyCode.VK_D);
                    m_StopWaypointEventWaitHandle.Set();
                });
            }
        }
        
        #endregion
    }
}
