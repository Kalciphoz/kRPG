using System;
using kRPG.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GUI
{
    /// <summary>
    ///     UI for the Anvil
    /// </summary>
    public class AnvilGUI : BaseGui
    {
        private bool GuardianCrown { get; set; }
        private Item Item { get; set; }
        private kItem Ki { get; set; }
        private bool PermanenceCrown { get; set; }

        /// <summary>
        ///     Player Character gui shown for
        /// </summary>
        private PlayerCharacter PlayerCharacter { get; set; }

        public Vector2 PlayerPosition { get; set; }

        private bool Selected { get; set; }
        private bool TranscendenceCrown { get; set; }

        private int UpgradeCost { get; set; }

        //in percent:
        private int UpgradeSuccess { get; set; }

        public AnvilGUI(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;

            AddButton(() => new Rectangle((int)BtnCancelPos.X, (int)BtnCancelPos.Y, (int)(GFX.BTN_WIDTH * Scale), (int)(GFX.BTN_HEIGHT * Scale)), delegate
            {
                Main.PlaySound(SoundID.MenuTick);
                CloseGui();
            });
            AddButton(() => new Rectangle((int)BtnExperiencePos.X, (int)BtnExperiencePos.Y, 48, 48), delegate { GuardianCrown = !GuardianCrown; });
            AddButton(() => new Rectangle((int)BtnPermanencePos.X, (int)BtnPermanencePos.Y, 48, 48), delegate
            {
                if (!PermanenceCrown && playerCharacter.Permanence > 0)
                    PermanenceCrown = true;
                else PermanenceCrown = false;
            });
            AddButton(() => new Rectangle((int)BtnTranscendencePos.X, (int)BtnTranscendencePos.Y, 48, 48), delegate
            {
                if (!TranscendenceCrown && playerCharacter.Transcendence > 0)
                    TranscendenceCrown = true;
                else TranscendenceCrown = false;
            });
        }

        private static Vector2 BtnCancelPos => new Vector2(Main.screenWidth / 2f - 92f * Scale, GuiPosition.Y + 268f * Scale);

        private static Vector2 BtnExperiencePos => new Vector2(GuiPosition.X + 150f * Scale, GuiPosition.Y - 76f * Scale);

        private static Vector2 BtnPermanencePos => new Vector2(GuiPosition.X + 150f * Scale, GuiPosition.Y - 12f * Scale);

        private static Vector2 BtnTranscendencePos => new Vector2(GuiPosition.X + 150f * Scale, GuiPosition.Y + 52f * Scale);

        private static Vector2 BtnUpgradePos => new Vector2(Main.screenWidth / 2f - 92f * Scale, GuiPosition.Y + 208f * Scale);

        private static Vector2 GuiPosition => new Vector2(Main.screenWidth / 2f - 128f * Scale, Main.screenHeight / 2f);

        private static float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.4f);

        public bool AttemptSelectItem(kItem tKi, Item tItem)
        {
            byte startLevel = tKi.UpgradeLevel;

            if (startLevel >= 7 && (startLevel >= 8 || !TranscendenceCrown))
                return false;

            Ki = tKi;
            Item = tItem;
            Selected = true;

            UpgradeSuccess = 90 - startLevel * 10;
            if (startLevel == 0)
                UpgradeCost = tItem.value / 20;
            else if (startLevel == 1)
                UpgradeCost = tItem.value / 15;
            else if (startLevel == 2)
                UpgradeCost = tItem.value / 10;
            else if (startLevel == 3)
                UpgradeCost = tItem.value / 8;
            else if (startLevel == 4)
                UpgradeCost = tItem.value / 5;
            else if (startLevel == 5)
                UpgradeCost = tItem.value / 3;
            else if (startLevel == 6)
                UpgradeCost = tItem.value / 2;
            else if (startLevel == 7)
                UpgradeCost = tItem.value;
            else if (startLevel == 8)
                UpgradeCost = (int)(tItem.value * 1.5);

            return true;
        }

        //public bool IsSelecting()
        //{
        //    return guiActive && !selected;
        //}

        public override void OnClose()
        {
            Reset();
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            if (PlayerCharacter.Permanence < 1)
                PermanenceCrown = false;
            if (PlayerCharacter.Transcendence < 1)
                TranscendenceCrown = false;

            spriteBatch.Draw(GFX.anvil, new Vector2(GuiPosition.X - 150f * Scale, GuiPosition.Y - 100f * Scale), Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrading an item increases its power, but does not always succeed",
                new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y - 148f * Scale), Color.White, Scale);
            if (PermanenceCrown)
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be downgraded.",
                    new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y - 124 * Scale), Color.Lime, Scale);
            else
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be destroyed.",
                    new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y - 124 * Scale), Color.Red, Scale);

            spriteBatch.Draw(GuardianCrown ? GFX.ButtonCrownPressed : GFX.ButtonCrown, BtnExperiencePos, Color.White, Scale);
            spriteBatch.Draw(GFX.GuardianCrown, BtnExperiencePos + new Vector2(9f, 10f) * Scale, GuardianCrown ? Color.Gray : Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases success chance of upgrades by 10%", BtnExperiencePos + new Vector2(64f, 4f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "at the cost of a much higher price in currency",
                BtnExperiencePos + new Vector2(64f, 28f) * Scale, Color.White, Scale);
            spriteBatch.Draw(PermanenceCrown ? GFX.ButtonCrownPressed : GFX.ButtonCrown, BtnPermanencePos, Color.White, Scale);
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<PermanenceCrown>()], BtnPermanencePos + new Vector2(9f, 10f) * Scale,
                PermanenceCrown ? Color.Gray : Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, PlayerCharacter.Permanence.ToString(), BtnPermanencePos + new Vector2(8f, 24f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "When an upgrade fails, items are downgraded", BtnPermanencePos + new Vector2(64f, 4f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText,
                "instead of being destroyed. " + (PlayerCharacter.Permanence == 1 ? "1 crown left." : PlayerCharacter.Permanence + " crowns left."),
                BtnPermanencePos + new Vector2(64f, 28f) * Scale, Color.White, Scale);
            spriteBatch.Draw(TranscendenceCrown ? GFX.ButtonCrownPressed : GFX.ButtonCrown, BtnTranscendencePos, Color.White, Scale);
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<BlacksmithCrown>()], BtnTranscendencePos + new Vector2(9f, 10f) * Scale,
                TranscendenceCrown ? Color.Gray : Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, PlayerCharacter.Transcendence.ToString(), BtnTranscendencePos + new Vector2(8f, 24f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Allows you to upgrade an item to +8", BtnTranscendencePos + new Vector2(64f, 4f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText,
                PlayerCharacter.Transcendence == 1 ? "1 crown left." : PlayerCharacter.Transcendence + " crowns left.",
                BtnTranscendencePos + new Vector2(64f, 28f) * Scale, Color.White, Scale);

            spriteBatch.Draw(GFX.ButtonClose, BtnCancelPos, Color.White, Scale);

            if (!Selected)
            {
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Right-click a weapon to select it for upgrading>",
                    new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y + 128f * Scale), Color.White, Scale);
            }

            else
            {
                if (Ki.UpgradeLevel >= PlayerCharacter.DefaultMaxUpgradeLevel && !TranscendenceCrown)
                {
                    Ki = null;
                    Item = null;
                    Selected = false;
                }

                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Selected item: " + (Item != null ? Item.HoverName : ""),
                    new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 128f * Scale), Color.White, Scale);

                int modifier = GuardianCrown ? 4 : 1;
                if (player.Wealth() >= UpgradeCost * modifier)
                {
                    int bonusChance = GuardianCrown ? 10 : 0;

                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(UpgradeCost * modifier),
                        new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 152f * Scale), Color.White, Scale);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Chance to succeed: " + (UpgradeSuccess + bonusChance) + "%",
                        new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 176f * Scale), bonusChance > 0 ? Color.Lime : Color.White, Scale);
                    spriteBatch.Draw(GFX.ButtonUpgrade, BtnUpgradePos, Color.White, Scale);
                    if (new Rectangle((int)BtnUpgradePos.X, (int)BtnUpgradePos.Y, (int)(GFX.BTN_WIDTH * Scale), (int)(GFX.BTN_HEIGHT * Scale)).Contains(
                            Main.mouseX, Main.mouseY) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        if (Ki == null)
                            throw new Exception("Sanity Check, Ki is null.");
                        if (Ki.UpgradeLevel >= PlayerCharacter.DefaultMaxUpgradeLevel)
                            PlayerCharacter.Transcendence -= 1;

                        if (Main.rand.Next(100) < UpgradeSuccess + bonusChance)
                        {
                            Ki.Upgrade(Item);
                        }
                        else
                        {
                            if (PermanenceCrown)
                                Ki.Downgrade(Item);
                            else
                                Ki.Destroy(Item);
                        }

                        if (PermanenceCrown && bonusChance + UpgradeSuccess < 100)
                            PlayerCharacter.Permanence -= 1;

                        player.RemoveCoins(UpgradeCost * modifier);
                        if (!AttemptSelectItem(Ki, Item)) CloseGui();
                    }
                }

                else
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(UpgradeCost * modifier),
                        new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 152f * Scale), Color.Red, Scale);
                }
            }

            //if (playerPosition != null) 
            if (Vector2.Distance(player.Center, PlayerPosition) > 128)
                CloseGui();
        }

        public void Reset()
        {
            Ki = null;
            Item = null;
            Selected = false;
        }
    }
}