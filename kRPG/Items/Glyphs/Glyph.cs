using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.Items.Glyphs
{
    public class Glyph : ModItem
    {
        public bool initialized;

        public List<GlyphModifier> modifiers = new List<GlyphModifier>();

        public bool minion => this is Star && !(this is Star_Blue);

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
            Glyph copy = (Glyph) base.Clone(tItem);
            copy.modifiers = new List<GlyphModifier>();
            if (modifiers == null)
                return copy;
            foreach (GlyphModifier modifier in modifiers)
                copy.modifiers.Add(modifier);
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
            modifiers.Clear();
            int count = tag.GetInt("ModifierCount");
            for (int i = 0; i < count; i += 1)
                modifiers.Add(GlyphModifier.modifiers[tag.GetInt("Modifier_" + i)]);
            initialized = true;
        }

        public float ManaModifier()
        {
            return ModifierManaModifier() * BaseManaModifier();
        }

        public float ModifierDamageModifier()
        {
            return modifiers.Aggregate(1f, (current, modi) => current * modi.dmgModifier);
        }

        public float ModifierManaModifier()
        {
            return modifiers.Aggregate(1f, (current, modi) => current * modi.manaModifier);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < modifiers.Count; i += 1)
                tooltips.Add(new TooltipLine(mod, "modifier" + i, modifiers[i].tooltip));
            tooltips.Add(new TooltipLine(mod, "damage", (int) Math.Round(DamageModifier() * 100) + "% damage"));
            tooltips.Add(new TooltipLine(mod, "mana", (int) Math.Round(ManaModifier() * 100) + "% mana cost"));
        }

        public override void NetRecieve(BinaryReader reader)
        {
            modifiers.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i += 1)
                modifiers.Add(GlyphModifier.modifiers[reader.ReadInt32()]);
            initialized = true;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(modifiers.Count);
            for (int i = 0; i < modifiers.Count; i += 1)
                writer.Write(modifiers[i].id);
        }

        public virtual void Randomize()
        {
            initialized = true;
            foreach (GlyphModifier modifier in GlyphModifier.modifiers.Where(modifier => modifier.match(this) && modifier.odds()))
                modifiers.Add(modifier.group == null ? modifier : modifier.group());
        }

        public override TagCompound Save()
        {
            TagCompound compound = new TagCompound {{"ModifierCount", modifiers.Count}};
            for (int i = 0; i < modifiers.Count; i += 1)
                compound.Add("Modifier_" + i, modifiers[i].id);
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
            if (!initialized) Randomize();
        }
    }
}