using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueWinChance.Core.Models
{
    public class WinrateAgainstChampionStat : WinrateAgainstChampion
    {
        public string Champion { get; set; }
    }
}
