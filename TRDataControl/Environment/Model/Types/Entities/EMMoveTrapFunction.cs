﻿using TRLevelControl.Model;

namespace TRDataControl.Environment;

public class EMMoveTrapFunction : BaseMoveTriggerableFunction
{
    public override void ApplyToLevel(TR1Level level)
    {
        TR1Entity trap = level.Entities[EntityIndex];
        RepositionTriggerable(trap, level);
    }

    public override void ApplyToLevel(TR2Level level)
    {
        TR2Entity trap = level.Entities[EntityIndex];
        RepositionTriggerable(trap, level);
    }

    public override void ApplyToLevel(TR3Level level)
    {
        TR3Entity trap = level.Entities[EntityIndex];
        RepositionTriggerable(trap, level);
    }
}
