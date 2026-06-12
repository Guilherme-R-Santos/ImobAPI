using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImobAPI.Entities;
using ImobAPI.Context;

[Route("[controller]")]
[ApiController]
public class FinalidadeController : ControllerBase
{
    private readonly ImobContext _context;
    public FinalidadeController(ImobContext context)
    {
        _context = context;
    }

    // GET: Finalidade/ObterTodos
    [HttpGet("ObterTodos")]
    public async Task<ActionResult<IEnumerable<Finalidade>>> GetFinalidade()
    {
        return await _context.Finalidades.ToListAsync();
    }

    // GET: Finalidade/ObterPorId/5
    [HttpGet("ObterPorId/{id}")]
    public async Task<ActionResult<Finalidade>> GetFinalidade(int id)
    {
        var finalidade = await _context.Finalidades.FindAsync(id);

        if (finalidade == null)
        {
            return NotFound();
        }

        return finalidade;
    }

    // PUT: Finalidade/Atualizar/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("Atualizar/{id}")]
    public async Task<IActionResult> PutFinalidade(int? id, Finalidade finalidade)
    {
        if (id != finalidade.Id)
        {
            return BadRequest();
        }

        _context.Entry(finalidade).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FinalidadeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: Finalidade/Criar
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("Criar")]
    public async Task<ActionResult<Finalidade>> PostFinalidade(Finalidade finalidade)
    {
        _context.Finalidades.Add(finalidade);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetFinalidade", new { id = finalidade.Id }, finalidade);
    }

    // DELETE: Finalidade/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFinalidade(int? id)
    {
        var finalidade = await _context.Finalidades.FindAsync(id);
        if (finalidade == null)
        {
            return NotFound();
        }

        _context.Finalidades.Remove(finalidade);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FinalidadeExists(int? id)
    {
        return _context.Finalidades.Any(e => e.Id == id);
    }

    // PUT: Finalidade/Inativar/5
    [HttpPut("Inativar/{id}")]
    public async Task<IActionResult> Inactivate(int id)
    {
        var existingFinalidade = _context.Finalidades.Find(id);
        if (existingFinalidade == null)
        {
            return NotFound("Finalidade não encontrada.");
        }
        existingFinalidade.Ativo = false;
        existingFinalidade.DataInativacao = DateTime.Now;
        existingFinalidade.DataAtualizacao = DateTime.Now;
        _context.SaveChanges();
        return Ok(existingFinalidade);
    }
}
