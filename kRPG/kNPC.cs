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
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using kRPG.Items.Weapons.RangedDrops;

namespace kRPG
{
    public class kNPC : GlobalNPC
    {
        public const double ELE_DMG_MODIFIER = 1.4;

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        private bool initialized = false;
        public int[] invincibilityTime = new int[Main.projectile.Length];
        public int immuneTime = 0;
        public bool dealseledmg = false;

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
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (hasAilment[ELEMENT.SHADOW])
                damage = damage * (20 + 9360 / (130 + ailmentIntensity[ELEMENT.SHADOW])) / 100;
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (projectile.modProjectile is ProceduralSpellProj)
                invincibilityTime[projectile.whoAmI] = 18;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            PlayerCharacter character = Main.player[npc.target].GetModPlayer<PlayerCharacter>();
            character.accuracyCounter += character.hitChance;
            if (character.accuracyCounter < 1f && !character.rituals[RITUAL.WARRIOR_OATH])
            {
                npc.NinjaDodge(npc, 10);
                damage = 0;
                crit = false;
                if (character.player.inventory[character.player.selectedItem] != character.lastSelectedWeapon)
                    knockback = 0f;
                SyncCounters(npc.target, character, false);
                return false;
            }
            character.accuracyCounter -= 1f;
            SyncCounters(npc.target, character, false);
            character.critAccuracyCounter += character.critHitChance;
            if (crit)
                if (character.critAccuracyCounter < 1f)
                    crit = false;
            else
                character.critAccuracyCounter -= 1f;
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
            else return null;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (immuneTime > 0) return false;
            else return null;
        }

        public override void PostAI(NPC npc)
        {
            for (int i = 0; i < invincibilityTime.Length; i += 1)
                if (invincibilityTime[i] > 0) invincibilityTime[i] -= 1;
            if (immuneTime > 0) immuneTime -= 1;
            if (initialized) return;
            if (npc.lifeMax < 10) return;
            for (int i = 0; i < invincibilityTime.Length; i += 1)
                invincibilityTime[i] = 0;
            //npc.lifeMax = (int)(0.49*Math.Pow(npc.lifeMax, 1.34));
            Player player = Main.netMode == 2 ? Main.player[0] : Main.player[Main.myPlayer];
            int playerlevel = Main.netMode == 0 ? player.GetModPlayer<PlayerCharacter>().level : 20;
            npc.lifeMax = (int)Math.Round(npc.lifeMax * (GetLevel(npc.netID) / 33f + 0.5f + playerlevel * 0.02f));
            npc.life = (int)Math.Round(npc.life * (GetLevel(npc.netID) / 33f + 0.5f + playerlevel * 0.02f));
            npc.defense = (int)Math.Round(npc.defense * (GetLevel(npc.netID) / 200f + 1f));
            //npc.damage = (int)(0.8*Math.Pow(npc.damage, 1.31));
            //npc.damage = (int)(0.8 * Math.Pow(npc.damage, 1.28));
            npc.lavaImmune = npc.lavaImmune || npc.defense > 60;
            if (npc.damage > 0 && !npc.boss && Main.rand.Next(3) != 0 || Main.netMode != 0)
            {
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
                int portionsize = (int)Math.Round((double)npc.damage * ELE_DMG_MODIFIER / 2.0 / count);
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    if (haselement[element]) elementalDamage[element] = Math.Max(1, portionsize);
                dealseledmg = count > 0;
            }
            if (!Main.expertMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.3);
                npc.life = (int)(npc.life * 1.3);
            }

