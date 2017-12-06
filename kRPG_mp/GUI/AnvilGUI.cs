using Terraria;
using Terraria.ModLoader;
using kRPG_mp.Items;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.ID;

namespace kRPG_mp.GUI
{
    public class AnvilGUI : BaseGUI
    {
        private PlayerCharacter character;
        private Mod mod;
        private kRPG krpg;
        private kItem ki;
        private Item item;
        public Vector2 position;

        private bool selected = false;

        private Vector2 GuiPosition
        {
            get
            {
                return new Vector2((float)Main.screenWidth / 2f - 128f, Main.screenHeight / 2f);
            }
        }
        private Vector2 BtnCancelPos
        {
            get
            {
                return new Vector2(GuiPosition.X + 36f, GuiPosition.Y + 268f);
            }
        }
        private Vector2 BtnUpgradePos
        {
            get
            {
                return new Vector2(GuiPosition.X + 36f, GuiPosition.Y + 208f);
            }
        }
        private Vector2 BtnExperiencePos
        {
            get
            {
                return new Vector2(GuiPosition.X + 150f, GuiPosition.Y - 76f);
            }
        }
        private Vector2 BtnPermanencePos
        {
            get
            {
                return new Vector2(GuiPosition.X + 150f, GuiPosition.Y - 12f);
            }
        }
        private Vector2 BtnTranscendencePos
        {
            get
            {
                return new Vector2(GuiPosition.X + 150f, GuiPosition.Y + 52f);
            }
        }

        private int upgradeCost = 0;

        //in percent:
        private int upgradeSuccess;

        private bool guardianCrown = false;
        private bool permanenceCrown = false;
        private bool transcendenceCrown = false;

