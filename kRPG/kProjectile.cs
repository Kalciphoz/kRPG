using kRPG.GUI;
using kRPG.Items;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace kRPG
{
    public class kProjectile : GlobalProjectile
    {
        public Dictionary<ELEMENT, int> elementalDamage;
        private Item item;

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public override void AI(Projectile projectile)
        {
            if (elementalDamage == null && Main.netMode != 1)
            {
                elementalDamage = new Dictionary<ELEMENT, int>()
                {
                    {ELEMENT.FIRE, 0},
                    {ELEMENT.COLD, 0},
                    {ELEMENT.LIGHTNING, 0},
                    {ELEMENT.SHADOW, 0}
                };

                if (projectile.modProjectile is Explosion || projectile.modProjectile is NPC_Explosion)
                {
                    elementalDamage[ELEMENT.FIRE] = projectile.damage;
                }
                if (projectile.hostile && !projectile.friendly)
                {
                    bool bossfight = false;
                    foreach (NPC n in Main.npc)
                        if (n.active)
                            if (n.boss) bossfight = true;
                    if (bossfight) return;

                    Player player = projectile.NearestPlayer();
                    Dictionary<ELEMENT, bool> haselement = new Dictionary<ELEMENT, bool>()
                    {
                        { ELEMENT.FIRE, player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert || Main.rand.Next(10) == 0 && Main.netMode == 0 },
                        { ELEMENT.COLD, player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain || Main.rand.Next(10) == 0 && Main.netMode == 0 },
                        { ELEMENT.LIGHTNING, player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly || Main.rand.Next(10) == 0 && Main.netMode == 0 },
                        { ELEMENT.SHADOW, player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula || !Main.dayTime && (Main.rand.Next(10) == 0 && Main.netMode == 0 && player.ZoneOverworldHeight) }
                    };
                    int count = 0;
                    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                        if (haselement[element]) count += 1;
                    int portionsize = (int)Math.Round((double)projectile.damage * kNPC.ELE_DMG_MODIFIER / 3.0 / count);
                    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                        if (haselement[element]) elementalDamage[element] = Math.Max(1, portionsize);
                    return;
                }
                if (projectile.type == mod.ProjectileType<ProceduralSpellProj>())
                {
                    PlayerCharacter character = Main.player[projectile.owner].GetModPlayer<PlayerCharacter>();
                    ProceduralSpellProj spell = (ProceduralSpellProj)projectile.modProjectile;
                    if (spell.source == null)
                        SelectItem(projectile);
                    else
                    {
                        Cross cross = ((Cross)spell.source.glyphs[(int)GLYPHTYPE.CROSS].modItem);
                        if (cross is Cross_Orange)
                            SelectItem(projectile, character.lastSelectedWeapon);
                        else foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                                elementalDamage[element] = (int)Math.Round(cross.eleDmg[element] * projectile.damage);
                    }
                }
                else if (projectile.friendly && !projectile.hostile && Main.player[projectile.owner] != null)
                {
                    Player player = Main.player[projectile.owner];
                    if (player.active)
                        if (player.inventory[player.selectedItem] != null)
                            if (player.inventory[player.selectedItem].active && projectile.type != mod.ProjectileType<Explosion>() && projectile.type != mod.ProjectileType<SmokePellets>())
                                SelectItem(projectile);
                }
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

        public void SelectItem(Projectile projectile, Item item)
        {
            this.item = item;

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                elementalDamage[element] = this.item.GetGlobalItem<kItem>().elementalDamage[element];
        }

        public void SelectItem(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            item = owner.inventory[owner.selectedItem];
            projectile.minion = item.summon || projectile.minion;

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                elementalDamage[element] = item.GetGlobalItem<kItem>().elementalDamage[element];
        }

        public int GetEleDamage(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> ele = new Dictionary<ELEMENT, int>();
            ele = GetIndividualElements(projectile, player, ignoreModifiers);
            return ele[ELEMENT.FIRE] + ele[ELEMENT.COLD] + ele[ELEMENT.LIGHTNING] + ele[ELEMENT.SHADOW];
        }

        public Dictionary<ELEMENT, int> GetIndividualElements(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> dictionary = new Dictionary<ELEMENT, int>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                dictionary[element] = 0;
            if (elementalDamage == null)
                elementalDamage = new Dictionary<ELEMENT, int>()
                {
                    {ELEMENT.FIRE, 0},
                    {ELEMENT.COLD, 0},
                    {ELEMENT.LIGHTNING, 0},
                    {ELEMENT.SHADOW, 0}
                };
            if (player.GetModPlayer<PlayerCharacter>().rituals[RITUAL.DEMON_PACT])
            {
                dictionary[ELEMENT.SHADOW] = GetEleDamage(projectile, player);
            }
            else foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                dictionary[element] = (int)Math.Round(elementalDamage[element] * (ignoreModifiers ? 1 : player.GetModPlayer<PlayerCharacter>().DamageMultiplier(element, projectile.melee, projectile.ranged, projectile.magic, projectile.thrown, projectile.minion)));
            }
            return dictionary;
        }
    }
}
