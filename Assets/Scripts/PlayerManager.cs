using Mirror;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    //test commit for git
    private Game _currentGame;

    public GameObject Card;
    public GameObject PlayerHand;
    public GameObject EnemyHand;
    public GameObject DiscardDeck;
    public GameObject Line;
    private GameObject GameInfo;
    private GameObject WaitingForSecondPlayer;
    private GameObject NotYourTurn;
    private GameObject EndGameMenu;
    private GameObject AnswerRight;
    private GameObject AnswerWrong;
    private GameObject AnswerEmpty;

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerHand = GameObject.Find("PlayerHand");
        EnemyHand = GameObject.Find("EnemyHand");
        DiscardDeck = GameObject.Find("DiscardDeck");
        Line = GameObject.Find("Line");
        GameInfo = GameObject.Find("GameInfo");
        WaitingForSecondPlayer = GameObject.Find("WaitingForSecondPlayer");
        NotYourTurn = GameObject.Find("NotYourTurn");
        EndGameMenu = GameObject.Find("EndGame");
        AnswerRight = GameObject.Find("AnswerRight");
        AnswerWrong = GameObject.Find("AnswerWrong");
        AnswerEmpty = GameObject.Find("AnswerEmpty");
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        _currentGame = GameObject.Find("GameManager").GetComponent<GameManagerNew>().CurrentGame;
    }

    /// <summary>
    ///     Команда раздает карты в руку игроку
    /// </summary>
    [Command]
    public void CmdGiveHandCards() => GiveCardsToHand();

    [Server]
    public void ServerGiveHandCards() => GiveCardsToHand();
   
    private void GiveCardsToHand()
    {
        Debug.Log("GiveHandCards");
        int i = 0;
        //Начинаем цикл, проходящий 6 итераций (от 0 до 5 включительно)
        while (i++ < 6)
            //Вызываем функцию GiveCardToHand и передаем ей текущие колоду и руку игрока
            GiveCardToHand(ref _currentGame.Deck);
    }    

    /// <summary>
    ///     Метод выдает клиенту одну карту в руку
    /// </summary>
    /// <param name="deck"></param>
    private void GiveCardToHand(ref List<Card> deck)        
    {
        Debug.Log("GiveCardToHand");
        //Задаем условие: "Если количество карт в колоде равно 0 (Если колода пустая), завершаем метод"
        if (deck.Count == 0)     
            return;     

        //Создаем новую временную переменную для карты и кладем в неё вернхнюю карту из колоды
        Card card = deck[0];        

        //Создадим новый игровой объект: карту
        GameObject cardGO = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(cardGO, connectionToClient);

        RpcShowDealtCard(cardGO, card);

        //Удаляем из колоды верхнюю (первую) карту
        deck.RemoveAt(0);
    }

    /// <summary>
    ///     Процедура отображает созданную карту на всмех клиентах
    /// </summary>
    /// <param name="cardGO"></param>
    /// <param name="card"></param>
    [ClientRpc]
    void RpcShowDealtCard(GameObject cardGO, Card card)
    {
        //Заполним игровой объект иинформацией из объекта типа Card
        cardGO.GetComponent<CardInfoScript>().ShowCardInfo(card);

        //Для игрока-владельца выкладываем карту в руку
        if (isLocalPlayer)
        {
            cardGO.transform.SetParent(PlayerHand.transform, false);
        }
        //Второму игроку показываем ее в зоне карт соперника
        else
        {
            cardGO.transform.SetParent(EnemyHand.transform, false);
        }
    }

    [Command]
    public void CmdDiscardCard(GameObject cardGO) => RpcDiscardCard(cardGO);

    [ClientRpc]
    void RpcDiscardCard(GameObject cardGO)
    {
        cardGO.transform.SetParent(DiscardDeck.transform, false);
    }
    
    [Command]
    public void CmdGiveCardToHand()
    {
        //Доберем карту в руку
        GiveCardToHand(ref _currentGame.Deck);
    }

    [Command]
    public void CmdEndTurn(GameObject cardGO, int timeLineIndex)
    {
        var id = connectionToClient.identity;

        Debug.Log($"CmdEndTurn: index: {timeLineIndex} identity: {id}");
        //Покажем карту на таймлайне всех игроков
        RpcShowPlayedCard(cardGO, timeLineIndex);
    }

    [Command]
    public void CmdBlockHand()
    {
        RpcBlockHand();
    }

    [Server]
    public void ServerBlockHand(NetworkConnectionToClient conn)
    {
        TargetBlockHand(conn);
    }

    //TargetRpc имеет параметр target,  который не используется в самой функции, однако Mirror определяет по нему, к какому клиенту относится данный запрос
    [TargetRpc]
    void TargetBlockHand(NetworkConnectionToClient target)
    {
        NotYourTurn.SetActive(false);
    }
    //Функция BlockHand существует в двух вариациях: TargetRpc и ClientRpc. TargetRpc используется только в самом
    //начале игры чтобы определить, кто будет ходить первым, а затем на протяжении всей игры ClientRpc "переключает" ходы.

    [ClientRpc]
    void RpcShowPlayedCard(GameObject cardGO, int i)
    {
        
        Debug.Log($"RpcShowPlayedCard: {cardGO.transform.parent.name}");

        cardGO.transform.SetParent(Line.transform, false);
        cardGO.transform.SetSiblingIndex(i);
        cardGO.GetComponent<CardInfoScript>().ShowCardYear();
        if (!isLocalPlayer)
        {
            cardGO.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    [Command]
    public void CmdShowGameInfo()
    {
        RpcShowGameInfo(_currentGame.Deck.Count);
    }

    [ClientRpc]
    void RpcShowGameInfo(int remainCardsCount)
    {
        //Отображаем кол-во всех карт на тайм-лайне
        var b1 = GameInfo.transform.Find("Text (TMP)");
        var b2 = b1.gameObject.GetComponent<TextMeshProUGUI>();
        b2.text = $"Выложено карт: {Convert.ToString(Line.transform.childCount)}; Осталось карт в колоде: {remainCardsCount}";
    }

    [ClientRpc]
    void RpcBlockHand() => NotYourTurn.SetActive(isLocalPlayer);

    [Command]
    public void CmdIncreaseCardSize(int endIndex)
    {
        Debug.Log($"CmdIncreaseCardSize: index:{endIndex}");
        RpcIncreaseCardSize(endIndex);
    }

    [Command]
    public void CmdDecreaseCardSize(int endIndex)
    {
        Debug.Log($"CmdDecreaseCardSize: index:{endIndex}");
        RpcDecreaseCardSize(endIndex);
    }

    [ClientRpc]
    public void RpcIncreaseCardSize(int endIndex)
    {
        Debug.Log($"RpcIncreaseCardSize: isLocalPlayer:{isLocalPlayer},  index: {endIndex},  childCont: {EnemyHand.transform.childCount}");

        if (!isLocalPlayer)
        {
            if (EnemyHand.transform.childCount < endIndex)
                return;

            GameObject cardGO = EnemyHand.transform.GetChild(endIndex).gameObject;

            cardGO.transform.localScale = new Vector3(1.125F, 1.125F, 1);
        }
    }

    [ClientRpc]
    public void RpcDecreaseCardSize(int endIndex)
    {
        Debug.Log($"RpcDecreaseCardSize: isLocalPlayer:{isLocalPlayer}, endIndex: {endIndex},  childCont: {EnemyHand.transform.childCount}");

        if (!isLocalPlayer)
        {
            if (EnemyHand.transform.childCount < endIndex)
                return;

            GameObject cardGO = EnemyHand.transform.GetChild(endIndex).gameObject;

            cardGO.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    [Command]
    public void CmdReplaceCard(int startIndex, int endIndex)
    {
        Debug.Log($"CmdReplaceCard: startIndex: {startIndex}, endIndex:{endIndex}");
        RpcReplaceCard(startIndex, endIndex);
    }

    //Метод перемещает карту на EnemyHand на экране противника, когда игрок перемещает её по своей руке
    [ClientRpc]
    void RpcReplaceCard(int startIndex, int endIndex)
    {
        if (!isLocalPlayer)
        {
            EnemyHand.transform.GetChild(startIndex).transform.SetSiblingIndex(endIndex);
        }
    }

    public void ShowWaitingMenu()
    {
        RpcShowWaitingMenu();
    }

    [ClientRpc]
    void RpcShowWaitingMenu()
    {
        WaitingForSecondPlayer.SetActive(true);
    }

    public void HideWaitingMenu()
    {
        RpcHideWaitingMenu();
    }

    [ClientRpc]
    void RpcHideWaitingMenu()
    {
        WaitingForSecondPlayer.SetActive(false);
    }

    [Command]
    public void CmdEndGame()
    {
        RpcEndGame();
    }

    [Server]
    public void ServerStartGame() => StartGame();

    private void StartGame() => RpcStartGame();

    [ClientRpc]
    void RpcStartGame()
    {
        if (isLocalPlayer)
        {
            EndGameMenu.SetActive(false);
        }
    }

    [ClientRpc]
    void RpcEndGame()
    {
        const string textName = "EndGameText";

        if (EndGameMenu == null) 
            return;

        EndGameMenu.SetActive(true);

        if (EndGameMenu.transform.Find(textName) == null)
            return;

        EndGameMenu.transform.Find(textName).gameObject.GetComponent<TextMeshProUGUI>().text = $"Вы {(isLocalPlayer ? "победили" : "проигали")} !";
    }
}

