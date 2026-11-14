using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Music")]
    [SerializeField] private AudioClip bgMusic;

    [Header("UI")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip acquireSound;

    [Header("Dice")]
    [SerializeField] private AudioClip rollSound;
    [SerializeField] private AudioClip holdSound;

    [Header("Combat")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip reviveSound;

    [Header("Results")]
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip gameOverSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            musicSource.Stop();
            musicSource.clip = null;
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayEffect(AudioClip clip)
    {
        if (clip != null)
            effectsSource.PlayOneShot(clip);
    }

    public void PlayUISound(AudioClip clip)
    {
        if (clip != null)
            uiSource.PlayOneShot(clip);
    }

    public void PlayBGMusic() => PlayMusic(bgMusic);

    public void PlayClick() => PlayUISound(clickSound);
    public void PlayAcquire() => PlayUISound(acquireSound);

    public void PlayRoll() => PlayEffect(rollSound);
    public void PlayHold() => PlayEffect(holdSound);
    public void PlayHit() => PlayEffect(hitSound);
    public void PlayBlock() => PlayEffect(blockSound);
    public void PlayRevive() => PlayEffect(reviveSound);

    public void PlayVictory()
    {
        musicSource.Stop();
        PlayUISound(victorySound);
    }

    public void PlayGameOver()
    {
        musicSource.Stop();
        PlayUISound(gameOverSound);
    }
}