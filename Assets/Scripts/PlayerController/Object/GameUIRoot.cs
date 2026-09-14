using UnityEngine;

public class GameUIRoot : MonoBehaviour
{
    [Header("Presenters")]
    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    [SerializeField]
    private GameResultPresenter gameResultPresenter;

    [SerializeField]
    private GameHUDPresenter gameHUDPresenter;


    [Header("Player UI")]
    [SerializeField]
    private SoldierCountUI soldierCountUI;

    [SerializeField]
    private ExpProgressUI expProgressUI;


    public UpgradeManager_PlayerController UpgradeManager =>
        upgradeManager;

    public GameResultPresenter GameResultPresenter =>
        gameResultPresenter;

    public GameHUDPresenter GameHUDPresenter =>
        gameHUDPresenter;

    public SoldierCountUI SoldierCountUI =>
        soldierCountUI;

    public ExpProgressUI ExpProgressUI =>
        expProgressUI;


    private void Awake()
    {
        // Prefab 내부이므로 자동 탐색 가능

        if (upgradeManager == null)
        {
            upgradeManager =
                GetComponentInChildren
                <UpgradeManager_PlayerController>(
                    true
                );
        }

        if (gameResultPresenter == null)
        {
            gameResultPresenter =
                GetComponentInChildren
                <GameResultPresenter>(
                    true
                );
        }

        if (gameHUDPresenter == null)
        {
            gameHUDPresenter =
                GetComponentInChildren
                <GameHUDPresenter>(
                    true
                );
        }

        if (soldierCountUI == null)
        {
            soldierCountUI =
                GetComponentInChildren
                <SoldierCountUI>(
                    true
                );
        }

        if (expProgressUI == null)
        {
            expProgressUI =
                GetComponentInChildren
                <ExpProgressUI>(
                    true
                );
        }
    }
}