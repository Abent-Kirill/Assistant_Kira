using Microsoft.EntityFrameworkCore;

namespace Assistant_Kira;

internal class BotDataContext : DbContext
{
	public BotDataContext(DbContextOptions<BotDataContext> options) : base(options)
	{
	}

	//public DbSet<AppUser> Users { get; set; }
}
