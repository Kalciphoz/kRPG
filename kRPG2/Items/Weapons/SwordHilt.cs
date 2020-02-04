﻿//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
// 
// 
// THIS SOFTWARE IS PROVIDED BY FairfieldTek LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using kRPG2.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Items.Weapons
{
    public class SwordHilt
    {
        public SwordHilt(string texture, int originX, int originY, string prefix, float dpsModifier, float speedModifier, float knockBack = 0f,
            int critBonus = 0, bool spear = false, bool autoSwing = false, int mana = 0, float scale = 0f)
        {
            Type = Hilts.Count + 1;
            if (Main.netMode != 2)
                Texture = ModLoader.GetMod("kRPG2").GetTexture("GFX/Items/Swordhilts/" + texture);
            Origin = new Vector2(originX, originY);
            DpsModifier = dpsModifier;
            SpeedModifier = speedModifier;
            KnockBack = knockBack;
            CritBonus = critBonus;
            AutoSwing = autoSwing;
            Prefix = prefix;
            Mana = mana;
            Spear = spear;
            AccentOffset = Point.Zero;
            Scale = scale;

            if (!Hilts.ContainsKey(Type))
                Hilts.Add(Type, this);
        }

        public Point AccentOffset { get; set; }
        public static SwordHilt ArcaneMount { get; set; }
        public bool AutoSwing { get; set; }
        public static SwordHilt BlackKatana { get; set; }
        public static SwordHilt Bone { get; set; }
        public static SwordHilt BoneCrest { get; set; }
        public static SwordHilt BoneMount { get; set; }
        public static SwordHilt Carbon { get; set; }
        public static SwordHilt Ceremonial { get; set; }
        public static SwordHilt CeremonialMount { get; set; }
        public static SwordHilt ChlorophyteMount { get; set; }
        public static SwordHilt Clock { get; set; }
        public static SwordHilt Coe { get; set; }
        public static SwordHilt CopperCrossguard { get; set; }
        public int CritBonus { get; set; }
        public static SwordHilt DemonEye { get; set; }
        public static SwordHilt DemoniteBat { get; set; }
        public float DpsModifier { get; set; }
        public static SwordHilt Eyes { get; set; }
        public static SwordHilt GoldenKatana { get; set; }
        public static SwordHilt GoldenRunicKatana { get; set; }
        public static SwordHilt HellstoneBasket { get; set; }
        public static SwordHilt HellstoneCrossguard { get; set; }
        public static SwordHilt HellstoneMount { get; set; }

        public static Dictionary<int, SwordHilt> Hilts { get; set; } = new Dictionary<int, SwordHilt>();

        private static Dictionary<SWORDTHEME, List<SwordHilt>> HiltsByTheme { get; set; }
        public static SwordHilt IronBasket { get; set; }
        public float KnockBack { get; set; }
        public static SwordHilt Lead { get; set; }
        public int Mana { get; set; }
        public static SwordHilt OminousHilt { get; set; }
        public Vector2 Origin { get; set; }
        public string Prefix { get; set; }
        public static SwordHilt PurpleCrossguard { get; set; }
        public float Scale { get; set; }
        public bool Spear { get; set; }
        public Vector2 SpearOrigin { get; set; }
        public Texture2D SpearTexture { get; set; }
        public float SpeedModifier { get; set; }
        public static SwordHilt Stick { get; set; }
        public Texture2D Texture { get; set; }
        public static SwordHilt Thorns { get; set; }
        public static SwordHilt Torch { get; set; }

        public int Type { get; set; }
        public static SwordHilt VioletCrossguard { get; set; }
        public static SwordHilt VioletRunicKatana { get; set; }
        public static SwordHilt Wooden { get; set; }
        public static SwordHilt WoodenMount { get; set; }

        public SwordHilt DefineSpear(string texture, int origin_x, int origin_y)
        {
            if (Main.netMode != 2)
                SpearTexture = ModLoader.GetMod("kRPG2").GetTexture("GFX/Projectiles/SpearMounts/" + texture);
            SpearOrigin = new Vector2(origin_x, origin_y);
            return this;
        }

        public static void Initialize()
        {
            Hilts = new Dictionary<int, SwordHilt>();
            HiltsByTheme = new Dictionary<SWORDTHEME, List<SwordHilt>>();

            Ceremonial = new SwordHilt("CeremonialHilt", 6, 2, "Ceremonial ", 1f, 1.1f);
            CopperCrossguard = new SwordHilt("CopperCrossHilt", 6, 2, "Crude ", 1.05f, 0.95f, 0f, 0, false, false, 0, 0.1f);
            GoldenKatana = new SwordHilt("GoldenKatanaHilt", 6, 1, "Traditional ", 0.96f, 1.1f, 0f, 5);
            BlackKatana = new SwordHilt("KatanaHilt", 6, 1, "Forged ", 1f, 1.1f, 1.5f, 7, false, true, 1);
            Wooden = new SwordHilt("WoodenHilt", 6, 2, "Training ", 1f, 0.9f, 1f);
            IronBasket = new SwordHilt("IronBasketHilt", 6, 2, "Folded ", 1.02f, 0.85f, 0.5f, -1);
            Lead = new SwordHilt("LeadHilt", 6, 4, "Blacksmithed ", 0.94f, 1.06f, -0.5f, 5);
            WoodenMount = new SwordHilt("WoodenMount", 21, 2, "Partisan ", 0.9f, 0.8f, 1f, -1, true).DefineSpear("WoodenMount", 33, 2);
            CeremonialMount = new SwordHilt("CeremonialMount", 19, 2, "Mounted ", 0.95f, 1f, 1f, 3, true).DefineSpear("CeremonialMount", 35, 2);
            Eyes = new SwordHilt("EyesHilt", 7, 3, "Vile ", 1f, 1f);
            Coe = new SwordHilt("CrownOfEyesHilt", 8, 4, "Corrupted ", 1.04f, 0.9f, 1f, -1);
            DemoniteBat = new SwordHilt("DemoniteBatHilt", 6, 4, "Shadowforged ", 1f, 1.1f, 0.4f, 1);
            BoneMount = new SwordHilt("BoneMount", 20, 2, "Partisan ", 0.8f, 0.9f, 0.5f, 0, true).DefineSpear("BoneMount", 29, 2);
            BoneCrest = new SwordHilt("BoneCrest", 9, 2, "Crested ", 1f, 1.1f, 0.4f, 0, false, true);
            DemonEye = new SwordHilt("DemonEyeHilt", 7, 3, "Demonforged ", 1f, 1f, 1f, 3);
            VioletCrossguard = new SwordHilt("VioletRunicCrossHilt", 8, 5, "Runic ", 1f, 1f, 0.3f);
            PurpleCrossguard = new SwordHilt("PurpleRunicCrossHilt", 8, 4, "Arcane ", 1.25f, 1.1f, 1f, 4, false, false, 4);
            GoldenRunicKatana = new SwordHilt("GoldenRunicKatanaHilt", 10, 1, "Runeforged ", 1.1f, 1.1f, 3);
            VioletRunicKatana = new SwordHilt("VioletRunicKatanaHilt", 10, 1, "Glyphic ", 1.05f, 1f, 1f);
            ArcaneMount = new SwordHilt("ArcaneMount", 19, 3, "Mounted ", 0.9f, 1f, 0.4f, 2, true, false, 3).DefineSpear("ArcaneMount", 33, 3);
            Stick = new SwordHilt("StickHilt", 6, 0, "Primitive ", 1f, 0.9f, 1f);
            HellstoneBasket = new SwordHilt("HellstoneBasketHilt", 9, 2, "Brutish ", 1f, 0.9f, 1f, 0, false, false, 0, 0.1f);
            HellstoneCrossguard = new SwordHilt("HellstoneCrossHilt", 7, 4, "Hellforged ", 1f, 1f, 0.5f, 4, false, false, 0, 0.05f);
            Torch = new SwordHilt("TorchHilt", 9, 4, "Underworldly ", 0.92f, 1.1f, 0, 10);
            HellstoneMount = new SwordHilt("HellstoneMount", 21, 2, "Mounted ", 0.9f, 1.05f, 2, 0, true).DefineSpear("HellstoneMount", 32, 2);
            Bone = new SwordHilt("BoneHilt", 9, 3, "Skeletal ", 1.1f, 0.8f, 0f, 0, false, false, 0, 0.05f);
            Clock = new SwordHilt("ClockHilt", 13, 6, "Clockwork ", 0.96f, 1.1f, 0, 0, false, true).SetAccentOffset(new Point(1, 2));
            Carbon = new SwordHilt("CarbonHilt", 11, 6, "High-tech ", 1.05f, 1f, 1f, 6, false, false, 2);
            Thorns = new SwordHilt("ThornHilt", 6, 7, "Thorny ", 1.1f, 0.8f, 1f);
            ChlorophyteMount = new SwordHilt("ChlorophyteMount", 19, 4, "Mounted ", 0.9f, 0.9f, 1f, 0, true).DefineSpear("ChlorophyteMount", 35, 4);
            OminousHilt = new SwordHilt("OminousHilt", 8, 6, "Ominous ", 1f, 0.9f, 2f, 2, false, true, 2, 0.08f);

            HiltsByTheme = new Dictionary<SWORDTHEME, List<SwordHilt>>
            {
                {
                    SWORDTHEME.GENERIC, new List<SwordHilt>
                    {
                        Ceremonial,
                        CopperCrossguard,
                        GoldenKatana,
                        BlackKatana,
                        Wooden,
                        Lead,
                        WoodenMount,
                        CeremonialMount,
                        DemoniteBat,
                        Stick
                    }
                },
                {
                    SWORDTHEME.MONSTROUS, new List<SwordHilt>
                    {
                        Eyes,
                        Coe,
                        BoneMount,
                        BoneCrest,
                        DemonEye
                    }
                },
                {
                    SWORDTHEME.RUNIC, new List<SwordHilt>
                    {
                        PurpleCrossguard,
                        VioletCrossguard,
                        GoldenRunicKatana,
                        VioletRunicKatana,
                        ArcaneMount
                    }
                },
                {
                    SWORDTHEME.HELLISH, new List<SwordHilt>
                    {
                        HellstoneBasket,
                        HellstoneCrossguard,
                        Torch,
                        HellstoneMount,
                        Bone,
                        BoneMount
                    }
                },
                {
                    SWORDTHEME.HARDMODE, new List<SwordHilt>
                    {
                        Clock,
                        Carbon,
                        Thorns,
                        ChlorophyteMount,
                        OminousHilt
                    }
                }
            };
        }

        public static SwordHilt RandomHilt(SWORDTHEME theme)
        {
            return HiltsByTheme[theme].Random();
        }

        public SwordHilt SetAccentOffset(Point offset)
        {
            AccentOffset = offset;
            return this;
        }

        public static void Unload()
        {
            foreach (var hilt in Hilts.Values)
                hilt.Texture = null;
        }
    }
}