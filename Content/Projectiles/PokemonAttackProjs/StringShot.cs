﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Pokemod.Common.Players;
using Pokemod.Content.Buffs;
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
	public class StringShot : PokemonAttack
	{
		public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.timeLeft = 60;

            Projectile.tileCollide = true;  
            Projectile.penetrate = 1;
            base.SetDefaults();
        }

        public override void Attack(Projectile pokemon, float distanceFromTarget, Vector2 targetCenter){
            var pokemonOwner = (PokemonPetProjectile)pokemon.ModProjectile;

			if(pokemon.owner == Main.myPlayer){
				for(int i = 0; i < pokemonOwner.nAttackProjs; i++){
					if(pokemonOwner.attackProjs[i] == null){
						pokemonOwner.attackProjs[i] = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(pokemon), pokemon.Center, 20f*Vector2.Normalize(targetCenter-pokemon.Center), ModContent.ProjectileType<StringShot>(), pokemonOwner.GetPokemonAttackDamage(GetType().Name), 2f, pokemon.owner)];
						pokemonOwner.currentStatus = (int)PokemonPetProjectile.ProjStatus.Attack;
						SoundEngine.PlaySound(SoundID.Item17, pokemon.position);
						pokemonOwner.timer = pokemonOwner.attackDuration;
						pokemonOwner.canAttack = false;
						break;
					}
				} 
			}
		}

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> chainTexture = TextureAssets.Projectile[Projectile.type];

            Vector2 center = Projectile.Center;
            Vector2 directionToOrigin = pokemonProj.Center - Projectile.Center;

            float distanceToOrigin = directionToOrigin.Length();

            while (distanceToOrigin > chainTexture.Width() && !float.IsNaN(distanceToOrigin))
            {
                directionToOrigin /= distanceToOrigin; 
                directionToOrigin *= chainTexture.Width(); 

                center += directionToOrigin; 
                directionToOrigin = pokemonProj.Center - center; 
                distanceToOrigin = directionToOrigin.Length();

                Color drawColor = Lighting.GetColor((int)(center.X / 16), (int)(center.Y / 16));

                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, directionToOrigin.ToRotation(),
                    chainTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
            
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            
            if(Projectile.owner == Main.myPlayer){
				Projectile.netUpdate = true;
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if(target.boss){
                target.AddBuff(ModContent.BuffType<StringShotDebuff>(), 20);
            }else{
                target.AddBuff(ModContent.BuffType<StringShotDebuff>(), 60);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.AddBuff(ModContent.BuffType<StringShotDebuff>(), 60);
            base.OnHitPlayer(target, info);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			width = 6;
			height = 6;
			fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}
