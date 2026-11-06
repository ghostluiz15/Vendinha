using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Contexts;
using Vendinha.Models;

namespace Vendinha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly VendinhaDbContext _context;

        public ClienteController(VendinhaDbContext context)
        {
            _context = context;
        }

        // LISTAR CLIENTES


        [HttpGet]
        public async Task<IActionResult> Listar(string? busca, int pagina = 1)
        {
            if (pagina < 1)
                return BadRequest("A página deve ser maior que zero.");

            // Busca os clientes
            var clientesQuery = _context.Clientes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                clientesQuery = clientesQuery.Where(c =>
                    c.NomeCompleto.Contains(busca));
            }

            var clientes = await clientesQuery.ToListAsync();

            // Busca as dívidas em aberto
            var dividas = await _context.Dividas
                .AsNoTracking()
                .Where(d => !d.Paga)
                .Select(d => new
                {
                    d.ClienteId,
                    d.Valor
                })
                .ToListAsync();

            // Faz a soma no C#, evitando o erro do SQLite
            var totaisPorCliente = dividas
                .GroupBy(d => d.ClienteId)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(d => d.Valor)
                );

            var resultado = clientes
                .Select(c => new
                {
                    c.Id,
                    c.NomeCompleto,
                    c.CPF,
                    c.DataNascimento,
                    c.Email,
                    c.Idade,
                    TotalDividas = totaisPorCliente.ContainsKey(c.Id)
                        ? totaisPorCliente[c.Id]
                        : 0m
                })
                .OrderByDescending(c => c.TotalDividas)
                .ToList();

            int totalClientes = resultado.Count;
            int porPagina = 10;

            var clientesPaginados = resultado
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToList();

            return Ok(new
            {
                Pagina = pagina,
                TotalClientes = totalClientes,
                TotalPaginas = (int)Math.Ceiling(
                    totalClientes / (double)porPagina),
                TotalDividasGeral = resultado.Sum(c => c.TotalDividas),
                Clientes = clientesPaginados
            });
        }



        // BUSCAR CLIENTE POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            return Ok(cliente);
        }

        // CADASTRAR CLIENTE
        [HttpPost]
        public async Task<IActionResult> Cadastrar(Cliente cliente)
        {
            var cpf = new string(cliente.CPF
                .Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return BadRequest("CPF deve conter 11 números.");

            cliente.CPF = cpf;

            bool cpfExiste = await _context.Clientes
                .AnyAsync(c => c.CPF == cliente.CPF);

            if (cpfExiste)
                return BadRequest("Este CPF já está cadastrado.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = cliente.Id },
                cliente
            );
        }

        // ATUALIZAR CLIENTE
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id, Cliente dados)
        {
            var cliente = await _context.Clientes
                .FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            var cpf = new string(dados.CPF
                .Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return BadRequest("CPF deve conter 11 números.");

            bool cpfExiste = await _context.Clientes
                .AnyAsync(c => c.CPF == cpf && c.Id != id);

            if (cpfExiste)
                return BadRequest("Este CPF já está cadastrado.");

            cliente.NomeCompleto = dados.NomeCompleto;
            cliente.CPF = cpf;
            cliente.DataNascimento = dados.DataNascimento;
            cliente.Email = dados.Email;

            if (!TryValidateModel(cliente))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // EXCLUIR CLIENTE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var cliente = await _context.Clientes
                .FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            // Verifica se o cliente possui dívidas registradas
            bool possuiDividas = await _context.Dividas
                .AnyAsync(d => d.ClienteId == id);

            if (possuiDividas)
                return BadRequest(
                    "Não é possível excluir um cliente que possui histórico de dívidas.");

            _context.Clientes.Remove(cliente);

            await _context.SaveChangesAsync();

            return Ok("Cliente excluído com sucesso.");
        }

    }
}
