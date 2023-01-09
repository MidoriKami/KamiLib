using System;
using System.Linq;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Misc;

public static class Time
{
    public static DateTime NextDailyReset()
    {
        var now = DateTime.UtcNow;
            
        if( now.Hour < 15 )
        {
            return now.Date.AddHours(15);
        }
        else
        {
            return now.AddDays(1).Date.AddHours(15);
        }
    }

    public static DateTime NextWeeklyReset()
    {
        return NextDayOfWeek(DayOfWeek.Tuesday, 8);
    }

    public static DateTime NextFashionReportReset()
    {
        return NextWeeklyReset().AddDays(-4);
    }

    public static DateTime NextGrandCompanyReset()
    {
        var now = DateTime.UtcNow;
        var targetHour = 20;    
        
        if( now.Hour < targetHour )
        {
            return now.Date.AddHours(targetHour);
        }
        else
        {
            return now.AddDays(1).Date.AddHours(targetHour);
        }
    }
    
    public static DateTime NextLeveAllowanceReset()
    {
        var now = DateTime.UtcNow;

        if( now.Hour < 12 )
        {
            return now.Date.AddHours(12);   
        }
        else
        {
            return now.Date.AddDays(1);
        }
    }

    public static DateTime NextDayOfWeek(DayOfWeek weekday, int hour)
    {
        var today = DateTime.UtcNow;
            
        if(today.Hour < hour && today.DayOfWeek == weekday)
        {
            return today.Date.AddHours(hour);
        }
        else
        {
            var nextReset = today.AddDays(1);

            while (nextReset.DayOfWeek != weekday)
            {
                nextReset = nextReset.AddDays(1);
            }
                
            return nextReset.Date.AddHours(hour);
        }
    }

    public static DateTime NextJumboCactpotReset()
    {
        var region = LookupDatacenterRegion(Service.ClientState.LocalPlayer?.HomeWorld.GameData?.DataCenter.Row);

        return region switch
        {
            // Japan
            1 => NextDayOfWeek(DayOfWeek.Saturday, 12),

            // North America
            2 => NextDayOfWeek(DayOfWeek.Sunday, 2),

            // Europe
            3 => NextDayOfWeek(DayOfWeek.Saturday, 19),

            // Australia
            4 => NextDayOfWeek(DayOfWeek.Saturday, 9),

            // Unknown Region
            _ => DateTime.MinValue
        };
    }

    private static byte LookupDatacenterRegion(uint? playerDatacenterID)
    {
        if (playerDatacenterID == null) return 0;

        return LuminaCache<WorldDCGroupType>.Instance
            .Where(world => world.RowId == playerDatacenterID.Value)
            .Select(dc => dc.Region)
            .FirstOrDefault();
    }
}