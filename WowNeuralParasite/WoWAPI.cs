using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WowNeuralParasite
{

    public class WoWPlayerData
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Player X Position: "); sb.Append(PlayerXPosition); sb.Append("\n");
            sb.Append("Player Y Position: "); sb.Append(PlayerYPosition); sb.Append("\n");
            sb.Append("Player Heading: "); sb.Append(PlayerHeading); sb.Append("\n");
            sb.Append("In Far Range: "); sb.Append(IsInFarRange); sb.Append("\n");
            sb.Append("In Medium Range: "); sb.Append(IsInMediumRange); sb.Append("\n");
            sb.Append("In Close Range: "); sb.Append(IsInCloseRange); sb.Append("\n");
            sb.Append("Player Health: "); sb.Append(PlayerHealth); sb.Append("\n");
            sb.Append("Player Max Health: "); sb.Append(MaxPlayerHealth); sb.Append("\n");
            sb.Append("Player Mana: "); sb.Append(PlayerMana); sb.Append("\n");
            sb.Append("Player Max Mana: "); sb.Append(MaxPlayerMana); sb.Append("\n");
            sb.Append("Has Target: "); sb.Append(HasTarget); sb.Append("\n");
            sb.Append("Target Health: "); sb.Append(TargetHealth); sb.Append("\n");
            sb.Append("Target Mana: "); sb.Append(TargetMana); sb.Append("\n");
            sb.Append("Spell Can Attack Target: "); sb.Append(SpellCanAttackTarget); sb.Append("\n");
            sb.Append("Can Attack Target: "); sb.Append(CanAttackTarget); sb.Append("\n");
            sb.Append("Player In Combat: "); sb.Append(PlayerInCombat); sb.Append("\n");
            sb.Append("Target In Combat: "); sb.Append(TargetInCombat); sb.Append("\n");
            sb.Append("Is Target Dead: "); sb.Append(IsTargetDead); sb.Append("\n");
            sb.Append("Is Target Elite: "); sb.Append(IsTargetElite); sb.Append("\n");
            sb.Append("Target Reaction: "); sb.Append(Reaction); sb.Append("\n");
            sb.Append("Player Level: "); sb.Append(PlayerLevel); sb.Append("\n");
            sb.Append("Target Level: "); sb.Append(TargetLevel); sb.Append("\n");
            sb.Append("Target Is Enemy: "); sb.Append(IsTargetEnemy); sb.Append("\n");
            sb.Append("Target Is Civilian: "); sb.Append(IsTargetCivillian); sb.Append("\n");
            sb.Append("Target Name: "); sb.Append(TargetName); sb.Append("\n");

            return sb.ToString();
        }

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

        public double PlayerXPosition;
        public double PlayerYPosition;
        public double PlayerHeading;
        public bool IsInFarRange;
        public bool IsInMediumRange;
        public bool IsInCloseRange;
        public ushort PlayerHealth;
        public ushort MaxPlayerHealth;
        public ushort PlayerMana;
        public ushort MaxPlayerMana;
        public bool HasTarget;
        public ushort TargetHealth;
        public ushort TargetMana;
        public bool SpellCanAttackTarget;
        public bool CanAttackTarget;
        public bool PlayerInCombat;
        public bool TargetInCombat;
        public bool IsTargetDead;
        public bool IsTargetElite;
        public TargetReaction Reaction;
        public ushort PlayerLevel;
        public ushort TargetLevel;
        public bool IsTargetEnemy;
        public bool IsTargetCivillian;
        public string TargetName;
    }

    public class WoWUpdateEventArguments : EventArgs
    {
        public WoWPlayerData PlayerData = new WoWPlayerData();

        public WoWUpdateEventArguments(WoWPlayerData playerData)
        {
            PlayerData = playerData;
        }
    }

    public class WoWAPI
    {
        public delegate void UpdateEventHandler(object sender, WoWUpdateEventArguments wea);
        public event UpdateEventHandler UpdateEvent;

        private ushort GetTwoBytePixelValue(Color pixel)
        {
            byte[] pixelBytes = new byte[2];

            pixelBytes[0] = pixel.R;
            pixelBytes[1] = pixel.G;

            return BitConverter.ToUInt16(pixelBytes, 0);
        }

        private double GetThreeBytePixelValue(Color pixel)
        {
            byte[] pixelBytes = new byte[4];

            pixelBytes[0] = pixel.R;
            pixelBytes[1] = pixel.G;
            pixelBytes[2] = pixel.B;
            pixelBytes[3] = 0;

            uint pixelInt = BitConverter.ToUInt32(pixelBytes, 0);

            return pixelInt / 16777215.0;
        }

        public void Run()
        {
            int x = 13;
            int y = 42;
            Rectangle bounds = new Rectangle(x, y, 5, 35);
            WoWPlayerData playerData = new WoWPlayerData();

            while (true)
            {

                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(new Point(x, y), Point.Empty, bounds.Size);
                    }

                    Color xPositionPixel = bitmap.GetPixel(0, 0);
                    playerData.PlayerXPosition = GetThreeBytePixelValue(xPositionPixel)*100;

                    Color yPositionPixel = bitmap.GetPixel(0, 2);
                    playerData.PlayerYPosition = GetThreeBytePixelValue(yPositionPixel)*100;

                    Color playerHeadingPixel = bitmap.GetPixel(0, 4);
                    playerData.PlayerHeading = GetThreeBytePixelValue(playerHeadingPixel)*2*Math.PI;

                    Color isInFarRangePixel = bitmap.GetPixel(0, 6);
                    playerData.IsInFarRange = isInFarRangePixel.R == 255 ? true : false;

                    Color isInMediumRangePixel = bitmap.GetPixel(0, 8);
                    playerData.IsInMediumRange = isInMediumRangePixel.R == 255 ? true : false;

                    Color isInCloseRangePixel = bitmap.GetPixel(0, 10);
                    playerData.IsInCloseRange = isInCloseRangePixel.R == 255 ? true : false;

                    Color maxPlayerHealthPixel = bitmap.GetPixel(0, 13);
                    playerData.MaxPlayerHealth = GetTwoBytePixelValue(maxPlayerHealthPixel);

                    Color playerHealthPixel = bitmap.GetPixel(0, 15);
                    playerData.PlayerHealth = GetTwoBytePixelValue(playerHealthPixel);

                    Color maxPlayerManaPixel = bitmap.GetPixel(0, 17);
                    playerData.MaxPlayerMana = GetTwoBytePixelValue(maxPlayerManaPixel);

                    Color playerManaPixel = bitmap.GetPixel(0, 19);
                    playerData.PlayerMana = GetTwoBytePixelValue(playerManaPixel);

                    Color hasTargetPixel = bitmap.GetPixel(0, 22);
                    playerData.HasTarget = hasTargetPixel.R == 255 ? true : false;

                    Color targetHealthPixel = bitmap.GetPixel(0, 24);
                    playerData.TargetHealth = GetTwoBytePixelValue(targetHealthPixel);

                    Color targetManaPixel = bitmap.GetPixel(0, 26);
                    playerData.TargetMana = GetTwoBytePixelValue(targetManaPixel);

                    Color inCombatPixel = bitmap.GetPixel(0, 28);
                    playerData.PlayerInCombat = inCombatPixel.R == 255 ? true : false;

                    Color spellCanAttackTargetPixel = bitmap.GetPixel(2, 0);
                    playerData.SpellCanAttackTarget = spellCanAttackTargetPixel.R == 255 ? true : false;

                    Color canAttackTargetPixel = bitmap.GetPixel(2, 2);
                    playerData.CanAttackTarget = canAttackTargetPixel.R == 255 ? true : false;

                    Color isDeadPixel = bitmap.GetPixel(2, 4);
                    playerData.IsTargetDead = isDeadPixel.R == 255 ? true : false;

                    Color isElitePixel = bitmap.GetPixel(2, 6);
                    playerData.IsTargetElite = isElitePixel.R == 255 ? true : false;

                    Color reactionPixel = bitmap.GetPixel(2, 9);
                    playerData.Reaction = (WoWPlayerData.TargetReaction)GetTwoBytePixelValue(reactionPixel);

                    Color targetLevelPixel = bitmap.GetPixel(2, 11);
                    playerData.TargetLevel = GetTwoBytePixelValue(targetLevelPixel);

                    Color playerLevelPixel = bitmap.GetPixel(2, 13);
                    playerData.PlayerLevel = GetTwoBytePixelValue(playerLevelPixel);

                    Color isEnemyPixel = bitmap.GetPixel(2, 15);
                    playerData.IsTargetEnemy = isEnemyPixel.R == 255 ? true : false;

                    Color isCivilianPixel = bitmap.GetPixel(2, 17);
                    playerData.IsTargetCivillian = isCivilianPixel.R == 255 ? true : false;

                    playerData.TargetName = string.Empty;

                    List<Color> targetNamePixels = new List<Color>();
                    targetNamePixels.Add(bitmap.GetPixel(2, 19));
                    targetNamePixels.Add(bitmap.GetPixel(2, 22));
                    targetNamePixels.Add(bitmap.GetPixel(2, 24));
                    targetNamePixels.Add(bitmap.GetPixel(2, 26));
                    targetNamePixels.Add(bitmap.GetPixel(2, 28));
                    targetNamePixels.Add(bitmap.GetPixel(4, 0));
                    targetNamePixels.Add(bitmap.GetPixel(4, 2));
                    targetNamePixels.Add(bitmap.GetPixel(4, 4));
                    targetNamePixels.Add(bitmap.GetPixel(4, 6));
                    targetNamePixels.Add(bitmap.GetPixel(4, 9));

                    for(int i = 0; i < targetNamePixels.Count; i++)
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

                    UpdateEvent?.Invoke(this, new WoWUpdateEventArguments(playerData));
                }

            }
        }

    }
}
