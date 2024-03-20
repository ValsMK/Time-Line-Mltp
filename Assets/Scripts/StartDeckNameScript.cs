using TMPro;    // ����������� ���������� TMPro
using UnityEngine;  // ����������� ���������� UnityEngine
using System.IO;

/// <summary>
/// ������ �������� �� ����������� ����������� �������� ������ � ������������ ���� � ������ ����
/// </summary>
public class StartDeckNameScript : MonoBehaviour
{
    [SerializeField] GameObject DeckName;   // ������������� ���� ��� ��������� ������ �� ������� ������, ������������ �������� ������
    public string SaveDeckName = "DECK_NAME";   // ������������� ��������� ���������, ������ ��� ��������� � ������ �������� ������ � ������

    void Start()    // ������ ������� Start, ������������� 1 ��� ��� ������� ����
    {
        var t = DeckName.GetComponent<TextMeshProUGUI>();   // ��������� ������ �� ��������� ���� ��� ����������� �������� ������

        if (PlayerPrefs.HasKey(SaveDeckName) && File.Exists(@"UserDeck.csv"))   // ��������, ���� �� ����������� �������� ������
        {
            t.text = PlayerPrefs.GetString(SaveDeckName);   // ��������� �������� ������
        }
    }
}
