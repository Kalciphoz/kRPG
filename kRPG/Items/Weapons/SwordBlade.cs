using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class SwordBlade
    {
        public static SwordBlade slimeBlue;
        public static SwordBlade slimeGreen;
        public static SwordBlade brutishDagger;
        public static SwordBlade chokuto;
        public static SwordBlade broadswordSilver;
        public static SwordBlade scimitar;
        public static SwordBlade fieldGlaive;
        public static SwordBlade stoneSword;
        public static SwordBlade crescentSword;
        public static SwordBlade fieldSword;
        public static SwordBlade eyeMallet;
        public static SwordBlade slumTwirl;
        public static SwordBlade broadswordVile;
        public static SwordBlade broadswordBone;
        public static SwordBlade demonEye;
        public static SwordBlade wizardSword;
        public static SwordBlade runicswordViolet;
        public static SwordBlade runicDaiKatana;
        public static SwordBlade blazeSword;
        public static SwordBlade executioner;
        public static SwordBlade obsidianBroadsword;
        public static SwordBlade obsidianMaul;
        public static SwordBlade fieryGreatsword;
        public static SwordBlade fieryBroadsword;
        public static SwordBlade hellstoneBroadsword;
        public static SwordBlade clockSword;
        public static SwordBlade grassBreaker;
        public static SwordBlade lazerCutter;
        public static SwordBlade phaseSword;
        public static SwordBlade terra;

        private static Dictionary<SWORDTHEME, List<SwordBlade>> bladeByTheme;

        public static Dictionary<int, SwordBlade> blades = new Dictionary<int, SwordBlade>();

        public int type = 0;
        public Texture2D texture;
        public Vector2 origin;
        public string name;
        public float dpsModifier;
        public int useTime;
        public float knockBack;
        public int critBonus = 0;
        public float scale = 0f;
        public bool autoswing = false;
        public bool spearable = true;
        public bool lighted = false;
        public Action<Rectangle, Player> effect;

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>()
        {
            { ELEMENT.FIRE, 0f },
            { ELEMENT.COLD, 0f },
            { ELEMENT.LIGHTNING, 0f },
            { ELEMENT.SHADOW, 0f }
        };

        public SwordBlade(string texture, int origin_x, int origin_y, string name, int useTime, float knockBack, float dpsModifier = 1f, int critBonus = 0, bool autoswing = false, float scale = 0f, bool spearable = true, bool lighted = false, Action<Rectangle, Player> effect = null)
        {
            this.type = blades.Count() + 1;
            if (Main.netMode != 2)
                this.texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Blades/" + texture);
            this.origin = new Vector2(origin_x, origin_y);
            this.name = name;
            this.useTime = useTime;
            this.knockBack = knockBack;
            this.dpsModifier = dpsModifier;
            this.critBonus = critBonus;
            this.scale = scale;
            this.autoswing = autoswing;
            this.spearable = spearable;
            this.effect = effect;
            this.lighted = lighted;

            if (!blades.ContainsKey(type))
                blades.Add(type, this);
        }

        public SwordBlade SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            this.eleDamage = eleDamage;
            return this;
        }

        public static void Initialize()
        {
            blades = new Dictionary<int, SwordBlade>();
            bladeByTheme = new Dictionary<SWORDTHEME, List<SwordBlade>>();

            slimeBlue = new SwordBlade("BlueSlimeBlade", 1, 10, "Slime Sword", 28, 7f, 1.05f, -3, false, 0.05f, false);
            slimeGreen = new SwordBlade("GreenSlimeBlade", 1, 10, "Slime Sword", 30, 7f, 1.03f, -3, false, 0f, false);
            brutishDagger = new SwordBlade("BrutishDagger", 2, 9, "Dagger", 31, 6.5f, 1.05f);
            chokuto = new SwordBlade("Chokuto", 1, 14, "Chokuto", 22, 2f, 0.95f, 7, true);
            broadswordSilver = new SwordBlade("SilverBroadsword", 1, 11, "Broadsword", 24, 5.5f);
            scimitar = new SwordBlade("Scimitar", 1, 12, "Scimitar", 27, 4f, 0.98f, 0, true);
            fieldGlaive = new SwordBlade("FieldGlaive", 2, 17, "Glaive", 47, 7f, 0.9f, 0, false, 0.05f, false, true);
            stoneSword = new SwordBlade("StoneSword", 2, 16, "Stone Sword", 34, 6f, 0.97f);
            crescentSword = new SwordBlade("CrescentSword", 1, 13, "Crescent Sword", 29, 3f, 1.05f, -2);
            fieldSword = new SwordBlade("ClaymoreBlade", 2, 17, "Field Sword", 41, 4f, 0.9f, 2, true);
            eyeMallet = new SwordBlade("EyeMallet", 2, 18, "Eye Mallet", 42, 6f, 0.9f, -1, false, 0f, false).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0.25f }
            });
            broadswordVile = new SwordBlade("VileBroadsword", 1, 15, "Meatfang", 29, 4.5f).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0.15f }
            });
            slumTwirl = new SwordBlade("SlumTwirl", 2, 18, "Twirlsword", 32, 3f, 1f, -3);
            broadswordBone = new SwordBlade("BoneBroadsword", 1, 15, "Bone Sword", 28, 4f, 0.95f, 0, true, 0f).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0.2f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0.1f }
            });
            demonEye = new SwordBlade("EyeSword", 1, 17, "Eyes on a Stick", 22, 4f).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0.2f }
            });
            wizardSword = new SwordBlade("WizardSword", 2, 18, "Wizard Sword", 19, 1f, 1.15f, 0, true, 0f, true, true);
            runicswordViolet = new SwordBlade("VioletRunicSword", 2, 13, "Sword", 28, 5f, 1f, 0, false, 0.1f, true, true);
            runicDaiKatana = new SwordBlade("RunicDaiKatana", 2, 17, "Dai-Katana", 33, 4f, 0.95f, 5, true, 0f, true, true);
            blazeSword = new SwordBlade("BlazeSword", 4, 14, "Lantern", 26, 3f, 1.05f, 0, false, 0f, true, true, delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(7) < 2)
                {
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 63, new Color());
                    Main.dust[dust].noGravity = true;
                }
            }).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0.4f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0.1f }
            });
            executioner = new SwordBlade("ExecutionerBlade", 2, 20, "Execution Sword", 33, 7f, 1f, 0, false, 0.2f, false);
            obsidianBroadsword = new SwordBlade("ObsidianGreatsword", 2, 15, "Obsidian Sword", 26, 5.5f, 1.1f, 1, false, 0.1f);
            obsidianMaul = new SwordBlade("ObsidianMaul", 2, 18, "Obsidian Maul", 44, 8.5f, 0.95f, 0, false, 0.15f, false);
            hellstoneBroadsword = new SwordBlade("HellstoneBroadsword", 2, 13, "Hellstone Broadsword", 23, 4.5f, 1.1f, 2, true, 0.1f);
            fieryBroadsword = new SwordBlade("FieryBroadsword", 2, 15, "Fiery Field Sword", 29, 4f, 0.92f, 0, true, 0.1f, true, true, delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 63, new Color(), 2f);
                    Main.dust[dust].noGravity = true;
                }
            }).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0.5f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0f }
            });
            fieryGreatsword = new SwordBlade("FieryGreatsword", 2, 16, "Fiery Greatsword", 36, 6f, 0.96f, 0, false, 0.2f, true, true, delegate (Rectangle rect, Player player)
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 63, new Color(), 2f);
                    Main.dust[dust].noGravity = true;
                }
            }).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0.5f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0f },
                { ELEMENT.SHADOW, 0f }
            });
            clockSword = new SwordBlade("ClockSword", 6, 25, "Chronosword", 19, 4f, 1f, 0, true);
            grassBreaker = new SwordBlade("GrassBreaker", 3, 32, "Buster Claymore", 31, 7.5f).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0.2f },
                { ELEMENT.SHADOW, 0f }
            });
            lazerCutter = new SwordBlade("LazerCutter", 3, 33, "Laser Cutter", 40, 5f).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0.5f },
                { ELEMENT.SHADOW, 0f }
            });
            phaseSword = new SwordBlade("PhaseSword", 2, 21, "Phase Sword", 25, 5f, 1.06f, 0, true, 0f, true, true).SetEleDamage(new Dictionary<ELEMENT, float>()
            {
                { ELEMENT.FIRE, 0f },
                { ELEMENT.COLD, 0f },
                { ELEMENT.LIGHTNING, 0.4f },
                { ELEMENT.SHADOW, 0f }
            });
            terra = new SwordBlade("ElementalNeedle", 2, 21, "Elemental Needle", 15, 5f, 1.1f, 0);

            bladeByTheme = new Dictionary<SWORDTHEME, List<SwordBlade>>()
            {
                {SWORDTHEME.GENERIC, new List<SwordBlade>(){ brutishDagger, chokuto, broadswordSilver, crescentSword, fieldGlaive, scimitar, stoneSword, fieldSword }},
                {SWORDTHEME.MONSTROUS, new List<SwordBlade>(){ chokuto, broadswordSilver, eyeMallet, broadswordVile, slumTwirl, broadswordBone, demonEye }},
                {SWORDTHEME.RUNIC, new List<SwordBlade>(){ wizardSword, runicswordViolet, runicDaiKatana, fieldGlaive, blazeSword }},
                {SWORDTHEME.HELLISH, new List<SwordBlade>(){ executioner, obsidianBroadsword, obsidianMaul, hellstoneBroadsword, fieryBroadsword, fieryGreatsword, broadswordBone }},
                {SWORDTHEME.HARDMODE, new List<SwordBlade>(){ clockSword, grassBreaker, lazerCutter, phaseSword, executioner, terra }}
            };
        }

        public static SwordBlade RandomBlade(SWORDTHEME theme)
        {
            return bladeByTheme[theme].Random();
        }

        public static void Unload()
        {
            foreach (SwordBlade blade in blades.Values)
                blade.texture = null;
        }
    }
}
