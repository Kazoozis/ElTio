using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Referências")]
    public PlayerInventory playerInventory;

    [Header("Slots de Inventário (3 slots)")]
    public List<Image> itemSlots;

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

    private Dictionary<string, Sprite> itemSprites;

    void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();
    }

    void Start()
    {
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

    public void UpdateUI()
    {
        // ✅ Se o inventário ainda não existir, apenas espera
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory == null)
            return;

        // ✅ Verifica se os slots foram atribuídos
        if (itemSlots == null || itemSlots.Count == 0)
        {
            Debug.LogWarning("⚠️ Nenhum slot de inventário foi atribuído no InventoryUI!");
            return;
        }

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
                itemSlots[i].sprite = vazioIcon;
                itemSlots[i].color = new Color(1, 1, 1, 0.3f);
                itemSlots[i].enabled = true;
            }
        }
    }
}
