using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.TranslationModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly EduCoreDbContext _db;

        public TranslationRepository(EduCoreDbContext db) => _db = db; 

        public async Task<string?> GetAsync(
            string entityType, int entityId, string field, string lang)
        {
            if (lang == "en") return null;

            return await _db.Translations
                .Where(t => t.EntityType == entityType
                         && t.EntityId == entityId
                         && t.Field == field
                         && t.Lang == lang)
                .Select(t => t.Value)
                .FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(
            string entityType, int entityId, string field, string lang, string value)
        {
            var existing = await _db.Translations.FirstOrDefaultAsync(t =>
                t.EntityType == entityType &&
                t.EntityId == entityId &&
                t.Field == field &&
                t.Lang == lang);

            if (existing != null)
                existing.Value = value;
            else
                await _db.Translations.AddAsync(new Translation
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Field = field,
                    Lang = lang,
                    Value = value
                });

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(string entityType, int entityId)
        {
            var rows = _db.Translations
                .Where(t => t.EntityType == entityType && t.EntityId == entityId);
            _db.Translations.RemoveRange(rows);
            await _db.SaveChangesAsync();
        }
    }
}
