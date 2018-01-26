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
using Terraria.Audio;

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
        private bool lifeRegen = false;
        private int regenTimer = 0;
        private float speedModifier = 1f;
        private bool elusive = false;
        private bool sagely = false;
        private bool explosive = false;
        
        public ProceduralSpellProj rotMissile = null;
        public ProceduralSpellProj rotSecondary = null;

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

            if (npc.friendly) return;

            if (elementalDamage[ELEMENT.FIRE] > 0)
            {
                if (Main.rand.Next(3) == 0)
                {
                    int height = npc.height / 6;
                    int dust = Dust.NewDust(npc.BottomLeft - new Vector2(2f, height - 4f), npc.width + 4, height + 4, DustID.Fire, 0f, 0f, 100, default(Color), 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Zero;
                }
            }

            if (elementalDamage[ELEMENT.COLD] > 0)
            {
                if (Main.rand.Next(3) == 0)
                {
                    int height = npc.height / 6;
                    int dust = Dust.NewDust(npc.BottomLeft - new Vector2(2f, height - 4f), npc.width + 4, height + 4, mod.GetDust<Ice>().Type, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Zero;
                }
            }

            if (elementalDamage[ELEMENT.LIGHTNING] > 0)
            {
                if (Main.rand.Next(3) == 0)
                {
                    int height = npc.height / 6;
                    int dust = Dust.NewDust(npc.BottomLeft - new Vector2(2f, height - 4f), npc.width + 4, height + 4, DustID.Electric, 0f, 0f, 100, default(Color), 0.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Zero;
                }
            }

            if (elementalDamage[ELEMENT.SHADOW] > 0)
            {
                if (Main.rand.Next(3) == 0)
                {
                    int height = npc.height / 6;
                    int dust = Dust.NewDust(npc.BottomLeft - new Vector2(2f, height - 4f), npc.width + 4, height + 4, DustID.Shadowflame, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Zero;
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
            Player player = Main.player[npc.target];
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            character.accuracyCounter += character.hitChance;
            if (character.accuracyCounter < (elusive ? 1.2f : 1f) && !character.rituals[RITUAL.WARRIOR_OATH])
            {
                npc.NinjaDodge(npc, 10);
                if (Vector2.Distance(player.Center, npc.Center) < 192)
                {
                    player.immune = true;
                    player.immuneTime = 30;
                }
                damage = 0;
                crit = false;
                if (character.player.inventory[character.player.selectedItem] != character.lastSelectedWeapon)
                    knockback = 0f;
                SyncCounters(npc.target, character, false);
                return false;
            }
            else
                character.accuracyCounter -= (elusive ? 1.2f : 1f);
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
            Player player = Main.netMode == 2 ? Main.player[0] : Main.player[Main.myPlayer];

            int playerlevel = 0;
            if (Main.netMode == 0) playerlevel = Main.LocalPlayer.GetModPlayer<PlayerCharacter>().level;
            else
            {
                int count = 0;
                for (int i = 0; i < Main.player.Length; i += 1)
                    if (Main.player[i] != null)
                        if (Main.player[i].active)
                        {
                            playerlevel += Main.player[i].GetModPlayer<PlayerCharacter>().level;
                            count += 1;
                        }
                playerlevel = playerlevel / count + 1;
            }

            npc.lifeMax = (int)Math.Round(npc.lifeMax * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
            npc.life = (int)Math.Round(npc.life * (GetLevel(npc.netID) / 30f + 0.4f + playerlevel * 0.025f));
            npc.defense = (int)Math.Round(npc.defense * (GetLevel(npc.netID) / 160f + 1f));
            npc.lavaImmune = npc.lavaImmune || npc.defense > 60;
            if (npc.damage > 0 && !npc.boss && Main.rand.Next(3) != 0 && Main.netMode != 1)
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
            if (Main.rand.Next(8) < 3 && !npc.boss && !npc.townNPC && !npc.friendly && Main.netMode != 1) Prefix(npc);
            if (!Main.expertMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.3);
                npc.life = (int)(npc.life * 1.3);
            }
            initialized = true;
        }

        public void Update(NPC npc)
        {
            if (lifeRegen) regenTimer += 1;
            int amount = npc.lifeMax / 20;
            if (regenTimer > 60f / amount)
            {
                npc.life = Math.Min(npc.life + (int)(regenTimer / (60f / amount)), npc.lifeMax);
                regenTimer = regenTimer % (60 / amount);
            }
            if (npc.aiStyle == 3 && npc.velocity.Y == 0f)
                    npc.velocity.X = MathHelper.Lerp(npc.velocity.X, npc.direction * Math.Max(Math.Abs(npc.velocity.X), 8f), 1f * speedModifier / 20f);
            if (sagely)
            {
                try
                {
                    int rotDistance = 64;
                    int rotTimeLeft = 36000;

                    if (rotMissile != null)
                        if (rotMissile.projectile.active && npc.active)
                            goto Secondary;
                        else
                            rotMissile.projectile.Kill();

                    Projectile proj1 = Main.projectile[Projectile.NewProjectile(npc.Center, new Vector2(0f, -1.5f), mod.ProjectileType<ProceduralSpellProj>(), npc.damage, 3f)];
                    proj1.hostile = true;
                    proj1.friendly = false;
                    ProceduralSpellProj ps1 = (ProceduralSpellProj)proj1.modProjectile;
                    ps1.origin = proj1.position;
                    Cross cross1 = Main.rand.Next(2) == 0 ? (Cross)new Cross_Red() : new Cross_Violet();
                    ps1.ai.Add(delegate (ProceduralSpellProj spell)
                    {
                        cross1.GetAIAction()(spell);

                        float displacementAngle = (float)API.Tau / 4f;
                        Vector2 displacementVelocity = Vector2.Zero;
                        if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                        {
                            Vector2 unitRelativePos = spell.RelativePos(spell.caster.Center);
                            unitRelativePos.Normalize();
                            spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                            displacementVelocity = new Vector2(-2f, 0f).RotatedBy((spell.RelativePos(spell.caster.Center)).ToRotation() + (float)API.Tau / 4f);

                            float angle = displacementAngle - 0.06f * (float)(rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                            spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                        }
                        else
                        {
                            spell.projectile.Center = spell.caster.Center + new Vector2(0f, -1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                        }
                        spell.projectile.velocity = displacementVelocity + spell.caster.velocity;
                        spell.basePosition = spell.caster.position;
                    });
                    ps1.init.Add(cross1.GetInitAction());
                    ps1.caster = npc;
                    ps1.Initialize();
                    ps1.projectile.penetrate = -1;
                    ps1.projectile.timeLeft = rotTimeLeft;
                    rotMissile = ps1;

                    Secondary:

                    if (rotSecondary != null)
                        if (rotSecondary.projectile.active && npc.active)
                            return;
                        else
                            rotSecondary.projectile.Kill();

                    Projectile proj2 = Main.projectile[Projectile.NewProjectile(npc.Center, new Vector2(0f, 1.5f), mod.ProjectileType<ProceduralSpellProj>(), npc.damage, 3f)];
                    proj2.hostile = true;
                    proj2.friendly = false;
                    ProceduralSpellProj ps2 = (ProceduralSpellProj)proj2.modProjectile;
                    ps2.origin = proj2.position;
                    Cross cross2 = Main.rand.Next(2) == 0 ? (Cross)new Cross_Blue() : new Cross_Purple();
                    ps2.ai.Add(delegate (ProceduralSpellProj spell)
                    {
                        cross2.GetAIAction()(spell);

                        float displacementAngle = (float)API.Tau / 4f + (float)Math.PI;
                        Vector2 displacementVelocity = Vector2.Zero;
                        if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                        {
                            Vector2 unitRelativePos = spell.RelativePos(spell.caster.Center);
                            unitRelativePos.Normalize();
                            spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                            displacementVelocity = new Vector2(-2f, 0f).RotatedBy((spell.RelativePos(spell.caster.Center)).ToRotation() + (float)API.Tau / 4f);

                            float angle = displacementAngle - 0.06f * (float)(rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                            spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                        }
                        else
                        {
                            spell.projectile.Center = spell.caster.Center + new Vector2(0f, 1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                        }
                        spell.projectile.velocity = displacementVelocity + spell.caster.velocity;
                        spell.basePosition = spell.caster.position;
                    });
                    ps2.init.Add(cross2.GetInitAction());
                    ps2.caster = npc;
                    ps2.Initialize();
                    ps2.projectile.penetrate = -1;
                    ps2.projectile.timeLeft = rotTimeLeft;
                    rotSecondary = ps2;
                }
                catch (SystemException e)
                {
                    Main.NewText(e.ToString());
                    ErrorLogger.Log(e.ToString());
                }
            }
        }

        public void Prefix(NPC npc, int i = -1)
        {
            int prefix;
            Reroll:
            prefix = i == -1 ? Main.rand.Next(8) : i;
            switch(prefix)
            {
                default:
                    if (npc.aiStyle != 3) goto Reroll;
                    npc.GivenName = "Swift " + npc.FullName;
                    speedModifier *= 2f;
                    break;
                case 1:
                    npc.GivenName = "Massive " + npc.FullName;
                    npc.scale *= 1.11f;
                    speedModifier *= 1.1f;
                    npc.lifeMax = (int)(npc.lifeMax * 1.4);
                    npc.life = (int)(npc.life * 1.4);
                    break;
                case 2:
                    npc.GivenName = "Shimmering " + npc.FullName;
                    lifeRegen = true;
                    break;
                case 3:
                    npc.GivenName = "Elusive " + npc.FullName;
                    elusive = true;
                    npc.scale *= 0.8f;
                    speedModifier *= 1.25f;
                    break;
                case 4:
                    npc.GivenName = "Brutal " + npc.FullName;
                    npc.damage = (int)Math.Round(npc.damage * 1.2);
                    npc.defense = 0;
                    break;
                case 5:
                    if (npc.aiStyle == 6 || Main.netMode != 0) goto Reroll;
                    npc.GivenName = "Sagely " + npc.FullName;
                    sagely = true;
                    break;
                case 6:
                    npc.GivenName = "Explosive " + npc.FullName;
                    npc.lifeMax = (int)(npc.lifeMax * 0.5);
                    explosive = true;
                    break;
            }
            npc.scale *= 1.1f;
            npc.lifeMax = (int)(npc.lifeMax * 1.2);
            
            speedModifier *= 1.09f;
        }

        public override void NPCLoot(NPC npc)
        {
            if (explosive)
            {
                Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), npc.Center);
                Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, mod.ProjectileType<NPC_Explosion>(), npc.damage * 5 / 4, 0f)];
            }

            if (npc.lifeMax < 10) return;
            if (npc.friendly) return;
            if (npc.townNPC) return;

            if (Main.rand.Next(1500) < GetLevel(npc.type))
            {
                if (Main.rand.Next(8) == 0)
                    Item.NewItem(npc.position, mod.ItemType<BlacksmithCrown>());
                else
                    Item.NewItem(npc.position, mod.ItemType<PermanenceCrown>());
            }

            int level = GetLevel(npc.netID);

            int playerlevel = 0;
            if (Main.netMode == 0) playerlevel = Main.LocalPlayer.GetModPlayer<PlayerCharacter>().level;
            else
            {
                int count = 0;
                for (int i = 0; i < Main.player.Length; i += 1)
                    if (Main.player[i] != null)
                        if (Main.player[i].active)
                        {
                            playerlevel += Main.player[i].GetModPlayer<PlayerCharacter>().level;
                            count += 1;
                        }
                playerlevel = playerlevel / count + 1;
            }

            int life = npc.type == NPCID.SolarCrawltipedeTail || npc.type == NPCID.SolarCrawltipedeBody || npc.type == NPCID.SolarCrawltipedeHead ? npc.lifeMax / 8 : npc.lifeMax;
            int defFactor = npc.defense * life / (level + 10);
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
                Main.LocalPlayer.GetModPlayer<PlayerCharacter>().AddXP(scaled);

            if (level < Math.Min(playerlevel - 15, 70)) return;
            
            float dps = Math.Min((float)(Math.Pow(1.04, Math.Min(130, playerlevel)) * 9f), (float)(Math.Pow(1.024, level)*15) + 18);
            int assumedDef = !Main.hardMode ? 5 : playerlevel/3;

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

            else if (Main.rand.Next((playerlevel < 5 ? 5 : (playerlevel < 10 ? 8 : 20))) == 0)
            {
                Player player = Main.player[npc.target] != null ? Main.player[npc.target] : Main.player[0];
                if (Main.rand.Next(5) == 0) Item.NewItem(npc.position, RangedWeapon.NewRangedWeapon(mod, npc.position, level, playerlevel, dps, assumedDef), Main.rand.Next(30, 90));
                else if (Main.rand.Next(9) < 5) ProceduralSword.GenerateSword(mod, npc.position, GetTheme(player), dps, assumedDef);
                else ProceduralStaff.GenerateStaff(mod, npc.position, GetStaffTheme(player), dps * 1.2f, assumedDef);
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
                if (Main.rand.Next(20) == 0)
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
