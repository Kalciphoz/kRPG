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
        private const string CHARACTERFRAME = GUI_DIRECTORY + "CharacterFrame";
        private const string DEERSKULL = GUI_DIRECTORY + "DeerSkull";
        private const string EYES_BLUE = GUI_DIRECTORY + "DeerSkull_Eyes_Potency";
        private const string EYES_GREEN = GUI_DIRECTORY + "DeerSkull_Eyes_Quickness";
        private const string EYES_RED = GUI_DIRECTORY + "DeerSkull_Eyes_Resilience";
        private const string FAVOURITEDSLOT = GUI_DIRECTORY + "FavouritedSlot";
        private const string FLAMES_BLUE = GUI_DIRECTORY + "Flames_Potency";
        private const string FLAMES_CONVERTED = GUI_DIRECTORY + "Flames_Converted";
        private const string FLAMES_GREEN = GUI_DIRECTORY + "Flames_Quickness";
        private const string FLAMES_RED = GUI_DIRECTORY + "Flames_Resilience";
        private const string GUARDIAN_CROWN = GUI_DIRECTORY + "GuardianCrown";
        private const string GUI_DIRECTORY = "GFX/GUI/";
        private const string HEART = PROJECTILES_DIRECTORY + "Heart";
        private const string INVENTORY_BARCOVERS = GUI_DIRECTORY + "Inventory_BarCovers";
        private const string INVENTORY_LIFE = GUI_DIRECTORY + "Inventory_Life";
        private const string INVENTORY_MANA = GUI_DIRECTORY + "Inventory_Mana";
        private const string INVENTORY_PANEL = GUI_DIRECTORY + "Inventory_Panel";
        private const string INVENTORY_POINTS = GUI_DIRECTORY + "Inventory_Points";
        private const string INVENTORY_SEPARATOR = GUI_DIRECTORY + "Inventory_Separator";
        private const string INVENTORY_XP = GUI_DIRECTORY + "Inventory_XP";
        private const string INVENTORYFRAME = GUI_DIRECTORY + "Inventory_Frame";
        private const string ITEMSLOT = GUI_DIRECTORY + "ItemSlot";
        private const string ITEMSLOT_BROKEN = GUI_DIRECTORY + "ItemSlot_Broken";
        private const string LETTERS = GUI_DIRECTORY + "Char/Gothic_";
        private const string LEVELUP = "GFX/LevelUP";
        private const string LEVELUPSOUND = "SFX/LevelUP";
        private const string NUMERALS = GUI_DIRECTORY + "Num/Gothic_";
        private const string PROJECTILE_BOULDER = PROJECTILES_DIRECTORY + "Boulder";
        private const string PROJECTILE_FIREBALL = PROJECTILES_DIRECTORY + "Fireball";
        private const string PROJECTILE_FROSTBOLT = PROJECTILES_DIRECTORY + "Frostbolt";
        private const string PROJECTILE_SHADOWBOLT = PROJECTILES_DIRECTORY + "Shadowbolt";
        private const string PROJECTILE_THUNDERBOLT = PROJECTILES_DIRECTORY + "Thunderbolt";
        private const string PROJECTILES_DIRECTORY = "GFX/Projectiles/";
        private const string SELECTEDSKILLSLOT = GUI_DIRECTORY + "SelectedSkillSlot";
        private const string SELECTEDSLOT = GUI_DIRECTORY + "SelectedSlot";
        private const string SKILLSLOT = GUI_DIRECTORY + "SkillSlot";
        private const string SPELLGUI = GUI_DIRECTORY + "SpellGUI";
        private const string STAR = PROJECTILES_DIRECTORY + "Star";
        private const string STATUSBARS = GUI_DIRECTORY + "CharacterFrame_Bars";
        private const string STATUSBARS_BG = GUI_DIRECTORY + "CharacterFrame_Bars_Background";
        private const string THORNCHAIN = PROJECTILES_DIRECTORY + "ThornChain";
        private const string UNSPENTPOINTS = GUI_DIRECTORY + "UnspentPoints";


        public static Texture2D ButtonCancel { get; set; }
        public static Texture2D ButtonClose { get; set; }
        public static Texture2D ButtonConfirm { get; set; }
        public static Texture2D ButtonCrown { get; set; }
        public static Texture2D ButtonCrownPressed { get; set; }
        public static Texture2D ButtonPage1 { get; set; }
        public static Texture2D ButtonPage1Pressed { get; set; }
        public static Texture2D ButtonPage2 { get; set; }
        public static Texture2D ButtonPage2Pressed { get; set; }
        public static Texture2D ButtonPage3 { get; set; }
        public static Texture2D ButtonPage3Pressed { get; set; }
        public static Texture2D ButtonStats { get; set; }
        public static Texture2D ButtonStatsPressed { get; set; }
        public static Texture2D ButtonUpgrade { get; set; }
        public static Texture2D CharacterFrame{ get; set; }
        public static Texture2D DeerSkull { get; set; }
        public static Dictionary<STAT, Texture2D> DeerSkullEyes { get; set; } = new Dictionary<STAT, Texture2D>();
        public static Texture2D FavouritedSlot { get; set; }
        public static Dictionary<STAT, Texture2D> Flames { get; set; } = new Dictionary<STAT, Texture2D>();
        public static Texture2D FlamesConverted { get; set; }
        public static Dictionary<Keys, Texture2D> GothicLetter { get; set; } = new Dictionary<Keys, Texture2D>();
        public static Texture2D[] GothicNumeral { get; set; } = new Texture2D[10];
        public static Texture2D GuardianCrown { get; set; }
        public static Texture2D Heart { get; set; }
        public static Texture2D InventoryBarCovers { get; set; }
        public static Texture2D InventoryFrame { get; set; }
        public static Texture2D InventoryLife { get; set; }
        public static Texture2D InventoryMana { get; set; }
        public static Texture2D InventoryPanel { get; set; }
        public static Texture2D InventoryPoints { get; set; }
        public static Texture2D InventorySeparator { get; set; }
        public static Texture2D InventoryXp { get; set; }
        public static Texture2D ItemSlot { get; set; }
        public static Texture2D ItemSlotBroken { get; set; }
        public static Texture2D LevelUp { get; set; }
        public static Texture2D ProjectileBoulder { get; set; }
        public static Texture2D ProjectileFireball { get; set; }
        public static Texture2D ProjectileFrostbolt { get; set; }
        public static Texture2D ProjectileShadowbolt { get; set; }
        public static Texture2D ProjectileThunderbolt { get; set; }
        public static Texture2D SelectedSkillSlot { get; set; }
        public static Texture2D SelectedSlot { get; set; }
        public static SoundEffect SfxLevelUp { get; set; }
        public static Texture2D SkillSlot { get; set; }
        public static Texture2D SpellGui { get; set; }
        public static Texture2D Star { get; set; }
        public static Texture2D StatusBars { get; set; }
        public static Texture2D StatusBarsBg { get; set; }
        public static Texture2D ThornChain { get; set; }
        public static Texture2D UnspentPoints { get; set; }
        

        public static Texture2D CombineTextures(List<Texture2D> textures, List<Point> origins, Point final_size)
        {
            Texture2D texture = new Texture2D(Main.spriteBatch.GraphicsDevice, final_size.X * 2, final_size.Y * 2);
            Color[] combinedTexture = new Color[texture.Width * texture.Height];

            for (int tex = 0; tex < textures.Count; tex += 1)
            {
                if (textures[tex] == null) continue;

                Color[] pixels = new Color[textures[tex].Width * textures[tex].Height];
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
            Mod loader = ModLoader.GetMod("kRPG");

            ButtonConfirm = loader.GetTexture(BUTTON_CONFIRM);
            ButtonUpgrade = loader.GetTexture(BUTTON_UPGRADE);
            ButtonCancel = loader.GetTexture(BUTTON_CANCEL);
            ButtonClose = loader.GetTexture(BUTTON_CLOSE);
            ButtonStats = loader.GetTexture(BUTTON_STATS);
            ButtonPage1 = loader.GetTexture(BUTTON_PAGE1);
            ButtonPage2 = loader.GetTexture(BUTTON_PAGE2);
            ButtonPage3 = loader.GetTexture(BUTTON_PAGE3);
            ButtonStatsPressed = loader.GetTexture(BUTTON_STATS_PRESSED);
            ButtonPage1Pressed = loader.GetTexture(BUTTON_PAGE1_PRESSED);
            ButtonPage2Pressed = loader.GetTexture(BUTTON_PAGE2_PRESSED);
            ButtonPage3Pressed = loader.GetTexture(BUTTON_PAGE3_PRESSED);
            ButtonCrown = loader.GetTexture(BUTTON_CROWN);
            ButtonCrownPressed = loader.GetTexture(BUTTON_CROWN_PRESSED);
            GuardianCrown = loader.GetTexture(GUARDIAN_CROWN);

            DeerSkull = loader.GetTexture(DEERSKULL);
            Flames[STAT.RESILIENCE] = loader.GetTexture(FLAMES_RED);
            Flames[STAT.QUICKNESS] = loader.GetTexture(FLAMES_GREEN);
            Flames[STAT.POTENCY] = loader.GetTexture(FLAMES_BLUE);
            FlamesConverted = loader.GetTexture(FLAMES_CONVERTED);
            DeerSkullEyes[STAT.RESILIENCE] = loader.GetTexture(EYES_RED);
            DeerSkullEyes[STAT.QUICKNESS] = loader.GetTexture(EYES_GREEN);
            DeerSkullEyes[STAT.POTENCY] = loader.GetTexture(EYES_BLUE);
            anvil = loader.GetTexture(ANVIL);
            CharacterFrame = loader.GetTexture(CHARACTERFRAME);
            StatusBars = loader.GetTexture(STATUSBARS);
            bubbles = loader.GetTexture(BUBBLES);
            bubbles_lava = loader.GetTexture(BUBBLES_LAVA);
            StatusBarsBg = loader.GetTexture(STATUSBARS_BG);
            for (int i = 0; i < 10; i++)
                GothicNumeral[i] = loader.GetTexture(NUMERALS + i);
            for (Keys k = Keys.A; k <= Keys.Z; k += 1)
                GothicLetter[k] = loader.GetTexture(LETTERS + k);
            UnspentPoints = loader.GetTexture(UNSPENTPOINTS);
            ItemSlot = loader.GetTexture(ITEMSLOT);
            ItemSlotBroken = loader.GetTexture(ITEMSLOT_BROKEN);
            FavouritedSlot = loader.GetTexture(FAVOURITEDSLOT);
            SelectedSlot = loader.GetTexture(SELECTEDSLOT);
            SkillSlot = loader.GetTexture(SKILLSLOT);
            SelectedSkillSlot = loader.GetTexture(SELECTEDSKILLSLOT);
            SpellGui = loader.GetTexture(SPELLGUI);
            InventoryFrame = loader.GetTexture(INVENTORYFRAME);
            InventorySeparator = loader.GetTexture(INVENTORY_SEPARATOR);
            InventoryLife = loader.GetTexture(INVENTORY_LIFE);
            InventoryMana = loader.GetTexture(INVENTORY_MANA);
            InventoryXp = loader.GetTexture(INVENTORY_XP);
            InventoryBarCovers = loader.GetTexture(INVENTORY_BARCOVERS);
            InventoryPoints = loader.GetTexture(INVENTORY_POINTS);
            InventoryPanel = loader.GetTexture(INVENTORY_PANEL);

            ProjectileFireball = loader.GetTexture(PROJECTILE_FIREBALL);
            ProjectileFrostbolt = loader.GetTexture(PROJECTILE_FROSTBOLT);
            ProjectileBoulder = loader.GetTexture(PROJECTILE_BOULDER);
            ProjectileShadowbolt = loader.GetTexture(PROJECTILE_SHADOWBOLT);
            ProjectileThunderbolt = loader.GetTexture(PROJECTILE_THUNDERBOLT);
            Heart = loader.GetTexture(HEART);
            Star = loader.GetTexture(STAR);
            ThornChain = loader.GetTexture(THORNCHAIN);

            LevelUp = loader.GetTexture(LEVELUP);

            SfxLevelUp = loader.GetSound(LEVELUPSOUND);
        }

        public static void UnloadGfx()
        {
            ButtonConfirm = null;
            ButtonUpgrade = null;
            ButtonCancel = null;
            ButtonClose = null;
            ButtonStats = null;
            ButtonPage1 = null;
            ButtonPage2 = null;
            ButtonPage3 = null;
            ButtonStatsPressed = null;
            ButtonPage1Pressed = null;
            ButtonPage2Pressed = null;
            ButtonPage3Pressed = null;
            ButtonCrown = null;
            ButtonCrownPressed = null;

            DeerSkull = null;
            Flames[STAT.RESILIENCE] = null;
            Flames[STAT.QUICKNESS] = null;
            Flames[STAT.POTENCY] = null;
            FlamesConverted = null;
            DeerSkullEyes[STAT.RESILIENCE] = null;
            DeerSkullEyes[STAT.QUICKNESS] = null;
            DeerSkullEyes[STAT.POTENCY] = null;
            anvil = null;
            CharacterFrame = null;
            StatusBars = null;
            bubbles = null;
            bubbles_lava = null;
            StatusBarsBg = null;
            for (int i = 0; i < 10; i++)
                GothicNumeral[i] = null;
            for (Keys k = Keys.A; k <= Keys.Z; k += 1)
                GothicLetter[k] = null;
            UnspentPoints = null;
            ItemSlot = null;
            ItemSlotBroken = null;
            FavouritedSlot = null;
            SelectedSlot = null;
            SkillSlot = null;
            SelectedSkillSlot = null;
            SpellGui = null;
            InventoryFrame = null;
            InventorySeparator = null;
            InventoryLife = null;
            InventoryMana = null;
            InventoryXp = null;
            InventoryBarCovers = null;
            InventoryPoints = null;
            InventoryPanel = null;

            ProjectileFireball = null;
            ProjectileFrostbolt = null;
            ProjectileBoulder = null;
            ProjectileShadowbolt = null;
            ProjectileThunderbolt = null;
            Heart = null;
            Star = null;
            ThornChain = null;

            LevelUp = null;

            SfxLevelUp = null;
        }
    }
}