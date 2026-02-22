using TenderingSystem.Shared.Models.Bids;

namespace TenderingSystem.Application.Interfaces.Services;

public interface IBidService
{
    Task<IReadOnlyList<BidDto>> GetBidsByTenderIdAsync(Guid tenderId);
    Task<IReadOnlyList<BidDto>> GetBidsBySupplierIdAsync(Guid supplierId);
    Task<BidDto> SubmitBidAsync(Guid supplierId, CreateBidDto dto);
    Task<bool> HasSupplierBidAsync(Guid tenderId, Guid supplierId);
    Task AwardBidAsync(Guid bidId);
}
