using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
    public class AppUser : IdentityUser
	{
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        [NotMapped]
        public string Fullname { get=>$"{Name} {Surname} "; }
        public ICollection<BasketItem> BasketItems { get; set; }
        public AppUser()
        {
            BasketItems = new HashSet<BasketItem>();
        }
	}
}

