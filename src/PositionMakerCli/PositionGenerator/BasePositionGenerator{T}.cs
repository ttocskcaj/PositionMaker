using PositionMakerCli.Positions;

namespace PositionMakerCli.PositionGenerator;

public abstract class BasePositionGenerator<T> : IPositionGenerator<T> where T : class, IPosition
{
    protected const float BreakChance = 0.5f;

    public abstract T Generate(T? referencePosition);

    protected static T1 GetUpdatedValue<T1>(double breakChance, T1? referenceValue, Func<T1> generateNewValue)
        where T1 : struct
    {
        if (referenceValue.HasValue && CommonUtils.RandomChance(breakChance))
        {
            return generateNewValue();
        }

        return referenceValue ?? generateNewValue();
    }

    protected static T1 GetUpdatedValue<T1>(double breakChance, T1? referenceValue, Func<T1> generateNewValue)
    {
        if (referenceValue is not null && CommonUtils.RandomChance(breakChance))
        {
            return generateNewValue();
        }

        return referenceValue ?? generateNewValue();
    }
}