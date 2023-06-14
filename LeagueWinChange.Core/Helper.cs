using System;

namespace LeagueWinChance.Core;
public class Helper
{
    public static DateTime ConvertLongToDate(long ticks)
    {
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        return dtDateTime.AddMilliseconds(ticks).ToLocalTime();
    }
}
