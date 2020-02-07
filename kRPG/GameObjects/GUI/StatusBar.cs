using System;
using System.Reflection;
using kRPG.Enums;
using kRPG.GameObjects.GUI.Base;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.SFX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace kRPG.GameObjects.GUI
{
    public class StatusBar : BaseGui
    {
        private const int BarLifeLength = 302;
        private const int BarLifeThickness = 28;

        // ReSharper disable once IdentifierTypo
        private const int BarManaLength = 286;

        // ReSharper disable once IdentifierTypo
        private const int BarManaThickness = 18;
        private const int BarXpLength = 138;
        private const int BarXpThickness = 6;
        private const int BubblesLength = 132;
        private const int BubblesThickness = 22;

        /// <summary>
        ///     They are using a reference to the function since this function is not public.
        /// </summary>
        private static readonly MethodInfo DrawBuffIcon = typeof(Main).GetMethod("DrawBuffIcon", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly Vector2 barLifeOrigin;

        // ReSharper disable once IdentifierTypo
        private readonly Vector2 barManaOrigin;

        private readonly Vector2 barXpOrigin;

        private readonly Vector2 bubblesOrigin;
        private PlayerCharacter character;

        public StatusBar(PlayerCharacter character, Mod mod)
        {
            Main.player[Main.myPlayer].hbLocked = false;
            this.character = character;
            barLifeOrigin = new Vector2(278f, 50f);
            barManaOrigin = new Vector2(254f, 92f);
            barXpOrigin = new Vector2(370f, 122f);
            bubblesOrigin = new Vector2(284, 134);

            AddButton(
                () => new Rectangle((int)PointsOrigin.X, (int)PointsOrigin.Y, (int)(GFX.GFX.UnspentPoints.Width * Scale),
                    (int)(GFX.GFX.UnspentPoints.Height * Scale)), delegate (Player player)
               {
                   character.CloseGuIs();
                   //Main.PlaySound(SoundID.MenuTick);
                   SoundManager.PlaySound(Sounds.MenuClose);
                   character.LevelGui.GuiActive = player.GetModPlayer<PlayerCharacter>().UnspentPoints() && !Main.playerInventory;
               }, delegate
               {
                   Main.LocalPlayer.mouseInterface = true;
                   string s = Main.player[Main.myPlayer].GetModPlayer<PlayerCharacter>().UnspentPoints()
                       ? "Click here to allocate stat points"
                       : "You have no unspent stat points";
                   Main.instance.MouseText(s);
               });
        }

        /// <summary>
        ///     Base guid xy position
        /// </summary>
        private static Vector2 GuiPosition => new Vector2(4f, 6f) * Scale;

        private static Vector2 PointsOrigin => GuiPosition + new Vector2(242f, 112f) * Scale;

        private static float Scale => Math.Min(1f, Main.screenWidth / Constants.MaxScreenWidth + 0.4f);

        public static void DrawBuffs()
        {
            int leftOffset = 320;
            int iconWidth = 38;
            int maxSlots = 21;

            int buffTypeId = -1;
            int secondRowOfBuffsStartIndex = 11;

            for (int buffSlot = 0; buffSlot <= maxSlots; buffSlot++)

                if (Main.player[Main.myPlayer].buffType[buffSlot] > 0)
                {
                    int buff = Main.player[Main.myPlayer].buffType[buffSlot];
                    int xPosition = leftOffset + buffSlot * iconWidth;

                    int yPosition = 8; //The y offset for the first row...

                    if (buffSlot >= secondRowOfBuffsStartIndex)
                    {
                        xPosition = 32 + (buffSlot - secondRowOfBuffsStartIndex) * iconWidth;
                        yPosition += 50; //put icon on second row.
                    }

                    buffTypeId = (int)DrawBuffIcon.Invoke(null,
                        new object[] { buffTypeId, buffSlot, buff, xPosition, yPosition }); // Main.DrawBuffIcon(num, i, b, x, num3);
                }
                else
                {
                    Main.buffAlpha[buffSlot] = 0.4f;
                }

            if (buffTypeId < 0)
                return;

            int buffId = Main.player[Main.myPlayer].buffType[buffTypeId];

            if (buffId <= 0)
                return;

            Main.buffString = Lang.GetBuffDescription(buffId);

            int itemRarity = 0;

            switch ((VanillaBuffId)buffId)
            {
                case VanillaBuffId.WellFed when Main.expertMode:
                    Main.buffString = Language.GetTextValue("BuffDescription.WellFed_Expert");
                    break;
                case VanillaBuffId.MonsterBanner:
                    Main.bannerMouseOver = true;
                    break;
                case VanillaBuffId.ManaSickness:
                    {
                        int percent = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
                        Main.buffString = Main.buffString + percent + "%";
                        break;
                    }
            }

            if (Main.meleeBuff[buffId])
                itemRarity = -10;

            BuffLoader.ModifyBuffTip(buffId, ref Main.buffString, ref itemRarity);

            Main.instance.MouseTextHackZoom(Lang.GetBuffName(buffId), itemRarity);
        }

        /// <summary>
        ///     This function draws the hotbar in the upper left screen when the user DOES NOT have the inventory window open.
        /// </summary>
        private void DrawHotbar()
        {
            DrawSelectedItemName();

            //Get the width of the screen, subtract 480 from it which will be the distance from the left we need to be able to draw the bar
            int leftOffset = Main.screenWidth - 480;

            //For each slot
            for (int slotIndex = 0; slotIndex < 10; slotIndex++)
            {
                //This code provides the animation of the selected item growing and shrinking as they scroll through the hotbar
                //The selected item will grow from .75 to 1 in scale when it is selected
                //and it will shrink from 1 to .75 when deselected.

                //If this slot is selected by the player
                if (slotIndex == Main.player[Main.myPlayer].selectedItem)
                {
                    //if the hotbar scale is less that 1
                    if (Main.hotbarScale[slotIndex] < 1f)
                        //Add .05 to it to make it larger
                        Main.hotbarScale[slotIndex] += 0.05f;
                }
                //Otherwise it is a slot that isn't selected.
                //So we check if the scale is greater that .75
                else if (Main.hotbarScale[slotIndex] > 0.75)
                {
                    //We shrink the scale of the slot.
                    Main.hotbarScale[slotIndex] -= 0.05f;
                }

                float itemHotbarScale = Main.hotbarScale[slotIndex];

                int topOffset = (int)(20f + 22f * (1f - itemHotbarScale));

                // int a = (int)(75f + 150f * itemHotbarScale);

                //var color= new Color(200, 255, 255, a);

                //If the user is mousing over the slot.
                if (!Main.player[Main.myPlayer].hbLocked && !PlayerInput.IgnoreMouseInterface && Main.mouseX >= leftOffset &&
                    Main.mouseX <= leftOffset + Main.inventoryBackTexture.Width * Main.hotbarScale[slotIndex] && Main.mouseY >= topOffset &&
                    Main.mouseY <= topOffset + Main.inventoryBackTexture.Height * Main.hotbarScale[slotIndex] && !Main.player[Main.myPlayer].channel)
                {
                    Main.player[Main.myPlayer].mouseInterface = true;
                    Main.player[Main.myPlayer].showItemIcon = false;

                    //If the user clicked the left mouse button on the item...
                    if (Main.mouseLeft && !Main.player[Main.myPlayer].hbLocked && !Main.blockMouse)
                        //Change the user's active item slot to this slot.
                        Main.player[Main.myPlayer].changeItem = slotIndex;

                    //Not sure what affixname does, but this sets the mouse text = to the item in the slot that you are hovering over.
                    Main.hoverItemName = Main.player[Main.myPlayer].inventory[slotIndex].AffixName();

                    //If they have more than one, show in parens the amount.
                    if (Main.player[Main.myPlayer].inventory[slotIndex].stack > 1)
                        Main.hoverItemName = string.Concat(Main.hoverItemName, " (", Main.player[Main.myPlayer].inventory[slotIndex].stack, ")");

                    //If the item is rare, set the rare flag.
                    Main.rare = Main.player[Main.myPlayer].inventory[slotIndex].rare;
                }

                //save InventoryScale
                float originalInventoryScale = Main.inventoryScale;

                Main.inventoryScale = itemHotbarScale;
                ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 13, slotIndex, new Vector2(leftOffset, topOffset), Color.White);

                //Restore Inventory Scale.
                Main.inventoryScale = originalInventoryScale;

                //Move to the left offset for the next button.
                leftOffset += (int)(Main.inventoryBackTexture.Width * Main.hotbarScale[slotIndex]) + 4;
            }
#if NOTUSED
            int selectedItem = Main.player[Main.myPlayer].selectedItem;

            Debug.WriteLine("Selected Item=>"+ selectedItem);
            //Not sure what this condition is to check for, since the selectedItem is always less that 10.
            if (selectedItem < 10 || selectedItem == 58 && Main.mouseItem.type <= 0)
                return;

            //Ok, so this draws a box at the end of the statusbar which shows which one is selected.
            //Since the selected item already is highlighted, I really don't see a purpose for this.


            float num5 = 1f;
            int num6 = (int)(20f + 22f * (1f - num5));
            int a2 = (int) (75f + 150f * num5);
            var lightColor2 = new Color(255, 255, 255, a2);
            float num7 = Main.inventoryScale;
            Main.inventoryScale = num5;
            ItemSlot.Draw(Main.spriteBatch, Main.player[Main.myPlayer].inventory, 13, selectedItem, new Vector2(posX, num6), lightColor2);// Color.White);
            Main.inventoryScale = num7;
#endif
        }

        /// <summary>
        ///     Draws Roman Numerals
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="level"></param>
        /// <param name="scale"></param>
        public static void DrawNumerals(SpriteBatch spriteBatch, int level, float scale)
        {
            Vector2 origin = Main.playerInventory ? new Vector2(132f, 60f) * scale : new Vector2(190f, 58f) * scale;
            if (level < 10)
            {
                spriteBatch.Draw(GFX.GFX.GothicNumeral[level], new Vector2(origin.X - 16f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
            }
            else if (level < 100)
            {
                spriteBatch.Draw(GFX.GFX.GothicNumeral[level / 10], new Vector2(origin.X - 34f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(GFX.GFX.GothicNumeral[level % 10], new Vector2(origin.X + 2f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
            }
            else if (level < 1000)
            {
                spriteBatch.Draw(GFX.GFX.GothicNumeral[level / 100], new Vector2(origin.X - 52f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(GFX.GFX.GothicNumeral[level % 100 / 10], new Vector2(origin.X - 16f, origin.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(GFX.GFX.GothicNumeral[level % 10], new Vector2(origin.X + 20f * scale, origin.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        ///     Draws the selected item's name above the toolbar
        /// </summary>
        public void DrawSelectedItemName()
        {
            string text = "";
            if (!string.IsNullOrEmpty(Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].Name))
                text = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].AffixName();

            Vector2 vector = Main.fontMouseText.MeasureString(text) / 2;

            Main.spriteBatch.DrawStringWithShadow(Main.fontMouseText, text, new Vector2(Main.screenWidth - 240 - vector.X - 16f, 0f),
                new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));
        }

        /// <summary>
        ///     This happens after the draw event
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="player"></param>
        public override void PostDraw(SpriteBatch spriteBatch, Player player)
        {
            //If the player is a ghost, than skip out.
            if (Main.playerInventory || Main.player[Main.myPlayer].ghost)
                return;

            character = player.GetModPlayer<PlayerCharacter>();

            DrawHotbar();

            //Draw the health, mana and exp bar background.  If you change the color it will only effect the exp bar.
            spriteBatch.Draw(GFX.GFX.StatusBarsBg, GuiPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            //Calculate the current life bar length.
            int currentLifeLength = (int)Math.Round(player.statLife / (decimal)player.statLifeMax2 * BarLifeLength);

            spriteBatch.Draw(GFX.GFX.StatusBars, GuiPosition + barLifeOrigin * Scale, new Rectangle(
                (int)(barLifeOrigin.X + BarLifeLength - currentLifeLength), //This how much of the bar should be blacked out.
                (int)barLifeOrigin.Y, currentLifeLength, BarLifeThickness), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            //Calculate the current length of the mana bar
            int currentManaLength = (int)Math.Round(character.Mana / (decimal)player.statManaMax2 * BarManaLength);

            spriteBatch.Draw(GFX.GFX.StatusBars, GuiPosition + barManaOrigin * Scale, new Rectangle(
                (int)(barManaOrigin.X + BarManaLength - currentManaLength), //How much of the bar should be blacked out.
                (int)barManaOrigin.Y, currentManaLength, BarManaThickness), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            //calculate the exp bar length
            int currentXpLength = (int)Math.Round(BarXpLength * (decimal)character.Experience / character.ExperienceToLevel());

            //Draw the exp bar
            spriteBatch.Draw(GFX.GFX.StatusBars, GuiPosition + barXpOrigin * Scale,
                new Rectangle((int)barXpOrigin.X, (int)barXpOrigin.Y, currentXpLength, BarXpThickness), Color.White, 0f, Vector2.Zero, Scale,
                SpriteEffects.None, 0f);

            //Draw the image that is the background for the hud hp/mana/exp bar
            spriteBatch.Draw(GFX.GFX.CharacterFrame, GuiPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            //Draw text showing there health
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, player.statLife + " / " + player.statLifeMax2,
                GuiPosition + new Vector2(barLifeOrigin.X * Scale + 24f * Scale, (barLifeOrigin.Y + 4f) * Scale), Color.White, Scale);

            //Draw text showing there manag
            spriteBatch.DrawStringWithShadow(Main.fontMouseText, character.Mana + " / " + player.statManaMax2,
                GuiPosition + new Vector2(barManaOrigin.X * Scale + 24f * Scale, barManaOrigin.Y * Scale), Color.White, 0.8f * Scale);

            //This should draw roman numerals for the characters level.... but doesn't seem to work.
            DrawNumerals(spriteBatch, character.Level, Scale);

            //If they have unspent level points, draw the icon
            if (character.UnspentPoints())
                spriteBatch.Draw(GFX.GFX.UnspentPoints, PointsOrigin, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            //  Main.spriteBatch.End();

            if (player.lavaTime < player.lavaMax || player.breath < player.breathMax)
            {
                // Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

                if (player.lavaTime < player.lavaMax)
                {
                    int currentBubbles = (int)Math.Round((decimal)BubblesLength * player.lavaTime / player.lavaMax);
                    spriteBatch.Draw(GFX.GFX.bubbles_lava, GuiPosition + bubblesOrigin * Scale, new Rectangle(0, 0, currentBubbles, BubblesThickness), Color.White,
                        Scale);
                }

                if (player.breath < player.breathMax)
                {
                    int currentBubbles = (int)Math.Round((decimal)BubblesLength * player.breath / player.breathMax);
                    spriteBatch.Draw(GFX.GFX.bubbles, GuiPosition + bubblesOrigin * Scale, new Rectangle(0, 0, currentBubbles, BubblesThickness), Color.White,
                        Scale);
                }

                //   Main.spriteBatch.End();
            }
            // you should only start and stop spritebatches 
            //   Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            Main.buffString = "";
            Main.bannerMouseOver = false;
            if (!Main.recBigList)
                Main.recStart = 0;
            if (!Main.ingameOptionsWindow && !Main.playerInventory && !Main.inFancyUI)
                DrawBuffs();

        }
    }
}