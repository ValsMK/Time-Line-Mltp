// ����������� ���������
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Globalization;
using SFB;

public class OpenFilePanel : MonoBehaviour
{
    // ������� GetFilePath, ���������� �� ���������� ���� � ������������ ����� � �������
    public static string GetFilePath()
    {
        // ������������� � ���������� ����������, �������� ���� � �����
        //string path = EditorUtility.OpenFilePanel("Overwrite with csv", "", "csv");
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open file", "", "csv", false);
        // ����������� �������� ���������� path
        return paths[0];
    }

    // ������� GetDeckFromFile, ���������� �� ���������� ������ � � �������� �� ������������ ����� � ���
    public static List<Card> GetDeckFromFile(string path, out string deckName)
    {
        // ���������� ������������� �����������, ������� ����� ��������� ������ ����� �� �����
        using (var reader = new StreamReader(path))
        {
            // ������������� ����������, ������� ����� ������� �������� ������
            deckName = string.Empty;
            // ������������� �������, � ������� ����� ������������ ������ �� �����
            var deck = new List<Card>();
            // ������������� ��������� ����������, ���������� �� �������, �������� �� ������� ������ ������
            bool isFirstLine = true;
            // ���� �� ����� �����
            while (!reader.EndOfStream)
            {
                // ��������, �������� �� ������� ������ ������
                if (isFirstLine)
                {
                    // ������ ������ ������ ����� � ���������� � ��������� ������
                    deckName = reader.ReadLine();
                    // ��������� ������� isFirstLine �� false
                    isFirstLine = false;
                } 
                else
                {
                    // ���������� ������� ������ �� ����� � ������ � � ���������� line
                    var line = reader.ReadLine();
                    // �������� ������ line �� ����, �������� � �������� ������� �� ������� ";"
                    var values = line.Split(';');
                    // �������� ����� �� ������, ���������� �� ������� ������
                    var card = new Card(values[1], string.Empty, DateTime.Parse(values[0], CultureInfo.CreateSpecificCulture("ru-RU")), values[2]);
                    // ���������� ���� ����� � ������������ ������
                    deck.Add(card);
                }
            }
            // ����������� ��������� ������
            return deck;
        }
    }

    // ������� SaveDeckToFile, ���������� �� ���������� ������ � ��������� ���� UserDeck.csv
    public static void SaveDeckToFile(List<Card> deck, string deckName)
    {
        // ���������� ����������, �������� ��� ���������� �����
        string fileName = @"UserDeck.csv";

        // �������� �� ��, ���� �� ��� � ������ ����� ���� � ��������� "fileName"
        if (File.Exists(fileName))
        {
            // ���� ����� ���� ������, ������� ���
            File.Delete(fileName);
        }

        // �������� ������ ����� � ��������� "fileName"
        File.Create(fileName).Dispose();

        // ���������� ������������� �����������, ������� ����� ���������� ������ � ��������� ����
        using (var writer = new StreamWriter(fileName, true))
        {
            // ������ ������ ������� � ����� ��������� ������
            writer.WriteLine(deckName);

            // ���� �� ���� ��������� (������) ������
            for (int i = 0; i < deck.Count; i++)
            {
                // ������ ������� ����� � ����
                writer.WriteLine(deck[i].ToString());
            }
        }
    }
}