using Mirror;
using UnityEngine;

public class IncreaseCardSizeScript : NetworkBehaviour
{
    private GameObject PlayerHand;
    private GameObject cardGO;

    public void OnClick()
    {
        PlayerHand = GameObject.Find("PlayerHand");
        cardGO = PlayerHand.gameObject.transform.GetChild(0).gameObject;

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        var playerManager = networkIdentity.GetComponent<PlayerManager>();
        //playerManager.RpcChangeCardSize(cardGO.transform.GetSiblingIndex(), true, "hand");
    }
}
