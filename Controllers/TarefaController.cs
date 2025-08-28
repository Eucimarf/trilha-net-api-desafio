using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // TODO: Buscar o Id no banco utilizando o EF
            var tarefa = _context.Tarefas.Find(id);
            // TODO: Validar o tipo de retorno. Se não encontrar a tarefa, retornar NotFound,
            if (tarefa == null)
                return NotFound();
            // caso contrário retornar OK com a tarefa encontrada
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // TODO: Buscar todas as tarefas no banco utilizando o EF
            var tarefa = _context.Tarefas.ToList();
            return Ok(tarefa);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            // TODO: Buscar  as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
            var tarefa = _context.Tarefas.Where(x => x.Titulo.ToUpper().Contains(titulo.ToUpper())).ToList();
            // Dica: Usar como exemplo o endpoint ObterPorData
            return Ok(tarefa);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // TODO: Buscar  as tarefas no banco utilizando o EF, que contenha o status recebido por parâmetro
            var tarefa = _context.Tarefas.Where(x => x.Status == status);
            // Dica: Usar como exemplo o endpoint ObterPorData
            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // TODO: Adicionar a tarefa recebida no EF e salvar as mudanças (save changes)
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaDb = _context.Tarefas.Find(id);

            if (tarefaDb == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // TODO: Atualizar as informações da variável tarefaBanco com a tarefa recebida via parâmetro
            tarefaDb.Titulo = tarefa.Titulo;
            tarefaDb.Descricao = tarefa.Descricao;
            tarefaDb.Data = tarefa.Data;
            tarefaDb.Status = tarefa.Status;
            // TODO: Atualizar a variável tarefaBanco no EF e salvar as mudanças (save changes)
            _context.Tarefas.Update(tarefaDb);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaDb = _context.Tarefas.Find(id);

            if (tarefaDb == null)
                return NotFound();

            // TODO: Remover a tarefa encontrada através do EF e salvar as mudanças (save changes)
            _context.Tarefas.Remove(tarefaDb);
            _context.SaveChanges();
            return NoContent();
        }

        //Novos Endpoints Implementados

        [HttpGet("ObterPendentes")]
        public IActionResult ObterPendentes()
        {
            var tarefas = _context.Tarefas.Where(t => t.Status == EnumStatusTarefa.Pendente).ToList();
            return Ok(tarefas);
        }

        [HttpGet("ObterFinalizadas")]
        public IActionResult ObterFinalizadas()
        {
            var tarefas = _context.Tarefas.Where(t => t.Status == EnumStatusTarefa.Finalizado).ToList();
            return Ok(tarefas);
        }

        [HttpPut("AtualizarStatus/{id}")]
        public IActionResult AtualizarStatus(int id, [FromBody] EnumStatusTarefa novoStatus)
        {
            var tarefaDb = _context.Tarefas.Find(id);

            if (tarefaDb == null)
            {
                return NotFound();
            }
            
            try
            {
                tarefaDb.Status = novoStatus;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = "Ocorreu um erro ao atualizar o status da tarefa.", Detalhes = ex.Message });
            }

            return Ok();
        }
    }
}
