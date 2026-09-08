using UnityEngine;

/// <summary>
/// 병사의 외형만 관리하는 컴포넌트.
///
/// 현재는 SaveManager의 selectedSkin 인덱스를 사용하지만,
/// 로비의 스킨 시스템이 변경되더라도
/// 이 클래스 내부만 수정할 수 있도록
/// 병사의 게임 로직과 외형 선택 로직을 분리한다.
/// </summary>
public class SoldierVisualController : MonoBehaviour
{
    [Header("병사 스킨")]
    [SerializeField]
    private GameObject[] skins;

    [SerializeField]
    private SoldierAnimationController animationController;

    /// <summary>
    /// 저장된 스킨 정보를 읽어서 현재 병사 외형에 적용한다.
    /// </summary>
    public void ApplySavedSkin()
    {
        // 아직 SaveManager가 준비되지 않았다면
        // 기본 스킨을 사용한다.
        if (SaveManager.Instance == null ||
            SaveManager.Instance.Data == null)
        {
#if UNITY_EDITOR
            Debug.Log("[Skin] SaveManager 없음 → 기본 스킨 0 적용");
#endif

            ApplySkin(0);
            return;
        }

        int selectedSkin =
            SaveManager.Instance.Data.selectedSkin;

#if UNITY_EDITOR
        Debug.Log(
            $"[Skin] 저장된 selectedSkin = {selectedSkin}"
        );
#endif

        ApplySkin(selectedSkin);
    }

    /// <summary>
    /// 전달받은 인덱스의 스킨만 활성화한다.
    /// </summary>
    public void ApplySkin(int skinIndex)
    {
        if (skins == null ||
            skins.Length == 0)
        {

            return;
        }

        // 잘못된 값이 들어왔을 경우
        // 기본 스킨인 0번 사용
        if (skinIndex < 0 ||
            skinIndex >= skins.Length)
        {
            skinIndex = 0;
        }

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null)
            {
                continue;
            }

            skins[i].SetActive(
                i == skinIndex
            );
        }

        Animator selectedAnimator = skins[skinIndex].GetComponent<Animator>();

        if (animationController != null)
        {
            animationController.SetAnimator(
                selectedAnimator
            );
        }
    }
}