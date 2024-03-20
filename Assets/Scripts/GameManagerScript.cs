using Mirror;
using System;     //���������� ���������� System
using System.Collections.Generic;     //���������� ���������� System.Collections.Generic
using TMPro;     //���������� ���������� TMPro
using UnityEngine;     //���������� ���������� UnityEngine
using UnityEngine.SceneManagement;

public enum DeckTypeEnum
{
    GeneralHistoryXVIIICentury = 1,
    RussianHistoryXVIIICentury = 2,
    UserDeck = 3
}

public class Game
{
    public List<Card> Deck;     //������� ����� ������ ���� - ������
    public int Lives;       //������� ����������, �������� ���������� ������
    public int Points;      //������� ����������, �������� ���������� �����
    public DeckTypeEnum DeckType = DeckTypeEnum.GeneralHistoryXVIIICentury;
    public string SaveDeckKey = "DECK_KEY";

    public Game()
    {
        Lives = 5;      //������ ��������� ���������� ������
        Points = 0;      //������ ��������� ���������� �����

        if (PlayerPrefs.HasKey(SaveDeckKey))
        {
            DeckType = (DeckTypeEnum)PlayerPrefs.GetFloat(SaveDeckKey);
        }
        else
        {
            DeckType = DeckTypeEnum.GeneralHistoryXVIIICentury;
            PlayerPrefs.SetFloat(this.SaveDeckKey, (int)DeckTypeEnum.GeneralHistoryXVIIICentury);
        }

        Deck = GiveDeckCard();      //������� ������ ���� � ������� ������� GiveDeckCard
    }

    public void RedeckDeck() => Deck = GiveDeckCard();

    List<Card> GiveDeckCard()      //������ ������� GiveDeckCard
    {
        List<Card> list = new();      //������� ����� ������ ������ ����
        List<Card> tmpList = new();           //������� ����� ��������� ������ ����, ���������� ��� ����� � ����

        switch (DeckType)
        {
            case DeckTypeEnum.GeneralHistoryXVIIICentury:
                tmpList = new(CardManager.GeneralHistoryXVIIICentury);
                break;
            case DeckTypeEnum.RussianHistoryXVIIICentury:
                tmpList = new(CardManager.RussianHistoryXVIIICentury);
                break;
            case DeckTypeEnum.UserDeck:
                tmpList = new(OpenFilePanel.GetDeckFromFile(@"UserDeck.csv", out _));
                break;
            default:
                break;
        }

        for (int i = tmpList.Count - 1; i >=0 ; i--)      //�������� ����, ����������� ������� ��������, ������� ���� � ����
        {
            int r = UnityEngine.Random.Range(0, tmpList.Count - 1);      //�������� ��������� ����� �� ���� �� ���������� ���� � ���� � ���������� ��� � ���������� r
            list.Add(tmpList[r]);      //��������� � ��� ������ ���� ����� � ������� r �� ���������� ������
            tmpList.RemoveAt(r);      //������� ����� � ������� r �� ���������� ������, ����� �������� �� �������� � � �������� ������ ��������
        }
        return list;      //���������� ������������ ������
    }
}

public class GameManagerScript : NetworkBehaviour
{
    [SyncVar] //���������� ���� SyncVar ����� ���������� ������ �� �������, � �� �� �������
    public NetworkIdentity IdentityOfDepartingPlayer = null; //������� ����������, � ������� ����� ��������� ������������ ������, ������� ������ ��� ������

    public Game CurrentGame;
    public Transform Hand;
    public Transform Timeline;
    public GameObject CardPref;
    public TextMeshProUGUI Answer;
    public TextMeshProUGUI Lives;
    public TextMeshProUGUI Points;

    void Start()
    {
        CurrentGame = new Game();       //������� ����� ����

        foreach (Transform child in Timeline)       //�������� ����, ���������� �� ���� ����� (������) ����-�����
            Destroy(child.gameObject);      //���������� �������� ������� (�����)
        
        foreach (Transform child in Hand)       //�������� ����, ���������� �� ���� ����� (������) ���� ������
            Destroy(child.gameObject);      //���������� �������� ������� (�����)        
        
        //GiveHandCards(CurrentGame.Deck, Hand);      //�������� ������� GiveHandCards � �������� �� ������� ������ ���� � ���� ������
        //Answer.text = string.Empty;     //������� ����� �� ���� ��� ������
        //Lives.text = $"�������� ������ {CurrentGame.Lives}";    //�������� ���������� ������
        //Lives.text = $"{CurrentGame.Lives}";    //�������� ���������� ������
        //Points.text = $"����� {CurrentGame.Points}";    //�������� ���������� �����
        //Points.text = $"{CurrentGame.Points}";    //�������� ���������� �����
    }

    //public void GiveHandCards()     //�������� ������� GiveHandCards, ����������� ���� ������ ������� �� ������
    //{
    //    int i = 0;      //������� ����� ���������� i � ������ �� �������� 0
    //    while (i++ < 6)     //�������� ����, ���������� 6 �������� (�� 0 �� 5 ������������)
    //        GiveCardToHand(CurrentGame.Deck, Hand);     //�������� ������� GiveCardToHand � �������� �� ������� ������ � ���� ������
    //}

