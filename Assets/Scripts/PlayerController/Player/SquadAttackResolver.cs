using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Squad 전체의 실제 공격 판정을 담당한다.
///
/// 기존에는 병사 수만큼 Raycast를 실행했지만,
/// 현재는 대형의 Column 단위로 공격을 묶어 처리한다.
///
/// 예:
/// 병사 400명 / 7열
///
/// 각 열의 앞쪽 병사 1명이 대표 공격
/// + 해당 열에 살아있는 병사 수만큼 데미지 합산
///
/// 이를 통해 병사 수가 증가해도
/// 공격 처리량이 크게 증가하지 않도록 한다.
/// </summary>
public class SquadAttackResolver : MonoBehaviour
{
    [Header("Squad")]

    [SerializeField]
    private SquadManager squadManager;


    // 다음 공격까지 경과한 시간
    private float attackTimer;

    [Header("그룹 데미지")]

    [Tooltip(
        "한 번의 Raycast에 몇 명의 병사 공격력을 묶어서 처리할지 설정한다."
    )]
    [SerializeField]
    [Min(1)]
    private int soldiersPerDamageBatch = 5;

    [Header("사격 FX")]

    [Tooltip("한 번의 공격에서 FX를 보여줄 최대 병사 수")]
    [SerializeField]
    [Min(1)]
    private int maxVisualShooters = 28;


    [Tooltip(
        "앞쪽 몇 줄의 병사를 FX 후보로 사용할지"
    )]
    [SerializeField]
    [Min(1)]
    private int visualFrontRows = 4;


    // 실제 Column 공격을 담당한 대표 병사를 기록한다.
    // 추가 Visual FX가 같은 병사에게 중복 재생되는 것을 막는다.
    private readonly HashSet<SoldierAttack>
        representativeShooters = new();

    private void Awake()
    {
        if (squadManager == null)
        {
            squadManager =
                GetComponent<SquadManager>();
        }
    }


    private void Update()
    {
        HandleAttack();
    }


    /// <summary>
    /// Squad의 공격 주기를 처리한다.
    ///
    /// 기존 분산 사격과 달리
    /// 병사 수가 증가해도 공격 주기가 늘어나지 않는다.
    /// </summary>
    private void HandleAttack()
    {
        if (squadManager == null)
        {
            return;
        }

        IReadOnlyList<SoldierAttack> attacks =
            squadManager.SoldierAttacks;

        if (attacks == null ||
            attacks.Count == 0)
        {
            return;
        }


        // =========================
        // 공격속도 가져오기
        // =========================

        SoldierAttack firstAttack =
            FindFirstAliveAttack(attacks);

        if (firstAttack == null)
        {
            return;
        }

        attackTimer +=
            Time.deltaTime;

        if (attackTimer <
            firstAttack.AttackDelay)
        {
            return;
        }

        attackTimer = 0f;


        // =========================
        // Column 단위 공격
        // =========================

        FireColumns(
            attacks
        );
    }


    /// <summary>
    /// 각 Column별로 대표 병사를 선정하고
    /// 해당 열의 병사 수를 데미지에 반영한다.
    ///
    /// 한 번의 Squad 공격 주기마다
    /// 총소리는 한 번만 재생한다.
    /// </summary>
    private void FireColumns(
        IReadOnlyList<SoldierAttack> attacks)
    {
        int columnCount =
            squadManager.CurrentColumnCount;


        if (columnCount <= 0)
        {
            return;
        }

        representativeShooters.Clear();

        // 실제로 이번 공격 주기에
        // 발사한 병사가 있었는지 확인한다.
        bool firedAnyColumn =
            false;


        for (int column = 0;
             column < columnCount;
             column++)
        {
            SoldierAttack representative =
                null;

            int aliveSoldierCount =
                0;


            // =========================
            // 해당 Column 병사 탐색
            // =========================

            for (int index = column;
                 index < attacks.Count;
                 index += columnCount)
            {
                SoldierAttack attack =
                    attacks[index];


                if (attack == null)
                {
                    continue;
                }


                // 죽어가는 병사는
                // 공격력 계산에서 제외한다.
                if (!attack.CanAttack)
                {
                    continue;
                }


                aliveSoldierCount++;


                // 가장 앞쪽의 살아있는 병사를
                // 대표 공격자로 사용한다.
                if (representative == null)
                {
                    representative =
                        attack;
                }
            }


            if (representative == null ||
                aliveSoldierCount <= 0)
            {
                continue;
            }


            // =========================
            // 대표 공격
            // =========================

            representative.FireGroup(
                aliveSoldierCount,
                soldiersPerDamageBatch,
                true
            );

            representativeShooters.Add(representative);

            // 최소 한 Column이라도
            // 실제 공격을 수행했다.
            firedAnyColumn =
                true;
        }

        // =========================
        // 추가 사격 FX
        // =========================

        if (firedAnyColumn)
        {
            PlayAdditionalVisuals(
                attacks,
                columnCount
            );
        }

        // =========================
        // Squad Shoot SFX
        // =========================
        //
        // Column 수나 병사 수와 관계없이
        // 한 번의 공격 주기마다
        // 총소리는 단 한 번만 재생한다.

        if (firedAnyColumn)
        {
            AudioManager_PlayerController
                .Instance?
                .PlaySfx(
                    PlayerSfxType.PlayerShoot
                );
        }
    }

    /// <summary>
    /// 실제 데미지는 추가하지 않고
    /// 앞쪽 병사들에게 시각적인 총알 FX만 추가한다.
    ///
    /// 대표 Column 병사는 이미 FireGroup에서
    /// FX가 발생했으므로 제외한다.
    /// </summary>
    private void PlayAdditionalVisuals(
        IReadOnlyList<SoldierAttack> attacks,
        int columnCount)
    {
        if (attacks == null ||
            attacks.Count == 0)
        {
            return;
        }

        if (maxVisualShooters <= 0 ||
            visualFrontRows <= 0)
        {
            return;
        }


        // 예:
        // Column 7
        // Front Rows 4
        //
        // → 앞쪽 최대 28명을 후보로 사용
        int candidateCount =
            Mathf.Min(
                attacks.Count,
                columnCount *
                visualFrontRows
            );


        // 대표 병사들은 이미 FX를 출력했다.
        int currentVisualCount =
            representativeShooters.Count;


        for (int i = 0;
             i < candidateCount;
             i++)
        {
            if (currentVisualCount >=
                maxVisualShooters)
            {
                break;
            }


            SoldierAttack attack =
                attacks[i];


            if (attack == null ||
                !attack.CanAttack)
            {
                continue;
            }


            // 대표 병사는 이미 FireGroup에서
            // FX를 출력했으므로 중복 제외
            if (representativeShooters.Contains(
                    attack))
            {
                continue;
            }


            // 데미지는 없는 시각 효과만 출력
            attack.PlayVisualOnly();


            currentVisualCount++;
        }
    }

    /// <summary>
    /// 현재 살아있는 병사 중
    /// 첫 번째 SoldierAttack을 찾는다.
    ///
    /// AttackDelay 확인용으로 사용한다.
    /// </summary>
    private SoldierAttack FindFirstAliveAttack(
        IReadOnlyList<SoldierAttack> attacks)
    {
        for (int i = 0;
             i < attacks.Count;
             i++)
        {
            SoldierAttack attack =
                attacks[i];

            if (attack != null &&
                attack.CanAttack)
            {
                return attack;
            }
        }

        return null;
    }
}