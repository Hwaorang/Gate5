using UnityEngine;

/// <summary>
/// PlayerRoot 내부의 주요 컴포넌트를 한 곳에서 제공한다.
///
/// 외부 시스템이 PlayerRoot 내부 구조를 하나하나 알 필요 없이
/// PlayerContext만 전달받아 필요한 기능에 접근할 수 있도록 한다.
///
/// Facade 역할:
/// - PlayerController
/// - PlayerStats
/// - PlayerExperience
/// - SquadManager
/// </summary>
public class PlayerContext : MonoBehaviour
{
    [Header("Player Components")]

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private PlayerExperience playerExperience;

    [SerializeField]
    private SquadManager squadManager;


    // 외부에서는 읽기만 가능
    public PlayerController PlayerController =>
        playerController;

    public PlayerStats PlayerStats =>
        playerStats;

    public PlayerExperience PlayerExperience =>
        playerExperience;

    public SquadManager SquadManager =>
        squadManager;


    private void Awake()
    {
        // Inspector 연결을 깜빡해도
        // 같은 PlayerRoot에 붙어 있다면 자동으로 찾는다.

        if (playerController == null)
        {
            playerController =
                GetComponent<PlayerController>();
        }

        if (playerStats == null)
        {
            playerStats =
                GetComponent<PlayerStats>();
        }

        if (playerExperience == null)
        {
            playerExperience =
                GetComponent<PlayerExperience>();
        }

        if (squadManager == null)
        {
            squadManager =
                GetComponent<SquadManager>();
        }
    }
}