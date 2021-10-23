﻿namespace TRRandomizerCore.SFX
{
    public enum TRSFXGeneralCategory
    {
        // General 0-20
        Unused,
        Misc,

        // Weapons 21-50
        StandardWeaponFiring = 21,
        FastWeaponFiring,

        // Explosions/Crashes 51-80
        Explosion = 51,
        Clattering,
        Breaking,

        // Footsteps 81-110
        StandardFootstep = 81,
        HeavyFootstep,

        // Death 111-140
        Death = 111,

        // Breathing 141-170
        Breathing = 141,

        // Grunting 171-200
        Grunting = 171,

        // Growling 201+
        Growling = 201
    }
}