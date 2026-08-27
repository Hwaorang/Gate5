using UnityEngine;

public class SoldierUnit : MonoBehaviour
{
    private SquadManager squadManager;

    /// <summary>
    /// SquadManager가 병사를 활성화할 때 호출
    /// </summary>
    public void Init(SquadManager manager)
    {
        squadManager = manager;

#if UNITY_EDITOR
        Debug.Log($"{name} SquadManager 연결 완료");
#endif
    }

    /// <summary>
    /// 이 병사가 공격받았을 때 호출
    /// </summary>
    public void Die()
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.RemoveUnit(this);
    }
}