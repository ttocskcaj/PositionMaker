namespace PositionMakerCli.PositionGenerator;

using System.Collections.Concurrent;
using PositionMakerCli.Positions;

public class CashPositionGenerator : BasePositionGenerator<CashPosition>
{
    private static readonly string[] AccountNames = { "Swap Account", "Margin Account", "Bonds Account", "Equity Account" };
    private static readonly string[] SecurityDescriptions = { "Swap", "Margin", "Bond", "Equity" };
    
    private readonly string[] owners;
    private readonly ConcurrentDictionary<string, string?> accountNumberCache;

    public CashPositionGenerator(int documentCount)
    {
        this.owners = CommonUtils.GenerateCodes((int)Math.Round(documentCount / 100m), 4);
        this.accountNumberCache = new ConcurrentDictionary<string, string?>();
    }
    
    public override CashPosition Generate(CashPosition? referencePosition)
    {
        var owner = referencePosition?.Counterparty ?? this.owners[CommonUtils.Random.Next(this.owners.Length)];
        
        string counterparty;
        if (referencePosition is null)
        {
            do
            {
                counterparty = this.owners[CommonUtils.Random.Next(this.owners.Length)];
            } while (counterparty == owner);
        }
        else
        {
            counterparty = referencePosition.Owner;
        }
        
        var currency = referencePosition?.Currency ?? CommonUtils.RandomCurrency();
        
        var accountIndex = CommonUtils.Random.Next(AccountNames.Length);
        
        var accountName = referencePosition?.AccountName ?? $"{counterparty} {currency} {AccountNames[accountIndex]}";
        var accountNumber = referencePosition?.AccountNumber ?? this.GetAccountNumber(accountName);
        var securityDescription = referencePosition?.SecurityDescription ?? SecurityDescriptions[accountIndex];

        return new CashPosition
        {
            PositionId = Guid.NewGuid(),
            ExpectedMatch = referencePosition?.PositionId ?? null,
            Owner = owner,
            Counterparty = counterparty,
            Currency = currency,
            AccountName = accountName,
            AccountNumber = accountNumber,
            Balance = GetBalance(referencePosition),
            SecurityDescription = securityDescription,
        };
    }

    private decimal GetBalance(CashPosition? referencePosition)
    {
        if (referencePosition is null)
        {
            return CommonUtils.GenerateRandomDecimal();
        }
        
        if(CommonUtils.RandomChance(0.9))
        {
            var breakAmount = CommonUtils.GenerateRandomDecimal(-1000m, 1000m);
            return referencePosition.Balance + breakAmount;
        }

        return referencePosition.Balance;
    }

    private string? GetAccountNumber(string accountName)
    {
        string? accountNumber;

        if (this.accountNumberCache.TryGetValue(accountName, out accountNumber))
        {
            return accountNumber;
        }

        accountNumber = CommonUtils.GenerateAccountNumber(accountName);
        this.accountNumberCache.TryAdd(accountName, accountNumber);

        return accountNumber;
    }
}
