namespace PositionMakerCli.Positions;

public interface IPosition
{
    Guid PositionId { get; set; }
    Guid? ExpectedMatch { get; set; }
}
