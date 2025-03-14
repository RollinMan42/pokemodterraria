﻿using System;
using Microsoft.Xna.Framework;
using Pokemod.Content.Projectiles.PokemonAttackProjs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemod.Content.Pets.FlareonPet
{
	public class FlareonPetProjectile : PokemonPetProjectile
	{
		public override int hitboxWidth => 32;
		public override int hitboxHeight => 28;

		public override int totalFrames => 22;
		public override int animationSpeed => 6;
		public override int[] idleStartEnd => [0,8];
		public override int[] walkStartEnd => [9,17];
		public override int[] jumpStartEnd => [12,12];
		public override int[] fallStartEnd => [15,15];
		public override int[] attackStartEnd => [18,21];

		public override int nAttackProjs => 16;
		public override float enemySearchDistance => 1000;
		public override bool canAttackThroughWalls => true;
		public override int attackDuration => 20;
		public override int attackCooldown => 40;
		public override bool canMoveWhileAttack => false;

		public override void Attack(float distanceFromTarget, Vector2 targetCenter){
			if(Projectile.owner == Main.myPlayer){
				for(int i = 0; i < nAttackProjs; i+=8){
					if(attackProjs[i] == null){
						SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
						currentStatus = (int)ProjStatus.Attack;
						for(int j = 0; j < 8; j++){
							attackProjs[j] = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, 15*Vector2.Normalize(targetCenter-Projectile.Center).RotatedBy(MathHelper.ToRadians(j*45f)), ModContent.ProjectileType<LavaPlume>(), GetPokemonDamage(80, true), 2f, Projectile.owner)];
						}
					}
				} 
			}
			timer = attackDuration;
			canAttack = false;
		}
	}

	public class FlareonPetProjectileShiny : FlareonPetProjectile{}
}
