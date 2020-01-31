//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
// 
// 
// THIS SOFTWARE IS PROVIDED BY FairfieldTek LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using kRPG2.Buffs;
using kRPG2.Enums;
using kRPG2.GUI;
using kRPG2.Items;
using kRPG2.Items.Dusts;
using kRPG2.Items.Glyphs;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG2
{
    public class PlayerCharacter : ModPlayer
    {
        public const int DefaultMaxUpgradeLevel = 7;

        public PlayerCharacter()
        {
            foreach (STAT stat in Enum.GetValues(typeof(STAT)))
            {
                BaseStats[stat] = 0;
                TempStats[stat] = 0;
            }

            Permanence = 0;
            Transcendence = 0;
            for (int i = 0; i < Abilities.Length; i += 1)
            {
                Abilities[i] = new ProceduralSpell(mod);
                for (int j = 0; j < Abilities[i].Glyphs.Length; j += 1)
                {
                    Abilities[i].Glyphs[j] = new Item();
                    Abilities[i].Glyphs[j].SetDefaults(0, true);
                }
            }

            Abilities[0].Key = Keys.Z;
            Abilities[1].Key = Keys.X;
            Abilities[2].Key = Keys.C;
            Abilities[3].Key = Keys.V;

            Inventories = new Item[3][];
            for (int i = 0; i < Inventories.Length; i += 1)
            {
                Inventories[i] = new Item[40];
                for (int j = 0; j < Inventories[i].Length; j += 1)
                {
                    Inventories[i][j] = new Item();
                    Inventories[i][j].SetDefaults(0, true);
                }
            }
        }

        public ProceduralSpell[] Abilities { get; set; } = new ProceduralSpell[4];
        public AbilitiesGui AbilitiesGui { get; set; }
        public int Accuracy { get; set; }

        public float AccuracyCounter { get; set; } = 0.5f;
        public int ActiveInvPage { get; set; }

        public Dictionary<ELEMENT, int> AilmentIntensity { get; set; } = new Dictionary<ELEMENT, int>
        {
            {ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}
        };

        public Dictionary<ELEMENT, Type> Ailments { get; set; } = new Dictionary<ELEMENT, Type>
        {
            {ELEMENT.FIRE, typeof(Fire)}, {ELEMENT.COLD, typeof(Cold)}, {ELEMENT.LIGHTNING, typeof(Lightning)}, {ELEMENT.SHADOW, typeof(Shadow)}
        };

        public int Allres { get; set; }
        public AnvilGUI AnvilGui { get; set; }

        public Dictionary<STAT, int> BaseStats { get; set; } = new Dictionary<STAT, int>();
        public int BigCritCounter { get; set; } = 50;
        public int BigHitCounter { get; set; } = 50;

        public int BonusLife { get; set; }
        public int BonusMana { get; set; }
        public bool CanHealMana { get; set; } = true;
        public List<ProceduralSpellProj> CirclingProtection { get; set; } = new List<ProceduralSpellProj>();
        public float CritAccuracyCounter { get; set; } = 0.5f;

        public int CritBoost { get; set; }
        public int CritCounter { get; set; } = 50;

        public float CritHitChance
        {
            get
            {
                float diff = 4f + Level / 12f;
                return 1f - diff * (1f - 0.8f) / (Accuracy + diff);
            }
        }

        public float CritMultiplier { get; set; } = 1f;

        public float DamageMultiplierPercent => 1f + TotalStats(STAT.POTENCY) * 0.05f + Math.Min(0.09f, TotalStats(STAT.POTENCY) * 0.06f);
        private float DegenTimer { get; set; }

        public Dictionary<ELEMENT, int> Eleres { get; set; } = new Dictionary<ELEMENT, int>
        {
            {ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}
        };

        public int Evasion { get; set; } = 2;
        public int EvasionCounter { get; set; } = 50;
        public int Experience { get; set; }

        public Dictionary<ELEMENT, bool> HasAilment { get; set; } = new Dictionary<ELEMENT, bool>
        {
            {ELEMENT.FIRE, false}, {ELEMENT.COLD, false}, {ELEMENT.LIGHTNING, false}, {ELEMENT.SHADOW, false}
        };

        public float HitChance
        {
            get
            {
                float diff = 7f + Level / 40f;
                return 1f - diff * (1f - 0.85f) / (Accuracy + diff);
            }
        }

        private bool Initialized { get; set; }

        public Item[][] Inventories { get; set; } = new Item[3][] {new Item[40], new Item[40], new Item[40]};
        public InventoryGui InventoryGui { get; set; }

        public double ItemRotation { get; set; }
        public Item LastSelectedWeapon { get; set; }
        private int LeechCooldown { get; set; }

        public int Level { get; set; } = 1;

        private int LevelAnimation { get; set; }
        public LevelGui LevelGui { get; set; }
        public float LifeDegen { get; set; }

        public float LifeLeech { get; set; }

        public float LifeRegen { get; set; } = 1f;

        public int Mana { get; set; }
        public float ManaRegen { get; set; }
        private float ManaRegenTimer { get; set; }
        public List<ProceduralMinion> Minions { get; set; } = new List<ProceduralMinion>();
        public int Permanence { get; set; }

        public int PointsAllocated
        {
            get { return Enum.GetValues(typeof(STAT)).Cast<STAT>().Sum(stat => BaseStats[stat]); }
        }

        private float RegenTimer { get; set; }

        public Dictionary<ELEMENT, int> Resistance
        {
            get
            {
                var dict = new Dictionary<ELEMENT, int>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    dict[element] = Eleres[element] + Allres;
                return dict;
            }
        }

        public Dictionary<RITUAL, bool> Rituals { get; set; } = new Dictionary<RITUAL, bool>();
        public ProceduralSpell SelectedAbility { get; set; } = null;
        public SpellcraftingGUI SpellCraftingGui { get; set; }
        public List<SpellEffect> SpellEffects { get; set; } = new List<SpellEffect>();
        public bool StatPage { get; set; } = true;
        public StatusBar StatusBar { get; set; }
        public Dictionary<STAT, int> TempStats { get; set; } = new Dictionary<STAT, int>();
        public List<Trail> Trails { get; set; } = new List<Trail>();
        public int Transcendence { get; set; }

        public void AddXp(int xp)
        {
            if (Main.gameMenu) return;
            if (xp == 0) return;
            Experience += xp;

            Check:
            if (Experience >= ExperienceToLevel())
            {
                Experience -= ExperienceToLevel();
                LevelUp();
                goto Check;
            }

            CombatText.NewText(player.getRect(), new Color(127, 159, 255), xp + " XP");
        }

        public float DamageMultiplier(ELEMENT? element, bool melee, bool ranged = false, bool magic = false, bool thrown = false, bool minion = false)
        {
            float dmgModifier = 1f;
            if (melee) dmgModifier *= player.meleeDamage;
            if (ranged) dmgModifier *= player.rangedDamage;
            if (magic) dmgModifier *= player.magicDamage;
            if (thrown) dmgModifier *= player.thrownDamage;
            if (minion) dmgModifier *= player.minionDamage;
            return dmgModifier;
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Main.netMode == 2 || Main.myPlayer != player.whoAmI) return;
            if (player.statLife < 1) return;
            if (HasAilment[ELEMENT.FIRE])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.Fire, player.velocity.X * 0.4f,
                        player.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }

                Lighting.AddLight(player.position, 0.7f, 0.4f, 0.1f);
                fullBright = true;
            }

            if (HasAilment[ELEMENT.COLD])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.GetInstance<Ice>().Type,
                        player.velocity.X, player.velocity.Y, 100, Color.White, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(player.position, 0f, 0.4f, 1f);
            }

            if (HasAilment[ELEMENT.LIGHTNING])
            {
                if (Main.rand.Next(2) == 0)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.Electric, player.velocity.X,
                        player.velocity.Y, 100, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(player.position, 0.5f, 0.5f, 0.5f);
                fullBright = true;
            }

            if (HasAilment[ELEMENT.SHADOW])
                if (Main.rand.Next(3) < 2)
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.Shadowflame, player.velocity.X,
                        player.velocity.Y, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

            if (Main.netMode == 2) return;
            var spriteBatch = Main.spriteBatch;

            foreach (var trail in Trails.ToArray())
                trail.Draw(spriteBatch, player);

            if (LevelAnimation >= 60)
                return;
            if (LevelAnimation < 24)
            {
                fullBright = true;
                Lighting.AddLight(player.position, 0.9f, 0.9f, 0.9f);
            }
            else
            {
                Lighting.AddLight(player.position, 0.4f, 0.4f, 0.4f);
            }

            spriteBatch.Draw(GFX.LevelUp, player.Bottom - new Vector2(48, 108) - Main.screenPosition, new Rectangle(0, LevelAnimation / 3 * 96, 96, 96),
                Color.White);
            LevelAnimation += 1;
        }

        public int ExperienceToLevel()
        {
            if (Level < 5)
                return 80 + Level * 20;
            if (Level < 10)
                return Level * 40;
            if (Level < 163)
                return (int) (280 * Math.Pow(1.09, Level - 5) + 3 * Level);
            return (int) (2000000000 - 288500000000 / Level);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            foreach (var projectile in Main.projectile)
                if (projectile.modProjectile is ProceduralSpear || projectile.modProjectile is ProceduralMinion)
                    projectile.Kill();
            foreach (var spell in CirclingProtection)
                spell.projectile.Kill();
            CirclingProtection.Clear();
        }

        private void LeechLife(Item item, int damage)
        {
            if (LeechCooldown != 0)
                return;
            int leechAmount = Math.Min((int) (damage * LifeLeech), (int) (player.inventory[player.selectedItem].damage / 2 * (1 + LifeLeech)));
            leechAmount = Math.Min(leechAmount, (int) (player.statLifeMax2 * LifeLeech * 0.2));
            if (leechAmount > 1)
            {
                player.statLife += leechAmount;
                player.HealEffect(leechAmount);
                LeechCooldown = item.useAnimation * 3;
            }

            else if (LifeLeech > 0f)
            {
                player.statLife += 1;
                player.HealEffect(1);
                LeechCooldown = (int) (item.useAnimation * (3 - Math.Min(1.4f, LifeLeech * 10f)));
            }
        }

        public void LevelUp()
        {
            Level += 1;
            if (!Main.gameMenu) GFX.SfxLevelUp.Play(0.5f * Main.soundVolume, 0f, 0f);
            if (Main.netMode == 1)
            {
                var packet = mod.GetPacket();
                packet.Write((byte) Message.SyncLevel);
                packet.Write(player.whoAmI);
                packet.Write(Level);
                packet.Send();
            }

            LevelAnimation = 0;
            Main.NewText("Congratulations! You are now level " + Level, 255, 223, 63);
        }

        private void ModifyDamage(ref int damage, ref bool crit, NPC target, Item item = null, Projectile proj = null)
        {
            if (Rituals[RITUAL.WARRIOR_OATH])
            {
                crit = false;
                float damageBoost = 1f + TotalStats(STAT.RESILIENCE) * 0.04f;
                damageBoost += Math.Min(0.1f, TotalStats(STAT.RESILIENCE) * 0.02f);
                damage = (int) (damage * damageBoost);
            }

            var eleDmg = new Dictionary<ELEMENT, int>();

            if (item != null)
            {
                var ki = item.GetGlobalItem<kItem>();
                damage += ki.GetEleDamage(item, player);
                eleDmg = ki.GetIndividualElements(item, player);
            }
            else if (proj != null)
            {
                var kp = proj.GetGlobalProjectile<kProjectile>();
                damage += kp.GetEleDamage(proj, player);
                eleDmg = kp.GetIndividualElements(proj, player);
            }

            if (HasAilment[ELEMENT.SHADOW])
                damage = Math.Min(damage * 2 / 5, damage - AilmentIntensity[ELEMENT.SHADOW]);
            //    damage = damage * (20 + 9360 / (130 + ailmentIntensity[ELEMENT.SHADOW])) / 100;

            var victim = target.GetGlobalNPC<kNPC>();

            if (!crit && Main.netMode == 0)
                crit = Main.rand.Next(500) < 50 + victim.AilmentIntensity[ELEMENT.COLD];

            if (crit)
            {
                damage = (int) (damage / DamageMultiplierPercent * (DamageMultiplierPercent + CritMultiplier));
                if (Rituals[RITUAL.ELDRITCH_FURY])
                {
                    int i = damage;
                    int c = target.boss ? 7 : 2;
                    damage += Math.Min(Mana * c, i);
                    Mana = Math.Max(0, Mana - i / c);
                }
            }

            if (item == null && proj == null) return;

            if (victim.HasAilment[ELEMENT.LIGHTNING])
                damage += 1 + victim.AilmentIntensity[ELEMENT.LIGHTNING];

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                if (Main.rand.Next(target.boss ? 500 : 200) >= 30 + eleDmg[element])
                    continue;
                if (eleDmg[element] <= 0)
                    continue;
                var t = Ailments[element];
                ModBuff buff;
                if (Ailments[element] == typeof(Fire))
                    buff = ModContent.GetInstance<Fire>();
                else if (Ailments[element] == typeof(Cold))
                    buff = ModContent.GetInstance<Cold>();
                else if (Ailments[element] == typeof(Lightning))
                    buff = ModContent.GetInstance<Lightning>();
                else
                    buff = ModContent.GetInstance<Shadow>();
                target.AddBuff(buff.Type, target.boss ? 30 + Math.Min(eleDmg[element], 30) * 3 : 120 + Math.Min(eleDmg[element], 15) * 12);
                victim.AilmentIntensity[element] = target.boss ? eleDmg[element] / 2 : eleDmg[element];
                victim.HasAilment[element] = true;
            }
        }

        public void ModifyDamageTakenFromNPC(ref int damage, ref bool crit, Dictionary<ELEMENT, int> eleDmg)
        {
            double dmg = 0.5 * Math.Pow(damage, 1.35);
            var originalEle = eleDmg;
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                eleDmg[element] = (int) (0.5 * Math.Pow(eleDmg[element], 1.35));
            if (!Main.expertMode)
            {
                dmg = dmg * 1.3;
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    eleDmg[element] = (int) (eleDmg[element] * 1.3);
            }

            damage = (int) Math.Round(Math.Min(dmg, (double) damage * 3));
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                eleDmg[element] = Math.Min(originalEle[element] * 3, eleDmg[element]);
            bool bossfight = false;
            foreach (var n in Main.npc)
                if (n.active)
                    if (n.boss)
                        bossfight = true;
            int elecount = Enum.GetValues(typeof(ELEMENT)).Cast<ELEMENT>().Count(element => eleDmg[element] > 0);
            if (elecount > 0) damage = (int) Math.Round(damage * (kNPC.EleDmgModifier + 1) / 2);
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                damage -= Math.Min(Resistance[element], eleDmg[element] * 3 / 5);
                if (Main.rand.Next(player.statLifeMax2 + Resistance[element] * 20) >= 15 + eleDmg[element] * (bossfight ? 2 : 8) || Main.netMode == 2)
                    continue;
                if (eleDmg[element] <= 0)
                    continue;
                var t = Ailments[element];
                ModBuff buff;
                if (Ailments[element] == typeof(Fire))
                    buff = ModContent.GetInstance<Fire>();
                else if (Ailments[element] == typeof(Cold))
                    buff = ModContent.GetInstance<Cold>();
                else if (Ailments[element] == typeof(Lightning))
                    buff = ModContent.GetInstance<Lightning>();
                else
                    buff = ModContent.GetInstance<Shadow>();
                player.AddBuff(buff.Type, bossfight ? 90 : 210);
                int intensity = eleDmg[element] * 3 / 2;
                AilmentIntensity[element] = Main.expertMode ? intensity * 2 / 3 : intensity;
                HasAilment[element] = true;
            }

            if (Main.rand.Next(player.statLifeMax2 + player.statDefense) < damage * 3)
                player.AddBuff(ModContent.BuffType<Physical>(), 15 + Math.Min(30, damage * 30 / player.statLifeMax2));
            if (HasAilment[ELEMENT.LIGHTNING])
                damage += 1 + AilmentIntensity[ELEMENT.LIGHTNING];
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            //if (kRPG2.Overhaul != null)
            //    return;
            for (int i = 0; i < layers.Count; i += 1)
                if (layers[i].Name.Contains("Held"))
                    layers.Insert(i + 2, new PlayerLayer("kRPG2", "ProceduralItem", drawinfo =>
                    {
                        if (player.itemAnimation <= 0)
                            return;
                        if (player.HeldItem.type == mod.GetItem("ProceduralStaff").item.type)
                        {
                            if (Main.gameMenu) return;

                            var staff = (ProceduralStaff) player.HeldItem.modItem;

                            var pos = player.Center - Main.screenPosition;
                            staff.DrawHeld(drawinfo, Lighting.GetColor((int) (player.Center.X / 16f), (int) (player.Center.Y / 16f)),
                                player.itemRotation + (float) API.Tau * player.direction / 8, staff.item.scale, pos);
                        }
                        else if (player.HeldItem.type == mod.GetItem("ProceduralSword").item.type)
                        {
                            if (Main.gameMenu) return;

                            var sword = (ProceduralSword) player.HeldItem.modItem;

                            if (sword.Spear) return;

                            var pos = player.Center - Main.screenPosition;
                            sword.DrawHeld(drawinfo, Lighting.GetColor((int) (player.Center.X / 16f), (int) (player.Center.Y / 16f)),
                                player.itemRotation + (float) API.Tau, sword.item.scale, pos);
                        }
                    }));
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            var dict = new Dictionary<ELEMENT, int>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                dict[element] = npc.GetGlobalNPC<kNPC>().ElementalDamage[element];
            ModifyDamageTakenFromNPC(ref damage, ref crit, dict);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            var dict = new Dictionary<ELEMENT, int>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                dict[element] = proj.GetGlobalProjectile<kProjectile>().GetIndividualElements(proj, player)[element];
            ModifyDamageTakenFromNPC(ref damage, ref crit, dict);
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            ModifyDamage(ref damage, ref crit, target, item);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.modProjectile is ProceduralSpellProj)
                ModifyDamage(ref damage, ref crit, target, null, proj);
            else
                ModifyDamage(ref damage, ref crit, target, null, proj);
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            LeechLife(item, damage);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            var item = player.inventory[player.selectedItem];
            LeechLife(item, damage);
            if (item.type == ModContent.ItemType<ProceduralStaff>())
            {
                var staff = (ProceduralStaff) item.modItem;
                bool proceed = false;
                if (proj.type == item.shoot)
                    proceed = true;
                else if (proj.type == ModContent.ProjectileType<ProceduralSpellProj>())
                    proceed = ((ProceduralSpellProj) proj.modProjectile).Source == null;
                if (proceed)
                    staff.Ornament?.OnHit?.Invoke(player, target, item, damage, crit);
            }
            else if (proj.type == ModContent.ProjectileType<ProceduralSpear>() && item.type == ModContent.ItemType<ProceduralSword>())
            {
                var spear = (ProceduralSword) item.modItem;
                spear.Accent?.OnHit?.Invoke(player, target, spear, damage, crit);
            }
        }

        public void OpenInventoryPage(int page)
        {
            for (int i = 0; i < 40; i += 1)
                player.inventory[i + 10] = Inventories[page][i];
            ActiveInvPage = page;
            StatPage = false;
            API.FindRecipes();
            for (int i = 0; i < 50; i += 1)
                if (player.inventory[i].type == 71 || player.inventory[i].type == 72 || player.inventory[i].type == 73 || player.inventory[i].type == 74)
                    player.DoCoins(i);
        }

        public override void PlayerConnect(Player player)
        {
            var packet = mod.GetPacket();
            packet.Write((byte) Message.SyncLevel);
            packet.Write(player.whoAmI);
            packet.Write(Level);
            packet.Send();
        }

        public override void PostItemCheck()
        {
            if (Main.netMode == 1)
                if (player.whoAmI != Main.myPlayer)
                    return;

            try
            {
                var item = player.inventory[player.selectedItem];
                if (item.type != ModContent.ItemType<ProceduralStaff>() || item.shoot > 0)
                    return;
                player.releaseUseItem = true;
                if (player.itemAnimation == 1 && item.stack > 0)
                {
                    if (player.whoAmI != Main.myPlayer && player.controlUseItem)
                    {
                        player.itemAnimation = (int) (item.useAnimation / PlayerHooks.TotalMeleeSpeedMultiplier(player, item));
                        player.itemAnimationMax = player.itemAnimation;
                        player.reuseDelay = (int) (item.reuseDelay / PlayerHooks.TotalUseTimeMultiplier(player, item));
                        if (item.UseSound != null)
                            Main.PlaySound(item.UseSound, player.Center);
                    }
                    else
                    {
                        player.itemAnimation = 0;
                    }
                }

                if (player.itemTime < 2)
                {
                    var pos = player.RotatedRelativePoint(player.MountedCenter);
                    var relativeMousePos = Main.MouseWorld - pos;
                    ItemRotation = Math.Atan2(relativeMousePos.Y * player.direction, relativeMousePos.X * player.direction) - player.fullRotation;
                    NetMessage.SendData(13, -1, -1, null, player.whoAmI);
                    NetMessage.SendData(41, -1, -1, null, player.whoAmI);
                }

                float scaleFactor = 6f;
                if (player.itemAnimation > 0)
                    player.itemRotation = (float) ItemRotation;
                player.itemLocation = player.MountedCenter;
                player.itemLocation += player.itemRotation.ToRotationVector2() * scaleFactor * player.direction;
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
            }
        }

        public override void PostUpdate()
        {
            switch (Main.netMode)
            {
                case 2:
                case 1 when Main.myPlayer != player.whoAmI:
                    return;
            }

            var item = player.inventory[player.selectedItem];
            if (item.damage > 0 && (item.melee || !item.noMelee || item.modItem is ProceduralSword))
                LastSelectedWeapon = item;

            switch (item.modItem)
            {
                case ProceduralSword s:
                {
                    //if (Main.itemTexture[item.type] != s.texture)
                        Main.itemTexture[item.type] = s.texture;
                    break;
                }
                case ProceduralStaff st:
                {
                    //if (Main.itemTexture[ModContent.ItemType<ProceduralStaff>()] != st.texture)
                        Main.itemTexture[ModContent.ItemType<ProceduralStaff>()] = st.texture;
                    break;
                }
            }

            //for (int i = 0; i < 40; i += 1)
            //    inventories[activeInvPage][i] = player.inventory[i + 10];

            //API.FindRecipes();
        }

        public override void PostUpdateEquips()
        {
            if (!Initialized)
            {
                InitializeGui();
                Initialized = true;
            }

            UpdateStats();
            if (LifeRegen > 0 && !player.bleed && !player.onFire && !player.poisoned && !player.onFire2 && !player.venom && !player.onFrostBurn)
                RegenTimer += 1f;
            if (RegenTimer > 60f / LifeRegen)
            {
                player.statLife = Math.Min(player.statLife + (int) (RegenTimer / (60f / LifeRegen)), player.statLifeMax2);
                RegenTimer = RegenTimer % (60f / LifeRegen);
            }

            if (LifeDegen > 0) DegenTimer += 1f;
            if (DegenTimer >= 20f && HasAilment[ELEMENT.FIRE])
            {
                int amount = (int) Math.Round(LifeDegen / 3, 1);
                player.statLife = player.statLife - amount;
                CombatText.NewText(new Rectangle((int) player.position.X, (int) player.position.Y, player.width, player.height), new Color(255, 95, 31),
                    amount);
                DegenTimer = 0;
                if (player.statLife <= 0) player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " burned to death."), amount, 0);
            }

            ManaRegenTimer += 1f;

            if (Main.chatRelease && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest && Main.netMode != 2)
                for (int i = 0; i < Abilities.Length; i += 1)
                    if (Abilities[i].CompleteSkill())
                    {
                        bool useable = true;
                        foreach (var item in Abilities[i].Glyphs)
                        {
                            var glyph = (Glyph) item.modItem;
                            if (!glyph.CanUse()) useable = false;
                        }

                        if (!Main.keyState.IsKeyDown(Abilities[i].Key) || !Main.keyState.IsKeyUp(Keys.LeftShift) || Abilities[i].Remaining != 0 || !useable ||
                            player.statMana < Abilities[i].ManaCost(this))
                            continue;
                        if (Main.netMode != 2)
                            Abilities[i].UseAbility(player, Main.MouseWorld);
                        player.statMana -= Abilities[i].ManaCost(this);
                    }

            for (int i = 0; i < SpellEffects.Count; i += 1)
                SpellEffects[i].Update(this);

            if (Main.mapStyle == 0 && kConfig.ConfigLocal.ClientSide.ArpgMiniMap) Main.mapStyle += 1;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound,
            ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool enemyCrit = Main.rand.Next(5) == 0 && Main.netMode == 0;
            int max = 80;
            int diff = 52;

            if (TotalStats(STAT.QUICKNESS) > 0 && !Rituals[RITUAL.STONE_ASPECT])
            {
                if (damage < (Level + 10) * 5)
                {
                    EvasionCounter += 100 - max + max * diff / (diff + Evasion);
                    if (EvasionCounter >= 100)
                    {
                        EvasionCounter -= 100;
                        if (enemyCrit)
                        {
                            CritCounter += 100 - max + max * diff / (diff + Evasion);
                            if (CritCounter >= 100)
                                CritCounter -= 100;
                            else
                                enemyCrit = false;

                            if (enemyCrit) damage = (int) (damage * 1.65);
                        }
                    }

                    else
                    {
                        player.NinjaDodge(40);
                        return false;
                    }
                }
                else
                {
                    max = 90;
                    diff = 38;
                    BigHitCounter += 100 - max + max * diff / (diff + Evasion);
                    if (BigHitCounter >= 100)
                    {
                        BigHitCounter -= 100;
                        if (enemyCrit)
                        {
                            BigCritCounter += 100 - max + max * diff / (diff + Evasion + TotalStats(STAT.WITS) * 5);
                            if (BigCritCounter >= 100)
                                BigCritCounter -= 100;
                            else
                                enemyCrit = false;

                            if (enemyCrit) damage = (int) (damage * 1.3);
                        }
                    }

                    else
                    {
                        player.NinjaDodge(40 + TotalStats(STAT.WITS) * 5);
                        return false;
                    }
                }
            }

            if (Rituals[RITUAL.MIND_FORTRESS])
            {
                int i = (int) Math.Round(damage * 0.25);
                if (Mana > i)
                {
                    damage -= i;
                    Mana -= i;
                }
                else
                {
                    damage -= Mana;
                    Mana = 0;
                }
            }

            return true;
        }

        public override void PreUpdate()
        {
            if (Main.chatRelease && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest && Main.netMode != 2)
            {
                if (PlayerInput.Triggers.Current.QuickHeal)
                    if (!PlayerInput.Triggers.Old.QuickHeal)
                    {
                        player.ApiQuickHeal();
                        PlayerInput.Triggers.Old.QuickHeal = true;
                    }

                if (PlayerInput.Triggers.Current.QuickMana)
                    if (!PlayerInput.Triggers.Old.QuickMana)
                    {
                        player.ApiQuickMana();
                        PlayerInput.Triggers.Old.QuickMana = true;
                    }

                if (PlayerInput.Triggers.Current.QuickBuff)
                    if (!PlayerInput.Triggers.Old.QuickBuff)
                    {
                        player.ApiQuickBuff();
                        PlayerInput.Triggers.Old.QuickBuff = true;
                    }
            }

            int selectedBinding3 = player.QuicksRadial.SelectedBinding;
            player.QuicksRadial.Update();

            if (player.QuicksRadial.SelectedBinding == -1 || !PlayerInput.Triggers.JustReleased.RadialQuickbar ||
                PlayerInput.MiscSettingsTEMP.HotbarRadialShouldBeUsed)
                return;

            switch (player.QuicksRadial.SelectedBinding)
            {
                case 0:
                    player.ApiQuickHeal();
                    break;
                case 1:
                    player.ApiQuickBuff();
                    break;
                case 2:
                    player.ApiQuickMana();
                    break;
            }

            PlayerInput.Triggers.JustReleased.RadialQuickbar = false;
        }

        public override void PreUpdateBuffs()
        {
        }

        public override void ResetEffects()
        {
            foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                TempStats[stat] = 0;
            Evasion = 2;
            Accuracy = 0;
            BonusLife = 0;
            BonusMana = 0;
            LifeRegen = 1;
            LifeDegen = 0;
            ManaRegen = 0;
            CanHealMana = true;

            CritBoost = 0;
            CritMultiplier = 0f;
            LifeLeech = 0f;
            Allres = 0;

            if (LeechCooldown > 0) LeechCooldown--;

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            {
                Eleres[element] = 0;
                if (!HasAilment[element]) AilmentIntensity[element] = 0;
                HasAilment[element] = false;
            }

            if (Main.netMode != 1 || (int) Main.time % 300 != 0)
                return;
            var packet = mod.GetPacket();
            packet.Write((byte) Message.SyncStats);
            packet.Write(player.whoAmI);
            packet.Write(Level);
            packet.Write(BaseStats[STAT.RESILIENCE]);
            packet.Write(BaseStats[STAT.QUICKNESS]);
            packet.Write(BaseStats[STAT.POTENCY]);
            packet.Write(BaseStats[STAT.WITS]);
            packet.Send();
        }

        public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
        {
            var rand = new Random();
            switch (rand.Next(8))
            {
                default:
                    items[0].SetDefaults(rand.Next(2) == 0 ? ItemID.TinBroadsword : ItemID.CopperBroadsword, true);
                    break;
                case 1:
                    items[0].SetDefaults(ItemID.Spear, true);
                    break;
                case 2:
                    items[0].SetDefaults(ItemID.WoodenBoomerang, true);
                    break;
                case 3:
                    items[0].SetDefaults(rand.Next(2) == 0 ? ItemID.TopazStaff : ItemID.AmethystStaff, true);
                    break;
                case 4:
                    items[0].SetDefaults(rand.Next(2) == 0 ? ItemID.TinBow : ItemID.CopperBow, true);
                    var arrows = new Item();
                    arrows.SetDefaults(rand.Next(2) == 0 ? ItemID.FlamingArrow : ItemID.WoodenArrow, true);
                    arrows.stack = rand.Next(2) == 0 ? 150 : 200;
                    items.Add(arrows);
                    break;
                case 5:
                    items[0].SetDefaults(ItemID.Shuriken, true);
                    items[0].stack = rand.Next(2) == 0 ? 150 : 100;
                    var knives = new Item();
                    knives.SetDefaults(rand.Next(2) == 0 ? ItemID.PoisonedKnife : ItemID.ThrowingKnife, true);
                    knives.stack = 50;
                    items.Add(knives);
                    break;
                case 6:
                    items[0].SetDefaults(ItemID.WoodYoyo, true);
                    break;
                case 7:
                    items[0].SetDefaults(ItemID.ChainKnife, true);
                    break;
            }

            items[1].SetDefaults(rand.Next(3) == 0 ? ItemID.TinPickaxe : rand.Next(2) == 0 ? ItemID.CactusPickaxe : ItemID.CopperPickaxe, true);
            items[1].GetGlobalItem<kItem>().Initialize(items[1]);
            items[2].SetDefaults(rand.Next(2) == 0 ? ItemID.TinAxe : ItemID.CopperAxe);
            items[2].GetGlobalItem<kItem>().Initialize(items[2]);

            var star = new Item();
            star.SetDefaults(ModContent.ItemType<Star_Blue>(), true);
            var cross = new Item();
            switch (rand.Next(4))
            {
                default:
                    cross.SetDefaults(ModContent.ItemType<Cross_Red>(), true);
                    break;
                case 1:
                    cross.SetDefaults(ModContent.ItemType<Cross_Orange>(), true);
                    break;
                case 2:
                    cross.SetDefaults(ModContent.ItemType<Cross_Yellow>(), true);
                    break;
                case 3:
                    cross.SetDefaults(ModContent.ItemType<Cross_Green>(), true);
                    break;
            }

            ((Glyph) cross.modItem).Randomize();
            var moon = new Item();
            switch (rand.Next(5))
            {
                default:
                    moon.SetDefaults(ModContent.ItemType<Moon_Yellow>(), true);
                    break;
                case 1:
                    moon.SetDefaults(ModContent.ItemType<Moon_Green>(), true);
                    break;
                case 2:
                    moon.SetDefaults(ModContent.ItemType<Moon_Blue>(), true);
                    break;
                case 3:
                    moon.SetDefaults(ModContent.ItemType<Moon_Violet>(), true);
                    break;
                case 4:
                    moon.SetDefaults(ModContent.ItemType<Moon_Purple>(), true);
                    break;
            }

            ((Glyph) moon.modItem).Randomize();
            items.Add(star);
            items.Add(cross);
            items.Add(moon);
        }

        public int TotalStats(STAT stat)
        {
            if (Rituals[RITUAL.DEMON_PACT] && stat == STAT.POTENCY)
                return BaseStats[stat] + TempStats[stat] + BaseStats[STAT.RESILIENCE];

            if (Rituals[RITUAL.DEMON_PACT] && stat == STAT.RESILIENCE)
                return TempStats[stat];

            return BaseStats[stat] + TempStats[stat];
        }

        public bool UnspentPoints()
        {
            return PointsAllocated < Level - 1;
        }

        public void UpdateStats()
        {
            float lifeMultiplier = 1f + (player.statLifeMax - 100f) / 400f;
            int addedLife = player.statLifeMax2 - player.statLifeMax;
            player.statLifeMax2 += 115 + TotalStats(STAT.RESILIENCE) * 10 + Level * 5 + BonusLife - player.statLifeMax;
            player.statLifeMax2 = (int) Math.Round(player.statLifeMax2 * lifeMultiplier) + addedLife;
            float manaMultiplier = 1f + (player.statManaMax - 20f) / 200f * 1.5f;
            int addedMana = player.statManaMax2 - player.statManaMax;
            player.statManaMax2 += 19 + Level + BonusMana + TotalStats(STAT.WITS) * 3 - player.statManaMax;
            player.statManaMax2 = (int) Math.Round(player.statManaMax2 * manaMultiplier) + addedMana;
            player.statDefense += TotalStats(STAT.RESILIENCE);
            Allres += TotalStats(STAT.WITS);
            Evasion += TotalStats(STAT.QUICKNESS);
            Accuracy += TotalStats(STAT.WITS);
            if (Rituals[RITUAL.STONE_ASPECT]) player.statDefense += TotalStats(STAT.QUICKNESS);
            LifeRegen += TotalStats(STAT.RESILIENCE) * 0.3f + TotalStats(STAT.WITS) * 0.2f;
            if (HasAilment[ELEMENT.FIRE])
                LifeDegen = AilmentIntensity[ELEMENT.FIRE] / 2;
            ManaRegen = player.statManaMax2 * 0.06f + TotalStats(STAT.WITS) * 0.6f;

            if (Main.netMode != 2 && Main.myPlayer == player.whoAmI)
            {
                if (Mana < 0) Mana = 0;
                if (player.statMana < 0) player.statMana = 0;
                if (player.statMana < Mana)
                    Mana = player.statMana;
                if (Rituals[RITUAL.ELAN_VITAL] && Mana < player.statManaMax2)
                {
                    if (player.statLife > player.statLifeMax2 * 0.4 + player.statManaMax2 - Mana)
                    {
                        player.statLife -= player.statManaMax2 - Mana;
                        Mana = player.statManaMax2;
                    }
                    else if (player.statLife > player.statLifeMax2 * 0.4)
                    {
                        Mana += (int) (player.statLife - player.statLifeMax2 * 0.4);
                        player.statLife = (int) (player.statLifeMax2 * 0.4);
                    }
                }

                if (player.statMana == player.statManaMax2 && Mana == player.statMana - 1)
                    Mana = player.statMana;
                else player.statMana = Mana;
                if (ManaRegenTimer > 60f / ManaRegen)
                {
                    Mana = Math.Min(Mana + (int) (ManaRegenTimer / (60f / ManaRegen)), player.statManaMax2);
                    ManaRegenTimer = ManaRegenTimer % (60f / ManaRegen);
                }

                player.statMana = Mana;
            }

            CritMultiplier += TotalStats(STAT.POTENCY) * 0.04f;
            LifeLeech += TotalStats(STAT.POTENCY) * 0.002f;
            LifeLeech += Math.Min(0.006f, TotalStats(STAT.POTENCY) * 0.002f);

            player.meleeDamage *= DamageMultiplierPercent;
            player.rangedDamage *= DamageMultiplierPercent;
            player.magicDamage *= DamageMultiplierPercent;
            player.minionDamage *= DamageMultiplierPercent;
            player.thrownDamage *= DamageMultiplierPercent;

            player.moveSpeed *= 1f + Math.Min(1.2f, TotalStats(STAT.QUICKNESS) * 0.02f + Math.Min(Level * 0.005f, 0.5f));
            player.meleeSpeed *= 1f + TotalStats(STAT.QUICKNESS) * 0.01f;
            player.jumpSpeedBoost += Math.Min(5f, TotalStats(STAT.QUICKNESS) * 0.2f + Math.Min(Level * 0.05f, 2f));

            CritBoost += Math.Min(TotalStats(STAT.QUICKNESS), Math.Max(4, TotalStats(STAT.QUICKNESS) / 2 + 2));
            player.magicCrit += CritBoost;
            player.meleeCrit += CritBoost;
            player.rangedCrit += CritBoost;
            player.thrownCrit += CritBoost;
        }

        #region Saving and loading

        public override void Initialize()
        {
            BaseStats = new Dictionary<STAT, int>();
            TempStats = new Dictionary<STAT, int>();

            Permanence = 0;
            Transcendence = 0;

            Level = 1;
            Experience = 0;

            foreach (STAT stat in Enum.GetValues(typeof(STAT)))
            {
                BaseStats[stat] = 0;
                TempStats[stat] = 0;
            }

            Inventories = new Item[3][];
            for (int i = 0; i < Inventories.Length; i += 1)
            {
                Inventories[i] = new Item[40];
                for (int j = 0; j < Inventories[i].Length; j += 1)
                    Inventories[i][j] = new Item();
            }

            Rituals = new Dictionary<RITUAL, bool>();
            foreach (RITUAL rite in Enum.GetValues(typeof(RITUAL)))
                Rituals[rite] = false;

            for (int i = 0; i < Abilities.Length; i += 1)
            {
                Abilities[i] = new ProceduralSpell(mod);
                for (int j = 0; j < Abilities[i].Glyphs.Length; j += 1)
                    Abilities[i].Glyphs[j] = new Item();
            }

            Abilities[0].Key = Keys.Z;
            Abilities[1].Key = Keys.X;
            Abilities[2].Key = Keys.C;
            Abilities[3].Key = Keys.V;
        }

        public void InitializeGui()
        {
            if (Main.netMode == 2) return;
            BaseGui.GuiElements.Clear();
            AnvilGui = new AnvilGUI(this);
            LevelGui = new LevelGui(this, mod);
            StatusBar = new StatusBar(this, mod) {GuiActive = true};
            InventoryGui = new InventoryGui(this);
            AbilitiesGui = new AbilitiesGui {GuiActive = true};
            SpellCraftingGui = new SpellcraftingGUI(mod /*, glyphs, this*/);
        }

        public void CloseGuIs()
        {
            AnvilGui.CloseGui();
            LevelGui.CloseGui();
            SpellCraftingGui.CloseGui();
        }

        public override void OnEnterWorld(Player player)
        {
            InitializeGui();

            if (player.whoAmI == Main.myPlayer)
                kRPG2.CheckForUpdates();
        }

        public override TagCompound Save()
        {
            var tagCompound = new TagCompound
            {
                {"level", Level},
                {"Experience", Experience},
                {"baseRESILIENCE", BaseStats[STAT.RESILIENCE]},
                {"baseQUICKNESS", BaseStats[STAT.QUICKNESS]},
                {"basePOTENCY", BaseStats[STAT.POTENCY]},
                {"baseWITS", BaseStats[STAT.WITS]},
                {"RITUAL_DEMON_PACT", Rituals[RITUAL.DEMON_PACT]},
                {"RITUAL_WARRIOR_OATH", Rituals[RITUAL.WARRIOR_OATH]},
                {"RITUAL_ELAN_VITAL", Rituals[RITUAL.ELAN_VITAL]},
                {"RITUAL_STONE_ASPECT", Rituals[RITUAL.STONE_ASPECT]},
                {"RITUAL_ELDRITCH_FURY", Rituals[RITUAL.ELDRITCH_FURY]},
                {"RITUAL_MIND_FORTRESS", Rituals[RITUAL.MIND_FORTRESS]},
                {"RITUAL_BLOOD_DRINKING", Rituals[RITUAL.BLOOD_DRINKING]},
                {"life", player.statLife},
                {"permanence", Permanence},
                {"transcendence", Transcendence}
            };

            try
            {
                for (int i = 0; i < Abilities.Length; i += 1)
                {
                    if (Abilities[i] == null) return tagCompound;
                    tagCompound.Add("abilities" + i + "_key", Abilities[i].Key.ToString());
                    for (int j = 0; j < Abilities[i].Glyphs.Length; j += 1)
                        if (Abilities[i].Glyphs[j] != null)
                            tagCompound.Add("ability" + i + j, ItemIO.Save(Abilities[i].Glyphs[j]));
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Abilities :: " + e);
            }

            try
            {
                for (int i = 0; i < Inventories.Length; i += 1)
                for (int j = 0; j < Inventories[i].Length; j += 1)
                    tagCompound.Add("item" + i + j, ItemIO.Save(Inventories[i][j]));
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Inventories :: " + e);
            }

            return tagCompound;
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                Level = tag.GetInt("level");
                Experience = tag.GetInt("Experience");
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Level&XP :: " + e);
            }

            try
            {
                foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                    BaseStats[stat] = tag.GetInt("base" + stat);
                foreach (RITUAL rite in Enum.GetValues(typeof(RITUAL)))
                    Rituals[rite] = tag.GetBool("RITUAL_" + rite);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Stats&Rituals :: " + e);
            }

            try
            {
                Abilities = new ProceduralSpell[4];
                for (int i = 0; i < Abilities.Length; i += 1)
                {
                    Abilities[i] = new ProceduralSpell(mod);
                    Abilities[i].Key = (Keys) Enum.Parse(typeof(Keys), tag.GetString("abilities" + i + "_key"));
                    for (int j = 0; j < Abilities[i].Glyphs.Length; j += 1)
                        if (tag.ContainsKey("ability" + i + j))
                            Abilities[i].Glyphs[j] = ItemIO.Load(tag.GetCompound("ability" + i + j));
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Abilities :: " + e);
            }

            try
            {
                for (int i = 0; i < Inventories.Length; i += 1)
                for (int j = 0; j < Inventories[i].Length; j += 1)
                    Inventories[i][j] = ItemIO.Load(tag.GetCompound("item" + i + j));
                OpenInventoryPage(0);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Inventory :: " + e);
            }

            try
            {
                player.statLife = tag.GetInt("life");
                Permanence = tag.GetInt("permanence");
                Transcendence = tag.GetInt("transcendence");

                Mana = 10 + (Level - 1) * 3;
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Miscellaneous :: " + e);
            }
        }

        #endregion
    }
}