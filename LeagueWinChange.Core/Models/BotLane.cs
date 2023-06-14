using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueWinChance.Core.Models
{
    public class BotLane
    {
        public string AllySupport { get; set; }
        public string AllyAdc { get; set; }
        public string EnemySupport { get; set; }
        public string EnemyAdc { get; set; }
        public bool Win { get; set; }

        public override string ToString()
        {
            return $"Support: {AllySupport}; Adc: {AllyAdc}; EnemySupport: {EnemySupport}; EnemyAdc: {EnemyAdc}; Win: {Win}";
        }
    }
}
