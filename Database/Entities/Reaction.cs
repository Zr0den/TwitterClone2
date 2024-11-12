using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{
    public class Reaction
    {
        string Content { get; set; }
        [ForeignKey("User")]
        int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Tweet")]
        int TweetId { get; set; }
        public virtual Tweet Tweet { get; set; }
        int CommentId { get; set; }
        public virtual Comment Comment { get; set; }

    }
}