    public void GiveCardToHand(List<Card> deck, Transform hand)        //�������� ������� GiveCardToHand, �������� � ���� 1 ����� �� ������
    {
        if(deck.Count == 0)     //������ �������: "���� ���������� ���� � ������ ����� 0 (���� ������ ������), ��..."
            return;     //���������� �������

        Card card = deck[0];        //������� ����� ��������� ���������� ��� ����� � ������ � �� �������� ����� �� ������

        GameObject cardGO = Instantiate(CardPref, hand, false);

        cardGO.GetComponent<CardInfoScript>().ShowCardInfo(card);

        deck.RemoveAt(0);       //������� �� ������ ������� (������) �����
    }

    public void EndTurn(GameObject cardGO)      //�������� ������� EndTurn
    {
        
        var i = cardGO.transform.GetSiblingIndex();     //������� ����� ���������� i � �������� �� ���������� ����� �����, ������� �� ������ ��� �������� �� ����-����.
        DateTime d = cardGO.GetComponent<CardInfoScript>().SelfCard.Date;       //������� ����� ���������� d � �������� �� �������� ���� ������� �����, ������� �� ������ ��� �������� �� ����-����
        DateTime prevCardDate = new(1, 1, 1);       //������� ����� ���������� prevCardDate � �������� �� � �������� �������� ���� 01.01.0001 - ����, ������� � ����� ������ ����� ������ ����� ���� � ����� ����
        if (i > 0)  //������ �������: "���� i ������ 0 (�� ���� ���� ���������� ����� ����� �� ����� ������), ��..."
        {
            prevCardDate = Timeline.GetChild(i - 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date;        //�������� ���������� prevCardDate �������� ���� ������� �� �����, ������� ����� ������ ��� ���������� ������
        }      

        DateTime nextCardDate = new(3000,1,1);
        if (i < Timeline.childCount - 1)        //������ �������: "���� i ������ ������� ��������� ����� �� ����-����� (�� ���� ���� ���������� ����� �� ���������), ��..."
            nextCardDate = Timeline.GetChild(i + 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date;        //�������� ���������� nextCardDate �������� ���� ������� �� �����, ������� ����� ������ ��� ���������� ������

        if (d >= prevCardDate && d <= nextCardDate)     //����� �������: "���� ���� �� ���������� ������ ��� �������� ������ ��� ����� ���� �������� ����� � ������ ���� ����� ���� �������� ������, ��..."
        {
            //Answer.text = "���������";      //������� � ���� ��� ������ ����� "���������"
            //float r = 0.15f, g = 0.3f, b = 0.0f, a = 0.6f;   //������� ���������� ��� �������� ���������� ����� � RGB (a - ������������, r, g � b - �������, ������ � ����� ��������������)
            //Answer.color = new Color(r, g, b, a);       //������� ������� � ���� ������ ���� � ��������� ������ ��� ����������� (� ������ ������ ������)
            cardGO.GetComponent<CardInfoScript>().ShowCardYear();       //���������� ��� �� �����
            GiveCardToHand(CurrentGame.Deck, Hand);     //��������� � ���� ��� ���� ����� �� ������
            //CurrentGame.Points++;   //���������� 1 ����
            //Points.text = $"����� {CurrentGame.Points}";    //������� ����� �������� �����
            //Points.text = $"{CurrentGame.Points}";    //������� ����� �������� �����
        }
        else
        {
            //CurrentGame.Lives--;    //��������� 1 �����
            //if (CurrentGame.Lives == 0)     //����� �������: "���� ���������� ������ ����� 0 (���� ����� �����������), ��..."
            //{
            //    GameOver();    //�������� ���� ������, ������ ������� Start
            //    return;     //���������� ��� �������
            //}

            //Answer.text = "�����������";        //������� � ���� ��� ������ ����� "�����������"
            //float r = 1.0f, g = 0.0f, b = 0.0f, a = 0.6f;   //������� ���������� ��� �������� ���������� ����� � RGB (a - ������������, r, g � b - �������, ������ � ����� ��������������)
            //Answer.color = new Color(r, g, b, a);       //������� ������� � ���� ������ ���� � ��������� ������ ��� ����������� (� ������ ������ �������)
            //Lives.text = $"{CurrentGame.Lives}";    //������� ����� �������� ������

            cardGO.GetComponent<Animator>().SetBool("IsWrong", true);

            cardGO.transform.SetParent(Hand);       //���������� ������������ ����� � ���� ������
        }
    }

    public void Moveleft()      //�������� ������ Moveleft
    {
        //Timeline.position  += new Vector3(3, 0, 0);   //�������� ����-���� �� 1 ��� ������
        GetComponent<PlayerManager>().CmdMoveLeft(Timeline);
    }

    public void MoveRight()      //�������� ������ MoveRight
    {
        //Timeline.position -= new Vector3(3, 0, 0);   //�������� ����-���� �� 1 ��� �����
        GetComponent<PlayerManager>().CmdMoveRight(Timeline);
    }

    public void RestartGame()       //������ ������� RestartGame, ���������� ��� ������� ������ "������ ������"
    {
        Start();        //�������� ������� Start, ������������ � ����������� ������ ��� ������� �������
    }

    public void ToMainMenu()      //������ ������� ToMainMenu, ���������� ��� ������� ������ "� ����"
    {
        SceneManager.LoadScene("MainMenu");     //������������ �� �����, ���������� ������� ����
    }

    public GameOverMenu GameOverMenu;

    public void GameOver()
    {
        GameOverMenu.Setup(CurrentGame.Points);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
