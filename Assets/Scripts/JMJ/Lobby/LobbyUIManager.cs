using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
   
    [SerializeField] private GameObject upgradePanel;


    [SerializeField] private GameObject skinPanel;

 
    [SerializeField] private GameObject difficultyPanel;

    public void OpenUpgradePanel()
    {

        skinPanel.SetActive(false);
        difficultyPanel.SetActive(false);

        upgradePanel.SetActive(true);
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }


   
    public void OpenSkinPanel()
    {
        
        upgradePanel.SetActive(false);
        difficultyPanel.SetActive(false);

        skinPanel.SetActive(true);
    }


    
    public void CloseSkinPanel()
    {
        skinPanel.SetActive(false);
    }

    
    public void OpenDifficultyPanel()
    {
        
        upgradePanel.SetActive(false);
        skinPanel.SetActive(false);

        difficultyPanel.SetActive(true);
    }


    public void CloseDifficultyPanel()
    {
        difficultyPanel.SetActive(false);
    }
}

