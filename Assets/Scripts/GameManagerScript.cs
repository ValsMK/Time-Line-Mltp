using Mirror;
using System;     //Подключаем библиотеку System
using System.Collections.Generic;     //Подключаем библиотеку System.Collections.Generic
using TMPro;     //Подключаем библиотеку TMPro
using UnityEngine;     //Подключаем библиотеку UnityEngine
using UnityEngine.SceneManagement;

public enum DeckTypeEnum
{
    GeneralHistoryXVIIICentury = 1,
    RussianHistoryXVIIICentury = 2,
    UserDeck = 3
}

public class Game
{
    public List<Card> Deck;     //Создаем новый массив карт - колоду
    public int Lives;       //Создаем переменную, хранящюю количество жизней
    public int Points;      //Создаем переменную, хранящюю количество очков
    public DeckTypeEnum DeckType = DeckTypeEnum.GeneralHistoryXVIIICentury;
    public string SaveDeckKey = "DECK_KEY";

    public Game()
    {
        Lives = 5;      //Задаем начальное количество жизней
        Points = 0;      //Задаем начальное количество очков

        if (PlayerPrefs.HasKey(SaveDeckKey))
        {
            DeckType = (DeckTypeEnum)PlayerPrefs.GetFloat(SaveDeckKey);
        }
        else
        {
            DeckType = DeckTypeEnum.GeneralHistoryXVIIICentury;
            PlayerPrefs.SetFloat(this.SaveDeckKey, (int)DeckTypeEnum.GeneralHistoryXVIIICentury);
        }

        Deck = GiveDeckCard();      //Создаем колоду карт с помощью функции GiveDeckCard
    }

    public void RedeckDeck() => Deck = GiveDeckCard();

    List<Card> GiveDeckCard()      //Начало функции GiveDeckCard
    {
        List<Card> list = new();      //Создаем новый пустой массив карт
        List<Card> tmpList = new();           //Создаем новый временный массив карт, содержащий все карты в игре

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

        for (int i = tmpList.Count - 1; i >=0 ; i--)      //Начинаем цикл, совершающий столько итераций, сколько карт в игре
        {
            int r = UnityEngine.Random.Range(0, tmpList.Count - 1);      //Выбираем случайное число от нуля до количества карт в игре и записываем его в переменную r
            list.Add(tmpList[r]);      //Добавляем в наш список карт карту с номером r из временного списка
            tmpList.RemoveAt(r);      //Удаляем карту с номером r из временного списка, чтобы случайно не добавить её в основной список повторно
        }
        return list;      //Возвращаем получившуюся колоду
    }
}

public class GameManagerScript : NetworkBehaviour
{
    [SyncVar] //Переменные типа SyncVar могут изменяться только на сервере, а не на клиенте
    public NetworkIdentity IdentityOfDepartingPlayer = null; //Создаем переменную, в которой будет храниться иднтефикатор игрока, который только что сходил

    public Game CurrentGame;
    public Transform Hand;
    public Transform Timeline;
    public GameObject CardPref;
    public TextMeshProUGUI Answer;
    public TextMeshProUGUI Lives;
    public TextMeshProUGUI Points;

    void Start()
    {
        CurrentGame = new Game();       //Создаем новую игру

        foreach (Transform child in Timeline)       //Начинаем цикл, проходящий по всем детям (картам) тайм-лайна
            Destroy(child.gameObject);      //Уничтожаем текущего ребенка (карту)
        
        foreach (Transform child in Hand)       //Начинаем цикл, проходящий по всем детям (картам) руки игрока
            Destroy(child.gameObject);      //Уничтожаем текущего ребенка (карту)        
        
        //GiveHandCards(CurrentGame.Deck, Hand);      //Вызываем функцию GiveHandCards и передаем ей текущую колоду карт и руку игрока
        //Answer.text = string.Empty;     //Убираем текст из поля для ответа
        //Lives.text = $"Осталось жизней {CurrentGame.Lives}";    //Обнуляем количество жизней
        //Lives.text = $"{CurrentGame.Lives}";    //Обнуляем количество жизней
        //Points.text = $"Очков {CurrentGame.Points}";    //Обнуляем количество очков
        //Points.text = $"{CurrentGame.Points}";    //Обнуляем количество очков
    }

    //public void GiveHandCards()     //Начинаем функцию GiveHandCards, заполнающюю руку игрока картами из колоды
    //{
    //    int i = 0;      //Создаем новую переменную i и задаем ей значение 0
    //    while (i++ < 6)     //Начинаем цикл, проходящий 6 итераций (от 0 до 5 включительно)
    //        GiveCardToHand(CurrentGame.Deck, Hand);     //Вызываем функцию GiveCardToHand и передаем ей текущие колоду и руку игрока
    //}

