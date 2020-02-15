using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DryadSweeper
{
    public class ConsoleMaster : MonoBehaviour
    {
        public Transform canvastransform;
        public ScrollRect consoleWindow;
        public TextMeshProUGUI consoleOutput;

        public static ConsoleMaster Instance = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Output(string outputRequest)
        {
            consoleOutput.text = consoleOutput.text + " \n" + outputRequest;

            UpdateLayout(canvastransform); // This canvas contains the scroll rect
            consoleWindow.verticalNormalizedPosition = 0f;
        }

        /// <summary>
        /// Forces the layout of a UI GameObject and all of it's children to update
        /// their positions and sizes.
        /// </summary>
        /// <param name="xform">
        /// The parent transform of the UI GameObject to update the layout of.
        /// </param>
        public static void UpdateLayout(Transform xform)
        {
            Canvas.ForceUpdateCanvases();
            UpdateLayout_Internal(xform);
        }

        private static void UpdateLayout_Internal(Transform xform)
        {
            if (xform == null || xform.Equals(null))
            {
                return;
            }

            // Update children first
            for (int x = 0; x < xform.childCount; ++x)
            {
                UpdateLayout_Internal(xform.GetChild(x));
            }

            // Update any components that might resize UI elements
            foreach (var layout in xform.GetComponents<LayoutGroup>())
            {
                layout.CalculateLayoutInputVertical();
                layout.CalculateLayoutInputHorizontal();
            }
            foreach (var fitter in xform.GetComponents<ContentSizeFitter>())
            {
                fitter.SetLayoutVertical();
                fitter.SetLayoutHorizontal();
            }
        }
    }
}
