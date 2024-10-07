namespace PositionMakerCli.PositionGenerator;

using System.Security.Cryptography;
using System.Text;

public class CommonUtils
{
    public static Random Random = new Random();
    public static readonly SHA256 Sha256 = SHA256.Create();
    const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string[] Currencies =
    {
        "USD", "EUR", "JPY", "GBP",// "AUD", "CAD", "CHF", "CNY", "SEK", "NZD",
        // "MXN", "SGD", "HKD", "NOK", "KRW", "TRY", "INR", "RUB", "BRL", "ZAR",
        // "DKK", "PLN", "TWD", "THB", "IDR", "HUF", "CZK", "ILS", "CLP", "PHP",
        // "AED", "COP", "SAR", "MYR", "RON", "BGN", "HRK", "VND", "ISK", "NGN",
        // "PKR", "EGP", "KZS", "ARS", "QAR", "KES", "GHS", "PEN", "UAH", "BDT",
    };

    public static string[] Words =
        { "Compression", "Amendment", "Novation", "Termination", "Modification", "Cancellation" };

    public static string[] Companies =
        { "Acme Corp", "Globex Corporation", "Soylent Corp", "Initech", "Umbrella Corporation", "Hooli" };

    public static string[] CountryCodes = { "US", "CA", "GB", "FR", "DE", "JP" };


    public static string RandomCurrency() => Currencies[Random.Next(Currencies.Length)];

    public static string[] GenerateCodes(int numberOfCodes, int size)
    {
        var codes = new string[numberOfCodes];

        for (var i = 0; i < numberOfCodes; i++)
        {
            codes[i] = new string(Enumerable.Repeat(Chars, size)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        return codes;
    }

    public static string GenerateAccountNumber(string accountName)
    {
        // Assume that accountName will never be larger than 64 bytes.
        Span<byte> inputBytes = stackalloc byte[64];
        Span<byte>
            hashBytes = stackalloc byte[20]; // SHA256 hash size is 32 bytes, but we only need the first 20 bytes.

        Encoding.UTF8.GetBytes(accountName, inputBytes);
        Sha256.TryComputeHash(inputBytes, hashBytes, out _);

        Span<char> hexChars = stackalloc char[10];
        for (var i = 0; i < 5; i++)
        {
            var b = hashBytes[i];
            hexChars[2 * i] = GetHexChar((b & 0xF0) >> 4);
            hexChars[2 * i + 1] = GetHexChar(b & 0x0F);
        }

        return new string(hexChars).ToUpper();
    }

    private static char GetHexChar(int value) => value < 10 ? (char)('0' + value) : (char)('A' + value - 10);

    public static decimal GenerateRandomDecimal(decimal min = 0.0001m, decimal max = 20_000_000, int decimalPlaces = 2)
    {
        var randomNumber = (decimal)Random.NextDouble() * (max - min) + min;
        return Math.Round(randomNumber, decimalPlaces);
    }

    public static bool RandomChance(double chance) => Random.NextDouble() < chance;

    public static string GenerateIsin() => Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 12);

    public static string RandomWord() => Words[Random.Next(Words.Length)];

    public static DateTime RandomFutureDate() => DateTime.Now.AddDays(Random.Next(1, 365));

    public static string GenerateTradeId(string reportingCounterpartyId) =>
        reportingCounterpartyId + Random.Next(10000, 99999).ToString();

    public static string RandomCompany() => Companies[Random.Next(Companies.Length)];

    public static string RandomCountryCode() => CountryCodes[Random.Next(CountryCodes.Length)];

    public static string GenerateLei() => Guid.NewGuid().ToString().Replace("-", "").ToUpper();

    // Generate multiple random LEI codes
    public static string[] GenerateLeiCodes(int count, int length)
    {
        string[] leiCodes = new string[count];
        for (var i = 0; i < count; i++)
        {
            leiCodes[i] = GenerateLei().Substring(0, length);
        }

        return leiCodes;
    }
}
