using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    [Header("기본 능력치")]
    [SerializeField] private int baseAttack = 10;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private int baseHP = 100;

    [Header("스킨")]
    [SerializeField] private GameObject[] skins;

    [Header("현재 능력치")]
    [SerializeField] private int attack;
    [SerializeField] private float speed;
    [SerializeField] private int maxHP;

    private int currentHP;

    public int Attack => attack;
    public float Speed => speed;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    private void Start()
    {
        ApplyUpgradeStats();
        ApplySkin();
    }

    private void ApplyUpgradeStats()
    {
        PlayerData data =
            SaveManager.Instance.Data;

        attack =
            baseAttack +
            data.attackLevel * 5;

        speed =
            baseSpeed +
            data.speedLevel * 0.5f;

        maxHP =
            baseHP +
            data.hpLevel * 20;

        currentHP = maxHP;

        Debug.Log(
            "공격력 : " + attack +
            " / 속도 : " + speed +
            " / 체력 : " + maxHP
        );
    }

    private void ApplySkin()
    {
        int selectedSkin =
            SaveManager.Instance.Data.selectedSkin;

        if (skins == null || skins.Length == 0)
        {
            return;
        }

        if (selectedSkin < 0 ||
            selectedSkin >= skins.Length)
        {
            selectedSkin = 0;
        }

        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].SetActive(i == selectedSkin);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;

            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
    }
}