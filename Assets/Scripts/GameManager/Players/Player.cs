using UnityEngine;
using UnityEngine.UI;

public class Player
{
    private Texture icon; //Icona del giocatore
    private string name; //Nome del giocatore
    private int money; //Denaro del giocatore
    private int votes; //Voti del giocatore

    public Player()
    {
        ResetStats();
    }

    public void ResetStats() //Assegna le statistiche iniziali al giocatore
    {
        money = 0;
        votes = 0;
    }

    //Metodi usati per ottenere da altri script valori di attributi privati
    public Texture GetIcon() { return icon; }
    public string GetName() { return name; }
    public int GetMoney() {  return money; }
    public int GetVotes() { return votes; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetIcon(Texture icon) { this.icon = icon; }
    public void SetName(string name) {  this.name = name; }

    public void AddMoney(int value) //Aggiunge una somma di monete al giocatore
    {
        money += value;
        if (money < 0)
        {
            money = 0;
        }
    }

    public void AddVotes(int value) //Aggiunge una somma di voti al giocatore
    {
        votes += value;
        if (votes < 0)
        {
            votes = 0;
        }
    }
}
