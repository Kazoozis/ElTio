using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Queue<string> lines = new();
    private GameManager gameManager;

    void Awake()
    {
        Instance = this;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void ShowDialogue(string[] textLines)
    {
        lines.Clear();
        foreach (string line in textLines)
            lines.Enqueue(line);

        gameManager.SetDialogueState(true);
        NextLine();
    }

    public void NextLine()
    {
        if (lines.Count == 0)
        {
            gameManager.SetDialogueState(false);
            return;
        }

        string line = lines.Dequeue();
        Debug.Log(line);

        // 🎧 Toca voz SOMENTE quando for uma fala de troca do inimigo
        if (AudioManager.Instance != null &&
            line.Contains("Tenho") &&
            line.Contains("para trocar"))
        {
            AudioManager.Instance.PlayVoiceFor4Seconds();
        }
    }
}
