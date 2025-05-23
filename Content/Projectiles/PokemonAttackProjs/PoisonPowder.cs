﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Pokemod.Common.Players;
using Pokemod.Content.NPCs;
using Pokemod.Content.Pets;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemod.Content.Projectiles.PokemonAttackProjs
{
	public class PoisonPowder : PokemonAttack
	{
		private Vector2 targetPosition;
		private bool canfollow = true;
		public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
            base.ReceiveExtraAI(reader);
        }
		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }
		public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.timeLeft = 120;

            Projectile.tileCollide = false;  
            Projectile.penetrate = 3;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 15;
			base.SetDefaults();
        }

		public override void Attack(Projectile pokemon, float distanceFromTarget, Vector2 targetCenter){
			var pokemonOwner = (PokemonPetProjectile)pokemon.ModProjectile;
			
			if(pokemon.owner == Main.myPlayer){
				for(int i = 0; i < pokemonOwner.nAttackProjs; i++){
					if(pokemonOwner.attackProjs[i] == null){
						pokemonOwner.attackProjs[i] = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(pokemon), pokemon.Center, 8*new Vector2(0,-1).RotatedByRandom(MathHelper.ToRadians(20)), ModContent.ProjectileType<PoisonPowder>(), pokemonOwner.GetPokemonAttackDamage(GetType().Name), 2f, pokemon.owner)];
						pokemonOwner.currentStatus = (int)PokemonPetProjectile.ProjStatus.Attack;
						SoundEngine.PlaySound(SoundID.Item17, pokemon.position);
						pokemonOwner.timer = pokemonOwner.attackDuration;
						pokemonOwner.canAttack = false;
						break;
					}
				} 
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}

        public override void AI()
        {
			Projectile.ai[0] += MathHelper.ToRadians(5);

			if(Projectile.timeLeft < 110){
				if(attackMode == (int)PokemonPlayer.AttackMode.Auto_Attack){
					SearchTarget(300F);
				}
			}

			float projSpeed = 12f;

			if(attackMode == (int)PokemonPlayer.AttackMode.Auto_Attack){
				if(foundTarget){
					if(targetPlayer != null){
						if(targetPlayer.active && !targetPlayer.dead){
							targetPosition = targetPlayer.Center;
						}else{
							targetPlayer = null;
						}
					}else if(targetEnemy != null){
						if(targetEnemy.active){
							targetPosition = targetEnemy.Center;
						}else{
							targetEnemy = null;
						}
					}

					if(canfollow){
						if(Vector2.Distance(Projectile.Center, targetPosition) > 4*projSpeed){
							Projectile.velocity +=  0.1f*(targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
							if(Projectile.velocity.Length() > projSpeed){
								Projectile.velocity = Vector2.Normalize(Projectile.velocity)*projSpeed;
							}
						}else if(Vector2.Distance(Projectile.Center, targetPosition) > projSpeed){
							Projectile.velocity +=  0.3f*(targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
							if(Projectile.velocity.Length() > 2f*projSpeed){
								Projectile.velocity = 2f*Vector2.Normalize(Projectile.velocity)*projSpeed;
							}
						}
					}
				}else{
					if(Projectile.velocity.Y < 4){
						Projectile.velocity.Y += 0.1f;
					}else{
						Projectile.velocity.Y = 4;
					}
				}
			}else{
				if(Vector2.Distance(Projectile.Center, Trainer.attackPosition) > 4*projSpeed){
					Projectile.velocity +=  0.1f*(Trainer.attackPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
					if(Projectile.velocity.Length() > projSpeed){
						Projectile.velocity = Vector2.Normalize(Projectile.velocity)*projSpeed;
					}
				}else if(Vector2.Distance(Projectile.Center, Trainer.attackPosition) > projSpeed){
					Projectile.velocity +=  0.3f*(Trainer.attackPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
					if(Projectile.velocity.Length() > 2f*projSpeed){
						Projectile.velocity = 2f*Vector2.Normalize(Projectile.velocity)*projSpeed;
					}
				}
			}

			if(Projectile.owner == Main.myPlayer){
				Projectile.netUpdate = true;
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Poisoned, 5*60);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.AddBuff(BuffID.Poisoned, 5*60);
            base.OnHitPlayer(target, info);
        }
    }
}