            initialized = true;
        }

        public override void NPCLoot(NPC npc)
        {
            if (npc.lifeMax < 10) return;
            if (npc.friendly) return;
            if (npc.townNPC) return;

            if (Main.rand.Next(3000) < GetLevel(npc.type))
            {
                if (Main.rand.Next(4) == 0)
                    Item.NewItem(npc.position, mod.ItemType<BlacksmithCrown>());
                else
                    Item.NewItem(npc.position, mod.ItemType<PermanenceCrown>());
            }
            //Item.NewItem(npc.position, mod.GetItem("EyeOnAStick").item.type);

            int level = GetLevel(npc.netID);

            Player player = Main.player[npc.target] == null ? Main.player[0] : Main.player[npc.target];
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            foreach (Player p in Main.player)
                if (p != null)
                    if (p.active)
                        if (p.GetModPlayer<PlayerCharacter>() != null)
                            if (p.GetModPlayer<PlayerCharacter>().level > character.level)
                                character = p.GetModPlayer<PlayerCharacter>();
            
            int life = npc.type == NPCID.SolarCrawltipedeTail || npc.type == NPCID.SolarCrawltipedeBody || npc.type == NPCID.SolarCrawltipedeHead ? npc.lifeMax / 8 : npc.lifeMax;
            int defFactor = npc.defense * life / (character.level + 10);
            int baseExp = Main.rand.Next((life + defFactor) / 5) + (life + defFactor) / 6;
            int scaled = Main.expertMode ? (int)(baseExp * 0.55) : baseExp;
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
            
            float dps = Math.Min((float)(Math.Pow(1.037, Math.Min(120, character.level)) * 10), (float)(Math.Pow(1.023, level)*15) + 14);
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

            else if (Main.rand.Next(14) < (character.level < 5 ? 3 : (character.level < 10 ? 2 : 1)))
            {
                if (Main.rand.Next(5) == 0) Item.NewItem(npc.position, RangedWeapon.NewRangedWeapon(mod, npc.position, level, character.level, dps * 0.9f, assumedDef), Main.rand.Next(20, 61));
                else if (Main.rand.Next(9) < 5) ProceduralSword.GenerateSword(mod, npc.position, GetTheme(player), dps, assumedDef);
                else ProceduralStaff.GenerateStaff(mod, npc.position, GetStaffTheme(player), dps * 1.1f, assumedDef);
            }

            else if (Main.rand.Next(35) == 0)
            {
                Item item = Main.item[Item.NewItem(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), mod.ItemType(Glyph.GetRandom()))];
                if (item.modItem is Glyph && Main.netMode == 0)
                {
                    ((Glyph)item.modItem).Randomize();
                }
            }

            /*if (level < 7) // Slimes, etc.
                RollDrops(npc, new int[2] { 50, 110 }, new int[2][] {
                    new int[4] { ItemID.WoodenBoomerang, ItemID.Spear, ItemID.WandofSparking, ItemID.CopperBroadsword },
                    new int[6] { ItemID.BorealWoodHelmet, ItemID.BorealWoodBreastplate, ItemID.BorealWoodGreaves, ItemID.CactusHelmet, ItemID.CactusBreastplate, ItemID.CactusLeggings }
                }, new int[1] { 100 }, new Action<NPC>[1] { 
                    delegate { ProceduralSword.GenerateSword(mod, npc.position, THEME.GENERIC, (level + 2)*2, 4); }
                });

            else if (level < 15) // Demon eyes, Zombies, etc.
                RollDrops(npc, new int[2] { 10, 30 }, new int[2][] {
                    new int[2] { ItemID.EbonwoodBow, ItemID.EnchantedBoomerang },
                    new int[3] { ItemID.GladiatorHelmet, ItemID.GladiatorBreastplate, ItemID.GladiatorLeggings }
                }, new int[2] { 70, 20 }, new Action<NPC>[2] {
                    delegate { ProceduralSword.GenerateSword(mod, npc.position, THEME.GENERIC, (level + 2)*3/2, npc.defense / 2); },
                    delegate { ProceduralSword.GenerateSword(mod, npc.position, THEME.MONSTROUS, (level + 2)*3/2, npc.defense / 2); }
                });

            else if (level < 18) // Eaters of Souls, Skeletons, etc
                RollDrops(npc, new int[2] { 45, 30 }, new int[2][] {
                    new int[5] { ItemID.TopazStaff, ItemID.SilverBroadsword, mod.GetItem("BarbarianSword").item.type, mod.GetItem("FineSteelKatana").item.type, mod.GetItem("Arbalest").item.type },
                    new int[6] { ItemID.PumpkinHelmet, ItemID.PumpkinBreastplate, ItemID.PumpkinLeggings, ItemID.IronHelmet, ItemID.IronChainmail, ItemID.IronGreaves }
                }, new int[1] { 110 }, new Action<NPC>[1] { 
                    delegate { ProceduralSword.GenerateSword(mod, npc.position, THEME.MONSTROUS, level*3/2, npc.defense / 2);
                }});

            else if (level < 20) // Harpies, Sharks, etc (add < 15 category)
                RollDrops(npc, new int[2] { 55, 40 }, new int[2][] {
                    new int[5] { ItemID.SapphireStaff, ItemID.PlatinumBroadsword, ItemID.SilverBow, mod.GetItem("BarbarianSword").item.type, mod.GetItem("Scythe").item.type },
                    new int[7] { ItemID.ObsidianHelm, ItemID.ObsidianShirt, ItemID.ObsidianPants, ItemID.SapphireRobe, ItemID.SilverHelmet, ItemID.SilverChainmail, ItemID.SilverGreaves }
                });

            else if (level < 28) // Demons, Hornets, etc
                RollDrops(npc, new int[4] { 50, 20, 8, 12 }, new int[4][] {
                    new int[8] { ItemID.LightsBane, ItemID.Musket, ItemID.DemonBow, ItemID.Katana, ItemID.BallOHurt, ItemID.GreenPhaseblade, ItemID.DiamondStaff, ItemID.AquaScepter },
                    new int[9] { ItemID.GoldHelmet, ItemID.GoldChainmail, ItemID.GoldGreaves, ItemID.MeteorHelmet, ItemID.MeteorSuit, ItemID.MeteorLeggings, ItemID.PlatinumHelmet, ItemID.PlatinumChainmail, ItemID.PlatinumGreaves },
                    new int[8] { ItemID.DyeTradersScimitar, ItemID.Muramasa, ItemID.SpaceGun, ItemID.Musket, ItemID.WaterBolt, ItemID.PurplePhaseblade, ItemID.RedPhaseblade, ItemID.MoltenFury },
                    new int[6] { ItemID.NecroHelmet, ItemID.NecroBreastplate, ItemID.NecroGreaves, ItemID.ShadowHelmet, ItemID.ShadowScalemail, ItemID.ShadowGreaves }
                });*/

            else if (npc.FullName.EndsWith(" Eye") && level < 20)
            {
                if (Main.rand.Next(20) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.MONSTROUS), SwordBlade.demonEye, Main.rand.Next(5) < 2 ? SwordAccent.RandomAccent() : SwordAccent.none, dps, assumedDef);
                else if (Main.rand.Next(15) == 0)
                    ProceduralSword.NewSword(mod, npc.position, SwordHilt.RandomHilt(SWORDTHEME.GENERIC), SwordBlade.demonEye, SwordAccent.none, dps, assumedDef);
            }

            //ProceduralSword.GenerateSword(mod, npc.position, GetTheme(player), dps, assumedDef);
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
