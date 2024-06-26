﻿using Newtonsoft.Json;
using TRLevelControl.Model;

namespace TRDataControl.Environment;

public abstract class BaseEMCondition
{
    [JsonProperty(Order = -2)]
    public string Comments { get; set; }
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Include)]
    public EMConditionType ConditionType { get; set; }
    public bool Negate { get; set; }
    public List<BaseEMCondition> And { get; set; }
    public List<BaseEMCondition> Or { get; set; }
    public BaseEMCondition Xor { get; set; }

    public bool GetResult(TR1Level level)
    {
        bool result = Evaluate(level);
        if (Negate)
        {
            result = !result;
        }

        And?.ForEach(a => result &= a.GetResult(level));
        Or?.ForEach(o => result |= o.GetResult(level));
        if (Xor != null)
        {
            result ^= Xor.GetResult(level);
        }

        return result;
    }

    public bool GetResult(TR2Level level)
    {
        bool result = Evaluate(level);
        if (Negate)
        {
            result = !result;
        }

        And?.ForEach(a => result &= a.GetResult(level));
        Or?.ForEach(o => result |= o.GetResult(level));
        if (Xor != null)
        {
            result ^= Xor.GetResult(level);
        }

        return result;
    }

    public bool GetResult(TR3Level level)
    {
        bool result = Evaluate(level);
        if (Negate)
        {
            result = !result;
        }

        And?.ForEach(a => result &= a.GetResult(level));
        Or?.ForEach(o => result |= o.GetResult(level));
        if (Xor != null)
        {
            result ^= Xor.GetResult(level);
        }

        return result;
    }

    protected abstract bool Evaluate(TR1Level level);
    protected abstract bool Evaluate(TR2Level level);
    protected abstract bool Evaluate(TR3Level level);
}
