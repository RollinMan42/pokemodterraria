﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Pokemod.Content.Pets.BayleefPet
{
	public class BayleefPetBuff: PokemonPetBuff
	{
        public override string PokeName => "Bayleef";
        public override int ProjType => ModContent.ProjectileType<BayleefPetProjectile>();
    }

    public class BayleefPetBuffShiny : PokemonPetBuff
	{
        public override string PokeName => "Bayleef";
        public override int ProjType => ModContent.ProjectileType<BayleefPetProjectileShiny>();
    }
}
