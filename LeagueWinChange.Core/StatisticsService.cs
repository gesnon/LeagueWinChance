using LeagueWinChance.Core.Models;
using LeagueWinChance.DataAccess;
using LeagueWinChance.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Plotly.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeagueWinChance.Core
{
    public class StatisticsService
    {
        public List<BotlaneResult> GetWinRateAgainstDuoBot()
        {
            List<BotLane> botLanes = new List<BotLane>();
            List<BotlaneResult> results = new List<BotlaneResult>();
            using (var context = new LeagueContext())
            {
                var matches = context.Matches.Where(_ => _.MatchDate.Year == 2023).Include(_ => _.Players).ThenInclude(__ => __.Stats);
                foreach (var match in matches)
                {
                    var bot = GetBotLanes(match);
                    if (bot == null)
                    {
                        continue;
                    }
                    botLanes.Add(bot);
                }
            }

            var groupBot = botLanes.GroupBy(_ => new { _.EnemyAdc, _.EnemySupport });
            foreach (var group in groupBot)
            {
                var totalGames = group.Count();
                var totalWins = group.Count(_ => _.Win);
                var winrate = totalWins == 0 ? 0 : Math.Round((float)totalWins / totalGames * 100, 2);

                results.Add(new BotlaneResult
                {
                    Adc = group.Key.EnemyAdc,
                    Support = group.Key.EnemySupport,
                    TotalGames = group.Count(),
                    Winrate = winrate
                });
            }

            return results.OrderByDescending(_ => _.TotalGames).ToList();
        }

        public void GetWinrateByKillParticipation(string summonerId)
        {
            using (var context = new LeagueContext())
            {
                List<WinrateByKillParticipation> winrateByKillParticipations = new List<WinrateByKillParticipation>();
                var matches = context.Matches.Where(_ => _.Players.Any(_ => _.SummonerId.Equals(summonerId)) && _.MatchDate >= new DateTime(2023, 1, 1)).Include(_ => _.Players).ThenInclude(__ => __.Stats).ToList();
                foreach (var match in matches)
                {
                    var summoner = match.Players.First(_ => _.SummonerId == summonerId);
                    var summonerTeam = match.Players.Where(_ => _.TeamId == summoner.TeamId);
                    winrateByKillParticipations.Add(new WinrateByKillParticipation
                    {
                        KillParticpation = (double)(summoner.Stats.Kills + summoner.Stats.Assists) * 100 / summonerTeam.Sum(_ => _.Stats.Kills),
                        Win = match.TeamWinId == summoner.TeamId
                    });
                }
                var groupKp = winrateByKillParticipations.GroupBy(_ => Math.Round(_.KillParticpation, 2, MidpointRounding.ToEven)).OrderBy(_ => _.Key);
                var data = groupKp.Select(_ => new WinrateByKPStat { KP = _.Key, WR = Math.Round((double)_.Count(m => m.Win) * 100 / _.Count(), 2, MidpointRounding.ToEven) });
                var winrate_stat = new List<WinrateByKPStat>();
                var step = 5;
                var currentKP = 0;
                var test = Enumerable.Range(0, 21).Select(_ => _ * 5);
                foreach (var item in test)
                {
                    var relevantStats = groupKp.Where(_ => _.Key >= item && _.Key <= item + step);
                    if (!relevantStats.Any())
                    {
                        winrate_stat.Add(new WinrateByKPStat { KP = item, WR = 0 });
                        continue;
                    }

                    var allStats = relevantStats.SelectMany(_ => _);
                    var totalWR = Math.Round((double)allStats.Count(_ => _.Win) / allStats.Count(), 2, MidpointRounding.ToEven) * 100;
                    var totalKP = Math.Round((double)relevantStats.Sum(_ => _.Sum(_ => _.KillParticpation)) / relevantStats.Sum(_ => _.Count()), 2, MidpointRounding.ToEven) * 100;
                    winrate_stat.Add(new WinrateByKPStat { KP = item, WR = totalWR });
                }
                
                var w = winrate_stat.Select(_ => _.KP).Count();
                var r = winrate_stat.Select(_ => _.WR).Count();
                //Chart.
                string kp = string.Join(";", winrate_stat.Select(_ => _.KP));
                string wr = string.Join(";", winrate_stat.Select(_ => _.WR));
                //var model = new PlotModel();
                //LineSeries lineSeries = new LineSeries();
                //lineSeries.Points.AddRange());
                //var stream = File.Create(@"C:\home\test.pdf");
                //OxyPlot.PdfExporter.Export(model, stream, 500, 500);
                //stream.Flush();
                //stream.Dispose();
                //System.Windows.Forms.DataVisualization.Charting.Chart7
                //model.Series.Add()
                //model.Axes.Add(new LinearColorAxis { Title = "KP", AbsoluteMaximum = 100, AbsoluteMinimum= 0 });
                //model.Axes.Add(new LinearColorAxis { Title = "WR", AbsoluteMaximum = 100, AbsoluteMinimum= 0 });

            }
        }

        public List<WinrateAgainstChampionStat> GetWinrateByChampions(string summonerId, bool enemyTeam)
        {
            using (var context = new LeagueContext())
            {
                var matches = context.Matches.Where(_ => _.Players.Any(_ => _.SummonerId.Equals(summonerId)) && _.MatchDate >= new DateTime(2023, 1, 1)).Include(_ => _.Players).ThenInclude(__ => __.Stats).ToList();
                List<WinrateAgainstChampion> stats = new List<WinrateAgainstChampion>();
                Dictionary<string, WinrateAgainstChampion> dict = new Dictionary<string, WinrateAgainstChampion>();
                foreach (var match in matches)
                {
                    var summonerTeamId = match.Players.FirstOrDefault(_ => _.SummonerId == summonerId).TeamId;
                    var teamPlayers = GetTeamPlayers(match, enemyTeam ? match.Players.First(_ => _.TeamId != summonerTeamId).TeamId : summonerTeamId);
                    foreach (var player in teamPlayers)
                    {
                        if (dict.ContainsKey(player.Champion))
                        {
                            var winrate = dict[player.Champion];
                            winrate.TotalGames++;
                            if (match.TeamWinId == summonerTeamId)
                            {
                                winrate.Win++;
                            }
                            winrate.Kills += player.Kills;
                            winrate.Deaths += player.Deaths;
                            winrate.Assists += player.Assists;

                            dict[player.Champion] = winrate;

                            continue;
                        }

                        dict.Add(player.Champion, new WinrateAgainstChampion { TotalGames = 1, Win = match.TeamWinId == summonerTeamId ? 1 : 0, Assists = player.Assists, Deaths = player.Deaths, Kills = player.Kills });
                    }
                }
                var statistic = dict.Select(_ =>
                new WinrateAgainstChampionStat
                {
                    Champion = _.Key,
                    TotalGames = _.Value.TotalGames,
                    Win = _.Value.Win,
                    Assists = _.Value.Assists,
                    Deaths = _.Value.Deaths,
                    Kills = _.Value.Kills
                });

                return statistic.OrderByDescending(_ => _.TotalGames).ToList();
            }
        }

        private List<WinrateAgainstChampionStat> GetTeamPlayers(LeagueMatch match, int teamId)
        {
            return match.Players.Where(_ => _.TeamId != teamId).Select(_ =>
            new WinrateAgainstChampionStat
            {
                Assists = _.Stats.Assists,
                Champion = _.Stats.ChampionName,
                Deaths = _.Stats.Deaths,
                Kills = _.Stats.Kills
            }).ToList();
        }

        private BotLane GetBotLanes(LeagueMatch match)
        {
            BotLane botLane = new BotLane();
            //botLane.Win = match.Win;
            var me = match.Players.FirstOrDefault(_ => _.Name == "Dârcy");
            var partner = match.Players.FirstOrDefault(_ => _.Name == "gesnons");
            if (partner == null)
            {
                return null;
            }
            var ourTeam = me.TeamId;
            botLane.AllyAdc = match.Players.FirstOrDefault(_ => _.Name == "gesnons" && _.TeamId == ourTeam)?.Stats?.ChampionName;
            botLane.AllySupport = match.Players.FirstOrDefault(_ => _.Name == "Dârcy" && _.TeamId == ourTeam)?.Stats?.ChampionName;
            botLane.EnemySupport = match.Players.FirstOrDefault(_ => _.Role == "Support" && _.TeamId != ourTeam)?.Stats?.ChampionName;
            botLane.EnemyAdc = match.Players.FirstOrDefault(_ => _.Role == "Carry" && _.TeamId != ourTeam)?.Stats?.ChampionName;

            return botLane;
        }
    }
}
