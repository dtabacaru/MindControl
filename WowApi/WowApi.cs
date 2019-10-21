using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClassicWowNeuralParasite
{
    public enum TargetReactionType
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

    public enum TargetFactionType
    {
        None = 0,
        Alliance = 1,
        Horde = 2
    }
    public enum PlayerClassType
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

    public enum ActionErrorType
    {
        None = 0,
        BehindTarget = 1,
        FacingWrongWay = 2,
        Moving = 3
    }

    public enum BuffType
    {
        None = 0,
        SealOfTheCrusader = 1,
        MarkOfTheWild = 2,
        Thorns = 4,
        SealOfCommand = 8,
        BlessingOfWisdom = 16,
        Stealth = 32,
        SealOfJustice = 64
    }

    public class PlayerData
    {
        private double PlayerXPositionContainer = 0;
        private object PlayerXPositionContainerLock = new object();
        private double PlayerYPositionContainer = 0;
        private object PlayerYPositionContainerLock = new object();
        private double PlayerHeadingContainer = 0;
        private object PlayerHeadingContainerLock = new object();
        private double PlayerHealthPercentageContainer = 0;
        private object PlayerHealthPercentageContainerLock = new object();

        public double PlayerXPosition
        {
            get { lock (PlayerXPositionContainerLock) { return PlayerXPositionContainer; } }
            set { lock (PlayerXPositionContainerLock) { PlayerXPositionContainer = value; } }
        }

        public double PlayerYPosition
        {
            get { lock (PlayerYPositionContainerLock) { return PlayerYPositionContainer; } }
            set { lock (PlayerYPositionContainerLock) { PlayerYPositionContainer = value; } }
        }

        public double PlayerHeading
        {
            get { lock (PlayerHeadingContainerLock) { return PlayerHeadingContainer; } }
            set { lock (PlayerHeadingContainerLock) { PlayerHeadingContainer = value; } }
        }

        public volatile bool IsInFarRange = false;
        public volatile bool IsInMediumRange = false;
        public volatile bool IsInCloseRange = false;
        public volatile uint PlayerHealth = 0;
        public volatile uint MaxPlayerHealth = 0;

        public double PlayerHealthPercentage
        {
            get { lock (PlayerHealthPercentageContainerLock) { return PlayerHealthPercentageContainer; } }
            set { lock (PlayerHealthPercentageContainerLock) { PlayerHealthPercentageContainer = value; } }
        }

        public volatile uint PlayerMana = 0;
        public volatile uint MaxPlayerMana = 0;
        public volatile bool PlayerHasTarget = false;
        public volatile uint TargetHealth = 0;
        public volatile uint TargetMana = 0;
        public volatile bool SpellCanAttackTarget = false;
        public volatile bool CanAttackTarget = false;
        public volatile bool PlayerInCombat = false;
        public volatile bool TargetInCombat = false;
        public volatile bool IsTargetDead = false;
        public volatile bool IsTargetElite = false;
        public volatile TargetReactionType Reaction = TargetReactionType.NoTarget;
        public volatile uint PlayerLevel = 0;
        public volatile uint TargetLevel = 0;
        public volatile bool IsTargetEnemy = false;
        public volatile string TargetName = string.Empty;
        public volatile uint TargetComboPoints = 0;
        public volatile ActionErrorType PlayerActionError = ActionErrorType.None;
        public volatile bool PlayerIsAttacking = false;
        public volatile uint AmmoCount = 0;
        public volatile bool IsTargetPlayer = false;
        public volatile bool IsPlayerDead = false;
        public volatile bool Start = false;
        public volatile bool Found = false;
        public volatile bool CanUseSkill = false;
        public volatile bool IsWowForeground = false;
        public volatile PlayerClassType Class = PlayerClassType.None;
        public volatile bool Casting = false;
        public volatile uint Shape = 0;
        public volatile uint Buffs = 0;
        public volatile bool MouseOverTarget = false;
        public volatile TargetFactionType TargetFaction = TargetFactionType.None;
        public volatile bool TargetTargetingPlayer = false;
        public volatile bool Whisper = false;
        public volatile uint FreeBagSlots = 0;

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
            output += "PlayerHealthPercentage: " + PlayerHealthPercentage.ToString("N3") + "\r\n";
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
            output += "TargetName: " + TargetName.ToString() + "\r\n";
            output += "TargetComboPoints: " + TargetComboPoints.ToString() + "\r\n";
            output += "PlayerActionError: " + PlayerActionError.ToString() + "\r\n";
            output += "PlayerIsAttacking: " + PlayerIsAttacking.ToString() + "\r\n";
            output += "AmmoCount: " + AmmoCount.ToString() + "\r\n";
            output += "IsTargetPlayer: " + IsTargetPlayer.ToString() + "\r\n";
            output += "Target Faction: " + TargetFaction.ToString() + "\r\n";
            output += "IsPlayerDead: " + IsPlayerDead.ToString() + "\r\n";
            output += "Start: " + Start.ToString() + "\r\n";
            output += "Found: " + Found.ToString() + "\r\n";
            output += "CanUseSkill: " + CanUseSkill.ToString() + "\r\n";
            output += "IsWowForeground: " + IsWowForeground.ToString() + "\r\n";
            output += "Class: " + Class.ToString() + "\r\n";
            output += "Casting: " + Casting.ToString() + "\r\n";
            output += "Shape: " + Shape.ToString() + "\r\n";
            output += "Buffs: " + Buffs.ToString() + "\r\n";
            output += "MouseOverTarget: " + MouseOverTarget.ToString() + "\r\n";
            output += "Whisper: " + Whisper.ToString() + "\r\n";
            output += "FreeBagSlots: " + FreeBagSlots.ToString() + "\r\n";

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
        private const int API_HEIGHT_PIXELS = 58;
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

        public delegate void UpdateEventHandler(object sender, EventArgs ea);
        public static event UpdateEventHandler UpdateEvent;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static volatile PlayerData PlayerData = new PlayerData();

        static WowApi()
        {
            Task.Run(() =>
            {
                Run();
            });
        }

        public static EventWaitHandle Sync = new EventWaitHandle(false, EventResetMode.AutoReset);

        public static void Run()
        {
            Rectangle bounds;
            PlayerData currentPlayerData = new PlayerData();

            while (true)
            {
                PlayerData = currentPlayerData;
                UpdateEvent?.Invoke(null, EventArgs.Empty);
                Sync.Set();

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

                    //bitmap.Save("wowapi.png");

                    Color find1Pixel = bitmap.GetPixel((int)Math.Round((FIND1_PIXEL_X * m_ApiXScale)), (int)Math.Round((FIND1_PIXEL_Y * m_ApiYScale)));
                    Color find2Pixel = bitmap.GetPixel((int)Math.Round((FIND2_PIXEL_X * m_ApiXScale)), (int)Math.Round((FIND2_PIXEL_Y * m_ApiYScale)));

                    bool found1 = find1Pixel.R == FIND1_PIXEL_R && find1Pixel.G == FIND1_PIXEL_G && find1Pixel.B == FIND1_PIXEL_B;
                    bool found2 = find2Pixel.R == FIND2_PIXEL_R && find2Pixel.G == FIND2_PIXEL_G && find2Pixel.B == FIND2_PIXEL_B;

                    currentPlayerData.Found = found1 && found2;

                    currentPlayerData.IsWowForeground = GetForegroundProcessName().ToLower() == "wow";

                    if (currentPlayerData.IsWowForeground && !currentPlayerData.Found)
                    {
                        FindApi();
                        continue;
                    }

                    Color playerHeadingPixelR = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((3 * m_ApiYScale)));
                    Color playerHeadingPixelG = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((6 * m_ApiYScale)));
                    Color playerHeadingPixelB = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((9 * m_ApiYScale)));
                    currentPlayerData.PlayerHeading = GetThreeByteFloatThreePixelValue(playerHeadingPixelR, playerHeadingPixelG, playerHeadingPixelB) * 2 * Math.PI;

                    Color isInFarRangePixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((12 * m_ApiYScale)));
                    currentPlayerData.IsInFarRange = isInFarRangePixel.R == PIXEL_SET ? true : false;

                    Color isInMediumRangePixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((15 * m_ApiYScale)));
                    currentPlayerData.IsInMediumRange = isInMediumRangePixel.R == PIXEL_SET ? true : false;

                    Color isInCloseRangePixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((18 * m_ApiYScale)));
                    currentPlayerData.IsInCloseRange = isInCloseRangePixel.R == PIXEL_SET ? true : false;

                    Color maxPlayerHealthPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((21 * m_ApiYScale)));
                    currentPlayerData.MaxPlayerHealth = GetThreeByteIntPixelValue(maxPlayerHealthPixel);

                    Color playerHealthPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((24 * m_ApiYScale)));
                    currentPlayerData.PlayerHealth = GetThreeByteIntPixelValue(playerHealthPixel);

                    currentPlayerData.PlayerHealthPercentage = ((double)currentPlayerData.PlayerHealth / currentPlayerData.MaxPlayerHealth)*100;

                    Color maxPlayerManaPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((27 * m_ApiYScale)));
                    currentPlayerData.MaxPlayerMana = GetThreeByteIntPixelValue(maxPlayerManaPixel);

                    Color playerManaPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((30 * m_ApiYScale)));
                    currentPlayerData.PlayerMana = GetThreeByteIntPixelValue(playerManaPixel);

                    Color hasTargetPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((33 * m_ApiYScale)));
                    currentPlayerData.PlayerHasTarget = hasTargetPixel.R == PIXEL_SET ? true : false;

                    Color targetHealthPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((36 * m_ApiYScale)));
                    currentPlayerData.TargetHealth = GetThreeByteIntPixelValue(targetHealthPixel);

                    Color targetManaPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((39 * m_ApiYScale)));
                    currentPlayerData.TargetMana = GetThreeByteIntPixelValue(targetManaPixel);

                    Color inCombatPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((42 * m_ApiYScale)));
                    currentPlayerData.PlayerInCombat = inCombatPixel.R == PIXEL_SET ? true : false;

                    Color classPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((45 * m_ApiYScale)));
                    currentPlayerData.Class = (PlayerClassType)GetThreeByteIntPixelValue(classPixel);

                    Color castingPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((54 * m_ApiYScale)));
                    currentPlayerData.Casting = castingPixel.R == PIXEL_SET ? true : false;

                    Color whisperPixel = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((57 * m_ApiYScale)));
                    currentPlayerData.Whisper = whisperPixel.R == PIXEL_SET ? true : false;

                    Color spellCanAttackTargetPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((0 * m_ApiYScale)));
                    currentPlayerData.SpellCanAttackTarget = spellCanAttackTargetPixel.R == PIXEL_SET ? true : false;

                    Color canAttackTargetPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((3 * m_ApiYScale)));
                    currentPlayerData.CanAttackTarget = canAttackTargetPixel.R == PIXEL_SET ? true : false;

                    Color isDeadPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((6 * m_ApiYScale)));
                    currentPlayerData.IsTargetDead = isDeadPixel.R == PIXEL_SET ? true : false;

                    Color isElitePixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((9 * m_ApiYScale)));
                    currentPlayerData.IsTargetElite = isElitePixel.R == PIXEL_SET ? true : false;

                    Color reactionPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((12 * m_ApiYScale)));
                    currentPlayerData.Reaction = (TargetReactionType)GetThreeByteIntPixelValue(reactionPixel);

                    Color targetLevelPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((15 * m_ApiYScale)));
                    currentPlayerData.TargetLevel = GetThreeByteIntPixelValue(targetLevelPixel);

                    Color playerLevelPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((18 * m_ApiYScale)));
                    currentPlayerData.PlayerLevel = GetThreeByteIntPixelValue(playerLevelPixel);

                    Color isEnemyPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((21 * m_ApiYScale)));
                    currentPlayerData.IsTargetEnemy = isEnemyPixel.R == PIXEL_SET ? true : false;

                    Color targetTargetingPlayerPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((24 * m_ApiYScale)));
                    currentPlayerData.TargetTargetingPlayer = targetTargetingPlayerPixel.R == PIXEL_SET ? true : false;

                    currentPlayerData.TargetName = string.Empty;

                    List<Color> targetNamePixels = new List<Color>();
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((27 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((30 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((33 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((36 * m_ApiYScale))));
                    targetNamePixels.Add(bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((39 * m_ApiYScale))));

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
                            currentPlayerData.TargetName += (char)targetNamePixels[i].R;

                        if (targetNamePixels[i].G == 0)
                            break;
                        else
                            currentPlayerData.TargetName += (char)targetNamePixels[i].G;

                        if (targetNamePixels[i].B == 0)
                            break;
                        else
                            currentPlayerData.TargetName += (char)targetNamePixels[i].B;
                    }

                    Color buffsPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((45 * m_ApiYScale)));
                    currentPlayerData.Buffs = GetThreeByteIntPixelValue(buffsPixel);

                    Color mouseOverPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((54 * m_ApiYScale)));
                    currentPlayerData.MouseOverTarget = mouseOverPixel.R == PIXEL_SET ? true : false;

                    Color freeBagSlotsPixel = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((57 * m_ApiYScale)));
                    currentPlayerData.FreeBagSlots = GetThreeByteIntPixelValue(freeBagSlotsPixel);

                    Color comboPointsPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((15 * m_ApiYScale)));
                    currentPlayerData.TargetComboPoints = GetThreeByteIntPixelValue(comboPointsPixel);

                    Color errorPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((18 * m_ApiYScale)));
                    currentPlayerData.PlayerActionError = (ActionErrorType)GetThreeByteIntPixelValue(errorPixel);

                    Color attackingPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((21 * m_ApiYScale)));
                    currentPlayerData.PlayerIsAttacking = attackingPixel.R == PIXEL_SET ? true : false;

                    Color ammoPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((24 * m_ApiYScale)));
                    currentPlayerData.AmmoCount = GetThreeByteIntPixelValue(ammoPixel);

                    Color targetIsPlayerPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((27 * m_ApiYScale)));
                    currentPlayerData.IsTargetPlayer = targetIsPlayerPixel.R == PIXEL_SET ? true : false;

                    Color targetCombatPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((30 * m_ApiYScale)));
                    currentPlayerData.TargetInCombat = targetCombatPixel.R == PIXEL_SET ? true : false;

                    Color targetFactionPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((33 * m_ApiYScale)));
                    currentPlayerData.TargetFaction = (TargetFactionType)GetThreeByteIntPixelValue(targetFactionPixel);

                    Color deadPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((36 * m_ApiYScale)));
                    currentPlayerData.IsPlayerDead = deadPixel.R == PIXEL_SET ? true : false;

                    Color startPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((39 * m_ApiYScale)));

                    currentPlayerData.Start = (startPixel.R == 1 && startPixel.G == PIXEL_SET && startPixel.B == 2) ? true : false;

                    Color canUseSkillPixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((42 * m_ApiYScale)));
                    currentPlayerData.CanUseSkill = canUseSkillPixel.R == PIXEL_SET ? true : false;

                    Color xPositionR = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((48 * m_ApiYScale)));
                    Color xPositionG = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((48 * m_ApiYScale)));
                    Color xPositionB = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((48 * m_ApiYScale)));

                    currentPlayerData.PlayerXPosition = GetThreeByteFloatThreePixelValue(xPositionR, xPositionG, xPositionB) * 100;

                    Color yPositionR = bitmap.GetPixel((int)Math.Round((0 * m_ApiXScale)), (int)Math.Round((51 * m_ApiYScale)));
                    Color yPositionG = bitmap.GetPixel((int)Math.Round((3 * m_ApiXScale)), (int)Math.Round((51 * m_ApiYScale)));
                    Color yPositionB = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((51 * m_ApiYScale)));

                    currentPlayerData.PlayerYPosition = GetThreeByteFloatThreePixelValue(yPositionR, yPositionG, yPositionB) * 100;

                    Color shapePixel = bitmap.GetPixel((int)Math.Round((6 * m_ApiXScale)), (int)Math.Round((54 * m_ApiYScale)));
                    currentPlayerData.Shape = GetThreeByteIntPixelValue(shapePixel);
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

        private static uint GetThreeByteIntPixelValue(Color pixel)
        {
            byte[] pixelBytes = new byte[4];

            pixelBytes[0] = pixel.R;
            pixelBytes[1] = pixel.G;
            pixelBytes[2] = pixel.B;
            pixelBytes[3] = 0;

            return BitConverter.ToUInt32(pixelBytes, 0);
        }

        private static double GetThreeByteFloatThreePixelValue(Color pixelR, Color pixelG, Color pixelB)
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
