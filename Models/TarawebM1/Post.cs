using System;
using System.Collections.Generic;

namespace Taraweb.Models.TarawebM1
{
    public partial class Post
    {
        public Post()
        {
            PostContents = new HashSet<PostContent>();
        }

        public int Id { get; set; }
        public int PageCategoryId { get; set; }
        public int? GalleryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreate { get; set; }
        public int UserCreateId { get; set; }
        public DateTime DateUpdate { get; set; }
        public int UserUpdateId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateFinish { get; set; }

        public virtual Gallery Gallery { get; set; }
        public virtual PageCategory PageCategory { get; set; }
        public virtual ICollection<PostContent> PostContents { get; set; }
    }
}
