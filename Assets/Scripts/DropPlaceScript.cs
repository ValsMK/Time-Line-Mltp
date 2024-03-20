using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Скрипт вешается на области руки и таймлайна меняет родителей в скрипте CardMovementScript
///     Когда объект переносится из одной области в другую
/// </summary>
public class DropPlaceScript : NetworkBehaviour, IDropHandler, IPointerEnterHandler
{
    public FieldTypeEnum Type;  //Создаем публичную переменную Type типа FieldType
    //public GameObject Hand;  //Создаем публичную переменную Hand типа GameObject
    //public GameObject CardPref;  //Создаем публичную переменную CardPref типа GameObject

    //public void Start()
    //{
    //    Hand = GameObject.Find("PlayerHand");
    //}

    /// <summary>Событие возникает, когда мы отпускаем объект на область</summary>
    void IDropHandler.OnDrop(PointerEventData eventData) 
    {
#if DEBUG
        Debug.Log($"OnDrop: {eventData.pointerEnter.name}");
#endif

        //Получим скрипт карты, которую тащим
        var cardMovementScript = eventData.pointerDrag.GetComponent<CardMovementScript>();    //Смотрим, что за объект перетащили на тайм-лайн

        //Передадим в нее текущую область в качестве родителя для объекта
        if (cardMovementScript)       
            cardMovementScript.DefaultParent = transform; 
    }

    /// <summary>Событие возникает, когда кусрос с объектом попадают в область</summary>
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)    //Начало класса IPointerEnterHandler
    {
        //eventData.pointerDrag - ссылка на объект, который мы сейчас перетаскиваем

        //Если ничего не перетаскиваем, то ничего не делаем
        if (eventData.pointerDrag == null)  
            return;

#if DEBUG
        Debug.Log($"OnPointerEnter: {eventData.pointerEnter.name}");
#endif

        //Получим скрипт карты, которую тащим
        var cardMovementScript = eventData.pointerDrag.GetComponent<CardMovementScript>();

        //Временной карте присваиваем родителем ту область, куда затянули карту
        if (cardMovementScript) 
            cardMovementScript.DefaultTempCardParent = transform; 
    }

    //Вроде действия события дублируют предыдущее событие. Решил закомментить
    /// <summary>Событие возникает, когда кусрос с объектом покидают в область</summary>
//    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
//    {
//        //eventData.pointerDrag - ссылка на объект, который мы сейчас перетаскиваем

//        //Если ничего не перетаскиваем, то ничего не делаем
//        if (eventData.pointerDrag == null) 
//            return;

//#if DEBUG
//        Debug.Log($"OnPointerExit: {eventData.pointerEnter.name}");
//#endif

//        //Получим скрипт карты, которую тащим
//        var cardMovementScript = eventData.pointerDrag.GetComponent<CardMovementScript>();

//        if (cardMovementScript && cardMovementScript.DefaultTempCardParent == transform)
//            cardMovementScript.DefaultTempCardParent = cardMovementScript.DefaultParent;
//    }
}