    public void GiveCardToHand(List<Card> deck, Transform hand)        //Начинаеи функцию GiveCardToHand, выдающюю в руку 1 карту из колоды
    {
        if(deck.Count == 0)     //Задаем условие: "Если количество карт в колоде равно 0 (Если колода пустая), то..."
            return;     //Прекращаем функцию

        Card card = deck[0];        //Создаем новую временную переменную для карты и кладем в неё вернхнюю карту из колоды

        GameObject cardGO = Instantiate(CardPref, hand, false);

        cardGO.GetComponent<CardInfoScript>().ShowCardInfo(card);

        deck.RemoveAt(0);       //Удаляем из колоды верхнюю (первую) карту
    }

    public void EndTurn(GameObject cardGO)      //Начинаем функцию EndTurn
    {
        
        var i = cardGO.transform.GetSiblingIndex();     //Создаем новую переменную i и передаем ей порядковый номер карты, которую мы только что положили на тайм-лайн.
        DateTime d = cardGO.GetComponent<CardInfoScript>().SelfCard.Date;       //Создаем новую переменную d и передаем ей значение даты события карты, которую мы только что положили на тайм-лайн
        DateTime prevCardDate = new(1, 1, 1);       //Создаем новую переменную prevCardDate и передаем ей в качестве значения дату 01.01.0001 - дату, которая в любом случае будет меньше любой даты в нашей игре
        if (i > 0)  //Задаем условие: "Если i больше 0 (то есть если выложенная карта лежит не самой первой), то..."
        {
            prevCardDate = Timeline.GetChild(i - 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date;        //Передаем переменной prevCardDate значение даты события на карте, лежащей перед только что выложенной картой
        }      

        DateTime nextCardDate = new(3000,1,1);
        if (i < Timeline.childCount - 1)        //Задаем условие: "Если i меньше индекса последней карты на тайм-лайне (то есть если выложенная карта не последняя), то..."
            nextCardDate = Timeline.GetChild(i + 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date;        //Передаем переменной nextCardDate значение даты события на карте, лежащей после только что выложенной картой

        if (d >= prevCardDate && d <= nextCardDate)     //Задаём условие: "Если дата на выложенной только что карточке больше или равна дате карточки слева и меньше либо равна дате карточки справа, то..."
        {
            //Answer.text = "Правильно";      //Выводим в поле для ответа слово "Правильно"
            //float r = 0.15f, g = 0.3f, b = 0.0f, a = 0.6f;   //Создаем переменные для хранения параметров цвета в RGB (a - прозрачность, r, g и b - красный, зелёный и синий соответственно)
            //Answer.color = new Color(r, g, b, a);       //Придаем надписи в поле ответа цвет с заданными тоьлко что параметрами (в данном случае зелёный)
            cardGO.GetComponent<CardInfoScript>().ShowCardYear();       //Показываем год на карте
            GiveCardToHand(CurrentGame.Deck, Hand);     //Добавляем в руку ещё одну карту из колоды
            //CurrentGame.Points++;   //Прибавляем 1 очко
            //Points.text = $"Очков {CurrentGame.Points}";    //Выводим новое значение очков
            //Points.text = $"{CurrentGame.Points}";    //Выводим новое значение очков
        }
        else
        {
            //CurrentGame.Lives--;    //Вычитаеим 1 жизнь
            //if (CurrentGame.Lives == 0)     //Задаём условие: "Если количество жизней равно 0 (Если жизни закончились), то..."
            //{
            //    GameOver();    //Начинаем игру заново, вызвав функцию Start
            //    return;     //Прекращаем эту функцию
            //}

            //Answer.text = "Неправильно";        //Выводим в поле для ответа слово "Неправильно"
            //float r = 1.0f, g = 0.0f, b = 0.0f, a = 0.6f;   //Создаем переменные для хранения параметров цвета в RGB (a - прозрачность, r, g и b - красный, зелёный и синий соответственно)
            //Answer.color = new Color(r, g, b, a);       //Придаем надписи в поле ответа цвет с заданными тоьлко что параметрами (в данном случае красный)
            //Lives.text = $"{CurrentGame.Lives}";    //Выводим новое значение жизней

            cardGO.GetComponent<Animator>().SetBool("IsWrong", true);

            cardGO.transform.SetParent(Hand);       //Возвращаем передвинутую карту в руку игрока
        }
    }

    public void Moveleft()      //Начинаем фунцию Moveleft
    {
        //Timeline.position  += new Vector3(3, 0, 0);   //Сдвигаем тайм-лайн на 1 шаг вправо
        GetComponent<PlayerManager>().CmdMoveLeft(Timeline);
    }

    public void MoveRight()      //Начинаем фунцию MoveRight
    {
        //Timeline.position -= new Vector3(3, 0, 0);   //Сдвигаем тайм-лайн на 1 шаг влево
        GetComponent<PlayerManager>().CmdMoveRight(Timeline);
    }

    public void RestartGame()       //Начало функции RestartGame, вызываемой при нажатии кнопки "Начать заново"
    {
        Start();        //Вызываем функцию Start, прекращающюю и запускающюю заново все игровые события
    }

    public void ToMainMenu()      //Начало функции ToMainMenu, вызываемой при нажатии кнопки "В меню"
    {
        SceneManager.LoadScene("MainMenu");     //Переключаемя на сцену, содержащюю главное меню
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
