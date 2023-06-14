using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueWinChance.Core.Models
{
    public class WinrateAgainstChampion
    {
        public int Win { get; set; }
        public int TotalGames { get; set; }
        public float Kills { get; set; }
        public float Deaths { get; set; }
        public float Assists { get; set; }
    }
}
