using FunkoCategoriaMongo.Categorias.Model;

public interface ICategoriaService
{
    public Task<List<Categoria>> GetAllAsync();
    public Task<Categoria?> GetByIdAsync(string id);
    public Task<Categoria> CreateAsync(Categoria categoria);
    public Task<Categoria?> UpdateAsync(string id, Categoria categoria);
    public Task<Categoria?> DeleteAsync(string id);
}