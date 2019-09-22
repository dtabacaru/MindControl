using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClassicWowNeuralParasite
{
    public enum TargetReaction
    {
        NoTarget = 0,
        ExceptionallyHostile = 1,
        VeryHostile = 2,
        Hostile = 3,
        Neutral = 4,
        Friendly = 5,
        VeryFriendly = 6,
        ExceptionallyFriendly = 7,
        Exalted = 8
    }

    public enum PlayerClass
    {
        None = 0,
        Warrior = 1,
        Paladin = 2,
        Rogue = 3,
        Priest = 4,
        Mage = 5,
        Warlock = 6,
        Hunter = 7,
        Shaman = 8,
        Druid = 9,
        LastPlayerClass = Druid
    }

    public enum ActionError
    {
        None = 0,
        BehindTarget = 1,
        FacingWrongWay = 2
    }

    public class PlayerData
    {
        public double PlayerXPosition = 0;
        public double PlayerYPosition = 0;
        public double PlayerHeading = 0;
        public bool IsInFarRange = false;
        public bool IsInMediumRange = false;
        public bool IsInCloseRange = false;
        public ushort PlayerHealth = 0;
        public ushort MaxPlayerHealth = 0;
        public double PlayerHealthPercentage = 0;
        public ushort PlayerMana = 0;
        public ushort MaxPlayerMana = 0;
        public bool PlayerHasTarget = false;
        public ushort TargetHealth = 0;
        public ushort TargetMana = 0;
        public bool SpellCanAttackTarget = false;
        public bool CanAttackTarget = false;
        public bool PlayerInCombat = false;
        public bool TargetInCombat = false;
        public bool IsTargetDead = false;
        public bool IsTargetElite = false;
        public TargetReaction Reaction = TargetReaction.NoTarget;
        public ushort PlayerLevel = 0;
        public ushort TargetLevel = 0;
        public bool IsTargetEnemy = false;
        public bool IsTargetAlliance = false;
        public string TargetName = string.Empty;
        public ushort TargetComboPoints = 0;
        public ActionError PlayerActionError = ActionError.None;
        public bool PlayerIsAttacking = false;
        public ushort AmmoCount = 0;
        public bool IsTargetPlayer = false;
        public bool IsTargetHorde = false;
        public bool IsPlayerDead = false;
        public bool Start = false;
        public bool Found = false;
        public bool CanUseSkill = false;
        public bool IsWowForeground = false;
        public PlayerClass Class = PlayerClass.None;
        public bool CastingSucceeded = false;
        public bool CastingInterrupted = false;
        public double dt = 0;
        public double Time = 0;

        public override string ToString()
        {
            string output = string.Empty;

            output += "PlayerXPosition: " + PlayerXPosition.ToString("N3") + "\r\n";
            output += "PlayerYPosition: " + PlayerYPosition.ToString("N3") + "\r\n";
            output += "PlayerHeading: " + (PlayerHeading * (180 / Math.PI)).ToString("N3") + "\r\n";
            output += "IsInFarRange: " + IsInFarRange.ToString() + "\r\n";
            output += "IsInMediumRange: " + IsInMediumRange.ToString() + "\r\n";
            output += "IsInCloseRange: " + IsInCloseRange.ToString() + "\r\n";
            output += "PlayerHealth: " + PlayerHealth.ToString() + "\r\n";
            output += "MaxPlayerHealth: " + MaxPlayerHealth.ToString() + "\r\n";
            output += "PlayerHealthPercentage: " + PlayerHealthPercentage.ToString() + "\r\n";
            output += "PlayerMana: " + PlayerMana.ToString() + "\r\n";
            output += "MaxPlayerMana: " + MaxPlayerMana.ToString() + "\r\n";
            output += "PlayerHasTarget: " + PlayerHasTarget.ToString() + "\r\n";
            output += "TargetHealth: " + TargetHealth.ToString() + "\r\n";
            output += "TargetMana: " + TargetMana.ToString() + "\r\n";
            output += "SpellCanAttackTarget: " + SpellCanAttackTarget.ToString() + "\r\n";
            output += "CanAttackTarget: " + CanAttackTarget.ToString() + "\r\n";
            output += "PlayerInCombat: " + PlayerInCombat.ToString() + "\r\n";
            output += "TargetInCombat: " + TargetInCombat.ToString() + "\r\n";
            output += "IsTargetDead: " + IsTargetDead.ToString() + "\r\n";
            output += "IsTargetElite: " + IsTargetElite.ToString() + "\r\n";
            output += "Reaction: " + Reaction.ToString() + "\r\n";
            output += "PlayerLevel: " + PlayerLevel.ToString() + "\r\n";
            output += "TargetLevel: " + TargetLevel.ToString() + "\r\n";
            output += "IsTargetEnemy: " + IsTargetEnemy.ToString() + "\r\n";
            output += "IsTargetAlliance: " + IsTargetAlliance.ToString() + "\r\n";
            output += "TargetName: " + TargetName.ToString() + "\r\n";
            output += "TargetComboPoints: " + TargetComboPoints.ToString() + "\r\n";
            output += "PlayerActionError: " + PlayerActionError.ToString() + "\r\n";
            output += "PlayerIsAttacking: " + PlayerIsAttacking.ToString() + "\r\n";
            output += "AmmoCount: " + AmmoCount.ToString() + "\r\n";
            output += "IsTargetPlayer: " + IsTargetPlayer.ToString() + "\r\n";
            output += "IsTargetHorde: " + IsTargetHorde.ToString() + "\r\n";
            output += "IsPlayerDead: " + IsPlayerDead.ToString() + "\r\n";
            output += "Start: " + Start.ToString() + "\r\n";
            output += "Found: " + Found.ToString() + "\r\n";
            output += "CanUseSkill: " + CanUseSkill.ToString() + "\r\n";
            output += "IsWowForeground: " + IsWowForeground.ToString() + "\r\n";
            output += "Class: " + Class.ToString() + "\r\n";
            output += "Casting Succeeded: " + CastingSucceeded.ToString() + "\r\n";
            output += "Casting Interrupted: " + CastingInterrupted.ToString() + "\r\n";
            output += "dt: " + dt.ToString() + "\r\n";
            output += "Time: " + Time.ToString() + "\r\n";

            return output;
        }
    }

    public class WowApiUpdateEventArguments : EventArgs
    {
        public PlayerData PlayerData = new PlayerData();

        public WowApiUpdateEventArguments(PlayerData playerData)
        {
            PlayerData = playerData;
        }
    }

    public class FindWowApiEventArguments : EventArgs
    {
        public bool Found = false;
        public int X = 0;
        public int Y = 0;

        public FindWowApiEventArguments(bool found, int x, int y)
        {
            Found = found;
            X = x;
            Y = y;
        }
    }

    public static class WowApi
    {
        private const int API_WIDTH_PIXELS = 7;
        private const int API_HEIGHT_PIXELS = 55;
        private const int SEARCH_SPACE_PIXELS = 250;

        private const byte FIND1_PIXEL_R = 50;
        private const byte FIND1_PIXEL_G = 100;
        private const byte FIND1_PIXEL_B = 150;
        private const byte FIND1_PIXEL_X = 0;
        private const byte FIND1_PIXEL_Y = 0;

        private const byte FIND2_PIXEL_R = 150;
        private const byte FIND2_PIXEL_G = 50;
        private const byte FIND2_PIXEL_B = 100;
        private const byte FIND2_PIXEL_X = 3;
        private const byte FIND2_PIXEL_Y = 42;

        private const byte PIXEL_SET = 255;

        private static int m_ApiStartXLocation = 0;
        private static int m_ApiStartYLocation = 0;
        private static int m_ApiStopXLocation = 0;
        private static int m_ApiStopYLocation = 0;
        private static double m_ApiXScale = 2;
        private static double m_ApiYScale = 2;

        private static EventWaitHandle m_FindApiEventWait = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static volatile bool m_FindingApi = false;

        public delegate void UpdateEventHandler(object sender, WowApiUpdateEventArguments wea);
        public static event UpdateEventHandler UpdateEvent;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static void Run()
        {
            Rectangle bounds;
            PlayerData playerData = new PlayerData();

            long lastTicks = DateTime.Now.Ticks;
            DateTime startTime = DateTime.Now;

            while (true)
            {
                UpdateEvent?.Invoke(null, new WowApiUpdateEventArguments(playerData));

                if (m_FindingApi)
                {
                    m_FindApiEventWait.WaitOne();
                }

                bounds = new Rectangle(m_ApiStartXLocation, m_ApiStartYLocation, (int)Math.Round((API_WIDTH_PIXELS * m_ApiXScale)), (int)Math.Round((API_HEIGHT_PIXELS * m_ApiYScale)));

                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(new Point(m_ApiStartXLocation, m_ApiStartYLocation), Point.Empty, bounds.Size);
                    }

                    playerData.Time = (DateTime.Now - startTime).TotalSeconds;

                    //bitmap.Save("wowapi.png");

                    Color find1Pixel = bitmap.GetPixel((int)Math.Round((FIND1_PIXEL_X * m_ApiXScale)), (int)Math.Round((FIND1_PIXEL_Y * m_ApiYScale)));
                    Color find2Pixel = bitmap.GetPixel((int)Math.Round((FIND2_PIXEL_X * m_ApiXScale)), (int)Math.Round((FIND2_PIXEL_Y * m_ApiYScale)));

                    bool found1 = find1Pixel.R == FIND1_PIXEL_R && find1Pixel.G == FIND1_PIXEL_G && find1Pixel.B == FIND1_PIXEL_B;
                    bool found2 = find2Pixel.R == FIND2_PIXEL_R && find2Pixel.G == FIND2_PIXEL_G && find2Pixel.B == FIND2_PIXEL_B;

                    playerData.Found = found1 && found2;

                    playerData.IsWowForeground = GetForegroundProcessName().ToLower() == "wow";

                    if (playerData.IsWowForeground && !playerData.Found)
                    {
                        FindApi();
                        continue;
                    }

                    long currentTicks = DateTime.Now.Ticks;
                    playerData.dt = (double)(currentTicks - lastTicks) / TimeSpan.TicksPerSecond;

                    lastTicks = currentTicks;

                    Color playerHeadingPixelR = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((3 * m_ApiYScale)));
                    Color playerHeadingPixelG = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((6 * m_ApiYScale)));
                    Color playerHeadingPixelB = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((9 * m_ApiYScale)));
                    playerData.PlayerHeading = GetThreeByte3PixelValue(playerHeadingPixelR, playerHeadingPixelG, playerHeadingPixelB) * 2 * Math.PI;

                    Color isInFarRangePixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((12 * m_ApiYScale)));
                    playerData.IsInFarRange = isInFarRangePixel.R == PIXEL_SET ? true : false;

                    Color isInMediumRangePixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((15 * m_ApiYScale)));
                    playerData.IsInMediumRange = isInMediumRangePixel.R == PIXEL_SET ? true : false;

                    Color isInCloseRangePixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((18 * m_ApiYScale)));
                    playerData.IsInCloseRange = isInCloseRangePixel.R == PIXEL_SET ? true : false;

                    Color maxPlayerHealthPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((21 * m_ApiYScale)));
                    playerData.MaxPlayerHealth = GetTwoBytePixelValue(maxPlayerHealthPixel);

                    Color playerHealthPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((24 * m_ApiYScale)));
                    playerData.PlayerHealth = GetTwoBytePixelValue(playerHealthPixel);

                    playerData.PlayerHealthPercentage = ((double)playerData.PlayerHealth / playerData.MaxPlayerHealth)*100;

                    Color maxPlayerManaPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((27 * m_ApiYScale)));
                    playerData.MaxPlayerMana = GetTwoBytePixelValue(maxPlayerManaPixel);

                    Color playerManaPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((30 * m_ApiYScale)));
                    playerData.PlayerMana = GetTwoBytePixelValue(playerManaPixel);

                    Color hasTargetPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((33 * m_ApiYScale)));
                    playerData.PlayerHasTarget = hasTargetPixel.R == PIXEL_SET ? true : false;

                    Color targetHealthPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((36 * m_ApiYScale)));
                    playerData.TargetHealth = GetTwoBytePixelValue(targetHealthPixel);

                    Color targetManaPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((39 * m_ApiYScale)));
                    playerData.TargetMana = GetTwoBytePixelValue(targetManaPixel);

                    Color inCombatPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((42 * m_ApiYScale)));
                    playerData.PlayerInCombat = inCombatPixel.R == PIXEL_SET ? true : false;

                    Color classPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((45 * m_ApiYScale)));
                    playerData.Class = (PlayerClass)GetTwoBytePixelValue(classPixel);

                    Color castingSucceededPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((54 * m_ApiYScale)));
                    playerData.CastingSucceeded = castingSucceededPixel.R == PIXEL_SET ? true : false;

                    Color spellCanAttackTargetPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((0 * m_ApiYScale)));
                    playerData.SpellCanAttackTarget = spellCanAttackTargetPixel.R == PIXEL_SET ? true : false;

                    Color canAttackTargetPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((3 * m_ApiYScale)));
                    playerData.CanAttackTarget = canAttackTargetPixel.R == PIXEL_SET ? true : false;

                    Color isDeadPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((6 * m_ApiYScale)));
                    playerData.IsTargetDead = isDeadPixel.R == PIXEL_SET ? true : false;

                    Color isElitePixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((9 * m_ApiYScale)));
                    playerData.IsTargetElite = isElitePixel.R == PIXEL_SET ? true : false;

                    Color reactionPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((12 * m_ApiYScale)));
                    playerData.Reaction = (TargetReaction)GetTwoBytePixelValue(reactionPixel);

                    Color targetLevelPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((15 * m_ApiYScale)));
                    playerData.TargetLevel = GetTwoBytePixelValue(targetLevelPixel);

                    Color playerLevelPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((18 * m_ApiYScale)));
                    playerData.PlayerLevel = GetTwoBytePixelValue(playerLevelPixel);

                    Color isEnemyPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((21 * m_ApiYScale)));
                    playerData.IsTargetEnemy = isEnemyPixel.R == PIXEL_SET ? true : false;

                    Color isAlliancePixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((24 * m_ApiYScale)));
                    playerData.IsTargetAlliance = isAlliancePixel.R == PIXEL_SET ? true : false;

                    playerData.TargetName = string.Empty;

                    List<Color> targetNamePixels = new List<Color>();
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((27 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((30 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((33 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((36 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((39 * m_ApiYScale))));

                    Color castingInterruptedPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((54 * m_ApiYScale)));
                    playerData.CastingInterrupted = castingInterruptedPixel.R == PIXEL_SET ? true : false;

                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((0 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((3 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((6 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((9 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((12 * m_ApiYScale))));

                    for (int i = 0; i < targetNamePixels.Count; i++)
                    {
                        if (targetNamePixels[i].R == 0)
                            break;
                        else
                            playerData.TargetName += (char)targetNamePixels[i].R;

                        if (targetNamePixels[i].G == 0)
                            break;
                        else
                            playerData.TargetName += (char)targetNamePixels[i].G;

                        if (targetNamePixels[i].B == 0)
                            break;
                        else
                            playerData.TargetName += (char)targetNamePixels[i].B;
                    }

                    Color comboPointsPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((15 * m_ApiYScale)));
                    playerData.TargetComboPoints = GetTwoBytePixelValue(comboPointsPixel);

                    Color errorPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((18 * m_ApiYScale)));
                    playerData.PlayerActionError = (ActionError)GetTwoBytePixelValue(errorPixel);

                    Color attackingPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((21 * m_ApiYScale)));
                    playerData.PlayerIsAttacking = attackingPixel.R == PIXEL_SET ? true : false;

                    Color ammoPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((24 * m_ApiYScale)));
                    playerData.AmmoCount = GetTwoBytePixelValue(ammoPixel);

                    Color targetIsPlayerPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((27 * m_ApiYScale)));
                    playerData.IsTargetPlayer = targetIsPlayerPixel.R == PIXEL_SET ? true : false;

                    Color targetCombatPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((30 * m_ApiYScale)));
                    playerData.TargetInCombat = targetCombatPixel.R == PIXEL_SET ? true : false;

                    Color hordePixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((33 * m_ApiYScale)));
                    playerData.IsTargetHorde = hordePixel.R == PIXEL_SET ? true : false;

                    Color deadPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((36 * m_ApiYScale)));
                    playerData.IsPlayerDead = deadPixel.R == PIXEL_SET ? true : false;

                    Color startPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((39 * m_ApiYScale)));

                    playerData.Start = (startPixel.R == 1 && startPixel.G == PIXEL_SET && startPixel.B == 2) ? true : false;

                    Color canUseSkillPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((42 * m_ApiYScale)));
                    playerData.CanUseSkill = canUseSkillPixel.R == PIXEL_SET ? true : false;

                    Color xPositionR = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((48 * m_ApiYScale)));
                    Color xPositionG = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((48 * m_ApiYScale)));
                    Color xPositionB = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((48 * m_ApiYScale)));

                    playerData.PlayerXPosition = GetThreeByte3PixelValue(xPositionR, xPositionG, xPositionB) * 100;

                    Color yPositionR = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((51 * m_ApiYScale)));
                    Color yPositionG = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((51 * m_ApiYScale)));
                    Color yPositionB = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((51 * m_ApiYScale)));

                    playerData.PlayerYPosition = GetThreeByte3PixelValue(yPositionR, yPositionG, yPositionB) * 100;
                }

            }
        }

        private static void FindApi()
        {
            m_FindingApi = true;

            Task.Run(() =>
            {
                FindApiPixels();
            });
        }

        private static void FindApiPixels()
        {
            Rectangle pixelBounds = new Rectangle(0, 0, SEARCH_SPACE_PIXELS, SEARCH_SPACE_PIXELS);

            bool foundPixel1 = false;
            bool foundPixel2 = false;

            List<int> foundPixel1XList = new List<int>();
            List<int> foundPixel1YList = new List<int>();

            List<int> foundPixel2XList = new List<int>();
            List<int> foundPixel2YList = new List<int>();

            using (Bitmap bitmap = new Bitmap(pixelBounds.Width, pixelBounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(0, 0), Point.Empty, pixelBounds.Size);
                }

                for (int y = 0; y < pixelBounds.Height; y++)
                {
                    for (int x = 0; x < pixelBounds.Width; x++)
                    {
                        Color findPixel = bitmap.GetPixel(x, y);

                        if (findPixel.R == FIND1_PIXEL_R && findPixel.G == FIND1_PIXEL_G && findPixel.B == FIND1_PIXEL_B)
                        {
                            foundPixel1XList.Add(x);
                            foundPixel1YList.Add(y);
                            foundPixel1 = true;
                        }

                        if (findPixel.R == FIND2_PIXEL_R && findPixel.G == FIND2_PIXEL_G && findPixel.B == FIND2_PIXEL_B)
                        {
                            foundPixel2XList.Add(x);
                            foundPixel2YList.Add(y);
                            foundPixel2 = true;
                        }
                    }
                }

                if (foundPixel1 && foundPixel2)
                {
                    m_ApiStartXLocation = (int)Math.Round(ExpectedValue(foundPixel1XList));
                    m_ApiStartYLocation = (int)Math.Round(ExpectedValue(foundPixel1YList));

                    m_ApiStopXLocation = (int)Math.Round(ExpectedValue(foundPixel2XList));
                    m_ApiStopYLocation = (int)Math.Round(ExpectedValue(foundPixel2YList));

                    double xdiff = m_ApiStopXLocation - m_ApiStartXLocation;
                    double ydiff = m_ApiStopYLocation - m_ApiStartYLocation;

                    m_ApiXScale = xdiff / (FIND2_PIXEL_X - FIND1_PIXEL_X);
                    m_ApiYScale = ydiff / (FIND2_PIXEL_Y - FIND1_PIXEL_Y);
                }

                m_FindingApi = false;
                m_FindApiEventWait.Set();
                return;
            }
        }

        private static string GetForegroundProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();

            // The foreground window can be NULL in certain circumstances, 
            // such as when a window is losing activation.
            if (hwnd == null)
                return "Unknown";

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id == pid)
                    return p.ProcessName;
            }

            return "Unknown";
        }

        private static ushort GetTwoBytePixelValue(Color pixel)
        {
            byte[] pixelBytes = new byte[2];

            pixelBytes[0] = pixel.R;
            pixelBytes[1] = pixel.G;

            return BitConverter.ToUInt16(pixelBytes, 0);
        }

        private static double GetThreeByte3PixelValue(Color pixelR, Color pixelG, Color pixelB)
        {
            byte[] pixelBytes = new byte[4];

            pixelBytes[0] = pixelR.R;
            pixelBytes[1] = pixelG.G;
            pixelBytes[2] = pixelB.B;
            pixelBytes[3] = 0;

            uint pixelInt = BitConverter.ToUInt32(pixelBytes, 0);

            return pixelInt / 16777215.0;
        }

        private static double ExpectedValue(List<int> numberList)
        {
            double average = 0;

            for (int i = 0; i < numberList.Count; i++)
                average += numberList[i];

            average /= numberList.Count;

            return average;
        }

    }
}
