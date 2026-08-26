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

    private void Start()
    {
        // 게임 시작 시 기본 병사 생성
        AddUnit(startCount);
    }

    private void Update()
    {
        // 테스트용 코드
        // Space 키를 누르면 병사 1명 추가
        // 게이트 시스템이 완성되면 제거 예정
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddUnit(1);
        }
    }

    /// <summary>
    /// 병사를 지정된 수만큼 추가한다.
    /// </summary>
    public void AddUnit(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // PlayerRoot의 자식으로 병사 생성
            GameObject soldier = Instantiate(
                soldierPrefab,
                transform
            );

            // 생성된 병사를 리스트에 저장
            soldiers.Add(soldier);
        }

        // 병사 수가 변경됐으므로 위치를 다시 정렬
        UpdateFormation();
    }

    /// <summary>
    /// 병사들을 3열 형태로 정렬한다.
    /// </summary>
    private void UpdateFormation()
    {
        // 한 줄에 배치할 병사 수
        int columnCount = 3;

        for (int i = 0; i < soldiers.Count; i++)
        {
            // 현재 병사가 몇 번째 행인지 계산
            int row = i / columnCount;

            // 현재 병사가 몇 번째 열인지 계산
            int column = i % columnCount;

            // 좌우 위치 계산
            float x = (column - 1) * spacing;

            // 뒤쪽 방향으로 행 배치
            float z = -row * spacing;

            // PlayerRoot 기준의 로컬 위치로 배치
            soldiers[i].transform.localPosition =
                new Vector3(x, 0f, z);
        }
    }
}