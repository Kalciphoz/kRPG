// Kalciphoz's RPG Mod
//  Copyright (c) 2016, Kalciphoz's RPG Mod
// 
// 
// THIS SOFTWARE IS PROVIDED BY Kalciphoz's ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
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

        public Dictionary<ELEMENT, float> EleDamage { get; set; } =
            new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}};

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
        public static Dictionary<STAFFTHEME, List<Staff>> StaffsByTheme { get; set; }

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

            StaffsByTheme = new Dictionary<STAFFTHEME, List<Staff>>
            {
                {STAFFTHEME.WOODEN, new List<Staff> {Carved, Branch, Ivy}},
                {STAFFTHEME.DUNGEON, new List<Staff> {Arcane, Gilded}},
                {STAFFTHEME.UNDERWORLD, new List<Staff> {Hellstone, Bone}}
            };
        }

        public static Staff RandomStaff(STAFFTHEME theme)
        {
            return StaffsByTheme[theme].Random();
        }

        public Staff SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
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