using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWalletSA.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int WalletId { get; set; }

        public Wallet? Wallet { get; set; }
    }
}