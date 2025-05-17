using System.Collections.Generic;
using UnityEngine;

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
    public int GetMoney() { return money; }
    public int GetVotes() { return votes; }

    //Metodi usati per cambiare da altri script valori di attributi privati
    public void SetIcon(Texture icon) { this.icon = icon; }
    public void SetName(string name) { this.name = name; }

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

public class PlayersManager
{
    public static List<Player> players; //Lista dei giocatori, segue l'ordine dei turni di un round
    
    public static void Awake() //Istanzia la lista dei giocatori e ne resetta le statistiche (Non è l'Awake di Unity)
    {
        if (players == null)
        {
            players = new List<Player>();
            for (int i = 0; i < 4; i++)
            {
                players.Add(new Player());
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                players[i].ResetStats();
            }
        }
    }

    public static void ChangePlayersOrder(int first, int second, int third, int fourth) //Ordina i giocatori secondo uno specifico criterio
    {
        Player[] temp = { players[first], players[second], players[third], players[fourth] };
        players[0] = temp[0];
        players[1] = temp[1];
        players[2] = temp[2];
        players[3] = temp[3];
    }

    public static void ShufflePlayers() //Mescola in ordine casuale l'ordine dei giocatori
    {
        for (int i = players.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            Player temp = players[i];
            players[i] = players[j];
            players[j] = temp;
        }
    }
}
