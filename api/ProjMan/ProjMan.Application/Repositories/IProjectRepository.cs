using ProjMan.Application.Repositories.Base;

namespace ProjMan.Application.Repositories;

public interface IProjectRepository : IRepository<ProjectInfoDto, ProjectInsertDto, ProjectUpdateDto, int>
{
}
