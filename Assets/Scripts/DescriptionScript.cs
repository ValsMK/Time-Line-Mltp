using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class DescriptionScript : MonoBehaviour
{
    public Image Logo;  //��������� ���������� ��� �������� ����������� �� �����
    public TextMeshProUGUI Name;    //��������� ���������� ��� �������� �������� ������� �� �����
    public TextMeshProUGUI DescriptionText;    //��������� ���������� ��� �������� �������� ������� �� �����

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
