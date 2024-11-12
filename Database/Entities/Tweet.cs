using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{
    public class Tweet
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string Hashtags { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
