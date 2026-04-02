using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public record RegisterDto(
        string Name,
        string UserName,
        [EmailAddress] string Email,
        string Password,
        [Phone] string PhoneNumber
       
        );
    
}
