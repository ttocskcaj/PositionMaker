namespace PositionMakerCli.Positions;

public class CashPosition : IPosition
{
    public Guid PositionId { get; set; }
    
    public Guid? ExpectedMatch { get; set; }
    public string Owner { get; set; }
    public string Counterparty { get; set; }
    public string Currency { get; set; }
    public string AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public string SecurityDescription { get; set; }
}