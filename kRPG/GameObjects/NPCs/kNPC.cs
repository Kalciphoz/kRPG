using System;
using System.Collections.Generic;
using System.Linq;
using kRPG.Enums;
using kRPG.GameObjects.Items.Crowns;
using kRPG.GameObjects.Items.Dusts;
using kRPG.GameObjects.Items.Glyphs;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.Items.Weapons.Melee;
using kRPG.GameObjects.Items.Weapons.Ranged;
using kRPG.GameObjects.Modifiers;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.Spells;
using kRPG.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.NPCs
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

        public static List<Func<kNPC, NPC, NpcModifier>> ModifierFuncs { get; set; } = new List<Func<kNPC, NPC, NpcModifier>>
        {
            DamageModifier.New,
            ElusiveModifier.New,
            ExplosiveModifier.New,
            LifeRegenModifier.New,
            SageModifier.New,
            SizeModifier.New,
            GameObjects.Modifiers.SpeedModifier.New
        };

        public static string[] ModifierDictionary =
        {
            typeof(DamageModifier).AssemblyQualifiedName, typeof(ElusiveModifier).AssemblyQualifiedName, typeof(ExplosiveModifier).AssemblyQualifiedName, typeof(LifeRegenModifier).AssemblyQualifiedName, typeof(SageModifier).AssemblyQualifiedName,
            typeof(SizeModifier).AssemblyQualifiedName, typeof(SpeedModifier).AssemblyQualifiedName
        };

        public List<NpcModifier> Modifiers { get; set; } = new List<NpcModifier>();



        public float SpeedModifier { get; set; } = 1f;

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

            if (HasAilment[Element.Fire])
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

            if (HasAilment[Element.Cold])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.GetInstance<Ice>().Type,
                        npc.velocity.X, npc.velocity.Y, 100, Color.White, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(npc.position, 0f, 0.4f, 1f);
            }

            if (HasAilment[Element.Lightning])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Electric, npc.velocity.X, npc.velocity.Y,
                        100, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(npc.position, 0.5f, 0.5f, 0.5f);
            }

            if (HasAilment[Element.Shadow])
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
            Dictionary<Element, int> ele = new Dictionary<Element, int>();
            return ElementalDamage[Element.Fire] + ElementalDamage[Element.Cold] + ElementalDamage[Element.Lightning] + ElementalDamage[Element.Shadow];
        }

        public static int GetLevel(int type)
        {
            NPC npc = new NPC();
            npc.SetDefaults(type);
            npc.active = false;
            return Math.Min(Main.expertMode ? (npc.damage + npc.defense * 4) / 3 : (npc.damage * 2 + npc.defense * 4) / 3, npc.boss ? 120 : 110);
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

        public void InitializeModifiers(NPC npc)
        {
            npc.GivenName = npc.FullName;
            int amount = new Random().Next(0, 3);
            for (int i = 0; i < amount; i++)
            {
                int random = Main.rand.Next(ModifierFuncs.Count);
                NpcModifier modifier = ModifierFuncs[random].Invoke(this, npc);
                Modifiers.Add(modifier);
            }

            PrefixNPCPacket.Write(npc, amount, Modifiers);
            MakeNotable(npc);
        }

        public void MakeNotable(NPC npc)
        {
            npc.scale *= 1.1f;
            npc.lifeMax = (int)(npc.lifeMax * 1.2);
            SpeedModifier *= 1.09f;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            foreach (NpcModifier t in Modifiers)
                t.ModifyHitPlayer(npc, target, ref damage, ref crit);

            if (HasAilment[Element.Shadow])
                damage = damage * (20 + 9360 / (130 + AilmentIntensity[Element.Shadow])) / 100;
        }

        public override void NPCLoot(NPC npc)
        {
            foreach (NpcModifier t in Modifiers)
                t.NpcLoot(npc);

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
                Item item = Main.item[
                    Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), mod.ItemType(Glyph.GetRandom()))];
                if (item.modItem is Glyph && Main.netMode == 0)
                    ((Glyph)item.modItem).Randomize();
            }

            else if (npc.FullName.EndsWith(" Eye") && level < 20)
            {
                if (Main.rand.Next(20) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SwordTheme.Monstrous), SwordBlade.DemonEye,
                        Main.rand.Next(5) < 2 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, assumedDef);
                else if (Main.rand.Next(15) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SwordTheme.Generic), SwordBlade.DemonEye, SwordAccent.None, dps,
                        assumedDef);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            foreach (NpcModifier t in Modifiers)
                t.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (!(projectile.modProjectile is ProceduralSpellProj))
                return;
            ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
            if (InvincibilityTime.ContainsKey(ps.Source))
                InvincibilityTime[ps.Source] = 30;
            else
                InvincibilityTime.Add(ps.Source, 30);
        }

        public override void PostAI(NPC npc)
        {
            foreach (NpcModifier t in Modifiers)
                t.PostAi(npc);

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
            npc.lifeMax = (int)Math.Round(npc.lifeMax * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
            npc.life = (int)Math.Round(npc.life * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
            npc.defense = (int)Math.Round(npc.defense * (GetLevel(npc.netID) / 160f + 1f));
            npc.lavaImmune = npc.lavaImmune || npc.defense > 60;

            if (npc.damage > 0 && !npc.boss && Main.rand.Next(3) != 0 || Main.netMode != 0)
            {
                Dictionary<Element, bool> hasElement = new Dictionary<Element, bool>
                {
                    {
                        Element.Fire,
                        player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert ||
                        Main.rand.Next(10) == 0 && Main.netMode == 0
                    },
                    {
                        Element.Cold,
                        player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain ||
                        Main.rand.Next(10) == 0 && Main.netMode == 0
                    },
                    {
                        Element.Lightning,
                        player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly ||
                        Main.rand.Next(10) == 0 && Main.netMode == 0
                    },
                    {
                        Element.Shadow,
                        player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula ||
                        !Main.dayTime && Main.rand.Next(10) == 0 && Main.netMode == 0 && player.ZoneOverworldHeight
                    }
                };
                int count = Enum.GetValues(typeof(Element)).Cast<Element>().Count(element => hasElement[element]);
                int portionSize = (int)Math.Round(npc.damage * EleDmgModifier / 2.0 / count);
                foreach (Element element in Enum.GetValues(typeof(Element)))
                    if (hasElement[element])
                        ElementalDamage[element] = Math.Max(1, portionSize);
                DealsEleDmg = count > 0;
            }

            //if (Main.rand.Next(8) < 3 && !npc.boss && !npc.townNPC && !npc.friendly && Main.netMode != 1)
            if (!npc.boss && !npc.townNPC && !npc.friendly && Main.netMode != 1)
            {
                //So we are randomly adding modifiers...
                InitializeModifiers(npc);
            }

            if (!Main.expertMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.3);
                npc.life = (int)(npc.life * 1.3);
            }

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
                dodgeChanceModifier *= t.StrikeNpc(npc, damage, defense, knockback, hitDirection, crit);

            if (character.AccuracyCounter < 1 * dodgeChanceModifier && !character.Rituals[Ritual.WarriorOath])
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
            if (crit)
                SyncCritHitPacket.Write(player, character.CritAccuracyCounter);
            else
                SyncHitPacket.Write(player, character.AccuracyCounter);
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
            if (!HasAilment[Element.Fire])
                return;
            if (npc.lifeRegen > 0) npc.lifeRegen = 0;
            npc.lifeRegen -= AilmentIntensity[Element.Fire] * 2;
            damage = AilmentIntensity[Element.Fire] / 3;
        }
    }
}