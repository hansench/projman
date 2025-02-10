namespace ProjMan.Application.Repositories.Base;

public interface IRepository<TInfo, TInsert, TUpdate, TId> where TInfo : class where TInsert : class where TUpdate : class
{
    Task<PagedListResponse<TInfo>> FetchPagedListAsync(PagedListRequest param);

    Task<RowResponse<TUpdate>> FetchAsync(TId id);

    Task<RowResponse<TUpdate>> InsertAsync(TInsert dto);

    Task<RowResponse<TUpdate>> UpdateAsync(TUpdate dto);
}
