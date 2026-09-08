using TMPro;
using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text survivalTimeText;

    [SerializeField]
    private TMP_Text levelText;

    public void Show(GameResultData result)
    {
        if (result == null)
        {
            return;
        }

        if (survivalTimeText != null)
        {
            int minute =
                Mathf.FloorToInt(
                    result.SurvivalTime / 60f
                );

            int second =
                Mathf.FloorToInt(
                    result.SurvivalTime % 60f
                );

            survivalTimeText.text =
                $"Survival time : {minute:00}:{second:00}";
        }

        if (levelText != null)
        {
            levelText.text =
                $"Level : {result.Level}";
        }
    }
}