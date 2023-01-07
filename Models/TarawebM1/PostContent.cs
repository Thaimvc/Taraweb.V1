using System;
using System.Collections.Generic;

namespace Taraweb.Models.TarawebM1
{
    public partial class PostContent
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int LanguageId { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public virtual Language Language { get; set; }
        public virtual Post Post { get; set; }
    }
}
