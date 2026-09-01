using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    [Header("기본 능력치")]
    [SerializeField] private int baseAttack = 10;
    [SerializeField] private float baseSpeed = 5f;

    [Header("기본 공격속도")]
    [SerializeField] private float baseAttackSpeed = 1f;

    [Header("현재 능력치")]
    [SerializeField] private int attack;
    [SerializeField] private float speed;
    [SerializeField] private float attackSpeed;

    [Header("스킨")]
    [SerializeField] private GameObject[] skins;


    public int Attack => attack;
    public float Speed => speed;
    public float AttackSpeed => attackSpeed;


    private void Start()
    {
        ApplyUpgradeStats();
        ApplySkin();
    }


    private void ApplyUpgradeStats()
    {
        PlayerData data =
            SaveManager.Instance.Data;


        // 공격력
        attack =
            baseAttack +
            data.attackLevel * 5;


        // 이동속도
        speed =
            baseSpeed +
            data.speedLevel * 0.5f;


        // 공격속도
        attackSpeed =
            baseAttackSpeed +
            data.attackSpeedLevel * 0.1f;


        Debug.Log(
            "공격력 : " + attack +
            " / 이동속도 : " + speed +
            " / 공격속도 : " + attackSpeed
        );
    }


    private void ApplySkin()
    {
        int selectedSkin =
            SaveManager.Instance.Data.selectedSkin;


        if (skins == null ||
            skins.Length == 0)
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
            skins[i].SetActive(
                i == selectedSkin
            );
        }
    }
}