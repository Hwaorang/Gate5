using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquadManager : MonoBehaviour
{
    [Header("병사 설정")]

    // 생성할 병사 프리팹
    [SerializeField] private GameObject soldierPrefab;

    // 게임 시작 시 생성할 병사 수
    [SerializeField] private int startCount = 1;

    // 병사 사이의 간격
    [SerializeField] private float spacing = 1.2f;

    // 현재 생성되어 있는 병사들을 관리하는 리스트
    private List<GameObject> soldiers = new List<GameObject>();

    // 현재 병사 수를 외부에서 읽을 수 있도록 제공
    public int CurrentCount => soldiers.Count;

    private void Start()
    {
        // 게임 시작 시 기본 병사 생성
        AddUnit(startCount);
    }

    private void Update()
    {
        // 병사 추가 테스트
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddUnit(1);
        }

        // 피격 테스트
        // H 키를 누르면 병사 1명 제거
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            RemoveUnit(1);
        }
    }

    /// <summary>
    /// 병사를 지정된 수만큼 추가한다.
    /// </summary>
    public void AddUnit(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject soldier = Instantiate(
                soldierPrefab,
                transform
            );

            soldiers.Add(soldier);
        }

        // 병사 위치 다시 정렬
        UpdateFormation();
    }

    /// <summary>
    /// 병사를 지정된 수만큼 제거한다.
    /// </summary>
    public void RemoveUnit(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // 병사가 더 이상 없으면 제거 중단
            if (soldiers.Count <= 0)
            {
                break;
            }

            // 리스트의 마지막 병사를 가져온다.
            int lastIndex = soldiers.Count - 1;

            GameObject soldier = soldiers[lastIndex];

            // 리스트에서 제거
            soldiers.RemoveAt(lastIndex);

            // 실제 오브젝트 삭제
            Destroy(soldier);
        }

        // 병사가 남아있다면 위치 다시 정렬
        UpdateFormation();

        // 모든 병사가 죽었는지 확인
        CheckGameOver();
    }

    /// <summary>
    /// 병사들을 3열 형태로 정렬한다.
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

    /// <summary>
    /// 병사가 모두 사망했는지 확인한다.
    /// </summary>
    private void CheckGameOver()
    {
        if (soldiers.Count <= 0)
        {
            Debug.Log("Game Over");
        }
    }
}