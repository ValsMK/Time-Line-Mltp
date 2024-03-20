using Mirror;

public class DrawCardsScript : NetworkBehaviour
{
    private PlayerManager _playerManager;

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        _playerManager = networkIdentity.GetComponent<PlayerManager>();
        _playerManager.CmdGiveHandCards();
    }
}
