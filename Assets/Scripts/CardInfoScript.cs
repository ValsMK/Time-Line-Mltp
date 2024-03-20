using UnityEngine;     //Подключаем библиотеку UnityEngine
using TMPro;     //Подключаем библиотеку TMPro
using UnityEngine.UI;     //Подключаем библиотеку UnityEngine.UI
using System;     //Подключаем библиотеку System

/// <summary>
///  Скрипт описывает отображение игральной карты. 
///  Вываодит на карту картинку, название, опиание и год
/// </summary>
public class CardInfoScript : MonoBehaviour
{
    public Card SelfCard;   //Добавляем переменную для обработки каждой отдельной карты
    public Image Logo;  //Добавляем переменную для хранения изображения на карте
    public TextMeshProUGUI Name;    //Добавляем переменную для хранения названия события на карте
    public TextMeshProUGUI Year;    //Добавляем переменную для хранения даты события\
    public Button ShowDescriptionButton;
    public GameObject DescriptionPref;
  
    public void ShowCardInfo(Card card)     //Начало функции ShowCardInfo, отвечающей за показ информации на карте
    {
        SelfCard = card;    //Принимаем карту для обработки

        //Logo.sprite = card.Logo;
        //Logo.preserveAspect = true;
        //Name.text = card.Name.Substring(0,Math.Min(card.Name.Length-1,30));

        Name.text = card.Name;
        Year.text = $"{card.Date:yyyy}";
        Year.enabled = false;   //Отключаем показ года

        ShowDescriptionButton.gameObject.SetActive(card.Description != String.Empty);

    }

    public void ShowCardYear()      //Начало функции ShowCardYear, отвечающей за показ года на карте
    {
        Year.enabled = true;    //Включаем показ года
    }

    public void OpenDescription()
    {
        GameObject DescriptionGO = Instantiate(DescriptionPref, transform.parent.parent, false);
        DescriptionGO.GetComponent<DescriptionScript>().ShowCardDescription(SelfCard);

    }
}
