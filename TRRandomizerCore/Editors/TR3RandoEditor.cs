﻿using System.Collections.Generic;
using System.Linq;
using TRGE.Coord;
using TRGE.Core;
using TRRandomizerCore.Randomizers;

namespace TRRandomizerCore.Editors
{
    public class TR3RandoEditor : TR3LevelEditor, ISettingsProvider
    {
        public RandomizerSettings Settings { get; private set; }

        public TR3RandoEditor(TRDirectoryIOArgs args, TREdition edition)
            : base(args, edition) { }

        protected override void ApplyConfig(Config config)
        {
            Settings = new RandomizerSettings();
            Settings.ApplyConfig(config);
        }

        protected override void StoreConfig(Config config)
        {
            Settings.StoreConfig(config);
        }

        protected override int GetSaveTarget(int numLevels)
        {
            // Add to the target as appropriate when each randomizer is implemented. Once all
            // randomizers are implemented, just call Settings.GetSaveTarget(numLevels) per TR2.
            int target = base.GetSaveTarget(numLevels);

            if (Settings.RandomizeAudio)
            {
                target += numLevels;
            }

            return target;
        }

        protected override void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor)
        {
            List<TR3ScriptedLevel> levels = new List<TR3ScriptedLevel>
            (
                scriptEditor.EnabledScriptedLevels.Cast<TR3ScriptedLevel>().ToList()
            );

            if (scriptEditor.GymAvailable)
            {
                levels.Add(scriptEditor.AssaultLevel as TR3ScriptedLevel);
            }

            // Each processor will have a reference to the script editor, so can
            // make on-the-fly changes as required.
            TR23ScriptEditor tr23ScriptEditor = scriptEditor as TR23ScriptEditor;
            string wipDirectory = _io.WIPOutputDirectory.FullName;

            if (Settings.DevelopmentMode)
            {
                (tr23ScriptEditor.Script as TR23Script).LevelSelectEnabled = true;
                scriptEditor.SaveScript();
            }

            if (!monitor.IsCancelled && Settings.RandomizeAudio)
            {
                monitor.FireSaveStateBeginning(TRSaveCategory.Custom, "Randomizing audio tracks");
                new TR3AudioRandomizer
                {
                    ScriptEditor = tr23ScriptEditor,
                    Levels = levels,
                    BasePath = wipDirectory,
                    SaveMonitor = monitor,
                    Settings = Settings
                }.Randomize(Settings.AudioSeed);
            }
        }
    }
}