using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

using kRPG.Items.Weapons;
using kRPG.Items;
using kRPG.Buffs;
using kRPG.Dusts;
using kRPG.GUI;
using kRPG.Items.Glyphs;
using System.IO;

namespace kRPG.Projectiles
{
    public class ProceduralProjectile : ModProjectile
    {
        public Texture2D texture;

        public virtual void Initialize() { }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale) { }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.scale = 1f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Projectile; Please Ignore");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.owner != Main.myPlayer && !(this is ProceduralSpellProj) && !(this is WingedEyeball)) return true;
            Draw(spriteBatch, projectile.position - Main.screenPosition, lightColor, projectile.rotation, projectile.scale);
            return false;
        }
    }

    public class ProceduralSpellProj : ProceduralProjectile
    {
        public Vector2 RelativePos(Vector2 referencePoint)
        {
            return projectile.Center - referencePoint;
        }

        public bool lighted = false;
        public bool homing = false;
        public bool bounce = false;

        public float displacementAngle = 0f;
        public Vector2 displacementVelocity = Vector2.Zero;
        public Vector2 baseVelocity = Vector2.Zero;
        public Vector2 basePosition = Vector2.Zero;
        public float alpha = 0f;
        public bool draw_trail = false;
        private Queue<Vector2> oldPositions = new Queue<Vector2>();
        private Queue<float> oldRotations = new Queue<float>();
        public Entity caster;
        public Item selectedItem;
        public Vector2 origin = Vector2.Zero;
        public ProceduralSpell source;
        public bool minion
        {
            get { return caster is Projectile; }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            if (source == null) return;
            writer.Write(projectile.owner);
            writer.Write(source.glyphs[(byte)GLYPHTYPE.STAR].type);
            writer.Write(source.glyphs[(byte)GLYPHTYPE.CROSS].type);
            writer.Write(source.glyphs[(byte)GLYPHTYPE.MOON].type);
            writer.Write(projectile.damage);
            writer.Write(minion);
            writer.Write(caster.whoAmI);
            List<GlyphModifier> modifiers = source.modifiers;
            writer.Write(modifiers.Count);
            for (int j = 0; j < modifiers.Count; j += 1)
                writer.Write(modifiers[j].id);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.owner = reader.ReadInt32();
            int startype = reader.ReadInt32();
            int crosstype = reader.ReadInt32();
            int moontype = reader.ReadInt32();
            projectile.damage = reader.ReadInt32();
            bool minion_caster = reader.ReadBoolean();
            caster = minion_caster ? (Entity)Main.projectile[reader.ReadInt32()] : (Entity)Main.player[reader.ReadInt32()];
            List<GlyphModifier> modifiers = new List<GlyphModifier>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i += 1)
                modifiers.Add(GlyphModifier.modifiers[reader.ReadInt32()]);
            if (source == null)
            {
                source = new ProceduralSpell(mod);
                source.glyphs[(byte)GLYPHTYPE.STAR].SetDefaults(startype,true);
                source.glyphs[(byte)GLYPHTYPE.CROSS].SetDefaults(crosstype,true);
                source.glyphs[(byte)GLYPHTYPE.MOON].SetDefaults(moontype,true);
                source.modifierOverride = modifiers;
            }
            foreach (Item item in source.glyphs)
            {
                Glyph glyph = (Glyph)item.modItem;
                if (glyph.GetAIAction() != null)
                    ai.Add(glyph.GetAIAction());
                if (glyph.GetInitAction() != null)
                    init.Add(glyph.GetInitAction());
                if (glyph.GetImpactAction() != null)
                    impact.Add(glyph.GetImpactAction());
                if (glyph.GetKillAction() != null)
                    kill.Add(glyph.GetKillAction());
            }
            foreach (GlyphModifier modifier in modifiers)
            {
                if (modifier.impact != null)
                    impact.Add(modifier.impact);
                if (modifier.draw != null)
                    draw.Add(modifier.draw);
                if (modifier.init != null)
                    init.Add(modifier.init);
            }
            Initialize();
        }

        public static Action<ProceduralSpellProj> AI_RotateToVelocity = delegate(ProceduralSpellProj spell)
        {
            spell.projectile.rotation = (float)Math.Atan2(spell.projectile.velocity.Y, spell.projectile.velocity.X) + (float)API.Tau / 8f;
        };
        public static Action<ProceduralSpellProj> AI_Whirlcast = delegate (ProceduralSpellProj spell)
        {
            Player owner = Main.player[spell.projectile.owner];
        };

        public List<Action<ProceduralSpellProj>> init = new List<Action<ProceduralSpellProj>>();
        public override void Initialize()
        {
            foreach (Action<ProceduralSpellProj> action in init) if (action != null) action(this);
            if (minion)
            {
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
            }
        }

        public List<Action<ProceduralSpellProj, SpriteBatch, Color>> draw = new List<Action<ProceduralSpellProj, SpriteBatch, Color>>();
        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            foreach (Action<ProceduralSpellProj, SpriteBatch, Color> action in draw) if (action != null) action(this, spriteBatch, color);
            if (texture == null)
            {
                Initialize();
                return;
            }
            if (draw_trail)
            {
                for (int i = 0; i < oldPositions.Count; i += 1)
                    spriteBatch.Draw(texture, oldPositions.ElementAt(i) - Main.screenPosition + texture.Bounds.Center(), null, (lighted ? Color.White : color) * alpha * (0.04f + 0.09f * i), oldRotations.ElementAt(i), texture.Bounds.Center(), scale, projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                
                oldPositions.Enqueue(position + Main.screenPosition);
                oldRotations.Enqueue(rotation);
                if (oldPositions.Count > 5)
                {
                    oldPositions.Dequeue();
                    oldRotations.Dequeue();
                }
            }
            spriteBatch.Draw(texture, position + texture.Bounds.Center(), null, (lighted ? Color.White : color)*alpha, rotation, texture.Bounds.Center(), scale, projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 210;
            projectile.tileCollide = false;
            projectile.knockBack = 4f;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public List<Action<ProceduralSpellProj>> kill = new List<Action<ProceduralSpellProj>>();
        public override void Kill(int timeLeft)
        {
            foreach (Action<ProceduralSpellProj> action in kill) if (action != null) action(this);
            if (timeLeft > 0)
                Main.PlaySound(SoundID.Dig, projectile.position);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Spell-Projectile; Please Ignore");
        }

        public List<Action<ProceduralSpellProj, NPC, int>> impact = new List<Action<ProceduralSpellProj, NPC, int>>();
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            foreach (Action<ProceduralSpellProj, NPC, int> action in impact) if (action != null) action(this, target, damage);
        }

        public List<Action<ProceduralSpellProj>> ai = new List<Action<ProceduralSpellProj>>(); // <=== Look here:
        public override void AI()
        {
            if (alpha < 1f) alpha += 0.02f;
            if (caster == null) projectile.Kill();
            else if (!caster.active) projectile.Kill();
            foreach (Action<ProceduralSpellProj> action in ai) if (action != null) action(this);
        }

        public override bool OnTileCollide(Vector2 velocity)
        {
            if (bounce)
            {
                Main.PlaySound(SoundID.Item10, projectile.position);

                if (projectile.velocity.Y != velocity.Y)
                    projectile.velocity.Y = -velocity.Y;

                if (projectile.velocity.X != velocity.X)
                    projectile.velocity.X = -velocity.X;

                return false;
            }
            return true;
        }

        public override ModProjectile Clone()
        {
            ProceduralSpellProj copy = (ProceduralSpellProj)MemberwiseClone();
            copy.ai = new List<Action<ProceduralSpellProj>>();
            foreach (Action<ProceduralSpellProj> action in ai) copy.ai.Add(action);
            copy.impact = new List<Action<ProceduralSpellProj, NPC, int>>();
            foreach (Action<ProceduralSpellProj, NPC, int> action in impact) copy.impact.Add(action);
            copy.init = new List<Action<ProceduralSpellProj>>();
            foreach (Action<ProceduralSpellProj> action in init) copy.init.Add(action);
            copy.kill = new List<Action<ProceduralSpellProj>>();
            foreach (Action<ProceduralSpellProj> action in kill) copy.kill.Add(action);
            return copy;
        }
    }

    public class ProceduralMinion : ProceduralProjectile
    {
        public ProceduralSpell source;
        public List<ProceduralSpellProj> circlingProtection = new List<ProceduralSpellProj>();
        public ProceduralSpellProj smallProt = null;
        public List<Action<ProceduralMinion>> glyphModifiers = new List<Action<ProceduralMinion>>();

        public int cooldown = 0;
        public bool attack = false;
        protected NPC target;
        protected float distance;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Minion; Please Ignore");
        }
        
        public override void Kill(int timeLeft)
        {
            foreach (ProceduralSpellProj spell in circlingProtection)
                spell.projectile.Kill();
            circlingProtection.Clear();
            if (smallProt != null) smallProt.projectile.Kill();
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void PostAI()
        {
            foreach (Action<ProceduralMinion> modifier in glyphModifiers)
                modifier(this);
        }

        public NPC GetTarget()
        {
            attack = false;
            target = Main.npc.First<NPC>();
            Player player = Main.player[projectile.owner];
            distance = Vector2.Distance(projectile.Center, target.Center);
            foreach (NPC npc in Main.npc)
            {
                float f = Vector2.Distance(projectile.Center, npc.Center);
                if (f < distance && npc.active && npc.life > 0 && !npc.friendly && npc.damage > 0)
                {
                    target = npc;
                    distance = f;
                    attack = true;
                }
            }
            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC];
                attack = true;
            }

            return target;
        }

        public override void AI()
        {
            if (Main.netMode == 2) return;
            bool self = source.glyphs[(byte)GLYPHTYPE.MOON].modItem is Moon_Green;
            if ((!self || circlingProtection.Where(spell => spell.projectile.active).Count() <= source.projCount - 3) && cooldown <= 0)
            {
                if (!self)
                {
                    GetTarget();
                    if (distance <= 480f && attack)
                        if (this is ProceduralMinion)
                            source.CastSpell(Main.player[projectile.owner], projectile.Center, target.Center, projectile);
                }
                else if (this is ProceduralMinion) source.CastSpell(Main.player[projectile.owner], projectile.Center, projectile.Center, projectile);

                cooldown = source.cooldown * 2;
            }
            else cooldown -= 1;
        }
    }

    public class Obelisk : ProceduralMinion
    {
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1800;
            projectile.tileCollide = false;
            projectile.knockBack = 0f;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            Texture2D t = Main.projectileTexture[mod.ProjectileType<Obelisk>()];
            spriteBatch.Draw(t, position + t.Bounds.Center(), null, Color.White, rotation, t.Bounds.Center(), scale, SpriteEffects.None, 0f);
        }
    }

    public class WingedEyeball : ProceduralMinion
    {
        public Vector2 acceleration = new Vector2(1f, 0.5f);
        public Vector2 maxSpeed = new Vector2(6f, 2f);

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 7200;
            projectile.tileCollide = false;
            projectile.knockBack = 0f;
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[projectile.owner];

            float acceleration = 0.4f;
            projectile.tileCollide = false;
            Vector2 v = player.Center - projectile.Center;
            v.X += Main.rand.Next(-10, 21);
            v.X += Main.rand.Next(-10, 21);
            v.X += 60f * -player.direction;
            v.Y -= 60f;
            float someDist = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            float num22 = 14f;

            if (someDist < 100 && player.velocity.Y == 0f && projectile.Bottom.Y <= player.Bottom.Y && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }
            if (someDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;
                }
                acceleration = 0.01f;
            }
            else
            {
                if (someDist < 100f)
                {
                    acceleration = 0.1f;
                }
                if (someDist > 300f)
                {
                    acceleration = 0.6f;
                }
                someDist = num22 / someDist;
                v.X *= someDist;
                v.Y *= someDist;
            }
            if (projectile.velocity.X < v.X)
            {
                projectile.velocity.X = projectile.velocity.X + acceleration;
                if (acceleration > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                }
            }
            if (projectile.velocity.X > v.X)
            {
                projectile.velocity.X = projectile.velocity.X - acceleration;
                if (acceleration > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                }
            }
            if (projectile.velocity.Y < v.Y)
            {
                projectile.velocity.Y = projectile.velocity.Y + acceleration;
                if (acceleration > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration * 2f;
                }
            }
            if (projectile.velocity.Y > v.Y)
            {
                projectile.velocity.Y = projectile.velocity.Y - acceleration;
                if (acceleration > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration * 2f;
                }
            }
            if (projectile.velocity.X > 0.25)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.25)
            {
                projectile.direction = 1;
            }
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.05f;
            int num9 = projectile.frameCounter;
            projectile.frameCounter = num9 + 1;
            if (projectile.frameCounter > 2)
            {
                num9 = projectile.frame;
                projectile.frame = num9 + 1;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            Texture2D t = Main.projectileTexture[mod.ProjectileType<WingedEyeball>()];
            spriteBatch.Draw(t, position + t.Bounds.Center(), new Rectangle(0, projectile.frame * 40, 90, 40), color, rotation, t.Bounds.Center(), scale, projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }

    //public class ProceduralSwordThrow : ProceduralProjectile
    //{
    //    public SwordHilt hilt;
    //    public SwordBlade blade;
    //    public SwordAccent accent;
    //    public Item sword;

    //    public override void Initialize()
    //    {
    //        this.texture = GFX.CombineTextures(new List<Texture2D>(){
    //            { blade.texture },
    //            { hilt.texture },
    //            { accent.texture }
    //        }, new List<Point>(){
    //            { new Point(CombinedTextureSize().X - blade.texture.Width, 0) },
    //            { new Point(0, CombinedTextureSize().Y - hilt.texture.Height) },
    //            { new Point((int)hilt.origin.X - (int)accent.origin.X, CombinedTextureSize().Y - hilt.texture.Height + (int)hilt.origin.Y - (int)accent.origin.Y) }
    //        }, CombinedTextureSize());
    //        projectile.width = texture.Width;
    //        projectile.height = texture.Height;
    //    }

    //    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    //    {
    //        hitbox = new Rectangle((int)projectile.position.X - texture.Width / 2, (int)projectile.position.Y - texture.Height / 2, texture.Width, texture.Height);
    //    }

    //    public override bool? CanHitNPC(NPC target)
    //    {
    //        Player owner = Main.player[projectile.owner];
    //        if ((target.position.X - owner.position.X) * owner.direction > -1f)
    //            return base.CanHitNPC(target);
    //        else return false;
    //    }

    //    public Point CombinedTextureSize()
    //    {
    //        return new Point(blade.texture.Width - (int)blade.origin.X + (int)hilt.origin.X, (int)blade.origin.Y + hilt.texture.Height - (int)hilt.origin.Y);
    //    }

    //    public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
    //    {
    //        if (texture == null)
    //        {
    //            Initialize();
    //            return;
    //        }
    //        spriteBatch.Draw(texture, position + texture.Size() / 2f, null, blade.lighted ? Color.White : color, rotation, texture.Bounds.Center(), scale, projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
    //    }

    //    public override void SetDefaults()
    //    {
    //        projectile.width = 40;
    //        projectile.height = 40;
    //        projectile.scale = 1f;
    //        projectile.friendly = true;
    //        projectile.hostile = false;
    //        projectile.penetrate = -1;
    //        projectile.timeLeft = 3600;
    //        projectile.tileCollide = false;
    //        projectile.aiStyle = 3;
    //    }

    //    public override void SetStaticDefaults()
    //    {
    //        DisplayName.SetDefault("Procedurally Generated Sword Projectile; Please Ignore");
    //    }

    //    public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
    //    {
    //        Player owner = Main.player[projectile.owner];
    //        //if (accent.onHit != null) accent.onHit(owner, target, (ProceduralSword)owner.inventory[owner.selectedItem].modItem, damage, crit);
    //    }
    //}

    public class ProceduralSpear : ProceduralProjectile
    {
        public SwordBlade blade;
        public SwordHilt hilt;
        public SwordAccent accent;

        public override void Initialize()
        {
            this.texture = GFX.CombineTextures(new List<Texture2D>(){
                { blade.texture },
                { hilt.spearTexture },
                { accent.texture }
            }, new List<Point>(){
                { new Point(CombinedTextureSize().X - blade.texture.Width, 0) },
                { new Point(0, CombinedTextureSize().Y - hilt.spearTexture.Height) },
                { new Point((int)hilt.spearOrigin.X - (int)accent.origin.X, CombinedTextureSize().Y - hilt.spearTexture.Height + (int)hilt.spearOrigin.Y - (int)accent.origin.Y) }
            }, CombinedTextureSize());
            projectile.width = texture.Width;
            projectile.height = texture.Height;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Player owner = Main.player[projectile.owner];
            hitbox = new Rectangle((int)projectile.position.X-2, (int)projectile.position.Y-2, (int)(projectile.Right.X - projectile.Left.X)+2, (int)(projectile.Bottom.Y - projectile.Top.Y+2));
            if (owner.direction < 0) hitbox.X += hitbox.Width / 2;
            else hitbox.X -= hitbox.Width / 2;
        }

        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[projectile.owner];
            if ((target.position.X - owner.position.X) * owner.direction > -1f)
                return base.CanHitNPC(target);
            else return false;
        }

        public Point CombinedTextureSize()
        {
            return new Point(blade.texture.Width - (int)blade.origin.X + (int)hilt.spearOrigin.X, (int)blade.origin.Y + hilt.spearTexture.Height - (int)hilt.spearOrigin.Y);
        }

        //public override void SendExtraAI(BinaryWriter writer)
        //{
        //    writer.Write(blade.type);
        //    writer.Write(hilt.type);
        //    writer.Write(accent.type);
        //}

        //public override void ReceiveExtraAI(BinaryReader reader)
        //{
        //    blade = SwordBlade.blades[reader.ReadInt32()];
        //    hilt = SwordHilt.hilts[reader.ReadInt32()];
        //    accent = SwordAccent.accents[reader.ReadInt32()];
        //    if (Main.netMode == 1) Initialize();
        //}

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (texture == null && Main.netMode != 2)
            {
                Initialize();
                return;
            }
            spriteBatch.Draw(texture, position + texture.Size() / 2f, null, blade.lighted ? Color.White : color, rotation, projectile.spriteDirection > 0 ? texture.Bounds.TopRight() : Vector2.Zero, scale, projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        public override void SetDefaults()
        {
            try
            { 
                projectile.width = 40;
                projectile.height = 40;
                projectile.scale = 1f;
                projectile.friendly = true;
                projectile.hostile = false;
                projectile.melee = true;
                projectile.penetrate = -1;
                projectile.timeLeft = 600;
                projectile.tileCollide = false;
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ErrorLogger.Log(e.ToString());
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Spear; Please Ignore");
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            try
            {
                Player owner = Main.player[projectile.owner];
                if (accent.onHit != null) accent.onHit(owner, target, (ProceduralSword)owner.inventory[owner.selectedItem].modItem, damage, crit);
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ErrorLogger.Log(e.ToString());
            }
        }

        public float movementFactor // Change this value to alter how fast the spear moves
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }
        // It appears that for this AI, only the ai0 field is used!

        public override void AI()
        {
            try
            {
                // Since we access the owner player instance so much, it's useful to create a helper local variable for this
                // Sadly, Projectile/ModProjectile does not have its own
                Player projOwner = Main.player[projectile.owner];
                // Here we set some of the projectile's owner properties, such as held item and itemtime, along with projectile directio and position based on the player
                //Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

                if (projectile.velocity.X > 0)
                    projOwner.direction = 1;
                else
                    projOwner.direction = -1;

                projectile.direction = projOwner.direction;
                projectile.spriteDirection = projectile.direction;
                projOwner.heldProj = projectile.whoAmI;
                projOwner.itemTime = projOwner.itemAnimation;
                projectile.position.X = projOwner.Center.X - (float)(texture.Width / 2)/* + 2f*projOwner.direction*/;
                projectile.position.Y = projOwner.Center.Y - (float)(texture.Height / 2)/* + 4f*/;
                // As long as the player isn't frozen, the spear can move
                if (!projOwner.frozen)
                {
                    if (movementFactor == 0f) // When intially thrown out, the ai0 will be 0f
                    {
                        movementFactor = 3f;
                        projectile.netUpdate = true;
                    }
                    if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
                    {
                        movementFactor -= 2.4f;
                    }
                    else
                    {
                        movementFactor += 2.1f;
                    }
                }

                projectile.position += projectile.velocity * movementFactor;
                Vector2 unitVelocity = projectile.velocity;
                unitVelocity.Normalize();
                projectile.position += unitVelocity * (blade.origin.Y * 2.8f + 8f);

                if (projOwner.itemAnimation == 1)
                {
                    projectile.Kill();
                }
                // Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
                // MathHelper.ToRadians(xx degrees here)
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + MathHelper.ToRadians(45f);
                // Offset by 90 degrees here
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation += MathHelper.ToRadians(90f);
                }

                Rectangle rect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, texture.Width, texture.Height);
                if (blade.effect != null) blade.effect(rect, projOwner);
                if (accent.effect != null) accent.effect(rect, projOwner);
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
            }
        }
    }
}
