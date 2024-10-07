namespace PositionMakerCli.Positions;

public class GtrPosition : IPosition
{
    public Guid PositionId { get; set; }
    public Guid? ExpectedMatch { get; set; }
    
    // Counterparty Data
    public string? ReportingCounterpartyId { get; set; }
    public string? OtherCounterpartyId { get; set; }
    public string? BeneficiaryId { get; set; }
    public string? BrokerId { get; set; }
    public string? TradingVenueId { get; set; }
    public string? CounterpartySide { get; set; }

    // Trade Data
    public string? TradeId { get; set; }
    public DateTime? TradeTimestamp { get; set; }
    public string? ProductId { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public string? TradeType { get; set; }

    // Contract Data
    public decimal? NotionalAmount { get; set; }
    public DateTime? MaturityDate { get; set; }
    public string? OptionType { get; set; }
    public decimal? StrikePrice { get; set; }
    public string? SettlementType { get; set; }
    public int? UnderlyingAsset { get; set; }
    public int? UnderlyingAssetQuantity { get; set; }

    // Risk Management Data
    public decimal? InitialMargin { get; set; }
    public decimal? VariationMargin { get; set; }
    public decimal? MarkToMarketValue { get; set; }
    public string? CollateralType { get; set; }
    public decimal? CollateralAmount { get; set; }
    public int? CreditSupportAnnexDetails { get; set; }

    // Clearing Data
    public string? ClearingHouse { get; set; }
    public string? ClearingMember { get; set; }
    public string? ClearingStatus { get; set; }
    public DateTime? ClearingTimestamp { get; set; }

    // Confirmation and Settlement Data
    public DateTime? ConfirmationTimestamp { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string? SettlementLocation { get; set; }

    // Additional Data
    public decimal? BrokerageFees { get; set; }
    public decimal? UpfrontPayment { get; set; }
    public string? CompressionInformation { get; set; }
    public string? AmendmentInformation { get; set; }
    public string? NovationInformation { get; set; }

    // Common Data
    public string? ActionType { get; set; }
    public DateTime? ExecutionTimestamp { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? MasterAgreementType { get; set; }
    public string? MasterAgreementVersion { get; set; }
}
