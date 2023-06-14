using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueWinChance.DataAccess.Models
{
    public class LeagueMatch
    {
        public string Id { get; set; }
        public int TeamWinId { get; set; }
        public List<MatchPlayer> Players { get; set; }
        public long Duration { get; set; }
        public DateTime MatchDate { get; set; }

    }
}
