using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vendinha.WinForms.Models;

public sealed class Divida
{
    [Key]
    public int Id { get; set; }

    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorPago { get; set; } = 0m;

    [NotMapped]
    public decimal ValorRestante => Math.Max(0m, Valor - ValorPago);

    public bool Paga { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataPagamento { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente? Cliente { get; set; }
}
