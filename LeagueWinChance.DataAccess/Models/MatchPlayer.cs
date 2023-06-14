using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueWinChance.DataAccess.Models
{
    public class MatchPlayer
    {
        public int Id { get; set; }
        public string SummonerId { get; set; }
        public string Name { get; set; }
        public string Lane { get; set; }
        public string Role { get; set; }
        public int TeamId { get; set; }
        public Stats Stats { get; set; }
    }
}
