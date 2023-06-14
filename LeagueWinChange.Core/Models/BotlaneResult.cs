using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueWinChance.Core.Models
{
    public class BotlaneResult
    {
        public string Support { get; set; }
        public string Adc { get; set; }
        public double Winrate { get; set; }
        public int TotalGames { get; set; }

        public override string ToString()
        {
            return $"Adc: {Adc}; Support: {Support}; Winrate: {Winrate * 100}; TotalGames: {TotalGames}";
        }
    }
}
