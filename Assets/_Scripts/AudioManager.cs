using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource stepsSource;
    public AudioSource voiceSource;
    public AudioSource whisperSource;
    public AudioSource devilSource;

    public AudioClip musicLoop;
    public AudioClip ambientLoop;
    public AudioClip[] voiceClips;
    public AudioClip stepClip;
    public AudioClip whisperClip;
    public AudioClip devilLaugh;
    public AudioClip deathClip;

    private Coroutine voiceCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayLoop(musicSource, musicLoop, 0.35f);
        PlayLoop(ambientSource, ambientLoop, 0.5f);
    }

    private void PlayLoop(AudioSource source, AudioClip clip, float volume)
    {
        if (clip == null) return;
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.Play();
    }

    // Passos
    public void StartSteps()
    {
        if (stepsSource.isPlaying) return;
        stepsSource.clip = stepClip;
        stepsSource.loop = true;
        stepsSource.volume = 0.4f;
        stepsSource.Play();
    }

    public void StopSteps()
    {
        stepsSource.Stop();
    }

    // Fala do minerador — **sempre apenas 1 coroutine**
    public void StartVoice()
    {
        StopVoice(); // garante que nenhum som antigo esteja ativo
        voiceCoroutine = StartCoroutine(VoiceLoop());
    }

    public void StopVoice()
    {
        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;
        }
        voiceSource.Stop();
    }

    private IEnumerator VoiceLoop()
    {
        while (true)
        {
            int idx = Random.Range(0, voiceClips.Length);
            voiceSource.pitch = Random.Range(0.95f, 1.05f);
            voiceSource.PlayOneShot(voiceClips[idx], 0.3f);
            yield return new WaitForSeconds(0.15f); // intervalo seguro
        }
    }

    public void PlayWhisper()
    {
        whisperSource.PlayOneShot(whisperClip, 0.6f);
    }

    public void PlayDevilLaugh()
    {
        devilSource.PlayOneShot(devilLaugh, 1f);
    }

    public void PlayDeath()
    {
        devilSource.PlayOneShot(deathClip, 1f);
    }
}
