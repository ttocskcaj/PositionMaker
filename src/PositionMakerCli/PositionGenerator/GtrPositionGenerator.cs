namespace PositionMakerCli.PositionGenerator;

using System.Collections.Concurrent;
using PositionMakerCli.Positions;

public class GtrPositionGenerator : BasePositionGenerator<GtrPosition>
{
    private static readonly string[] CounterpartySides = { "Buyer", "Seller" };
    private static readonly string[] TradeTypes = { "New", "Modification", "Cancellation" };
    public static readonly string[] OptionTypes = {
        "Vanilla Call", "Vanilla Put",
        "Barrier Knock-In Call", "Barrier Knock-Out Call", "Barrier Knock-In Put", "Barrier Knock-Out Put",
        "Binary Call", "Binary Put",
        "Asian Call", "Asian Put",
        "Lookback Call", "Lookback Put",
        "Chooser",
        "Compound Call on Call", "Compound Put on Call", "Compound Call on Put", "Compound Put on Put",
        "Bermudan Call", "Bermudan Put",
        "Rainbow",
        "Swaption Payer", "Swaption Receiver",
        "Credit",
        "Weather Temperature", "Weather Precipitation",
    };
    private static readonly string[] SettlementTypes = { "Cash", "Physical" };
    private static readonly string[] CollateralTypes = { "Cash", "Securities" };
    private static readonly string[] ClearingStatuses = { "Cleared", "Uncleared" };
    private static readonly string[] ActionTypes = { "New", "Modify", "Cancel", "Error" };
    private static readonly string[] MasterAgreementTypes = { "ISDA", "OTHER" };

    private readonly string[] leiCodes;
    private readonly ConcurrentDictionary<string, string?> tradeIdCache;
    
    public GtrPositionGenerator(int documentCount)
    {
        this.leiCodes = CommonUtils.GenerateLeiCodes(2, 20);
        this.tradeIdCache = new ConcurrentDictionary<string, string?>();
    }
    
