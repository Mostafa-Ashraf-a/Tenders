using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Domain.Enums;
using TenderingSystem.Shared.Models.Bids;

namespace TenderingSystem.Infrastructure.Services;

public class BidService : IBidService
{
    private readonly IBidRepository _bidRepository;
    private readonly ITenderRepository _tenderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IEmailService _emailService;

    public BidService(IBidRepository bidRepository, ITenderRepository tenderRepository, ISupplierRepository supplierRepository, IEmailService emailService)
    {
        _bidRepository = bidRepository;
        _tenderRepository = tenderRepository;
        _supplierRepository = supplierRepository;
        _emailService = emailService;
    }

    public async Task<IReadOnlyList<BidDto>> GetBidsByTenderIdAsync(Guid tenderId)
    {
        var bids = await _bidRepository.GetBidsByTenderIdAsync(tenderId);
        return bids.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<BidDto>> GetBidsBySupplierIdAsync(Guid supplierId)
    {
        // Need to add this method or just use Generic repo GetAsync
        var bids = await _bidRepository.GetAsync(b => b.SupplierId == supplierId);
        return bids.Select(MapToDto).ToList();
    }

    public async Task<BidDto> SubmitBidAsync(Guid supplierId, CreateBidDto dto)
    {
        var tender = await _tenderRepository.GetByIdAsync(dto.TenderId);
        if (tender == null)
            throw new Exception("Tender not found.");

        if (tender.Status != TenderStatus.Published)
            throw new Exception("Cannot bid on a tender that is not currently published.");

        if (DateTime.UtcNow > tender.ClosingDate)
            throw new Exception("The closing date for this tender has passed.");

        var existingBids = await _bidRepository.GetAsync(b => b.TenderId == dto.TenderId && b.SupplierId == supplierId);
        if (existingBids.Any())
            throw new Exception("لقد قمت بتقديم عرض بالفعل لهذا العطاء.");

        var newBid = new Bid
        {
            TenderId = dto.TenderId,
            SupplierId = supplierId,
            SubmittedPrice = dto.SubmittedPrice,
            TechnicalProposalUrl = dto.TechnicalProposalUrl,
            Status = BidStatus.Submitted
        };

        var createdBid = await _bidRepository.AddAsync(newBid);

        var savedBid = await _bidRepository.GetByIdAsync(createdBid.Id);
        var supplier = await _supplierRepository.GetByIdAsync(supplierId);
        
        return new BidDto
        {
            Id = createdBid.Id,
            TenderId = createdBid.TenderId,
            TenderTitle = tender.Title,
            SupplierId = createdBid.SupplierId,
            SupplierName = supplier?.CompanyName ?? string.Empty,
            SubmittedPrice = createdBid.SubmittedPrice,
            TechnicalProposalUrl = createdBid.TechnicalProposalUrl,
            SubmissionDate = createdBid.SubmissionDate,
            Status = createdBid.Status.ToString()
        };
    }

    private static BidDto MapToDto(Bid b) => new BidDto
    {
        Id = b.Id,
        TenderId = b.TenderId,
        TenderTitle = b.Tender?.Title ?? string.Empty,
        SupplierId = b.SupplierId,
        SupplierName = b.Supplier?.CompanyName ?? string.Empty,
        SubmittedPrice = b.SubmittedPrice,
        TechnicalProposalUrl = b.TechnicalProposalUrl,
        SubmissionDate = b.SubmissionDate,
        Status = b.Status.ToString()
    };

    public async Task<bool> HasSupplierBidAsync(Guid tenderId, Guid supplierId)
    {
        var existingBids = await _bidRepository.GetAsync(b => b.TenderId == tenderId && b.SupplierId == supplierId);
        return existingBids.Any();
    }

    public async Task AwardBidAsync(Guid bidId)
    {
        var acceptedBid = await _bidRepository.GetByIdAsync(bidId);
        if (acceptedBid == null)
            throw new Exception("Bid not found.");

        var tender = await _tenderRepository.GetByIdAsync(acceptedBid.TenderId);
        if (tender == null)
            throw new Exception("Tender not found.");

        var allBids = await _bidRepository.GetBidsByTenderIdAsync(acceptedBid.TenderId);
        
        foreach (var bid in allBids)
        {
            if (bid.Id == bidId)
            {
                bid.Status = BidStatus.Accepted;
                if (bid.Supplier != null && !string.IsNullOrWhiteSpace(bid.Supplier.Email))
                {
                    var subject = $"تهانينا! فوزكم بمناقصة: {tender.Title}";
                    var body = $@"
                        <h3>عزيزي المورد {bid.Supplier.CompanyName}،</h3>
                        <p>نهنئكم على اختيار عرضكم وفوزكم بمناقصة <strong>{tender.Title}</strong>.</p>
                        <p>يرجى التواصل مع الإدارة لاستكمال إجراءات الترسية.</p>
                        <p>مع تحيات نظام المناقصات.</p>";
                    await _emailService.SendEmailAsync(bid.Supplier.Email, subject, body);
                }
            }
            else
            {
                bid.Status = BidStatus.Rejected;
                if (bid.Supplier != null && !string.IsNullOrWhiteSpace(bid.Supplier.Email))
                {
                    var subject = $"تحديث بخصوص مناقصة: {tender.Title}";
                    var body = $@"
                        <h3>عزيزي المورد {bid.Supplier.CompanyName}،</h3>
                        <p>نشكركم على اهتمامكم ومشاركتكم في العطاء الخاص بمناقصة <strong>{tender.Title}</strong>.</p>
                        <p>نود إعلامكم بأنه تم اختيار عرض آخر لهذه المناقصة. نتمنى لكم حظاً أوفر في المناقصات القادمة.</p>
                        <p>مع تحيات نظام المناقصات.</p>";
                    await _emailService.SendEmailAsync(bid.Supplier.Email, subject, body);
                }
            }
            await _bidRepository.UpdateAsync(bid);
        }

        tender.Status = TenderStatus.Awarded;
        await _tenderRepository.UpdateAsync(tender);
    }
}
