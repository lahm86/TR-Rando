﻿using Newtonsoft.Json;
using System.Drawing;
using TRGE.Core;
using TRImageControl.Textures;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRRandomizerCore.Helpers;
using TRRandomizerCore.Levels;
using TRRandomizerCore.Processors;
using TRRandomizerCore.Textures;
using TRRandomizerCore.Utilities;

namespace TRRandomizerCore.Randomizers;

public class TR3TextureRandomizer : BaseTR3Randomizer, ITextureVariantHandler
{
    private static readonly Color[] _wireframeColours = ColorUtilities.GetWireframeColours();

    private readonly Dictionary<AbstractTextureSource, string> _persistentVariants;
    private readonly Dictionary<string, WireframeData<TR3Type>> _wireframeData;
    private readonly object _drawLock;
    private TR3TextureDatabase _textureDatabase;
    private Dictionary<TextureCategory, bool> _textureOptions;
    private List<TR3ScriptedLevel> _wireframeLevels;
    private List<TR3ScriptedLevel> _solidLaraLevels;
    private Color _persistentWireColour;

    internal bool NightModeOnly => !Settings.RandomizeTextures;
    internal TR3TextureMonitorBroker TextureMonitor { get; set; }

    public TR3TextureRandomizer()
    {
        _persistentVariants = new Dictionary<AbstractTextureSource, string>();
        _wireframeData = JsonConvert.DeserializeObject<Dictionary<string, WireframeData<TR3Type>>>(ReadResource("TR3/Textures/wireframing.json"));
        _drawLock = new object();
    }

    public override void Randomize(int seed)
    {
        _generator = new(seed);
        _textureDatabase = new();
        if (NightModeOnly)
        {
            RandomizeNightModeTextures();
        }
        else
        {
            RandomizeAllTextures();
        }
    }

    private void RandomizeNightModeTextures()
    {
        // This is called if global texture randomization is disabled, but night-mode randomization is selected.
        // The main idea is to replace the SkyBox in levels that are now set at night, but this is treated as a
        // texture category so potentially any other textures could also be targeted.
        _textureOptions = new Dictionary<TextureCategory, bool>();

        foreach (TR3ScriptedLevel lvl in Levels)
        {
            LoadLevelInstance(lvl);

            TextureMonitor<TR3Type> monitor = TextureMonitor.GetMonitor(_levelInstance.Name);
            if (monitor != null && monitor.UseNightTextures)
            {
                TR3TextureMapping mapping = GetMapping(_levelInstance);
                using (TextureHolder<TR3Type, TR3Level> holder = new(mapping, this))
                {
                    foreach (AbstractTextureSource source in holder.Variants.Keys)
                    {
                        if (source.IsInCategory(TextureCategory.NightMode))
                        {
                            RedrawTargets(holder.Mapping, source, holder.Variants[source], _textureOptions);
                        }
                    }

                    DrawReplacements(holder.Mapping);
                }

                SaveLevelInstance();
            }

            if (!TriggerProgress())
            {
                break;
            }
        }
    }

    private void RandomizeAllTextures()
    {
        // These options are used to switch on/off specific textures
        _textureOptions = new Dictionary<TextureCategory, bool>
        {
            [TextureCategory.KeyItem] = !Settings.RetainKeySpriteTextures,
            [TextureCategory.Secret] = !Settings.RetainSecretSpriteTextures,
            [TextureCategory.Enemy] = !Settings.RetainEnemyTextures,
            [TextureCategory.Lara] = !Settings.RetainLaraTextures,
            [TextureCategory.Braid] = !Settings.RetainLaraTextures
        };

        SetMessage("Randomizing textures - loading levels");

        ChooseWireframeLevels();

        List<TextureProcessor> processors = new() { new TextureProcessor(this) };
        int levelSplit = (int)(Levels.Count / _maxThreads);

        bool beginProcessing = true;
        foreach (TR3ScriptedLevel lvl in Levels)
        {
            if (processors[^1].LevelCount >= levelSplit)
            {
                // Kick start the last one
                processors[^1].Start();
                processors.Add(new TextureProcessor(this));
            }

            processors[^1].AddLevel(LoadCombinedLevel(lvl));

            if (!TriggerProgress())
            {
                beginProcessing = false;
                break;
            }
        }

        if (beginProcessing)
        {
            SetMessage("Randomizing textures - applying texture packs");
            foreach (TextureProcessor processor in processors)
            {
                processor.Start();
            }

            foreach (TextureProcessor processor in processors)
            {
                processor.Join();
            }
        }

        _processingException?.Throw();
    }

