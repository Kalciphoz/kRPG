using System.Collections.Generic;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;

namespace kRPG
{
    public static class GFX
    {
        public static Texture2D anvil;
        private const string ANVIL = GUI_DIRECTORY + "Anvil";
        public const int BTN_HEIGHT = 48;
        public const int BTN_WIDTH = 184;
        public static Texture2D bubbles;
        private const string BUBBLES = GUI_DIRECTORY + "CharacterFrame_Bubbles";
        public static Texture2D bubbles_lava;
        private const string BUBBLES_LAVA = GUI_DIRECTORY + "CharacterFrame_Bubbles_Lava";
        private const string BUTTON_CANCEL = GUI_DIRECTORY + "Button_Cancel";
        private const string BUTTON_CLOSE = GUI_DIRECTORY + "Button_Close";

        private const string BUTTON_CONFIRM = GUI_DIRECTORY + "Button_Confirm";
        private const string BUTTON_CROWN = GUI_DIRECTORY + "Button_Crown";
        private const string BUTTON_CROWN_PRESSED = GUI_DIRECTORY + "Button_Crown_Pressed";
        private const string BUTTON_PAGE1 = GUI_DIRECTORY + "Button_Page1";
        private const string BUTTON_PAGE1_PRESSED = GUI_DIRECTORY + "Button_Page1_Pressed";
        private const string BUTTON_PAGE2 = GUI_DIRECTORY + "Button_Page2";
        private const string BUTTON_PAGE2_PRESSED = GUI_DIRECTORY + "Button_Page2_Pressed";
        private const string BUTTON_PAGE3 = GUI_DIRECTORY + "Button_Page3";
        private const string BUTTON_PAGE3_PRESSED = GUI_DIRECTORY + "Button_Page3_Pressed";
        private const string BUTTON_STATS = GUI_DIRECTORY + "Button_Stats";
        private const string BUTTON_STATS_PRESSED = GUI_DIRECTORY + "Button_Stats_Pressed";
        private const string BUTTON_UPGRADE = GUI_DIRECTORY + "Button_Upgrade";
        public static Texture2D buttonCancel;
        public static Texture2D buttonClose;

        public static Texture2D buttonConfirm;
        public static Texture2D buttonCrown;
        public static Texture2D buttonCrownPressed;
        public static Texture2D buttonPage1;
        public static Texture2D buttonPage1Pressed;
        public static Texture2D buttonPage2;
        public static Texture2D buttonPage2Pressed;
        public static Texture2D buttonPage3;
        public static Texture2D buttonPage3Pressed;
        public static Texture2D buttonStats;
        public static Texture2D buttonStatsPressed;
        public static Texture2D buttonUpgrade;
        public static Texture2D characterFrame;
        private const string CHARACTERFRAME = GUI_DIRECTORY + "CharacterFrame";

        public static Texture2D deerSkull;
        private const string DEERSKULL = GUI_DIRECTORY + "DeerSkull";
        public static Dictionary<STAT, Texture2D> deerskull_eyes = new Dictionary<STAT, Texture2D>();
        private const string EYES_BLUE = GUI_DIRECTORY + "DeerSkull_Eyes_Potency";
        private const string EYES_GREEN = GUI_DIRECTORY + "DeerSkull_Eyes_Quickness";
        private const string EYES_RED = GUI_DIRECTORY + "DeerSkull_Eyes_Resilience";
        public static Texture2D favouritedSlot;
        private const string FAVOURITEDSLOT = GUI_DIRECTORY + "FavouritedSlot";
        public static Dictionary<STAT, Texture2D> flames = new Dictionary<STAT, Texture2D>();
        private const string FLAMES_BLUE = GUI_DIRECTORY + "Flames_Potency";
        public static Texture2D flames_converted;
        private const string FLAMES_CONVERTED = GUI_DIRECTORY + "Flames_Converted";
        private const string FLAMES_GREEN = GUI_DIRECTORY + "Flames_Quickness";
        private const string FLAMES_RED = GUI_DIRECTORY + "Flames_Resilience";
        public static Dictionary<Keys, Texture2D> gothicLetter = new Dictionary<Keys, Texture2D>();
        public static Texture2D[] gothicNumeral = new Texture2D[10];
        private const string GUARDIAN_CROWN = GUI_DIRECTORY + "GuardianCrown";
        public static Texture2D guardianCrown;

