namespace ProjMan.Infrastructure.Repositories;

public class SelectRepository : ISelectRepository
{
    private readonly IDbConnection _connection;

    public SelectRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ListResponse<SelectItemDto>> FetchCategorySelectList()
    {
        var response = new ListResponse<SelectItemDto>();

        try
        {
            var list = new List<SelectItemDto>();
            var sql = @"SELECT ""Id"", ""Name"" FROM ""ProjectCategory"" ORDER BY ""Id"" ";

            var q = await _connection.QueryAsync<SelectItemDto>(sql);
            list.AddRange(q);

            response.Ok = true;
            response.Message = string.Empty;
            response.Data = list.AsList() ?? new List<SelectItemDto>();
        }
        catch (Exception ex)
        {
            response.Ok = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ListResponse<SelectItemDto>> FetchStageSelectList()
    {
        var response = new ListResponse<SelectItemDto>();

        try
        {
            var list = new List<SelectItemDto>();
            var sql = @"SELECT ""Id"", ""Name"" FROM ""ProjectStage"" ORDER BY ""Id"" ";

            var q = await _connection.QueryAsync<SelectItemDto>(sql);
            list.AddRange(q);

            response.Ok = true;
            response.Message = string.Empty;
            response.Data = list.AsList() ?? new List<SelectItemDto>();
        }
        catch (Exception ex)
        {
            response.Ok = false;
            response.Message = ex.Message;
        }

        return response;
    }
}
