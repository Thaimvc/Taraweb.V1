using System;
using System.Collections.Generic;

namespace Taraweb.Models.TarawebM1
{
    public partial class PageCategory
    {
        public PageCategory()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int LanguageId { get; set; }
        public int Sorting { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreate { get; set; }

        public virtual Language Language { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
