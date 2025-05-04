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
                players[i].SetName("Player" + i);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                players[i].ResetStats();
                players[i].SetName("Player" + i);
            }
        }
    }
}
