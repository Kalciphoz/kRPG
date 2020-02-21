using System;
using System.Collections.Generic;
using System.IO;
using kRPG.Content.Items.Glyphs;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.Spells;
using kRPG.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Packets
{
    public static class CreateProjectilePacket
    {
        public static void Read(BinaryReader reader)
        {
            try
            {
                int playerWhoAmI = reader.ReadInt32();
                int projectileWhoAmI = reader.ReadInt32();
                int starType = reader.ReadInt32();
                int crossType = reader.ReadInt32();
                int moonType = reader.ReadInt32();
                float damage = reader.ReadSingle();
                bool minion = reader.ReadBoolean();
                int casterWhoAmI = reader.ReadInt32();
                int modCount = reader.ReadInt32();



                if (Main.netMode == NetmodeID.MultiplayerClient)
                    if (playerWhoAmI == Main.myPlayer)
                        return;


                int modifierCount = modCount;
                List<GlyphModifier> modifiers = new List<GlyphModifier>();
                for (int i = 0; i < modifierCount; i += 1)
                    modifiers.Add(GlyphModifier.Modifiers[reader.ReadInt32()]);



                Projectile projectile = Main.projectile[projectileWhoAmI];
                if (projectile == null)
                    return;

                projectile.owner = playerWhoAmI;

                if (!(projectile.modProjectile is ProceduralSpellProj))
                    return;
                ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
                ps.Source = new ProceduralSpell(kRPG.Mod) { Glyphs = new Item[3] };
                for (int i = 0; i < ps.Source.Glyphs.Length; i += 1)
                {
                    ps.Source.Glyphs[i] = new Item();
                    ps.Source.Glyphs[i].SetDefaults(0, true);
                }

                ps.Source.Glyphs[(byte)GlyphType.Star].SetDefaults(starType, true);
                ps.Source.Glyphs[(byte)GlyphType.Cross].SetDefaults(crossType, true);
                ps.Source.Glyphs[(byte)GlyphType.Moon].SetDefaults(moonType, true);

                projectile.damage = (int)damage;
                projectile.minion = minion;
                try
                {
                    if (projectile.minion)
                        ps.Caster = Main.projectile[casterWhoAmI];
                    else if (projectile.hostile)
                        ps.Caster = Main.npc[casterWhoAmI];
                    else
                        ps.Caster = Main.player[casterWhoAmI];
                }
                catch (SystemException e)
                {
                    kRPG.LogMessage("Source-assignment failed, aborting..." + e);
                    return;
                }

                ps.Source.ModifierOverride = modifiers;
                foreach (Item item in ps.Source.Glyphs)
                {
                    if (item == null)
                        continue;
                    Glyph glyph = (Glyph)item.modItem;
                    if (glyph.GetAiAction() != null)
                        ps.Ai.Add(glyph.GetAiAction());
                    if (glyph.GetInitAction() != null)
                        ps.Inits.Add(glyph.GetInitAction());
                    if (glyph.GetImpactAction() != null)
                        ps.Impacts.Add(glyph.GetImpactAction());
                    if (glyph.GetKillAction() != null)
                        ps.Kills.Add(glyph.GetKillAction());
                }

                foreach (GlyphModifier modifier in modifiers)
                {
                    if (modifier.Impact != null)
                        ps.Impacts.Add(modifier.Impact);
                    if (modifier.Draw != null)
                        ps.SpellDraw.Add(modifier.Draw);
                    if (modifier.Init != null)
                        ps.Inits.Add(modifier.Init);
                }

                ps.Initialize();

                CreateProjectilePacket.Write(projectile.owner,
                    projectile.whoAmI,
                    ps.Source.Glyphs[(byte)GlyphType.Star].type,
                    ps.Source.Glyphs[(byte)GlyphType.Cross].type,
                    ps.Source.Glyphs[(byte)GlyphType.Moon].type,
                    projectile.damage,
                    projectile.minion,
                    ps.Caster.whoAmI,
                    modifiers
                    );
            }
            catch (SystemException e)
            {
                kRPG.LogMessage("Error handling packet: CreateProjectilePacket on " + (Main.netMode == NetmodeID.Server ? "serverside" : "clientSide") +
                           ", full error trace: " + e);

            }
        }


        public static void Write(int playerWhoAmI, int projectileWhoAmI, int starType, int crossType, int moonType, float damage, bool minion, int casterWhoAmI, List<GlyphModifier> modifiers)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = kRPG.Mod.GetPacket();
                packet.Write((byte) Message.CreateProjectile);
                packet.Write(playerWhoAmI);
                packet.Write(projectileWhoAmI);
                packet.Write(starType);
                packet.Write(crossType);
                packet.Write(moonType);
                packet.Write(damage);
                packet.Write(minion);
                packet.Write(casterWhoAmI);
                List<GlyphModifier> mods = modifiers;
                packet.Write(mods.Count);
                for (int j = 0; j < mods.Count; j += 1)
                    packet.Write(mods[j].Id);
                packet.Send();
            }
        }
    }
}