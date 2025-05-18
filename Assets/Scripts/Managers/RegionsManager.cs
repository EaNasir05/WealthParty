using System.Collections.Generic;
using UnityEngine;

public class Region
{
    private string name; //Nome della regione
    private int level; //Livello di investimenti della regione
    private int cost; //Costo dell'attività regionale
    private int votesRate; //Tasso iniziale di guadagno di voti dell'attività regionale
    private int currentVotesRate; //Tasso di guadagno di voti dell'attività regionale influenzato dagli investimenti
    private string activity;
    private bool active; //Non ti interessa

    public Region()
    {
        active = true;
    }

    public void ResetStats(string name, int cost, int votesRate, string activity) //Assegna i valori iniziali alla regione
    {
        level = 0;
        this.name = name;
        this.cost = cost;
        this.votesRate = votesRate;
        this.currentVotesRate = votesRate;
        this.activity = activity;
    }

    //Metodi usati per ottenere da altri script valori di attributi privati
    public string GetName() { return name; }
    public int GetLevel() { return level; }
    public int GetCost() { return cost; }
    public int GetVotesRate() { return votesRate; }
    public bool isActive() { return active; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetActive(bool value) { active = value; }
    public void SetCost(int cost) { this.cost = cost; }

    public int[] GetCurrentVotesRate()
    {
        int[] production = { (int)(currentVotesRate * 0.9), (int)(currentVotesRate * 1.1) };
        return production;
    }
    
    public void ChangeLevel(int value) //Applica gli investimenti dei giocatori
    {
        level += value;
        if (level < -5) { level = -5; }
        if (level > 5) { level = 5; }
        currentVotesRate = votesRate + (int)(votesRate * 0.15 * level);
    }
}

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
        switch (index)
        {
            case 0:
                regions[index].ResetStats("ABRUZZO", 500, 1000, "Aumeta la produzione di arrosticini");
                break;
            case 1:
                regions[index].ResetStats("CAMPANIA", 750, 1500, "Pulisci le strade in città");
                break;
            case 2:
                regions[index].ResetStats("PUGLIA", 1000, 2000, "Organizza eventi di ballo in paese");
                break;
            case 3:
                regions[index].ResetStats("BASILICATA", 1250, 2500, "Ridicolizza il progresso");
                break;
            case 4:
                regions[index].ResetStats("CALABRIA", 1500, 3000, "Aumenta la coltivazione di peperoncino");
                break;
            case 5:
                regions[index].ResetStats("SICILIA", 1750, 3500, "Garantisci la sicurezza locale");
                break;
        }
    }
}
