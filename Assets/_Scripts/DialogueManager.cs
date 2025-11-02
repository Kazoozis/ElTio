using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI de Diálogo")]
    [SerializeField] private GameObject dialoguePanel;   // Caixa fixa
    [SerializeField] private TextMeshProUGUI dialogueText; // Texto principal
    [SerializeField] private TextMeshProUGUI hintText;     // Texto auxiliar (ex: instruções)

    private Queue<string> lines = new();
    private GameManager gameManager;

    private bool introShown = false;

    void Awake()
    {
        Instance = this;
        gameManager = FindObjectOfType<GameManager>();

        // Painel sempre ativo (HUD fixa)
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Limpa textos no início
        if (dialogueText != null)
            dialogueText.text = "";

        if (hintText != null)
        {
            hintText.text = "🡆 Aperte [ESPAÇO] para avançar";
            hintText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // Mostra instrução inicial apenas até o jogador apertar espaço pela primeira vez
        if (!introShown && Input.GetKeyDown(KeyCode.Space))
        {
            introShown = true;
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }

    // Exibe falas do diálogo
    public void ShowDialogue(string[] textLines)
    {
        lines.Clear();

        // Filtra mensagens internas
        foreach (string line in textLines)
        {
            if (!line.StartsWith("Você inicia com:") &&
                !line.Contains("porcentagem de hostilidade") &&
                !line.Contains("Ele quer trocar"))
            {
                lines.Enqueue(line);
            }
        }

        if (lines.Count == 0) return;

        gameManager.SetDialogueState(true);
        NextLine();
    }

    // Avança as falas
    public void NextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = lines.Dequeue();
        if (dialogueText != null)
            dialogueText.text = line;

        // Exibe instrução visual quando for hora de escolher
        if (line.Contains("ACEITAR") && line.Contains("RECUSAR"))
        {
            if (hintText != null)
            {
                hintText.text = "🡆 Aperte [1] para ACEITAR ou [2] para RECUSAR";
                hintText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }

        // Som opcional
        if (AudioManager.Instance != null &&
            line.Contains("Tenho") &&
            line.Contains("para trocar"))
        {
            AudioManager.Instance.PlayVoiceFor4Seconds();
        }
    }

    private void EndDialogue()
    {
        if (dialogueText != null)
            dialogueText.text = "";

        // Após o diálogo, reexibe a instrução de espaço
        if (hintText != null)
        {
            hintText.text = "🡆 Aperte [ESPAÇO] para avançar";
            hintText.gameObject.SetActive(true);
        }

        gameManager.SetDialogueState(false);
    }
}
