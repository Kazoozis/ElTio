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
    private bool voicePlayedThisEncounter = false; // ✅ controla uma vez por encontro

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

    // 🎧 Passos
    public void StartSteps()
    {
        if (stepsSource.isPlaying) return;
        stepsSource.clip = stepClip;
        stepsSource.loop = true;
        stepsSource.volume = 0.4f;
        stepsSource.Play();
    }

    public void StopSteps() => stepsSource.Stop();

    // 🎧 Voz curta de 4 segundos (uma vez por encontro)
    public void PlayVoiceFor4Seconds()
    {
        if (voicePlayedThisEncounter) return; // ✅ ignora se já tocou neste encontro

        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;
        }

        voiceCoroutine = StartCoroutine(PlayVoiceCoroutine());
    }

    private IEnumerator PlayVoiceCoroutine()
    {
        voicePlayedThisEncounter = true; // marca como tocada

        // toca um único clip aleatório por 4 segundos
        if (voiceClips.Length > 0)
        {
            int idx = Random.Range(0, voiceClips.Length);
            voiceSource.pitch = Random.Range(0.95f, 1.05f);
            voiceSource.PlayOneShot(voiceClips[idx], 0.3f);
        }

        yield return new WaitForSeconds(4f);

        voiceCoroutine = null;
    }

    // Chamado no fim do encontro para resetar a voz
    public void ResetVoiceForNextEncounter()
    {
        voicePlayedThisEncounter = false;
    }

    // 🎧 Efeitos especiais
    public void PlayWhisper() => whisperSource.PlayOneShot(whisperClip, 0.6f);
    public void PlayDevilLaugh() => devilSource.PlayOneShot(devilLaugh, 1f);
    public void PlayDeath() => devilSource.PlayOneShot(deathClip, 1f);
}
