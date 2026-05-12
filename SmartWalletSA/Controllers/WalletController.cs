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
    }
}