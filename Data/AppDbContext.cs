using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Pustok.Data
{
	public class AppDbContext: DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



    }
}

