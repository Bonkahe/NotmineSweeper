using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DryadSweeper
{
    public class MenuController : MonoBehaviour
    {
        public TMP_InputField size_x_input;
        public TMP_InputField size_y_input;

        public TMP_InputField new_mapname;

        public Button mapitemPrefab;
        public Transform maplistParent;

        public string SavefolderName = "DryadSweeperSaveData";

        public UnityEvent LoadMapEvent;

        private WorldGen worldGen;
        private List<Button> mapOptions = new List<Button>();

        private void Awake()
        {
            worldGen = GetComponent<WorldGen>();

            //check map directory.
            if (!CheckSaveDir())
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + SavefolderName); // returns a DirectoryInfo object
            }
        }

        private bool CheckSaveDir()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //Debug.Log(path);
            string[] subdirectoryEntries = Directory.GetDirectories(path);
            bool found = false;
            foreach (string subdirectory in subdirectoryEntries)
            {
                //Debug.Log(subdirectory);
                if (subdirectory == SavefolderName)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        //Exposed functions for the unity canvas to use.

        public void CreateTemplate()
        {
            worldGen.editmode = true;

            if (size_x_input.text == null) { size_x_input.text = "0"; }
            if (size_y_input.text == null) { size_y_input.text = "0"; }

            int sizex = int.Parse(size_x_input.text);
            int sizey = int.Parse(size_y_input.text);

            if (sizex == 0) { sizex = 1; }
            if (sizey == 0) { sizey = 1; }

            worldGen.CreateNew(sizex,sizey);
        }

        public void SaveMap()
        {
            string name = new_mapname.text;
            if (name == null) { name = "DefaultMapName"; }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + SavefolderName + "/" + name + ".dsmap");

            StoredData data = worldGen.PackageData();

            bf.Serialize(file, data);
            file.Close();

            worldGen.Purge();
        }

        public void Refresh()
        {
            foreach(Button obj in mapOptions)
            {
                Destroy(obj.gameObject);
            }

            mapOptions = new List<Button>();

            string[] fileEntries = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + SavefolderName + "/");
            foreach (string fileName in fileEntries)
            {
                if (fileName.Substring(fileName.Length - 5) == "dsmap")
                {
                    Button obj = Instantiate(mapitemPrefab, new Vector3(0, 0, 0), Quaternion.identity, maplistParent);
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(fileName);
                    mapOptions.Add(obj);

                    obj.onClick.AddListener(delegate { LoadFile(fileName); });
                }
            }
        }

        public void LoadFile(string filename)
        {
            worldGen.editmode = false;
            LoadMapEvent.Invoke();

            Debug.Log("Loading map... " + filename);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open);
            StoredData data = (StoredData)bf.Deserialize(file);
            file.Close();

            worldGen.Initialize(data);
        }
    }
}