        public AnvilGUI(Mod mod, PlayerCharacter character) : base()
        {
            this.character = character;
            this.mod = mod;
            krpg = (kRPG)mod;

            AddButton(delegate () { return new Rectangle((int)BtnCancelPos.X, (int)BtnCancelPos.Y, GFX.BTN_WIDTH, GFX.BTN_HEIGHT); }, delegate (Player player)
            {
                Main.PlaySound(SoundID.MenuTick);
                CloseGUI();
            });
            AddButton(delegate () { return new Rectangle((int)BtnExperiencePos.X, (int)BtnExperiencePos.Y, 48, 48); }, delegate (Player player)
            {
                guardianCrown = !guardianCrown;
            });
            AddButton(delegate () { return new Rectangle((int)BtnPermanencePos.X, (int)BtnPermanencePos.Y, 48, 48); }, delegate (Player player)
            {
                if (!permanenceCrown && character.permanence > 0)
                    permanenceCrown = true;
                else permanenceCrown = false;
            });
            AddButton(delegate () { return new Rectangle((int)BtnTranscendencePos.X, (int)BtnTranscendencePos.Y, 48, 48); }, delegate (Player player)
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

            spriteBatch.Draw(GFX.anvil, new Vector2(GuiPosition.X - 150f, GuiPosition.Y - 100f), Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrading an item increases its power, but does not always succeed", new Vector2(GuiPosition.X - 176f, GuiPosition.Y - 148f), Color.White);
            if (permanenceCrown)
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be downgraded.", new Vector2(GuiPosition.X - 176f, GuiPosition.Y - 124), Color.Lime);
            else
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "If the upgrade fails, the item will be destroyed.", new Vector2(GuiPosition.X - 176f, GuiPosition.Y - 124), Color.Red);

            spriteBatch.Draw(guardianCrown ? GFX.button_crown_pressed : GFX.button_crown, BtnExperiencePos, Color.White);
            spriteBatch.Draw(GFX.guardianCrown, BtnExperiencePos + new Vector2(9f, 10f), guardianCrown ? Color.Gray : Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Increases success chance of upgrades by 10%", BtnExperiencePos + new Vector2(64f, 4f), Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "at the cost of a much higher price in currency", BtnExperiencePos + new Vector2(64f, 28f), Color.White);
            spriteBatch.Draw(permanenceCrown ? GFX.button_crown_pressed : GFX.button_crown, BtnPermanencePos, Color.White);
            spriteBatch.Draw(Main.itemTexture[mod.ItemType<PermanenceCrown>()], BtnPermanencePos + new Vector2(9f, 10f), permanenceCrown ? Color.Gray : Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, character.permanence.ToString(), BtnPermanencePos + new Vector2(8f, 24f), Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "When an upgrade fails, items are downgraded", BtnPermanencePos + new Vector2(64f, 4f), Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "instead of being destroyed. " + (character.permanence == 1 ? "1 crown left." : character.permanence + " crowns left."), BtnPermanencePos + new Vector2(64f, 28f), Color.White);
            spriteBatch.Draw(transcendenceCrown ? GFX.button_crown_pressed : GFX.button_crown, BtnTranscendencePos, Color.White);
            spriteBatch.Draw(Main.itemTexture[mod.ItemType<BlacksmithCrown>()], BtnTranscendencePos + new Vector2(9f, 10f), transcendenceCrown ? Color.Gray : Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, character.transcendence.ToString(), BtnTranscendencePos + new Vector2(8f, 24f), Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Allows you to upgrade an item to +8", BtnTranscendencePos + new Vector2(64f, 4f), Color.White);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.transcendence == 1 ? "1 crown left." : character.transcendence + " crowns left.", BtnTranscendencePos + new Vector2(64f, 28f), Color.White);

            spriteBatch.Draw(GFX.button_close, BtnCancelPos, Color.White);

            if (!selected)
            {
                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "<Right-click a weapon to select it for upgrading>", new Vector2(GuiPosition.X - 176f, GuiPosition.Y + 128f), Color.White);
            }

            else
            {
                if (ki.upgradeLevel >= PlayerCharacter.defaultMaxUpgradeLevel && !transcendenceCrown)
                {
                    ki = null;
                    item = null;
                    selected = false;
                }

                spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Selected item: " + item.HoverName, new Vector2(GuiPosition.X - 192f, GuiPosition.Y + 128f), Color.White);

                int modifier = guardianCrown ? 4 : 1;
                if (player.Wealth() >= upgradeCost * modifier)
                {
                    int bonusChance = guardianCrown ? 10 : 0;

                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(upgradeCost * modifier), new Vector2(GuiPosition.X - 192f, GuiPosition.Y + 152f), Color.White);
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Chance to succeed: " + (upgradeSuccess + bonusChance).ToString() + "%", new Vector2(GuiPosition.X - 192f, GuiPosition.Y + 176f), bonusChance > 0 ? Color.Lime : Color.White);
                    spriteBatch.Draw(GFX.button_upgrade, BtnUpgradePos, Color.White);
                    if (new Rectangle((int)BtnUpgradePos.X, (int)BtnUpgradePos.Y, GFX.BTN_WIDTH, GFX.BTN_HEIGHT).Contains(Main.mouseX, Main.mouseY) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        if (ki.upgradeLevel >= PlayerCharacter.defaultMaxUpgradeLevel)
                            character.transcendence -= 1;

                        if (Main.rand.Next(100) < upgradeSuccess + bonusChance)
                        {
                            this.ki.Upgrade(item);
                        }
                        else
                        {
                            if (permanenceCrown)
                                this.ki.Downgrade(item);
                            else
                                this.ki.Destroy(item);
                        }

                        if (permanenceCrown && bonusChance + upgradeSuccess < 100)
                            character.permanence -= 1;

                        player.RemoveCoins(upgradeCost * modifier);
                        Reset();
                    }
                }

                else
                {
                    spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Upgrade Cost: " + API.MoneyToString(upgradeCost * modifier), new Vector2(GuiPosition.X - 192f, GuiPosition.Y + 152f), Color.Red);
                }
            }

            if (position != null) if (Vector2.Distance(player.Center, position) > 128) CloseGUI();
        }

        public void AttemptSelectItem(kItem ki, Item item)
        {
            byte startLevel = ki.upgradeLevel;

            if (startLevel >= 7 && (startLevel >= 8 || !transcendenceCrown))
                return;

            this.ki = ki;
            this.item = item;
            selected = true;

            upgradeSuccess = 90 - startLevel * 10;
            if (startLevel == 0)
            {
                upgradeCost = (int)(item.value / 50);
            }
            else if (startLevel == 1)
            {
                upgradeCost = (int)(item.value / 30);
            }
            else if (startLevel == 2)
            {
                upgradeCost = (int)(item.value / 20);
            }
            else if (startLevel == 3)
            {
                upgradeCost = (int)(item.value / 10);
            }
            else if (startLevel == 4)
            {
                upgradeCost = (int)(item.value / 5);
            }
            else if (startLevel == 5)
            {
                upgradeCost = (int)(item.value / 3);
            }
            else if (startLevel == 6)
            {
                upgradeCost = (int)(item.value / 2);
            }
            else if (startLevel == 7)
            {
                upgradeCost = (int)(item.value);
            }
            else if (startLevel == 8)
            {
                upgradeCost = (int)(item.value * 1.5);
            }
        }

        public bool IsSelecting()
        {
            return guiActive && !selected;
        }

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
