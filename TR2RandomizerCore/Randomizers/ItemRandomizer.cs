﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TR2RandomizerCore.Helpers;
using TR2RandomizerCore.Utilities;
using TR2RandomizerCore.Zones;
using TRGE.Core;
using TRLevelReader.Helpers;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;

namespace TR2RandomizerCore.Randomizers
{
    public class ItemRandomizer : RandomizerBase
    {
        public bool IncludeKeyItems { get; set; }
        public bool IsDevelopmentModeOn { get; set; }
        public bool PerformEnemyWeighting { get; set; }

        // This replaces plane cargo index as TRGE may have randomized the weaponless level(s), but will also have injected pistols
        // into predefined locations. See FindUnarmedPistolsLocation below.
        private int _unarmedLevelPistolIndex;
        private readonly Dictionary<string, List<Location>> _pistolLocations;

        public ItemRandomizer()
        {
            _pistolLocations = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(File.ReadAllText(@"Resources\unarmed_locations.json"));
        }

        public override void Randomize(int seed)
        {
            _generator = new Random(seed);

            Dictionary<string, List<Location>> locations = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(File.ReadAllText(@"Resources\item_locations.json"));

            foreach (TR23ScriptedLevel lvl in Levels)
            {
                //Read the level into a combined data/script level object
                LoadLevelInstance(lvl);

                FindUnarmedPistolsLocation();

                //Apply the modifications
                RepositionItems(locations[_levelInstance.Name]);

                //#44 - Randomize OR pistol type
                if (lvl.RemovesWeapons) { RandomizeORPistol(); }

                //#47 - Randomize the HSH weapon closet
                if (lvl.Is(LevelNames.HOME)) { PopulateHSHCloset(); }

                //Write back the level file
                SaveLevelInstance();

                if (!TriggerProgress())
                {
                    break;
                }
            }
        }

        // roomNumber is specified if ONLY that room is to be populated
        private void PlaceAllItems(List<Location> locations, int roomNumber = -1)
        {
            List<TR2Entity> ents = _levelInstance.Data.Entities.ToList();

            foreach (Location loc in locations)
            {
                Location copy = SpatialConverters.TransformToLevelSpace(loc, _levelInstance.Data.Rooms[loc.Room].Info);

                if (roomNumber == -1 || roomNumber == copy.Room)
                {
                    ents.Add(new TR2Entity
                    {
                        TypeID = (int)TR2Entities.LargeMed_S_P,
                        Room = Convert.ToInt16(copy.Room),
                        X = copy.X,
                        Y = copy.Y,
                        Z = copy.Z,
                        Angle = 0,
                        Intensity1 = -1,
                        Intensity2 = -1,
                        Flags = 0
                    });
                }
            }

            // Test unarmed locations
            if (_pistolLocations.ContainsKey(_levelInstance.Name))
            {
                foreach (Location loc in _pistolLocations[_levelInstance.Name])
                {
                    if (roomNumber == -1 || roomNumber == loc.Room)
                    {
                        ents.Add(new TR2Entity
                        {
                            TypeID = (int)TR2Entities.Pistols_S_P,
                            Room = (short)loc.Room,
                            X = loc.X,
                            Y = loc.Y,
                            Z = loc.Z,
                            Angle = 0,
                            Intensity1 = -1,
                            Intensity2 = -1,
                            Flags = 0
                        });
                    }
                }
            }

            _levelInstance.Data.NumEntities = (uint)ents.Count;
            _levelInstance.Data.Entities = ents.ToArray();
        }

