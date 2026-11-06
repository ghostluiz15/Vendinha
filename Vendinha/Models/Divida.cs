
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vendinha.Models
{
    public class Divida
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        // Valor que já foi pago
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorPago { get; set; } = 0;

        // Valor que ainda falta pagar
        [NotMapped]
        public decimal ValorRestante => Valor - ValorPago;

        public bool Paga { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataPagamento { get; set; }

        // Relacionamento com o cliente
        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
    }
}

