using System;
using System.Collections.Generic;
using System.Linq;
using kRPG2.Enums;
using kRPG2.Items;
using kRPG2.Items.Dusts;
using kRPG2.Items.Glyphs;
using kRPG2.Items.Weapons;
using kRPG2.Items.Weapons.RangedDrops;
using kRPG2.Modifiers;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG2
{
    public class kNPC : GlobalNPC
    {
        public const double EleDmgModifier = 1.2;

        public Dictionary<ELEMENT, int> AilmentIntensity { get; set; } = new Dictionary<ELEMENT, int>
        {
            {ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}
        };

        public bool DealsEleDmg { get; set; }

        public Dictionary<ELEMENT, int> ElementalDamage { get; set; }= new Dictionary<ELEMENT, int>
        {
            {ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}
        };

        public Dictionary<ELEMENT, bool> HasAilment { get; set; } = new Dictionary<ELEMENT, bool>
        {
            {ELEMENT.FIRE, false}, {ELEMENT.COLD, false}, {ELEMENT.LIGHTNING, false}, {ELEMENT.SHADOW, false}
        };

        public int ImmuneTime { get; set; }
        private bool Initialized { get; set; }
        public Dictionary<ProceduralSpell, int> InvincibilityTime { get; set; } = new Dictionary<ProceduralSpell, int>();

        public List<Func<kNPC, NPC, NpcModifier>> ModifierFuncs { get; set; } = new List<Func<kNPC, NPC, NpcModifier>>
        {
            DamageModifier.New,
            ElusiveModifier.New,
            ExplosiveModifier.New,
            LifeRegenModifier.New,
            SageModifier.New,
            SizeModifier.New,
            global::kRPG2.Modifiers.SpeedModifier.New
        };

        public List<NpcModifier> Modifiers { get; set; } = new List<NpcModifier>();

        public float SpeedModifier { get; set; } = 1f;

        public override bool InstancePerEntity => true;

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (ImmuneTime > 0) return false;
            return null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (ImmuneTime > 0) return false;
            ProceduralSpellProj ps = projectile.modProjectile as ProceduralSpellProj;
            if (ps?.Source == null)
                return null;
            if (!InvincibilityTime.ContainsKey(ps.Source))
                return null;
            if (InvincibilityTime[ps.Source] > 0) return false;
            return null;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            foreach (NpcModifier t in Modifiers)
                t.DrawEffects(npc, ref drawColor);

            if (HasAilment[ELEMENT.FIRE])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Fire, npc.velocity.X * 0.4f,
                        npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }

                Lighting.AddLight(npc.position, 0.7f, 0.4f, 0.1f);
            }

            if (HasAilment[ELEMENT.COLD])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.GetInstance<Ice>().Type,
                        npc.velocity.X, npc.velocity.Y, 100, Color.White, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(npc.position, 0f, 0.4f, 1f);
            }

            if (HasAilment[ELEMENT.LIGHTNING])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Electric, npc.velocity.X, npc.velocity.Y,
                        100, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(npc.position, 0.5f, 0.5f, 0.5f);
            }

            if (HasAilment[ELEMENT.SHADOW])
            {
                if (Main.rand.Next(3) >= 2)
                    return;
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Shadowflame, npc.velocity.X, npc.velocity.Y,
                    100, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public int GetEleDamage(Player player, bool ignoreModifiers = false)
        {
            Dictionary<ELEMENT, int> ele = new Dictionary<ELEMENT, int>();
            return ElementalDamage[ELEMENT.FIRE] + ElementalDamage[ELEMENT.COLD] + ElementalDamage[ELEMENT.LIGHTNING] + ElementalDamage[ELEMENT.SHADOW];
        }

        public static int GetLevel(int type)
        {
            NPC npc = new NPC();
            npc.SetDefaults(type);
            npc.active = false;
            return Math.Min(Main.expertMode ? (npc.damage + npc.defense * 4) / 3 : (npc.damage * 2 + npc.defense * 4) / 3, npc.boss ? 120 : 110);
        }

        public STAFFTHEME GetStaffTheme(Player player)
        {
            if (player.ZoneDungeon)
                return STAFFTHEME.DUNGEON;

            if (player.ZoneUnderworldHeight)
                return STAFFTHEME.UNDERWORLD;

            return STAFFTHEME.WOODEN;
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

        public void InitializeModifiers(NPC npc)
        {
            npc.GivenName = npc.FullName;
            int amount = 1;
            for (int i = 0; i < amount; i++)
            {
                int random = Main.rand.Next(ModifierFuncs.Count);
                NpcModifier modifier = ModifierFuncs[random].Invoke(this, npc);
                Modifiers.Add(modifier);
            }

            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte) Message.PrefixNPC);
                packet.Write(npc.whoAmI);
                packet.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    packet.Write(Modifiers.FindIndex(modifier => modifier == Modifiers[i]));
                    Modifiers[i].Write(packet);
                }

                packet.Send();
            }

            MakeNotable(npc);
        }

        public void MakeNotable(NPC npc)
        {
            npc.scale *= 1.1f;
            npc.lifeMax = (int) (npc.lifeMax * 1.2);
            SpeedModifier *= 1.09f;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            foreach (NpcModifier t in Modifiers)
                t.ModifyHitPlayer(npc, target, ref damage, ref crit);

            if (HasAilment[ELEMENT.SHADOW])
                damage = damage * (20 + 9360 / (130 + AilmentIntensity[ELEMENT.SHADOW])) / 100;
        }

        public override void NPCLoot(NPC npc)
        {
            foreach (NpcModifier t in Modifiers)
                t.NPCLoot(npc);

            if (npc.lifeMax < 10) return;
            if (npc.friendly) return;
            if (npc.townNPC) return;

            if (Main.rand.Next(2500) < GetLevel(npc.type))
                Item.NewItem(npc.position, Main.rand.Next(8) == 0 ? ModContent.ItemType<BlacksmithCrown>() : ModContent.ItemType<PermanenceCrown>());

            int level = GetLevel(npc.netID);

            Player player = Array.Find(Main.player, p => p.active);
            if (Main.netMode == 0)
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
            int scaled = Main.expertMode ? (int) (baseExp * 0.5) : baseExp;
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte) Message.AddXP);
                packet.Write(scaled);
                packet.Write(npc.target);
                packet.Send();
            }
            else
            {
                character.AddXp(scaled);
            }

            if (level < Math.Min(character.Level - 17, 70)) return;

            float dps = Math.Min((float) (Math.Pow(1.04, Math.Min(130, character.Level)) * 9f), (float) (Math.Pow(1.023, level) * 15) + 14);
            int assumedDef = !Main.hardMode ? 5 : character.Level / 3;

            if (npc.FullName.Contains("Green Slime"))
            {
                if (Main.rand.Next(22) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.SlimeGreen,
                        Main.rand.Next(3) < 1 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, assumedDef);
            }

            else if (npc.FullName.Contains("Blue Slime"))
            {
                if (Main.rand.Next(30) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.SlimeBlue,
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
                Item item = Main.item[
                    Item.NewItem(new Rectangle((int) npc.position.X, (int) npc.position.Y, npc.width, npc.height), mod.ItemType(Glyph.GetRandom()))];
                if (item.modItem is Glyph && Main.netMode == 0)
                    ((Glyph) item.modItem).Randomize();
            }

            else if (npc.FullName.EndsWith(" Eye") && level < 20)
            {
                if (Main.rand.Next(20) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.MONSTROUS), SwordBlade.DemonEye,
                        Main.rand.Next(5) < 2 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, assumedDef);
                else if (Main.rand.Next(15) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.DemonEye, SwordAccent.None, dps,
                        assumedDef);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            foreach (NpcModifier t in Modifiers)
                t.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (!(projectile.modProjectile is ProceduralSpellProj))
                return;
            ProceduralSpellProj ps = (ProceduralSpellProj) projectile.modProjectile;
            if (InvincibilityTime.ContainsKey(ps.Source))
                InvincibilityTime[ps.Source] = 30;
            else
                InvincibilityTime.Add(ps.Source, 30);
        }

        public override void PostAI(NPC npc)
        {
            foreach (NpcModifier t in Modifiers)
                t.PostAI(npc);

            List<ProceduralSpell> keys = new List<ProceduralSpell>(InvincibilityTime.Keys);
            foreach (ProceduralSpell spell in keys)
                if (InvincibilityTime[spell] > 0) InvincibilityTime[spell] -= 1;
                else InvincibilityTime.Remove(spell);
            if (ImmuneTime > 0) ImmuneTime -= 1;
            if (Initialized)
            {
                Update(npc);
                return;
            }

            if (npc.lifeMax < 10) return;

            InvincibilityTime = new Dictionary<ProceduralSpell, int>();
            Player player = Main.netMode == 2 ? Main.player[0] : Main.player[Main.myPlayer];
            int playerlevel = Main.netMode == 0 ? player.GetModPlayer<PlayerCharacter>().Level : 20;
            npc.lifeMax = (int) Math.Round(npc.lifeMax * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
            npc.life = (int) Math.Round(npc.life * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
            npc.defense = (int) Math.Round(npc.defense * (GetLevel(npc.netID) / 160f + 1f));
            npc.lavaImmune = npc.lavaImmune || npc.defense > 60;

            if (npc.damage > 0 && !npc.boss && Main.rand.Next(3) != 0 || Main.netMode != 0)
            {
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
                int portionSize = (int) Math.Round(npc.damage * EleDmgModifier / 2.0 / count);
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    if (haselement[element])
                        ElementalDamage[element] = Math.Max(1, portionSize);
                DealsEleDmg = count > 0;
            }

            if (Main.rand.Next(8) < 3 && !npc.boss && !npc.townNPC && !npc.friendly && Main.netMode != 1)
                InitializeModifiers(npc);

            if (!Main.expertMode)
            {
                npc.lifeMax = (int) (npc.lifeMax * 1.3);
                npc.life = (int) (npc.life * 1.3);
            }

            Initialized = true;
        }

        public override void ResetEffects(NPC npc)
        {
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                if (!HasAilment[element]) AilmentIntensity[element] = 0;
                HasAilment[element] = false;
            }
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

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            Player player = Main.player[npc.target];
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            character.AccuracyCounter += character.HitChance;

            float dodgeChanceModifier = 1f;

            foreach (NpcModifier t in Modifiers)
                dodgeChanceModifier *= t.StrikeNPC(npc, damage, defense, knockback, hitDirection, crit);

            if (character.AccuracyCounter < 1 * dodgeChanceModifier && !character.Rituals[RITUAL.WARRIOR_OATH])
            {
                npc.NinjaDodge(npc, 10);
                if (Vector2.Distance(player.Center, npc.Center) < 192)
                {
                    player.immune = true;
                    player.immuneTime = 30;
                }

                damage = 0;
                crit = false;
                if (character.player.inventory[character.player.selectedItem] != character.LastSelectedWeapon)
                    knockback = 0f;
                SyncCounters(npc.target, character, false);
                return false;
            }

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
            if (Main.netMode != 2) return;

            ModPacket packet = mod.GetPacket();
            packet.Write((byte) (crit ? Message.SyncCritHit : Message.SyncHit));
            packet.Write(player);
            packet.Write(crit ? character.CritAccuracyCounter : character.AccuracyCounter);
            packet.Send();
        }

        public void Update(NPC npc)
        {
            if (npc.aiStyle == 3 && Math.Abs(npc.velocity.Y) < .01)
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.direction * Math.Max(Math.Abs(npc.velocity.X), 8f), 1f * SpeedModifier / 20f);

            foreach (NpcModifier t in Modifiers)
                t.Update(npc);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!HasAilment[ELEMENT.FIRE])
                return;
            if (npc.lifeRegen > 0) npc.lifeRegen = 0;
            npc.lifeRegen -= AilmentIntensity[ELEMENT.FIRE] * 2;
            damage = AilmentIntensity[ELEMENT.FIRE] / 3;
        }
    }
}