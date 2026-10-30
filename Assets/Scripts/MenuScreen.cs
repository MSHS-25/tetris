using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public AudioClip mainThemeMusic;
    public Button quitButton;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayBackgroundMusic();

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(ExitGame);
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
