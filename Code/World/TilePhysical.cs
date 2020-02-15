using UnityEngine;
using UnityEditor;
using TMPro;


namespace DryadSweeper
{
    public class TilePhysical : MonoBehaviour
    {
        public TextMeshProUGUI display;

        [HideInInspector]
        public int x, y;

        [HideInInspector]
        public WorldGen masterref;

        public void SetDisplay(string displayinfo)
        {            
            if (display != null)
            {
                display.text = displayinfo;
            }
        }
    }
}