        private const string GUI_DIRECTORY = "GFX/GUI/";
        public static Texture2D heart;
        private const string HEART = PROJECTILES_DIRECTORY + "Heart";
        private const string INVENTORY_BARCOVERS = GUI_DIRECTORY + "Inventory_BarCovers";
        private const string INVENTORY_LIFE = GUI_DIRECTORY + "Inventory_Life";
        private const string INVENTORY_MANA = GUI_DIRECTORY + "Inventory_Mana";
        private const string INVENTORY_PANEL = GUI_DIRECTORY + "Inventory_Panel";
        private const string INVENTORY_POINTS = GUI_DIRECTORY + "Inventory_Points";
        private const string INVENTORY_SEPARATOR = GUI_DIRECTORY + "Inventory_Separator";
        private const string INVENTORY_XP = GUI_DIRECTORY + "Inventory_XP";
        public static Texture2D inventoryBarCovers;
        public static Texture2D inventoryFrame;
        private const string INVENTORYFRAME = GUI_DIRECTORY + "Inventory_Frame";
        public static Texture2D inventoryLife;
        public static Texture2D inventoryMana;
        public static Texture2D inventoryPanel;
        public static Texture2D inventoryPoints;
        public static Texture2D inventorySeparator;
        public static Texture2D inventoryXp;
        public static Texture2D itemSlot;
        private const string ITEMSLOT = GUI_DIRECTORY + "ItemSlot";
        private const string ITEMSLOT_BROKEN = GUI_DIRECTORY + "ItemSlot_Broken";
        public static Texture2D itemSlotBroken;
        private const string LETTERS = GUI_DIRECTORY + "Char/Gothic_";

        public static Texture2D levelUp;

        private const string LEVELUP = "GFX/LevelUP";

        private const string LEVELUPSOUND = "SFX/LevelUP";
        private const string NUMERALS = GUI_DIRECTORY + "Num/Gothic_";
        private const string PROJECTILE_BOULDER = PROJECTILES_DIRECTORY + "Boulder";
        private const string PROJECTILE_FIREBALL = PROJECTILES_DIRECTORY + "Fireball";
        private const string PROJECTILE_FROSTBOLT = PROJECTILES_DIRECTORY + "Frostbolt";
        private const string PROJECTILE_SHADOWBOLT = PROJECTILES_DIRECTORY + "Shadowbolt";
        private const string PROJECTILE_THUNDERBOLT = PROJECTILES_DIRECTORY + "Thunderbolt";
        public static Texture2D projectileBoulder;

        public static Texture2D projectileFireball;

        // ReSharper disable once IdentifierTypo
        public static Texture2D projectileFrostbolt;

        private const string PROJECTILES_DIRECTORY = "GFX/Projectiles/";

        // ReSharper disable once IdentifierTypo
        public static Texture2D projectileShadowbolt;
        public static Texture2D projectileThunderbolt;
        public static Texture2D selectedSkillSlot;
        private const string SELECTEDSKILLSLOT = GUI_DIRECTORY + "SelectedSkillSlot";
        public static Texture2D selectedSlot;
        private const string SELECTEDSLOT = GUI_DIRECTORY + "SelectedSlot";

        public static SoundEffect sfxLevelUp;
        public static Texture2D skillSlot;
        private const string SKILLSLOT = GUI_DIRECTORY + "SkillSlot";
        public static Texture2D spellGui;
        private const string SPELLGUI = GUI_DIRECTORY + "SpellGUI";
        public static Texture2D star;
        private const string STAR = PROJECTILES_DIRECTORY + "Star";
        public static Texture2D statusBars;
        private const string STATUSBARS = GUI_DIRECTORY + "CharacterFrame_Bars";
        public static Texture2D statusBars_BG;
        private const string STATUSBARS_BG = GUI_DIRECTORY + "CharacterFrame_Bars_Background";
        public static Texture2D thornChain;
        private const string THORNCHAIN = PROJECTILES_DIRECTORY + "ThornChain";
        public static Texture2D unspentPoints;
        private const string UNSPENTPOINTS = GUI_DIRECTORY + "UnspentPoints";

