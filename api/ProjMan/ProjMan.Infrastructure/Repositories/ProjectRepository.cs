namespace ProjMan.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjManDbContext _context;
    private readonly IDbConnection _connection;
    private readonly ICurrentUser _currentUser;

    public ProjectRepository(ProjManDbContext context, IDbConnection connection, ICurrentUser currentUser)
    {
        _context = context;
        _connection = connection;
        _currentUser = currentUser;
    }

    public async Task<RowResponse<ProjectUpdateDto>> FetchAsync(int id)
    {
        var result = new RowResponse<ProjectUpdateDto>();

        var sql = @"SELECT * FROM ""Project"" WHERE ""Id"" = @id ";

        try
        {
            var row = await _connection.QuerySingleOrDefaultAsync<ProjectUpdateDto>(sql, new { id });
            
            result.Row = row ?? new ProjectUpdateDto();
            result.Ok = true;
            result.Message = string.Empty;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Ok = false;
        }

        return result;
    }

    public async Task<PagedListResponse<ProjectInfoDto>> FetchPagedListAsync(PagedListRequest param)
    {
        var result = new PagedListResponse<ProjectInfoDto>();

        if (param.PageSize == 0) param.PageSize = 10;
        if (param.Page == 0) param.Page = 1;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("pageno", param.Page);
            parameters.Add("pagesize", param.PageSize);

            var sqlWhere = " WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                sqlWhere += SqlHelper.GenerateWhere<ProjectInfoDto>();
                parameters.Add("search", $"%{param.Search.Trim().ToLowerInvariant()}%");
            }

            string sqlOrderBy = SqlHelper.GenerateSort(param.SortField, param.SortOrder);
            if (string.IsNullOrWhiteSpace(sqlOrderBy))
            {
                sqlOrderBy = @" ""Id"" ";
            }

            var offset = param.PageSize * (param.Page - 1);
            var sql = @$"SELECT * FROM ""vwProject"" {sqlWhere} ORDER BY {sqlOrderBy} LIMIT {param.PageSize} OFFSET {offset}";
            var list = await _connection.QueryAsync<ProjectInfoDto>(sql, parameters);

            var sqlCount = @$"SELECT COUNT(1) FROM ""vwProject"" {sqlWhere}";
            int recordCount = await _connection.ExecuteScalarAsync<int>(sqlCount, parameters);

            result.Ok = true;
            result.Data = list?.AsList() ?? new List<ProjectInfoDto>();
            result.Total = recordCount;
            result.Message = string.Empty;
        }
        catch (Exception ex)
        {
            result.Ok = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public async Task<RowResponse<ProjectUpdateDto>> InsertAsync(ProjectInsertDto dto)
    {
        var result = new RowResponse<ProjectUpdateDto>();

        // Error: Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported.
        // Note that it's not possible to mix DateTimes with different Kinds in an array, range, or multirange. (Parameter 'value')
        dto.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);

        var projectEntity = new ProjectEntity
        {
            ProjectName = dto.ProjectName,
            ProjectLocation = dto.ProjectLocation,
            ProjectDetails = dto.ProjectDetails,
            StageId = dto.StageId,
            CategoryId = dto.CategoryId,
            CategoryOthersDescr = dto.CategoryOthersDescr,
            StartDate = dto.StartDate,
            InsertedUserId = _currentUser.UserId,
            InsertedUtc = DateTime.UtcNow,
        };

        try
        {
            using (var transaction = await _context.Database.BeginTransactionAsync()) 
            {
                try
                {
                    await _context.ProjectDbSet.AddAsync(projectEntity);
                    await _context.SaveChangesAsync();

                    var projectStageEntity = new ProjectStageDetailEntity
                    {
                        ProjectId = projectEntity.Id,
                        StageId = dto.StageId,
                        Remarks = string.Empty,
                        InsertedUserId = _currentUser.UserId,
                        InsertedUtc = DateTime.UtcNow,
                    };

                    await _context.ProjectStageDetailDbSet.AddAsync(projectStageEntity);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            var row = new ProjectUpdateDto
            {
                Id = projectEntity.Id,
                ProjectName = projectEntity.ProjectName,
                ProjectLocation = projectEntity.ProjectLocation,
                ProjectDetails = projectEntity.ProjectDetails,
                StageId = projectEntity.StageId,
                CategoryId = projectEntity.CategoryId,
                CategoryOthersDescr = projectEntity.CategoryOthersDescr,
                StartDate = projectEntity.StartDate,
            };

            result.Row = row;
            result.Ok = true;
            result.Message = string.Empty;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Ok = false;
        }

        return result;
    }

    public async Task<RowResponse<ProjectUpdateDto>> UpdateAsync(ProjectUpdateDto dto)
    {
        var result = new RowResponse<ProjectUpdateDto>();

        // Error: Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported.
        // Note that it's not possible to mix DateTimes with different Kinds in an array, range, or multirange. (Parameter 'value')
        dto.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);

        try
        {
            var projectEntity = await _context.ProjectDbSet
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync();

            if (projectEntity == null)
            {
                throw new Exception("Project not found");
            }

            var newStage = false;
            if (dto.StageId != projectEntity.StageId)
            {
                newStage = true;
            }

            projectEntity.ProjectName = dto.ProjectName;
            projectEntity.ProjectLocation = dto.ProjectLocation;
            projectEntity.ProjectDetails = dto.ProjectDetails;
            projectEntity.StageId = dto.StageId;
            projectEntity.CategoryId = dto.CategoryId;
            projectEntity.CategoryOthersDescr = dto.CategoryOthersDescr;
            projectEntity.StartDate = dto.StartDate;
            projectEntity.UpdatedUserId = _currentUser.UserId;
            projectEntity.UpdatedUtc = DateTime.UtcNow;

            if (newStage)
            {
                var projectStageEntity = new ProjectStageDetailEntity
                {
                    ProjectId = projectEntity.Id,
                    StageId = dto.StageId,
                    Remarks = string.Empty,
                    InsertedUserId = _currentUser.UserId,
                    InsertedUtc = DateTime.UtcNow,
                };

                await _context.ProjectStageDetailDbSet.AddAsync(projectStageEntity);
            }

            await _context.SaveChangesAsync();

            var row = new ProjectUpdateDto
            {
                Id = projectEntity.Id,
                ProjectName = projectEntity.ProjectName,
                ProjectLocation = projectEntity.ProjectLocation,
                ProjectDetails = projectEntity.ProjectDetails,
                StageId = projectEntity.StageId,
                CategoryId = projectEntity.CategoryId,
                CategoryOthersDescr = projectEntity.CategoryOthersDescr,
                StartDate = projectEntity.StartDate,
            };

            result.Row = row;
            result.Ok = true;
            result.Message = string.Empty;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Ok = false;
        }

        return result;
    }
}
