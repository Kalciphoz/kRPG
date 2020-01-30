using System;
using kRPG.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GUI
{
    public class AnvilGUI : BaseGUI
    {
        private PlayerCharacter character;
        private Mod mod;
        private kRPG krpg;
        private kItem ki;
        private Item item;
        public Vector2 position;

        private bool selected;

        private Vector2 GuiPosition => new Vector2(Main.screenWidth / 2f - 128f * scale, Main.screenHeight / 2f);

        private float scale => Math.Min(1f, Main.screenWidth / 3840f + 0.4f);

        private Vector2 BtnCancelPos => new Vector2(Main.screenWidth / 2f - 92f * scale, GuiPosition.Y + 268f * scale);

        private Vector2 BtnUpgradePos => new Vector2(Main.screenWidth / 2f - 92f * scale, GuiPosition.Y + 208f * scale);

        private Vector2 BtnExperiencePos => new Vector2(GuiPosition.X + 150f * scale, GuiPosition.Y - 76f * scale);

        private Vector2 BtnPermanencePos => new Vector2(GuiPosition.X + 150f * scale, GuiPosition.Y - 12f * scale);

        private Vector2 BtnTranscendencePos => new Vector2(GuiPosition.X + 150f * scale, GuiPosition.Y + 52f * scale);

        private int upgradeCost;

        //in percent:
        private int upgradeSuccess;

        private bool guardianCrown;
        private bool permanenceCrown;
        private bool transcendenceCrown;

        public AnvilGUI(Mod mod, PlayerCharacter character)
        {
            this.character = character;
            this.mod = mod;
            krpg = (kRPG)mod;

            AddButton(() => new Rectangle((int)BtnCancelPos.X, (int)BtnCancelPos.Y, (int)(GFX.BTN_WIDTH * scale), (int)(GFX.BTN_HEIGHT * scale)), delegate
        {
            Main.PlaySound(SoundID.MenuTick);
            CloseGUI();
        });
            AddButton(() => new Rectangle((int)BtnExperiencePos.X, (int)BtnExperiencePos.Y, 48, 48), delegate
          {
              guardianCrown = !guardianCrown;
          });
            AddButton(() => new Rectangle((int)BtnPermanencePos.X, (int)BtnPermanencePos.Y, 48, 48), delegate
          {
              if (!permanenceCrown && character.permanence > 0)
                  permanenceCrown = true;
              else permanenceCrown = false;
          });
            AddButton(() => new Rectangle((int)BtnTranscendencePos.X, (int)BtnTranscendencePos.Y, 48, 48), delegate
          {
              if (!transcendenceCrown && character.transcendence > 0)
                  transcendenceCrown = true;
              else transcendenceCrown = false;
          });
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            if (character.permanence < 1) permanenceCrown = false;
            if (character.transcendence < 1) transcendenceCrown = false;

            spriteBatch.Draw(GFX.anvil, new Vector2(GuiPosition.X - 150f * scale, GuiPosition.Y - 100f * scale), Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrading an item increases its power, but does not always succeed", new Vector2(GuiPosition.X - 176f * scale, GuiPosition.Y - 148f * scale), Color.White, scale);
            if (permanenceCrown)
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be downgraded.", new Vector2(GuiPosition.X - 176f * scale, GuiPosition.Y - 124 * scale), Color.Lime, scale);
            else
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be destroyed.", new Vector2(GuiPosition.X - 176f * scale, GuiPosition.Y - 124 * scale), Color.Red, scale);

            spriteBatch.Draw(guardianCrown ? GFX.button_crown_pressed : GFX.button_crown, BtnExperiencePos, Color.White, scale);
            spriteBatch.Draw(GFX.guardianCrown, BtnExperiencePos + new Vector2(9f, 10f) * scale, guardianCrown ? Color.Gray : Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases success chance of upgrades by 10%", BtnExperiencePos + new Vector2(64f, 4f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "at the cost of a much higher price in currency", BtnExperiencePos + new Vector2(64f, 28f) * scale, Color.White, scale);
            spriteBatch.Draw(permanenceCrown ? GFX.button_crown_pressed : GFX.button_crown, BtnPermanencePos, Color.White, scale);
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<PermanenceCrown>()], BtnPermanencePos + new Vector2(9f, 10f) * scale, permanenceCrown ? Color.Gray : Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, character.permanence.ToString(), BtnPermanencePos + new Vector2(8f, 24f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "When an upgrade fails, items are downgraded", BtnPermanencePos + new Vector2(64f, 4f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "instead of being destroyed. " + (character.permanence == 1 ? "1 crown left." : character.permanence + " crowns left."), BtnPermanencePos + new Vector2(64f, 28f) * scale, Color.White, scale);
            spriteBatch.Draw(transcendenceCrown ? GFX.button_crown_pressed : GFX.button_crown, BtnTranscendencePos, Color.White, scale);
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<BlacksmithCrown>()], BtnTranscendencePos + new Vector2(9f, 10f) * scale, transcendenceCrown ? Color.Gray : Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, character.transcendence.ToString(), BtnTranscendencePos + new Vector2(8f, 24f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Allows you to upgrade an item to +8", BtnTranscendencePos + new Vector2(64f, 4f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.transcendence == 1 ? "1 crown left." : character.transcendence + " crowns left.", BtnTranscendencePos + new Vector2(64f, 28f) * scale, Color.White, scale);

            spriteBatch.Draw(GFX.button_close, BtnCancelPos, Color.White, scale);

            if (!selected)
            {
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Right-click a weapon to select it for upgrading>", new Vector2(GuiPosition.X - 176f * scale, GuiPosition.Y + 128f * scale), Color.White, scale);
            }

            else
            {
                if (ki.upgradeLevel >= PlayerCharacter.defaultMaxUpgradeLevel && !transcendenceCrown)
                {
                    ki = null;
                    item = null;
                    selected = false;
                }


                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Selected item: " + (item != null ? item.HoverName : ""), new Vector2(GuiPosition.X - 192f * scale, GuiPosition.Y + 128f * scale), Color.White, scale);

                int modifier = guardianCrown ? 4 : 1;
                if (player.Wealth() >= upgradeCost * modifier)
                {
                    int bonusChance = guardianCrown ? 10 : 0;

                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(upgradeCost * modifier), new Vector2(GuiPosition.X - 192f * scale, GuiPosition.Y + 152f * scale), Color.White, scale);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Chance to succeed: " + (upgradeSuccess + bonusChance).ToString() + "%", new Vector2(GuiPosition.X - 192f * scale, GuiPosition.Y + 176f * scale), bonusChance > 0 ? Color.Lime : Color.White, scale);
                    spriteBatch.Draw(GFX.button_upgrade, BtnUpgradePos, Color.White, scale);
                    if (new Rectangle((int)BtnUpgradePos.X, (int)BtnUpgradePos.Y, (int)(GFX.BTN_WIDTH * scale), (int)(GFX.BTN_HEIGHT * scale)).Contains(Main.mouseX, Main.mouseY) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        if (ki == null)
                            throw new Exception("Sanity Check, Ki is null.");
                        if (ki.upgradeLevel >= PlayerCharacter.defaultMaxUpgradeLevel)
                            character.transcendence -= 1;

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
                            character.permanence -= 1;

                        player.RemoveCoins(upgradeCost * modifier);
                        if (!AttemptSelectItem(ki, item)) CloseGUI();
                    }
                }

                else
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(upgradeCost * modifier), new Vector2(GuiPosition.X - 192f * scale, GuiPosition.Y + 152f * scale), Color.Red, scale);
                }
            }

            //if (position != null) 
            if (Vector2.Distance(player.Center, position) > 128)
                CloseGUI();
        }

        public bool AttemptSelectItem(kItem tKi, Item tItem)
        {
            byte startLevel = tKi.upgradeLevel;

            if (startLevel >= 7 && (startLevel >= 8 || !transcendenceCrown))
                return false;

            this.ki = tKi;
            this.item = tItem;
            selected = true;

            upgradeSuccess = 90 - startLevel * 10;
            if (startLevel == 0)
            {
                upgradeCost = (tItem.value / 20);
            }
            else if (startLevel == 1)
            {
                upgradeCost = (tItem.value / 15);
            }
            else if (startLevel == 2)
            {
                upgradeCost = (tItem.value / 10);
            }
            else if (startLevel == 3)
            {
                upgradeCost = (tItem.value / 8);
            }
            else if (startLevel == 4)
            {
                upgradeCost = (tItem.value / 5);
            }
            else if (startLevel == 5)
            {
                upgradeCost = (tItem.value / 3);
            }
            else if (startLevel == 6)
            {
                upgradeCost = (tItem.value / 2);
            }
            else if (startLevel == 7)
            {
                upgradeCost = (tItem.value);
            }
            else if (startLevel == 8)
            {
                upgradeCost = (int)(tItem.value * 1.5);
            }

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
