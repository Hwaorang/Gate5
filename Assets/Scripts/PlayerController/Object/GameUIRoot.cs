using UnityEngine;

/// <summary>
/// 게임 UI 모듈의 진입점.
///
/// PlayerBootstrapper가 UI 내부 구조를 직접 찾아다니지 않고
/// GameUIRoot 하나를 통해 필요한 UI 시스템을 전달받는다.
/// </summary>
public class GameUIRoot : MonoBehaviour
{
    [Header("Presenters")]

    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    [SerializeField]
    private GameResultPresenter gameResultPresenter;


    [Header("Player HUD")]

    [SerializeField]
    private SoldierCountUI soldierCountUI;

    [SerializeField]
    private ExpProgressUI expProgressUI;


    public UpgradeManager_PlayerController UpgradeManager
        => upgradeManager;

    public GameResultPresenter GameResultPresenter
        => gameResultPresenter;

    public SoldierCountUI SoldierCountUI
        => soldierCountUI;

    public ExpProgressUI ExpProgressUI
        => expProgressUI;
}