using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings
{
    [Header("Display Settings")]
    public int resolutionIndex = 0;     // 해상도 인덱스
    public bool isFullscreen = true;    // 전체화면 여부
    public int targetFrameRate = 60;    // 목표 프레임레이트

    [Header("Audio Settings")]
    public float masterVolume = 1f;     // 마스터 볼륨 (0~1)
    public float sfxVolume = 1f;        // 효과음 볼륨 (0~1)
    public float musicVolume = 1f;      // 배경음 볼륨 (0~1)

    [Header("UI Setting")]
    public float uiScale = 1f;          // UI 스케일

    [Header("Graphics Setting")]
    public int qualityLevel = 2;        // 그래픽 품질 레벨
    public bool vsyncEnabled = true;    // 수직동기화 여부
}


public class SettingsManager : SingletonMonoBehaviour<SettingsManager>
{
    [Header("Audio References")]
    public AudioMixer masterMixer;

    [Header("UI Scale References")]
    public Canvas mainCanvas;

    [Header("Settings")]
    public GameSettings currentSettings = new GameSettings();

    private Resolution[] availableResolutions;
    private const string SETTINGS_KEY = "GameSettings";

    // 오디오 믹서 파라미터 이름
    private const string MASTER_VOLUME_PARAM = "MasterVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";
    private const string MUSIC_VOLUME_PARAM = "MusicVolume";

    protected override void Awake()
    {
        base.Awake();
        InitializeSettings();           //설정 초기화
    }

    private void Start()
    {
        LoadSettings();                 //설정 데이터 로드
        ApplyAllSettings();             //모든 설정 적용
    }
    private void InitializeSettings()
    {
        // 사용 가능한 해당도 목록 가져오기
        availableResolutions = Screen.resolutions;      // Screen.resolutions 현재 모니터에서 지원하는 해상도 배열을 반환

        // 현재 해상도를 기본값으로 설정
        Resolution currentRes = Screen.currentResolution;
        for(int i = 0; i < availableResolutions.Length; i++)
        {
            if (availableResolutions[i].width == currentRes.width &&
                availableResolutions[i].height == currentRes.height)
            {
                currentSettings.resolutionIndex = i;                    // 현재 해상도의 인덱스 저장
                break;
            }
        }

        // 현재 품질 설정 가져오기
        currentSettings.qualityLevel = QualitySettings.GetQualityLevel();
        currentSettings.vsyncEnabled = QualitySettings.vSyncCount > 0;
        // vSyncCount = 0 (없음)/ 1 (1번의 Vertical Blank마다 동기화, 60에 60FPS 고정)/ 2 (2번 마다, 60에 30FPS 고정)

        Debug.Log("SettingManager 초기화 완료");
    }

