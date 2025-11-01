using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GameManager : MonoBehaviour
{
    [Header("=== CONFIGURAÇÕES GERAIS ===")]
    [Tooltip("Número de vidas iniciais do jogador.")]
    public int startingLives = 3;

    [Tooltip("Quantidade de encontros antes do final do jogo.")]
    public int encountersCount = 3;

    [Header("=== CHANCES DE HOSTILIDADE (recusa de troca) ===")]
    public int baseHostilityChance = 15;
    public int torchEffectOnHostility = -5;
    public int pickaxeEffectOnHostility = -5;
    public int foodEffectOnHostility = +5;
    public int eatDuringEncounterPenalty = 50;

    [Header("=== MODIFICADORES POR MINERADOR (recusa) ===")]
    public int famintoModifierIfHasFood = 5;
    public int assombradoModifierIfHasTorch = 5;
    public int briguentoModifierIfNoPickaxe = 5;

    [Header("=== ROUBO ===")]
    public int baseSecondLifeLossChance = 15;
    public int famintoStealModifier = -10;
    public int assombradoStealModifier = 10;
    public int briguentoStealModifier = 20;
    public int torchStealModifier = 10;
    public int pickaxeStealModifier = -10;

    [Header("=== REFERÊNCIAS ===")]
    public List<Encounter> possibleEncounters;
    public UIManager uiManager;

    [Header("=== ESTADO DE JOGO ===")]
    public Inventory inventory;
    public int vidas;
    public List<string> oferendas = new List<string>();

    private List<Encounter> encountersToPlay;
    private bool comendoNoEncontro = false;

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        vidas = startingLives;
        inventory = new Inventory() { temTocha = true, temPicareta = true, foodCharges = 2 };
        oferendas.Clear();
        comendoNoEncontro = false;

        GenerateEncounters();

        uiManager.UpdateHUD(inventory, vidas, oferendas);
        Debug.Log("🕯️ [GAME] Início do jogo. Vidas: " + vidas + " | Encontros: " + encountersToPlay.Count);
        StartCoroutine(RunEncounters());
    }

    void GenerateEncounters()
    {
        encountersToPlay = new List<Encounter>();
        List<Encounter> pool = new List<Encounter>(possibleEncounters);
        for (int i = 0; i < encountersCount; i++)
        {
            if (pool.Count == 0) break;
            int r = Random.Range(0, pool.Count);
            encountersToPlay.Add(pool[r]);
            pool.RemoveAt(r);
        }
    }

    System.Collections.IEnumerator RunEncounters()
    {
        foreach (Encounter e in encountersToPlay)
        {
            uiManager.ShowAmbientEvent(ChooseAmbientEvent());
            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(HandleEncounter(e));

            if (vidas <= 0)
            {
                uiManager.ShowMessage("Você morreu. Fim da execução.");
                yield break;
            }
            yield return new WaitForSeconds(0.4f);
        }

        VerifyEnd();
    }

    System.Collections.IEnumerator HandleEncounter(Encounter e)
    {
        string ofertaVisivel = inventory.temTocha && e.HasOferenda() ? e.itemOferecido : "algo nas sombras";
        uiManager.PresentEncounter(e.tipo.ToString(), e.itemPedido, ofertaVisivel, e.HasOferenda());

        bool finished = false;
        uiManager.EnableEncounterButtons(
            onAccept: () => { AcceptTrade(e); finished = true; },
            onRefuse: () => { RefuseTrade(e); finished = true; },
            onSteal: () => { Steal(e); finished = true; },
            onEatHere: () => { EatDuringEncounter(); uiManager.UpdateHUD(inventory, vidas, oferendas); }
        );

        while (!finished) yield return null;

        comendoNoEncontro = false;
        uiManager.DisableEncounterButtons();
        yield return null;
    }

    // ==========================================================
    // ===================== AÇÕES ==============================
    // ==========================================================

    public void AcceptTrade(Encounter e)
    {
        if (e.itemPedido == "comida")
        {
            if (inventory.foodCharges >= 1)
            {
                inventory.Remove("comida");
                e.RemoverOferenda();
                AddOferenda(e.itemOferecido);
                LogAction($"TROCA ACEITA — entregou comida, recebeu {e.itemOferecido}");
            }
            else
            {
                uiManager.ShowMessage("Você não tem comida suficiente pra essa troca.");
                return;
            }
        }
        else
        {
            if (inventory.Has(e.itemPedido))
            {
                inventory.Remove(e.itemPedido);
                e.RemoverOferenda();
                AddOferenda(e.itemOferecido);
                LogAction($"TROCA ACEITA — entregou {e.itemPedido}, recebeu {e.itemOferecido}");
            }
            else
            {
                uiManager.ShowMessage("Você não tem o item pedido.");
                return;
            }
        }

        uiManager.UpdateHUD(inventory, vidas, oferendas);
    }

    public void RefuseTrade(Encounter e)
    {
        int chance = CalculateRefusalHostility(e);
        uiManager.ShowMessage($"Você recusou. Chance de ataque: {chance}%");
        Debug.Log($"[HOSTILIDADE] {e.tipo} → {chance}% (tocha={inventory.temTocha}, picareta={inventory.temPicareta}, comida={inventory.foodCharges})");

        bool atacou = HostilityCalculator.RollPercent(chance);
        if (atacou)
        {
            ProcessHostileAttack(e);
        }
        else
        {
            uiManager.ShowMessage("Você recusa e segue em silêncio.");
        }
        uiManager.UpdateHUD(inventory, vidas, oferendas);
    }

    public void Steal(Encounter e)
    {
        vidas -= 1;
        uiManager.ShowMessage("Você tenta roubar! Perde 1 vida instantaneamente.");
        LogAction($"ROUBO — vida -1 (vidas restantes: {vidas})");

        int chanceSegunda = CalculateSecondLifeLossChance(e);
        Debug.Log($"[ROUBO] Chance de perder segunda vida ({e.tipo}): {chanceSegunda}%");
        if (HostilityCalculator.RollPercent(chanceSegunda))
        {
            vidas -= 1;
            uiManager.ShowMessage("O roubo deu errado: você perdeu uma segunda vida!");
        }
        else
        {
            uiManager.ShowMessage("Você consegue escapar ferido, mas com a oferenda.");
        }

        if (e.HasOferenda())
        {
            AddOferenda(e.itemOferecido);
            e.RemoverOferenda();
        }

        uiManager.UpdateHUD(inventory, vidas, oferendas);

        if (vidas <= 0)
        {
            uiManager.ShowMessage("Você sucumbe aos ferimentos.");
        }
    }

    public void EatDuringEncounter()
    {
        if (inventory.foodCharges >= 1)
        {
            inventory.foodCharges -= 1;
            comendoNoEncontro = true;
            uiManager.ShowMessage("Você comeu uma carga durante o encontro (arriscado!).");
            LogAction("Comeu durante o encontro (+50% hostilidade em recusa futura)");
        }
        else
        {
            uiManager.ShowMessage("Sem cargas para comer.");
        }
        uiManager.UpdateHUD(inventory, vidas, oferendas);
    }

    // ==========================================================
    // ===================== LÓGICA AUXILIAR ====================
    // ==========================================================

    int CalculateRefusalHostility(Encounter e)
    {
        int chance = baseHostilityChance;

        if (e.tipo == MineradorTipo.Faminto && inventory.foodCharges > 0) chance += famintoModifierIfHasFood;
        if (e.tipo == MineradorTipo.Assombrado && inventory.temTocha) chance += assombradoModifierIfHasTorch;
        if (e.tipo == MineradorTipo.Briguento && !inventory.temPicareta) chance += briguentoModifierIfNoPickaxe;

        if (inventory.temTocha) chance += torchEffectOnHostility;
        if (inventory.temPicareta) chance += pickaxeEffectOnHostility;
        if (inventory.foodCharges > 0) chance += foodEffectOnHostility;

        if (comendoNoEncontro) chance += eatDuringEncounterPenalty;

        return Mathf.Clamp(chance, 0, 100);
    }

    int CalculateSecondLifeLossChance(Encounter e)
    {
        int chance = baseSecondLifeLossChance;

        if (e.tipo == MineradorTipo.Faminto) chance += famintoStealModifier;
        if (e.tipo == MineradorTipo.Assombrado) chance += assombradoStealModifier;
        if (e.tipo == MineradorTipo.Briguento) chance += briguentoStealModifier;

        if (inventory.temTocha) chance += torchStealModifier;
        if (inventory.temPicareta) chance += pickaxeStealModifier;

        return Mathf.Clamp(chance, 0, 100);
    }

    void AddOferenda(string oferta)
    {
        oferendas.Add(oferta);
    }

    void ProcessHostileAttack(Encounter e)
    {
        uiManager.ShowMessage("O minerador ataca e tenta roubar o item pedido!");
        string item = e.itemPedido;

        if (item == "tocha" && inventory.temTocha)
        {
            inventory.temTocha = false;
            LogAction("⚔️ Ataque: perdeu a tocha.");
        }
        else if (item == "picareta" && inventory.temPicareta)
        {
            inventory.temPicareta = false;
            LogAction("⚔️ Ataque: perdeu a picareta.");
        }
        else if (item == "comida" && inventory.foodCharges > 0)
        {
            inventory.foodCharges = Mathf.Max(0, inventory.foodCharges - 1);
            LogAction("⚔️ Ataque: perdeu 1 carga de comida.");
        }
        else
        {
            vidas -= 1;
            LogAction("⚔️ Ataque: dano direto (-1 vida).");
        }

        uiManager.UpdateHUD(inventory, vidas, oferendas);
    }

    void VerifyEnd()
    {
        uiManager.ShowMessage("Você chega ao altar do Diabo...");
        bool temTodas = oferendas.Contains("álcool") && oferendas.Contains("cigarro") && oferendas.Contains("folha de coca");

        if (vidas <= 0)
        {
            uiManager.ShowMessage("Você caiu antes de completar o pacto. DERROTA.");
            Debug.Log("☠️ DERROTA — morreu antes do final.");
        }
        else if (temTodas)
        {
            uiManager.ShowMessage("O Diabo sorri e o abençoa. VITÓRIA!");
            Debug.Log("🔥 VITÓRIA — todas as oferendas reunidas!");
        }
        else
        {
            uiManager.ShowMessage("Faltam oferendas... O Diabo te amaldiçoa. DERROTA.");
            Debug.Log("😈 DERROTA — oferendas incompletas.");
        }
    }

    string ChooseAmbientEvent()
    {
        if (Random.Range(0, 3) != 0) return null;

        if (inventory.temTocha) return "A chama da tocha vacila, projetando sombras dançantes.";
        if (!inventory.temTocha) return "A escuridão parece respirar ao seu redor.";
        if (inventory.temPicareta) return "O som distante de picaretas ecoa.";
        if (!inventory.temPicareta) return "Um desabamento bloqueia o caminho atrás de você.";
        if (oferendas.Count > 0) return "Você sente o peso das oferendas em suas mãos.";
        return "O silêncio da mina pesa sobre você.";
    }

    void LogAction(string msg)
    {
        Debug.Log($"[GAME LOG] {msg}");
    }

    public void TryEatOutsideEncounter()
    {
        if (inventory.foodCharges >= 1 && vidas < startingLives)
        {
            inventory.foodCharges -= 1;
            vidas += 1;
            uiManager.ShowMessage("Você comeu fora do encontro e recuperou 1 vida.");
            LogAction("Comeu fora do encontro (+1 vida).");
            uiManager.UpdateHUD(inventory, vidas, oferendas);
        }
        else
        {
            uiManager.ShowMessage("Não pode comer agora.");
        }
    }
}
