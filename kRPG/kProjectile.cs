using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using kRPG.Enums;
using kRPG.Items;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace kRPG
{
    public class kProjectile : GlobalProjectile
    {
        public Dictionary<ELEMENT, int> ElementalDamage { get; set; }

        public override bool InstancePerEntity => true;
        private Item SelectedItem { get; set; }

        public override void AI(Projectile projectile)
        {
            if (ElementalDamage != null || Main.netMode == 1)
                return;
            ElementalDamage = new Dictionary<ELEMENT, int> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

            if (Main.npc.GetUpperBound(0) >= projectile.owner)
                if (projectile.hostile && !projectile.friendly)
                {
                    bool bossfight = false;
                    foreach (NPC n in Main.npc)
                        if (n.active)
                            if (n.boss)
                                bossfight = true;
                    if (bossfight) return;

                    Player player = Main.netMode == 2 ? Main.player[0] : Main.player[Main.myPlayer];
                    Dictionary<ELEMENT, bool> haselement = new Dictionary<ELEMENT, bool>
                    {
                        {
                            ELEMENT.FIRE,
                            player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert ||
                            Main.rand.Next(10) == 0 && Main.netMode == 0
                        },
                        {
                            ELEMENT.COLD,
                            player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain ||
                            Main.rand.Next(10) == 0 && Main.netMode == 0
                        },
                        {
                            ELEMENT.LIGHTNING,
                            player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly ||
                            Main.rand.Next(10) == 0 && Main.netMode == 0
                        },
                        {
                            ELEMENT.SHADOW,
                            player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula ||
                            !Main.dayTime && Main.rand.Next(10) == 0 && Main.netMode == 0 && player.ZoneOverworldHeight
                        }
                    };
                    int count = Enum.GetValues(typeof(ELEMENT)).Cast<ELEMENT>().Count(element => haselement[element]);
                    int portionsize = (int) Math.Round(projectile.damage * kNPC.EleDmgModifier / 3.0 / count);
                    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                        if (haselement[element])
                            ElementalDamage[element] = Math.Max(1, portionsize);
                    return;
                }

            if (projectile.type == ModContent.ProjectileType<ProceduralSpellProj>())
            {
                PlayerCharacter character = Main.player[projectile.owner].GetModPlayer<PlayerCharacter>();
                ProceduralSpellProj spell = (ProceduralSpellProj) projectile.modProjectile;
                if (spell.Source == null)
                {
                    SelectItem(projectile);
                }
                else
                {
                    Cross cross = (Cross) spell.Source.Glyphs[(int) GLYPHTYPE.CROSS].modItem;
                    if (cross is Cross_Orange)
                        SelectItem(projectile, character.LastSelectedWeapon);
                    else
                        foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                            ElementalDamage[element] = (int) Math.Round(cross.EleDmg[element] * projectile.damage);
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
            //    packet.Write((byte)Message.InitProjEleDmg);
            //    packet.Write(projectile.whoAmI);
            //    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            //        packet.Write(elementalDamage[element]);
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
            Dictionary<ELEMENT, int> ele = GetIndividualElements(projectile, player, ignoreModifiers);
            return ele[ELEMENT.FIRE] + ele[ELEMENT.COLD] + ele[ELEMENT.LIGHTNING] + ele[ELEMENT.SHADOW];
        }

        public Dictionary<ELEMENT, int> GetIndividualElements(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> dictionary = new Dictionary<ELEMENT, int>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                dictionary[element] = 0;

            if (ElementalDamage == null)
                ElementalDamage = new Dictionary<ELEMENT, int> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

            if (player.GetModPlayer<PlayerCharacter>().Rituals[RITUAL.DEMON_PACT])

                dictionary[ELEMENT.SHADOW] = GetEleDamage(projectile, player);

            else

                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))

                    dictionary[element] = (int) Math.Round(ElementalDamage[element] * (ignoreModifiers
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
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))

                ElementalDamage[element] = item.GetGlobalItem<kItem>().ElementalDamage[element];
        }

        public void SelectItem(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            SelectedItem = owner.inventory[owner.selectedItem];
            projectile.minion = SelectedItem.summon || projectile.minion;

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                //I normally do not like try/catches, but in this case there is no way around it.
                //TMod throws a null reference error occasionally when you try to get globalitem.
                try
                {
                    kItem t = SelectedItem.GetGlobalItem<kItem>();
                    if (t == null)
                        continue;
                    if (t.ElementalDamage.ContainsKey(element))
                        ElementalDamage[element] = t.ElementalDamage[element];
                    else
                        ElementalDamage[element] = 0;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
        }
    }
}