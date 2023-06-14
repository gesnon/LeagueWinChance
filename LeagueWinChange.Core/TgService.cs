using LeagueWinChance.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LeagueWinChance.Core;
public class TgService
{
    public async Task<string> GetSummonerNameByTgId(long tgId)
    {
        using (var context = new LeagueContext())
        {
            var user = await context.TgUsers.FirstOrDefaultAsync(_ => _.TgId == tgId);

            return user?.SummonerName;
        }
    }

    public async Task AddSummonerNameByTgId(string summonerName, long tgId)
    {
        using (var context = new LeagueContext())
        {
            var user = await context.TgUsers.AddAsync(new DataAccess.Models.TgUser { TgId = tgId, SummonerName = summonerName });

            await context.SaveChangesAsync();
        }
    }
}
