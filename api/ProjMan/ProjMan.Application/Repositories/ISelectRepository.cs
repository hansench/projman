namespace ProjMan.Application.Repositories;

public interface ISelectRepository
{
    Task<ListResponse<SelectItemDto>> FetchStageSelectList();

    Task<ListResponse<SelectItemDto>> FetchCategorySelectList();
}
