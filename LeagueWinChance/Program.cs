using ConsoleTables;
using LeagueWinChance.Core;
using MingweiSamuel.Camille.Enums;
using System;
using System.Threading.Tasks;

namespace LeagueWinChance
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            RiotService riotService = new RiotService();
            var account = await riotService.GetSummonerInfoByNameAsync(Region.EUW, "gesnons");
            //StatisticsService statisticsService = new StatisticsService();
            //statisticsService.GetWinrateByKillParticipation(account.SummonerId);
            await riotService.SaveMatchHistory(account);

            //StatisticsService statisticsService = new StatisticsService();
            //var data = statisticsService.GetWinRateAgainstDuoBot();
            //var table = new ConsoleTable("Champion1", "Champion2", "TotalGames", "Winrate");
            //foreach (var item in data)
            //{
            //    table.AddRow(item.Adc, item.Support, item.TotalGames, item.Winrate);
            //}
            //table.Write();


            //StatisticsService statisticsService1 = new StatisticsService();
            //var data = statisticsService1.GetWinrateByChampionsChampions(account.SummonerId,);
            //var table = new ConsoleTable("Champion", "TotalGames", "Winrate", "Kills", "Deaths", "Assists");
            //foreach (var item in data)
            //{
            //    table.AddRow(item.Champion, item.TotalGames, Math.Round((float)item.Win / item.TotalGames * 100, 2), Math.Round(item.Kills / item.TotalGames, 2), Math.Round(item.Deaths / item.TotalGames, 2), Math.Round(item.Assists / item.TotalGames, 2));
            //}
            //table.ToMarkDownString();

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
