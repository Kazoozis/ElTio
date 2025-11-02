using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class IntroCutscene : MonoBehaviour
{
    [Header("Referências")]
    public VideoPlayer videoPlayer;    // Componente VideoPlayer
    public Image fadeImage;            // Imagem preta sobre o vídeo
    public string nextSceneName = "GameScene"; // Nome da próxima cena

    [Header("Configurações")]
    public float fadeDuration = 1.5f;  // Tempo do fade (em segundos)

    private void Start()
    {
        // Garante que o fade começa invisível (tela preta desaparece antes do vídeo)
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        // Começa reproduzindo o vídeo e depois faz fade in (preto -> vídeo)
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        if (videoPlayer == null) yield break;

        // Aguarda o vídeo preparar (caso ainda esteja carregando)
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        // Reproduz
        videoPlayer.Play();

        // Fade in (tira o preto no começo)
        yield return StartCoroutine(Fade(1f, 0f));

        // Aguarda o vídeo acabar
        while (videoPlayer.isPlaying)
            yield return null;

        // Quando o vídeo termina, faz fade out (preto)
        yield return StartCoroutine(Fade(0f, 1f));

        // Carrega a próxima cena
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadeImage.color = c;
    }
}
