using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Domain.Enums;
using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.Infrastructure.Services;

public class TenderService : ITenderService
{
    private readonly ITenderRepository _tenderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IEmailService _emailService;

    public TenderService(
        ITenderRepository tenderRepository,
        ISupplierRepository supplierRepository,
        IEmailService emailService)
    {
        _tenderRepository = tenderRepository;
        _supplierRepository = supplierRepository;
        _emailService = emailService;
    }

    public async Task<IReadOnlyList<TenderDto>> GetAllTendersAsync()
    {
        // Ideally we should eagerly load Category here if we had custom repo method, 
        // but for now we'll rely on what we have or add Include in repo if needed.
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
            CategoryId = dto.CategoryId,
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
        tender.CategoryId = dto.CategoryId;
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

    public async Task<bool> PublishTenderAsync(Guid id)
    {
        var tender = await _tenderRepository.GetByIdAsync(id);
        if (tender == null || tender.Status != TenderStatus.Draft) return false;

        tender.Status = TenderStatus.Published;
        tender.PublishDate = DateTime.UtcNow;
        
        await _tenderRepository.UpdateAsync(tender);

        // Notify suppliers in the same category
        if (tender.CategoryId.HasValue)
        {
            // Note: We'll fetch suppliers for this category. To do this properly, 
            // you might want a specific repository method. For now, we'll fetch all 
            // and filter, or assume the repository has navigation properties loaded.
            
            // For efficiency, usually we'd add `GetSuppliersByCategoryIdAsync` to ISupplierRepository.
            // Let's call it here assuming we will add it next.
            var suppliers = await _supplierRepository.GetAsync(s => s.SupplierCategories.Any(sc => sc.CategoryId == tender.CategoryId));
            
            foreach (var supplier in suppliers)
            {
                if (!string.IsNullOrEmpty(supplier.Email))
                {
                    var subject = $"مناقصة جديدة في مجال اهتمامك: {tender.Title}";
                    var body = $@"
                        <h3>مرحباً {supplier.CompanyName}،</h3>
                        <p>تم نشر مناقصة جديدة توافق مجال عملكم.</p>
                        <p><strong>العنوان:</strong> {tender.Title}</p>
                        <p><strong>آخر موعد للتقديم:</strong> {tender.ClosingDate.ToString("yyyy-MM-dd")}</p>
                        <br/>
                        <p>مع تحيات،<br/>نظام إدارة المناقصات</p>
                    ";
                    
                    try
                    {
                        await _emailService.SendEmailAsync(supplier.Email, subject, body, true);
                    }
                    catch
                    {
                        // Log error optionally, but don't fail the publishing process
                    }
                }
            }
        }

        return true;
    }

    private static TenderDto MapToDto(Tender tender) => new()
    {
        Id = tender.Id,
        Title = tender.Title,
        Description = tender.Description,
        CategoryId = tender.CategoryId,
        CategoryName = tender.Category?.Name ?? string.Empty,
        PublishDate = tender.PublishDate,
        ClosingDate = tender.ClosingDate,
        Status = tender.Status.ToString(),
        HasAiTargeting = tender.HasAiTargeting
    };
}
