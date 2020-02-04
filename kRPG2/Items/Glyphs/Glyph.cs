//  Fairfield Tek L.L.C.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG2.Items.Glyphs
{
    public class Glyph : ModItem
    {
        public bool Initialized { get; set; }

        public bool Minion => this is Star && !(this is Star_Blue);

        public List<GlyphModifier> Modifiers { get; set; } = new List<GlyphModifier>();

        public virtual float BaseDamageModifier()
        {
            return 1f;
        }

        public virtual float BaseManaModifier()
        {
            return 1f;
        }

        public virtual bool CanUse()
        {
            return true;
        }

        public override ModItem Clone(Item tItem)
        {
            var copy = (Glyph) base.Clone(tItem);
            copy.Modifiers = new List<GlyphModifier>();
            if (Modifiers == null)
                return copy;
            foreach (var modifier in Modifiers)
                copy.Modifiers.Add(modifier);
            return copy;
        }

        public float DamageModifier()
        {
            return ModifierDamageModifier() * BaseDamageModifier();
        }

        public virtual Action<ProceduralSpellProj> GetAiAction()
        {
            return null;
        }

        public virtual Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return null;
        }

        public virtual Action<ProceduralSpellProj, NPC, int> GetImpactAction()
        {
            return null;
        }

        public virtual Action<ProceduralSpellProj> GetInitAction()
        {
            return null;
        }

        public virtual Action<ProceduralSpellProj> GetKillAction()
        {
            return null;
        }

        public static string GetRandom()
        {
            switch (Main.rand.Next(26))
            {
                default:
                    return "Star_Blue";
                case 1:
                    return "Star_Orange";
                case 2:
                    return "Star_Purple";
                case 3:
                    return "Cross_Red";
                case 4:
                    return "Cross_Orange";
                case 5:
                    return "Cross_Yellow";
                case 6:
                    return "Cross_Green";
                case 7:
                    return "Cross_Blue";
                case 8:
                    return "Cross_Violet";
                case 9:
                    return "Cross_Purple";
                case 10:
                    return "Moon_Yellow";
                case 11:
                    return "Moon_Green";
                case 12:
                    return "Moon_Blue";
                case 13:
                    return "Moon_Violet";
                case 14:
                    return "Cross_Orange";
                case 15:
                    return "Cross_Green";
                case 16:
                    return "Moon_Yellow";
                case 17:
                    return "Moon_Green";
                case 18:
                    return "Moon_Blue";
                case 19:
                    return "Moon_Violet";
                case 20:
                    return "Star_Orange";
                case 21:
                    return "Star_Purple";
                case 22:
                    return "Moon_Purple";
            }
        }

        public virtual Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return null;
        }

        public override void Load(TagCompound tag)
        {
            Modifiers.Clear();
            int count = tag.GetInt("ModifierCount");
            for (int i = 0; i < count; i += 1)
                Modifiers.Add(GlyphModifier.Modifiers[tag.GetInt("Modifier_" + i)]);
            Initialized = true;
        }

        public float ManaModifier()
        {
            return ModifierManaModifier() * BaseManaModifier();
        }

        public float ModifierDamageModifier()
        {
            return Modifiers.Aggregate(1f, (current, modi) => current * modi.DamageModifier);
        }

        public float ModifierManaModifier()
        {
            return Modifiers.Aggregate(1f, (current, modi) => current * modi.ManaModifier);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < Modifiers.Count; i += 1)
                tooltips.Add(new TooltipLine(mod, "modifier" + i, Modifiers[i].Tooltip));
            tooltips.Add(new TooltipLine(mod, "damage", (int) Math.Round(DamageModifier() * 100) + "% damage"));
            tooltips.Add(new TooltipLine(mod, "mana", (int) Math.Round(ManaModifier() * 100) + "% mana cost"));
        }

        public override void NetRecieve(BinaryReader reader)
        {
            Modifiers.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i += 1)
                Modifiers.Add(GlyphModifier.Modifiers[reader.ReadInt32()]);
            Initialized = true;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Modifiers.Count);
            for (int i = 0; i < Modifiers.Count; i += 1)
                writer.Write(Modifiers[i].Id);
        }

        public virtual void Randomize()
        {
            Initialized = true;
            foreach (var modifier in GlyphModifier.Modifiers.Where(modifier => modifier.Match(this) && modifier.Odds()))
                Modifiers.Add(modifier.Group == null ? modifier : modifier.Group());
        }

        public override TagCompound Save()
        {
            var compound = new TagCompound {{"ModifierCount", Modifiers.Count}};
            for (int i = 0; i < Modifiers.Count; i += 1)
                compound.Add("Modifier_" + i, Modifiers[i].Id);
            return compound;
        }

        public override void SetDefaults()
        {
            if (item == null) return;
            item.width = 48;
            item.height = 48;
            item.value = 2500;
            item.rare = 2;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Glyph; Please Ignore");
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode == 0) return;
            if (!Initialized) Randomize();
        }
    }
}