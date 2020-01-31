using System;
using System.Collections.Generic;
using kRPG2.Enums;
using kRPG2.Items.Dusts;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG2.Items.Weapons
{
    public class SwordAccent
    {
        public static Dictionary<int, SwordAccent> Accents { get; set; } = new Dictionary<int, SwordAccent>();
        public static SwordAccent Flame { get; set; }
        public static SwordAccent GemBlue { get; set; }
        public static SwordAccent GemGreen { get; set; }
        public static SwordAccent GemPurple { get; set; }
        public static SwordAccent GemRed { get; set; }
        public static SwordAccent None { get; set; }
        public int CritBonus { get; set; }
        public float DpsModifier { get; set; }
        public Action<Rectangle, Player> Effect { get; set; }

        public Dictionary<ELEMENT, float> EleDamage { get; set; } = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public int Mana { get; set; }

        public Action<Player, NPC, ProceduralSword, int, bool> OnHit { get; set; }
        public Vector2 Origin { get; set; }
        public string Suffix { get; set; } 
        public Texture2D Texture { get; set; }

        public int Type { get; set; }

        public SwordAccent(string texture, string suffix, int originX, int originY, int mana = 0,
            Action<Player, NPC, ProceduralSword, int, bool> onHit = null, float dpsModifier = 1f, int critBonus = 0, Action<Rectangle, Player> effect = null)
        {
            Type = Accents.Count;
            if (Main.netMode != 2)
                if (texture != null)
                    this.Texture = ModLoader.GetMod("kRPG2").GetTexture("GFX/Items/Accents/" + texture);
            this.Suffix = suffix;
            Origin = new Vector2(originX, originY);
            this.OnHit = onHit;
            this.DpsModifier = dpsModifier;
            this.CritBonus = critBonus;
            this.Effect = effect;
            this.Mana = mana;

            if (!Accents.ContainsKey(Type))
                Accents.Add(Type, this);
        }

        public static void Initialize()
        {
            Accents = new Dictionary<int, SwordAccent>();

            None = new SwordAccent(null, "", 0, 0);
            GemRed = new SwordAccent("GemRed", " of Rejuvenation", 2, 2, 2, delegate(Player player, NPC npc, ProceduralSword sword, int damage, bool crit)
            {
                if (Main.rand.Next(4) != 0)
                    return;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                float distance = Vector2.Distance(npc.Center, character.player.Center);
                int count = (int) (distance / 20);
                Trail trail = new Trail(npc.Center, 60, delegate(SpriteBatch spriteBatch, Player p, Vector2 end, Vector2[] displacement, float scale)
                {
                    for (int i = 0; i < count; i += 1)
                    {
                        Vector2 position = (npc.position - p.Center) * i / count + p.Center;
                        spriteBatch.Draw(GFX.Heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale,
                            SpriteEffects.None, 0f);
                    }
                }) {Displacement = new Vector2[count]};
                for (int i = 0; i < count; i += 1)
                    trail.Displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                character.Trails.Add(trail);
                int healAmount = Main.rand.Next(5) + 3;
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            });
            Flame = new SwordAccent("Flame", " of Ignition", 2, 2, 3, delegate(Player player, NPC npc, ProceduralSword sword, int damage, bool crit)
                {
                    Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), player.Center);
                    Projectile proj = Main.projectile[
                        Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<Explosion>(),
                            Math.Max(1, (int) Math.Round(sword.EleDamage[ELEMENT.FIRE] * damage * 2)), 0f, player.whoAmI)];
                }, 1.05f).SetEleDamage(new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.2f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}})
                .SetEffect(delegate(Rectangle rect, Player player)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f,
                        player.velocity.Y * 0.2f, 63, new Color(), 1.5f);
                    Main.dust[dust].noGravity = true;
                });
            GemGreen = new SwordAccent("GemGreen", " of Thunder", 2, 2, 2, delegate(Player player, NPC npc, ProceduralSword sword, int damage, bool crit)
                {
                    Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), player.Center);
                    Projectile proj = Main.projectile[
                        Projectile.NewProjectile(npc.Center - new Vector2(24, 48), Vector2.Zero, ModContent.ProjectileType<SmokePellets>(),
                            Math.Max(1, damage / 6),
                            0f, player.whoAmI)];
                }, 1.1f, 4).SetEleDamage(new Dictionary<ELEMENT, float>
                    {
                        {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0.2f}, {ELEMENT.SHADOW, 0f}
                    })
                .SetEffect(delegate(Rectangle rect, Player player)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Electric,
                        player.velocity.X * 0.2f + player.direction * 3f,
                        player.velocity.Y * 0.2f, 63, new Color(), 0.5f);
                    Main.dust[dust].noGravity = true;
                });
            GemBlue = new SwordAccent("GemBlue", " of Everest", 2, 2, 0, null, 1.15f, 0, delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(3) == 0)
                {
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.GetInstance<Ice>().Type,
                        player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, new Color(), 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(rect.Center(), 0f, 0.4f, 1f);
            }).SetEleDamage(new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0.3f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}});
            GemPurple = new SwordAccent("GemPurple", " of Starlight", 2, 1, 3, null, 1.1f).SetEffect(delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(2) != 0)
                    return;
                int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 27, player.velocity.X * 0.2f + player.direction * 3f,
                    player.velocity.Y * 0.2f, 63, new Color(), 1.5f);
                Main.dust[dust].noGravity = true;
            });
        }

        public static SwordAccent RandomAccent()
        {
            return Accents.Random();
        }

        public SwordAccent SetEffect(Action<Rectangle, Player> effect)
        {
            this.Effect = effect;
            return this;
        }

        public SwordAccent SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            this.EleDamage = eleDamage;
            return this;
        }

        public static void Unload()
        {
            foreach (SwordAccent accent in Accents.Values)
                accent.Texture = null;
        }
    }
}