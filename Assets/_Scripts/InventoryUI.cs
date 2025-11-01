using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Referências")]
    public PlayerInventory playerInventory;

    [Header("Slots de Inventário (3 slots)")]
    public List<Image> itemSlots; // arraste os 3 Image slots no Inspector

    [Header("Ícones dos Equipamentos")]
    public Sprite picaretaIcon;
    public Sprite tochaIcon;
    public Sprite comidaIcon;

    [Header("Ícones das Oferendas")]
    public Sprite cigarroIcon;
    public Sprite alcoolIcon;
    public Sprite folhaCocaIcon;

    [Header("Ícone Vazio")]
    public Sprite vazioIcon;

    [Header("Configurações Visuais")]
    public Color highlightColor = new Color(1f, 0.9f, 0.5f, 1f);
    public float pulseSpeed = 2f;
    public bool enablePulseEffect = true;

    private Dictionary<string, Sprite> itemSprites;
    private float pulseTimer = 0f;

    void Awake()
    {
        // 🔎 Garante referência ao inventário
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();
    }

    void Start()
    {
        // Cria o mapa item → sprite
        itemSprites = new Dictionary<string, Sprite>
        {
            { "picareta", picaretaIcon },
            { "tocha", tochaIcon },
            { "comida", comidaIcon },
            { "cigarro", cigarroIcon },
            { "álcool", alcoolIcon },
            { "folha de coca", folhaCocaIcon }
        };

        // Atualiza UI inicial
        UpdateUI();
    }

    void OnEnable()
    {
        UpdateUI();
    }

    void Update()
    {
        // 🔒 Evita erros se algo estiver nulo
        if (playerInventory == null || itemSlots == null || itemSlots.Count == 0)
            return;

        // Efeito de pulsação nos slots preenchidos
        if (enablePulseEffect)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = (Mathf.Sin(pulseTimer) + 1f) / 2f;

            List<string> items = playerInventory.GetItems();
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (i < items.Count && !string.IsNullOrEmpty(items[i]))
                {
                    float alpha = Mathf.Lerp(0.8f, 1f, pulse);
                    Color currentColor = itemSlots[i].color;
                    itemSlots[i].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                }
            }
        }
    }

    // 🔄 Atualiza os ícones conforme o inventário atual
    public void UpdateUI()
    {
        // 🆕 Garante que o inventário esteja referenciado
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory == null || itemSlots == null)
            return;

        List<string> items = playerInventory.GetItems();

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count && itemSprites.ContainsKey(items[i]))
            {
                itemSlots[i].sprite = itemSprites[items[i]];
                itemSlots[i].color = Color.white;
                itemSlots[i].enabled = true;
            }
            else
            {
                // Mostra ícone vazio
                if (vazioIcon != null)
                {
                    itemSlots[i].sprite = vazioIcon;
                    itemSlots[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    itemSlots[i].enabled = true;
                }
                else
                {
                    itemSlots[i].enabled = false;
                }
            }
        }
    }

    // 💡 Destaque visual de um slot específico
    public void HighlightSlot(int slotIndex, float duration = 0.5f)
    {
        if (slotIndex >= 0 && slotIndex < itemSlots.Count)
            StartCoroutine(HighlightSlotCoroutine(slotIndex, duration));
    }

    private System.Collections.IEnumerator HighlightSlotCoroutine(int slotIndex, float duration)
    {
        Image slot = itemSlots[slotIndex];
        Color originalColor = slot.color;

        float elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            slot.color = Color.Lerp(originalColor, highlightColor, elapsed / (duration / 2f));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            slot.color = Color.Lerp(highlightColor, originalColor, elapsed / (duration / 2f));
            yield return null;
        }

        slot.color = originalColor;
    }
}
