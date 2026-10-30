using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MenuScreen : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip mainThemeMusic;
    public Button quitButton; // Verwijzing naar de Quit Button

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (mainThemeMusic != null)
        {
            PlayBackgroundMusic();
        }

        // Voeg de functie voor de quit-knop toe
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(ExitGame);  // Wanneer de knop wordt ingedrukt, wordt ExitGame aangeroepen
        }
    }

    public void PlayGame()
    {
        StopBackgroundMusic();
        SceneManager.LoadSceneAsync("Tetris");
    }

        public void ExitGame()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        }


    private void PlayBackgroundMusic()
    {
        if (audioSource != null && mainThemeMusic != null)
        {
            audioSource.clip = mainThemeMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
