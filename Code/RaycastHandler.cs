using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DryadSweeper
{
    public class RaycastHandler : MonoBehaviour
    {
        private int fingerID = -1;

        void Awake()
        {
            #if !UNITY_EDITOR
                 fingerID = 0; 
            #endif
        }

        // Update is called once per frame
        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject(fingerID))    // is the touch on the GUI
            {
                // GUI Action
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Query(0);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Query(1);
            }
        }

        private void Query(int index)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                TilePhysical tile = hit.transform.GetComponent<TilePhysical>();

                if (tile != null)
                {
                    if (index == 0)
                    {
                        tile.masterref.QueryActivate(tile.x, tile.y);
                    }
                    else
                    {
                        tile.masterref.QuerySeal(tile.x, tile.y);
                    }
                }
            }
        }
    }
}
