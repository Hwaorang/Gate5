using System.Collections.Generic;
using UnityEngine;

namespace TinyGiantStudio.Text.SampleScene
{
    public class MText_SampleScene_FontTest : MonoBehaviour
    {
#if MODULAR_3D_TEXT //This shouldn't be required. Adding this just in case
        [SerializeField] private Modular3DText modular3DText = null;
        [SerializeField] private Modular3DText fontText = null;

        [Space]
        [SerializeField] private List<Font> fonts = new List<Font>();

        private int selectedFont = 0;

        public void NextFont()
        {
            selectedFont++;
            if (selectedFont >= fonts.Count) selectedFont = 0;

            UpdateInfo();
        }

        public void PreviousFont()
        {
            selectedFont--;
            if (selectedFont < 0) selectedFont = fonts.Count - 1;

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            modular3DText.Font = fonts[selectedFont];
            fontText.Font = fonts[selectedFont];
            fontText.Text = fonts[selectedFont].name;
        }

#endif
    }
}