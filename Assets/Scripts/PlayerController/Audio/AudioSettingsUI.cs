using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Master")]
    [SerializeField]
    private Slider masterSlider;

    [SerializeField]
    private TMP_Text masterValueText;


    [Header("BGM")]
    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private TMP_Text bgmValueText;


    [Header("SFX")]
    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private TMP_Text sfxValueText;


    private void OnEnable()
    {
        RemoveListeners();

        InitializeUI();

        AddListeners();
    }


    private void OnDisable()
    {
        RemoveListeners();
    }


    // ========================================================
    // Initialize
    // ========================================================

    private void InitializeUI()
    {
        AudioManager_PlayerController audioManager =
            AudioManager_PlayerController.Instance;

        if (audioManager == null)
        {
            Debug.LogWarning(
                "[AudioSettingsUI] " +
                "AudioManager_PlayerController가 없습니다."
            );

            return;
        }


        SetInitialValue(
            masterSlider,
            masterValueText,
            audioManager.GetMasterVolume()
        );

        SetInitialValue(
            bgmSlider,
            bgmValueText,
            audioManager.GetBgmVolume()
        );

        SetInitialValue(
            sfxSlider,
            sfxValueText,
            audioManager.GetSfxVolume()
        );
    }


    private void SetInitialValue(
        Slider slider,
        TMP_Text valueText,
        float value)
    {
        if (slider != null)
        {
            slider.SetValueWithoutNotify(
                value
            );
        }


        RefreshValueText(
            valueText,
            value
        );
    }


    // ========================================================
    // Listener
    // ========================================================

    private void AddListeners()
    {
        if (masterSlider != null)
        {
            masterSlider.onValueChanged.AddListener(
                HandleMasterVolumeChanged
            );
        }


        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(
                HandleBgmVolumeChanged
            );
        }


        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(
                HandleSfxVolumeChanged
            );
        }
    }


    private void RemoveListeners()
    {
        if (masterSlider != null)
        {
            masterSlider.onValueChanged.RemoveListener(
                HandleMasterVolumeChanged
            );
        }


        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(
                HandleBgmVolumeChanged
            );
        }


        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(
                HandleSfxVolumeChanged
            );
        }
    }


    // ========================================================
    // Volume
    // ========================================================

    private void HandleMasterVolumeChanged(
        float value)
    {
        AudioManager_PlayerController
            .Instance?
            .SetMasterVolume(
                value
            );


        RefreshValueText(
            masterValueText,
            value
        );
    }


    private void HandleBgmVolumeChanged(
        float value)
    {
        AudioManager_PlayerController
            .Instance?
            .SetBgmVolume(
                value
            );


        RefreshValueText(
            bgmValueText,
            value
        );
    }


    private void HandleSfxVolumeChanged(
        float value)
    {
        AudioManager_PlayerController
            .Instance?
            .SetSfxVolume(
                value
            );


        RefreshValueText(
            sfxValueText,
            value
        );
    }


    private void RefreshValueText(
        TMP_Text valueText,
        float value)
    {
        if (valueText == null)
        {
            return;
        }


        int percentage =
            Mathf.RoundToInt(
                value * 100f
            );


        valueText.text =
            $"{percentage}%";
    }
}