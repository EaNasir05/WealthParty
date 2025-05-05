using System.Collections.Generic;
using UnityEngine;

public class PlayersManager
{
    public static List<Player> players;
    
    public static void Awake()
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

    public static void ChangePlayersOrder(int first, int second, int third, int fourth)
    {
        Player[] temp = { players[first], players[second], players[third], players[fourth] };
        players[0] = temp[0];
        players[1] = temp[1];
        players[2] = temp[2];
        players[3] = temp[3];
    }

    public static void ShufflePlayers()
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