        private void RepositionItems(List<Location> ItemLocs)
        {
            if (IsDevelopmentModeOn)
            {
                PlaceAllItems(ItemLocs);
                return;
            }

            if (ItemLocs.Count > 0)
            {
                //We are currently looking guns + ammo
                List<TR2Entities> targetents = TR2EntityUtilities.GetListOfGunTypes();
                targetents.AddRange(TR2EntityUtilities.GetListOfAmmoTypes());

                //And also key items...
                if (IncludeKeyItems)
                {
                    targetents.AddRange(TR2EntityUtilities.GetListOfKeyItemTypes());
                }

                //It's important to now start zoning key items as softlocks must be avoided.
                ZonedLocationCollection ZonedLocations = new ZonedLocationCollection();
                ZonedLocations.PopulateZones(_levelInstance.Name, ItemLocs, ZonePopulationMethod.KeyPuzzleQuestOnly);

                for (int i = 0; i < _levelInstance.Data.Entities.Count(); i++)
                {
                    if (targetents.Contains((TR2Entities)_levelInstance.Data.Entities[i].TypeID) && (i != _unarmedLevelPistolIndex))
                    {
                        Location RandomLocation = new Location();
                        bool FoundPossibleLocation = false;

                        if (TR2EntityUtilities.IsKeyItemType((TR2Entities)_levelInstance.Data.Entities[i].TypeID))
                        {
                            TR2Entities type = (TR2Entities)_levelInstance.Data.Entities[i].TypeID;

                            // Apply zoning for key items
                            switch (type)
                            {
                                case TR2Entities.Puzzle1_S_P:
                                    if (ZonedLocations.Puzzle1Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Puzzle1Zone[_generator.Next(0, ZonedLocations.Puzzle1Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Puzzle2_S_P:
                                    if (ZonedLocations.Puzzle2Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Puzzle2Zone[_generator.Next(0, ZonedLocations.Puzzle2Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Puzzle3_S_P:
                                    if (ZonedLocations.Puzzle3Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Puzzle3Zone[_generator.Next(0, ZonedLocations.Puzzle3Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Puzzle4_S_P:
                                    if (ZonedLocations.Puzzle4Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Puzzle4Zone[_generator.Next(0, ZonedLocations.Puzzle4Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Key1_S_P:
                                    if (ZonedLocations.Key1Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Key1Zone[_generator.Next(0, ZonedLocations.Key1Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Key2_S_P:
                                    if (ZonedLocations.Key2Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Key2Zone[_generator.Next(0, ZonedLocations.Key2Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Key3_S_P:
                                    if (ZonedLocations.Key3Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Key3Zone[_generator.Next(0, ZonedLocations.Key3Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Key4_S_P:
                                    if (ZonedLocations.Key4Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Key4Zone[_generator.Next(0, ZonedLocations.Key4Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Quest1_S_P:
                                    if (ZonedLocations.Quest1Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Quest1Zone[_generator.Next(0, ZonedLocations.Quest1Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                case TR2Entities.Quest2_S_P:
                                    if (ZonedLocations.Quest2Zone.Count > 0)
                                    {
                                        RandomLocation = ZonedLocations.Quest2Zone[_generator.Next(0, ZonedLocations.Quest2Zone.Count)];
                                        FoundPossibleLocation = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //Place standard items as normal for now
                            RandomLocation = ItemLocs[_generator.Next(0, ItemLocs.Count)];
                            FoundPossibleLocation = true;
                        }

                        if (FoundPossibleLocation)
                        {
                            Location GlobalizedRandomLocation = SpatialConverters.TransformToLevelSpace(RandomLocation, _levelInstance.Data.Rooms[RandomLocation.Room].Info);

                            _levelInstance.Data.Entities[i].Room = Convert.ToInt16(GlobalizedRandomLocation.Room);
                            _levelInstance.Data.Entities[i].X = GlobalizedRandomLocation.X;
                            _levelInstance.Data.Entities[i].Y = GlobalizedRandomLocation.Y;
                            _levelInstance.Data.Entities[i].Z = GlobalizedRandomLocation.Z;
                            _levelInstance.Data.Entities[i].Intensity1 = -1;
                            _levelInstance.Data.Entities[i].Intensity2 = -1;
                        }
                    }
                }
            }
        }

        private void FindUnarmedPistolsLocation()
        {
            // #66 - checks were previously performed to clean locations from previous
            // randomization sessions to avoid item pollution. This is no longer required
            // as randomization is now always performed on the original level files.

            // # Default pistol locations are no longer limited to one per level.

            _unarmedLevelPistolIndex = -1;

            if (_levelInstance.Script.RemovesWeapons && _pistolLocations.ContainsKey(_levelInstance.Name))
            {
                short pistolID = (short)TR2Entities.Pistols_S_P;
                int pistolIndex = _levelInstance.Data.Entities.ToList().FindIndex(e => e.TypeID == pistolID);
                if (pistolIndex != -1)
                {
                    // Sanity check that the location is one that we expect
                    TR2Entity pistols = _levelInstance.Data.Entities[pistolIndex];
                    Location pistolLocation = new Location
                    {
                        X = pistols.X,
                        Y = pistols.Y,
                        Z = pistols.Z,
                        Room = pistols.Room
                    };

                    int match = _pistolLocations[_levelInstance.Name].FindIndex
                    (
                        location =>
                            location.X == pistolLocation.X &&
                            location.Y == pistolLocation.Y &&
                            location.Z == pistolLocation.Z &&
                            location.Room == pistolLocation.Room
                    );

                    if (match != -1)
                    {
                        _unarmedLevelPistolIndex = pistolIndex;
                    }
                }
            }
        }

        private readonly Dictionary<TR2Entities, uint> _startingAmmoToGive = new Dictionary<TR2Entities, uint>()
        {
            {TR2Entities.Shotgun_S_P, 8},
            {TR2Entities.Automags_S_P, 4},
            {TR2Entities.Uzi_S_P, 4},
            {TR2Entities.Harpoon_S_P, 24},
            {TR2Entities.M16_S_P, 2},
            {TR2Entities.GrenadeLauncher_S_P, 4},
        };

        private void RandomizeORPistol()
        {
            //Is there something in the unarmed level pistol location?
            if (_unarmedLevelPistolIndex != -1)
            {
                List<TR2Entities> ReplacementWeapons = TR2EntityUtilities.GetListOfGunTypes();
                ReplacementWeapons.Add(TR2Entities.Pistols_S_P);

                TR2Entities Weap = ReplacementWeapons[_generator.Next(0, ReplacementWeapons.Count)];
                if (_levelInstance.Is(LevelNames.CHICKEN))
                {
                    // Grenade Launcher and Harpoon cannot trigger the bells in Ice Palace
                    while (Weap.Equals(TR2Entities.GrenadeLauncher_S_P) || Weap.Equals(TR2Entities.Harpoon_S_P))
                    {
                        Weap = ReplacementWeapons[_generator.Next(0, ReplacementWeapons.Count)];
                    }
                }

                TR2Entity unarmedLevelWeapons = _levelInstance.Data.Entities[_unarmedLevelPistolIndex];

                uint ammoToGive = 0;
                bool addPistols = false;
                uint smallMediToGive = 0;
                uint largeMediToGive = 0;

                if (_startingAmmoToGive.ContainsKey(Weap))
                {
                    ammoToGive = _startingAmmoToGive[Weap];
                    if (PerformEnemyWeighting)
                    {
                        // Create a score based on each type of enemy in this level and increase the ammo count based on this
                        EnemyDifficulty difficulty = EnemyUtilities.GetEnemyDifficulty(_levelInstance.GetEnemyEntities());
                        ammoToGive *= (uint)difficulty;

                        // Depending on how difficult the enemy combination is, allocate some extra helpers.
                        addPistols = difficulty > EnemyDifficulty.Easy;

                        if (difficulty == EnemyDifficulty.Medium || difficulty == EnemyDifficulty.Hard)
                        {
                            smallMediToGive++;
                        }
                        if (difficulty > EnemyDifficulty.Medium)
                        {
                            largeMediToGive++;
                        }
                        if (difficulty == EnemyDifficulty.VeryHard)
                        {
                            largeMediToGive++;
                        }
                    }
                    else if (_levelInstance.Is(LevelNames.LAIR))
                    {
                        ammoToGive *= 6;
                    }
                }

                //#68 - Provide some additional ammo for a weapon if not pistols
                if (Weap != TR2Entities.Pistols_S_P)
                {
                    AddORAmmo(GetWeaponAmmo(Weap), ammoToGive, unarmedLevelWeapons);
                }

                unarmedLevelWeapons.TypeID = (short)Weap;

                if (Weap != TR2Entities.Pistols_S_P)
                {
                    // If we haven't decided to add the pistols (i.e. for enemy difficulty)
                    // add a 1/3 chance of getting them anyway.
                    if (addPistols || _generator.Next(0, 3) == 0)
                    {
                        CopyEntity(unarmedLevelWeapons, TR2Entities.Pistols_S_P);
                    }
                }

                for (int i = 0; i < smallMediToGive; i++)
                {
                    CopyEntity(unarmedLevelWeapons, TR2Entities.SmallMed_S_P);
                }
                for (int i = 0; i < largeMediToGive; i++)
                {
                    CopyEntity(unarmedLevelWeapons, TR2Entities.LargeMed_S_P);
                }
            }
        }

        private void CopyEntity(TR2Entity entity, TR2Entities newType)
        {
            List<TR2Entity> ents = _levelInstance.Data.Entities.ToList();
            TR2Entity copy = entity.Clone();
            copy.TypeID = (short)newType;
            ents.Add(copy);
            _levelInstance.Data.NumEntities++;
            _levelInstance.Data.Entities = ents.ToArray();
        }

        private TR2Entities GetWeaponAmmo(TR2Entities weapon)
        {
            switch (weapon)
            {
                case TR2Entities.Shotgun_S_P:
                    return TR2Entities.ShotgunAmmo_S_P;
                case TR2Entities.Automags_S_P:
                    return TR2Entities.AutoAmmo_S_P;
                case TR2Entities.Uzi_S_P:
                    return TR2Entities.UziAmmo_S_P;
                case TR2Entities.Harpoon_S_P:
                    return TR2Entities.HarpoonAmmo_S_P;
                case TR2Entities.M16_S_P:
                    return TR2Entities.M16Ammo_S_P;
                case TR2Entities.GrenadeLauncher_S_P:
                    return TR2Entities.Grenades_S_P;
                default:
                    return TR2Entities.PistolAmmo_S_P;
            }
        }

        private void AddORAmmo(TR2Entities ammoType, uint count, TR2Entity weapon)
        {
            List<TR2Entity> ents = _levelInstance.Data.Entities.ToList();

            for (uint i = 0; i < count; i++)
            {
                TR2Entity ammo = weapon.Clone();

                ammo.TypeID = (short)ammoType;

                ents.Add(ammo);
            };

            _levelInstance.Data.NumEntities += count;
            _levelInstance.Data.Entities = ents.ToArray();
        }

        private void PopulateHSHCloset()
        {
            List<TR2Entities> replacementWeapons = TR2EntityUtilities.GetListOfGunTypes();
            if (_levelInstance.Script.RemovesWeapons)
            {
                replacementWeapons.Add(TR2Entities.Pistols_S_P);
            }

            // Pick a new weapon, but exclude the grenade launcher because it affects the kill
            // count. Also exclude the harpoon as neither it nor the grenade launcher can break
            // Lara's bedroom window, and the enemy there may have been randomized to one without
            // a gun. Probably not a softlock scenario but safer to exclude for now.
            TR2Entities replacementWeapon;
            do
            {
                replacementWeapon = replacementWeapons[_generator.Next(0, replacementWeapons.Count)];
            }
            while (replacementWeapon == TR2Entities.GrenadeLauncher_S_P || replacementWeapon == TR2Entities.Harpoon_S_P);

            TR2Entities replacementAmmo = GetWeaponAmmo(replacementWeapon);
            
            List<TR2Entity> ents = _levelInstance.Data.Entities.ToList();
            foreach (TR2Entity entity in ents)
            {
                if (entity.Room != 57)
                {
                    continue;
                }

                TR2Entities entityType = (TR2Entities)entity.TypeID;
                if (TR2EntityUtilities.IsGunType(entityType))
                {
                    entity.TypeID = (short)replacementWeapon;
                }
                else if (TR2EntityUtilities.IsAmmoType(entityType) && replacementWeapon != TR2Entities.Pistols_S_P)
                {
                    entity.TypeID = (short)replacementAmmo;
                }
            }
        }
    }
}