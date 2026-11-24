using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Checkbox Sprites")]
    public Sprite checkedSprite;
    public Sprite uncheckedSprite;
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    [Header("Music Checkbox")]
    public Image musicCheckboxImage;
    [Header("Sound Checkbox")]
    public Image soundCheckboxImage;

    private bool musicEnabled = true;
    private bool soundEnabled = true;

    void Start()
    {
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        ApplyMusic();
        ApplySound();
        RefreshCheckboxes();
    }

    public void RetryLevel() // 👈 new
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMusic();
        RefreshCheckboxes();
    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplySound();
        RefreshCheckboxes();
    }

    void ApplyMusic()
    {
        audioMixer.SetFloat("MusicVolume", musicEnabled ? 0f : -80f);
    }

    void ApplySound()
    {
        audioMixer.SetFloat("SFXVolume", soundEnabled ? 0f : -80f);
    }

    void RefreshCheckboxes()
    {
        musicCheckboxImage.sprite = musicEnabled ? checkedSprite : uncheckedSprite;
        soundCheckboxImage.sprite = soundEnabled ? checkedSprite : uncheckedSprite;
    }
}