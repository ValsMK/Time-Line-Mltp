using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        GameObject optionsMenu = GameObject.Find("OptionsMenu");
        if (optionsMenu == null)
            return;
        optionsMenu.transform.localPosition = new Vector3(0,0);
    }

    public void CloseSettings()
    {
        GameObject optionsMenu = GameObject.Find("OptionsMenu");
        if (optionsMenu == null)
            return;
        optionsMenu.transform.localPosition = new Vector3(2000, 0);
    }

    public void ChooseFile()
    {
        string SaveDeckName = "DECK_NAME";
        string UserDeckNameTag = "TG_USER_DECK_NAME";

        var deck = OpenFilePanel.GetDeckFromFile(OpenFilePanel.GetFilePath(), out string deckName);
        OpenFilePanel.SaveDeckToFile(deck, deckName);
        PlayerPrefs.SetString(SaveDeckName, deckName);

        GameObject DeckName = GameObject.FindWithTag(UserDeckNameTag);
        var t = DeckName.GetComponent<TextMeshProUGUI>();
        t.text = deckName;
    }
}
