using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Скрипт описывает перемещение объекта по экрану
/// </summary>
public class CardMovementScript : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    [SyncVar]
    public int StartIndex;
    /// <summary> Родитель по умолчанию для объекта</summary>
    public Transform DefaultParent;
    
    /// <summary> Родитель по умолчанию для "Временной карты" </summary>
    public Transform  DefaultTempCardParent;

    /// <summary> Флаг показывает, можно ли перетаскивать объект (находится ли он в руке игрока) (устанавливается в OnBeginDrag)</summary>    
    private bool _isDraggable;

    /// <summary> Главная камера (у нас единственная, поэтому берем первую по индексу)</summary>
    private Camera _mainCamera;

    /// <summary> Сдвиг координат курсора относительно ценрта объекта  (устанавливается в OnBeginDrag)</summary>    
    private Vector3 _offset;

    private GameObject Answer;

    /// <summary>
    ///     Объект "Временная карта". 
    ///     Серая полупрозрачная карта появляется вместо взятой с руки карты
    /// </summary>
    private GameObject _tempCardGameObject;      //Объявляем переменную TempCardGO

    private GameObject _timeLine;      //Объявляем переменную TempCardGO

    private GameObject _playerHand;      //Объявляем переменную TempCardGO

    //Метод вызывается до начала любых методов и после иницилизации префаба
    void Awake()        
    {
        //Камера у нас одна. Она же главная
        _mainCamera = Camera.allCameras[0];      //Определяем камеру
        //Найдем обхект по названию
        _tempCardGameObject = GameObject.Find("TempCardGO");     //Присваиваем переменной TempCardGO соответствующий объект интерфейса
        _timeLine = GameObject.Find("Line");     //Присваиваем переменной TempCardGO соответствующий объект интерфейса
        _playerHand = GameObject.Find("PlayerHand");     //Присваиваем переменной TempCardGO соответствующий объект интерфейса
        Answer = GameObject.Find("Answer");
    }

    /// <summary>Событие возникает при начале перетаскивания объекта</summary>
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)      //Создаем функцию для начала перемещения карты
    {
#if DEBUG
        Debug.Log($"OnBeginDrag: {eventData.pointerEnter.name}");
#endif

        //опредлим сдвиг, как центр объекта минус координаты курсора (определяются так: _mainCamera.ScreenToWorldPoint(eventData.position)) 
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);

        //Объявляем родителя объекта и временной карты. Берем текущего родителя объекта, который схватили
        DefaultParent = DefaultTempCardParent = transform.parent;

        //На родителе доолжен быть скрипт DropPlaceScript, а в нем объявлен тип поля. Будем перемещать только те объектв, тип родителя у которых "FieldType.HAND"
        _isDraggable = DefaultParent.GetComponent<DropPlaceScript>().Type == FieldTypeEnum.Hand;

        //Находим GameManagerScript, в котором содержится переменная IdentityOfDepartingPlayer, чтобы изменять её
        //var _gameManagerS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //IsPlayersTurn = _gameManagerS.IdentityOfDepartingPlayer == null || _gameManagerS.IdentityOfDepartingPlayer != networkIdentity;

        if (!_isDraggable)
            return;

        var ind = transform.GetSiblingIndex();
        StartIndex = ind;
        Debug.Log($"cardindex: {ind}");
        //Временной карте присвоим родителя по умочанию
        _tempCardGameObject.transform.SetParent(DefaultTempCardParent); 
        //И индекс в HorizontalLayoutGroup
        _tempCardGameObject.transform.SetSiblingIndex(ind);

        //Находим текущего игрока по NetworkIdentity, а также его скрипт PlayerManagerS
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        var _playerManagerS = networkIdentity.GetComponent<PlayerManager>();
        _playerManagerS.CmdIncreaseCardSize(ind);

        //Временной карте присвоим родителя по умочанию
        transform.SetParent(DefaultParent.parent); 
        //Компонент висит на карте. Данное свойство позволяет перехватывать события других объектов во время перетаскивания
        //Нужны для работы скрпита DropPlaceScript, который висит на руке и таймлайне
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>
    /// Событие возникает во время перетаскниания объекта (срабатывает на каждое движение мыши)
    /// Сначала вызывается собтие OnDrop из скрипта DropPlaceScript, потом это
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)  
    {
        if (!_isDraggable)
            return;

        //Получаем координаты курсора
        var newPos = _mainCamera.ScreenToWorldPoint(eventData.position);

        //Присваиваем карте позицию курсора
        transform.position = newPos + _offset;

        //Меняем родителя временной карты, если мы переместили курсор на другую область
        //DefaultTempCardParent - меняется в скрипте DropPlaceScript
        if (_tempCardGameObject.transform.parent != DefaultTempCardParent)
            _tempCardGameObject.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }

    /// <summary>Событие возникает при окончании перемещения объекта</summary>
    void IEndDragHandler.OnEndDrag(PointerEventData eventData) 
    {
        Debug.Log($"OnEndDrag: {eventData.pointerEnter.name}");

        if (!_isDraggable)
            return;

        //Присвоим объекту родителя, который определен в скрипте DropPlaceScript (либо рука, либо таймлайн)
        transform.SetParent(DefaultParent);

        //Индекс внутри HorizontalLayoutGroup возьмем от временной карты 
        transform.SetSiblingIndex(_tempCardGameObject.transform.GetSiblingIndex());

        //Установим в качестве родителя сременной карты Canvas
        var canvasTransform = GameObject.Find("Canvas").transform;
        _tempCardGameObject.transform.SetParent(canvasTransform);
        //Уберем временную карты за поле
        _tempCardGameObject.transform.localPosition = new Vector3(2388, 0,100);

        //Включим назад свойство, которые выключили в OnBeginDrag
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        //Если карта упала на таймлайн, проврим ее правильность в скрипте     
        if (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldTypeEnum.Timeline)
        {
            //Создаем новую переменную i и передаем ей порядковый номер карты, которую мы только что положили на тайм-лайн.
            var i = transform.GetSiblingIndex();
            //Создаем новую переменную d и передаем ей значение даты события карты, которую мы только что положили на тайм-лайн
            DateTime d = GetComponent<CardInfoScript>().SelfCard.Date;
            //Получим дату на предыдущей карте от того места, куда мы положили свою
            DateTime prevCardDate = new(1, 1, 1); 
            if (i > 0)  
                prevCardDate = _timeLine.transform.GetChild(i - 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date; 
            

            //Получим дату на следующей карте от того места, куда мы положили свою
            DateTime nextCardDate = new(3000, 1, 1);
            if (i < _timeLine.transform.childCount - 1) 
                nextCardDate = _timeLine.transform.GetChild(i + 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date;

            //получим скрипт PlayerManager 
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            var playerManager = networkIdentity.GetComponent<PlayerManager>();

            //Если карта положена на таймлайн правильно
            if (d >= prevCardDate && d <= nextCardDate)     
            {
                //Запустим команду EndTurn из скрипта PlayerManager
                playerManager.CmdEndTurn(gameObject, i);

                playerManager.CmdBlockHand();
                playerManager.CmdShowGameInfo();

                Answer.transform.GetComponent<TextMeshProUGUI>().text = "Правильно";
                float r = 0.15f, g = 0.3f, b = 0.0f, a = 0.6f;
                Answer.transform.GetComponent<TextMeshProUGUI>().color = new Color(r, g, b, a);


                if (_playerHand.transform.childCount == 0)
                {
                    playerManager.CmdEndGame();
                }

            }
            else
            {
                GetComponent<Animator>().SetBool("IsWrong", true);

                //Убираем эту карту в сброс и выдаем игроку новую   
                playerManager.CmdDiscardCard(gameObject);
                playerManager.CmdGiveCardToHand();

                playerManager.CmdBlockHand();
                playerManager.CmdShowGameInfo();

                Answer.transform.GetComponent<TextMeshProUGUI>().text = "Неправильно";
                float r = 1.0f, g = 0.0f, b = 0.0f, a = 0.6f;
                Answer.transform.GetComponent<TextMeshProUGUI>().color = new Color(r, g, b, a);
            }
        }

        if (gameObject.transform.parent == _playerHand.transform)
        {
            Debug.Log($"cardindex: {gameObject.transform.GetSiblingIndex()}");
            //Получим скрипт PlayerManager
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            var playerManager = networkIdentity.GetComponent<PlayerManager>();
            playerManager.CmdReplaceCard(StartIndex, gameObject.transform.GetSiblingIndex());
            playerManager.CmdDecreaseCardSize(gameObject.transform.GetSiblingIndex());
        }
    }

    /// <summary>
    ///     Метод определяет индекс временной карты внутри HorizontalLayoutGroup
    /// </summary>
    private void CheckPosition() 
    {
        int newIndex = DefaultTempCardParent.childCount;
        for (int i = 0; i < DefaultTempCardParent.childCount; i++)       
        {
            if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;
                //Debug.Log(newIndex);

                if (_tempCardGameObject.transform.GetSiblingIndex() < newIndex)
                    newIndex--;

                //Debug.Log(newIndex);

                break;
            }
        }
       _tempCardGameObject.transform.SetSiblingIndex(newIndex);
    }
}