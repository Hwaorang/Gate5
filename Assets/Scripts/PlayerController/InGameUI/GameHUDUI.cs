using TMPro;
using UnityEngine;

/// <summary>
/// 인게임 HUD 화면 표시만 담당한다.
/// 게임 로직은 직접 참조하지 않는다.
/// </summary>
public class GameHUDUI : MonoBehaviour
{
    [Header("Game Info")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timerText;


    [Header("Skill HUD")]
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text projectileText;
    [SerializeField] private TMP_Text moveSpeedText;


    public void SetLevel(int level)
    {
        if (levelText == null)
        {
            return;
        }

        levelText.text = $"Lv. {level}";
    }


    public void SetTime(float time)
    {
        if (timerText == null)
        {
            return;
        }

        int minutes =
            Mathf.FloorToInt(time / 60f);

        int seconds =
            Mathf.FloorToInt(time % 60f);

        timerText.text =
            $"{minutes:00}:{seconds:00}";
    }


    public void SetDamageLevel(int level)
    {
        SetSkillLevel(
            damageText,
            "damage",
            level
        );
    }


    public void SetAttackSpeedLevel(int level)
    {
        SetSkillLevel(
            attackSpeedText,
            "attackSpeed",
            level
        );
    }


    public void SetProjectileLevel(int level)
    {
        SetSkillLevel(
            projectileText,
            "projectile",
            level
        );
    }


    public void SetMoveSpeedLevel(int level)
    {
        SetSkillLevel(
            moveSpeedText,
            "moveSpeed",
            level
        );
    }


    private void SetSkillLevel(
        TMP_Text text,
        string skillName,
        int level)
    {
        if (text == null)
        {
            return;
        }

        // 아직 획득하지 않은 강화
        if (level <= 0)
        {
            text.gameObject.SetActive(false);
            return;
        }

        text.gameObject.SetActive(true);

        text.text =
            $"{skillName} Lv.{level}";
    }
}