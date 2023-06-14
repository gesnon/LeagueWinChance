using LeagueWinChance.DataAccess;
using LeagueWinChance.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.MatchV5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueWinChance.Core
{
    public class MatchService
    {        
        public async Task<List<string>> GetAllMatchesId()
        {
            using (var context = new LeagueContext())
            {
                return await context.Matches.Select(_ => _.Id).ToListAsync();
            }
        }

        public async Task AddMatchesAsync(List<Match> matches)
        {
            using (var context = new LeagueContext())
            {
                var existingMatches = await context.Matches.Select(_ => _.Id).ToListAsync();
                
                var matchesToAdd = matches.Select(m => new LeagueMatch
                {                   
                    Duration = m.Info.GameDuration,
                    Id = m.Metadata.MatchId,
                    MatchDate = Helper.ConvertLongToDate(m.Info.GameCreation),
                    TeamWinId = m.Info.Teams.First(_ => _.Win).TeamId,
                    Players = m.Info.Participants.Select(p => new MatchPlayer
                    {
                        SummonerId = p.SummonerId,
                        Lane = p.Lane.ToString(),
                        Role = p.Role.ToString(),
                        Name = p.SummonerName,
                        TeamId = p.TeamId,
                        Stats = new Stats
                        {
                            Assists = p.Assists,
                            ChampionName = p.ChampionName,
                            Deaths = p.Deaths,
                            Kills = p.Kills
                        }
                    }).ToList()
                });

                context.Matches.AddRange(matchesToAdd.ExceptBy(existingMatches, _ => _.Id));

                context.SaveChanges();
            }
        }

        //private MatchPlayer EnsurePlayer(Match match, string playerId)
        //{
        //    var matchPlayer = match.Info.Participants.FirstOrDefault(_ => _.SummonerId == playerId);
        //    return new MatchPlayer
        //    {
        //        SummonerId = matchPlayer.SummonerId,
        //        Lane = matchPlayer.Lane.ToString(),
        //        Role = matchPlayer.Role.ToString(),
        //        Name = matchPlayer.SummonerName,
        //        Stats = new Stats
        //        {
        //            Assists = matchPlayer.Assists,
        //            ChampionName = matchPlayer.ChampionName,
        //            Deaths = matchPlayer.Deaths,
        //            Kills = matchPlayer.Kills
        //        }
        //    };
        //}

        //private List<MatchPlayer> GetPlayersFromMatch(LeagueContext context, Match match)
        //{
        //    return match.Info.Participants.Select(p => EnsurePlayer(context, match, p.SummonerId)).ToList();
        //}
    }
}
