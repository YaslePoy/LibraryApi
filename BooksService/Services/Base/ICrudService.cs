using LibApi.Model;

namespace LibApi.Services.Base;

public interface ICrudService<TEntity, TData> : ICrudService<TEntity, TData, TData>;

public interface ICrudService<TEntity, TCreate, TUpdate> : IExistable
{
    Task<int> Create(TCreate data);
    TEntity Get(int id);
    Task Update(int id, TUpdate data);
    Task Delete(int id);
}