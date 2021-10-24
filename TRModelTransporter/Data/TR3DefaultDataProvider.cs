﻿using System.Collections.Generic;
using TRLevelReader.Model.Enums;

namespace TRModelTransporter.Data
{
    public class TR3DefaultDataProvider : ITransportDataProvider<TR3Entities>
    {
        public Dictionary<TR3Entities, TR3Entities> AliasPriority { get; set; }

        public IEnumerable<TR3Entities> GetModelDependencies(TR3Entities entity)
        {
            return _entityDependencies.ContainsKey(entity) ? _entityDependencies[entity] : _emptyEntities;
        }

        public IEnumerable<TR3Entities> GetSpriteDependencies(TR3Entities entity)
        {
            return _spriteDependencies.ContainsKey(entity) ? _spriteDependencies[entity] : _emptyEntities;
        }

        public IEnumerable<TR3Entities> GetCinematicEntities()
        {
            return _cinematicEntities;
        }

        public IEnumerable<TR3Entities> GetLaraDependants()
        {
            return _laraDependentModels;
        }

        public bool IsAlias(TR3Entities entity)
        {
            foreach (List<TR3Entities> aliases in _entityAliases.Values)
            {
                if (aliases.Contains(entity))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAliases(TR3Entities entity)
        {
            return _entityAliases.ContainsKey(entity);
        }

        public TR3Entities TranslateAlias(TR3Entities entity)
        {
            foreach (TR3Entities root in _entityAliases.Keys)
            {
                if (_entityAliases[root].Contains(entity))
                {
                    return root;
                }
            }

            return entity;
        }

        public IEnumerable<TR3Entities> GetAliases(TR3Entities entity)
        {
            return _entityAliases.ContainsKey(entity) ? _entityAliases[entity] : _emptyEntities;
        }

        public TR3Entities GetLevelAlias(string level, TR3Entities entity)
        {
            return entity;// TR2EntityUtilities.GetAliasForLevel(level, entity);
        }

        public bool IsAliasDuplicatePermitted(TR3Entities entity)
        {
            return _permittedAliasDuplicates.Contains(entity);
        }

        public bool IsNonGraphicsDependency(TR3Entities entity)
        {
            return _nonGraphicsDependencies.Contains(entity);
        }

        public bool IsSoundOnlyDependency(TR3Entities entity)
        {
            return _soundOnlyDependencies.Contains(entity);
        }

        public short[] GetHardcodedSounds(TR3Entities entity)
        {
            return _hardcodedSoundIndices.ContainsKey(entity) ? _hardcodedSoundIndices[entity] : null;
        }

        public IEnumerable<int> GetIgnorableTextureIndices(TR3Entities entity)
        {
            return _ignoreEntityTextures.ContainsKey(entity) ? _ignoreEntityTextures[entity] : null;
        }

        #region Data

        private static readonly IEnumerable<TR3Entities> _emptyEntities = new List<TR3Entities>();

        private static readonly Dictionary<TR3Entities, TR3Entities[]> _entityDependencies = new Dictionary<TR3Entities, TR3Entities[]>
        {
            [TR3Entities.LaraIndia]
                = new TR3Entities[] { TR3Entities.LaraSkin_H_India, TR3Entities.LaraPistolAnimation_H_India, TR3Entities.LaraDeagleAnimation_H_India, TR3Entities.LaraUziAnimation_H_India },

            [TR3Entities.LaraCoastal]
                = new TR3Entities[] { TR3Entities.LaraSkin_H_Coastal, TR3Entities.LaraPistolAnimation_H_Coastal, TR3Entities.LaraDeagleAnimation_H_Coastal, TR3Entities.LaraUziAnimation_H_Coastal },

            [TR3Entities.LaraLondon]
                = new TR3Entities[] { TR3Entities.LaraSkin_H_London, TR3Entities.LaraPistolAnimation_H_London, TR3Entities.LaraDeagleAnimation_H_London, TR3Entities.LaraUziAnimation_H_London },

            [TR3Entities.LaraNevada]
                = new TR3Entities[] { TR3Entities.LaraSkin_H_Nevada, TR3Entities.LaraPistolAnimation_H_Nevada, TR3Entities.LaraDeagleAnimation_H_Nevada, TR3Entities.LaraUziAnimation_H_Nevada },

            [TR3Entities.LaraAntarc]
                = new TR3Entities[] { TR3Entities.LaraSkin_H_Antarc, TR3Entities.LaraPistolAnimation_H_Antarc, TR3Entities.LaraDeagleAnimation_H_Antarc, TR3Entities.LaraUziAnimation_H_Antarc },

            [TR3Entities.Shiva]
                = new TR3Entities[] { TR3Entities.ShivaStatue, TR3Entities.LaraExtraAnimation_H },

            [TR3Entities.Quad]
                = new TR3Entities[] { TR3Entities.LaraVehicleAnimation_H_Quad },

            [TR3Entities.Kayak]
                = new TR3Entities[] { TR3Entities.LaraVehicleAnimation_H_Kayak },

            [TR3Entities.UPV]
                = new TR3Entities[] { TR3Entities.LaraVehicleAnimation_H_UPV },

            [TR3Entities.Boat]
                = new TR3Entities[] { TR3Entities.LaraVehicleAnimation_H_Boat },

            [TR3Entities.Tyrannosaur]
                 = new TR3Entities[] { TR3Entities.LaraExtraAnimation_H },

            [TR3Entities.Willie]
                 = new TR3Entities[] { TR3Entities.LaraExtraAnimation_H },

            [TR3Entities.Infada_P]
                 = new TR3Entities[] { TR3Entities.Infada_M_H },
            [TR3Entities.OraDagger_P]
                 = new TR3Entities[] { TR3Entities.OraDagger_M_H },
            [TR3Entities.EyeOfIsis_P]
                 = new TR3Entities[] { TR3Entities.EyeOfIsis_M_H },
            [TR3Entities.Element115_P]
                 = new TR3Entities[] { TR3Entities.Element115_M_H }
        };

        private static readonly Dictionary<TR3Entities, List<TR3Entities>> _spriteDependencies = new Dictionary<TR3Entities, List<TR3Entities>>
        {
            
        };

        private static readonly List<TR3Entities> _cinematicEntities = new List<TR3Entities>
        {
            
        };

        private static readonly List<TR3Entities> _laraDependentModels = new List<TR3Entities>
        {
            
        };

        private static readonly Dictionary<TR3Entities, List<TR3Entities>> _entityAliases = new Dictionary<TR3Entities, List<TR3Entities>>
        {
            [TR3Entities.Lara] = new List<TR3Entities>
            {
                TR3Entities.LaraIndia, TR3Entities.LaraCoastal, TR3Entities.LaraLondon, TR3Entities.LaraNevada, TR3Entities.LaraAntarc
            },
            [TR3Entities.LaraSkin_H] = new List<TR3Entities>
            {
                TR3Entities.LaraSkin_H_India, TR3Entities.LaraSkin_H_Coastal, TR3Entities.LaraSkin_H_London, TR3Entities.LaraSkin_H_Nevada, TR3Entities.LaraSkin_H_Antarc
            },

            [TR3Entities.LaraPistolAnimation_H] = new List<TR3Entities>
            {
                TR3Entities.LaraPistolAnimation_H_India, TR3Entities.LaraPistolAnimation_H_Coastal, TR3Entities.LaraPistolAnimation_H_London, TR3Entities.LaraPistolAnimation_H_Nevada, TR3Entities.LaraPistolAnimation_H_Antarc
            },
            [TR3Entities.LaraDeagleAnimation_H] = new List<TR3Entities>
            {
                TR3Entities.LaraDeagleAnimation_H_India, TR3Entities.LaraDeagleAnimation_H_Coastal, TR3Entities.LaraDeagleAnimation_H_London, TR3Entities.LaraDeagleAnimation_H_Nevada, TR3Entities.LaraDeagleAnimation_H_Antarc
            },
            [TR3Entities.LaraUziAnimation_H] = new List<TR3Entities>
            {
                TR3Entities.LaraUziAnimation_H_India, TR3Entities.LaraUziAnimation_H_Coastal, TR3Entities.LaraUziAnimation_H_London, TR3Entities.LaraUziAnimation_H_Nevada, TR3Entities.LaraUziAnimation_H_Antarc
            },

            [TR3Entities.LaraVehicleAnimation_H] = new List<TR3Entities>
            {
                TR3Entities.LaraVehicleAnimation_H_Quad, TR3Entities.LaraVehicleAnimation_H_BigGun, TR3Entities.LaraVehicleAnimation_H_Kayak, TR3Entities.LaraVehicleAnimation_H_UPV, TR3Entities.LaraVehicleAnimation_H_Boat
            },

            [TR3Entities.Cobra] = new List<TR3Entities>
            {
                TR3Entities.CobraIndia, TR3Entities.CobraNevada
            },

            [TR3Entities.Dog] = new List<TR3Entities>
            {
                TR3Entities.DogLondon, TR3Entities.DogNevada
            }
        };

        private static readonly List<TR3Entities> _permittedAliasDuplicates = new List<TR3Entities>
        {
            
        };

        private static readonly List<TR3Entities> _nonGraphicsDependencies = new List<TR3Entities>
        {
            
        };

        // If these are imported into levels that already have another alias for them, only their hardcoded sounds will be imported
        protected static readonly List<TR3Entities> _soundOnlyDependencies = new List<TR3Entities>
        {
            
        };

        private static readonly Dictionary<TR3Entities, short[]> _hardcodedSoundIndices = new Dictionary<TR3Entities, short[]>
        {
            [TR3Entities.Quad] = new short[]
            {
                152, // Starting
                153, // Idling
                154, // Switch off 1
                155, // High RPM,
                156  // Switch off 2
            },
            [TR3Entities.TonyFirehands] = new short[]
            {
                76,  // Powering down
                234, // Dying
                235, // Dying
                236, // Laughing
                366, // Fireball1
                367, // Fireball2
                368  // Fireball3
            },
            [TR3Entities.Puna] = new short[]
            {
                359  // Hoo-uh!
            },
            [TR3Entities.LondonMerc] = new short[]
            {
                299  // Hey/Oi!
            },
            [TR3Entities.LondonGuard] = new short[]
            {
                299, // Hey/Oi!
                305  // Gunshot
            },
            [TR3Entities.Punk] = new short[]
            {
                299  // Hey/Oi!
            },
            [TR3Entities.UPV] = new short[]
            {
                346, // Starting
                347, // Running
                348  // Stopping
            },
            [TR3Entities.MPWithStick] = new short[]
            {
                300  // Hey!
            },
            [TR3Entities.MPWithGun] = new short[]
            {
                300  // Hey!
            },
            [TR3Entities.MPWithMP5] = new short[]
            {
                137, // Gunshot
                300  // Hey!
            },
            [TR3Entities.DamGuard] = new short[]
            {
                300  // Hey!
            },
            [TR3Entities.Prisoner] = new short[]
            {
                300  // Hey!
            },
            [TR3Entities.RXRedBoi] = new short[]
            {
                300  // Hey!
            },
            [TR3Entities.RXGunLad] = new short[]
            {
                300  // Hey!
            },
            [TR3Entities.Boat] = new short[]
            {
                194, // Starting
                195, // Idling
                196, // Accelerating
                197, // High RPM
                198, // Stopping
                199  // Hitting something
            },
            [TR3Entities.Winston] = new short[]
            {
                308, // Shuffle
                311, // Hit by shield
                314  // General grunt
            }
        };

        private static readonly Dictionary<TR3Entities, List<int>> _ignoreEntityTextures = new Dictionary<TR3Entities, List<int>>
        {
            [TR3Entities.LaraVehicleAnimation_H]
                = new List<int>(), // empty list indicates to ignore everything
            [TR3Entities.LaraExtraAnimation_H]
                = new List<int>()
        };

        #endregion
    }
}