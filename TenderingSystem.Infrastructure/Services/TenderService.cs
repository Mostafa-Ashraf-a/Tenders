using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Domain.Enums;
using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.Infrastructure.Services;

public class TenderService : ITenderService
{
    private readonly ITenderRepository _tenderRepository;

    public TenderService(ITenderRepository tenderRepository)
    {
        _tenderRepository = tenderRepository;
    }

    public async Task<IReadOnlyList<TenderDto>> GetAllTendersAsync()
    {
        var tenders = await _tenderRepository.GetAllAsync();
        return tenders.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TenderDto>> GetActiveTendersAsync()
    {
        var tenders = await _tenderRepository.GetActiveTendersAsync();
        return tenders.Select(MapToDto).ToList();
    }

    public async Task<TenderDto?> GetTenderByIdAsync(Guid id)
    {
        var tender = await _tenderRepository.GetByIdAsync(id);
        return tender == null ? null : MapToDto(tender);
    }

    public async Task<TenderDto> CreateTenderAsync(CreateTenderDto dto)
    {
        var tender = new Tender
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            PublishDate = dto.PublishDate,
            ClosingDate = dto.ClosingDate,
            HasAiTargeting = dto.HasAiTargeting,
            Status = TenderStatus.Draft
        };

        await _tenderRepository.AddAsync(tender);
        return MapToDto(tender);
    }

    public async Task<TenderDto?> UpdateTenderAsync(Guid id, UpdateTenderDto dto)
    {
        var tender = await _tenderRepository.GetByIdAsync(id);
        if (tender == null) return null;

        tender.Title = dto.Title;
        tender.Description = dto.Description;
        tender.Category = dto.Category;
        tender.PublishDate = dto.PublishDate;
        tender.ClosingDate = dto.ClosingDate;
        tender.HasAiTargeting = dto.HasAiTargeting;

        if (Enum.TryParse<TenderStatus>(dto.Status, true, out var status))
            tender.Status = status;

        await _tenderRepository.UpdateAsync(tender);
        return MapToDto(tender);
    }

    public async Task<bool> DeleteTenderAsync(Guid id)
    {
        var tender = await _tenderRepository.GetByIdAsync(id);
        if (tender == null) return false;

        await _tenderRepository.DeleteAsync(tender);
        return true;
    }

    private static TenderDto MapToDto(Tender tender) => new()
    {
        Id = tender.Id,
        Title = tender.Title,
        Description = tender.Description,
        Category = tender.Category,
        PublishDate = tender.PublishDate,
        ClosingDate = tender.ClosingDate,
        Status = tender.Status.ToString(),
        HasAiTargeting = tender.HasAiTargeting
    };
}
