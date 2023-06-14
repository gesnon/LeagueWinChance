using LeagueWinChance.Core.Models;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueWinChance.Core
{
    public class RiotService
    {
        private readonly RiotApi _api;
        private readonly MatchService _matchService;
        public RiotService(string token)
        {
            _api = RiotApi.NewInstance(token);
            _matchService = new MatchService();
        }

        public async Task<SummonerDto> GetSummonerInfoByNameAsync(Region region, string name)
        {
            var summoner = await _api.SummonerV4.GetBySummonerNameAsync(region, name);

            return new SummonerDto(summoner, region);
        }

        public async Task SaveMatchHistory(SummonerDto summonerDto)
        {
            var step = 100;
            var take = 100;
            var skip = 0;
            Console.WriteLine(summonerDto.AccountId);
            var matchList = await _api.MatchV5.GetMatchIdsByPUUIDAsync(Region.Europe, summonerDto.AccountId, queue: 420, start: skip, count: take);
            var exisitingMatches = await _matchService.GetAllMatchesId();
            while (matchList.Length > 0)
            {
                Console.WriteLine($"{DateTime.Now} Нашел {matchList.Length} матчей");
                var matchesToAdd = matchList.Except(exisitingMatches);
                Console.WriteLine($"{DateTime.Now} Из них {matchesToAdd.Count()} нет в базе");
                var matches = new List<Match>();
                var tooOld = false;
                foreach (var matchId in matchesToAdd)
                {
                    Thread.Sleep(2000);

                    var match = await _api.MatchV5.GetMatchAsync(Region.Europe, matchId);
                    if (match.Info.QueueId != 420)
                    {
                        continue;
                    }
                    var q = await _api.MatchV5.GetTimelineAsync(Region.Europe, matchId);
                    
                    if (Helper.ConvertLongToDate(match.Info.GameCreation).Year < 2023)
                    {
                        tooOld = true;
                        break;
                    }
                    matches.Add(match);
                }
                Console.WriteLine($"{DateTime.Now} Добавляем в базе {matches.Count} матчей");
                await _matchService.AddMatchesAsync(matches);
                if (tooOld)
                {
                    break;
                }
                skip += step;
                matchList = await _api.MatchV5.GetMatchIdsByPUUIDAsync(Region.Europe, summonerDto.AccountId, queue: 420, start: skip, count: take);
            }
        }
    }
}
