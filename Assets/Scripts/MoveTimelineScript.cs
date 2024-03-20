using Mirror;
using UnityEngine;

public class MoveTimelineScript : NetworkBehaviour
{
    public void  MoveLeft()
    {
        var Timeline = GameObject.Find("Line").transform;
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        var _playerManager = networkIdentity.GetComponent<PlayerManager>();
        _playerManager.CmdMoveLeft(Timeline);
    }

    public void MoveRight()
    {
        var Timeline = GameObject.Find("Line").transform;
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        var _playerManager = networkIdentity.GetComponent<PlayerManager>();
        _playerManager.CmdMoveRight(Timeline);
    }
}
