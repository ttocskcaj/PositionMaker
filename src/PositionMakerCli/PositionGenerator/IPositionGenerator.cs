namespace PositionMakerCli.PositionGenerator;

using PositionMakerCli.Positions;

public interface IPositionGenerator<TPosition>
    where TPosition : class, IPosition
{
    public TPosition Generate(TPosition? referencePosition);
}
