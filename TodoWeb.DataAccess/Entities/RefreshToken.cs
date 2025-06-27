using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; } // đánh dấu token đã bị thu hồi hay chưa
        public int UserId { get; set; }
        public User User { get; set; } // chỉ ra người dùng sở hữu token này
    }
}
