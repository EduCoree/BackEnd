using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.TranslationModel
{
   public class Translation
    {
        public int Id { get; set; }
        public string EntityType { get; set; } = null!; // "Course", "Center", etc.
        public int EntityId { get; set; }
        public string Field { get; set; } = null!;  // "Title", "Name", etc.
        public string Lang { get; set; } = null!;  // "ar", "fr", etc.
        public string Value { get; set; } = null!;
    }
}
