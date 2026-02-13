using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance { get; private set; }

    [Header("UI Refs")]
    public GameObject panel;
    public Text titleText;
    public Text levelText;
    public Text atkText;
    public Button upgradeButton;
    public Button closeButton;

    Turret selectedTurret;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (upgradeButton != null) upgradeButton.onClick.AddListener(OnUpgradeClicked);
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
    }

    public void Show(Turret turret)
    {
        selectedTurret = turret;
        if (panel != null) panel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        selectedTurret = null;
        if (panel != null) panel.SetActive(false);
    }

    void Refresh()
    {
        if (selectedTurret == null) return;
        if (titleText != null) titleText.text = selectedTurret.gameObject.name;
        if (levelText != null) levelText.text = "Level: " + selectedTurret.level.ToString();
        if (atkText != null) atkText.text = "ATK: " + selectedTurret.atk.ToString();
    }

    void OnUpgradeClicked()
    {
        if (selectedTurret == null) return;

        selectedTurret.level += 1;
        selectedTurret.atk += 5;
        selectedTurret.atkMultiplier *= 1.1f;

        Refresh();
    }
}
