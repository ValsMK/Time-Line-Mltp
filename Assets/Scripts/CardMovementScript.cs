using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     ������ ��������� ����������� ������� �� ������
/// </summary>
public class CardMovementScript : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    [SyncVar]
    public int StartIndex;
    /// <summary> �������� �� ��������� ��� �������</summary>
    public Transform DefaultParent;
    
    /// <summary> �������� �� ��������� ��� "��������� �����" </summary>
    public Transform  DefaultTempCardParent;

    /// <summary> ���� ����������, ����� �� ������������� ������ (��������� �� �� � ���� ������) (��������������� � OnBeginDrag)</summary>    
    private bool _isDraggable;

    /// <summary> ������� ������ (� ��� ������������, ������� ����� ������ �� �������)</summary>
    private Camera _mainCamera;

    /// <summary> ����� ��������� ������� ������������ ������ �������  (��������������� � OnBeginDrag)</summary>    
    private Vector3 _offset;

    private GameObject Answer;

    /// <summary>
    ///     ������ "��������� �����". 
    ///     ����� �������������� ����� ���������� ������ ������ � ���� �����
    /// </summary>
    private GameObject _tempCardGameObject;      //��������� ���������� TempCardGO

    private GameObject _timeLine;      //��������� ���������� TempCardGO

    private GameObject _playerHand;      //��������� ���������� TempCardGO

    //����� ���������� �� ������ ����� ������� � ����� ������������ �������
    void Awake()        
    {
        //������ � ��� ����. ��� �� �������
        _mainCamera = Camera.allCameras[0];      //���������� ������
        //������ ������ �� ��������
        _tempCardGameObject = GameObject.Find("TempCardGO");     //����������� ���������� TempCardGO ��������������� ������ ����������
        _timeLine = GameObject.Find("Line");     //����������� ���������� TempCardGO ��������������� ������ ����������
        _playerHand = GameObject.Find("PlayerHand");     //����������� ���������� TempCardGO ��������������� ������ ����������
        Answer = GameObject.Find("Answer");
    }

    /// <summary>������� ��������� ��� ������ �������������� �������</summary>
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)      //������� ������� ��� ������ ����������� �����
    {
#if DEBUG
        Debug.Log($"OnBeginDrag: {eventData.pointerEnter.name}");
#endif

        //�������� �����, ��� ����� ������� ����� ���������� ������� (������������ ���: _mainCamera.ScreenToWorldPoint(eventData.position)) 
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);

        //��������� �������� ������� � ��������� �����. ����� �������� �������� �������, ������� ��������
        DefaultParent = DefaultTempCardParent = transform.parent;

        //�� �������� ������� ���� ������ DropPlaceScript, � � ��� �������� ��� ����. ����� ���������� ������ �� �������, ��� �������� � ������� "FieldType.HAND"
        _isDraggable = DefaultParent.GetComponent<DropPlaceScript>().Type == FieldTypeEnum.Hand;

        //������� GameManagerScript, � ������� ���������� ���������� IdentityOfDepartingPlayer, ����� �������� �
        //var _gameManagerS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        //IsPlayersTurn = _gameManagerS.IdentityOfDepartingPlayer == null || _gameManagerS.IdentityOfDepartingPlayer != networkIdentity;

        if (!_isDraggable)
            return;

        var ind = transform.GetSiblingIndex();
        StartIndex = ind;
        Debug.Log($"cardindex: {ind}");
        //��������� ����� �������� �������� �� ��������
        _tempCardGameObject.transform.SetParent(DefaultTempCardParent); 
        //� ������ � HorizontalLayoutGroup
        _tempCardGameObject.transform.SetSiblingIndex(ind);

        //������� �������� ������ �� NetworkIdentity, � ����� ��� ������ PlayerManagerS
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        var _playerManagerS = networkIdentity.GetComponent<PlayerManager>();
        _playerManagerS.CmdIncreaseCardSize(ind);

        //��������� ����� �������� �������� �� ��������
        transform.SetParent(DefaultParent.parent); 
        //��������� ����� �� �����. ������ �������� ��������� ������������� ������� ������ �������� �� ����� ��������������
        //����� ��� ������ ������� DropPlaceScript, ������� ����� �� ���� � ���������
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>
    /// ������� ��������� �� ����� �������������� ������� (����������� �� ������ �������� ����)
    /// ������� ���������� ������ OnDrop �� ������� DropPlaceScript, ����� ���
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)  
    {
        if (!_isDraggable)
            return;

        //�������� ���������� �������
        var newPos = _mainCamera.ScreenToWorldPoint(eventData.position);

        //����������� ����� ������� �������
        transform.position = newPos + _offset;

        //������ �������� ��������� �����, ���� �� ����������� ������ �� ������ �������
        //DefaultTempCardParent - �������� � ������� DropPlaceScript
        if (_tempCardGameObject.transform.parent != DefaultTempCardParent)
            _tempCardGameObject.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }

    /// <summary>������� ��������� ��� ��������� ����������� �������</summary>
    void IEndDragHandler.OnEndDrag(PointerEventData eventData) 
    {
        Debug.Log($"OnEndDrag: {eventData.pointerEnter.name}");

        if (!_isDraggable)
            return;

        //�������� ������� ��������, ������� ��������� � ������� DropPlaceScript (���� ����, ���� ��������)
        transform.SetParent(DefaultParent);

        //������ ������ HorizontalLayoutGroup ������� �� ��������� ����� 
        transform.SetSiblingIndex(_tempCardGameObject.transform.GetSiblingIndex());

        //��������� � �������� �������� ��������� ����� Canvas
        var canvasTransform = GameObject.Find("Canvas").transform;
        _tempCardGameObject.transform.SetParent(canvasTransform);
        //������ ��������� ����� �� ����
        _tempCardGameObject.transform.localPosition = new Vector3(2388, 0,100);

        //������� ����� ��������, ������� ��������� � OnBeginDrag
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        //���� ����� ����� �� ��������, ������� �� ������������ � �������     
        if (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldTypeEnum.Timeline)
        {
            //������� ����� ���������� i � �������� �� ���������� ����� �����, ������� �� ������ ��� �������� �� ����-����.
            var i = transform.GetSiblingIndex();
            //������� ����� ���������� d � �������� �� �������� ���� ������� �����, ������� �� ������ ��� �������� �� ����-����
            DateTime d = GetComponent<CardInfoScript>().SelfCard.Date;
            //������� ���� �� ���������� ����� �� ���� �����, ���� �� �������� ����
            DateTime prevCardDate = new(1, 1, 1); 
            if (i > 0)  
                prevCardDate = _timeLine.transform.GetChild(i - 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date; 
            

            //������� ���� �� ��������� ����� �� ���� �����, ���� �� �������� ����
            DateTime nextCardDate = new(3000, 1, 1);
            if (i < _timeLine.transform.childCount - 1) 
                nextCardDate = _timeLine.transform.GetChild(i + 1).gameObject.GetComponent<CardInfoScript>().SelfCard.Date;

            //������� ������ PlayerManager 
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            var playerManager = networkIdentity.GetComponent<PlayerManager>();

            //���� ����� �������� �� �������� ���������
            if (d >= prevCardDate && d <= nextCardDate)     
            {
                //�������� ������� EndTurn �� ������� PlayerManager
                playerManager.CmdEndTurn(gameObject, i);

                playerManager.CmdBlockHand();
                playerManager.CmdShowGameInfo();

                Answer.transform.GetComponent<TextMeshProUGUI>().text = "���������";
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

                //������� ��� ����� � ����� � ������ ������ �����   
                playerManager.CmdDiscardCard(gameObject);
                playerManager.CmdGiveCardToHand();

                playerManager.CmdBlockHand();
                playerManager.CmdShowGameInfo();

                Answer.transform.GetComponent<TextMeshProUGUI>().text = "�����������";
                float r = 1.0f, g = 0.0f, b = 0.0f, a = 0.6f;
                Answer.transform.GetComponent<TextMeshProUGUI>().color = new Color(r, g, b, a);
            }
        }

        if (gameObject.transform.parent == _playerHand.transform)
        {
            Debug.Log($"cardindex: {gameObject.transform.GetSiblingIndex()}");
            //������� ������ PlayerManager
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            var playerManager = networkIdentity.GetComponent<PlayerManager>();
            playerManager.CmdReplaceCard(StartIndex, gameObject.transform.GetSiblingIndex());
            playerManager.CmdDecreaseCardSize(gameObject.transform.GetSiblingIndex());
        }
    }

    /// <summary>
    ///     ����� ���������� ������ ��������� ����� ������ HorizontalLayoutGroup
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