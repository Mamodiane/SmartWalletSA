using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWalletSA.Models
{
    public class Wallet
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
}