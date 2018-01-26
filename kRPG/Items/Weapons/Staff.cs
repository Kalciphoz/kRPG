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
    public class Staff : StaffPart
    {
        public static Staff carved;
        public static Staff branch;
        public static Staff ivy;
        public static Staff arcane;
        public static Staff gilded;
        public static Staff hellstone;
        public static Staff bone;

        public static Dictionary<int, Staff> staves;
        public static Dictionary<STAFFTHEME, List<Staff>> stavesByTheme;

        public int type = 0;
        public int useTime;
        public int iterations = 1;
        public float knockBack;
        public int critBonus = 0;
        public bool autoswing = true;
        public string prefix = "";
        public float mana = 1f;
        public float shootSpeed = 8f;
        public bool front = false;

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>()
        {
            { ELEMENT.FIRE, 0f },
            { ELEMENT.COLD, 0f },
            { ELEMENT.LIGHTNING, 0f },
            { ELEMENT.SHADOW, 0f }
        };

        public Staff(string texture, int origin_x, int origin_y, int useTime, float knockBack, string prefix, float shootSpeed = 8f, bool front = false, float manaMultiplier = 1f, int critBonus = 0, int iterations = 1, bool autoswing = true)
        {
            this.type = staves.Count + 1;
            if (Main.netMode != 2)
                this.texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Staves/" + texture);
            this.origin = new Vector2(origin_x, origin_y);
            this.useTime = useTime;
            this.knockBack = knockBack;
            this.iterations = iterations;
            this.prefix = prefix;
            this.mana = manaMultiplier;
            this.critBonus = critBonus;
            this.autoswing = autoswing;
            this.shootSpeed = shootSpeed;
            this.front = front;

            if (!staves.ContainsKey(type))
                staves.Add(type, this);
        }

        public Staff SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            this.eleDamage = eleDamage;
            return this;
        }

        public static void Initialize()
        {
            staves = new Dictionary<int, Staff>();

            carved = new Staff("Carved", 14, 1, 28, 4f, "Carved ", 10f);
            branch = new Staff("Branch", 14, 1, 33, 3f, "Rustic ", 6f, true);
            ivy = new Staff("Ivy", 13, 1, 18, 2f, "Ivy ", 6f, false, 2.5f, -1, 3);
            arcane = new Staff("Arcane", 16, 2, 24, 5f, "Arcane ", 11f, true, 1.1f, 5);
            gilded = new Staff("Gilded", 17, 2, 38, 6f, "Scholarly ", 7f, true, 0.9f);
            hellstone = new Staff("Hellstone", 16, 2, 26, 5f, "Molten ", 9f, true);
            bone = new Staff("Bone", 17, 3, 36, 6f, "Underworldly ", 11f, true);

            stavesByTheme = new Dictionary<STAFFTHEME, List<Staff>>()
            {
                { STAFFTHEME.WOODEN, new List<Staff>() { carved, branch, ivy } },
                { STAFFTHEME.DUNGEON, new List<Staff>() { arcane, gilded } },
                { STAFFTHEME.UNDERWORLD, new List<Staff>() { hellstone, bone } }
            };
        }

        public static Staff RandomStaff(STAFFTHEME theme)
        {
            return stavesByTheme[theme].Random();
        }

        public static void Unload()
        {
            foreach (Staff staff in staves.Values)
                staff.texture = null;
        }
    }
}
