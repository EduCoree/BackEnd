using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.CommonResult
{
    public class Error
    {
        private Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        public string Code { get; set; }
        public string Description { get; set; } 
        
        public ErrorType Type { get; set; }
        #region factory methods
        //static factory method to create error instances
        public static Error Failure ( string code="General.Failure", string description="General Failure has Occurred")
        {
            return new Error(code, description, ErrorType.Failure);
        }
        public static Error Validation(string code = "General.Validation", string description = "A validation error has occurred")
        {
            return new Error(code, description, ErrorType.Validation);
        }

        public static Error NotFound(string code = "General.NotFound", string description = "The requested resource was not found")
        {
            return new Error(code, description, ErrorType.NotFound);
        }

        public static Error Unauthorized(string code = "General.Unauthorized", string description = "Unauthorized access. Authentication is required")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }

        public static Error Forbidden(string code = "General.Forbidden", string description = "Access to this resource is forbidden")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }

        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "The provided credentials are invalid")
        {
            return new Error(code, description, ErrorType.InvalidCredentials);
        }
        #endregion
    }
}
