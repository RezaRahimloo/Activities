using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class UserFollowing
    {
        public Guid ObserverId { get; set; }
        public AppUser Observer { get; set; }
        public Guid TargetId { get; set; }
        public AppUser Target { get; set; }
    }
}