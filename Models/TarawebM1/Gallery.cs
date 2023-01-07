using System;
using System.Collections.Generic;

namespace Taraweb.Models.TarawebM1
{
    public partial class Gallery
    {
        public Gallery()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreate { get; set; }
        public int UserCreateId { get; set; }
        public DateTime DateUpdate { get; set; }
        public int UserUpdateId { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
