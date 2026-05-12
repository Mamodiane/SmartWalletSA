using System.ComponentModel.DataAnnotations;

namespace SmartWalletSA.DTOs
{
    public class WithdrawRequest
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}