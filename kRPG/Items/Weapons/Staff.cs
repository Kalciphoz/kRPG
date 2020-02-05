using System.Collections.Generic;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class Staff : StaffPart
    {
        public Staff(string texture, int originX, int originY, int useTime, float knockBack, string prefix, float shootSpeed = 8f, bool front = false,
            float manaMultiplier = 1f, int critBonus = 0, int iterations = 1, bool autoSwing = true)
        {
            Type = Staffs.Count + 1;
            if (Main.netMode != 2)
                Texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Staves/" + texture);
            Origin = new Vector2(originX, originY);
            UseTime = useTime;
            KnockBack = knockBack;
            Iterations = iterations;
            Prefix = prefix;
            Mana = manaMultiplier;
            CritBonus = critBonus;
            AutoSwing = autoSwing;
            ShootSpeed = shootSpeed;
            Front = front;

            if (!Staffs.ContainsKey(Type))
                Staffs.Add(Type, this);
        }

        public static Staff Arcane { get; set; }
        public bool AutoSwing { get; set; }
        public static Staff Bone { get; set; }
        public static Staff Branch { get; set; }
        public static Staff Carved { get; set; }
        public int CritBonus { get; set; }

        public Dictionary<Element, float> EleDamage { get; set; } =
            new Dictionary<Element, float> {{Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}};

        public bool Front { get; set; }
        public static Staff Gilded { get; set; }
        public static Staff Hellstone { get; set; }
        public int Iterations { get; set; }
        public static Staff Ivy { get; set; }
        public float KnockBack { get; set; }
        public float Mana { get; set; }
        public string Prefix { get; set; }
        public float ShootSpeed { get; set; }

        public static Dictionary<int, Staff> Staffs { get; set; }
        public static Dictionary<StaffTheme, List<Staff>> StaffsByTheme { get; set; }

        public int Type { get; set; }
        public int UseTime { get; set; }

        public static void Initialize()
        {
            Staffs = new Dictionary<int, Staff>();

            Carved = new Staff("Carved", 14, 1, 28, 4f, "Carved ", 10f);
            Branch = new Staff("Branch", 14, 1, 33, 3f, "Rustic ", 6f, true);
            Ivy = new Staff("Ivy", 13, 1, 18, 2f, "Ivy ", 6f, false, 2.5f, -1, 3);
            Arcane = new Staff("Arcane", 16, 2, 24, 5f, "Arcane ", 11f, true, 1.1f, 5);
            Gilded = new Staff("Gilded", 17, 2, 38, 6f, "Scholarly ", 7f, true, 0.9f);
            Hellstone = new Staff("Hellstone", 16, 2, 26, 5f, "Molten ", 9f, true);
            Bone = new Staff("Bone", 17, 3, 36, 6f, "Underworldly ", 11f, true);

            StaffsByTheme = new Dictionary<StaffTheme, List<Staff>>
            {
                {StaffTheme.Wooden, new List<Staff> {Carved, Branch, Ivy}},
                {StaffTheme.Dungeon, new List<Staff> {Arcane, Gilded}},
                {StaffTheme.Underworld, new List<Staff> {Hellstone, Bone}}
            };
        }

        public static Staff RandomStaff(StaffTheme theme)
        {
            return StaffsByTheme[theme].Random();
        }

        public Staff SetEleDamage(Dictionary<Element, float> eleDamage)
        {
            EleDamage = eleDamage;
            return this;
        }

        public static void Unload()
        {
            foreach (Staff staff in Staffs.Values)
                staff.Texture = null;
        }
    }
}