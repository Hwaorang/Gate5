using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class AudioManager_PlayerController : MonoBehaviour
{
    [ContextMenu("Test Player Shoot SFX")]
    private void TestPlayerShootSfx()
    {
        PlaySfx(
            PlayerSfxType.PlayerShoot
        );
    }

    public static AudioManager_PlayerController Instance
    {
        get;
        private set;
    }

    [Header("Default Volume")]

    [SerializeField]
    [Range(0f, 1f)]
    private float defaultMasterVolume = 0.5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float defaultBgmVolume = 0.5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float defaultSfxVolume = 0.5f;

    private float currentMasterVolume;
    private float currentBgmVolume;
    private float currentSfxVolume;

    // =========================
    // SFX Data
    // =========================

    [Serializable]
    private class SfxData
    {
        public PlayerSfxType type;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;


        [Tooltip(
            "같은 효과음이 너무 빠르게 반복되는 것을 막는 최소 간격"
        )]
        public float minInterval = 0f;
    }


    // =========================
    // Audio Source
    // =========================

    [Header("Audio Sources")]

    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource sfxSource;


    // =========================
    // Mixer
    // =========================

    [Header("Audio Mixer")]

    [SerializeField]
    private AudioMixer audioMixer;


    // =========================
    // BGM
    // =========================

    [Header("BGM")]

    [SerializeField]
    private AudioClip inGameBgm;


    // =========================
    // SFX
    // =========================

    [Header("SFX")]

    [SerializeField]
    private List<SfxData> sfxDatas;


    private Dictionary<PlayerSfxType, SfxData>
        sfxDictionary;


    // 같은 효과음 연속 재생 제한용
    private Dictionary<PlayerSfxType, float>
        lastPlayTimes =
            new Dictionary<PlayerSfxType, float>();


    // =========================
    // PlayerPrefs Keys
    // =========================

    private const string MasterVolumeKey =
        "Audio_PlayerController_Master";

    private const string BgmVolumeKey =
        "Audio_PlayerController_BGM";

    private const string SfxVolumeKey =
        "Audio_PlayerController_SFX";


    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);

            return;
        }


        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );


        CreateDictionary();

        // PlayerPrefs의 값만 읽어온다.
        LoadVolumeValues();
    }

    private void Start()
    {
        // AudioMixer가 초기화된 이후
        // 실제 볼륨을 적용한다.
        ApplyVolumeSettings();
    }

    // ========================================================
    // Dictionary
    // ========================================================

    private void CreateDictionary()
    {
        sfxDictionary =
            new Dictionary<PlayerSfxType, SfxData>();


        foreach (SfxData data in sfxDatas)
        {
            if (data == null ||
                data.clip == null)
            {
                continue;
            }


            sfxDictionary[data.type] =
                data;
        }
    }


    // ========================================================
    // SFX
    // ========================================================

    public void PlaySfx(
        PlayerSfxType type)
    {
        if (sfxSource == null)
        {
            return;
        }


        if (!sfxDictionary.TryGetValue(
                type,
                out SfxData data))
        {
            Debug.LogWarning(
                $"[AudioManager_PlayerController] " +
                $"{type} 사운드가 없습니다."
            );

            return;
        }

        //test
        Debug.Log(
            $"[AudioManager_PlayerController] 재생 : " +
            $"{type} -> {data.clip.name}"
        );


        // =========================
        // 연속 재생 제한
        // =========================

        if (data.minInterval > 0f)
        {
            if (lastPlayTimes.TryGetValue(
                    type,
                    out float lastTime))
            {
                float elapsed =
                    Time.unscaledTime - lastTime;


                if (elapsed <
                    data.minInterval)
                {
                    return;
                }
            }


            lastPlayTimes[type] =
                Time.unscaledTime;
        }


        sfxSource.PlayOneShot(
            data.clip,
            data.volume
        );
    }


    // ========================================================
    // BGM
    // ========================================================

    public void PlayInGameBgm()
    {
        PlayBgm(
            inGameBgm
        );
    }


    public void PlayBgm(
        AudioClip clip)
    {
        if (bgmSource == null ||
            clip == null)
        {
            return;
        }


        if (bgmSource.clip == clip &&
            bgmSource.isPlaying)
        {
            return;
        }


        bgmSource.clip =
            clip;

        bgmSource.loop =
            true;

        bgmSource.Play();
    }


    public void StopBgm()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }


    // ========================================================
    // Volume
    // ========================================================

    public void SetMasterVolume(
    float value)
    {
        value =
            Mathf.Clamp01(value);

        currentMasterVolume =
            value;

        SetMixerVolume(
            "MasterVolume",
            value
        );

        PlayerPrefs.SetFloat(
            MasterVolumeKey,
            value
        );

        PlayerPrefs.Save();
    }


    public void SetBgmVolume(
        float value)
    {
        value =
            Mathf.Clamp01(value);

        currentBgmVolume =
            value;

        SetMixerVolume(
            "BGMVolume",
            value
        );

        PlayerPrefs.SetFloat(
            BgmVolumeKey,
            value
        );

        PlayerPrefs.Save();
    }


    public void SetSfxVolume(
        float value)
    {
        value =
            Mathf.Clamp01(value);

        currentSfxVolume =
            value;

        SetMixerVolume(
            "SFXVolume",
            value
        );

        PlayerPrefs.SetFloat(
            SfxVolumeKey,
            value
        );

        PlayerPrefs.Save();
    }


    private void SetMixerVolume(
        string parameterName,
        float value)
    {
        if (audioMixer == null)
        {
            return;
        }


        value =
            Mathf.Clamp01(
                value
            );


        // 0은 Log10 계산이 불가능하므로
        // 사실상 음소거 값 사용
        float db =
            value <= 0.0001f
                ? -80f
                : Mathf.Log10(value) * 20f;


        audioMixer.SetFloat(
            parameterName,
            db
        );
    }


    // ========================================================
    // Load
    // ========================================================

    // ========================================================
    // Load Volume
    // ========================================================

    private void LoadVolumeValues()
    {
        currentMasterVolume =
            PlayerPrefs.GetFloat(
                MasterVolumeKey,
                defaultMasterVolume
            );

        currentBgmVolume =
            PlayerPrefs.GetFloat(
                BgmVolumeKey,
                defaultBgmVolume
            );

        currentSfxVolume =
            PlayerPrefs.GetFloat(
                SfxVolumeKey,
                defaultSfxVolume
            );


        Debug.Log(
            $"[AudioManager] 볼륨 불러오기 | " +
            $"Master : {currentMasterVolume} / " +
            $"BGM : {currentBgmVolume} / " +
            $"SFX : {currentSfxVolume}"
        );
    }


    // ========================================================
    // Apply Volume
    // ========================================================

    private void ApplyVolumeSettings()
    {
        SetMixerVolume(
            "MasterVolume",
            currentMasterVolume
        );

        SetMixerVolume(
            "BGMVolume",
            currentBgmVolume
        );

        SetMixerVolume(
            "SFXVolume",
            currentSfxVolume
        );


        Debug.Log(
            "[AudioManager] 저장된 볼륨을 AudioMixer에 적용했습니다."
        );
    }


    // Settings UI 초기화용

    public float GetMasterVolume()
    {
        return currentMasterVolume;
    }


    public float GetBgmVolume()
    {
        return currentBgmVolume;
    }


    public float GetSfxVolume()
    {
        return currentSfxVolume;
    }
}