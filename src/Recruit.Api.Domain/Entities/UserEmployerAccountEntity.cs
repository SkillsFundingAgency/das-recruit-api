using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class UserEmployerAccountEntity
{
    [Key]
    [Column(Order = 1)]
    public Guid UserId { get; set; }
    
    [Key]
    [Column(Order = 2)]
    public string EmployerAccountId { get; set; }
    
    public virtual UserEntity User { get; set; }
}