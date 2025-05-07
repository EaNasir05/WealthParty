using System.Collections.Generic;
using UnityEngine;

public class RegionsManager
{
    public static List<Region> regions;

    public static void Awake()
    {
        if (regions == null)
        {
            regions = new List<Region>();
            for (int i = 0; i < 6; i++)
            {
                regions.Add(new Region());
                ResetRegionStats(i);
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                ResetRegionStats(i);
            }
        }
    }

    private static void ResetRegionStats(int index)
    {
        switch (index) {
            case 0:
                regions[index].ResetStats(500, 1500, 1000);
                break;
            case 1:
                regions[index].ResetStats(900, 2250, 1500);
                break;
            case 2:
                regions[index].ResetStats(1300, 3000, 2000);
                break;
            case 3:
                regions[index].ResetStats(1700, 3750, 2500);
                break;
            case 4:
                regions[index].ResetStats(2100, 4500, 3000);
                break;
            case 5:
                regions[index].ResetStats(2500, 5250, 3500);
                break;
            default:
                break;
        }
    }
}
