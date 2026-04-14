using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface ITranslationRepository
    {
        Task<string?> GetAsync(string entityType, int entityId, string field, string lang);
        Task UpsertAsync(string entityType, int entityId, string field, string lang, string value);
        Task DeleteAllAsync(string entityType, int entityId);
    }
}
