﻿using TRLevelControl.Model;

namespace TRDataControl.Environment;

public class EMEditorSet : List<BaseEMFunction>, ITextureModifier
{
    // If one member is classed as hard, the entire set is hard. This differs from
    // mods having a hard variant - in that case, GetModToExecute will select the
    // relevant function to run.
    public bool IsHard => this.Any(e => e.Tags?.Contains(EMTag.Hard) ?? false);

    public void ApplyToLevel(TR1Level level, EMOptions options = null)
    {
        if (IsApplicable(options))
        {
            foreach (BaseEMFunction mod in this)
            {
                GetModToExecute(mod, options)?.ApplyToLevel(level);
            }
        }
    }

    public void ApplyToLevel(TR2Level level, EMOptions options = null)
    {
        if (IsApplicable(options))
        {
            foreach (BaseEMFunction mod in this)
            {
                GetModToExecute(mod, options)?.ApplyToLevel(level);
            }
        }
    }

    public void ApplyToLevel(TR3Level level, EMOptions options = null)
    {
        if (IsApplicable(options))
        {
            foreach (BaseEMFunction mod in this)
            {
                GetModToExecute(mod, options)?.ApplyToLevel(level);
            }
        }
    }

    public bool IsApplicable(EMOptions options)
    {
        if (options != null)
        {
            if (IsHard && !options.EnableHardMode)
            {
                // This entire set is classed as difficult.
                return false;
            }

            if (options.ExcludedTags != null && options.ExclusionMode == EMExclusionMode.BreakOnAny)
            {
                // The modification will only be performed if all tags in this set are to be included.
                foreach (BaseEMFunction mod in this)
                {
                    if (mod.Tags?.Any(options.ExcludedTags.Contains) ?? false)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void RemapTextures(Dictionary<ushort, ushort> indexMap)
    {
        // Find every EMTextureMap (or any other texture type) and remap the old texture indices to new.
        foreach (BaseEMFunction mod in this)
        {
            if (mod is ITextureModifier textureMod)
            {
                textureMod.RemapTextures(indexMap);
            }
        }
    }

    public void SetCommunityPatch(bool isCommunityPatch)
    {
        ForEach(m => m.SetCommunityPatch(isCommunityPatch));
    }

    private static BaseEMFunction GetModToExecute(BaseEMFunction mod, EMOptions options)
    {
        if (options != null)
        {
            // Check if individual tags in this mod result in it being excluded.
            if (options.ExclusionMode == EMExclusionMode.Individual
                && (mod.Tags?.Any(options.ExcludedTags.Contains) ?? false))
            {
                return null;
            }

            // Enforce the hard variant if hard mode is selected and it exists.
            if (options.EnableHardMode && mod.HardVariant != null)
            {
                return mod.HardVariant;
            }
        }

        return mod;
    }
}
