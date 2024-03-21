using Mirror;
using System.Collections.Generic;
using UnityEngine;

//NetworkBehaviour
public class NetworkGameManager : NetworkManager
{
    /// <summary>Called on server when a client requests to add the player. Adds playerPrefab by default. Can be overwritten.</summary>
    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        var identity = conn.identity;
        var playerManager = identity.GetComponent<PlayerManager>();

        //Если клиента 2, то для каждого клиента запустим раздачу карт
        if (NetworkServer.connections.Count == 2)
        {
            var gameManager = GameObject.Find("GameManager").GetComponent<GameManagerNew>();
            gameManager.CurrentGame.RedeckDeck();

            foreach (var connection in NetworkServer.connections)
            {
                //Получим PlayerManager 
                var id = connection.Value.identity;
                var pm = id.GetComponent<PlayerManager>();
                pm.ServerGiveHandCards();
                pm.HideWaitingMenu();
                pm.ServerStartGame();
            }

            playerManager.ServerBlockHand(conn);
        }
        else
        {
            playerManager.ShowWaitingMenu();
            playerManager.ServerStartGame();
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        NetworkConnectionToClient cn = null;
        foreach (var connection in NetworkServer.connections)
            cn = connection.Value;
        
        NetworkServer.DestroyPlayerForConnection(cn);
        cn.Disconnect();
    }
}
