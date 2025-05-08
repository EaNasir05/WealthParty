using UnityEngine;

public class Region
{
    private string name; //Nome della regione
    private int level; //Livello di investimenti della regione
    private int cost; //Costo dell'attività regionale
    private int moneyRate; //Tasso iniziale di guadagno di denaro dell'attività regionale
    private int currentMoneyRate; //Tasso di guadagno di denaro dell'attività regionale influenzato dagli investimenti
    private int votesRate; //Tasso iniziale di guadagno di voti dell'attività regionale
    private int currentVotesRate; //Tasso di guadagno di voti dell'attività regionale influenzato dagli investimenti
    private bool active; //Non ti interessa

    public Region()
    {
        active = true;
    }

    public void ResetStats(string name, int cost, int moneyRate, int votesRate) //Assegna i valori iniziali alla regione
    {
        level = 0;
        this.name = name;
        this.cost = cost;
        this.moneyRate = moneyRate;
        this.currentMoneyRate = moneyRate;
        this.votesRate = votesRate;
        this.currentVotesRate = votesRate;
    }

    //Metodi usati per ottenere da altri script valori di attributi privati
    public string GetName() { return name; }
    public int GetLevel() { return level; }
    public int GetCost() { return cost; }
    public int GetMoneyRate() { return currentMoneyRate; }
    public int GetVotesRate() { return currentVotesRate; }
    public bool isActive() { return active; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetActive(bool value) { active = value; }
    public void SetCost(int cost) { this.cost = cost; }

    public void ChangeLevel(int value) //Applica gli investimenti dei giocatori
    {
        level += value;
        if (level < -5) { level = -5; }
        if (level > 5) { level = 5; }
        int increase = (int)(moneyRate * 0.1 * level);
        currentMoneyRate = moneyRate + increase;
    }
}
