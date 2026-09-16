using UnityEngine;

/// <summary>
/// 병사의 외형만 관리하는 컴포넌트.
///
/// SaveManager의 selectedSkin 인덱스를 사용해서
/// 현재 병사의 스킨을 적용한다.
///
/// Pool에서 병사가 다시 활성화될 때마다
/// 저장된 스킨을 다시 읽어 적용하므로,
/// 로비에서 선택한 스킨이 게임 병사에도 반영된다.
/// </summary>
public class SoldierVisualController : MonoBehaviour
{
    [Header("병사 스킨")]

    [SerializeField]
    private GameObject[] skins;

    [SerializeField]
    private SoldierAnimationController animationController;


    /// <summary>
    /// 현재 적용된 Animator.
    /// </summary>
    public Animator CurrentAnimator { get; private set; }


    /// <summary>
    /// 등록된 스킨 개수.
    /// </summary>
    public int SkinCount =>
        skins != null
            ? skins.Length
            : 0;


    /// <summary>
    /// Object Pool에서 Soldier가 활성화될 때마다
    /// 현재 저장된 스킨을 다시 적용한다.
    /// </summary>
    private void OnEnable()
    {
        ApplySavedSkin();
    }


    /// <summary>
    /// 저장된 스킨 정보를 읽어서 현재 병사 외형에 적용한다.
    /// </summary>
    public void ApplySavedSkin()
    {
        ApplySkin(
            GetSavedSkinIndex()
        );
    }


    /// <summary>
    /// SaveManager에 저장된 현재 스킨 번호를 반환한다.
    /// SaveManager가 아직 준비되지 않았다면 기본 스킨 0번을 사용한다.
    /// </summary>
    public int GetSavedSkinIndex()
    {
        if (SaveManager.Instance == null ||
            SaveManager.Instance.Data == null)
        {
            return 0;
        }

        int selectedSkin =
            SaveManager.Instance.Data.selectedSkin;

        if (skins == null ||
            skins.Length == 0)
        {
            return 0;
        }

        if (selectedSkin < 0 ||
            selectedSkin >= skins.Length)
        {
            return 0;
        }

        return selectedSkin;
    }


    /// <summary>
    /// 전달받은 인덱스에 해당하는 Skin GameObject를 반환한다.
    ///
    /// PlayerCharacterVisual이 Soldier와 같은 외형을 만들 때
    /// 원본 Skin으로 사용한다.
    /// </summary>
    public GameObject GetSkinObject(
        int skinIndex)
    {
        if (skins == null ||
            skins.Length == 0)
        {
            return null;
        }

        if (skinIndex < 0 ||
            skinIndex >= skins.Length)
        {
            skinIndex = 0;
        }

        return skins[skinIndex];
    }


    /// <summary>
    /// 현재 저장된 스킨에 해당하는 Skin GameObject를 반환한다.
    /// </summary>
    public GameObject GetSavedSkinObject()
    {
        return GetSkinObject(
            GetSavedSkinIndex()
        );
    }


    /// <summary>
    /// 전달받은 인덱스의 스킨만 활성화한다.
    /// </summary>
    public void ApplySkin(
        int skinIndex)
    {
        if (skins == null ||
            skins.Length == 0)
        {
            CurrentAnimator = null;
            return;
        }

        // 잘못된 값이 들어왔을 경우
        // 기본 스킨인 0번 사용
        if (skinIndex < 0 ||
            skinIndex >= skins.Length)
        {
            skinIndex = 0;
        }

        for (int i = 0;
             i < skins.Length;
             i++)
        {
            if (skins[i] == null)
            {
                continue;
            }

            skins[i].SetActive(
                i == skinIndex
            );
        }


        GameObject selectedSkin =
            skins[skinIndex];

        if (selectedSkin == null)
        {
            CurrentAnimator = null;
            return;
        }


        CurrentAnimator =
            selectedSkin.GetComponent<Animator>();

        if (CurrentAnimator == null)
        {
            CurrentAnimator =
                selectedSkin.GetComponentInChildren<Animator>(true);
        }


        if (animationController != null)
        {
            animationController.SetAnimator(
                CurrentAnimator
            );
        }
    }
}
