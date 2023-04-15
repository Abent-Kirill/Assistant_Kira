using Microsoft.EntityFrameworkCore;

namespace Assistant_Kira;

internal sealed class BotDataContext : DbContext
{
	public BotDataContext(DbContextOptions<BotDataContext> options) : base(options)
	{
	}

	//public DbSet<AppUser> Users { get; set; }
}
