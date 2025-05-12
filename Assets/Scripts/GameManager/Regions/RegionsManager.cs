using System.Collections.Generic;
using UnityEngine;

public class RegionsManager
{
    public static List<Region> regions; //Lista delle regioni nella mappa di gioco

    public static void Awake() //Istanzia la lista delle regioni e ne resetta le statistiche (Non è l'Awake di Unity)
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

    private static void ResetRegionStats(int index) //Resetta le statistiche delle regioni
    {
        switch (index) {
            case 0:
                regions[index].ResetStats("ABRUZZO", 500, 1500, 750);
                break;
            case 1:
                regions[index].ResetStats("CAMPANIA", 750, 2250, 1125);
                break;
            case 2:
                regions[index].ResetStats("PUGLIA", 1000, 3000, 1500);
                break;
            case 3:
                regions[index].ResetStats("BASILICATA", 1250, 3750, 1875);
                break;
            case 4:
                regions[index].ResetStats("CALABRIA", 1500, 4500, 2250);
                break;
            case 5:
                regions[index].ResetStats("SICILIA", 1750, 5250, 2625);
                break;
            default:
                break;
        }
    }
}
