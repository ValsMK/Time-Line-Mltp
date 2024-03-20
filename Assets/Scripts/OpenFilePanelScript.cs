// Подключение библиотек
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Globalization;
using SFB;

public class OpenFilePanel : MonoBehaviour
{
    // Функция GetFilePath, отвечающая за нахождение пути к сохраненному файлу с колодой
    public static string GetFilePath()
    {
        // Инициализация и заполнение переменной, хранящей путь к файлу
        //string path = EditorUtility.OpenFilePanel("Overwrite with csv", "", "csv");
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open file", "", "csv", false);
        // Возвращение значения переменной path
        return paths[0];
    }

    // Функция GetDeckFromFile, отвечающая за считывание колоды и её названия из сохраненного файла с ней
    public static List<Card> GetDeckFromFile(string path, out string deckName)
    {
        // Объявление использование инструмента, который будет считывать данные даные из файла
        using (var reader = new StreamReader(path))
        {
            // Инициализация переменной, которая будет хранить название колоды
            deckName = string.Empty;
            // Инициализация массива, в который будет записываться колода из файла
            var deck = new List<Card>();
            // Инициализация служебной переменной, отвечающей за условие, является ли текущая строка первой
            bool isFirstLine = true;
            // Цикл до конца файла
            while (!reader.EndOfStream)
            {
                // Проверка, является ли текущая строка первой
                if (isFirstLine)
                {
                    // Запись первой строки файла в переменную с названием колоды
                    deckName = reader.ReadLine();
                    // Изменение условия isFirstLine на false
                    isFirstLine = false;
                } 
                else
                {
                    // Считывание текущей строки из файла и запись её в переменную line
                    var line = reader.ReadLine();
                    // Разбитие строки line на дату, название и описание события по символу ";"
                    var values = line.Split(';');
                    // Создание карты из данных, полученных из текущей строки
                    var card = new Card(values[1], string.Empty, DateTime.Parse(values[0], CultureInfo.CreateSpecificCulture("ru-RU")), values[2]);
                    // Добавление этой карты в возвращаемую колоду
                    deck.Add(card);
                }
            }
            // Возвращение считанной колоды
            return deck;
        }
    }

    // Функция SaveDeckToFile, отвечающая за сохранение колоды в служебный файл UserDeck.csv
    public static void SaveDeckToFile(List<Card> deck, string deckName)
    {
        // Объявление переменной, хранящей имя служебного файла
        string fileName = @"UserDeck.csv";

        // Проверка на то, есть ли уже в данной папке файл с названием "fileName"
        if (File.Exists(fileName))
        {
            // Если такой файл найден, удалить его
            File.Delete(fileName);
        }

        // Создание нового файла с названием "fileName"
        File.Create(fileName).Dispose();

        // Объявление использование инструмента, который будет записывать данные в созданный файл
        using (var writer = new StreamWriter(fileName, true))
        {
            // Запись первой строкой в файле навзвания колоды
            writer.WriteLine(deckName);

            // Цикл по всем элементам (картам) колоды
            for (int i = 0; i < deck.Count; i++)
            {
                // Запись текущей карты в файл
                writer.WriteLine(deck[i].ToString());
            }
        }
    }
}