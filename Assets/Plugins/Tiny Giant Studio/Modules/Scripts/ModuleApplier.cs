using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyGiantStudio.Modules
{
    public class ModuleApplier : MonoBehaviour
    {
        public GameObject moduleTarget;

        [Tooltip("Target needs to be enabled to apply module")]
        public List<ModuleContainer> modules = new List<ModuleContainer>();

        public bool applyModules = true;

        [ContextMenu("Apply all modules")]
        public void ApplyAllModule()
        {
            if (moduleTarget == null)
                return;

            if (!moduleTarget.activeInHierarchy)
                return;

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].module)
                    StartCoroutine(modules[i].module.ModuleRoutine(moduleTarget, modules[i].variableHolders));
            }
        }

        public void ApplyModule(int i)
        {
            if (moduleTarget == null)
                return;

            if (!moduleTarget.activeInHierarchy)
                return;

            if (modules.Count >= i)
                return;

            if (modules[i].module)
                StartCoroutine(modules[i].module.ModuleRoutine(moduleTarget, modules[i].variableHolders));
        }
    }
}