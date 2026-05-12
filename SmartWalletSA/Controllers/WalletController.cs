using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWalletSA.DTOs;
using SmartWalletSA.Services;
using System.Security.Claims;

namespace SmartWalletSA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
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

            var wallet = await _walletService.GetWalletAsync(userId);

            if (wallet == null)
            {
                return NotFound("Wallet not found.");
            }

            return Ok(wallet);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                int userId = int.Parse(userIdClaim.Value);

                var result = await _walletService.DepositAsync(userId, request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                int userId = int.Parse(userIdClaim.Value);

                var result = await _walletService.WithdrawAsync(userId, request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                int senderUserId = int.Parse(userIdClaim.Value);

                var result = await _walletService.TransferAsync(senderUserId, request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}