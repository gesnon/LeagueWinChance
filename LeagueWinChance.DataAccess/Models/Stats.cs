using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueWinChance.DataAccess.Models
{
    public class Stats
    {
        public int Id { get; set; }
        public MatchPlayer MatchPlayer { get; set; }
        public int MatchPlayerId { get; set; }
        public string ChampionName { get; set; }
        public long Kills { get; set; }
        public long Deaths { get; set; }
        public long Assists { get; set; }
    }
}
