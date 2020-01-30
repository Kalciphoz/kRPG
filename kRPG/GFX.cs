using System.Collections.Generic;
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
        public const int BTN_WIDTH = 184;
        public const int BTN_HEIGHT = 48;

        private const string GUI_DIRECTORY = "GFX/GUI/";
        private const string DEERSKULL = GUI_DIRECTORY + "DeerSkull";
        private const string FLAMES_RED = GUI_DIRECTORY + "Flames_Resilience";
        private const string FLAMES_GREEN = GUI_DIRECTORY + "Flames_Quickness";
        private const string FLAMES_BLUE = GUI_DIRECTORY + "Flames_Potency";
        private const string FLAMES_CONVERTED = GUI_DIRECTORY + "Flames_Converted";
        private const string EYES_RED = GUI_DIRECTORY + "DeerSkull_Eyes_Resilience";
        private const string EYES_GREEN = GUI_DIRECTORY + "DeerSkull_Eyes_Quickness";
        private const string EYES_BLUE = GUI_DIRECTORY + "DeerSkull_Eyes_Potency";
        private const string ANVIL = GUI_DIRECTORY + "Anvil";
        private const string CHARACTERFRAME = GUI_DIRECTORY + "CharacterFrame";
        private const string STATUSBARS = GUI_DIRECTORY + "CharacterFrame_Bars";
        private const string BUBBLES = GUI_DIRECTORY + "CharacterFrame_Bubbles";
        private const string BUBBLES_LAVA = GUI_DIRECTORY + "CharacterFrame_Bubbles_Lava";
        private const string STATUSBARS_BG = GUI_DIRECTORY + "CharacterFrame_Bars_Background";
        private const string NUMERALS = GUI_DIRECTORY + "Num/Gothic_";
        private const string LETTERS = GUI_DIRECTORY + "Char/Gothic_";
        private const string UNSPENTPOINTS = GUI_DIRECTORY + "UnspentPoints";
        private const string ITEMSLOT = GUI_DIRECTORY + "ItemSlot";
        private const string ITEMSLOT_BROKEN = GUI_DIRECTORY + "ItemSlot_Broken";
        private const string FAVOURITEDSLOT = GUI_DIRECTORY + "FavouritedSlot";
        private const string SELECTEDSLOT = GUI_DIRECTORY + "SelectedSlot";
        private const string SKILLSLOT = GUI_DIRECTORY + "SkillSlot";
        private const string SELECTEDSKILLSLOT = GUI_DIRECTORY + "SelectedSkillSlot";
        private const string SPELLGUI = GUI_DIRECTORY + "SpellGUI";
        private const string INVENTORYFRAME = GUI_DIRECTORY + "Inventory_Frame";
        private const string INVENTORY_SEPARATOR = GUI_DIRECTORY + "Inventory_Separator";
        private const string INVENTORY_LIFE = GUI_DIRECTORY + "Inventory_Life";
        private const string INVENTORY_MANA = GUI_DIRECTORY + "Inventory_Mana";
        private const string INVENTORY_XP = GUI_DIRECTORY + "Inventory_XP";
        private const string INVENTORY_BARCOVERS = GUI_DIRECTORY + "Inventory_BarCovers";
        private const string INVENTORY_POINTS = GUI_DIRECTORY + "Inventory_Points";
        private const string INVENTORY_PANEL = GUI_DIRECTORY + "Inventory_Panel";

        private const string BUTTON_CONFIRM = GUI_DIRECTORY + "Button_Confirm";
        private const string BUTTON_UPGRADE = GUI_DIRECTORY + "Button_Upgrade";
        private const string BUTTON_CANCEL = GUI_DIRECTORY + "Button_Cancel";
        private const string BUTTON_CLOSE = GUI_DIRECTORY + "Button_Close";
        private const string BUTTON_STATS = GUI_DIRECTORY + "Button_Stats";
        private const string BUTTON_STATS_PRESSED = GUI_DIRECTORY + "Button_Stats_Pressed";
        private const string BUTTON_PAGE1 = GUI_DIRECTORY + "Button_Page1";
        private const string BUTTON_PAGE1_PRESSED = GUI_DIRECTORY + "Button_Page1_Pressed";
        private const string BUTTON_PAGE2 = GUI_DIRECTORY + "Button_Page2";
        private const string BUTTON_PAGE2_PRESSED = GUI_DIRECTORY + "Button_Page2_Pressed";
        private const string BUTTON_PAGE3 = GUI_DIRECTORY + "Button_Page3";
        private const string BUTTON_PAGE3_PRESSED = GUI_DIRECTORY + "Button_Page3_Pressed";
        private const string BUTTON_CROWN = GUI_DIRECTORY + "Button_Crown";
        private const string BUTTON_CROWN_PRESSED = GUI_DIRECTORY + "Button_Crown_Pressed";
        private const string GUARDIAN_CROWN = GUI_DIRECTORY + "GuardianCrown";

        private const string PROJECTILES_DIRECTORY = "GFX/Projectiles/";
        private const string PROJECTILE_FIREBALL = PROJECTILES_DIRECTORY + "Fireball";
        private const string PROJECTILE_FROSTBOLT = PROJECTILES_DIRECTORY + "Frostbolt";
        private const string PROJECTILE_BOULDER = PROJECTILES_DIRECTORY + "Boulder";
        private const string PROJECTILE_SHADOWBOLT = PROJECTILES_DIRECTORY + "Shadowbolt";
        private const string PROJECTILE_THUNDERBOLT = PROJECTILES_DIRECTORY + "Thunderbolt";
        private const string HEART = PROJECTILES_DIRECTORY + "Heart";
        private const string STAR = PROJECTILES_DIRECTORY + "Star";
        private const string THORNCHAIN = PROJECTILES_DIRECTORY + "ThornChain";

        private const string LEVELUP = "GFX/LevelUP";

        private const string LEVELUPSOUND = "SFX/LevelUP";

        public static Texture2D deerskull;
        public static Dictionary<STAT, Texture2D> flames = new Dictionary<STAT, Texture2D>();
        public static Texture2D flames_converted;
        public static Dictionary<STAT, Texture2D> deerskull_eyes = new Dictionary<STAT, Texture2D>();
        public static Texture2D anvil;
        public static Texture2D characterFrame;
        public static Texture2D statusBars;
        public static Texture2D bubbles;
        public static Texture2D bubbles_lava;
        public static Texture2D statusBars_BG;
        public static Texture2D[] gothicNumeral = new Texture2D[10];
        public static Dictionary<Keys, Texture2D> gothicLetter = new Dictionary<Keys, Texture2D>();
        public static Texture2D unspentPoints;
        public static Texture2D itemSlot;
        public static Texture2D itemSlot_broken;
        public static Texture2D favouritedSlot;
        public static Texture2D selectedSlot;
        public static Texture2D skillSlot;
        public static Texture2D selectedSkillSlot;
        public static Texture2D spellGui;
        public static Texture2D inventoryFrame;
        public static Texture2D inventory_separator;
        public static Texture2D inventory_life;
        public static Texture2D inventory_mana;
        public static Texture2D inventory_xp;
        public static Texture2D inventory_barCovers;
        public static Texture2D inventory_points;
        public static Texture2D inventory_panel;

        public static Texture2D button_confirm;
        public static Texture2D button_upgrade;
        public static Texture2D button_cancel;
        public static Texture2D button_close;
        public static Texture2D button_stats;
        public static Texture2D button_stats_pressed;
        public static Texture2D button_page1;
        public static Texture2D button_page1_pressed;
        public static Texture2D button_page2;
        public static Texture2D button_page2_pressed;
        public static Texture2D button_page3;
        public static Texture2D button_page3_pressed;
        public static Texture2D button_crown;
        public static Texture2D button_crown_pressed;
        public static Texture2D guardianCrown;

        public static Texture2D projectile_fireball;
        public static Texture2D projectile_frostbolt;
        public static Texture2D projectile_boulder;
        public static Texture2D projectile_shadowbolt;
        public static Texture2D projectile_thunderbolt;
        public static Texture2D heart;
        public static Texture2D star;
        public static Texture2D thornChain;

        public static Texture2D levelUp;

        public static SoundEffect sfx_levelUp;

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
                                combinedTexture[origins[tex].X * 2 + x * 2 + i % 2 + (origins[tex].Y * 2 + y * 2 + (int)(i / 2)) * texture.Width] = pixels[x + y * textures[tex].Width];
            }

            texture.SetData<Color>(combinedTexture);
            return texture;
        }

        public static void LoadGFX()
        {
            var loader = ModLoader.GetMod("kRPG");


            button_confirm = loader.GetTexture(BUTTON_CONFIRM);
            button_upgrade = loader.GetTexture(BUTTON_UPGRADE);
            button_cancel = loader.GetTexture(BUTTON_CANCEL);
            button_close = loader.GetTexture(BUTTON_CLOSE);
            button_stats = loader.GetTexture(BUTTON_STATS);
            button_page1 = loader.GetTexture(BUTTON_PAGE1);
            button_page2 = loader.GetTexture(BUTTON_PAGE2);
            button_page3 = loader.GetTexture(BUTTON_PAGE3);
            button_stats_pressed = loader.GetTexture(BUTTON_STATS_PRESSED);
            button_page1_pressed = loader.GetTexture(BUTTON_PAGE1_PRESSED);
            button_page2_pressed = loader.GetTexture(BUTTON_PAGE2_PRESSED);
            button_page3_pressed = loader.GetTexture(BUTTON_PAGE3_PRESSED);
            button_crown = loader.GetTexture(BUTTON_CROWN);
            button_crown_pressed = loader.GetTexture(BUTTON_CROWN_PRESSED);
            guardianCrown = loader.GetTexture(GUARDIAN_CROWN);

            deerskull = loader.GetTexture(DEERSKULL);
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
            for (int i = 0; i < 10; i++ )
            {
                gothicNumeral[i] = loader.GetTexture(NUMERALS + i.ToString());
            }
            for (Keys k = Keys.A; k <= Keys.Z; k += 1)
            {
                gothicLetter[k] = loader.GetTexture(LETTERS + k.ToString());
            }
            unspentPoints = loader.GetTexture(UNSPENTPOINTS);
            itemSlot = loader.GetTexture(ITEMSLOT);
            itemSlot_broken = loader.GetTexture(ITEMSLOT_BROKEN);
            favouritedSlot = loader.GetTexture(FAVOURITEDSLOT);
            selectedSlot = loader.GetTexture(SELECTEDSLOT);
            skillSlot = loader.GetTexture(SKILLSLOT);
            selectedSkillSlot = loader.GetTexture(SELECTEDSKILLSLOT);
            spellGui = loader.GetTexture(SPELLGUI);
            inventoryFrame = loader.GetTexture(INVENTORYFRAME);
            inventory_separator = loader.GetTexture(INVENTORY_SEPARATOR);
            inventory_life = loader.GetTexture(INVENTORY_LIFE);
            inventory_mana = loader.GetTexture(INVENTORY_MANA);
            inventory_xp = loader.GetTexture(INVENTORY_XP);
            inventory_barCovers = loader.GetTexture(INVENTORY_BARCOVERS);
            inventory_points = loader.GetTexture(INVENTORY_POINTS);
            inventory_panel = loader.GetTexture(INVENTORY_PANEL);

            projectile_fireball = loader.GetTexture(PROJECTILE_FIREBALL);
            projectile_frostbolt = loader.GetTexture(PROJECTILE_FROSTBOLT);
            projectile_boulder = loader.GetTexture(PROJECTILE_BOULDER);
            projectile_shadowbolt = loader.GetTexture(PROJECTILE_SHADOWBOLT);
            projectile_thunderbolt = loader.GetTexture(PROJECTILE_THUNDERBOLT);
            heart = loader.GetTexture(HEART);
            star = loader.GetTexture(STAR);
            thornChain = loader.GetTexture(THORNCHAIN);

            levelUp = loader.GetTexture(LEVELUP);

            sfx_levelUp = loader.GetSound(LEVELUPSOUND);
        }

        public static void UnloadGFX()
        {
            button_confirm = null;
            button_upgrade = null;
            button_cancel = null;
            button_close = null;
            button_stats = null;
            button_page1 = null;
            button_page2 = null;
            button_page3 = null;
            button_stats_pressed = null;
            button_page1_pressed = null;
            button_page2_pressed = null;
            button_page3_pressed = null;
            button_crown = null;
            button_crown_pressed = null;

            deerskull = null;
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
            for (Keys k = Keys.A; k <= Keys.Z; k += 1)
                gothicLetter[k] = null;
            unspentPoints = null;
            itemSlot = null;
            itemSlot_broken = null;
            favouritedSlot = null;
            selectedSlot = null;
            skillSlot = null;
            selectedSkillSlot = null;
            spellGui = null;
            inventoryFrame = null;
            inventory_separator = null;
            inventory_life = null;
            inventory_mana = null;
            inventory_xp = null;
            inventory_barCovers = null;
            inventory_points = null;
            inventory_panel = null;

            projectile_fireball = null;
            projectile_frostbolt = null;
            projectile_boulder = null;
            projectile_shadowbolt = null;
            projectile_thunderbolt = null;
            heart = null;
            star = null;
            thornChain = null;

            levelUp = null;

            sfx_levelUp = null;
        }
    }
}