        public static Texture2D CombineTextures(List<Texture2D> textures, List<Point> origins, Point final_size)
        {
            var texture = new Texture2D(Main.spriteBatch.GraphicsDevice, final_size.X * 2, final_size.Y * 2);
            var combinedTexture = new Color[texture.Width * texture.Height];

            for (int tex = 0; tex < textures.Count; tex += 1)
            {
                if (textures[tex] == null) continue;

                var pixels = new Color[textures[tex].Width * textures[tex].Height];
                textures[tex].GetData(pixels);

                for (int x = 0; x < textures[tex].Width; x += 1)
                for (int y = 0; y < textures[tex].Height; y += 1)
                    if (pixels[x + y * textures[tex].Width].A > 0)
                        for (int i = 0; i < 4; i += 1)
                            combinedTexture[origins[tex].X * 2 + x * 2 + i % 2 + (origins[tex].Y * 2 + y * 2 + i / 2) * texture.Width] =
                                pixels[x + y * textures[tex].Width];
            }

            texture.SetData(combinedTexture);
            return texture;
        }

        public static void LoadGfx()
        {
            var loader = ModLoader.GetMod("kRPG");

            buttonConfirm = loader.GetTexture(BUTTON_CONFIRM);
            buttonUpgrade = loader.GetTexture(BUTTON_UPGRADE);
            buttonCancel = loader.GetTexture(BUTTON_CANCEL);
            buttonClose = loader.GetTexture(BUTTON_CLOSE);
            buttonStats = loader.GetTexture(BUTTON_STATS);
            buttonPage1 = loader.GetTexture(BUTTON_PAGE1);
            buttonPage2 = loader.GetTexture(BUTTON_PAGE2);
            buttonPage3 = loader.GetTexture(BUTTON_PAGE3);
            buttonStatsPressed = loader.GetTexture(BUTTON_STATS_PRESSED);
            buttonPage1Pressed = loader.GetTexture(BUTTON_PAGE1_PRESSED);
            buttonPage2Pressed = loader.GetTexture(BUTTON_PAGE2_PRESSED);
            buttonPage3Pressed = loader.GetTexture(BUTTON_PAGE3_PRESSED);
            buttonCrown = loader.GetTexture(BUTTON_CROWN);
            buttonCrownPressed = loader.GetTexture(BUTTON_CROWN_PRESSED);
            guardianCrown = loader.GetTexture(GUARDIAN_CROWN);

            deerSkull = loader.GetTexture(DEERSKULL);
            flames[STAT.RESILIENCE] = loader.GetTexture(FLAMES_RED);
            flames[STAT.QUICKNESS] = loader.GetTexture(FLAMES_GREEN);
            flames[STAT.POTENCY] = loader.GetTexture(FLAMES_BLUE);
            flames_converted = loader.GetTexture(FLAMES_CONVERTED);
            deerskull_eyes[STAT.RESILIENCE] = loader.GetTexture(EYES_RED);
            deerskull_eyes[STAT.QUICKNESS] = loader.GetTexture(EYES_GREEN);
            deerskull_eyes[STAT.POTENCY] = loader.GetTexture(EYES_BLUE);
            anvil = loader.GetTexture(ANVIL);
            characterFrame = loader.GetTexture(CHARACTERFRAME);
            statusBars = loader.GetTexture(STATUSBARS);
            bubbles = loader.GetTexture(BUBBLES);
            bubbles_lava = loader.GetTexture(BUBBLES_LAVA);
            statusBars_BG = loader.GetTexture(STATUSBARS_BG);
            for (int i = 0; i < 10; i++)
                gothicNumeral[i] = loader.GetTexture(NUMERALS + i);
            for (var k = Keys.A; k <= Keys.Z; k += 1)
                gothicLetter[k] = loader.GetTexture(LETTERS + k);
            unspentPoints = loader.GetTexture(UNSPENTPOINTS);
            itemSlot = loader.GetTexture(ITEMSLOT);
            itemSlotBroken = loader.GetTexture(ITEMSLOT_BROKEN);
            favouritedSlot = loader.GetTexture(FAVOURITEDSLOT);
            selectedSlot = loader.GetTexture(SELECTEDSLOT);
            skillSlot = loader.GetTexture(SKILLSLOT);
            selectedSkillSlot = loader.GetTexture(SELECTEDSKILLSLOT);
            spellGui = loader.GetTexture(SPELLGUI);
            inventoryFrame = loader.GetTexture(INVENTORYFRAME);
            inventorySeparator = loader.GetTexture(INVENTORY_SEPARATOR);
            inventoryLife = loader.GetTexture(INVENTORY_LIFE);
            inventoryMana = loader.GetTexture(INVENTORY_MANA);
            inventoryXp = loader.GetTexture(INVENTORY_XP);
            inventoryBarCovers = loader.GetTexture(INVENTORY_BARCOVERS);
            inventoryPoints = loader.GetTexture(INVENTORY_POINTS);
            inventoryPanel = loader.GetTexture(INVENTORY_PANEL);

            projectileFireball = loader.GetTexture(PROJECTILE_FIREBALL);
            projectileFrostbolt = loader.GetTexture(PROJECTILE_FROSTBOLT);
            projectileBoulder = loader.GetTexture(PROJECTILE_BOULDER);
            projectileShadowbolt = loader.GetTexture(PROJECTILE_SHADOWBOLT);
            projectileThunderbolt = loader.GetTexture(PROJECTILE_THUNDERBOLT);
            heart = loader.GetTexture(HEART);
            star = loader.GetTexture(STAR);
            thornChain = loader.GetTexture(THORNCHAIN);

            levelUp = loader.GetTexture(LEVELUP);

            sfxLevelUp = loader.GetSound(LEVELUPSOUND);
        }

