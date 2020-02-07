using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using kRPG.Items;
using kRPG.Items.Weapons;
using kRPG.Dusts;
using kRPG.Buffs;
using System.Collections.Generic;
using System.Runtime.Remoting;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using kRPG.Items.Weapons.RangedDrops;
using kRPG.Modifiers;
using Terraria.Audio;
using Terraria.ModLoader.IO;

namespace kRPG
{
    public class kNPC : GlobalNPC
    {
        public const double ELE_DMG_MODIFIER = 1.2;

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        private bool initialized = false;
        public Dictionary<ProceduralSpell, int> invincibilityTime = new Dictionary<ProceduralSpell, int>();
        public int immuneTime = 0;
        public bool dealseledmg = false;
        
        public NPCModifier modifier;

        public float speedModifier = 1f;

        public Dictionary<ELEMENT, bool> hasAilment = new Dictionary<ELEMENT, bool>()
        {
            {ELEMENT.FIRE, false},
            {ELEMENT.COLD, false},
            {ELEMENT.LIGHTNING, false},
            {ELEMENT.SHADOW, false}
        };
        public Dictionary<ELEMENT, int> ailmentIntensity = new Dictionary<ELEMENT, int>
        {
            {ELEMENT.FIRE, 0},
            {ELEMENT.COLD, 0},
            {ELEMENT.LIGHTNING, 0},
            {ELEMENT.SHADOW, 0}
        };
        public Dictionary<ELEMENT, int> elementalDamage = new Dictionary<ELEMENT, int>()
        {
            {ELEMENT.FIRE, 0},
            {ELEMENT.COLD, 0},
            {ELEMENT.LIGHTNING, 0},
            {ELEMENT.SHADOW, 0}
        };
        
        public static List<Func<kNPC, NPC, NPCModifier>> modifierFuncs = new List<Func<kNPC, NPC, NPCModifier>>()
        {
            DamageModifier.New,
            ElusiveModifier.New,
            ExplosiveModifier.New,
            LifeRegenModifier.New,
            SageModifier.New,
            SizeModifier.New,
            SpeedModifier.New,
            WizardModifier.New
        }; 

        public void InitializeModifiers(NPC npc)
        {
            npc.GivenName = npc.FullName;
            ModPacket packet;
            int random = Main.rand.Next(modifierFuncs.Count);
            NPCModifier modifier = modifierFuncs[random].Invoke(this, npc);
            this.modifier = modifier;
            if (Main.netMode == 2)
            {
                packet = mod.GetPacket();
                packet.Write((byte)Message.PrefixNPC);
                packet.Write(npc.whoAmI);
                packet.Write(random);
                modifier.Write(packet);
                packet.Send();
            }
            MakeNotable(npc);
        }

        public void MakeNotable(NPC npc)
        {
            npc.scale *= 1.1f;
            npc.lifeMax = (int)(npc.lifeMax * 1.2);
            speedModifier *= 1.09f;
        }
        
        public override void ResetEffects(NPC npc)
        {
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                if (!hasAilment[element]) ailmentIntensity[element] = 0;
                hasAilment[element] = false;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (hasAilment[ELEMENT.FIRE])
            {
                if (npc.lifeRegen > 0) npc.lifeRegen = 0;
                npc.lifeRegen -= ailmentIntensity[ELEMENT.FIRE] * 2;
                damage = ailmentIntensity[ELEMENT.FIRE] / 3;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (modifier != null) modifier.DrawEffects(npc, ref drawColor);
            
            if (hasAilment[ELEMENT.FIRE])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Fire, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }
                Lighting.AddLight(npc.position, 0.7f, 0.4f, 0.1f);
            }

            if (hasAilment[ELEMENT.COLD])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.GetDust<Ice>().Type, npc.velocity.X, npc.velocity.Y, 100, Color.White, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
                Lighting.AddLight(npc.position, 0f, 0.4f, 1f);
            }