    #region Save/Load Settings (세이브 / 로드 설정)
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(currentSettings, true);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("설정 저장됨");
    }

    public void LoadSettings()
    {
        if(PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            JsonUtility.FromJsonOverwrite(json, currentSettings);

            Debug.Log("설정 로드됨");
        }
        else
        {
            Debug.Log(" 저장된 설정이 없어 기본값 사용");
        }
    }

    public void ResetToDefault()
    {
        currentSettings = new GameSettings();
        ApplyAllSettings();
        SaveSettings();

        Debug.Log("설정이 기본값으로 초기화됨");
    }
    #endregion

    #region Apply Settings (설정 적용 메서드)
    public void ApplyAllSettings()
    {
        ApplyDisplaySettings();
        ApplyAudioSettings();
        ApplyUISettings();
        ApplyGraphicsSettings();
    }

    private void ApplyDisplaySettings()
    {
        // 해상도 (및 전체화면) 적용
        if(availableResolutions != null && currentSettings.resolutionIndex < availableResolutions.Length)
        {
            Resolution targetRes = availableResolutions[currentSettings.resolutionIndex];
            Screen.SetResolution(targetRes.width, targetRes.height, currentSettings.isFullscreen);
        }

        // 프레임레이트 설정
        Application.targetFrameRate = currentSettings.targetFrameRate;
    }

    private void ApplyAudioSettings()
    {
        if(masterMixer != null)
        {
            // 볼륨을 dB로 변환 (0~1 범위를 -80dB ~ 80dB로)
            float masterDB = currentSettings.masterVolume > 0 ?
                Mathf.Log10(currentSettings.masterVolume) * 20 : -80f;
            float sfxDB = currentSettings.masterVolume > 0 ?
                Mathf.Log10(currentSettings.masterVolume) * 20 : -80f;
            float musicDB = currentSettings.masterVolume > 0 ?
                Mathf.Log10(currentSettings.masterVolume) * 20 : -80f;

            masterMixer.SetFloat(MASTER_VOLUME_PARAM, masterDB);
            masterMixer.SetFloat(SFX_VOLUME_PARAM, sfxDB);
            masterMixer.SetFloat(MUSIC_VOLUME_PARAM, musicDB);
        }
        else
        {
            // 오디오 믹서가 없으면 AudioListener 볼륨 조정
            AudioListener.volume = currentSettings.masterVolume;
        }
    }

    private void ApplyUISettings()
    {
        if(mainCanvas != null)
        {
            var canvasScaler = mainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if(canvasScaler != null)
            {
                canvasScaler.scaleFactor = currentSettings.uiScale;
            }
        }
    }

    private void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel(currentSettings.qualityLevel);
        QualitySettings.vSyncCount = currentSettings.vsyncEnabled ? 1 : 0;
    }
    #endregion

    #region Individual Setting Methods (개별 설정 메서드)
    // 해상도 설정
    public void SetResoultion(int resoultionIndex)
    {
        if (resoultionIndex > 0 && resoultionIndex < availableResolutions.Length)
        { 
            currentSettings.resolutionIndex = resoultionIndex;
            ApplyDisplaySettings();
        }
    }

    // 전체화면 설정
    public void SetFullscreen(bool fullscreen)
    {
        currentSettings.isFullscreen = fullscreen;
        ApplyDisplaySettings();
    }

    // 프레임레이트 설정
    public void SetTargetFrameRate(int frameRate)
    {
        currentSettings.targetFrameRate = frameRate;
        Application.targetFrameRate = frameRate;
    }

    // MasterVolume 설정
    public void SetMasterVolume(float volume)
    {
        currentSettings.masterVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        GameEvents.VolumeChanged(currentSettings.masterVolume);
    }

    // SFXVolume 설정
    public void SetSFXVolume(float volume)
    {
        currentSettings.sfxVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
    }

    // MusicVolume 설정
    public void SetMusicVolume(float volume)
    {
        currentSettings.masterVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
    }

    // 그래픽 퀄리티 설정
    public void SetQualityLevel(int qualityLevel)
    {
        currentSettings.qualityLevel = Mathf.Clamp(qualityLevel, 0, QualitySettings.names.Length - 1);
                                                                    //Project Settings → Quality 탭에서 설정한 모든 품질 레벨의 이름 리스트
        ApplyGraphicsSettings();
    }

    // 수직동기화 설정
    public void SetVSync(bool ebabled)
    {
        currentSettings.vsyncEnabled = ebabled;
        ApplyGraphicsSettings() ;
    }
    #endregion

    #region Getters
    // 지원 해상도 배열 반환
    public Resolution[] GetAvailableResolutions()
    {
        return availableResolutions;
    }

    // 지원 해상도 문자열 반환
    public string[] GetResolutionStrings()
    {
        if(availableResolutions == null) return new string[0];

        string[] resStrings = new string[availableResolutions.Length];
        for(int i = 0; i < resStrings.Length; i++)
        {
            resStrings[i] = $"{availableResolutions[i].width} x {availableResolutions[i].height}";
        }
        return resStrings;
    }

    // 그래픽 퀄리티 레벨(문자열) 반환
    public string[] GetQualityLevelStrings()
    {
        return QualitySettings.names;
    }
    #endregion
}
