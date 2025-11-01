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
    public Color highlightColor = new Color(1f, 0.9f, 0.5f, 1f); // Amarelo suave para destaque
    public float pulseSpeed = 2f; // Velocidade da pulsação
    public bool enablePulseEffect = true; // Ativar/desativar efeito de pulsação

    private Dictionary<string, Sprite> itemSprites;
    private float pulseTimer = 0f;

    void Start()
    {
        // Mapa item → sprite
        itemSprites = new Dictionary<string, Sprite>
        {
            { "picareta", picaretaIcon },
            { "tocha", tochaIcon },
            { "comida", comidaIcon },
            { "cigarro", cigarroIcon },
            { "álcool", alcoolIcon },
            { "folha de coca", folhaCocaIcon }
        };

        UpdateUI();
    }

    void OnEnable()
    {
        UpdateUI();
    }

    void Update()
    {
        // Efeito de pulsação sutil nos slots preenchidos
        if (enablePulseEffect)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = (Mathf.Sin(pulseTimer) + 1f) / 2f; // Valor entre 0 e 1
            
            List<string> items = playerInventory.GetItems();
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (i < items.Count && !string.IsNullOrEmpty(items[i]))
                {
                    // Aplica um leve efeito de brilho nos itens
                    float alpha = Mathf.Lerp(0.8f, 1f, pulse);
                    Color currentColor = itemSlots[i].color;
                    itemSlots[i].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                }
            }
        }
    }

    public void UpdateUI()
    {
        List<string> items = playerInventory.GetItems();

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count && itemSprites.ContainsKey(items[i]))
            {
                // Atualiza o sprite do item
                itemSlots[i].sprite = itemSprites[items[i]];
                itemSlots[i].color = Color.white; // Cor normal para itens válidos
                itemSlots[i].enabled = true;
            }
            else
            {
                // Slot vazio
                if (vazioIcon != null)
                {
                    itemSlots[i].sprite = vazioIcon;
                    itemSlots[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Cinza translúcido
                }
                else
                {
                    itemSlots[i].enabled = false; // Desabilita se não houver sprite vazio
                }
            }
        }
    }

    // Método para destacar um slot específico (útil para feedback visual)
    public void HighlightSlot(int slotIndex, float duration = 0.5f)
    {
        if (slotIndex >= 0 && slotIndex < itemSlots.Count)
        {
            StartCoroutine(HighlightSlotCoroutine(slotIndex, duration));
        }
    }

    private System.Collections.IEnumerator HighlightSlotCoroutine(int slotIndex, float duration)
    {
        Image slot = itemSlots[slotIndex];
        Color originalColor = slot.color;
        
        // Fase de destaque
        float elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            slot.color = Color.Lerp(originalColor, highlightColor, elapsed / (duration / 2f));
            yield return null;
        }
        
        // Fase de retorno
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