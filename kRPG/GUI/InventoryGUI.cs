using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Graphics;
using Terraria.GameInput;
using Terraria.UI.Gamepad;
using Terraria.Localization;
using Terraria.UI.Chat;
using Terraria.Map;
using Terraria.GameContent.UI.States;
using kRPG.Items;

namespace kRPG.GUI
{
    public class InventoryGUI : BaseGUI
    {
        public Vector2 origin
        {
            get
            {
                return new Vector2(40f, 8f) * scale;
            }
        }

        private float scale
        {
            get
            {
                return Math.Min(1f, Main.screenWidth / 3840f + 0.4f);
            }
        }

        public override bool PreDraw()
        {
            return Main.playerInventory;
        }

        public InventoryGUI(PlayerCharacter character, Mod mod) : base()
        {
            AddButton(delegate () { return new Rectangle((int)(origin.X + 142f * scale), (int)(origin.Y + 102f * scale), (int)(GFX.button_stats.Width * scale), (int)(GFX.button_stats.Height * scale)); }, delegate (Player player)
            {
                Main.PlaySound(SoundID.MenuTick);
                PlayerCharacter pc = player.GetModPlayer<PlayerCharacter>();
                pc.statPage = !pc.statPage;
            });
            AddButton(delegate () { return new Rectangle((int)(origin.X + 174f * scale), (int)(origin.Y + 102f * scale), (int)(GFX.button_page1.Width * scale), (int)(GFX.button_page1.Height * scale)); }, delegate (Player player)
            {
                Main.PlaySound(SoundID.MenuTick);
                PlayerCharacter pc = player.GetModPlayer<PlayerCharacter>();
                pc.OpenInventoryPage(0);
            });
            AddButton(delegate () { return new Rectangle((int)(origin.X + 206f * scale), (int)(origin.Y + 102f * scale), (int)(GFX.button_page2.Width * scale), (int)(GFX.button_page2.Height * scale)); }, delegate (Player player)
            {
                Main.PlaySound(SoundID.MenuTick);
                PlayerCharacter pc = player.GetModPlayer<PlayerCharacter>();
                pc.OpenInventoryPage(1);
            });
            AddButton(delegate() { return new Rectangle((int)(origin.X + 238f * scale), (int)(origin.Y + 102f * scale), (int)(GFX.button_page3.Width * scale), (int)(GFX.button_page3.Height * scale)); }, delegate (Player player)
            {
                Main.PlaySound(SoundID.MenuTick);
                PlayerCharacter pc = player.GetModPlayer<PlayerCharacter>();
                pc.OpenInventoryPage(2);
            });
            AddButton(delegate () { return new Rectangle((int)points_origin.X, (int)points_origin.Y, (int)(GFX.inventory_points.Width * scale), (int)(GFX.inventory_points.Height * scale)); }, delegate (Player player)
            {
                character.CloseGUIs();
                Main.PlaySound(SoundID.MenuTick);
                character.levelGUI.guiActive = player.GetModPlayer<PlayerCharacter>(mod).UnspentPoints();
            }, delegate (Player player, SpriteBatch spriteBatch)
            {
                Main.LocalPlayer.mouseInterface = true;
                string s = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>(mod).UnspentPoints() ? "Click here to allocate stat points" : "You have no unspent stat points";
                Main.instance.MouseText(s);
            });
        }
        private Vector2 points_origin
        {
            get
            {
                return origin + new Vector2(538f, 76f) * scale;
            }
        }
        private const int bar_length = 192;
        private float bar_x
        {
            get
            {
                return 314f * scale;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();

            for (int i = 0; i < 40; i += 1)
                character.inventories[character.activeInvPage][i] = player.inventory[i + 10];
            
            if ((int)Main.time % 30 < 1)
                API.FindRecipes();

            Vanilla(!character.statPage);
            if (character.statPage) DrawStatPage(spriteBatch, character);
            spriteBatch.Draw(GFX.inventoryFrame, new Vector2((int)origin.X, (int)origin.Y), Color.White, scale);
            if (!character.statPage) spriteBatch.Draw(GFX.inventory_separator, new Vector2(origin.X + 56 * scale, origin.Y + 354 * scale), Color.White, scale);
            DrawHotbar(spriteBatch);
            spriteBatch.Draw(character.statPage ? GFX.button_stats_pressed : GFX.button_stats, origin + new Vector2(142f, 102f) * scale, Color.White, scale);
            spriteBatch.Draw(character.activeInvPage == 0 && !character.statPage ? GFX.button_page1_pressed : GFX.button_page1, origin + new Vector2(174f, 102f) * scale, Color.White, scale);
            spriteBatch.Draw(character.activeInvPage == 1 && !character.statPage ? GFX.button_page2_pressed : GFX.button_page2, origin + new Vector2(206f, 102f) * scale, Color.White, scale);
            spriteBatch.Draw(character.activeInvPage == 2 && !character.statPage ? GFX.button_page3_pressed : GFX.button_page3, origin + new Vector2(238f, 102f) * scale, Color.White, scale);
            StatusBar.DrawNumerals(spriteBatch, player.GetModPlayer<PlayerCharacter>().level, scale);

            int currentLifeLength = (int)Math.Round((decimal)player.statLife / (decimal)player.statLifeMax2 * bar_length);
            spriteBatch.Draw(GFX.inventory_life, origin + new Vector2(bar_x, 70 * scale), new Rectangle(0, 0, currentLifeLength, 20), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            int currentManaLength = (int)Math.Round((decimal)character.mana / (decimal)player.statManaMax2 * bar_length);
            spriteBatch.Draw(GFX.inventory_mana, origin + new Vector2(bar_x, 98 * scale), new Rectangle(0, 0, currentManaLength, 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            int currentXPLength = (int)Math.Round((decimal)bar_length * (decimal)character.xp / (decimal)character.ExperienceToLevel());
            spriteBatch.Draw(GFX.inventory_xp, origin + new Vector2(bar_x, 126 * scale), new Rectangle(0, 0, currentXPLength, 8), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(GFX.inventory_barCovers, origin + new Vector2(302, 68) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, player.statLife.ToString() + " / " + player.statLifeMax2.ToString(), origin + new Vector2(bar_x + 16f * scale, 72f * scale), Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.mana.ToString() + " / " + player.statManaMax2.ToString(), origin + new Vector2(bar_x + 16f * scale, 100f * scale), Color.White, 0.6f * scale);

            if (character.UnspentPoints())
                spriteBatch.Draw(GFX.inventory_points, points_origin, Color.White, scale);

            Mod mod = ModLoader.GetMod("kRPG");
            spriteBatch.Draw(Main.itemTexture[mod.ItemType<PermanenceCrown>()], origin + new Vector2(600f, 68f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, "x" + character.permanence.ToString(), origin + new Vector2(640f, 72f) * scale, Color.White, scale * 1.2f);
            spriteBatch.Draw(Main.itemTexture[mod.ItemType<BlacksmithCrown>()], origin + new Vector2(600f, 108f) * scale, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontItemStack, "x" + character.transcendence.ToString(), origin + new Vector2(640f, 112f) * scale, Color.White, scale * 1.2f);

            StatusBar.DrawBuffs();
        }

        private static MethodInfo DrawPVP = typeof(Main).GetMethod("DrawPVPIcons", BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo PageIcons = typeof(Main).GetMethod("DrawPageIcons", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo mH = typeof(Main).GetField("mH", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo mouseReforge = typeof(Main).GetField("mouseReforge", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo reforgeScale = typeof(Main).GetField("reforgeScale", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo allChestStackHover = typeof(Main).GetField("allChestStackHover", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo inventorySortMouseOver = typeof(Main).GetField("inventorySortMouseOver", BindingFlags.NonPublic | BindingFlags.Static);

        private void DrawStatPage(SpriteBatch spriteBatch, PlayerCharacter character)
        {
            Vector2 panel_origin = origin + new Vector2(56, 146) * scale;
            spriteBatch.Draw(GFX.inventory_panel, panel_origin, Color.White, scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, (character.ExperienceToLevel() - character.xp).ToString() + " XP to level", panel_origin + new Vector2(24f, 24f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Resilience:", panel_origin + new Vector2(24f, 42f) * scale, new Color(223, 0, 0), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.TotalStats(STAT.RESILIENCE).ToString(), panel_origin + new Vector2(128f, 42f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Quickness:", panel_origin + new Vector2(24f, 60f) * scale, new Color(0, 191, 31), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.TotalStats(STAT.QUICKNESS).ToString(), panel_origin + new Vector2(128f, 60f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Potency:", panel_origin + new Vector2(24f, 78f) * scale, new Color(27, 65, 255), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.TotalStats(STAT.POTENCY).ToString(), panel_origin + new Vector2(128f, 78f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Wits:", panel_origin + new Vector2(24f, 96f) * scale, new Color(239, 223, 31), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.TotalStats(STAT.WITS).ToString(), panel_origin + new Vector2(128f, 96f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Damage:", panel_origin + new Vector2(24f, 114f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round(character.damageMultiplier * 100f).ToString() + "%", panel_origin + new Vector2(128f, 114f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Melee speed:", panel_origin + new Vector2(24f, 132f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round(1f / character.player.meleeSpeed * 100f).ToString() + "%", panel_origin + new Vector2(128f, 132f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Crit chance:", panel_origin + new Vector2(24f, 150f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "+" + character.critBoost.ToString() + "%", panel_origin + new Vector2(120f, 150f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Crit multiplier:", panel_origin + new Vector2(24f, 168f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round((character.damageMultiplier + character.critMultiplier) * 200f).ToString() + "%", panel_origin + new Vector2(128f, 168f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Defense:", panel_origin + new Vector2(24f, 184f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.player.statDefense.ToString(), panel_origin + new Vector2(128f, 184f) * scale, Color.White, 0.8f * scale);
            
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Fire resistance:", panel_origin + new Vector2(184f, 42f) * scale, new Color(255, 63, 0), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.resistance[ELEMENT.FIRE].ToString(), panel_origin + new Vector2(288f, 42f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Cold resistance:", panel_origin + new Vector2(184f, 60f) * scale, new Color(63, 127, 255), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.resistance[ELEMENT.COLD].ToString(), panel_origin + new Vector2(288f, 60f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Light. resist.:", panel_origin + new Vector2(184f, 78f) * scale, new Color(255, 239, 0), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.resistance[ELEMENT.LIGHTNING].ToString(), panel_origin + new Vector2(288f, 78f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Shadow resist.:", panel_origin + new Vector2(184f, 96f) * scale, new Color(95, 0, 191), 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.resistance[ELEMENT.SHADOW].ToString(), panel_origin + new Vector2(288f, 96f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Evasion:", panel_origin + new Vector2(184f, 114f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.evasion.ToString(), panel_origin + new Vector2(288f, 114f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Dodge chance:", panel_origin + new Vector2(184f, 132f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round((80f - (80 * 25) / (25 + character.evasion))).ToString() + "%", panel_origin + new Vector2(288f, 132f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Crit avoidance:", panel_origin + new Vector2(184f, 150f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round((90f - (90 * 38) / (38 + character.evasion + character.TotalStats(STAT.WITS)*2))).ToString() + "%", panel_origin + new Vector2(288f, 150f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Accuracy:", panel_origin + new Vector2(184f, 168f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.accuracy.ToString(), panel_origin + new Vector2(288f, 168f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Hit chance:", panel_origin + new Vector2(184f, 186f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round(character.hitChance * 100).ToString() + "%", panel_origin + new Vector2(288f, 186f) * scale, Color.White, 0.8f * scale);
            
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Life regen:", panel_origin + new Vector2(344f, 42f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, (Math.Round(character.lifeRegen * 10) / 10).ToString(), panel_origin + new Vector2(448f, 42f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Mana regen:", panel_origin + new Vector2(344f, 60f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, (Math.Round(character.manaRegen*10)/10).ToString(), panel_origin + new Vector2(448f, 60f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Movespeed:", panel_origin + new Vector2(344f, 78f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, Math.Round(character.player.moveSpeed*100).ToString() + "%", panel_origin + new Vector2(448f, 78f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Jump boost:", panel_origin + new Vector2(344f, 96f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.player.jumpSpeedBoost.ToString(), panel_origin + new Vector2(448f, 96f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, "Life leech:", panel_origin + new Vector2(344f, 114f) * scale, Color.White, 0.8f * scale);
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, (character.lifeLeech * 100).ToString() + "%", panel_origin + new Vector2(448f, 114f) * scale, Color.White, 0.8f * scale);
        }

        private void DrawHotbar(SpriteBatch spriteBatch)
        {
            float oldinvscale = Main.inventoryScale;
            Main.inventoryScale = scale;
            for (int i = 0; i < 10; i++)
            {
                float x2 = origin.X + 54 * scale + (float)(i * 52 * scale);
                float y2 = origin.Y + 364 * scale;
                if (Main.mouseX >= x2 && (float)Main.mouseX <= (float)x2 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= y2 && (float)Main.mouseY <= (float)y2 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                {
                    Main.player[Main.myPlayer].mouseInterface = true;
                    ItemSlot.OverrideHover(Main.player[Main.myPlayer].inventory, 0, i);
                    if (Main.player[Main.myPlayer].inventoryChestStack[i] && (Main.player[Main.myPlayer].inventory[i].type == 0 || Main.player[Main.myPlayer].inventory[i].stack == 0))
                    {
                        Main.player[Main.myPlayer].inventoryChestStack[i] = false;
                    }
                    if (!Main.player[Main.myPlayer].inventoryChestStack[i])
                    {
                        if (Main.mouseLeftRelease && Main.mouseLeft)
                        {
                            ItemSlot.LeftClick(Main.player[Main.myPlayer].inventory, 0, i);
                            API.FindRecipes();
                        }
                        else
                        {
                            ItemSlot.RightClick(Main.player[Main.myPlayer].inventory, 0, i);
                        }
                    }
                    ItemSlot.MouseHover(Main.player[Main.myPlayer].inventory, 0, i);
                }
                API.ItemSlotDrawExtension(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 0, i, new Vector2((float)x2, (float)y2), Color.White, Color.White);
            }
            Main.inventoryScale = oldinvscale;
        }

        private void DrawInventory(SpriteBatch spriteBatch)
        {
            float oldinvscale = Main.inventoryScale;
            Main.inventoryScale = scale;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    float x2 = origin.X + 54 * scale + (float)(i * 52 * scale);
                    float y2 = origin.Y + 94 * scale + (float)(j * 52 * scale);
                    int id = i + j * 10;
                    if (Main.mouseX >= x2 && (float)Main.mouseX <= (float)x2 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= y2 && (float)Main.mouseY <= (float)y2 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                        ItemSlot.OverrideHover(Main.player[Main.myPlayer].inventory, 0, id);
                        if (Main.player[Main.myPlayer].inventoryChestStack[id] && (Main.player[Main.myPlayer].inventory[id].type == 0 || Main.player[Main.myPlayer].inventory[id].stack == 0))
                        {
                            Main.player[Main.myPlayer].inventoryChestStack[id] = false;
                        }
                        if (!Main.player[Main.myPlayer].inventoryChestStack[id])
                        {
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                ItemSlot.LeftClick(Main.player[Main.myPlayer].inventory, 0, id);
                                API.FindRecipes();
                            }
                            else
                            {
                                ItemSlot.RightClick(Main.player[Main.myPlayer].inventory, 0, id);
                            }
                        }
                        ItemSlot.MouseHover(Main.player[Main.myPlayer].inventory, 0, id);
                    }
                    try
                    {
                        API.ItemSlotDrawExtension(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 0, id, new Vector2((float)x2, (float)y2), Color.White, Color.White);
                    }
                    catch (SystemException e)
                    {
                        ErrorLogger.Log(e.ToString());

                        if (Main.LocalPlayer.inventory[id].modItem is ProceduralItem)
                        {
                            try
                            {
                                ProceduralItem item = (ProceduralItem)Main.LocalPlayer.inventory[id].modItem;
                                item.Initialize();
                            }
                            catch (SystemException e2)
                            {
                                ErrorLogger.Log("Failed to initialize: " + e2);
                                spriteBatch.Draw(GFX.itemSlot_broken, new Vector2((float)x2, (float)y2), Color.White, Main.inventoryScale);
                                Main.LocalPlayer.inventory[id].SetDefaults();
                                ErrorLogger.Log("ITEM " + id + " WAS DESTROYED.");
                            }
                        }
                    }
                }
            }
            Main.inventoryScale = oldinvscale;
            Main.instance.invBottom = (int)(436 * scale);
        }

        private void Vanilla(bool drawInv)
        {
            try
            {
                Main main = Main.instance;

                if (Main.ShouldPVPDraw)
                {
                    DrawPVP.Invoke(null, null);
                }

                Main.inventoryScale = 0.85f;
                int num = 40 + (int)(600 * scale);
                int num2 = 314;
                new Color(150, 150, 150, 150);
                if (Main.mouseX >= num && (float)Main.mouseX <= (float)num + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num2 && (float)Main.mouseY <= (float)num2 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                {
                    Main.player[Main.myPlayer].mouseInterface = true;
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        ItemSlot.LeftClick(ref Main.player[Main.myPlayer].trashItem, 6);
                        API.FindRecipes();
                    }
                    ItemSlot.MouseHover(ref Main.player[Main.myPlayer].trashItem, 6);
                }
                ItemSlot.Draw(Main.spriteBatch, ref Main.player[Main.myPlayer].trashItem, 6, new Vector2((float)num, (float)num2), default(Color));

                if (drawInv) DrawInventory(Main.spriteBatch);

                if (!PlayerInput.UsingGamepad)
                {
                    int num6 = 0;
                    int num7 = 2;
                    int num8 = 32;
                    Player player = Main.player[Main.myPlayer];
                    int num9 = player.InfoAccMechShowWires.ToInt() * 6 + player.rulerLine.ToInt() + player.rulerGrid.ToInt() + player.autoActuator.ToInt() + player.autoPaint.ToInt();
                    if (num9 >= 8)
                    {
                        num8 = 2;
                    }
                    if (!Main.player[Main.myPlayer].hbLocked)
                    {
                        num6 = 1;
                    }
                    Main.spriteBatch.Draw(Main.HBLockTexture[num6], new Vector2((float)num7, (float)num8), new Rectangle?(new Rectangle(0, 0, Main.HBLockTexture[num6].Width, Main.HBLockTexture[num6].Height)), Main.inventoryBack, 0f, default(Vector2), 0.9f, SpriteEffects.None, 0f);
                    if (Main.mouseX > num7 && (float)Main.mouseX < (float)num7 + (float)Main.HBLockTexture[num6].Width * 0.9f && Main.mouseY > num8 && (float)Main.mouseY < (float)num8 + (float)Main.HBLockTexture[num6].Height * 0.9f)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                        if (!Main.player[Main.myPlayer].hbLocked)
                        {
                            main.MouseText(Lang.inter[5].Value, 0, 0, -1, -1, -1, -1);
                            Main.mouseText = true;
                        }
                        else
                        {
                            main.MouseText(Lang.inter[6].Value, 0, 0, -1, -1, -1, -1);
                            Main.mouseText = true;
                        }
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Main.PlaySound(22, -1, -1, 1, 1f, 0f);
                            if (!Main.player[Main.myPlayer].hbLocked)
                            {
                                Main.player[Main.myPlayer].hbLocked = true;
                            }
                            else
                            {
                                Main.player[Main.myPlayer].hbLocked = false;
                            }
                        }
                    }
                }
                ItemSlot.DrawRadialDpad(Main.spriteBatch, new Vector2(20f) + new Vector2(56f * Main.inventoryScale * 10f, 56f * Main.inventoryScale * 5f) + new Vector2(26f, 70f));
                if (Main.mapEnabled)
                {
                    bool flag = false;
                    int num10 = Main.screenWidth - 440;
                    int num11 = 40;
                    if (Main.screenWidth < 940)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        num10 = Main.screenWidth - 40;
                        num11 = Main.screenHeight - 200;
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        int num12 = num10 + k * 32;
                        int num13 = num11;
                        if (flag)
                        {
                            num12 = num10;
                            num13 = num11 + k * 32;
                        }
                        int num14 = k;
                        int num15 = 120;
                        if (k > 0 && Main.mapStyle == k - 1)
                        {
                            num15 = 200;
                        }
                        if (Main.mouseX >= num12 && Main.mouseX <= num12 + 32 && Main.mouseY >= num13 && Main.mouseY <= num13 + 30 && !PlayerInput.IgnoreMouseInterface)
                        {
                            num15 = 255;
                            num14 += 4;
                            Main.player[Main.myPlayer].mouseInterface = true;
                            if (Main.mouseLeft && Main.mouseLeftRelease)
                            {
                                if (k == 0)
                                {
                                    Main.playerInventory = false;
                                    Main.player[Main.myPlayer].talkNPC = -1;
                                    Main.npcChatCornerItem = 0;
                                    Main.PlaySound(10, -1, -1, 1, 1f, 0f);
                                    float num16 = 2.5f;
                                    Main.mapFullscreenScale = num16;
                                    Main.mapFullscreen = true;
                                    Main.resetMapFull = true;
                                }
                                if (k == 1)
                                {
                                    Main.mapStyle = 0;
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                }
                                if (k == 2)
                                {
                                    Main.mapStyle = 1;
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                }
                                if (k == 3)
                                {
                                    Main.mapStyle = 2;
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                }
                            }
                        }
                        Main.spriteBatch.Draw(Main.mapIconTexture[num14], new Vector2((float)num12, (float)num13), new Rectangle?(new Rectangle(0, 0, Main.mapIconTexture[num14].Width, Main.mapIconTexture[num14].Height)), new Color(num15, num15, num15, num15), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                }
                if (Main.armorHide)
                {
                    Main.armorAlpha -= 0.1f;
                    if (Main.armorAlpha < 0f)
                    {
                        Main.armorAlpha = 0f;
                    }
                }
                else
                {
                    Main.armorAlpha += 0.025f;
                    if (Main.armorAlpha > 1f)
                    {
                        Main.armorAlpha = 1f;
                    }
                }
                new Color((int)((byte)((float)Main.mouseTextColor * Main.armorAlpha)), (int)((byte)((float)Main.mouseTextColor * Main.armorAlpha)), (int)((byte)((float)Main.mouseTextColor * Main.armorAlpha)), (int)((byte)((float)Main.mouseTextColor * Main.armorAlpha)));
                Main.armorHide = false;
                int num17 = (int)PageIcons.Invoke(main, null);
                if (num17 > -1)
                {
                    Main.HoverItem = new Item();
                    switch (num17)
                    {
                        case 1:
                            Main.hoverItemName = Lang.inter[80].Value;
                            break;
                        case 2:
                            Main.hoverItemName = Lang.inter[79].Value;
                            break;
                        case 3:
                            Main.hoverItemName = (Main.CaptureModeDisabled ? Lang.inter[115].Value : Lang.inter[81].Value);
                            break;
                    }
                }
                if (Main.EquipPage == 2)
                {
                    Point value = new Point(Main.mouseX, Main.mouseY);
                    Rectangle r = new Rectangle(0, 0, (int)((float)Main.inventoryBackTexture.Width * Main.inventoryScale), (int)((float)Main.inventoryBackTexture.Height * Main.inventoryScale));
                    Item[] inv = Main.player[Main.myPlayer].miscEquips;
                    int num18 = Main.screenWidth - 92;
                    int num19 = (int)mH.GetValue(null) + 174;
                    for (int l = 0; l < 2; l++)
                    {
                        if (l == 0)
                        {
                            inv = Main.player[Main.myPlayer].miscEquips;
                        }
                        else if (l == 1)
                        {
                            inv = Main.player[Main.myPlayer].miscDyes;
                        }
                        r.X = num18 + l * -47;
                        for (int m = 0; m < 5; m++)
                        {
                            int context = 0;
                            int num20 = -1;
                            switch (m)
                            {
                                case 0:
                                    context = 19;
                                    num20 = 0;
                                    break;
                                case 1:
                                    context = 20;
                                    num20 = 1;
                                    break;
                                case 2:
                                    context = 18;
                                    break;
                                case 3:
                                    context = 17;
                                    break;
                                case 4:
                                    context = 16;
                                    break;
                            }
                            if (l == 1)
                            {
                                context = 12;
                                num20 = -1;
                            }
                            r.Y = num19 + m * 47;
                            Texture2D texture2D = Main.inventoryTickOnTexture;
                            if (Main.player[Main.myPlayer].hideMisc[num20])
                            {
                                texture2D = Main.inventoryTickOffTexture;
                            }
                            Rectangle r2 = new Rectangle(r.Left + 34, r.Top - 2, texture2D.Width, texture2D.Height);
                            int num21 = 0;
                            bool flag2 = false;
                            if (r2.Contains(value) && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.player[Main.myPlayer].mouseInterface = true;
                                flag2 = true;
                                if (Main.mouseLeft && Main.mouseLeftRelease)
                                {
                                    if (num20 == 0)
                                    {
                                        Main.player[Main.myPlayer].TogglePet();
                                    }
                                    if (num20 == 1)
                                    {
                                        Main.player[Main.myPlayer].ToggleLight();
                                    }
                                    Main.mouseLeftRelease = false;
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                    if (Main.netMode == 1)
                                    {
                                        NetMessage.SendData(4, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
                                    }
                                }
                                if (Main.player[Main.myPlayer].hideMisc[num20])
                                {
                                    num21 = 2;
                                }
                                else
                                {
                                    num21 = 1;
                                }
                            }
                            if (r.Contains(value) && !flag2 && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.player[Main.myPlayer].mouseInterface = true;
                                Main.armorHide = true;
                                ItemSlot.Handle(inv, context, m);
                            }
                            ItemSlot.Draw(Main.spriteBatch, inv, context, m, r.TopLeft(), default(Color));
                            if (num20 != -1)
                            {
                                Main.spriteBatch.Draw(texture2D, r2.TopLeft(), Color.White * 0.7f);
                                if (num21 > 0)
                                {
                                    Main.HoverItem = new Item();
                                    Main.hoverItemName = Lang.inter[58 + num21].Value;
                                }
                            }
                        }
                    }
                    num19 += 247;
                    num18 += 8;
                    int num22 = -1;
                    int num23 = 0;
                    int num24 = 3;
                    int num25 = 260;
                    if (Main.screenHeight > 630 + num25 * (Main.mapStyle == 1).ToInt())
                    {
                        num24++;
                    }
                    if (Main.screenHeight > 680 + num25 * (Main.mapStyle == 1).ToInt())
                    {
                        num24++;
                    }
                    if (Main.screenHeight > 730 + num25 * (Main.mapStyle == 1).ToInt())
                    {
                        num24++;
                    }
                    UILinkPointNavigator.Shortcuts.BUFFS_DRAWN = num23;
                    UILinkPointNavigator.Shortcuts.BUFFS_PER_COLUMN = num24;
                    if (num22 >= 0)
                    {
                        int num30 = Main.player[Main.myPlayer].buffType[num22];
                        if (num30 > 0)
                        {
                            Main.buffString = Lang.GetBuffDescription(num30);
                            if (num30 == 26 && Main.expertMode)
                            {
                                Main.buffString = Language.GetTextValue("BuffDescription.WellFed_Expert");
                            }
                            if (num30 == 147)
                            {
                                Main.bannerMouseOver = true;
                            }
                            if (num30 == 94)
                            {
                                int num31 = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
                                Main.buffString = Main.buffString + num31 + "%";
                            }
                            int itemRarity = 0;
                            if (Main.meleeBuff[num30])
                            {
                                itemRarity = -10;
                            }
                            BuffLoader.ModifyBuffTip(num30, ref Main.buffString, ref itemRarity);
                            main.MouseTextHackZoom(Lang.GetBuffName(num30), itemRarity, 0);
                        }
                    }
                }

                else if (Main.EquipPage == 1)
                {
                    UILinkPointNavigator.Shortcuts.NPCS_LastHovered = -1;
                    if (Main.mouseX > Main.screenWidth - 64 - 28 && Main.mouseX < (int)((float)(Main.screenWidth - 64 - 28) + 56f * Main.inventoryScale) && Main.mouseY > 174 + (int)mH.GetValue(null) && Main.mouseY < (int)((float)(174 + (int)mH.GetValue(null)) + 448f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    int num32 = 0;
                    string text = "";
                    int num33 = 0;
                    int num34 = 0;
                    for (int num35 = 0; num35 < Main.npcHeadTexture.Length; num35++)
                    {
                        bool flag3 = false;
                        int num36 = 0;
                        if (num35 == 0)
                        {
                            flag3 = true;
                        }
                        else if (num35 == 21)
                        {
                            flag3 = false;
                        }
                        else
                        {
                            for (int num37 = 0; num37 < 200; num37++)
                            {
                                if (Main.npc[num37].active && NPC.TypeToHeadIndex(Main.npc[num37].type) == num35)
                                {
                                    flag3 = true;
                                    num36 = num37;
                                    break;
                                }
                            }
                        }
                        if (flag3)
                        {
                            int num38 = Main.screenWidth - 64 - 28 + num34;
                            int num39 = (int)((float)(174 + (int)mH.GetValue(null)) + (float)(num32 * 56) * Main.inventoryScale) + num33;
                            Color white = new Color(100, 100, 100, 100);
                            if (num39 > Main.screenHeight - 80)
                            {
                                num34 -= 48;
                                num33 -= num39 - (174 + (int)mH.GetValue(null));
                                num38 = Main.screenWidth - 64 - 28 + num34;
                                num39 = (int)((float)(174 + (int)mH.GetValue(null)) + (float)(num32 * 56) * Main.inventoryScale) + num33;
                                if (UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn == 100)
                                {
                                    UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn = num32;
                                }
                            }
                            if (Main.mouseX >= num38 && (float)Main.mouseX <= (float)num38 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num39 && (float)Main.mouseY <= (float)num39 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale)
                            {
                                UILinkPointNavigator.Shortcuts.NPCS_LastHovered = num36;
                                Main.mouseText = true;
                                if (num35 == 0)
                                {
                                    text = Lang.inter[8].Value;
                                }
                                else
                                {
                                    text = Main.npc[num36].FullName;
                                }
                                if (!PlayerInput.IgnoreMouseInterface)
                                {
                                    Main.player[Main.myPlayer].mouseInterface = true;
                                    if (Main.mouseLeftRelease && Main.mouseLeft && !PlayerInput.UsingGamepadUI && Main.mouseItem.type == 0)
                                    {
                                        Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                        main.mouseNPC = num35;
                                        Main.mouseLeftRelease = false;
                                    }
                                }
                            }
                            UILinkPointNavigator.SetPosition(600 + num32, new Vector2((float)num38, (float)num39) + Main.inventoryBackTexture.Size() * 0.75f);
                            Texture2D texture = Main.inventoryBack11Texture;
                            Color white2 = Main.inventoryBack;
                            if (UILinkPointNavigator.CurrentPoint - 600 == num32)
                            {
                                texture = Main.inventoryBack14Texture;
                                white2 = Color.White;
                            }
                            Main.spriteBatch.Draw(texture, new Vector2((float)num38, (float)num39), new Rectangle?(new Rectangle(0, 0, Main.inventoryBackTexture.Width, Main.inventoryBackTexture.Height)), white2, 0f, default(Vector2), Main.inventoryScale, SpriteEffects.None, 0f);
                            white = Color.White;
                            int num40 = num35;
                            float s = 1f;
                            float num41;
                            if (Main.npcHeadTexture[num40].Width > Main.npcHeadTexture[num40].Height)
                            {
                                num41 = (float)Main.npcHeadTexture[num40].Width;
                            }
                            else
                            {
                                num41 = (float)Main.npcHeadTexture[num40].Height;
                            }
                            if (num41 > 36f)
                            {
                                s = 36f / num41;
                            }
                            Main.spriteBatch.Draw(Main.npcHeadTexture[num40], new Vector2((float)num38 + 26f * Main.inventoryScale, (float)num39 + 26f * Main.inventoryScale), new Rectangle?(new Rectangle(0, 0, Main.npcHeadTexture[num40].Width, Main.npcHeadTexture[num40].Height)), white, 0f, new Vector2((float)(Main.npcHeadTexture[num40].Width / 2), (float)(Main.npcHeadTexture[num40].Height / 2)), s, SpriteEffects.None, 0f);
                            num32++;
                        }
                        UILinkPointNavigator.Shortcuts.NPCS_IconsTotal = num32;
                    }
                    if (text != "" && Main.mouseItem.type == 0)
                    {
                        main.MouseText(text, 0, 0, -1, -1, -1, -1);
                    }
                }
                else
                {
                    int num42 = 4;
                    if (Main.mouseX > Main.screenWidth - 64 - 28 && Main.mouseX < (int)((float)(Main.screenWidth - 64 - 28) + 56f * Main.inventoryScale) && Main.mouseY > 174 + (int)mH.GetValue(null) && Main.mouseY < (int)((float)(174 + (int)mH.GetValue(null)) + 448f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    float num43 = Main.inventoryScale;
                    int num44 = 8 + Main.player[Main.myPlayer].extraAccessorySlots;
                    bool flag4 = false;
                    int num45 = num44 - 1;
                    if (num44 == 8 && (Main.player[Main.myPlayer].armor[8].type > 0 || Main.player[Main.myPlayer].armor[18].type > 0 || Main.player[Main.myPlayer].dye[8].type > 0))
                    {
                        num44 = 9;
                        flag4 = true;
                        num45 = 7;
                    }
                    if (Main.screenHeight < 900 && num45 == 8)
                    {
                        num45--;
                    }
                    Color color = Main.inventoryBack;
                    Color color2 = new Color(80, 80, 80, 80);
                    for (int num46 = 0; num46 < num44; num46++)
                    {
                        bool flag5 = false;
                        if (flag4 && num46 == num44 - 1 && Main.mouseItem.type > 0)
                        {
                            flag5 = true;
                        }
                        int num47 = Main.screenWidth - 64 - 28;
                        int num48 = (int)((float)(174 + (int)mH.GetValue(null)) + (float)(num46 * 56) * Main.inventoryScale);
                        new Color(100, 100, 100, 100);
                        if (num46 > 2)
                        {
                            num48 += num42;
                        }
                        if (num46 == num45)
                        {
                            Vector2 vector = new Vector2((float)(num47 - 10 - 47 - 47 - 14), (float)num48 + (float)Main.inventoryBackTexture.Height * 0.5f);
                            Main.spriteBatch.Draw(Main.extraTexture[58], vector, null, Color.White, 0f, Main.extraTexture[58].Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
                            Vector2 value2 = Main.fontMouseText.MeasureString(Main.player[Main.myPlayer].statDefense.ToString());
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, Main.player[Main.myPlayer].statDefense.ToString(), vector - value2 * 0.5f * Main.inventoryScale, Color.White, 0f, Vector2.Zero, new Vector2(Main.inventoryScale), -1f, 2f);
                            if (Utils.CenteredRectangle(vector, Main.extraTexture[58].Size()).Contains(new Point(Main.mouseX, Main.mouseY)) && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.player[Main.myPlayer].mouseInterface = true;
                                string value3 = Main.player[Main.myPlayer].statDefense + " " + Lang.inter[10].Value;
                                if (!string.IsNullOrEmpty(value3))
                                {
                                    Main.hoverItemName = value3;
                                }
                            }
                            UILinkPointNavigator.SetPosition(1557, vector + Main.extraTexture[58].Size() * Main.inventoryScale / 4f);
                        }
                        int context2 = 8;
                        if (num46 > 2)
                        {
                            context2 = 10;
                        }
                        Texture2D texture2D2 = Main.inventoryTickOnTexture;
                        if (Main.player[Main.myPlayer].hideVisual[num46])
                        {
                            texture2D2 = Main.inventoryTickOffTexture;
                        }
                        int num49 = Main.screenWidth - 58;
                        int num50 = (int)((float)(172 + (int)mH.GetValue(null)) + (float)(num46 * 56) * Main.inventoryScale);
                        if (num46 > 2)
                        {
                            num50 += num42;
                        }
                        Rectangle rectangle = new Rectangle(num49, num50, texture2D2.Width, texture2D2.Height);
                        int num51 = 0;
                        if (num46 >= 3 && num46 < num44 && rectangle.Contains(new Point(Main.mouseX, Main.mouseY)) && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            if (Main.mouseLeft && Main.mouseLeftRelease)
                            {
                                Main.player[Main.myPlayer].hideVisual[num46] = !Main.player[Main.myPlayer].hideVisual[num46];
                                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                if (Main.netMode == 1)
                                {
                                    NetMessage.SendData(4, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                            if (Main.player[Main.myPlayer].hideVisual[num46])
                            {
                                num51 = 2;
                            }
                            else
                            {
                                num51 = 1;
                            }
                        }
                        else if (Main.mouseX >= num47 && (float)Main.mouseX <= (float)num47 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num48 && (float)Main.mouseY <= (float)num48 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.armorHide = true;
                            Main.player[Main.myPlayer].mouseInterface = true;
                            ItemSlot.OverrideHover(Main.player[Main.myPlayer].armor, context2, num46);
                            if (!flag5 && Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                ItemSlot.LeftClick(Main.player[Main.myPlayer].armor, context2, num46);
                            }
                            ItemSlot.MouseHover(Main.player[Main.myPlayer].armor, context2, num46);
                        }
                        if (flag4 && num46 == num44 - 1)
                        {
                            Main.inventoryBack = color2;
                        }
                        ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].armor, context2, num46, new Vector2((float)num47, (float)num48), default(Color));
                        if (num46 > 2 && num46 < num44)
                        {
                            Main.spriteBatch.Draw(texture2D2, new Vector2((float)num49, (float)num50), Color.White * 0.7f);
                            if (num51 > 0)
                            {
                                Main.HoverItem = new Item();
                                Main.hoverItemName = Lang.inter[58 + num51].Value;
                            }
                        }
                    }
                    Main.inventoryBack = color;
                    if (Main.mouseX > Main.screenWidth - 64 - 28 - 47 && Main.mouseX < (int)((float)(Main.screenWidth - 64 - 20 - 47) + 56f * Main.inventoryScale) && Main.mouseY > 174 + (int)mH.GetValue(null) && Main.mouseY < (int)((float)(174 + (int)mH.GetValue(null)) + 168f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    for (int num52 = 10; num52 < 10 + num44; num52++)
                    {
                        bool flag6 = false;
                        if (flag4 && num52 == 10 + num44 - 1 && Main.mouseItem.type > 0)
                        {
                            flag6 = true;
                        }
                        int num53 = Main.screenWidth - 64 - 28 - 47;
                        int num54 = (int)((float)(174 + (int)mH.GetValue(null)) + (float)((num52 - 10) * 56) * Main.inventoryScale);
                        new Color(100, 100, 100, 100);
                        if (num52 > 12)
                        {
                            num54 += num42;
                        }
                        int context3 = 9;
                        if (num52 > 12)
                        {
                            context3 = 11;
                        }
                        if (Main.mouseX >= num53 && (float)Main.mouseX <= (float)num53 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num54 && (float)Main.mouseY <= (float)num54 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            Main.armorHide = true;
                            ItemSlot.OverrideHover(Main.player[Main.myPlayer].armor, context3, num52);
                            if (!flag6)
                            {
                                if (Main.mouseLeftRelease && Main.mouseLeft)
                                {
                                    ItemSlot.LeftClick(Main.player[Main.myPlayer].armor, context3, num52);
                                }
                                else
                                {
                                    ItemSlot.RightClick(Main.player[Main.myPlayer].armor, context3, num52);
                                }
                            }
                            ItemSlot.MouseHover(Main.player[Main.myPlayer].armor, context3, num52);
                        }
                        if (flag4 && num52 == num44 + 10 - 1)
                        {
                            Main.inventoryBack = color2;
                        }
                        ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].armor, context3, num52, new Vector2((float)num53, (float)num54), default(Color));
                    }
                    Main.inventoryBack = color;
                    if (Main.mouseX > Main.screenWidth - 64 - 28 - 47 && Main.mouseX < (int)((float)(Main.screenWidth - 64 - 20 - 47) + 56f * Main.inventoryScale) && Main.mouseY > 174 + (int)mH.GetValue(null) && Main.mouseY < (int)((float)(174 + (int)mH.GetValue(null)) + 168f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    for (int num55 = 0; num55 < num44; num55++)
                    {
                        bool flag7 = false;
                        if (flag4 && num55 == num44 - 1 && Main.mouseItem.type > 0)
                        {
                            flag7 = true;
                        }
                        int num56 = Main.screenWidth - 64 - 28 - 47 - 47;
                        int num57 = (int)((float)(174 + (int)mH.GetValue(null)) + (float)(num55 * 56) * Main.inventoryScale);
                        new Color(100, 100, 100, 100);
                        if (num55 > 2)
                        {
                            num57 += num42;
                        }
                        if (Main.mouseX >= num56 && (float)Main.mouseX <= (float)num56 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num57 && (float)Main.mouseY <= (float)num57 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            Main.armorHide = true;
                            ItemSlot.OverrideHover(Main.player[Main.myPlayer].dye, 12, num55);
                            if (!flag7)
                            {
                                if (Main.mouseRightRelease && Main.mouseRight)
                                {
                                    ItemSlot.RightClick(Main.player[Main.myPlayer].dye, 12, num55);
                                }
                                else if (Main.mouseLeftRelease && Main.mouseLeft)
                                {
                                    ItemSlot.LeftClick(Main.player[Main.myPlayer].dye, 12, num55);
                                }
                            }
                            ItemSlot.MouseHover(Main.player[Main.myPlayer].dye, 12, num55);
                        }
                        if (flag4 && num55 == num44 - 1)
                        {
                            Main.inventoryBack = color2;
                        }
                        ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].dye, 12, num55, new Vector2((float)num56, (float)num57), default(Color));
                    }
                    Main.inventoryBack = color;
                    Main.inventoryScale = num43;
                }
                int num58 = (Main.screenHeight - 600) / 2;
                int num59 = (int)((float)Main.screenHeight / 600f * 250f);
                if (Main.screenHeight < 700)
                {
                    num58 = (Main.screenHeight - 508) / 2;
                    num59 = (int)((float)Main.screenHeight / 600f * 200f);
                }
                else if (Main.screenHeight < 850)
                {
                    num59 = (int)((float)Main.screenHeight / 600f * 225f);
                }
                if (Main.craftingHide)
                {
                    Main.craftingAlpha -= 0.1f;
                    if (Main.craftingAlpha < 0f)
                    {
                        Main.craftingAlpha = 0f;
                    }
                }
                else
                {
                    Main.craftingAlpha += 0.025f;
                    if (Main.craftingAlpha > 1f)
                    {
                        Main.craftingAlpha = 1f;
                    }
                }
                Color color3 = new Color((int)((byte)((float)Main.mouseTextColor * Main.craftingAlpha)), (int)((byte)((float)Main.mouseTextColor * Main.craftingAlpha)), (int)((byte)((float)Main.mouseTextColor * Main.craftingAlpha)), (int)((byte)((float)Main.mouseTextColor * Main.craftingAlpha)));
                Main.craftingHide = false;
                if (Main.InReforgeMenu)
                {
                    if ((bool)mouseReforge.GetValue(null))
                    {
                        if ((float)reforgeScale.GetValue(null) < 1f)
                        {
                            reforgeScale.SetValue(null, (float)reforgeScale.GetValue(null) + 0.02f);
                        }
                    }
                    else if ((float)reforgeScale.GetValue(null) > 1f)
                    {
                        reforgeScale.SetValue(null, (float)reforgeScale.GetValue(null) - 0.02f);
                    }
                    if (Main.player[Main.myPlayer].chest != -1 || Main.npcShop != 0 || Main.player[Main.myPlayer].talkNPC == -1 || Main.InGuideCraftMenu)
                    {
                        Main.InReforgeMenu = false;
                        Main.player[Main.myPlayer].dropItemCheck();
                        API.FindRecipes();
                    }
                    else
                    {
                        int num60 = 50;
                        int num61 = 448;
                        string text2 = Lang.inter[46].Value + ": ";
                        if (Main.reforgeItem.type > 0)
                        {
                            int num62 = Main.reforgeItem.value;
                            if (Main.player[Main.myPlayer].discount)
                            {
                                num62 = (int)((double)num62 * 0.8);
                            }
                            num62 /= 3;
                            string text3 = "";
                            int num63 = 0;
                            int num64 = 0;
                            int num65 = 0;
                            int num66 = 0;
                            int num67 = num62;
                            if (num67 < 1)
                            {
                                num67 = 1;
                            }
                            if (num67 >= 1000000)
                            {
                                num63 = num67 / 1000000;
                                num67 -= num63 * 1000000;
                            }
                            if (num67 >= 10000)
                            {
                                num64 = num67 / 10000;
                                num67 -= num64 * 10000;
                            }
                            if (num67 >= 100)
                            {
                                num65 = num67 / 100;
                                num67 -= num65 * 100;
                            }
                            if (num67 >= 1)
                            {
                                num66 = num67;
                            }
                            if (num63 > 0)
                            {
                                object obj = text3;
                                text3 = string.Concat(new object[]
                                {
                        obj,
                        "[c/",
                        Colors.AlphaDarken(Colors.CoinPlatinum).Hex3(),
                        ":",
                        num63,
                        " ",
                        Lang.inter[15].Value,
                        "] "
                                });
                            }
                            if (num64 > 0)
                            {
                                object obj2 = text3;
                                text3 = string.Concat(new object[]
                                {
                        obj2,
                        "[c/",
                        Colors.AlphaDarken(Colors.CoinGold).Hex3(),
                        ":",
                        num64,
                        " ",
                        Lang.inter[16].Value,
                        "] "
                                });
                            }
                            if (num65 > 0)
                            {
                                object obj3 = text3;
                                text3 = string.Concat(new object[]
                                {
                        obj3,
                        "[c/",
                        Colors.AlphaDarken(Colors.CoinSilver).Hex3(),
                        ":",
                        num65,
                        " ",
                        Lang.inter[17].Value,
                        "] "
                                });
                            }
                            if (num66 > 0)
                            {
                                object obj4 = text3;
                                text3 = string.Concat(new object[]
                                {
                        obj4,
                        "[c/",
                        Colors.AlphaDarken(Colors.CoinCopper).Hex3(),
                        ":",
                        num66,
                        " ",
                        Lang.inter[18].Value,
                        "] "
                                });
                            }
                            ItemSlot.DrawSavings(Main.spriteBatch, (float)(num60 + 130), (float)main.invBottom, true);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text3, new Vector2((float)(num60 + 50) + Main.fontMouseText.MeasureString(text2).X, (float)num61), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                            int num68 = num60 + 70;
                            int num69 = num61 + 40;
                            bool flag8 = Main.mouseX > num68 - 15 && Main.mouseX < num68 + 15 && Main.mouseY > num69 - 15 && Main.mouseY < num69 + 15 && !PlayerInput.IgnoreMouseInterface;
                            Texture2D texture2D3 = Main.reforgeTexture[0];
                            if (flag8)
                            {
                                texture2D3 = Main.reforgeTexture[1];
                            }
                            Main.spriteBatch.Draw(texture2D3, new Vector2((float)num68, (float)num69), null, Color.White, 0f, texture2D3.Size() / 2f, (float)reforgeScale.GetValue(null), SpriteEffects.None, 0f);
                            UILinkPointNavigator.SetPosition(304, new Vector2((float)num68, (float)num69) + texture2D3.Size() / 4f);
                            if (flag8)
                            {
                                Main.hoverItemName = Lang.inter[19].Value;
                                if (!(bool)mouseReforge.GetValue(null))
                                {
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                }
                                mouseReforge.SetValue(null, true);
                                Main.player[Main.myPlayer].mouseInterface = true;
                                if (Main.mouseLeftRelease && Main.mouseLeft && Main.player[Main.myPlayer].CanBuyItem(num62, -1))
                                {
                                    ItemLoader.PreReforge(Main.reforgeItem);
                                    Main.reforgeItem.position.X = Main.player[Main.myPlayer].position.X + (float)(Main.player[Main.myPlayer].width / 2) - (float)(Main.reforgeItem.width / 2);
                                    Main.reforgeItem.position.Y = Main.player[Main.myPlayer].position.Y + (float)(Main.player[Main.myPlayer].height / 2) - (float)(Main.reforgeItem.height / 2);
                                    ItemText.NewText(Main.reforgeItem, Main.reforgeItem.stack, true, false);
                                    Main.PlaySound(SoundID.Item37, -1, -1);
                                }
                            }
                            else
                            {
                                mouseReforge.SetValue(null, false);
                            }
                        }
                        else
                        {
                            text2 = Lang.inter[20].Value;
                        }
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text2, new Vector2((float)(num60 + 50), (float)num61), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                        if (Main.mouseX >= num60 && (float)Main.mouseX <= (float)num60 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num61 && (float)Main.mouseY <= (float)num61 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            Main.craftingHide = true;
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                ItemSlot.LeftClick(ref Main.reforgeItem, 5);
                                API.FindRecipes();
                            }
                            else
                            {
                                ItemSlot.RightClick(ref Main.reforgeItem, 5);
                            }
                            ItemSlot.MouseHover(ref Main.reforgeItem, 5);
                        }
                        ItemSlot.Draw(Main.spriteBatch, ref Main.reforgeItem, 5, new Vector2((float)num60, (float)num61), default(Color));
                    }
                }
                else if (Main.InGuideCraftMenu)
                {
                    if (Main.player[Main.myPlayer].chest != -1 || Main.npcShop != 0 || Main.player[Main.myPlayer].talkNPC == -1 || Main.InReforgeMenu)
                    {
                        Main.InGuideCraftMenu = false;
                        Main.player[Main.myPlayer].dropItemCheck();
                        API.FindRecipes();
                    }
                    else
                    {
                        int num70 = 73;
                        int num71 = 331;
                        num71 += num58;
                        string text4;
                        if (Main.guideItem.type > 0)
                        {
                            text4 = Lang.inter[21].Value + " " + Main.guideItem.Name;
                            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[22].Value, new Vector2((float)num70, (float)(num71 + 118)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            int num72 = Main.focusRecipe;
                            int num73 = 0;
                            int num74 = 0;
                            while (num74 < Recipe.maxRequirements)
                            {
                                int num75 = (num74 + 1) * 26;
                                if (Main.recipe[Main.availableRecipe[num72]].requiredTile[num74] == -1)
                                {
                                    if (num74 == 0 && !Main.recipe[Main.availableRecipe[num72]].needWater && !Main.recipe[Main.availableRecipe[num72]].needHoney && !Main.recipe[Main.availableRecipe[num72]].needLava && !Main.recipe[Main.availableRecipe[num72]].needSnowBiome)
                                    {
                                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[23].Value, new Vector2((float)num70, (float)(num71 + 118 + num75)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                                        break;
                                    }
                                    break;
                                }
                                else
                                {
                                    num73++;
                                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.GetMapObjectName(MapHelper.TileToLookup(Main.recipe[Main.availableRecipe[num72]].requiredTile[num74], 0)), new Vector2((float)num70, (float)(num71 + 118 + num75)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                                    num74++;
                                }
                            }
                            if (Main.recipe[Main.availableRecipe[num72]].needWater)
                            {
                                int num76 = (num73 + 1) * 26;
                                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[53].Value, new Vector2((float)num70, (float)(num71 + 118 + num76)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            }
                            if (Main.recipe[Main.availableRecipe[num72]].needHoney)
                            {
                                int num77 = (num73 + 1) * 26;
                                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[58].Value, new Vector2((float)num70, (float)(num71 + 118 + num77)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            }
                            if (Main.recipe[Main.availableRecipe[num72]].needLava)
                            {
                                int num78 = (num73 + 1) * 26;
                                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[56].Value, new Vector2((float)num70, (float)(num71 + 118 + num78)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            }
                            if (Main.recipe[Main.availableRecipe[num72]].needSnowBiome)
                            {
                                int num79 = (num73 + 1) * 26;
                                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[123].Value, new Vector2((float)num70, (float)(num71 + 118 + num79)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            }
                        }
                        else
                        {
                            text4 = Lang.inter[24].Value;
                        }
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, text4, new Vector2((float)(num70 + 50), (float)(num71 + 12)), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        new Color(100, 100, 100, 100);
                        if (Main.mouseX >= num70 && (float)Main.mouseX <= (float)num70 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num71 && (float)Main.mouseY <= (float)num71 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            Main.craftingHide = true;
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                ItemSlot.LeftClick(ref Main.guideItem, 7);
                                API.FindRecipes();
                            }
                            else
                            {
                                ItemSlot.RightClick(ref Main.guideItem, 7);
                            }
                            ItemSlot.MouseHover(ref Main.guideItem, 7);
                        }
                        ItemSlot.Draw(Main.spriteBatch, ref Main.guideItem, 7, new Vector2((float)num70, (float)num71), default(Color));
                    }
                }
                if (!Main.InReforgeMenu)
                {
                    UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig = -1;
                    UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall = -1;
                    if (Main.numAvailableRecipes > 0)
                    {
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[25].Value, new Vector2(76f, (float)(414 + num58)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                    for (int num80 = 0; num80 < Recipe.maxRecipes; num80++)
                    {
                        Main.inventoryScale = 100f / (Math.Abs(Main.availableRecipeY[num80]) + 100f);
                        if ((double)Main.inventoryScale < 0.75)
                        {
                            Main.inventoryScale = 0.75f;
                        }
                        if (Main.recFastScroll)
                        {
                            Main.inventoryScale = 0.75f;
                        }
                        if (Main.availableRecipeY[num80] < (float)((num80 - Main.focusRecipe) * 65))
                        {
                            if (Main.availableRecipeY[num80] == 0f && !Main.recFastScroll)
                            {
                                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                            }
                            Main.availableRecipeY[num80] += 6.5f;
                            if (Main.recFastScroll)
                            {
                                Main.availableRecipeY[num80] += 130000f;
                            }
                            if (Main.availableRecipeY[num80] > (float)((num80 - Main.focusRecipe) * 65))
                            {
                                Main.availableRecipeY[num80] = (float)((num80 - Main.focusRecipe) * 65);
                            }
                        }
                        else if (Main.availableRecipeY[num80] > (float)((num80 - Main.focusRecipe) * 65))
                        {
                            if (Main.availableRecipeY[num80] == 0f && !Main.recFastScroll)
                            {
                                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                            }
                            Main.availableRecipeY[num80] -= 6.5f;
                            if (Main.recFastScroll)
                            {
                                Main.availableRecipeY[num80] -= 130000f;
                            }
                            if (Main.availableRecipeY[num80] < (float)((num80 - Main.focusRecipe) * 65))
                            {
                                Main.availableRecipeY[num80] = (float)((num80 - Main.focusRecipe) * 65);
                            }
                        }
                        else
                        {
                            Main.recFastScroll = false;
                        }
                        if (num80 < Main.numAvailableRecipes && Math.Abs(Main.availableRecipeY[num80]) <= (float)num59)
                        {
                            int num81 = (int)(46f - 26f * Main.inventoryScale);
                            int num82 = (int)(410f + Main.availableRecipeY[num80] * Main.inventoryScale - 30f * Main.inventoryScale + (float)num58);
                            double num83 = (double)(Main.inventoryBack.A + 50);
                            double num84 = 255.0;
                            if (Math.Abs(Main.availableRecipeY[num80]) > (float)num59 - 100f)
                            {
                                num83 = (double)(150f * (100f - (Math.Abs(Main.availableRecipeY[num80]) - ((float)num59 - 100f)))) * 0.01;
                                num84 = (double)(255f * (100f - (Math.Abs(Main.availableRecipeY[num80]) - ((float)num59 - 100f)))) * 0.01;
                            }
                            new Color((int)((byte)num83), (int)((byte)num83), (int)((byte)num83), (int)((byte)num83));
                            Color lightColor = new Color((int)((byte)num84), (int)((byte)num84), (int)((byte)num84), (int)((byte)num84));
                            if (Main.mouseX >= num81 && (float)Main.mouseX <= (float)num81 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num82 && (float)Main.mouseY <= (float)num82 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.player[Main.myPlayer].mouseInterface = true;
                                if (Main.focusRecipe == num80 && Main.guideItem.type == 0 && Main.LocalPlayer.itemTime == 0 && Main.LocalPlayer.itemAnimation == 0)
                                {
                                    if ((Main.mouseItem.type == 0 || (Main.mouseItem.IsTheSameAs(Main.recipe[Main.availableRecipe[num80]].createItem) && Main.mouseItem.stack + Main.recipe[Main.availableRecipe[num80]].createItem.stack <= Main.mouseItem.maxStack)) && !Main.player[Main.myPlayer].IsStackingItems())
                                    {
                                        if (Main.mouseLeftRelease && Main.mouseLeft)
                                        {
                                            API.CraftItem(Main.recipe[Main.availableRecipe[num80]]);
                                        }
                                        else if (Main.stackSplit <= 1 && Main.mouseRight && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0))
                                        {
                                            if (Main.stackSplit == 0)
                                            {
                                                Main.stackSplit = 15;
                                            }
                                            else
                                            {
                                                Main.stackSplit = Main.stackDelay;
                                            }
                                            API.CraftItem(Main.recipe[Main.availableRecipe[num80]]);
                                        }
                                    }
                                }
                                else if (Main.mouseLeftRelease && Main.mouseLeft)
                                {
                                    Main.focusRecipe = num80;
                                }
                                Main.craftingHide = true;
                                Main.hoverItemName = Main.recipe[Main.availableRecipe[num80]].createItem.Name;
                                Main.HoverItem = Main.recipe[Main.availableRecipe[num80]].createItem.Clone();
                                if (Main.recipe[Main.availableRecipe[num80]].createItem.stack > 1)
                                {
                                    object obj5 = Main.hoverItemName;
                                    Main.hoverItemName = string.Concat(new object[]
                                    {
                            obj5,
                            " (",
                            Main.recipe[Main.availableRecipe[num80]].createItem.stack,
                            ")"
                                    });
                                }
                            }
                            if (Main.numAvailableRecipes > 0)
                            {
                                num83 -= 50.0;
                                if (num83 < 0.0)
                                {
                                    num83 = 0.0;
                                }
                                if (num80 == Main.focusRecipe)
                                {
                                    UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall = 0;
                                }
                                else
                                {
                                    UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall = -1;
                                }
                                Color color4 = Main.inventoryBack;
                                Main.inventoryBack = new Color((int)((byte)num83), (int)((byte)num83), (int)((byte)num83), (int)((byte)num83));
                                ItemSlot.Draw(Main.spriteBatch, ref Main.recipe[Main.availableRecipe[num80]].createItem, 22, new Vector2((float)num81, (float)num82), lightColor);
                                Main.inventoryBack = color4;
                            }
                        }
                    }
                    if (Main.numAvailableRecipes > 0)
                    {
                        UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig = -1;
                        UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall = -1;
                        for (int num85 = 0; num85 < Recipe.maxRequirements; num85++)
                        {
                            if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type == 0)
                            {
                                UILinkPointNavigator.Shortcuts.CRAFT_CurrentIngridientsCount = num85 + 1;
                                break;
                            }
                            int num86 = 80 + num85 * 40;
                            int num87 = 380 + num58;
                            double num88 = (double)(Main.inventoryBack.A + 50);
                            Color white3 = Color.White;
                            Color white4 = Color.White;
                            num88 = (double)((float)(Main.inventoryBack.A + 50) - Math.Abs(Main.availableRecipeY[Main.focusRecipe]) * 2f);
                            double num89 = (double)(255f - Math.Abs(Main.availableRecipeY[Main.focusRecipe]) * 2f);
                            if (num88 < 0.0)
                            {
                                num88 = 0.0;
                            }
                            if (num89 < 0.0)
                            {
                                num89 = 0.0;
                            }
                            white3.R = (byte)num88;
                            white3.G = (byte)num88;
                            white3.B = (byte)num88;
                            white3.A = (byte)num88;
                            white4.R = (byte)num89;
                            white4.G = (byte)num89;
                            white4.B = (byte)num89;
                            white4.A = (byte)num89;
                            Main.inventoryScale = 0.6f;
                            if (num88 == 0.0)
                            {
                                break;
                            }
                            if (Main.mouseX >= num86 && (float)Main.mouseX <= (float)num86 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num87 && (float)Main.mouseY <= (float)num87 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.craftingHide = true;
                                Main.player[Main.myPlayer].mouseInterface = true;
                                Main.hoverItemName = Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].Name;
                                Main.HoverItem = Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].Clone();
                                string nameOverride;
                                if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].ProcessGroupsForText(Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type, out nameOverride))
                                {
                                    Main.HoverItem.SetNameOverride(nameOverride);
                                }
                                if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].anyIronBar && Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type == 22)
                                {
                                    Main.HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.GetItemNameValue(22));
                                }
                                else if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].anyWood && Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type == 9)
                                {
                                    Main.HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.GetItemNameValue(9));
                                }
                                else if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].anySand && Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type == 169)
                                {
                                    Main.HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.GetItemNameValue(169));
                                }
                                else if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].anyFragment && Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type == 3458)
                                {
                                    Main.HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.misc[51].Value);
                                }
                                else if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].anyPressurePlate && Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].type == 542)
                                {
                                    Main.HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.misc[38].Value);
                                }
                                if (Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].stack > 1)
                                {
                                    object obj6 = Main.hoverItemName;
                                    Main.hoverItemName = string.Concat(new object[]
                                    {
                            obj6,
                            " (",
                            Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85].stack,
                            ")"
                                    });
                                }
                            }
                            num88 -= 50.0;
                            if (num88 < 0.0)
                            {
                                num88 = 0.0;
                            }
                            UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall = 1 + num85;
                            Color color5 = Main.inventoryBack;
                            Main.inventoryBack = new Color((int)((byte)num88), (int)((byte)num88), (int)((byte)num88), (int)((byte)num88));
                            ItemSlot.Draw(Main.spriteBatch, ref Main.recipe[Main.availableRecipe[Main.focusRecipe]].requiredItem[num85], 22, new Vector2((float)num86, (float)num87), default(Color));
                            Main.inventoryBack = color5;
                        }
                    }
                    if (Main.numAvailableRecipes == 0)
                    {
                        Main.recBigList = false;
                    }
                    else
                    {
                        int num90 = 94;
                        int num91 = 450 + num58;
                        if (Main.InGuideCraftMenu)
                        {
                            num91 -= 150;
                        }
                        bool flag9 = Main.mouseX > num90 - 15 && Main.mouseX < num90 + 15 && Main.mouseY > num91 - 15 && Main.mouseY < num91 + 15 && !PlayerInput.IgnoreMouseInterface;
                        int num92 = Main.recBigList.ToInt() * 2 + flag9.ToInt();
                        Main.spriteBatch.Draw(Main.craftToggleTexture[num92], new Vector2((float)num90, (float)num91), null, Color.White, 0f, Main.craftToggleTexture[num92].Size() / 2f, 1f, SpriteEffects.None, 0f);
                        if (flag9)
                        {
                            main.MouseText(Language.GetTextValue("GameUI.CraftingWindow"), 0, 0, -1, -1, -1, -1);
                            Main.player[Main.myPlayer].mouseInterface = true;
                            if (Main.mouseLeft && Main.mouseLeftRelease)
                            {
                                if (!Main.recBigList)
                                {
                                    Main.recBigList = true;
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                }
                                else
                                {
                                    Main.recBigList = false;
                                    Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                }
                            }
                        }
                    }
                }
                if (Main.recBigList)
                {
                    UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig = -1;
                    UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall = -1;
                    int num93 = 42;
                    if ((double)Main.inventoryScale < 0.75)
                    {
                        Main.inventoryScale = 0.75f;
                    }
                    int num94 = Main.instance.invBottom + 48; //340;
                    int num95 = 310;
                    int num96 = (Main.screenWidth - num95 - 280) / num93;
                    int num97 = (Main.screenHeight - num94 - 20) / num93;
                    UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow = num96;
                    UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn = num97;
                    int num98 = 0;
                    int num99 = 0;
                    int num100 = num95;
                    int num101 = num94;
                    int num102 = num95 - 20;
                    int num103 = num94 + 2;
                    if (Main.recStart > Main.numAvailableRecipes - num96 * num97)
                    {
                        Main.recStart = Main.numAvailableRecipes - num96 * num97;
                        if (Main.recStart < 0)
                        {
                            Main.recStart = 0;
                        }
                    }
                    if (Main.recStart > 0)
                    {
                        if (Main.mouseX >= num102 && Main.mouseX <= num102 + Main.craftUpButtonTexture.Width && Main.mouseY >= num103 && Main.mouseY <= num103 + Main.craftUpButtonTexture.Height && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                Main.recStart -= num96;
                                if (Main.recStart < 0)
                                {
                                    Main.recStart = 0;
                                }
                                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                Main.mouseLeftRelease = false;
                            }
                        }
                        Main.spriteBatch.Draw(Main.craftUpButtonTexture, new Vector2((float)num102, (float)num103), new Rectangle?(new Rectangle(0, 0, Main.craftUpButtonTexture.Width, Main.craftUpButtonTexture.Height)), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                    if (Main.recStart < Main.numAvailableRecipes - num96 * num97)
                    {
                        num103 += 20;
                        if (Main.mouseX >= num102 && Main.mouseX <= num102 + Main.craftUpButtonTexture.Width && Main.mouseY >= num103 && Main.mouseY <= num103 + Main.craftUpButtonTexture.Height && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                Main.recStart += num96;
                                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                if (Main.recStart > Main.numAvailableRecipes - num96)
                                {
                                    Main.recStart = Main.numAvailableRecipes - num96;
                                }
                                Main.mouseLeftRelease = false;
                            }
                        }
                        Main.spriteBatch.Draw(Main.craftDownButtonTexture, new Vector2((float)num102, (float)num103), new Rectangle?(new Rectangle(0, 0, Main.craftUpButtonTexture.Width, Main.craftUpButtonTexture.Height)), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                    int num104 = Main.recStart;
                    while (num104 < Recipe.maxRecipes && num104 < Main.numAvailableRecipes)
                    {
                        int num105 = num100;
                        int num106 = num101;
                        double num107 = (double)(Main.inventoryBack.A + 50);
                        double num108 = 255.0;
                        new Color((int)((byte)num107), (int)((byte)num107), (int)((byte)num107), (int)((byte)num107));
                        new Color((int)((byte)num108), (int)((byte)num108), (int)((byte)num108), (int)((byte)num108));
                        if (Main.mouseX >= num105 && (float)Main.mouseX <= (float)num105 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num106 && (float)Main.mouseY <= (float)num106 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                Main.focusRecipe = num104;
                                Main.recFastScroll = true;
                                Main.recBigList = false;
                                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                                Main.mouseLeftRelease = false;
                                if (PlayerInput.UsingGamepadUI)
                                {
                                    UILinkPointNavigator.ChangePage(9);
                                }
                            }
                            Main.craftingHide = true;
                            Main.hoverItemName = Main.recipe[Main.availableRecipe[num104]].createItem.Name;
                            Main.HoverItem = Main.recipe[Main.availableRecipe[num104]].createItem.Clone();
                            if (Main.recipe[Main.availableRecipe[num104]].createItem.stack > 1)
                            {
                                object obj7 = Main.hoverItemName;
                                Main.hoverItemName = string.Concat(new object[]
                                {
                        obj7,
                        " (",
                        Main.recipe[Main.availableRecipe[num104]].createItem.stack,
                        ")"
                                });
                            }
                        }
                        if (Main.numAvailableRecipes > 0)
                        {
                            num107 -= 50.0;
                            if (num107 < 0.0)
                            {
                                num107 = 0.0;
                            }
                            UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig = num104 - Main.recStart;
                            Color color6 = Main.inventoryBack;
                            Main.inventoryBack = new Color((int)((byte)num107), (int)((byte)num107), (int)((byte)num107), (int)((byte)num107));
                            ItemSlot.Draw(Main.spriteBatch, ref Main.recipe[Main.availableRecipe[num104]].createItem, 22, new Vector2((float)num105, (float)num106), default(Color));
                            Main.inventoryBack = color6;
                        }
                        num100 += num93;
                        num98++;
                        if (num98 >= num96)
                        {
                            num100 = num95;
                            num101 += num93;
                            num98 = 0;
                            num99++;
                            if (num99 >= num97)
                            {
                                break;
                            }
                        }
                        num104++;
                    }
                }
                Vector2 vector2 = Main.fontMouseText.MeasureString("Coins");
                Vector2 vector3 = Main.fontMouseText.MeasureString(Lang.inter[26].Value);
                float num109 = vector2.X / vector3.X;
                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[26].Value, new Vector2(39 + (int)(600 * scale), 140f + (vector2.Y - vector2.Y * num109) / 2f), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 0.75f * num109, SpriteEffects.None, 0f);
                Main.inventoryScale = 0.6f;
                for (int num110 = 0; num110 < 4; num110++)
                {
                    int num111 = 40 + (int)(600 * scale);
                    int num112 = (int)(140f + (float)(num110 * 56) * Main.inventoryScale + 20f);
                    int slot = num110 + 50;
                    new Color(100, 100, 100, 100);
                    if (Main.mouseX >= num111 && (float)Main.mouseX <= (float)num111 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num112 && (float)Main.mouseY <= (float)num112 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                        ItemSlot.OverrideHover(Main.player[Main.myPlayer].inventory, 1, slot);
                        if (Main.mouseLeftRelease && Main.mouseLeft)
                        {
                            ItemSlot.LeftClick(Main.player[Main.myPlayer].inventory, 1, slot);
                            API.FindRecipes();
                        }
                        else
                        {
                            ItemSlot.RightClick(Main.player[Main.myPlayer].inventory, 1, slot);
                        }
                        ItemSlot.MouseHover(Main.player[Main.myPlayer].inventory, 1, slot);
                    }
                    ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 1, slot, new Vector2((float)num111, (float)num112), default(Color));
                }
                Vector2 vector4 = Main.fontMouseText.MeasureString("Ammo");
                Vector2 vector5 = Main.fontMouseText.MeasureString(Lang.inter[27].Value);
                float num113 = vector4.X / vector5.X;
                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[27].Value, new Vector2(76 + (int)(600 * scale), 140f + (vector4.Y - vector4.Y * num113) / 2f), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 0.75f * num113, SpriteEffects.None, 0f);
                Main.inventoryScale = 0.6f;
                for (int num114 = 0; num114 < 4; num114++)
                {
                    int num115 = 78 + (int)(600 * scale);
                    int num116 = (int)(140f + (float)(num114 * 56) * Main.inventoryScale + 20f); //was 85
                    int slot2 = 54 + num114;
                    new Color(100, 100, 100, 100);
                    if (Main.mouseX >= num115 && (float)Main.mouseX <= (float)num115 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num116 && (float)Main.mouseY <= (float)num116 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                        ItemSlot.OverrideHover(Main.player[Main.myPlayer].inventory, 2, slot2);
                        if (Main.mouseLeftRelease && Main.mouseLeft)
                        {
                            ItemSlot.LeftClick(Main.player[Main.myPlayer].inventory, 2, slot2);
                            API.FindRecipes();
                        }
                        else
                        {
                            ItemSlot.RightClick(Main.player[Main.myPlayer].inventory, 2, slot2);
                        }
                        ItemSlot.MouseHover(Main.player[Main.myPlayer].inventory, 2, slot2);
                    }
                    ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 2, slot2, new Vector2((float)num115, (float)num116), default(Color));
                }
                if (Main.npcShop > 0 && (!Main.playerInventory || Main.player[Main.myPlayer].talkNPC == -1))
                {
                    Main.npcShop = 0;
                }
                if (Main.npcShop > 0 && !Main.recBigList)
                {
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, Lang.inter[28].Value, 504f, (float)main.invBottom, Color.White * ((float)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
                    ItemSlot.DrawSavings(Main.spriteBatch, 504f, (float)main.invBottom, false);
                    Main.inventoryScale = 0.755f;
                    if (Main.mouseX > 73 && Main.mouseX < (int)(73f + 560f * Main.inventoryScale) && Main.mouseY > main.invBottom && Main.mouseY < (int)((float)main.invBottom + 224f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    for (int num117 = 0; num117 < 10; num117++)
                    {
                        for (int num118 = 0; num118 < 4; num118++)
                        {
                            int num119 = (int)(80f + (float)(num117 * 56) * Main.inventoryScale);
                            int num120 = (int)((float)main.invBottom + (float)(num118 * 56) * Main.inventoryScale);
                            int slot3 = num117 + num118 * 10;
                            new Color(100, 100, 100, 100);
                            if (Main.mouseX >= num119 && (float)Main.mouseX <= (float)num119 + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= num120 && (float)Main.mouseY <= (float)num120 + (float)Main.inventoryBackTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface)
                            {
                                Main.player[Main.myPlayer].mouseInterface = true;
                                if (Main.mouseLeftRelease && Main.mouseLeft)
                                {
                                    ItemSlot.LeftClick(main.shop[Main.npcShop].item, 15, slot3);
                                }
                                else
                                {
                                    ItemSlot.RightClick(main.shop[Main.npcShop].item, 15, slot3);
                                }
                                ItemSlot.MouseHover(main.shop[Main.npcShop].item, 15, slot3);
                            }
                            ItemSlot.Draw(Main.spriteBatch, main.shop[Main.npcShop].item, 15, slot3, new Vector2((float)num119, (float)num120), default(Color));
                        }
                    }
                }
                if (Main.player[Main.myPlayer].chest > -1 && !Main.tileContainer[(int)Main.tile[Main.player[Main.myPlayer].chestX, Main.player[Main.myPlayer].chestY].type])
                {
                    Main.player[Main.myPlayer].chest = -1;
                    API.FindRecipes();
                }
                int offsetDown = 0;
                if (!PlayerInput.UsingGamepad)
                {
                    offsetDown = 9999;
                }
                UIVirtualKeyboard.OffsetDown = offsetDown;
                ChestUI.Draw(Main.spriteBatch);
                if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
                {
                    int num121 = 0;
                    int num122 = 498;
                    int num123 = (int)Math.Round(414f * scale) + 34;
                    int width = Main.chestStackTexture[num121].Width;
                    int height = Main.chestStackTexture[num121].Height;
                    UILinkPointNavigator.SetPosition(301, new Vector2((float)num122 + (float)width * 0.75f, (float)num123 + (float)height * 0.75f));
                    if (Main.mouseX >= num122 && Main.mouseX <= num122 + width && Main.mouseY >= num123 && Main.mouseY <= num123 + height && !PlayerInput.IgnoreMouseInterface)
                    {
                        num121 = 1;
                        if (!(bool)allChestStackHover.GetValue(null))
                        {
                            Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                            allChestStackHover.SetValue(null, true);
                        }
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Main.mouseLeftRelease = false;
                            Main.player[Main.myPlayer].QuickStackAllChests();
                            API.FindRecipes();
                        }
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    else if ((bool)allChestStackHover.GetValue(null))
                    {
                        Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                        allChestStackHover.SetValue(null, false);
                    }
                    Main.spriteBatch.Draw(Main.chestStackTexture[num121], new Vector2((float)num122, (float)num123), new Rectangle?(new Rectangle(0, 0, Main.chestStackTexture[num121].Width, Main.chestStackTexture[num121].Height)), Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    if (!Main.mouseText && num121 == 1)
                    {
                        main.MouseText(Language.GetTextValue("GameUI.ToNearby"), 0, 0, -1, -1, -1, -1);
                    }
                }
                if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
                {
                    int num124 = 0;
                    int num125 = 534;
                    int num126 = (int)Math.Round(414f * scale) + 34;
                    int num127 = 30;
                    int num128 = 30;
                    UILinkPointNavigator.SetPosition(302, new Vector2((float)num125 + (float)num127 * 0.75f, (float)num126 + (float)num128 * 0.75f));
                    bool flag10 = false;
                    if (Main.mouseX >= num125 && Main.mouseX <= num125 + num127 && Main.mouseY >= num126 && Main.mouseY <= num126 + num128 && !PlayerInput.IgnoreMouseInterface)
                    {
                        num124 = 1;
                        flag10 = true;
                        Main.player[Main.myPlayer].mouseInterface = true;
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Main.mouseLeftRelease = false;
                            ItemSorting.SortInventory();
                            API.FindRecipes();
                        }
                    }
                    if (flag10 != (bool)inventorySortMouseOver.GetValue(null))
                    {
                        Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                        inventorySortMouseOver.SetValue(null, flag10);
                    }
                    Texture2D texture2 = Main.inventorySortTexture[(bool)inventorySortMouseOver.GetValue(null) ? 1 : 0];
                    Main.spriteBatch.Draw(texture2, new Vector2((float)num125, (float)num126), null, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    if (!Main.mouseText && num124 == 1)
                    {
                        main.MouseText(Language.GetTextValue("GameUI.SortInventory"), 0, 0, -1, -1, -1, -1);
                    }
                }
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
            }
        }
    }
}
