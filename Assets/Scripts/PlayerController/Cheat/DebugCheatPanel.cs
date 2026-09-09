using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개발용 Debug 버튼 UI를 동적으로 생성한다.
///
/// Scene에는 DebugPanel과 Content만 두고,
/// 실제 버튼들은 Button Prefab을 이용해
/// 실행 시 코드에서 생성한다.
/// </summary>
public class DebugCheatPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private Transform content;

    [SerializeField]
    private Button buttonPrefab;


    [Header("시스템 참조")]
    [SerializeField]
    private SquadManager squadManager;

    [SerializeField]
    private PlayerExperience playerExperience;

    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    private bool isInitialized;

    /// <summary>
    /// DebugCanvas가 런타임에 생성된 뒤
    /// Scene에 존재하는 실제 게임 시스템들을 전달받는다.
    /// </summary>
    public void Initialize(
        SquadManager squad,
        PlayerExperience experience,
        UpgradeManager_PlayerController upgrade)
    {
        squadManager = squad;
        playerExperience = experience;
        upgradeManager = upgrade;

        CreateDebugButtons();
    }

    /// <summary>
    /// 버튼 하나에 필요한 정보를 묶는다.
    /// </summary>
    private class DebugAction
    {
        public string Name;
        public Action Action;

        public DebugAction(
            string name,
            Action action)
        {
            Name = name;
            Action = action;
        }
    }

    /// <summary>
    /// 사용할 Debug 기능 목록을 만든 뒤
    /// 버튼 Prefab을 동적으로 생성한다.
    /// </summary>
    private void CreateDebugButtons()
    {
        if (isInitialized)
        {
            return;
        }

        if (content == null ||
            buttonPrefab == null)
        {
            return;
        }

        isInitialized = true;

        List<DebugAction> actions =
            new List<DebugAction>
            {
                new DebugAction(
                    "soldier +10",
                    () => AddSoldiers(10)
                ),

                new DebugAction(
                    "soldier +100",
                    () => AddSoldiers(100)
                ),

                new DebugAction(
                    "soldier +1000",
                    () => AddSoldiers(1000)
                ),

                new DebugAction(
                    "EXP +100",
                    () => AddExp(100)
                ),

                new DebugAction(
                    "Execute Enhancement",
                    ForceUpgrade
                ),

                new DebugAction(
                    "Game Over",
                    ForceGameOver
                )
            };


        foreach (DebugAction debugAction in actions)
        {
            CreateButton(debugAction);
        }
    }


    /// <summary>
    /// DebugButton Prefab 하나를 생성하고
    /// 전달받은 기능을 연결한다.
    /// </summary>
    private void CreateButton(
        DebugAction debugAction)
    {
        Button button =
            Instantiate(
                buttonPrefab,
                content
            );

        TMP_Text buttonText =
            button.GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            buttonText.text =
                debugAction.Name;
        }

        button.onClick.AddListener(
            () => debugAction.Action?.Invoke()
        );
    }


    private void AddSoldiers(int amount)
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.AddUnit(amount);
    }


    private void AddExp(int amount)
    {
        if (playerExperience == null)
        {
            return;
        }

        playerExperience.AddExp(amount);
    }


    private void ForceUpgrade()
    {
        if (upgradeManager == null)
        {
            return;
        }

        upgradeManager.OpenUpgradePanel();
    }


    private void ForceGameOver()
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.RemoveUnits(
            squadManager.CurrentCount
        );
    }
}