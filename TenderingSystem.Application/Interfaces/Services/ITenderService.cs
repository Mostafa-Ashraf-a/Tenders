using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.Application.Interfaces.Services;

public interface ITenderService
{
    Task<IReadOnlyList<TenderDto>> GetAllTendersAsync();
    Task<IReadOnlyList<TenderDto>> GetActiveTendersAsync();
    Task<TenderDto?> GetTenderByIdAsync(Guid id);
    Task<TenderDto> CreateTenderAsync(CreateTenderDto dto);
    Task<TenderDto?> UpdateTenderAsync(Guid id, UpdateTenderDto dto);
    Task<bool> DeleteTenderAsync(Guid id);
}
