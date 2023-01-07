using System;
using System.Collections.Generic;

namespace Taraweb.Models.TarawebM1
{
    public partial class Language
    {
        public Language()
        {
            PageCategories = new HashSet<PageCategory>();
            PostContents = new HashSet<PostContent>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<PageCategory> PageCategories { get; set; }
        public virtual ICollection<PostContent> PostContents { get; set; }
    }
}
