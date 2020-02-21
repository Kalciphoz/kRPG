﻿using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Content.Items.Crowns;
using kRPG.Content.Items.Dusts;
using kRPG.Content.Items.Glyphs;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.Items.Weapons.Melee;
using kRPG.Content.Items.Weapons.Ranged;
using kRPG.Content.Modifiers;
using kRPG.Content.Players;
using kRPG.Content.Spells;
using kRPG.Enums;
using kRPG.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.NPCs
{
    public class kNPC : GlobalNPC
    {



        public const double EleDmgModifier = 1.2;

        public Dictionary<Element, int> AilmentIntensity { get; set; } = new Dictionary<Element, int>
        {
            {Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}
        };

        public bool DealsEleDmg { get; set; }

        public Dictionary<Element, int> ElementalDamage { get; set; } = new Dictionary<Element, int>
        {
            {Element.Fire, 0}, {Element.Cold, 0}, {Element.Lightning, 0}, {Element.Shadow, 0}
        };

        public Dictionary<Element, bool> HasAilment { get; set; } = new Dictionary<Element, bool>
        {
            {Element.Fire, false}, {Element.Cold, false}, {Element.Lightning, false}, {Element.Shadow, false}
        };

        public int ImmuneTime { get; set; }
        private bool Initialized { get; set; }

        public override bool InstancePerEntity => true;
        public Dictionary<ProceduralSpell, int> InvincibilityTime { get; set; } = new Dictionary<ProceduralSpell, int>();

        public Dictionary<int, NpcModifier> Modifiers { get; set; } = new Dictionary<int, NpcModifier>();



        public float SpeedModifier { get; set; } = 1f;

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (ImmuneTime > 0)
            {
                Main.NewText($"{npc.FullName} is immune for {ImmuneTime} seconds!");
                return false;
            }

            return null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (ImmuneTime > 0)
            {
                Main.NewText($"{npc.FullName} is immune for {ImmuneTime} seconds!");
                return false;
            }

            ProceduralSpellProj ps = projectile.modProjectile as ProceduralSpellProj;

            if (ps?.Source == null)
                return null;

            if (!npc.GetGlobalNPC<kNPC>().InvincibilityTime.ContainsKey(ps.Source))
                return null;

            if (npc.GetGlobalNPC<kNPC>().InvincibilityTime[ps.Source] > 0)
                return false;
            return null;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            foreach (NpcModifier modifier in Modifiers.Values)
                modifier.DrawEffects(npc, ref drawColor);

            if (HasAilment[Element.Fire])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Fire, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }

                Lighting.AddLight(npc.position, 0.7f, 0.4f, 0.1f);
            }

            if (HasAilment[Element.Cold])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.GetInstance<Ice>().Type, npc.velocity.X, npc.velocity.Y, 100, Color.White, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(npc.position, 0f, 0.4f, 1f);
            }

            if (HasAilment[Element.Lightning])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Electric, npc.velocity.X, npc.velocity.Y, 100, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(npc.position, 0.5f, 0.5f, 0.5f);
            }

            if (HasAilment[Element.Shadow])
            {
                if (Main.rand.Next(3) >= 2)
                    return;
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Shadowflame, npc.velocity.X, npc.velocity.Y, 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public int GetEleDamage(Player player, bool ignoreModifiers = false)
        {
            Dictionary<Element, int> ele = new Dictionary<Element, int>();
            return ElementalDamage[Element.Fire] + ElementalDamage[Element.Cold] + ElementalDamage[Element.Lightning] + ElementalDamage[Element.Shadow];
        }
        
        public StaffTheme GetStaffTheme(Player player)
        {
            if (player.ZoneDungeon)
                return StaffTheme.Dungeon;

            if (player.ZoneUnderworldHeight)
                return StaffTheme.Underworld;

            return StaffTheme.Wooden;
        }

        public SwordTheme GetTheme(Player player)
        {
            if (player.ZoneCorrupt || player.ZoneCrimson)
                return SwordTheme.Monstrous;

            if (player.ZoneDungeon)
                return SwordTheme.Runic;

            if (player.ZoneUnderworldHeight)
                return SwordTheme.Hellish;

            if (!Main.dayTime && Main.rand.Next(4) == 0 && !Main.hardMode)
                return SwordTheme.Monstrous;

            return Main.hardMode ? SwordTheme.Hardmode : SwordTheme.Generic;
        }

        /// <summary>
        /// Adds Modifiers to mobs.
        /// </summary>
        /// <param name="npc"></param>
        public void InitializeModifiers(NPC npc)
        {
            kNPC kn = npc.GetGlobalNPC<kNPC>();

            List<int> buffs = new List<int>();

            int rnd = new Random().Next(0, 100);
            if (rnd < 40)
            {
                npc.GivenName = npc.FullName;
            }
            else if (rnd < 100)
                buffs.Add(Main.rand.Next(ModiferFunctions.Instance.Modifiers.Count));

            //buffs.Add(ModiferFunctions.Instance.SpeedModifier.Id);

            //This shouldn't be happening
            if (kn.Modifiers.Values.Count > 0)
                return;

            foreach (int t in buffs)
            {


                NpcModifier modifier = ModiferFunctions.Instance.Modifiers[t].Function.Invoke(this, npc);
                modifier.Initialize();
                modifier.Apply();
                kn.Modifiers.Add(t, modifier);
            }


            //MakeNotable(npc);


        }

        public static int GetLevel(NPC npc)
        {
            return Math.Min(Main.expertMode ? (npc.damage + npc.defense * 4) / 3 : (npc.damage * 2 + npc.defense * 4) / 3, npc.boss ? 120 : 110);
        }

        //This just makes the mobs tougher, not sure why you would always want to do this.
        //public void MakeNotable(NPC npc)
        //{
        //    npc.scale *= 1.1f;
        //    npc.lifeMax = (int)(npc.lifeMax * 1.2);
        //    SpeedModifier *= 1.09f;
        //}

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            foreach (NpcModifier t in Modifiers.Values)
                t.ModifyHitPlayer(npc, target, ref damage, ref crit);

            if (HasAilment[Element.Shadow])
                damage = damage * (20 + 9360 / (130 + AilmentIntensity[Element.Shadow])) / 100;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="npc"></param>
        public override void NPCLoot(NPC npc)
        {
            kNPC kNpc = npc.GetGlobalNPC<kNPC>();

            foreach (NpcModifier npcModifier in kNpc.Modifiers.Values)
                npcModifier.NpcLoot(npc);

            if (npc.lifeMax < 10) return;
            if (npc.friendly) return;
            if (npc.townNPC) return;

            if (Main.rand.Next(2500) < GetLevel(npc))  //GetLevel(npc.type))
                Item.NewItem(npc.position, Main.rand.Next(8) == 0 ? ModContent.ItemType<BlacksmithCrown>() : ModContent.ItemType<PermanenceCrown>());

            int level = GetLevel(npc);
            //GetLevel(npc.netID);

            Player player = Array.Find(Main.player, p => p.active);
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                player = Main.LocalPlayer;
            }
            else if (Main.player[npc.target].active)
            {
                player = Main.player[npc.target];
            }
            else
            {
                PlayerCharacter c = player.GetModPlayer<PlayerCharacter>();
                foreach (Player p in Main.player)
                    if (p != null)
                        if (p.active)
                            if (p.GetModPlayer<PlayerCharacter>() != null)
                                if (p.GetModPlayer<PlayerCharacter>().Level > c.Level)
                                    player = p;
            }

            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            int life = npc.type == NPCID.SolarCrawltipedeTail || npc.type == NPCID.SolarCrawltipedeBody || npc.type == NPCID.SolarCrawltipedeHead
                ? npc.lifeMax / 8
                : npc.lifeMax;
            int defFactor = npc.defense < 0 ? 1 : npc.defense * life / (character.Level + 10);
            int baseExp = Main.rand.Next((life + defFactor) / 5) + (life + defFactor) / 6;
            int scaled = Main.expertMode ? (int)(baseExp * 0.5) : baseExp;


            if (!AddXPPacket.Write(scaled, npc.target))
                character.AddXp(scaled);

            if (level < Math.Min(character.Level - 17, 70)) return;

            float dps = Math.Min((float)(Math.Pow(1.04, Math.Min(130, character.Level)) * 9f), (float)(Math.Pow(1.023, level) * 15) + 14);
            int assumedDef = !Main.hardMode ? 5 : character.Level / 3;

            if (npc.FullName.Contains("Green Slime"))
            {
                if (Main.rand.Next(22) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SwordTheme.Generic), SwordBlade.SlimeGreen,
                        Main.rand.Next(3) < 1 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, assumedDef);
            }

            else if (npc.FullName.Contains("Blue Slime"))
            {
                if (Main.rand.Next(30) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SwordTheme.Generic), SwordBlade.SlimeBlue,
                        Main.rand.Next(2) < 1 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, assumedDef);
            }

            else if (Main.rand.Next(character.Level < 5 ? 5 : character.Level < 10 ? 8 : 20) == 0)
            {
                if (Main.rand.Next(5) == 0)
                    Item.NewItem(npc.position, RangedWeapon.NewRangedWeapon(mod, npc.position, level, character.Level, dps, assumedDef),
                        Main.rand.Next(30, 90));
                else if (Main.rand.Next(9) < 5) ProceduralSword.GenerateSword(mod, npc.position, GetTheme(player), dps, assumedDef);
                else ProceduralStaff.GenerateStaff(mod, npc.position, GetStaffTheme(player), dps * 1.2f, assumedDef);
            }

            else if (Main.rand.Next(40) == 0)
            {
                Item item = Main.item[Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), mod.ItemType(Glyph.GetRandom()))];
                if (item.modItem is Glyph)// && Main.netMode ==NetmodeID.SinglePlayer)
                    ((Glyph)item.modItem).Randomize();
            }

            else if (npc.FullName.EndsWith(" Eye") && level < 20)
            {
                if (Main.rand.Next(20) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SwordTheme.Monstrous), SwordBlade.DemonEye, Main.rand.Next(5) < 2 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, assumedDef);
                else if (Main.rand.Next(15) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SwordTheme.Generic), SwordBlade.DemonEye, SwordAccent.None, dps, assumedDef);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            foreach (NpcModifier npcModifier in Modifiers.Values)
                npcModifier.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            ProcessSpellHit(projectile);
        }

        private void ProcessSpellHit(Projectile projectile)
        {
            if (!(projectile.modProjectile is ProceduralSpellProj))
                return;

            ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;

            if (InvincibilityTime.ContainsKey(ps.Source))
                InvincibilityTime[ps.Source] = 30;
            else
                InvincibilityTime.Add(ps.Source, 30);
        }

        private int GetPlayerLevel()
        {
            try
            {
                Player player = Main.netMode == NetmodeID.Server ? Main.player[0] : Main.player[Main.myPlayer];
                return player.GetModPlayer<PlayerCharacter>().Level;
            }
            catch (Exception )
            {
                return 20;
            }

        }


        /// <summary>
        /// This function is called after a new npc is created but before it is spawned in the world
        /// </summary>
        /// <param name="npc"></param>
        public override void SetDefaults(NPC npc)
        {
            //If we are in single player mode, and the player hasn't entered the world, exit out.
            if (Main.netMode == NetmodeID.SinglePlayer && !kRPG.PlayerEnteredWorld)
                return;

            //If the mob is a boss, townnpc, friendly and the client is mulitplayer client... exit out
            if (npc.boss || npc.townNPC || npc.friendly || Main.netMode == NetmodeID.MultiplayerClient)
                return;


            int playerLevel = Main.netMode == NetmodeID.SinglePlayer ? GetPlayerLevel() : 20;

            int npcLevel = GetLevel(npc);

            //Calculate new npc max life
            npc.lifeMax = (int)Math.Round(npc.lifeMax * (npcLevel / 30f + 0.4f + playerLevel * 0.025f));
            npc.life = npc.lifeMax;

            //Calculate npc defense
            npc.defense = (int)Math.Round(npc.defense * (npcLevel / 160f + 1f));

            //NPC is immune to lava if they are flagged as immune or their defense is greater that 60.
            npc.lavaImmune = npc.lavaImmune || npc.defense > 60;




            InitializeElementalDamage(npc);

            //So we are randomly adding modifiers...
            InitializeModifiers(npc);
            



            //Try and resolve mod's name
            npc.GivenName = NPC.getNewNPCName(npc.type);
            
            //Adjust life in expert mode.
            if (Main.expertMode)
            {
                //npc.lifeMax = (int)(npc.lifeMax * 1.3);
                npc.life = (int)(npc.life * 1.3);
            }
        }


        private void InitializeElementalDamage(NPC npc)
        {
            if ((npc.damage <= 0 || npc.boss || Main.rand.Next(3) == 0) || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            kNPC knpc = npc.GetGlobalNPC<kNPC>();
            Player player = Main.netMode == NetmodeID.Server ? Main.player[0] : Main.player[Main.myPlayer];
            Dictionary<Element, bool> hasElement = new Dictionary<Element, bool>
            {
                {
                    Element.Fire,
                    player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert || Main.rand.Next(10) == 0
                },
                {
                    Element.Cold,
                    player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain ||  Main.rand.Next(10) == 0
                },
                {
                    Element.Lightning,
                    player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly || Main.rand.Next(10) == 0
                },
                {
                    Element.Shadow,
                    player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula || !Main.dayTime && Main.rand.Next(10) == 0
                }
            };

            int count = Enum.GetValues(typeof(Element)).Cast<Element>().Count(element => hasElement[element]);

            int portionSize = (int)Math.Round(npc.damage * EleDmgModifier / 2.0 / count);

            foreach (Element element in Enum.GetValues(typeof(Element)))
                if (hasElement[element])
                    knpc.ElementalDamage[element] = Math.Max(1, portionSize);

            knpc.DealsEleDmg = count > 0;

        }

        public override void PostAI(NPC npc)
        {
            kNPC kNpc = npc.GetGlobalNPC<kNPC>();

            foreach (NpcModifier npcModifier in kNpc.Modifiers.Values)
                npcModifier.PostAi(npc);

            List<ProceduralSpell> keys = new List<ProceduralSpell>(InvincibilityTime.Keys);
            foreach (ProceduralSpell spell in keys)
            {
                if (kNpc.InvincibilityTime[spell] > 0)
                {
                    kNpc.InvincibilityTime[spell] -= 1;
                }
                else
                {
                    kNpc.InvincibilityTime.Remove(spell);
                }
            }

            if (ImmuneTime > 0)
                ImmuneTime -= 1;

            if (Initialized || Main.netMode == NetmodeID.MultiplayerClient)
            {
                Update(npc);
                return;
            }


            kNpc.InvincibilityTime = new Dictionary<ProceduralSpell, int>();

            //We only run this code if in single player mode, cause we aren't replicating this information.
#if SINGLEPLAYER

#endif
            foreach (NpcModifier npcModifier in kNpc.Modifiers.Values)
            {
                npcModifier.Apply();
            }

            npc.netUpdate = true;


            PrefixNPCPacket.Write(npc, Modifiers);
            Initialized = true;
        }



        public override void ResetEffects(NPC npc)
        {
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                if (!HasAilment[element]) AilmentIntensity[element] = 0;
                HasAilment[element] = false;
            }
        }

        //public void RollDrops(NPC npc, int[] odds, int[][] dropTables)
        //{
        //    int sum = 0;
        //    for (int i = 0; i < odds.Length; i += 1)
        //    {
        //        if (Main.rand.Next(1000 - sum) < odds[i])
        //            Item.NewItem(npc.position, dropTables[i].Random());

        //        sum += odds[i];
        //    }
        //}

        //public void RollDrops(NPC npc, int[] odds, int[][] dropTables, int[] odds2, Action<NPC>[] drops2)
        //{
        //    int sum = 0;
        //    for (int i = 0; i < odds.Length; i += 1)
        //    {
        //        if (Main.rand.Next(1000 - sum) < odds[i])
        //            Item.NewItem(npc.position, dropTables[i].Random());

        //        sum += odds[i];
        //    }

        //    for (int i = 0; i < odds2.Length; i += 1)
        //    {
        //        if (Main.rand.Next(1000 - sum) < odds2[i])
        //            drops2[i](npc);

        //        sum += odds[i];
        //    }
        //}

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockBack, int hitDirection, ref bool crit)
        {
            Player player = Main.player[npc.target];
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            character.AccuracyCounter += character.HitChance;



            float dodgeChanceModifier = 1f;

            foreach (NpcModifier npcModifier in Modifiers.Values)
                dodgeChanceModifier *= npcModifier.StrikeNpc(npc, damage, defense, knockBack, hitDirection, crit);
#if DODGE

            if (character.AccuracyCounter < .5 * dodgeChanceModifier && !character.Rituals[Ritual.WarriorOath])
            {
                npc.NinjaDodge(npc, 10);
                //if (Vector2.Distance(player.Center, npc.Center) < 192)
                //{
                //    player.immune = true;
                //    player.immuneTime = 30;
                //}

                damage = 0;
                crit = false;
                if (character.player.inventory[character.player.selectedItem] != character.LastSelectedWeapon)
                    knockBack = 0f;
                SyncCounters(npc.target, character, false);
                return false;
            }
#endif

            character.AccuracyCounter -= 1 * dodgeChanceModifier;
            SyncCounters(npc.target, character, false);
            character.CritAccuracyCounter += character.CritHitChance;
            if (crit)
            {
                if (character.CritAccuracyCounter < 1f)
                    crit = false;
                else
                    character.CritAccuracyCounter -= 1f;
            }

            SyncCounters(npc.target, character, true);
            return true;
        }

        public void SyncCounters(int player, PlayerCharacter character, bool crit)
        {
            if (crit)
                SyncCritHitPacket.Write(player, character.CritAccuracyCounter);
            else
                SyncHitPacket.Write(player, character.AccuracyCounter);
        }

        public void Update(NPC npc)
        {

            if (npc.aiStyle ==NpcAiStyles.SimpleFighter && Math.Abs(npc.velocity.Y) < .01)
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.direction * Math.Max(Math.Abs(npc.velocity.X), 8f), 1f * SpeedModifier / 20f);

            foreach (NpcModifier t in Modifiers.Values)
                t.Update(npc);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Modifiers.ContainsKey(ModiferFunctions.Instance.LifeRegenModifier.Id))
            {
                npc.lifeRegen = 20;
            }

            if (HasAilment[Element.Fire])
            {

                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= AilmentIntensity[Element.Fire] * 2;

                damage = AilmentIntensity[Element.Fire] / 3;
            }

            npc.netUpdate = true;
            npc.netUpdate2 = true;

        }



    }
}