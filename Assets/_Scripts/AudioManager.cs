using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource stepsSource;
    public AudioSource voiceSource;
    public AudioSource whisperSource;
    public AudioSource devilSource;

    [Header("Clips")]
    public AudioClip musicLoop;
    public AudioClip ambientLoop;
    public AudioClip[] voiceClips;
    public AudioClip stepClip;
    public AudioClip whisperClip;
    public AudioClip devilLaugh;
    public AudioClip deathClip;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // inicia música e som ambiente em loop
        if (musicLoop != null)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;
            musicSource.volume = 0.35f;
            musicSource.Play();
        }

        if (ambientLoop != null)
        {
            ambientSource.clip = ambientLoop;
            ambientSource.loop = true;
            ambientSource.volume = 0.5f;
            ambientSource.Play();
        }
    }

    // 🔹 Som de passos
    public void PlayStep()
    {
        if (stepClip != null)
            stepsSource.PlayOneShot(stepClip, 0.4f);
    }

    // 🔹 Voz estilo Animal Crossing
    public void PlayVoice()
    {
        if (voiceClips.Length == 0) return;
        int index = Random.Range(0, voiceClips.Length);
        voiceSource.PlayOneShot(voiceClips[index], 0.3f);
    }

    // 🔹 Sussurros aleatórios
    public void PlayWhisper()
    {
        if (whisperClip != null)
            whisperSource.PlayOneShot(whisperClip, 0.6f);
    }

    // 🔹 Risada do El Tío
    public void PlayDevilLaugh()
    {
        if (devilLaugh != null)
            devilSource.PlayOneShot(devilLaugh, 1f);
    }

    // 🔹 Som de morte
    public void PlayDeath()
    {
        if (deathClip != null)
            devilSource.PlayOneShot(deathClip, 1f);
    }

    // 🔹 Fade opcional de volume (pra efeitos dramáticos)
    public void FadeMusic(float targetVolume, float duration)
    {
        StartCoroutine(FadeVolume(musicSource, targetVolume, duration));
    }

    private System.Collections.IEnumerator FadeVolume(AudioSource source, float target, float duration)
    {
        float start = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
    }
}
