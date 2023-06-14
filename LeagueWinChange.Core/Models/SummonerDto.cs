using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;

namespace LeagueWinChance.Core.Models;
public class SummonerDto
{
    public string SummonerName { get; set; }
    public string AccountId { get; set; }
    public string SummonerId { get; set; }
    public Region Region { get; set; }

    public SummonerDto(Summoner apiSummoner, Region region)
    {
        SummonerId = apiSummoner.Id;
        SummonerName = apiSummoner.Name;
        AccountId = apiSummoner.Puuid;
        Region = region;
    }
}
