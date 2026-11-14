namespace Vendinha.WinForms.Services;

public static class CpfValidator
{
    public static string OnlyDigits(string? value) =>
        new((value ?? string.Empty).Where(char.IsDigit).ToArray());

    public static bool IsValid(string? value)
    {
        string cpf = OnlyDigits(value);
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;

        int first = CalculateDigit(cpf[..9], new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 });
        int second = CalculateDigit(cpf[..9] + first, new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 });
        return cpf.EndsWith($"{first}{second}", StringComparison.Ordinal);
    }

    private static int CalculateDigit(string value, int[] weights)
    {
        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
            sum += (value[i] - '0') * weights[i];

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
