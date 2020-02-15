using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DryadSweeper
{
    public class SelectionController : MonoBehaviour
    {
        public TMP_InputField messageDisplay;

        public GameObject selectorPrefab;

        public WorldGen worldgen;

        private GameObject curSelector;

        private int x, y, size_x, size_y;

        private void Update()
        {
            if (curSelector != null && !IsUIactive())
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    x--;
                    if (x < 0) x++;
                    curSelector.transform.position = new Vector3(x - size_x / 2, 0, y - size_y / 2);

                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    x++;
                    if (x >= size_x) x--;
                    curSelector.transform.position = new Vector3(x - size_x / 2, 0, y - size_y / 2);

                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    y--;
                    if (y < 0) y++;
                    curSelector.transform.position = new Vector3(x - size_x / 2, 0, y - size_y / 2);
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    y++;
                    if (y >= size_y) y--; 
                    curSelector.transform.position = new Vector3(x - size_x / 2, 0, y - size_y / 2);
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Query(0);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Query(1);
                }
            }
        }

        public void Spawn(int x, int y, int size_x, int size_y)
        {
            this.x = x;
            this.y = y;
            this.size_x = size_x;
            this.size_y = size_y;

            curSelector = Instantiate(selectorPrefab, new Vector3(x - size_x / 2, 0, y - size_y / 2), Quaternion.identity);
        }

        public void Purge()
        {
            Destroy(curSelector);
        }

        public void SaveMessage()
        {
            if (x > 0 && x < size_x && y > 0 && y < size_y)
            {
                worldgen.OverwriteMessage(messageDisplay.text, x, y);
            }
        }

        private void Query(int index)
        {
            if (x > 0 && x < size_x && y > 0 && y < size_y)
            {
                if (index == 0)
                {
                    worldgen.QueryActivate(x, y);
                }
                else
                {
                    worldgen.QuerySeal(x, y);
                }
            }
        }

        private bool IsUIactive()
        {
            return EventSystem.current.currentSelectedGameObject != null;
        }
    }
}
