using Mirror;

public class GameManagerNew : NetworkBehaviour
{
    public Game CurrentGame;
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        CurrentGame = new Game();
    }
}