    private void ChooseWireframeLevels()
    {
        TR3ScriptedLevel assaultCourse = Levels.Find(l => l.Is(TR3LevelNames.ASSAULT));
        ISet<TR3ScriptedLevel> exlusions = new HashSet<TR3ScriptedLevel> { assaultCourse };

        _wireframeLevels = Levels.RandomSelection(_generator, (int)Settings.WireframeLevelCount, exclusions: exlusions);
        if (Settings.AssaultCourseWireframe)
        {
            _wireframeLevels.Add(assaultCourse);
        }

        if (Settings.UseSolidLaraWireframing)
        {
            _solidLaraLevels = new List<TR3ScriptedLevel>(_wireframeLevels);
        }
        else
        {
            _solidLaraLevels = _wireframeLevels.RandomSelection(_generator, _generator.Next(Math.Min(1, _wireframeLevels.Count), _wireframeLevels.Count));
        }

        if (Settings.PersistTextureVariants)
        {
            _persistentWireColour = _wireframeColours[_generator.Next(0, _wireframeColours.Length)];
        }

        foreach (WireframeData<TR3Type> data in _wireframeData.Values)
        {
            data.HighlightLadders = Settings.UseWireframeLadders;
            data.HighlightTriggers = data.HighlightDeathTiles = Settings.ShowWireframeTriggers;
            data.SolidInteractables = Settings.UseSolidInteractableWireframing;
            foreach (SpecialTextureHandling special in data.SpecialTextures)
            {
                List<SpecialTextureMode> modes = WireframeData<TR3Type>.GetDrawModes(special.Type);
                special.Mode = modes[_generator.Next(0, modes.Count)];
            }
        }
    }

    public string GetSourceVariant(AbstractTextureSource source)
    {
        lock (_drawLock)
        {
            if (Settings.PersistTextureVariants && _persistentVariants.ContainsKey(source))
            {
                return _persistentVariants[source];
            }

            string[] variants = source.Variants;
            string variant = variants[_generator.Next(0, variants.Length)];

            StoreVariant(source, variant);
            return variant;
        }
    }

    public void StoreVariant(AbstractTextureSource source, string variant)
    {
        if (Settings.PersistTextureVariants)
        {
            _persistentVariants[source] = variant;
        }
    }

    private TR3TextureMapping GetMapping(TR3CombinedLevel level)
    {
        lock (_drawLock)
        {
            return TR3TextureMapping.Get
            (
                level.Data,
                level.Name,
                _textureDatabase,
                TextureMonitor.GetLevelMapping(level.Name),
                TextureMonitor.GetIgnoredTypes(level.Name),
                TextureMonitor.GetTypeMap(level.Name)
            );
        }
    }

    private void RedrawTargets(AbstractTextureMapping<TR3Type, TR3Level> mapping, AbstractTextureSource source, string variant, Dictionary<TextureCategory, bool> options)
    {
        lock (_drawLock)
        {
            mapping.RedrawTargets(source, variant, options);
        }
    }

    private void DrawReplacements(AbstractTextureMapping<TR3Type, TR3Level> mapping)
    {
        lock (_drawLock)
        {
            mapping.DrawReplacements();
        }
    }

    private bool IsWireframeLevel(TR3CombinedLevel lvl)
    {
        return !NightModeOnly &&
            _wireframeData.ContainsKey(lvl.Name) &&
            (_wireframeLevels.Contains(lvl.Script) || (lvl.IsCutScene && _wireframeLevels.Contains(lvl.ParentLevel.Script)));
    }

    private bool IsSolidLaraLevel(TR3CombinedLevel lvl)
    {
        return IsWireframeLevel(lvl) &&
            (_solidLaraLevels.Contains(lvl.Script) || (lvl.IsCutScene && _solidLaraLevels.Contains(lvl.ParentLevel.Script)));
    }

    private WireframeData<TR3Type> GetWireframeData(TR3CombinedLevel lvl)
    {
        if (IsWireframeLevel(lvl))
        {
            // Allow for JP version overrides in the data
            string japaneseName = lvl.Name + "-JP";
            return IsJPVersion && _wireframeData.ContainsKey(japaneseName) ?
                _wireframeData[japaneseName] :
                _wireframeData[lvl.Name];
        }
        return null;
    }

    private Color GetWireframeVariant(bool overridePersistent = false)
    {
        return Settings.PersistTextureVariants && !overridePersistent ?
            _persistentWireColour :
            _wireframeColours[_generator.Next(0, _wireframeColours.Length)];
    }

    internal class TextureProcessor : AbstractProcessorThread<TR3TextureRandomizer>
    {
        private readonly Dictionary<TR3CombinedLevel, TextureHolder<TR3Type, TR3Level>> _holders;
        private readonly TR3LandmarkImporter _landmarkImporter;
        private readonly TR3Wireframer _wireframer;

        internal override int LevelCount => _holders.Count;

        internal TextureProcessor(TR3TextureRandomizer outer)
            : base(outer)
        {
            _holders = new Dictionary<TR3CombinedLevel, TextureHolder<TR3Type, TR3Level>>();
            _landmarkImporter = new TR3LandmarkImporter();
            _wireframer = new TR3Wireframer();
        }