            if (hasAilment[ELEMENT.LIGHTNING])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Electric, npc.velocity.X, npc.velocity.Y, 100, default(Color), 0.5f);
                    Main.dust[dust].noGravity = true;
                }
                Lighting.AddLight(npc.position, 0.5f, 0.5f, 0.5f);
            }

            if (hasAilment[ELEMENT.SHADOW])
            {
                if (Main.rand.Next(3) < 2)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Shadowflame, npc.velocity.X, npc.velocity.Y, 100, default(Color), 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }

            if (modifier != null) modifier.DrawEffects(npc, ref drawColor);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (modifier != null) modifier.ModifyHitPlayer(npc, target, ref damage, ref crit);
            if (hasAilment[ELEMENT.SHADOW])
                damage = damage * (20 + 9360 / (130 + ailmentIntensity[ELEMENT.SHADOW])) / 100;
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (modifier != null) modifier.OnHitByProjectile(npc, projectile, damage, knockback, crit);
            if (projectile.modProjectile is ProceduralSpellProj)
            {
                ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
                if (invincibilityTime.ContainsKey(ps.source))
                    invincibilityTime[ps.source] = 30;
                else
                    invincibilityTime.Add(ps.source, 30);
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            Player player = npc.NearestPlayer();
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            if (Main.netMode != 0)
            {
                if (crit)
                    damage *= character.critHitChance;
                damage *= character.hitChance;
                return true;
            }
            character.accuracyCounter += character.hitChance;

            float dodgeChanceModifier = 1f;
            
            dodgeChanceModifier *= modifier != null ? modifier.StrikeNPC(npc, damage, defense, knockback, hitDirection, crit) : 1f;

            if (character.accuracyCounter < 1 * dodgeChanceModifier && !character.rituals[RITUAL.WARRIOR_OATH])
            {
                npc.NinjaDodge(npc, 10);
                if (Vector2.Distance(player.Center, npc.Center) < 192)
                {
                    player.immune = true;
                    player.immuneTime = 30;
                }

                damage = (damage - defense) / 8 + defense;
                crit = false;
                SyncCounters(npc.target, character, false);
            }
            else
                character.accuracyCounter -= 1 * dodgeChanceModifier;
            SyncCounters(npc.target, character, false);
            character.critAccuracyCounter += character.critHitChance;
            if (crit)
            {
                if (character.critAccuracyCounter < 1f)
                    crit = false;
                else
                    character.critAccuracyCounter -= 1f;
            }
            SyncCounters(npc.target, character, true);
            return true;
        }

        public void SyncCounters(int player, PlayerCharacter character, bool crit)
        {
            if (Main.netMode != 2) return;

            ModPacket packet = mod.GetPacket();
            packet.Write((byte)(crit ? Message.SyncCritHit : Message.SyncHit));
            packet.Write(player);
            packet.Write(crit ? character.critAccuracyCounter : character.accuracyCounter);
            packet.Send();
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (immuneTime > 0) return false;
            if (projectile.modProjectile is ProceduralSpellProj)
            {
                ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
                if (ps.source != null)
                    if (invincibilityTime.ContainsKey(ps.source))
                        if (invincibilityTime[ps.source] > 0) return false;
            }
            return null;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (immuneTime > 0) return false;
            else return null;
        }

        public override void PostAI(NPC npc)
        {
            if (modifier != null) modifier.PostAI(npc);
            
            List<ProceduralSpell> keys = new List<ProceduralSpell>(invincibilityTime.Keys);
            foreach (ProceduralSpell spell in keys)
                if (invincibilityTime[spell] > 0) invincibilityTime[spell] -= 1;
                else invincibilityTime.Remove(spell);
            if (immuneTime > 0) immuneTime -= 1;
            if (initialized)
            {
                Update(npc);
                return;
            }
            
            if (npc.lifeMax < 10) return;
            
            invincibilityTime = new Dictionary<ProceduralSpell, int>();
            if (Main.netMode != 1)
            {
                Player player = npc.NearestPlayer();
                int playerlevel = player.GetModPlayer<PlayerCharacter>().level;
                if (npc.defense < 2000)
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
                    npc.life = (int)Math.Round(npc.life * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
                    npc.defense = (int)Math.Round(npc.defense * (GetLevel(npc.netID) / 160f + 1f));
                    npc.lavaImmune = npc.lavaImmune || npc.defense > 60;

                    if (Main.netMode == 2)
                    {
                        ModPacket packet = mod.GetPacket();
                        packet.Write((byte)Message.ScaleNPC);
                        packet.Write(npc.whoAmI);
                        packet.Write(npc.lifeMax);
                        packet.Write(npc.life);
                        packet.Write(npc.defense);
                        packet.Send();
                    }
                }

                if (npc.damage > 0 && !npc.boss)
                {
                    Dictionary<ELEMENT, bool> haselement = new Dictionary<ELEMENT, bool>()
                    {
                        { ELEMENT.FIRE, player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert },
                        { ELEMENT.COLD, player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain },
                        { ELEMENT.LIGHTNING, player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly },
                        { ELEMENT.SHADOW, player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula }
                    };
                    int count = 0;
                    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                        if (haselement[element]) count += 1;
                    int portionsize = (int)Math.Round((double)npc.damage * ELE_DMG_MODIFIER / 2.0 / count);
                    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                        if (haselement[element]) elementalDamage[element] = Math.Max(1, portionsize);
                    dealseledmg = count > 0;
                    
                    if (Main.netMode == 2)
                    {
                        ModPacket packet = mod.GetPacket();
                        packet.Write((byte)Message.NPCEleDmg);
                        packet.Write(npc.whoAmI);
                        packet.Write(haselement[ELEMENT.FIRE]);
                        packet.Write(haselement[ELEMENT.COLD]);
                        packet.Write(haselement[ELEMENT.LIGHTNING]);
                        packet.Write(haselement[ELEMENT.SHADOW]);
                        packet.Send();
                    }
                }

                if (Main.rand.Next(8) < 3 && !npc.boss && !npc.townNPC && !npc.friendly && npc.aiStyle != 6)
                    InitializeModifiers(npc);

                if (!Main.expertMode)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.3);
                    npc.life = (int)(npc.life * 1.3);
                }
            }
            
            initialized = true;
        }

        public void Update(NPC npc)
        {
            if (npc.aiStyle == 3 && npc.velocity.Y == 0f)
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.direction * Math.Max(Math.Abs(npc.velocity.X), 8f), 1f * speedModifier / 20f);

            if (modifier != null) modifier.Update(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            if (modifier != null) modifier.NPCLoot(npc);
            
            if (npc.lifeMax < 10) return;
            if (npc.friendly) return;
            if (npc.townNPC) return;

            if (Main.rand.Next(2250) < GetLevel(npc.type))
            {
                if (Main.rand.Next(9) == 0)
                    Item.NewItem(npc.position, mod.ItemType<BlacksmithCrown>());
                else
                    Item.NewItem(npc.position, mod.ItemType<PermanenceCrown>());
            }

            int level = GetLevel(npc.netID);
            
            Player player = npc.NearestPlayer();
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            int life = npc.type == NPCID.SolarCrawltipedeTail || npc.type == NPCID.SolarCrawltipedeBody || npc.type == NPCID.SolarCrawltipedeHead ? npc.lifeMax / 8 : npc.lifeMax;
            int defFactor = (npc.defense < 0) ? 1 : npc.defense * life / (character.level + 10);
            int baseExp = Main.rand.Next((life + defFactor) / 5) + (life + defFactor) / 6;
            int scaled = Main.expertMode ? (int)(baseExp * 0.5) : baseExp;
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.AddXP);
                packet.Write(scaled);
                packet.Write(npc.target);
                packet.Send();
            }
            else
                character.AddXP(scaled);

            if (level < Math.Min(character.level - 17, 70)) return;
            
            float dps = Math.Min((float)(Math.Pow(1.045, Math.Min(130, character.level)) * 8.5f), (float)(Math.Pow(1.04, level)*15) + 14);
            int assumedDef = !Main.hardMode ? 5 : character.level/3;

            if (npc.FullName.Contains("Green Slime"))
            {
                if (Main.rand.Next(22) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.slimeGreen, Main.rand.Next(3) < 1 ? SwordAccent.RandomAccent() : SwordAccent.none, dps, assumedDef);
            }

            else if (npc.FullName.Contains("Blue Slime"))
            {
                if (Main.rand.Next(30) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.slimeBlue, Main.rand.Next(2) < 1 ? SwordAccent.RandomAccent() : SwordAccent.none, dps, assumedDef);
            }

            else if (Main.rand.Next((character.level < 5 ? 8 : (character.level < 10 ? 15 : 25))) == 0)
            {
                if (Main.rand.Next(5) == 0) Item.NewItem(npc.position, RangedWeapon.NewRangedWeapon(mod, npc.position, level, character.level, dps, assumedDef), Main.rand.Next(30, 90));
                else if (Main.rand.Next(9) < 5) ProceduralSword.GenerateSword(mod, npc.position, GetTheme(player), dps, assumedDef);
                else ProceduralStaff.GenerateStaff(mod, npc.position, GetStaffTheme(player), dps * 1.3f, assumedDef);
            }

            else if (Main.rand.Next(40) == 0)
            {
                Item item = Main.item[Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), mod.ItemType(Glyph.GetRandom()))];
                if (item.modItem is Glyph && Main.netMode == 0)
                {
                    ((Glyph)item.modItem).Randomize();
                }
            }

            else if (npc.FullName.EndsWith(" Eye") && level < 20)
            {
                if (Main.rand.Next(25) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.MONSTROUS), SwordBlade.demonEye, Main.rand.Next(5) < 2 ? SwordAccent.RandomAccent() : SwordAccent.none, dps, assumedDef);
                else if (Main.rand.Next(15) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.demonEye, SwordAccent.none, dps, assumedDef);
            }
        }

        public SWORDTHEME GetTheme(Player player)
        {
            if (player.ZoneCorrupt || player.ZoneCrimson)
                return SWORDTHEME.MONSTROUS;

            if (player.ZoneDungeon)
                return SWORDTHEME.RUNIC;

            if (player.ZoneUnderworldHeight)
                return SWORDTHEME.HELLISH;

            if (!Main.dayTime && Main.rand.Next(4) == 0 && !Main.hardMode)
                return SWORDTHEME.MONSTROUS;

            return Main.hardMode ? SWORDTHEME.HARDMODE : SWORDTHEME.GENERIC;
        }

        public STAFFTHEME GetStaffTheme(Player player)
        {
            if (player.ZoneDungeon)
                return STAFFTHEME.DUNGEON;

            if (player.ZoneUnderworldHeight)
                return STAFFTHEME.UNDERWORLD;

            return STAFFTHEME.WOODEN;
        }

        public static int GetLevel(int type)
        {
            NPC npc = new NPC();
            npc.SetDefaults(type);
            npc.active = false;
            return Math.Min(Main.expertMode ? (int)((npc.damage + npc.defense * 4)/3) : (int)((npc.damage * 2 + npc.defense * 4)/3), npc.boss ? 120 : 110);
        }

        public void RollDrops(NPC npc, int[] odds, int[][] droptables)
        {
            int sum = 0;
            for (int i = 0; i < odds.Length; i += 1)
            {
                if (Main.rand.Next(1000 - sum) < odds[i])
                    Item.NewItem(npc.position, droptables[i].Random());

                sum += odds[i];
            }
        }

        public void RollDrops(NPC npc, int[] odds, int[][] droptables, int[] odds2, Action<NPC>[] drops2)
        {
            int sum = 0;
            for (int i = 0; i < odds.Length; i += 1)
            {
                if (Main.rand.Next(1000 - sum) < odds[i])
                    Item.NewItem(npc.position, droptables[i].Random());

                sum += odds[i];
            }
            for (int i = 0; i < odds2.Length; i += 1)
            {
                if (Main.rand.Next(1000 - sum) < odds2[i])
                    drops2[i](npc);

                sum += odds[i];
            }
        }

        public int GetEleDamage(Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> ele = new Dictionary<ELEMENT, int>();
            return elementalDamage[ELEMENT.FIRE] + elementalDamage[ELEMENT.COLD] + elementalDamage[ELEMENT.LIGHTNING] + elementalDamage[ELEMENT.SHADOW];
        }
    }
}
