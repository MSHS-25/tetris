using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    // Method to load the game scene with the selected level
    public void StartGameWithLevel(int level)
    {
        Debug.Log("Niveau geselecteerd: " + level); // Voeg deze regel toe om te zien welk niveau wordt geselecteerd
        PlayerPrefs.SetInt("SelectedLevel", level);
        PlayerPrefs.Save();  // Zorg ervoor dat de veranderingen worden opgeslagen
        Debug.Log("Geselecteerd niveau opgeslagen: " + PlayerPrefs.GetInt("SelectedLevel")); // Controleer of het niveau is opgeslagen
        SceneManager.LoadScene("Tetris");
    }



    // Method to go back to the main menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
