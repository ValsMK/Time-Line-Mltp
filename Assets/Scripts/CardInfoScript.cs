using UnityEngine;     //���������� ���������� UnityEngine
using TMPro;     //���������� ���������� TMPro
using UnityEngine.UI;     //���������� ���������� UnityEngine.UI
using System;     //���������� ���������� System

/// <summary>
///  ������ ��������� ����������� ��������� �����. 
///  �������� �� ����� ��������, ��������, ������� � ���
/// </summary>
public class CardInfoScript : MonoBehaviour
{
    public Card SelfCard;   //��������� ���������� ��� ��������� ������ ��������� �����
    public Image Logo;  //��������� ���������� ��� �������� ����������� �� �����
    public TextMeshProUGUI Name;    //��������� ���������� ��� �������� �������� ������� �� �����
    public TextMeshProUGUI Year;    //��������� ���������� ��� �������� ���� �������\
    public Button ShowDescriptionButton;
    public GameObject DescriptionPref;
  
    public void ShowCardInfo(Card card)     //������ ������� ShowCardInfo, ���������� �� ����� ���������� �� �����
    {
        SelfCard = card;    //��������� ����� ��� ���������

        //Logo.sprite = card.Logo;
        //Logo.preserveAspect = true;
        //Name.text = card.Name.Substring(0,Math.Min(card.Name.Length-1,30));

        Name.text = card.Name;
        Year.text = $"{card.Date:yyyy}";
        Year.enabled = false;   //��������� ����� ����

        ShowDescriptionButton.gameObject.SetActive(card.Description != String.Empty);

    }

    public void ShowCardYear()      //������ ������� ShowCardYear, ���������� �� ����� ���� �� �����
    {
        Year.enabled = true;    //�������� ����� ����
    }

    public void OpenDescription()
    {
        GameObject DescriptionGO = Instantiate(DescriptionPref, transform.parent.parent, false);
        DescriptionGO.GetComponent<DescriptionScript>().ShowCardDescription(SelfCard);

    }
}