    public override GtrPosition Generate(GtrPosition? referenceData)
    {
        var reportingCounterpartyId = referenceData?.OtherCounterpartyId ?? this.leiCodes[CommonUtils.Random.Next(this.leiCodes.Length)];
        var otherCounterpartyId = referenceData?.ReportingCounterpartyId ?? this.leiCodes[CommonUtils.Random.Next(this.leiCodes.Length)];
        var beneficiaryId = referenceData?.BeneficiaryId ?? this.leiCodes[CommonUtils.Random.Next(this.leiCodes.Length)];
        var brokerId = referenceData?.BrokerId ?? this.leiCodes[CommonUtils.Random.Next(this.leiCodes.Length)];
        var tradingVenueId = referenceData?.TradingVenueId ?? this.leiCodes[CommonUtils.Random.Next(this.leiCodes.Length)];

        string counterpartySide;
        if (referenceData is not null)
        {
            if (referenceData.CounterpartySide == "Buyer")
            {
                counterpartySide = "Seller";
            }
            else
            {
                counterpartySide = "Buyer";
            }
        }
        else
        {
            counterpartySide = CounterpartySides[CommonUtils.Random.Next(CounterpartySides.Length)];
        }

        string? tradeId;
        if (CommonUtils.RandomChance(0.9))
        {
            tradeId = referenceData?.TradeId ?? this.GetTradeId(reportingCounterpartyId);
        }
        else
        {
            tradeId = null;
        }
        var tradeTimestamp = referenceData?.TradeTimestamp ?? DateTime.Now;

        string? productId;
        if (CommonUtils.RandomChance(0.8))
        {
            productId = referenceData?.ProductId ?? CommonUtils.GenerateIsin();
        }
        else
        {
            productId = null;
        }        

        var quantity = referenceData?.Quantity ?? CommonUtils.Random.Next(1, 1000);
        var price = referenceData?.Price ?? CommonUtils.GenerateRandomDecimal();
        var currency = referenceData?.Currency ?? CommonUtils.RandomCurrency();
        var tradeType = referenceData?.TradeType ?? TradeTypes[CommonUtils.Random.Next(TradeTypes.Length)];

        const float breakChance = 0.5f;

        var notionalAmount = GetUpdatedValue(breakChance, referenceData?.NotionalAmount , () => CommonUtils.GenerateRandomDecimal());
        var maturityDate = GetUpdatedValue(breakChance, referenceData?.MaturityDate , () => CommonUtils.RandomFutureDate());
        var optionType = GetUpdatedValue(breakChance, referenceData?.OptionType , () => OptionTypes[CommonUtils.Random.Next(OptionTypes.Length)]);
        var strikePrice = GetUpdatedValue(breakChance, referenceData?.StrikePrice , () => CommonUtils.GenerateRandomDecimal());
        var settlementType = GetUpdatedValue(breakChance, referenceData?.SettlementType , () => SettlementTypes[CommonUtils.Random.Next(SettlementTypes.Length)]);
        var underlyingAsset = GetUpdatedValue(breakChance, referenceData?.UnderlyingAsset , () => CommonUtils.Random.Next(10000, 99999));
        var underlyingAssetQuantity = GetUpdatedValue(breakChance, referenceData?.UnderlyingAssetQuantity , () => CommonUtils.Random.Next(1, 1000));

        var initialMargin = GetUpdatedValue(breakChance, referenceData?.InitialMargin , () => CommonUtils.GenerateRandomDecimal());
        var variationMargin = GetUpdatedValue(breakChance, referenceData?.VariationMargin , () => CommonUtils.GenerateRandomDecimal());
        var markToMarketValue = GetUpdatedValue(breakChance, referenceData?.MarkToMarketValue , () => CommonUtils.GenerateRandomDecimal());
        var collateralType = GetUpdatedValue(breakChance, referenceData?.CollateralType , () => CollateralTypes[CommonUtils.Random.Next(CollateralTypes.Length)]);
        var collateralAmount = GetUpdatedValue(breakChance, referenceData?.CollateralAmount , () => CommonUtils.GenerateRandomDecimal());
        var creditSupportAnnexDetails = GetUpdatedValue(breakChance, referenceData?.CreditSupportAnnexDetails , () => CommonUtils.Random.Next(10000, 99999));

        var clearingHouse = GetUpdatedValue(breakChance, referenceData?.ClearingHouse , () => CommonUtils.RandomCompany());
        var clearingMember = GetUpdatedValue(breakChance, referenceData?.ClearingMember , () => CommonUtils.RandomCompany());
        var clearingStatus = GetUpdatedValue(breakChance, referenceData?.ClearingStatus , () => ClearingStatuses[CommonUtils.Random.Next(ClearingStatuses.Length)]);
        var clearingTimestamp = GetUpdatedValue(breakChance, referenceData?.ClearingTimestamp , () => DateTime.Now);

        var confirmationTimestamp = GetUpdatedValue(breakChance, referenceData?.ConfirmationTimestamp , () => DateTime.Now);
        var settlementDate = GetUpdatedValue(breakChance, referenceData?.SettlementDate , () => CommonUtils.RandomFutureDate());
        var settlementLocation = GetUpdatedValue(breakChance, referenceData?.SettlementLocation , () => CommonUtils.RandomCountryCode());

        var brokerageFees = GetUpdatedValue(breakChance, referenceData?.BrokerageFees , () => CommonUtils.GenerateRandomDecimal());
        var upfrontPayment = GetUpdatedValue(breakChance, referenceData?.UpfrontPayment , () => CommonUtils.GenerateRandomDecimal());
        var compressionInformation = GetUpdatedValue(breakChance, referenceData?.CompressionInformation , () => CommonUtils.RandomWord());
        var amendmentInformation = GetUpdatedValue(breakChance, referenceData?.AmendmentInformation , () => CommonUtils.RandomWord());
        var novationInformation = GetUpdatedValue(breakChance, referenceData?.NovationInformation , () => CommonUtils.RandomWord());

        var actionType = GetUpdatedValue(breakChance, referenceData?.ActionType , () => ActionTypes[CommonUtils.Random.Next(ActionTypes.Length)]);
        var executionTimestamp = GetUpdatedValue(breakChance, referenceData?.ExecutionTimestamp , () => DateTime.Now);
        var effectiveDate = GetUpdatedValue(breakChance, referenceData?.EffectiveDate , () => DateTime.Now);
        var terminationDate = GetUpdatedValue(breakChance, referenceData?.TerminationDate , () => CommonUtils.RandomFutureDate());
        var masterAgreementType = GetUpdatedValue(breakChance, referenceData?.MasterAgreementType , () => MasterAgreementTypes[CommonUtils.Random.Next(MasterAgreementTypes.Length)]);
        var masterAgreementVersion = GetUpdatedValue(breakChance, referenceData?.MasterAgreementVersion , () => CommonUtils.Random.Next(2002, 2022).ToString());

        var positionId = Guid.NewGuid();
        if (referenceData is not null)
        {
            referenceData.ExpectedMatch = positionId;
        }
        
        return new GtrPosition
        {
            PositionId = positionId,
            ExpectedMatch = referenceData?.PositionId,
            ReportingCounterpartyId = reportingCounterpartyId,
            OtherCounterpartyId = otherCounterpartyId,
            BeneficiaryId = beneficiaryId,
            BrokerId = brokerId,
            TradingVenueId = tradingVenueId,
            CounterpartySide = counterpartySide,

            TradeId = tradeId,
            TradeTimestamp = tradeTimestamp,
            ProductId = productId,
            Quantity = quantity,
            Price = price,
            Currency = currency,
            TradeType = tradeType,

            NotionalAmount = notionalAmount,
            MaturityDate = maturityDate,
            OptionType = optionType,
            StrikePrice = strikePrice,
            SettlementType = settlementType,
            UnderlyingAsset = underlyingAsset,
            UnderlyingAssetQuantity = underlyingAssetQuantity,

            InitialMargin = initialMargin,
            VariationMargin = variationMargin,
            MarkToMarketValue = markToMarketValue,
            CollateralType = collateralType,
            CollateralAmount = collateralAmount,
            CreditSupportAnnexDetails = creditSupportAnnexDetails,

            ClearingHouse = clearingHouse,
            ClearingMember = clearingMember,
            ClearingStatus = clearingStatus,
            ClearingTimestamp = clearingTimestamp,

            ConfirmationTimestamp = confirmationTimestamp,
            SettlementDate = settlementDate,
            SettlementLocation = settlementLocation,

            BrokerageFees = brokerageFees,
            UpfrontPayment = upfrontPayment,
            CompressionInformation = compressionInformation,
            AmendmentInformation = amendmentInformation,
            NovationInformation = novationInformation,

            ActionType = actionType,
            ExecutionTimestamp = executionTimestamp,
            EffectiveDate = effectiveDate,
            TerminationDate = terminationDate,
            MasterAgreementType = masterAgreementType,
            MasterAgreementVersion = masterAgreementVersion,
        };
    }
    
    private string? GetTradeId(string reportingCounterpartyId)
    {
        string? tradeId;

        if (this.tradeIdCache.TryGetValue(reportingCounterpartyId, out tradeId))
        {
            return tradeId;
        }

        tradeId = CommonUtils.GenerateTradeId(reportingCounterpartyId);
        this.tradeIdCache.TryAdd(reportingCounterpartyId, tradeId);

        return tradeId;
    }
}
