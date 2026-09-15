using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField]
    private PlayerSfxType soundType =
    PlayerSfxType.ButtonClick;


    private Button button;


    private void Awake()
    {
        button =
            GetComponent<Button>();
    }


    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(
                PlaySound
            );
        }
    }


    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                PlaySound
            );
        }
    }


    private void PlaySound()
    {
        if (AudioManager_PlayerController.Instance == null)
        {
            Debug.LogWarning(
                "[UIButtonSound] " +
                "AudioManager_PlayerController가 없습니다."
            );

            return;
        }


        AudioManager_PlayerController
            .Instance
            .PlaySfx(
                soundType
            );
    }
}