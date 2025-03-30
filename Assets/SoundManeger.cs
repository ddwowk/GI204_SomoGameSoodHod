using UnityEngine;

public class SoundManeger : MonoBehaviour
{
    public AudioSource m_AudioSource;
    public static SoundManeger Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void PlaySound(AudioClip audioClip, bool loop)
    {
        m_AudioSource.clip = audioClip;
        m_AudioSource.loop = loop;
        m_AudioSource.Play();
    } public void PlaySound(AudioClip audioClip, bool loop,AudioSource audioPlayer)
    {
        audioPlayer.clip = audioClip;
        audioPlayer.loop = loop;
        audioPlayer.Play();
    }


}
