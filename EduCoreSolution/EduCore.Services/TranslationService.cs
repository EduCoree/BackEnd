using EduCore.Domain.Contracts.Repositories;

namespace EduCore.Services
{
    public class TranslationService
    {
        private readonly ITranslationRepository _repo;

        public TranslationService(ITranslationRepository repo) => _repo = repo;

        public Task<string?> GetAsync(
            string entityType, int entityId, string field, string lang)
            => _repo.GetAsync(entityType, entityId, field, lang);

        public async Task TranslateAsync<T>(
            T obj, string entityType, int entityId, string lang,
            params (string field, Action<T, string> setter)[] fields)
        {
            if (lang == "en") return;

            foreach (var (field, setter) in fields)
            {
                var val = await _repo.GetAsync(entityType, entityId, field, lang);
                if (val != null) setter(obj, val);
            }
        }

        public Task UpsertAsync(
            string entityType, int entityId, string field, string lang, string value)
            => _repo.UpsertAsync(entityType, entityId, field, lang, value);

        public Task DeleteAllAsync(string entityType, int entityId)
            => _repo.DeleteAllAsync(entityType, entityId);
    }
}