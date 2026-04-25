using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Middlewares
{
    /// <summary>
    /// Hangfire dashboard authorization filter.
    /// 
    /// CURRENT MODE: Open access (anyone can view the dashboard).
    /// This is suitable for development and testing environments.
    /// 
    /// FOR PRODUCTION: Add authentication logic (Admin role check, Basic Auth, 
    /// IP whitelist, etc.) by modifying the Authorize method below.
    /// </summary>
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Open access — dashboard visible to anyone who knows the URL.
            // Safe for local development. For production, restrict access.
            return true;
        }
    }
}
