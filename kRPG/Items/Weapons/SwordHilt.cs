using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class SwordHilt
    {
        public static SwordHilt ceremonial;
        public static SwordHilt copperCrossguard;
        public static SwordHilt goldenKatana;
        public static SwordHilt blackKatana;
        public static SwordHilt wooden;
        public static SwordHilt ironBasket;
        public static SwordHilt lead;
        public static SwordHilt woodenMount;
        public static SwordHilt ceremonialMount;
        public static SwordHilt eyes;
        public static SwordHilt coe;
        public static SwordHilt demoniteBat;
        public static SwordHilt boneMount;
        public static SwordHilt boneCrest;
        public static SwordHilt demonEye;
        public static SwordHilt purpleCrossguard;
        public static SwordHilt violetCrossguard;
        public static SwordHilt violetRunicKatana;
        public static SwordHilt goldenRunicKatana;
        public static SwordHilt arcaneMount;
        public static SwordHilt stick;
        public static SwordHilt hellstoneCrossguard;
        public static SwordHilt hellstoneBasket;
        public static SwordHilt hellstoneMount;
        public static SwordHilt torch;
        public static SwordHilt bone;
        public static SwordHilt clock;
        public static SwordHilt carbon;
        public static SwordHilt thorns;
        public static SwordHilt chlorophyteMount;
        public static SwordHilt ominousHilt;

        private static Dictionary<SWORDTHEME, List<SwordHilt>> hiltsByTheme;

        public static Dictionary<int, SwordHilt> hilts = new Dictionary<int, SwordHilt>();

        public int type = 0;
        public Texture2D texture;
        public Texture2D spearTexture;
        public Vector2 origin;
        public Point accentOffset = Point.Zero;
        public Vector2 spearOrigin;
        public float dpsModifier = 1f;
        public float speedModifier = 1f;
        public float knockBack = 0f;
        public int critBonus = 0;
        public bool autoswing = false;
        public string prefix = "";
        public int mana = 0;
        public bool spear = false;
        public float scale = 0f;

        public SwordHilt(string texture, int origin_x, int origin_y, string prefix, float dpsModifier, float speedModifier, float knockBack = 0f, int critBonus = 0, bool spear = false, bool autoswing = false, int mana = 0, float scale = 0f)
        {
            this.type = hilts.Count + 1;
            if (Main.netMode != 2)
                this.texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Swordhilts/" + texture);
            this.origin = new Vector2(origin_x, origin_y);
            this.dpsModifier = dpsModifier;
            this.speedModifier = speedModifier;
            this.knockBack = knockBack;
            this.critBonus = critBonus;
            this.autoswing = autoswing;
            this.prefix = prefix;
            this.mana = mana;
            this.spear = spear;
            accentOffset = Point.Zero;
            this.scale = scale;

            if (!hilts.ContainsKey(type))
                hilts.Add(type, this);
        }

        public SwordHilt defineSpear(string texture, int origin_x, int origin_y)
        {
            if (Main.netMode != 2)
                this.spearTexture = ModLoader.GetMod("kRPG").GetTexture("GFX/Projectiles/SpearMounts/" + texture);
            this.spearOrigin = new Vector2(origin_x, origin_y);
            return this;
        }

        public SwordHilt SetAccentOffset(Point offset)
        {
            this.accentOffset = offset;
            return this;
        }

        public static void Initialize()
        {
            hilts = new Dictionary<int, SwordHilt>();
            hiltsByTheme = new Dictionary<SWORDTHEME, List<SwordHilt>>();

            ceremonial = new SwordHilt("CeremonialHilt", 6, 2, "Ceremonial ", 1f, 1.1f);
            copperCrossguard = new SwordHilt("CopperCrossHilt", 6, 2, "Crude ", 1.05f, 0.95f, 0f, 0, false, false, 0, 0.1f);
            goldenKatana = new SwordHilt("GoldenKatanaHilt", 6, 1, "Traditional ", 0.96f, 1.1f, 0f, 5);
            blackKatana = new SwordHilt("KatanaHilt", 6, 1, "Forged ", 1f, 1.1f, 1.5f, 7, false, true, 1);
            wooden = new SwordHilt("WoodenHilt", 6, 2, "Training ", 1f, 0.9f, 1f);
            ironBasket = new SwordHilt("IronBasketHilt", 6, 2, "Folded ", 1.02f, 0.85f, 0.5f, -1);
            lead = new SwordHilt("LeadHilt", 6, 4, "Blacksmithed ", 0.94f, 1.06f, -0.5f, 5);
            woodenMount = new SwordHilt("WoodenMount", 21, 2, "Partisan ", 0.9f, 0.8f, 1f, -1, true).defineSpear("WoodenMount", 33, 2);
            ceremonialMount = new SwordHilt("CeremonialMount", 19, 2, "Mounted ", 0.95f, 1f, 1f, 3, true).defineSpear("CeremonialMount", 35, 2);
            eyes = new SwordHilt("EyesHilt", 7, 3, "Vile ", 1f, 1f);
            coe = new SwordHilt("CrownOfEyesHilt", 8, 4, "Corrupted ", 1.04f, 0.9f, 1f, -1);
            demoniteBat = new SwordHilt("DemoniteBatHilt", 6, 4, "Shadowforged ", 1f, 1.1f, 0.4f, 1);
            boneMount = new SwordHilt("BoneMount", 20, 2, "Partisan ", 0.8f, 0.9f, 0.5f, 0, true).defineSpear("BoneMount", 29, 2);
            boneCrest = new SwordHilt("BoneCrest", 9, 2, "Crested ", 1f, 1.1f, 0.4f, 0, false, true);
            demonEye = new SwordHilt("DemonEyeHilt", 7, 3, "Demonforged ", 1f, 1f, 1f, 3);
            violetCrossguard = new SwordHilt("VioletRunicCrossHilt", 8, 5, "Runic ", 1f, 1f, 0.3f);
            purpleCrossguard = new SwordHilt("PurpleRunicCrossHilt", 8, 4, "Arcane ", 1.25f, 1.1f, 1f, 4, false, false, 4);
            goldenRunicKatana = new SwordHilt("GoldenRunicKatanaHilt", 10, 1, "Runeforged ", 1.1f, 1.1f, 3);
            violetRunicKatana = new SwordHilt("VioletRunicKatanaHilt", 10, 1, "Glyphic ", 1.05f, 1f, 1f);
            arcaneMount = new SwordHilt("ArcaneMount", 19, 3, "Mounted ", 0.9f, 1f, 0.4f, 2, true, false, 3).defineSpear("ArcaneMount", 33, 3);
            stick = new SwordHilt("StickHilt", 6, 0, "Primitive ", 1f, 0.9f, 1f);
            hellstoneBasket = new SwordHilt("HellstoneBasketHilt", 9, 2, "Brutish ", 1f, 0.9f, 1f, 0, false, false, 0, 0.1f);
            hellstoneCrossguard = new SwordHilt("HellstoneCrossHilt", 7, 4, "Hellforged ", 1f, 1f, 0.5f, 4, false, false, 0, 0.05f);
            torch = new SwordHilt("TorchHilt", 9, 4, "Underworldly ", 0.92f, 1.1f, 0, 10);
            hellstoneMount = new SwordHilt("HellstoneMount", 21, 2, "Mounted ", 0.9f, 1.05f, 2, 0, true).defineSpear("HellstoneMount", 32, 2);
            bone = new SwordHilt("BoneHilt", 9, 3, "Skeletal ", 1.1f, 0.8f, 0f, 0, false, false, 0, 0.05f);
            clock = new SwordHilt("ClockHilt", 13, 6, "Clockwork ", 0.96f, 1.1f, 0, 0, false, true).SetAccentOffset(new Point(1, 2));
            carbon = new SwordHilt("CarbonHilt", 11, 6, "High-tech ", 1.05f, 1f, 1f, 6, false, false, 2);
            thorns = new SwordHilt("ThornHilt", 6, 7, "Thorny ", 1.1f, 0.8f, 1f);
            chlorophyteMount = new SwordHilt("ChlorophyteMount", 19, 4, "Mounted ", 0.9f, 0.9f, 1f, 0, true).defineSpear("ChlorophyteMount", 35, 4);
            ominousHilt = new SwordHilt("OminousHilt", 8, 6, "Ominous ", 1f, 0.9f, 2f, 2, false, true, 2, 0.08f);

            hiltsByTheme = new Dictionary<SWORDTHEME, List<SwordHilt>>()
            {
                {SWORDTHEME.GENERIC, new List<SwordHilt>(){ ceremonial, copperCrossguard, goldenKatana, blackKatana, wooden, lead, woodenMount, ceremonialMount, demoniteBat, stick }},
                {SWORDTHEME.MONSTROUS, new List<SwordHilt>(){ eyes, coe, boneMount, boneCrest, demonEye }},
                {SWORDTHEME.RUNIC, new List<SwordHilt>(){ purpleCrossguard, violetCrossguard, goldenRunicKatana, violetRunicKatana, arcaneMount }},
                {SWORDTHEME.HELLISH, new List<SwordHilt>(){ hellstoneBasket, hellstoneCrossguard, torch, hellstoneMount, bone, boneMount }},
                {SWORDTHEME.HARDMODE, new List<SwordHilt>(){ clock, carbon, thorns, chlorophyteMount, ominousHilt }}
            };
        }

        public static SwordHilt RandomHilt(SWORDTHEME theme)
        {
            return hiltsByTheme[theme].Random();
        }

        public static void Unload()
        {
            foreach (SwordHilt hilt in hilts.Values)
                hilt.texture = null;
        }
    }
}
