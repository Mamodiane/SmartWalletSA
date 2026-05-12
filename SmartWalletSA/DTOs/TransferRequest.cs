using System.ComponentModel.DataAnnotations;

namespace SmartWalletSA.DTOs
{
    public class TransferRequest
    {
        [Required]
        [EmailAddress]
        public string ReceiverEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}