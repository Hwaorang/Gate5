using UnityEngine;

public class StageReward : MonoBehaviour
{
   
    [SerializeField] private int baseStageGold = 100;

    private bool rewardGiven = false;


    public void StageClear()
    {
       
        if (rewardGiven)
        {
            return;
        }

        rewardGiven = true;
        PlayerData data = SaveManager.Instance.Data;

        int bonusGold = data.goldRewardLevel * 10;


        int totalGold = baseStageGold + bonusGold;
 
        SaveManager.Instance.AddGold(totalGold);

    }
}