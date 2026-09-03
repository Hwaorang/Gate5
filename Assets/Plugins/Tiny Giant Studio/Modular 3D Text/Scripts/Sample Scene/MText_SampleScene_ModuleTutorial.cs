#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text.SampleScene
{
    [ExecuteInEditMode]
    public class MText_SampleScene_ModuleTutorial : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject target;

        [SerializeField] private Sprite pressPlaySprite;
        [SerializeField] private Sprite selectTextSprite;
        [SerializeField] private Sprite openModuleSprite;
        [SerializeField] private Sprite addModuleSprite;
        [SerializeField] private Sprite selectModuleSprite;
        [SerializeField] private Sprite modifyModuleSprite;
        [SerializeField] private Sprite doSameForDeleteSprite;

#if MODULAR_3D_TEXT //This shouldn't be required. Adding this just in case
        private Modular3DText text;

        private void Awake()
        {
            text = target.GetComponent<Modular3DText>();
        }

        private void Update()
        {
            if (!Application.isPlaying) //Press play
            {
                spriteRenderer.sprite = pressPlaySprite;
                return;
            }
            if (!Selection.Contains(target)) //Select Target
            {
                spriteRenderer.sprite = selectTextSprite;
                return;
            }

            if (text.addingModules.Count == 0)
            {
                spriteRenderer.sprite = addModuleSprite;
            }
            else if (!text.addingModules[0].module)
            {
                spriteRenderer.sprite = selectModuleSprite;
            }
            else if (text.addingModules[0].variableHolders.Length <= 2)
                return;
            else if (text.addingModules[0].variableHolders[1].floatValue == 0 || text.addingModules[0].variableHolders[4].boolValue == false)
            {
                spriteRenderer.sprite = modifyModuleSprite;
            }
            else
            {
                spriteRenderer.sprite = doSameForDeleteSprite;
            }
        }

#endif
    }
}

#endif