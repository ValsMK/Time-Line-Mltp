using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ToggleScript : MonoBehaviour
{
    Toggle m_Toggle;

    private string GeneralHistoryTag = "TG_GENERAL_HISTORY";
    private string RussianHistoryTag = "TG_RUSSIAN_HISTORY";
    private string UserDeckTag = "TG_USER_DECK";
    private string UserDeckNameTag = "TG_USER_DECK_NAME";

    public string SaveDeckKey = "DECK_KEY";
    public string SaveDeckName = "DECK_NAME";

    void Start()
    {
        m_Toggle = GetComponent<Toggle>();
        if (PlayerPrefs.HasKey(SaveDeckKey))
        {
            if (m_Toggle.tag == GeneralHistoryTag)
            {
                m_Toggle.isOn = ((int)DeckTypeEnum.GeneralHistoryXVIIICentury == PlayerPrefs.GetFloat(SaveDeckKey));
            }
            if (m_Toggle.tag == RussianHistoryTag)
            {
                m_Toggle.isOn = ((int)DeckTypeEnum.RussianHistoryXVIIICentury == PlayerPrefs.GetFloat(SaveDeckKey));
            }
            if (m_Toggle.tag == UserDeckTag)
            {
                m_Toggle.isOn = ((int)DeckTypeEnum.UserDeck == PlayerPrefs.GetFloat(SaveDeckKey));
            }
        }
        else
        {
            m_Toggle.isOn = (m_Toggle.tag == GeneralHistoryTag);
            PlayerPrefs.SetFloat(this.SaveDeckKey, (int)DeckTypeEnum.GeneralHistoryXVIIICentury);
        }
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });
    }

    void ToggleValueChanged(Toggle change)
    {
        if (change.tag == GeneralHistoryTag && change.isOn)
        {
            PlayerPrefs.SetFloat(this.SaveDeckKey, (int)DeckTypeEnum.GeneralHistoryXVIIICentury);
        } 
        if (change.tag == RussianHistoryTag && change.isOn)
        {
            PlayerPrefs.SetFloat(this.SaveDeckKey, (int)DeckTypeEnum.RussianHistoryXVIIICentury);
        }
        if (change.tag == UserDeckTag && change.isOn)
        {
            PlayerPrefs.SetFloat(this.SaveDeckKey, (int)DeckTypeEnum.UserDeck);

            if (!File.Exists(@"UserDeck.csv"))
            {
                var deck = OpenFilePanel.GetDeckFromFile(OpenFilePanel.GetFilePath(), out string deckName);
                OpenFilePanel.SaveDeckToFile(deck, deckName);
                PlayerPrefs.SetString(this.SaveDeckName, deckName);
                
                GameObject DeckName = GameObject.FindWithTag(UserDeckNameTag);
                var t = DeckName.GetComponent<TextMeshProUGUI>();
                t.text = deckName;
            }
        }
    }
}