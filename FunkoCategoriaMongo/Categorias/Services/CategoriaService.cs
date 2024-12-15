using FunkoCategoriaMongo.Categorias.Model;
using FunkoCategoriaMongo.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FunkoCategoriaMongo.Categorias.Services;

public class CategoriaService : ICategoriaService
{
    private const string CacheKeyPrefix = "_Categoria";
    private readonly IMongoCollection<Categoria> _categoriasCollection;
    private readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;

    public CategoriaService(IOptions<CategoriaMongoConfig> categoriaDatabaseConfig, ILogger<CategoriaService> logger,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        var mongoClient = new MongoClient(categoriaDatabaseConfig.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(categoriaDatabaseConfig.Value.DatabaseName);
        _categoriasCollection =
            mongoDatabase.GetCollection<Categoria>(categoriaDatabaseConfig.Value.CategoriaCollectionName);
    }
    
    public async Task<List<Categoria>> GetAllAsync()
    {
        _logger.LogInformation("Obteniendo todas las categorias de la base de datos");

        return await _categoriasCollection.Find(categorias => true).ToListAsync();
    }

    public async Task<Categoria?> GetByIdAsync(string id)
    {
        _logger.LogInformation($"Obteniendo categoria de la base de datos por id {id}");
        var cacheKey = _memoryCache + id;

        if (_memoryCache.TryGetValue(cacheKey, out Categoria? cachedCategoria))
        {
            _logger.LogInformation("Obteniendo categoria de la cache");
            return cachedCategoria;
        }
        
        _logger.LogInformation("Obteniendo categoria de la base de datos");
        var categoriaBd = await _categoriasCollection.Find(categoria => categoria.Id == id).FirstOrDefaultAsync();

        if (categoriaBd != null)
        {
            _memoryCache.Set(cacheKey, categoriaBd, TimeSpan.FromSeconds(30));
            _logger.LogInformation("Metiendo la categoria en la cache");
        }
        
        return categoriaBd;
    }

    public async Task<Categoria> CreateAsync(Categoria categoria)
    {
        _logger.LogInformation("Creando una categoria");
        categoria.Id = ObjectId.GenerateNewId().ToString();
        var timeStamp = DateTime.Now;
        categoria.CreatedAt = timeStamp;
        categoria.UpdatedAt = timeStamp;

        await _categoriasCollection.InsertOneAsync(categoria);
        
        _logger.LogInformation($"Categoría creada con éxito con id: {categoria.Id}");
        
        return categoria;
    }

    public async Task<Categoria?> UpdateAsync(string id, Categoria categoria)
    {
        _logger.LogInformation($"Actualizado categoria: {categoria.Id}");
        
        categoria.Id = id;
        categoria.UpdatedAt = DateTime.Now;

        var categoriaActualizada = await _categoriasCollection.FindOneAndReplaceAsync<Categoria>(
            _categoria => categoria.Id == id
            , categoria,
            new FindOneAndReplaceOptions<Categoria> { ReturnDocument = ReturnDocument.After }
            );

        var cacheKey = CacheKeyPrefix + id;

        if (categoriaActualizada!=null)
        {
            _memoryCache.Remove(cacheKey);
            _logger.LogInformation("Eliminando categoria de la cache");
        }
        
        _logger.LogInformation($"Categoria actualizada con éxito");
        
        return categoriaActualizada;
    }

    public async Task<Categoria?> DeleteAsync(string id)
    {
        _logger.LogInformation($"Eliminando categoria con id: {id}");

        var categoriaEliminada = await _categoriasCollection.FindOneAndDeleteAsync(_categoria => _categoria.Id == id);
        
        var cacheKey = CacheKeyPrefix + id;

        if (categoriaEliminada != null)
        {
            _logger.LogInformation("Eliminando categoria de la cache");
            _memoryCache.Remove(cacheKey);
        }
        
        _logger.LogInformation($"Categoría eliminada con éxito");
        
        return categoriaEliminada;
    }
}