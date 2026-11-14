using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vendinha.WinForms.Models;

public sealed class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required, StringLength(11, MinimumLength = 11)]
    public string CPF { get; set; } = string.Empty;

    [Required]
    public DateTime DataNascimento { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [NotMapped]
    public int Idade
    {
        get
        {
            var hoje = DateTime.Today;
            int idade = hoje.Year - DataNascimento.Year;
            if (DataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }
}
