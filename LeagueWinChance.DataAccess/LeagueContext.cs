using LeagueWinChance.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace LeagueWinChance.DataAccess
{
    public class LeagueContext : DbContext
    {
        public DbSet<LeagueMatch> Matches { get; set; }
        public DbSet<MatchPlayer> Players { get; set; }
        public DbSet<TgUser> TgUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB; Database=LeagueWinChance;Trusted_Connection=True;");
        }
    }
}
