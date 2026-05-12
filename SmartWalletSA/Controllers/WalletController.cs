using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWalletSA.Data;
using System.Security.Claims;
using SmartWalletSA.DTOs;
using SmartWalletSA.Models;


namespace SmartWalletSA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return NotFound("Wallet not found.");
            }

            return Ok(new
            {
                wallet.Id,
                wallet.Balance,
                User = new
                {
                    wallet.User!.Id,
                    wallet.User.FullName,
                    wallet.User.Email
                }
            });
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return NotFound("Wallet not found.");
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

            return Ok(new
            {
                message = "Deposit successful.",
                walletId = wallet.Id,
                amount = request.Amount,
                newBalance = wallet.Balance
            });
        }
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return NotFound("Wallet not found.");
            }

            if (wallet.Balance < request.Amount)
            {
                return BadRequest("Insufficient funds.");
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

            return Ok(new
            {
                message = "Withdrawal successful.",
                walletId = wallet.Id,
                amount = request.Amount,
                newBalance = wallet.Balance
            });
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int senderUserId = int.Parse(userIdClaim.Value);

            var senderWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == senderUserId);

            if (senderWallet == null)
            {
                return NotFound("Sender wallet not found.");
            }

            var receiver = await _context.Users
                .Include(u => u.Wallet)
                .FirstOrDefaultAsync(u => u.Email == request.ReceiverEmail);

            if (receiver == null || receiver.Wallet == null)
            {
                return NotFound("Receiver wallet not found.");
            }

            if (receiver.Id == senderUserId)
            {
                return BadRequest("You cannot transfer money to yourself.");
            }

            if (senderWallet.Balance < request.Amount)
            {
                return BadRequest("Insufficient funds.");
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

                return Ok(new
                {
                    message = "Transfer successful.",
                    amount = request.Amount,
                    senderWalletId = senderWallet.Id,
                    receiverWalletId = receiver.Wallet.Id,
                    newBalance = senderWallet.Balance
                });
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                return StatusCode(500, "Transfer failed. No money was moved.");
            }
        }
    }
}