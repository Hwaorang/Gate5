using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquadManager : MonoBehaviour
{
    [Header("병사 설정")]

    // 병사를 생성/반환할 오브젝트 풀
    [SerializeField] private SoldierPool soldierPool;

    // 시작 병사 수
    [SerializeField] private int startCount = 1;

    // 병사 간격
    [SerializeField] private float spacing = 1.2f;

    // 현재 사용 중인 병사 목록
    private List<GameObject> soldiers = new List<GameObject>();

    // 현재 병사 수
    public int CurrentCount => soldiers.Count;

    private void Start()
    {
        AddUnit(startCount);
    }

    private void Update()
    {
        // 테스트용 병사 추가
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddUnit(1);
        }
    }

    /// <summary>
    /// 풀에서 병사를 가져와 현재 분대에 추가한다.
    /// </summary>
    public void AddUnit(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject soldier =
                soldierPool.GetSoldier(transform);

            soldiers.Add(soldier);

            // Soldier에게 자신을 관리하는 SquadManager 전달
            SoldierUnit soldierUnit =
                soldier.GetComponent<SoldierUnit>();

            if (soldierUnit != null)
            {
                soldierUnit.Init(this);
            }
        }

        UpdateFormation();
    }

    /// <summary>
    /// 현재 병사를 제거하고 풀에 반환한다.
    /// </summary>
    public void RemoveUnit(SoldierUnit soldier)
    {
        if (soldier == null)
        {
            return;
        }

        GameObject soldierObject = soldier.gameObject;

        // 현재 병사 리스트에서 해당 병사 제거
        soldiers.Remove(soldierObject);

        // 오브젝트 풀로 반환
        soldierPool.ReturnSoldier(soldierObject);

        // 남은 병사 재정렬
        UpdateFormation();

        CheckGameOver();
    }

    /// <summary>
    /// 현재 병사들을 대형에 맞게 정렬한다.
    /// </summary>
    private void UpdateFormation()
    {
        int columnCount = 3;

        for (int i = 0; i < soldiers.Count; i++)
        {
            int row = i / columnCount;
            int column = i % columnCount;

            float x = (column - 1) * spacing;
            float z = -row * spacing;

            soldiers[i].transform.localPosition =
                new Vector3(x, 0f, z);
        }
    }

    private void CheckGameOver()
    {
        if (soldiers.Count <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}