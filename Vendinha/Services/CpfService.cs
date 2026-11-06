
using System.Linq;

namespace Vendinha.Services
{
    public class CpfService
    {
        public static bool Validar(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int[] numeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += numeros[i] * (10 - i);

            int resto = soma % 11;
            int primeiroDigito = resto < 2 ? 0 : 11 - resto;

            if (numeros[9] != primeiroDigito)
                return false;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += numeros[i] * (11 - i);

            resto = soma % 11;
            int segundoDigito = resto < 2 ? 0 : 11 - resto;

            return numeros[10] == segundoDigito;
        }
    }
}
