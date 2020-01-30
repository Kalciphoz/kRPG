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
    /// UI for the Anvil
    /// </summary>
    public class AnvilGUI : BaseGui
    {
        /// <summary>
        /// Player Character gui shown for
        /// </summary>
        private readonly PlayerCharacter playerCharacter;
        private kItem ki;
        private Item item;
        public Vector2 playerPosition;

        private bool selected;

        private static Vector2 GuiPosition => new Vector2(Main.screenWidth / 2f - 128f * Scale, Main.screenHeight / 2f);

        private static float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.4f);

        private static Vector2 BtnCancelPos => new Vector2(Main.screenWidth / 2f - 92f * Scale, GuiPosition.Y + 268f * Scale);

        private static Vector2 BtnUpgradePos => new Vector2(Main.screenWidth / 2f - 92f * Scale, GuiPosition.Y + 208f * Scale);

        private static Vector2 BtnExperiencePos => new Vector2(GuiPosition.X + 150f * Scale, GuiPosition.Y - 76f * Scale);

        private static Vector2 BtnPermanencePos => new Vector2(GuiPosition.X + 150f * Scale, GuiPosition.Y - 12f * Scale);

        private static Vector2 BtnTranscendencePos => new Vector2(GuiPosition.X + 150f * Scale, GuiPosition.Y + 52f * Scale);

        private int upgradeCost;

        //in percent:
        private int upgradeSuccess;

        private bool guardianCrown;
        private bool permanenceCrown;
        private bool transcendenceCrown;

        public AnvilGUI(PlayerCharacter playerCharacter)
        {
            this.playerCharacter = playerCharacter;

            AddButton(() => new Rectangle((int)BtnCancelPos.X, (int)BtnCancelPos.Y, (int)(GFX.BTN_WIDTH * Scale), (int)(GFX.BTN_HEIGHT * Scale)), delegate
        {
            Main.PlaySound(SoundID.MenuTick);
            CloseGui();
        });
            AddButton(() => new Rectangle((int)BtnExperiencePos.X, (int)BtnExperiencePos.Y, 48, 48), delegate { guardianCrown = !guardianCrown; });
            AddButton(() => new Rectangle((int)BtnPermanencePos.X, (int)BtnPermanencePos.Y, 48, 48), delegate
          {
              if (!permanenceCrown && playerCharacter.permanence > 0)
                  permanenceCrown = true;
              else permanenceCrown = false;
          });
            AddButton(() => new Rectangle((int)BtnTranscendencePos.X, (int)BtnTranscendencePos.Y, 48, 48), delegate
          {
              if (!transcendenceCrown && playerCharacter.transcendence > 0)
                  transcendenceCrown = true;
              else transcendenceCrown = false;
          });
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            if (playerCharacter.permanence < 1) 
                permanenceCrown = false;
            if (playerCharacter.transcendence < 1) 
                transcendenceCrown = false;

            spriteBatch.Draw(GFX.anvil, new Vector2(GuiPosition.X - 150f * Scale, GuiPosition.Y - 100f * Scale), Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrading an item increases its power, but does not always succeed",
                new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y - 148f * Scale), Color.White, Scale);
            if (permanenceCrown)
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be downgraded.",
                    new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y - 124 * Scale), Color.Lime, Scale);
            else
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be destroyed.",
                    new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y - 124 * Scale), Color.Red, Scale);

            spriteBatch.Draw(guardianCrown ? GFX.buttonCrownPressed : GFX.buttonCrown, BtnExperiencePos, Color.White, Scale);
            spriteBatch.Draw(GFX.guardianCrown, BtnExperiencePos + new Vector2(9f, 10f) * Scale, guardianCrown ? Color.Gray : Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases success chance of upgrades by 10%", BtnExperiencePos + new Vector2(64f, 4f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "at the cost of a much higher price in currency",
                BtnExperiencePos + new Vector2(64f, 28f) * Scale, Color.White, Scale);
            spriteBatch.Draw(permanenceCrown ? GFX.buttonCrownPressed : GFX.buttonCrown, BtnPermanencePos, Color.White, Scale);
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<PermanenceCrown>()], BtnPermanencePos + new Vector2(9f, 10f) * Scale,
                permanenceCrown ? Color.Gray : Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, playerCharacter.permanence.ToString(), BtnPermanencePos + new Vector2(8f, 24f) * Scale, Color.White,
                Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "When an upgrade fails, items are downgraded", BtnPermanencePos + new Vector2(64f, 4f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText,
                "instead of being destroyed. " + (playerCharacter.permanence == 1 ? "1 crown left." : playerCharacter.permanence + " crowns left."),
                BtnPermanencePos + new Vector2(64f, 28f) * Scale, Color.White, Scale);
            spriteBatch.Draw(transcendenceCrown ? GFX.buttonCrownPressed : GFX.buttonCrown, BtnTranscendencePos, Color.White, Scale);
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<BlacksmithCrown>()], BtnTranscendencePos + new Vector2(9f, 10f) * Scale,
                transcendenceCrown ? Color.Gray : Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, playerCharacter.transcendence.ToString(), BtnTranscendencePos + new Vector2(8f, 24f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Allows you to upgrade an item to +8", BtnTranscendencePos + new Vector2(64f, 4f) * Scale,
                Color.White, Scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, playerCharacter.transcendence == 1 ? "1 crown left." : playerCharacter.transcendence + " crowns left.",
                BtnTranscendencePos + new Vector2(64f, 28f) * Scale, Color.White, Scale);

            spriteBatch.Draw(GFX.buttonClose, BtnCancelPos, Color.White, Scale);

            if (!selected)
            {
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Right-click a weapon to select it for upgrading>",
                    new Vector2(GuiPosition.X - 176f * Scale, GuiPosition.Y + 128f * Scale), Color.White, Scale);
            }

            else
            {
                if (ki.upgradeLevel >= PlayerCharacter.defaultMaxUpgradeLevel && !transcendenceCrown)
                {
                    ki = null;
                    item = null;
                    selected = false;
                }

                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Selected item: " + (item != null ? item.HoverName : ""),
                    new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 128f * Scale), Color.White, Scale);

                int modifier = guardianCrown ? 4 : 1;
                if (player.Wealth() >= upgradeCost * modifier)
                {
                    int bonusChance = guardianCrown ? 10 : 0;

                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(upgradeCost * modifier),
                        new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 152f * Scale), Color.White, Scale);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Chance to succeed: " + (upgradeSuccess + bonusChance).ToString() + "%",
                        new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 176f * Scale), bonusChance > 0 ? Color.Lime : Color.White, Scale);
                    spriteBatch.Draw(GFX.buttonUpgrade, BtnUpgradePos, Color.White, Scale);
                    if (new Rectangle((int)BtnUpgradePos.X, (int)BtnUpgradePos.Y, (int)(GFX.BTN_WIDTH * Scale), (int)(GFX.BTN_HEIGHT * Scale)).Contains(
                            Main.mouseX, Main.mouseY) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        if (ki == null)
                            throw new Exception("Sanity Check, Ki is null.");
                        if (ki.upgradeLevel >= PlayerCharacter.defaultMaxUpgradeLevel)
                            playerCharacter.transcendence -= 1;

                        if (Main.rand.Next(100) < upgradeSuccess + bonusChance)
                        {
                            ki.Upgrade(item);
                        }
                        else
                        {
                            if (permanenceCrown)
                                ki.Downgrade(item);
                            else
                                ki.Destroy(item);
                        }

                        if (permanenceCrown && bonusChance + upgradeSuccess < 100)
                            playerCharacter.permanence -= 1;

                        player.RemoveCoins(upgradeCost * modifier);
                        if (!AttemptSelectItem(ki, item)) CloseGui();
                    }
                }

                else
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(upgradeCost * modifier),
                        new Vector2(GuiPosition.X - 192f * Scale, GuiPosition.Y + 152f * Scale), Color.Red, Scale);
                }
            }

            //if (playerPosition != null) 
            if (Vector2.Distance(player.Center, playerPosition) > 128)
                CloseGui();
        }

        public bool AttemptSelectItem(kItem tKi, Item tItem)
        {
            byte startLevel = tKi.upgradeLevel;

            if (startLevel >= 7 && (startLevel >= 8 || !transcendenceCrown))
                return false;

            ki = tKi;
            item = tItem;
            selected = true;

            upgradeSuccess = 90 - startLevel * 10;
            if (startLevel == 0)
                upgradeCost = tItem.value / 20;
            else if (startLevel == 1)
                upgradeCost = tItem.value / 15;
            else if (startLevel == 2)
                upgradeCost = tItem.value / 10;
            else if (startLevel == 3)
                upgradeCost = tItem.value / 8;
            else if (startLevel == 4)
                upgradeCost = tItem.value / 5;
            else if (startLevel == 5)
                upgradeCost = tItem.value / 3;
            else if (startLevel == 6)
                upgradeCost = tItem.value / 2;
            else if (startLevel == 7)
                upgradeCost = tItem.value;
            else if (startLevel == 8)
                upgradeCost = (int)(tItem.value * 1.5);

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

        public void Reset()
        {
            ki = null;
            item = null;
            selected = false;
        }
    }
}