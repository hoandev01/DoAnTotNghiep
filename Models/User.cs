using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace ChickenF.Models
{
    [Table("Users")]
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }
        // 👉 Bổ sung thuộc tính virtual
        public virtual string RoleName => "User";
        
    }

}
