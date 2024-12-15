using FunkoCategoriaMongo.Categorias.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FunkoCategoriaMongo.Categorias.Controller;

[ApiController]
[Route("api/categorias")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;
    
    public CategoriaController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Categoria>>> GetAll()
    {
        var categorias = await _categoriaService.GetAllAsync();

        return Ok(categorias);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Categoria>> GetById(string id)
    {
        var categoria = await _categoriaService.GetByIdAsync(id);

        if (categoria is null)
        {
            return NotFound();
        }
        
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<Categoria>> Create(Categoria categoria)
    {
        var categoriaCreate = await _categoriaService.CreateAsync(categoria);
        
        return CreatedAtAction(nameof(GetById), new {id = categoria.Id}, categoriaCreate);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, [FromBody] Categoria categoria)
    {
        var categoriaUpdated = await _categoriaService.UpdateAsync(id, categoria);

        if (categoriaUpdated is null)
        {
            return NotFound();
        }
        
        return Ok(categoriaUpdated);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var categoriaDeleted = await _categoriaService.DeleteAsync(id);

        if (categoriaDeleted is null)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}