        internal void AddLevel(TR3CombinedLevel level)
        {
            _holders.Add(level, null);
            if (level.HasCutScene)
            {
                _holders.Add(level.CutSceneLevel, null);
            }
        }

        protected override void StartImpl()
        {
            List<TR3CombinedLevel> levels = new(_holders.Keys);
            levels.Sort(delegate (TR3CombinedLevel lvl1, TR3CombinedLevel lvl2)
            {
                return lvl1.IsCutScene && lvl1.ParentLevel == lvl2 ? 1 : 0;
            });

            foreach (TR3CombinedLevel level in levels)
            {
                TR3TextureMapping mapping = _outer.GetMapping(level);
                if (mapping != null)
                {
                    TextureHolder<TR3Type, TR3Level> parentHolder = null;
                    if (level.IsCutScene)
                    {
                        parentHolder = _holders[level.ParentLevel];
                    }
                    _holders[level] = new TextureHolder<TR3Type, TR3Level>(mapping, _outer, parentHolder);

                    if (_outer.IsWireframeLevel(level))
                    {
                        WireframeData<TR3Type> data = _outer.GetWireframeData(level);
                        data.SolidEnemies = _outer.Settings.UseSolidEnemyWireframing;
                        if (level.IsCutScene)
                        {
                            WireframeData<TR3Type> parentData = _outer.GetWireframeData(level.ParentLevel);
                            data.HighlightColour = parentData.HighlightColour;
                            data.SolidLara = parentData.SolidLara;
                        }
                        else
                        {
                            data.HighlightColour = _outer.GetWireframeVariant();
                            data.SolidLara = _outer.IsSolidLaraLevel(level);
                        }

                        if (_outer.Settings.ShowWireframeTriggerColours)
                        {
                            do
                            {
                                data.TriggerColour = _outer.GetWireframeVariant(true);
                            }
                            while (!ColorUtilities.TestWireframeContrast(data.TriggerColour, data.HighlightColour));
                            do
                            {
                                data.DeathColour = _outer.GetWireframeVariant(true);
                            }
                            while (!ColorUtilities.TestWireframeContrast(data.DeathColour, data.HighlightColour)
                                || !ColorUtilities.TestWireframeContrast(data.DeathColour, data.TriggerColour));
                        }
                        else
                        {
                            data.TriggerColour = data.HighlightColour;
                            data.DeathColour = data.HighlightColour;
                        }

                        if (_outer.Settings.UseDifferentWireframeColours)
                        {
                            foreach (TR3Type type in level.Data.Models.Keys)
                            {
                                data.ModelColours[type] = _outer.GetWireframeVariant();
                            }
                        }
                    }
                }
                else
                {
                    _holders.Remove(level);
                    _outer.TriggerProgress(2); // Skip processing
                }
            }
        }

        protected override void ProcessImpl()
        {
            Dictionary<TextureCategory, bool> options = new(_outer._textureOptions);

            foreach (TR3CombinedLevel level in _holders.Keys)
            {
                ProcessLevel(level, options);

                int progress = level.IsCutScene ? 0 : 1;
                if (!_outer.TriggerProgress(progress))
                {
                    break;
                }

                _outer.SaveLevel(level);
                if (!_outer.TriggerProgress(progress))
                {
                    break;
                }
            }
        }

        private void ProcessLevel(TR3CombinedLevel level, Dictionary<TextureCategory, bool> options)
        {
            TextureMonitor<TR3Type> monitor = _outer.TextureMonitor.GetMonitor(level.Name);
            bool isWireframe = _outer.IsWireframeLevel(level);

            options[TextureCategory.NightMode] = monitor != null && monitor.UseNightTextures;
            options[TextureCategory.DayMode] = !options[TextureCategory.NightMode];
            options[TextureCategory.Lara] = _outer._textureOptions[TextureCategory.Lara]
                && (monitor == null || monitor.UseLaraOutfitTextures);

            using (TextureHolder<TR3Type, TR3Level> holder = _holders[level])
            {
                foreach (AbstractTextureSource source in holder.Variants.Keys)
                {
                    _outer.RedrawTargets(holder.Mapping, source, holder.Variants[source], options);
                }

                if (!isWireframe)
                {
                    // Add landmarks, but only if there is room available for them
                    if (holder.Mapping.LandmarkMapping.Count > 0)
                    {
                        _landmarkImporter.Import(level.Data, holder.Mapping, monitor != null && monitor.UseMirroring);
                    }

                    _outer.DrawReplacements(holder.Mapping);
                }
            }

            if (isWireframe)
            {
                _wireframer.Apply(level.Data, _outer.GetWireframeData(level));
            }
        }
    }
}
