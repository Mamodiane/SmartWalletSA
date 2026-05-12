using SmartWalletSA.DTOs;

namespace SmartWalletSA.Services
{
    public interface IWalletService
    {
        Task<object?> GetWalletAsync(int userId);
        Task<object> DepositAsync(int userId, DepositRequest request);
        Task<object> WithdrawAsync(int userId, WithdrawRequest request);
        Task<object> TransferAsync(int senderUserId, TransferRequest request);
    }
}