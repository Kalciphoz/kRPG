using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class SwordBlade
    {
        private static Dictionary<SwordTheme, List<SwordBlade>> bladeByTheme;

        public SwordBlade(string texture, int originX, int originY, string name, int useTime, float knockBack, float dpsModifier = 1f, int critBonus = 0,
            bool autoSwing = false, float scale = 0f, bool spearable = true, bool lighted = false, Action<Rectangle, Player> effect = null)
        {
            Type = Blades.Count() + 1;
            if (Main.netMode != 2)
                Texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Blades/" + texture);
            Origin = new Vector2(originX, originY);
            Name = name;
            UseTime = useTime;
            KnockBack = knockBack;
            DpsModifier = dpsModifier;
            CritBonus = critBonus;
            Scale = scale;
            AutoSwing = autoSwing;
            Spearable = spearable;
            Effect = effect;
            Lighted = lighted;

            if (!Blades.ContainsKey(Type))
                Blades.Add(Type, this);
        }

        public bool AutoSwing { get; set; }

        public static Dictionary<int, SwordBlade> Blades { get; set; } = new Dictionary<int, SwordBlade>();
        public static SwordBlade BlazeSword { get; set; }
        public static SwordBlade BroadswordBone { get; set; }
        public static SwordBlade BroadswordSilver { get; set; }
        public static SwordBlade BroadswordVile { get; set; }
        public static SwordBlade BrutishDagger { get; set; }
        public static SwordBlade Chokuto { get; set; }
        public static SwordBlade ClockSword { get; set; }
        public static SwordBlade CrescentSword { get; set; }
        public int CritBonus { get; set; }
        public static SwordBlade DemonEye { get; set; }
        public float DpsModifier { get; set; }
        public Action<Rectangle, Player> Effect { get; set; }

        public Dictionary<Element, float> EleDamage { get; set; } = new Dictionary<Element, float>
        {
            {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}
        };

        public static SwordBlade Executioner { get; set; }
        public static SwordBlade EyeMallet { get; set; }
        public static SwordBlade FieldGlaive { get; set; }
        public static SwordBlade FieldSword { get; set; }
        public static SwordBlade FieryBroadsword { get; set; }
        public static SwordBlade FieryGreatsword { get; set; }
        public static SwordBlade GrassBreaker { get; set; }
        public static SwordBlade HellstoneBroadsword { get; set; }

        public float KnockBack { get; set; }
        public static SwordBlade LazerCutter { get; set; }
        public bool Lighted { get; set; }
        public string Name { get; set; }
        public static SwordBlade ObsidianBroadsword { get; set; }
        public static SwordBlade ObsidianMaul { get; set; }
        public Vector2 Origin { get; set; }
        public static SwordBlade PhaseSword { get; set; }
        public static SwordBlade RunicDaiKatana { get; set; }
        public static SwordBlade RunicswordViolet { get; set; }
        public float Scale { get; set; }
        public static SwordBlade Scimitar { get; set; }
        public static SwordBlade SlimeBlue { get; set; }
        public static SwordBlade SlimeGreen { get; set; }
        public static SwordBlade SlumTwirl { get; set; }
        public bool Spearable { get; set; } = true;
        public static SwordBlade StoneSword { get; set; }
        public static SwordBlade Terra { get; set; }
        public Texture2D Texture { get; set; }

        public int Type { get; set; }
        public int UseTime { get; set; }
        public static SwordBlade WizardSword { get; set; }

        public static void Initialize()
        {
            Blades = new Dictionary<int, SwordBlade>();
            bladeByTheme = new Dictionary<SwordTheme, List<SwordBlade>>();

            SlimeBlue = new SwordBlade("BlueSlimeBlade", 1, 10, "Slime Sword", 28, 7f, 1.05f, -3, false, 0.05f, false);
            SlimeGreen = new SwordBlade("GreenSlimeBlade", 1, 10, "Slime Sword", 30, 7f, 1.03f, -3, false, 0f, false);
            BrutishDagger = new SwordBlade("BrutishDagger", 2, 9, "Dagger", 31, 6.5f, 1.05f);
            Chokuto = new SwordBlade("Chokuto", 1, 14, "Chokuto", 22, 2f, 0.95f, 7, true);
            BroadswordSilver = new SwordBlade("SilverBroadsword", 1, 11, "Broadsword", 24, 5.5f);
            Scimitar = new SwordBlade("Scimitar", 1, 12, "Scimitar", 27, 4f, 0.98f, 0, true);
            FieldGlaive = new SwordBlade("FieldGlaive", 2, 17, "Glaive", 47, 7f, 0.9f, 0, false, 0.05f, false, true);
            StoneSword = new SwordBlade("StoneSword", 2, 16, "Stone Sword", 34, 6f, 0.97f);
            CrescentSword = new SwordBlade("CrescentSword", 1, 13, "Crescent Sword", 29, 3f, 1.05f, -2);
            FieldSword = new SwordBlade("ClaymoreBlade", 2, 17, "Field Sword", 41, 4f, 0.9f, 2, true);
            EyeMallet = new SwordBlade("EyeMallet", 2, 18, "Eye Mallet", 42, 6f, 0.9f, -1, false, 0f, false).SetEleDamage(new Dictionary<Element, float>
            {
                {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0.25f}
            });
            BroadswordVile = new SwordBlade("VileBroadsword", 1, 15, "Meatfang", 29, 4.5f).SetEleDamage(new Dictionary<Element, float>
            {
                {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0.15f}
            });
            SlumTwirl = new SwordBlade("SlumTwirl", 2, 18, "Twirlsword", 32, 3f, 1f, -3);
            BroadswordBone = new SwordBlade("BoneBroadsword", 1, 15, "Bone Sword", 28, 4f, 0.95f, 0, true).SetEleDamage(new Dictionary<Element, float>
            {
                {Element.Fire, 0f}, {Element.Cold, 0.2f}, {Element.Lightning, 0f}, {Element.Shadow, 0.1f}
            });
            DemonEye = new SwordBlade("EyeSword", 1, 17, "Eyes on a Stick", 22, 4f).SetEleDamage(new Dictionary<Element, float>
            {
                {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0.2f}
            });
            WizardSword = new SwordBlade("WizardSword", 2, 18, "Wizard Sword", 19, 1f, 1.15f, 0, true, 0f, true, true);
            RunicswordViolet = new SwordBlade("VioletRunicSword", 2, 13, "Sword", 28, 5f, 1f, 0, false, 0.1f, true, true);
            RunicDaiKatana = new SwordBlade("RunicDaiKatana", 2, 17, "Dai-Katana", 33, 4f, 0.95f, 5, true, 0f, true, true);
            BlazeSword = new SwordBlade("BlazeSword", 4, 14, "Lantern", 26, 3f, 1.05f, 0, false, 0f, true, true, delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(7) >= 2)
                    return;
                int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f,
                    player.velocity.Y * 0.2f, 63);
                Main.dust[dust].noGravity = true;
            }).SetEleDamage(new Dictionary<Element, float> {{Element.Fire, 0.4f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0.1f}});
            Executioner = new SwordBlade("ExecutionerBlade", 2, 20, "Execution Sword", 33, 7f, 1f, 0, false, 0.2f, false);
            ObsidianBroadsword = new SwordBlade("ObsidianGreatsword", 2, 15, "Obsidian Sword", 26, 5.5f, 1.1f, 1, false, 0.1f);
            ObsidianMaul = new SwordBlade("ObsidianMaul", 2, 18, "Obsidian Maul", 44, 8.5f, 0.95f, 0, false, 0.15f, false);
            HellstoneBroadsword = new SwordBlade("HellstoneBroadsword", 2, 13, "Hellstone Broadsword", 23, 4.5f, 1.1f, 2, true, 0.1f);
            FieryBroadsword = new SwordBlade("FieryBroadsword", 2, 15, "Fiery Field Sword", 29, 4f, 0.92f, 0, true, 0.1f, true, true,
                delegate(Rectangle rect, Player player)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f,
                        player.velocity.Y * 0.2f, 63, new Color(), 2f);
                    Main.dust[dust].noGravity = true;
                }).SetEleDamage(new Dictionary<Element, float> {{Element.Fire, 0.5f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}});
            FieryGreatsword = new SwordBlade("FieryGreatsword", 2, 16, "Fiery Greatsword", 36, 6f, 0.96f, 0, false, 0.2f, true, true,
                delegate(Rectangle rect, Player player)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f,
                        player.velocity.Y * 0.2f, 63, new Color(), 2f);
                    Main.dust[dust].noGravity = true;
                }).SetEleDamage(new Dictionary<Element, float> {{Element.Fire, 0.5f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}});
            ClockSword = new SwordBlade("ClockSword", 6, 25, "Chronosword", 19, 4f, 1f, 0, true);
            GrassBreaker = new SwordBlade("GrassBreaker", 3, 32, "Buster Claymore", 31, 7.5f).SetEleDamage(new Dictionary<Element, float>
            {
                {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0.2f}, {Element.Shadow, 0f}
            });
            LazerCutter = new SwordBlade("LazerCutter", 3, 33, "Laser Cutter", 40, 5f).SetEleDamage(new Dictionary<Element, float>
            {
                {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0.5f}, {Element.Shadow, 0f}
            });
            PhaseSword = new SwordBlade("PhaseSword", 2, 21, "Phase Sword", 25, 5f, 1.06f, 0, true, 0f, true, true).SetEleDamage(
                new Dictionary<Element, float> {{Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0.4f}, {Element.Shadow, 0f}});
            Terra = new SwordBlade("ElementalNeedle", 2, 21, "Elemental Needle", 15, 5f, 1.1f);

            bladeByTheme = new Dictionary<SwordTheme, List<SwordBlade>>
            {
                {
                    SwordTheme.Generic, new List<SwordBlade>
                    {
                        BrutishDagger,
                        Chokuto,
                        BroadswordSilver,
                        CrescentSword,
                        FieldGlaive,
                        Scimitar,
                        StoneSword,
                        FieldSword
                    }
                },
                {
                    SwordTheme.Monstrous, new List<SwordBlade>
                    {
                        Chokuto,
                        BroadswordSilver,
                        EyeMallet,
                        BroadswordVile,
                        SlumTwirl,
                        BroadswordBone,
                        DemonEye
                    }
                },
                {
                    SwordTheme.Runic, new List<SwordBlade>
                    {
                        WizardSword,
                        RunicswordViolet,
                        RunicDaiKatana,
                        FieldGlaive,
                        BlazeSword
                    }
                },
                {
                    SwordTheme.Hellish, new List<SwordBlade>
                    {
                        Executioner,
                        ObsidianBroadsword,
                        ObsidianMaul,
                        HellstoneBroadsword,
                        FieryBroadsword,
                        FieryGreatsword,
                        BroadswordBone
                    }
                },
                {
                    SwordTheme.Hardmode, new List<SwordBlade>
                    {
                        ClockSword,
                        GrassBreaker,
                        LazerCutter,
                        PhaseSword,
                        Executioner,
                        Terra
                    }
                }
            };
        }

        public static SwordBlade RandomBlade(SwordTheme theme)
        {
            return bladeByTheme[theme].Random();
        }

        public SwordBlade SetEleDamage(Dictionary<Element, float> eleDamage)
        {
            EleDamage = eleDamage;
            return this;
        }

        public static void Unload()
        {
            foreach (SwordBlade blade in Blades.Values)
                blade.Texture = null;
        }
    }
}