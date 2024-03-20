using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class DescriptionScript : MonoBehaviour
{
    public Image Logo;  //Добавляем переменную для хранения изображения на карте
    public TextMeshProUGUI Name;    //Добавляем переменную для хранения названия события на карте
    public TextMeshProUGUI DescriptionText;    //Добавляем переменную для хранения описания события на карте

    [Header("TestHeader") ]
    public TextMeshProUGUI testProp;

    public void CloseDescription()
    {
        Destroy(gameObject);
    }

    public void ShowCardDescription(Card card)
    {
        Logo.sprite = card.Logo;
        Name.text = card.Name;
        DescriptionText.text = card.Description;
    }
}
