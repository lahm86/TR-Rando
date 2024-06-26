﻿using TRImageControl.Textures;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRRandomizerCore.Textures;

public class TR1TextureMonitorBroker : AbstractTextureMonitorBroker<TR1Type>
{
    private static readonly Dictionary<TR1Type, TR1Type> _expandedMonitorMap = new()
    {
        [TR1Type.TRex] = TR1Type.LaraMiscAnim_H_Valley
    };

    protected override Dictionary<TR1Type, TR1Type> ExpandedMonitorMap => _expandedMonitorMap;

    protected override TextureDatabase<TR1Type> CreateDatabase()
    {
        return new TR1TextureDatabase();
    }

    protected override TR1Type TranslateAlias(string lvlName, TR1Type type)
    {
        return TR1TypeUtilities.GetAliasForLevel(lvlName, type);
    }
}
