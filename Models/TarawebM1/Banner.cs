using System;
using System.Collections.Generic;

namespace Taraweb.Models.TarawebM1
{
    public partial class Banner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateFinish { get; set; }
        public bool IsActive { get; set; }
    }
}
