using Microsoft.EntityFrameworkCore;
using SmartWalletSA.Data;
using SmartWalletSA.DTOs;
using SmartWalletSA.Models;

namespace SmartWalletSA.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;

        public WalletService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object?> GetWalletAsync(int userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return null;
            }

            return new
            {
                wallet.Id,
                wallet.Balance,
                User = new
                {
                    wallet.User!.Id,
                    wallet.User.FullName,
                    wallet.User.Email
                }
            };
        }

        public async Task<object> DepositAsync(int userId, DepositRequest request)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }

            wallet.Balance += request.Amount;

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Type = "Deposit",
                Amount = request.Amount
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new
            {
                message = "Deposit successful.",
                walletId = wallet.Id,
                amount = request.Amount,
                newBalance = wallet.Balance
            };
        }

        public async Task<object> WithdrawAsync(int userId, WithdrawRequest request)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }

            if (wallet.Balance < request.Amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            wallet.Balance -= request.Amount;

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Type = "Withdraw",
                Amount = request.Amount
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new
            {
                message = "Withdrawal successful.",
                walletId = wallet.Id,
                amount = request.Amount,
                newBalance = wallet.Balance
            };
        }

        public async Task<object> TransferAsync(int senderUserId, TransferRequest request)
        {
            var senderWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == senderUserId);

            if (senderWallet == null)
            {
                throw new Exception("Sender wallet not found.");
            }

            var receiver = await _context.Users
                .Include(u => u.Wallet)
                .FirstOrDefaultAsync(u => u.Email == request.ReceiverEmail);

            if (receiver == null || receiver.Wallet == null)
            {
                throw new Exception("Receiver wallet not found.");
            }

            if (receiver.Id == senderUserId)
            {
                throw new InvalidOperationException("You cannot transfer money to yourself.");
            }

            if (senderWallet.Balance < request.Amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                senderWallet.Balance -= request.Amount;
                receiver.Wallet.Balance += request.Amount;

                var senderTransaction = new Transaction
                {
                    WalletId = senderWallet.Id,
                    Type = "Transfer Out",
                    Amount = request.Amount
                };

                var receiverTransaction = new Transaction
                {
                    WalletId = receiver.Wallet.Id,
                    Type = "Transfer In",
                    Amount = request.Amount
                };

                _context.Transactions.Add(senderTransaction);
                _context.Transactions.Add(receiverTransaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return new
                {
                    message = "Transfer successful.",
                    amount = request.Amount,
                    senderWalletId = senderWallet.Id,
                    receiverWalletId = receiver.Wallet.Id,
                    newBalance = senderWallet.Balance
                };
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}