        public static void UnloadGfx()
        {
            buttonConfirm = null;
            buttonUpgrade = null;
            buttonCancel = null;
            buttonClose = null;
            buttonStats = null;
            buttonPage1 = null;
            buttonPage2 = null;
            buttonPage3 = null;
            buttonStatsPressed = null;
            buttonPage1Pressed = null;
            buttonPage2Pressed = null;
            buttonPage3Pressed = null;
            buttonCrown = null;
            buttonCrownPressed = null;

            deerSkull = null;
            flames[STAT.RESILIENCE] = null;
            flames[STAT.QUICKNESS] = null;
            flames[STAT.POTENCY] = null;
            flames_converted = null;
            deerskull_eyes[STAT.RESILIENCE] = null;
            deerskull_eyes[STAT.QUICKNESS] = null;
            deerskull_eyes[STAT.POTENCY] = null;
            anvil = null;
            characterFrame = null;
            statusBars = null;
            bubbles = null;
            bubbles_lava = null;
            statusBars_BG = null;
            for (int i = 0; i < 10; i++)
                gothicNumeral[i] = null;
            for (var k = Keys.A; k <= Keys.Z; k += 1)
                gothicLetter[k] = null;
            unspentPoints = null;
            itemSlot = null;
            itemSlotBroken = null;
            favouritedSlot = null;
            selectedSlot = null;
            skillSlot = null;
            selectedSkillSlot = null;
            spellGui = null;
            inventoryFrame = null;
            inventorySeparator = null;
            inventoryLife = null;
            inventoryMana = null;
            inventoryXp = null;
            inventoryBarCovers = null;
            inventoryPoints = null;
            inventoryPanel = null;

            projectileFireball = null;
            projectileFrostbolt = null;
            projectileBoulder = null;
            projectileShadowbolt = null;
            projectileThunderbolt = null;
            heart = null;
            star = null;
            thornChain = null;

            levelUp = null;

            sfxLevelUp = null;
        }
    }
}