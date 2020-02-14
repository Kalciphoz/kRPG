using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Enums;
using kRPG.GameObjects.Items.Glyphs;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.Players;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Projectiles.Base
{
    public class kProjectile : GlobalProjectile
    {
        public Dictionary<Element, int> ElementalDamage { get; set; }

        public override bool InstancePerEntity => true;
        private Item SelectedItem { get; set; }

        public override void AI(Projectile projectile)
        {
            if (ElementalDamage != null || Main.netMode == Constants.NetModes.Client)
                return;
            ElementalDamage = new Dictionary<Element, int> { { Element.Fire, 0 }, { Element.Cold, 0 }, { Element.Lightning, 0 }, { Element.Shadow, 0 } };

            if (Main.npc.GetUpperBound(0) >= projectile.owner)
                if (projectile.hostile && !projectile.friendly)
                {
                    bool bossFight = false;
                    foreach (NPC n in Main.npc)
                        if (n.active)
                            if (n.boss)
                                bossFight = true;
                    if (bossFight) return;

                    Player player = Main.netMode == Constants.NetModes.Server ? Main.player[0] : Main.player[Main.myPlayer];
                    Dictionary<Element, bool> hasElement = new Dictionary<Element, bool>
                    {
                        {
                            Element.Fire,
                            player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert ||
                            Main.rand.Next(10) == 0 && Main.netMode == Constants.NetModes.Client
                        },
                        {
                            Element.Cold,
                            player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain ||
                            Main.rand.Next(10) == 0 && Main.netMode == Constants.NetModes.Client
                        },
                        {
                            Element.Lightning,
                            player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly ||
                            Main.rand.Next(10) == 0 && Main.netMode ==Constants.NetModes.Client
                        },
                        {
                            Element.Shadow,
                            player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula ||
                            !Main.dayTime && Main.rand.Next(10) == 0 && Main.netMode ==Constants.NetModes.Client && player.ZoneOverworldHeight
                        }
                    };
                    int count = Enum.GetValues(typeof(Element)).Cast<Element>().Count(element => hasElement[element]);
                    int portionSize = (int)Math.Round(projectile.damage * kNPC.EleDmgModifier / 3.0 / count);
                    foreach (Element element in Enum.GetValues(typeof(Element)))
                        if (hasElement[element])
                            ElementalDamage[element] = Math.Max(1, portionSize);
                    return;
                }

            if (projectile.type == ModContent.ProjectileType<ProceduralSpellProj>())
            {
                PlayerCharacter character = Main.player[projectile.owner].GetModPlayer<PlayerCharacter>();
                ProceduralSpellProj spell = (ProceduralSpellProj)projectile.modProjectile;
                if (spell.Source == null)
                {
                    SelectItem(projectile);
                }
                else
                {
                    Cross cross = (Cross)spell.Source.Glyphs[(int)GlyphType.Cross].modItem;
                    if (cross is Cross_Orange)
                        SelectItem(projectile, character.LastSelectedWeapon);
                    else
                        foreach (Element element in Enum.GetValues(typeof(Element)))
                            ElementalDamage[element] = (int)Math.Round(cross.EleDmg[element] * projectile.damage);
                }
            }
            else if (projectile.friendly && !projectile.hostile && Main.player[projectile.owner] != null)
            {
                Player player = Main.player[projectile.owner];
                if (!player.active)
                    return;
                if (player.inventory[player.selectedItem] == null)
                    return;
                if (player.inventory[player.selectedItem].active && projectile.type != ModContent.ProjectileType<Explosion>() &&
                    projectile.type != ModContent.ProjectileType<SmokePellets>())
                    SelectItem(projectile);
            }

            //if (Main.netMode != 0)
            //{
            //    ModPacket packet = mod.GetPacket();
            //    packet.Pack((byte)Message.InitProjEleDmg);
            //    packet.Pack(projectile.whoAmI);
            //    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            //        packet.Pack(elementalDamage[element]);
            //    packet.Send();
            //}
        }

        /// <summary>
        ///     Returns the total elemental damage, something doesn't seem right with the code though.
        ///     Cause this adds up all there elemental damages into one value.... kinda not sure yet what
        ///     the author was thinking, since each type of damage does different type of damage
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="player"></param>
        /// <param name="ignoreModifiers"></param>
        /// <returns></returns>
        public int GetEleDamage(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            Dictionary<Element, int> ele = GetIndividualElements(projectile, player, ignoreModifiers);
            return ele[Element.Fire] + ele[Element.Cold] + ele[Element.Lightning] + ele[Element.Shadow];
        }

        public Dictionary<Element, int> GetIndividualElements(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            Dictionary<Element, int> dictionary = new Dictionary<Element, int>();
            foreach (Element element in Enum.GetValues(typeof(Element)))
                dictionary[element] = 0;

            if (ElementalDamage == null)
                ElementalDamage = new Dictionary<Element, int> { { Element.Fire, 0 }, { Element.Cold, 0 }, { Element.Lightning, 0 }, { Element.Shadow, 0 } };

            if (player.GetModPlayer<PlayerCharacter>().Rituals[Ritual.DemonPact])

                dictionary[Element.Shadow] = GetEleDamage(projectile, player);

            else

                foreach (Element element in Enum.GetValues(typeof(Element)))

                    dictionary[element] = (int)Math.Round(ElementalDamage[element] * (ignoreModifiers
                                                               ? 1
                                                               : player.GetModPlayer<PlayerCharacter>().DamageMultiplier(element, projectile.melee,
                                                                   projectile.ranged, projectile.magic, projectile.thrown, projectile.minion)));

            return dictionary;
        }

        public void SelectItem(Projectile projectile, Item item)
        {
            //Set the selected item.
            SelectedItem = item;

            //Figure out what the element damage is for the item.
            foreach (Element element in Enum.GetValues(typeof(Element)))

                ElementalDamage[element] = item.GetGlobalItem<kItem>().ElementalDamage[element];
        }

        public void SelectItem(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            SelectedItem = owner.inventory[owner.selectedItem];
            projectile.minion = SelectedItem.summon || projectile.minion;

            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                if (SelectedItem.netID == 0)
                    continue;
                kItem t = SelectedItem.GetGlobalItem<kItem>();
                if (t.ElementalDamage.ContainsKey(element))
                    ElementalDamage[element] = t.ElementalDamage[element];
                else
                    ElementalDamage[element] = 0;
            }
        }
    }
}