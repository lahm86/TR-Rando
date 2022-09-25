using System;
using System.Collections.Generic;
using TRGE.Core;
using TRLevelReader.Helpers;
using TRLevelReader.Model;
using TRRandomizerCore.Helpers;
using TRRandomizerCore.Levels;

namespace TRRandomizerCore.Randomizers
{
    public class TR3WeatherRandomizer : BaseTR3Randomizer
    {
        private List<TR3ScriptedLevel> _rainLevels, _snowLevels, _coldLevels;

        public override void Randomize(int seed)
        {
            _generator = new Random(seed);
            ChooseWeatherLevels();

            foreach (TR3ScriptedLevel lvl in Levels)
            {
                LoadLevelInstance(lvl);

                SetWeatherFlags(_levelInstance.Script);
                SetWeatherDependencies(_levelInstance);

                SaveLevelInstance();

                if (!TriggerProgress())
                {
                    break;
                }
            }
        }

        private void ChooseWeatherLevels()
        {
            _rainLevels = ChooseWeatherLevels(Settings.RainLevelCount, Settings.RainyAssaultCourse);
            _snowLevels = ChooseWeatherLevels(Settings.SnowLevelCount, Settings.SnowyAssaultCourse);
            _coldLevels = ChooseWeatherLevels(Settings.ColdLevelCount, Settings.ColdAssaultCourse);
        }

        private List<TR3ScriptedLevel> ChooseWeatherLevels(uint levelCount, bool includeAssault)
        {
            TR3ScriptedLevel assaultCourse = Levels.Find(l => l.Is(TR3LevelNames.ASSAULT));
            List<TR3ScriptedLevel> levels = Levels.RandomSelection(_generator, (int)levelCount, exclusions: new HashSet<TR3ScriptedLevel>
            {
                assaultCourse
            });

            if (includeAssault)
            {
                levels.Add(assaultCourse);
            }

            return levels;
        }

        private void SetWeatherFlags(TR3ScriptedLevel level)
        {
            level.HasRain = _rainLevels.Contains(level);
            level.HasSnow = _snowLevels.Contains(level);
            level.HasColdWater = _coldLevels.Contains(level);
        }

        private void SetWeatherDependencies(TR3CombinedLevel level)
        {
            // Outside rooms need to have IsWindy set to show weather
            if (level.Script.HasRain || level.Script.HasSnow)
            {
                foreach (TR3Room room in level.Data.Rooms)
                {
                    if (room.IsSkyboxVisible)
                    {
                        room.IsWindy = true;
                    }
                }
            }
        }
    }
}