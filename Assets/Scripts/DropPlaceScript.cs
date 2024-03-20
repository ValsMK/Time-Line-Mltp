using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     ������ �������� �� ������� ���� � ��������� ������ ��������� � ������� CardMovementScript
///     ����� ������ ����������� �� ����� ������� � ������
/// </summary>
public class DropPlaceScript : NetworkBehaviour, IDropHandler, IPointerEnterHandler
{
    public FieldTypeEnum Type;  //������� ��������� ���������� Type ���� FieldType
    //public GameObject Hand;  //������� ��������� ���������� Hand ���� GameObject
    //public GameObject CardPref;  //������� ��������� ���������� CardPref ���� GameObject

    //public void Start()
    //{
    //    Hand = GameObject.Find("PlayerHand");
    //}

    /// <summary>������� ���������, ����� �� ��������� ������ �� �������</summary>
    void IDropHandler.OnDrop(PointerEventData eventData) 
    {
#if DEBUG
        Debug.Log($"OnDrop: {eventData.pointerEnter.name}");
#endif

        //������� ������ �����, ������� �����
        var cardMovementScript = eventData.pointerDrag.GetComponent<CardMovementScript>();    //�������, ��� �� ������ ���������� �� ����-����

        //��������� � ��� ������� ������� � �������� �������� ��� �������
        if (cardMovementScript)       
            cardMovementScript.DefaultParent = transform; 
    }

    /// <summary>������� ���������, ����� ������ � �������� �������� � �������</summary>
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)    //������ ������ IPointerEnterHandler
    {
        //eventData.pointerDrag - ������ �� ������, ������� �� ������ �������������

        //���� ������ �� �������������, �� ������ �� ������
        if (eventData.pointerDrag == null)  
            return;

#if DEBUG
        Debug.Log($"OnPointerEnter: {eventData.pointerEnter.name}");
#endif

        //������� ������ �����, ������� �����
        var cardMovementScript = eventData.pointerDrag.GetComponent<CardMovementScript>();

        //��������� ����� ����������� ��������� �� �������, ���� �������� �����
        if (cardMovementScript) 
            cardMovementScript.DefaultTempCardParent = transform; 
    }

    //����� �������� ������� ��������� ���������� �������. ����� ������������
    /// <summary>������� ���������, ����� ������ � �������� �������� � �������</summary>
//    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
//    {
//        //eventData.pointerDrag - ������ �� ������, ������� �� ������ �������������

//        //���� ������ �� �������������, �� ������ �� ������
//        if (eventData.pointerDrag == null) 
//            return;

//#if DEBUG
//        Debug.Log($"OnPointerExit: {eventData.pointerEnter.name}");
//#endif

//        //������� ������ �����, ������� �����
//        var cardMovementScript = eventData.pointerDrag.GetComponent<CardMovementScript>();

//        if (cardMovementScript && cardMovementScript.DefaultTempCardParent == transform)
//            cardMovementScript.DefaultTempCardParent = cardMovementScript.DefaultParent;
//    }
}
