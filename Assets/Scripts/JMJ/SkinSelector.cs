using TMPro;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    [Header("스킨 오브젝트")]
    [SerializeField] private GameObject[] skins;

    [Header("UI")]
    [SerializeField] private TMP_Text skinNameText;

    [SerializeField] private string[] skinNames;

    private int currentSkin;

    private void Start()
    {
        currentSkin = SaveManager.Instance.Data.selectedSkin;

        ApplySkin();
    }

    public void NextSkin()
    {
        currentSkin++;

        if (currentSkin >= skins.Length)
        {
            currentSkin = 0;
        }

        ApplySkin();

        SaveSkin();
    }

    public void PreviousSkin()
    {
        currentSkin--;

        if (currentSkin < 0)
        {
            currentSkin = skins.Length - 1;
        }

        ApplySkin();

        SaveSkin();
    }

    private void ApplySkin()
    {
        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].SetActive(i == currentSkin);
        }

        if (skinNameText != null &&
            skinNames != null &&
            currentSkin < skinNames.Length)
        {
            skinNameText.text = skinNames[currentSkin];
        }
    }

    private void SaveSkin()
    {
        SaveManager.Instance.Data.selectedSkin = currentSkin;

        SaveManager.Instance.Save();
    }

    public int GetCurrentSkin()
    {
        return currentSkin;
    }
}