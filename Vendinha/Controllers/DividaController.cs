
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Contexts;
using Vendinha.Models;

namespace Vendinha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DividaController : ControllerBase
    {
        private readonly VendinhaDbContext _context;

        public DividaController(VendinhaDbContext context)
        {
            _context = context;
        }

        // LISTAR DIVIDAS DE UM CLIENTE
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> Listar(int clienteId)
        {
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Id == clienteId);

            if (!clienteExiste)
                return NotFound("Cliente não encontrado.");

            var dividas = await _context.Dividas
                .Where(d => d.ClienteId == clienteId)
                .Select(d => new
                {
                    d.Id,
                    d.Valor,
                    ValorPago = d.Paga ? d.Valor : d.ValorPago,
                    ValorRestante = d.Paga ? 0m : d.Valor - d.ValorPago,
                    d.Paga,
                    d.DataCriacao,
                    d.DataPagamento
                })
                .ToListAsync();

            return Ok(dividas);
        }

        // CADASTRAR DIVIDA
        [HttpPost("cliente/{clienteId}")]
        public async Task<IActionResult> Cadastrar(
            int clienteId, Divida divida)
        {
            var cliente = await _context.Clientes
                .FindAsync(clienteId);

            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            if (divida.Valor <= 0)
                return BadRequest("O valor deve ser maior que zero.");

            bool possuiDividaAberta = await _context.Dividas
                .AnyAsync(d =>
                    d.ClienteId == clienteId && !d.Paga);

            if (possuiDividaAberta)
                return BadRequest(
                    "Este cliente já possui uma dívida em aberto.");

            divida.Id = 0;
            divida.ClienteId = clienteId;
            divida.ValorPago = 0;
            divida.Paga = false;
            divida.DataCriacao = DateTime.Now;
            divida.DataPagamento = null;

            _context.Dividas.Add(divida);

            await _context.SaveChangesAsync();

            return Ok("Dívida cadastrada com sucesso!");
        }

        // REGISTRAR PAGAMENTO PARCIAL
        [HttpPut("{id}/pagamento")]
        public async Task<IActionResult> RegistrarPagamento(
            int id, [FromBody] PagamentoRequest pagamento)
        {
            var divida = await _context.Dividas.FindAsync(id);

            if (divida == null)
                return NotFound("Dívida não encontrada.");

            if (divida.Paga)
                return BadRequest("Esta dívida já foi paga.");

            if (pagamento.Valor <= 0)
                return BadRequest(
                    "O valor do pagamento deve ser maior que zero.");

            decimal valorRestante = divida.Valor - divida.ValorPago;

            if (pagamento.Valor > valorRestante)
                return BadRequest(
                    "O pagamento não pode ser maior que o valor restante.");

            divida.ValorPago += pagamento.Valor;

            if (divida.ValorPago >= divida.Valor)
            {
                divida.ValorPago = divida.Valor;
                divida.Paga = true;
                divida.DataPagamento = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = divida.Paga
                    ? "Dívida totalmente paga!"
                    : "Pagamento parcial registrado com sucesso!",

                ValorOriginal = divida.Valor,
                ValorPago = divida.ValorPago,
                ValorRestante = divida.ValorRestante,
                Paga = divida.Paga
            });
        }

        // QUITAR DIVIDA INTEIRA
        [HttpPut("{id}/pagar")]
        public async Task<IActionResult> Pagar(int id)
        {
            var divida = await _context.Dividas.FindAsync(id);

            if (divida == null)
                return NotFound("Dívida não encontrada.");

            if (divida.Paga)
                return BadRequest("Esta dívida já foi paga.");

            divida.ValorPago = divida.Valor;
            divida.Paga = true;
            divida.DataPagamento = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("Dívida paga com sucesso!");
        }
    }

    // DADOS RECEBIDOS NO PAGAMENTO
    public class PagamentoRequest
    {
        public decimal Valor { get; set; }
    }
}
