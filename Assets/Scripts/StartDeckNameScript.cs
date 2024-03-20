using TMPro;    // Подключение библиотеки TMPro
using UnityEngine;  // Подключение библиотеки UnityEngine
using System.IO;

/// <summary>
/// Скрипт отвечает за отображение сохранённого названия колоды в спецциальном поле в начале игры
/// </summary>
public class StartDeckNameScript : MonoBehaviour
{
    [SerializeField] GameObject DeckName;   // Инициализация поля для получения ссылки на игровой объект, отображающий название колоды
    public string SaveDeckName = "DECK_NAME";   // Инициализация служебной перемнной, нужной для получения и записи названия колоды в реестр

    void Start()    // Начало функции Start, запускающейся 1 раз при запуске игры
    {
        var t = DeckName.GetComponent<TextMeshProUGUI>();   // Получение ссылки на текстовое поле для отображения названия колоды

        if (PlayerPrefs.HasKey(SaveDeckName) && File.Exists(@"UserDeck.csv"))   // Проверка, есть ли сохраненное название колоды
        {
            t.text = PlayerPrefs.GetString(SaveDeckName);   // Изменение названия колоды
        }
    }
}
