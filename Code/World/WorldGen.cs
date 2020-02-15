using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DryadSweeper
{
    [Serializable]
    public struct StoredData
    {
        public StoredTile[,] storedTiles;
        public int stored_size_x;
        public int stored_size_y;
    }

    [Serializable]
    public struct StoredTile
    {
        public int x;
        public int y;
        public int type;
        public string message;

        public StoredTile(int x, int y, int type, string message)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.message = message;
        }
    }

    [Serializable]
    public struct TileColor
    {
        public Tile.Tiletype tile;
        public Color color;
    }

    public class Tile
    {
        public enum Tiletype { nothing, spawn, normal, bomb, echo, treasure, random, antagonist }; //Will be used to keep track of what's selected
        public Tiletype currentTile; // Create a Selection object that will be used throughout script
        public string message = "";

        public Color currentColor;

        public int value;
        public bool issealed = false;

        public TilePhysical obj;

        private bool visible = false;

        //Constructors:
        public Tile(TilePhysical obj)
        {
            value = 0;
            currentTile = Tiletype.normal;
            this.obj = obj;
        }

        public Tile(Tiletype currentTile, TilePhysical obj)
        {
            value = 0;
            this.currentTile = currentTile;
            this.obj = obj;
        }

        public Tile(Tiletype currentTile, bool visible, TilePhysical obj)
        {
            value = 0;
            this.currentTile = currentTile;
            this.obj = obj;
        }
        //end Constructors


        public void SetColor(WorldGen worldGenref)
        {
            foreach(TileColor tileColor in worldGenref.TileColors)
            {
                if (tileColor.tile == currentTile)
                {
                    currentColor = tileColor.color;
                    break;
                }
            }

            obj.display.color = currentColor;
        }

        public void EditVisible()
        {
            obj.SetDisplay(((int)currentTile).ToString());
        }

        public void SonarRecieved()
        {
            if (visible) return;

            visible = true;

            if (currentTile == Tiletype.bomb || currentTile == Tiletype.antagonist)
            {
                obj.SetDisplay("[x]");
                issealed = true;
            }
            else
            {
                obj.SetDisplay(value.ToString());
                switch (currentTile)
                {
                    case Tiletype.spawn:
                        Tile_Base(false, true);
                        break;
                    case Tiletype.normal:
                        Tile_Base(false, true);
                        break;
                    case Tiletype.bomb:
                        Tile_Security(false, true);
                        break;
                    case Tiletype.echo:
                        Tile_Sonar(false, true);
                        break;
                    case Tiletype.treasure:
                        Tile_Treasure(false, true);
                        break;
                    case Tiletype.random:
                        Tile_Random(false, true);
                        break;
                    case Tiletype.antagonist:
                        Tile_Antagonist(false, true);
                        break;
                    default:
                        Debug.LogWarning("InvalidNodeType");
                        break;
                }
            }
        }

        public void Activate()
        {
            if (!visible)
            {
                visible = true;

                ConsoleMaster.Instance.Output(string.Format("//Run:(BreachAttempt(Node[{0},{1}]));", obj.x, obj.y));

                switch (currentTile)
                {
                    case Tiletype.spawn:
                        Tile_Base(false, false);
                        break;
                    case Tiletype.normal:
                        Tile_Base(false, false);
                        break;
                    case Tiletype.bomb:
                        Tile_Security(false, false);
                        break;
                    case Tiletype.echo:
                        Tile_Sonar(false, false);
                        break;
                    case Tiletype.treasure:
                        Tile_Treasure(false, false);
                        break;
                    case Tiletype.random:
                        Tile_Random(false, false);
                        break;
                    case Tiletype.antagonist:
                        Tile_Antagonist(false, false);
                        break;
                    default:
                        Debug.LogWarning("InvalidNodeType");
                        break;
                }
            }
        }

        public void Seal()
        {
            if (!visible)
            {
                visible = true;
                issealed = true;

                ConsoleMaster.Instance.Output(string.Format("//Run:(SecuritySeal(Node[{0},{1}]));", obj.x, obj.y));

                switch (currentTile)
                {
                    case Tiletype.spawn:
                        Tile_Base(true, false);
                        break;
                    case Tiletype.normal:
                        Tile_Base(true, false);
                        break;
                    case Tiletype.bomb:
                        Tile_Security(true, false);
                        break;
                    case Tiletype.echo:
                        Tile_Sonar(true, false);
                        break;
                    case Tiletype.treasure:
                        Tile_Treasure(true, false);
                        break;
                    case Tiletype.random:
                        Tile_Random(true, false);
                        break;
                    case Tiletype.antagonist:
                        Tile_Antagonist(true, false);
                        break;
                    default:
                        Debug.LogWarning("InvalidNodeType");
                        break;
                }
            }
        }

        /*
         * Handles every type of node:
         */

        void Tile_Base(bool seal, bool muted)
        {
            if (!seal)
            {
                obj.SetDisplay(value.ToString());
                if (!muted) ConsoleMaster.Instance.Output(string.Format("//ResultOutput = SUCCESS(Output:Datamine_Results(Nearby_Secure_Nodes) = {0})", value));
            }
            else
            {
                obj.SetDisplay("[x]");
                if (!muted) ConsoleMaster.Instance.Output("//ResultOutput = FAILURE(Output:DataNodeSealed - WARNING: 'Possible detection of activities, Integrity comprimised.')");
                // Add integrity damage
            }
        }

        void Tile_Security(bool seal, bool muted)
        {
            if (!seal)
            {
                obj.SetDisplay("!");
                if (!muted) ConsoleMaster.Instance.Output("//ResultOutput = FAILURE(Output:SecurityNodeActivated - WARNING: 'Possible detection of activities, Integrity comprimised.')");
                // Add integrity damage
            }
            else
            {
                obj.SetDisplay("[x]");
                if (!muted) ConsoleMaster.Instance.Output("//ResultOutput = SUCCESS(Output:'Security node successfully sealed.')");
            }
        }

        void Tile_Sonar(bool seal, bool muted)
        {
            if (!seal)
            {
                obj.SetDisplay(value.ToString());
                if (!muted) ConsoleMaster.Instance.Output(string.Format("//ResultOutput = SUCCESS(Output:Datamine_Results(Nearby_Secure_Nodes) = {0} - NOTE: 'Search index detected, scanning...')", value));

                if (muted) ConsoleMaster.Instance.Output("//SearchIndex NOTE: 'Secondary Sonar node detected, running scan...'");

                ConsoleMaster.Instance.Output("//Run: (IndexSecurityBreaker(Walls(1-6))).AddoutputListener();");

                int rollA = UnityEngine.Random.Range(1, 7);
                int rollE = UnityEngine.Random.Range(1, 7);

                rollA = rollA - rollE;
                if (rollA < 0) { rollA *= -1; }

                rollE = 6 - rollA;

                ConsoleMaster.Instance.Output(string.Format("//OutputListener.Results(SecurityErrorThreshold = {0}, NodeRangeRetrieved = {1})", rollA, rollE));

                if (rollE > 1)
                {
                    for (int i = 1; i < rollE; i++)
                    {
                        if (obj.x - i < 0) continue;
                        if (obj.masterref.world[obj.x - i, obj.y] != null) obj.masterref.world[obj.x - i, obj.y].SonarRecieved();
                    }
                    for (int i = 1; i < rollE; i++)
                    {
                        if (obj.x + i >= obj.masterref.size_x) continue;
                        if (obj.masterref.world[obj.x + i, obj.y] != null) obj.masterref.world[obj.x + i, obj.y].SonarRecieved();
                    }
                    for (int i = 1; i < rollE; i++)
                    {
                        if (obj.y - i < 0) continue;
                        if (obj.masterref.world[obj.x, obj.y - i] != null) obj.masterref.world[obj.x, obj.y - i].SonarRecieved();
                    }
                    for (int i = 1; i < rollE; i++)
                    {
                        if (obj.y + i >= obj.masterref.size_y) continue;
                        if (obj.masterref.world[obj.x, obj.y + i] != null) obj.masterref.world[obj.x, obj.y + i].SonarRecieved();
                    }
                }
            }
            else
            {
                obj.SetDisplay("[x]");
                if (!muted) ConsoleMaster.Instance.Output("//ResultOutput = FAILURE(Output:SonarNodeSealed - WARNING: 'Possible detection of activities, Integrity comprimised.')");
                // Add integrity damage
            }
        }

        void Tile_Treasure(bool seal, bool muted)
        {
            if (!seal)
            {
                obj.SetDisplay(value.ToString());
                if (!muted) ConsoleMaster.Instance.Output(string.Format("//ResultOutput = SUCCESS(Output:Datamine_Results(Nearby_Secure_Nodes) = {0} - NOTE: 'Resource data detected, scanning...')", value));
                if (muted) ConsoleMaster.Instance.Output("//SearchIndex NOTE: 'Treasure node detected via Sonar, running scan...'");

                ConsoleMaster.Instance.Output("//Run: (IndexSecurityBreaker(Walls(1-3))).AddoutputListener();");

                int rollA = UnityEngine.Random.Range(1, 4);

                ConsoleMaster.Instance.Output(string.Format("//OutputListener.Results(EncryptedDocDecrypted = {0})", rollA));

                switch (rollA)
                {
                    case (1):
                        ConsoleMaster.Instance.Output("//DecryptionResults('Security codes recovered, integrity increased.')");
                        // Return lost integrity
                        break;
                    case (2):
                        ConsoleMaster.Instance.Output("//DecryptionResults('Possible datanode security code detected, searching for node...')");
                        // unseal one damaged node
                        bool complete = false;
                        // check there are any sealed good nodes...
                        foreach(Tile tile in obj.masterref.world)
                        {
                            if (tile.issealed && tile.currentTile != Tiletype.bomb && tile.currentTile != Tiletype.antagonist)
                            {
                                complete = true;
                                ConsoleMaster.Instance.Output("//NodeSearchResults('Sealed data node found, unlocking.')");
                                tile.SonarRecieved();

                                break;
                            }
                        }

                        if (!complete)
                        {
                            ConsoleMaster.Instance.Output("//NodeSearchResults('No sealed data node found with matching security code.')");
                        }
                        break;
                    case (3):
                        ConsoleMaster.Instance.Output("//DecryptionResults('Cache code detected, recovering...')");
                        ConsoleMaster.Instance.Output(string.Format("//CacheCode = '{0}'", message));
                        break;
                }
            }
            else
            {
                obj.SetDisplay("[x]");
                if (!muted) ConsoleMaster.Instance.Output("//ResultOutput = FAILURE(Output:TreasureNodeSealed - WARNING: 'Possible detection of activities, Integrity comprimised.')");
                // Add integrity damage
            }
        }

        void Tile_Random(bool seal, bool muted)
        {

        }

        void Tile_Antagonist(bool seal, bool muted)
        {

        }
    }

    public class WorldGen : MonoBehaviour
    {
        [Header("Prefabs:")]
        public TilePhysical tile_prefab;
        //public GameObject player_prefab;

        [Header("Settings:")]
        //[Tooltip("If selected, first use will create a new file called 'testmap' in the documents folder of your computer, this should contain the map info, and can be edited.")]
        //public bool UseCustomMap = false;
        //public string SavefolderName = "DryadSweeperSaveData";

        [Tooltip("Option main camera, if filled will zoom the camera based on map size.")]        
        public Camera localcamera;
        public SelectionController selectionController;

        public List<TileColor> TileColors = new List<TileColor>();

        [Header("RandomMap Settings:")]

        public int size_x;
        public int size_y;

        [HideInInspector]
        public Tile[,] world;

        [HideInInspector]
        public bool editmode = false;

        public StoredData PackageData()
        {
            StoredData data = new StoredData
            {
                storedTiles = new StoredTile[size_x, size_y],
                stored_size_x = size_x,
                stored_size_y = size_y
            };

            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    data.storedTiles[x, y] = new StoredTile(x, y, (int)world[x, y].currentTile, world[x, y].message);
                }
            }

            return data;
        }

        public void CreateNew(int size_x, int size_y)
        {
            this.size_x = size_x;
            this.size_y = size_y;

            SetupCamera();


            world = new Tile[size_x, size_y];

            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {

                    TilePhysical obj = Instantiate(tile_prefab, new Vector3(x - size_x / 2, 0, y - size_y / 2), Quaternion.identity, null);

                    world[x, y] = new Tile(Tile.Tiletype.nothing, false, obj);

                    world[x, y].obj.x = x;
                    world[x, y].obj.y = y;
                    world[x, y].obj.masterref = this;
                    world[x, y].SetColor(this);
                    world[x, y].EditVisible();
                }
            }

            SpawnPlayer();
        }

        public void Initialize(StoredData data)
        {
            size_x = data.stored_size_x;
            size_y = data.stored_size_y;

            SetupCamera();

            world = new Tile[size_x, size_y];

            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    if (data.storedTiles[x, y].type != 0)
                    {
                        TilePhysical obj = Instantiate(tile_prefab, new Vector3(x - size_x / 2, 0, y - size_y / 2), Quaternion.identity, null);

                        int type = data.storedTiles[x, y].type;

                        world[x, y] = new Tile((Tile.Tiletype)type, false, obj);

                        world[x, y].obj.x = x;
                        world[x, y].obj.y = y;
                        world[x, y].obj.masterref = this;
                        world[x, y].SetColor(this);
                    }
                }
            }
            AssignNumbers();

            if (!SpawnPlayer())
            {
                Debug.Log("No Spawn region");
            }            
        }

        private void AssignNumbers()
        {
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    if (world[x, y] != null)
                    {
                        if (world[x, y].currentTile == Tile.Tiletype.bomb)
                        {
                            for (int i = x - 1; i < x + 2; i++)
                            {
                                for (int j = y - 1; j < y + 2; j++)
                                {
                                    if (i > -1 && i < size_x && j > -1 && j < size_y)
                                    {
                                        if (world[i, j] != null)
                                        {
                                            if (world[i, j].currentTile != Tile.Tiletype.bomb)
                                            {
                                                world[i, j].value = world[i, j].value + 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void QueryActivate(int x, int y)
        {
            if (editmode)
            {
                int curtype = (int)world[x, y].currentTile;
                curtype--;
                if (curtype < 0) { curtype = Enum.GetValues(typeof(Tile.Tiletype)).Length - 1; }

                world[x, y].currentTile = (Tile.Tiletype)curtype;
                world[x, y].EditVisible();
                world[x, y].SetColor(this);
            }
            else
            {
                if (world[x, y] != null) world[x, y].Activate();
            }
        }

        public void QuerySeal(int x, int y)
        {
            if (editmode)
            {
                int curtype = (int)world[x, y].currentTile;
                curtype++;
                if (curtype > Enum.GetValues(typeof(Tile.Tiletype)).Length - 1) { curtype = 0; }

                world[x, y].currentTile = (Tile.Tiletype)curtype;
                world[x, y].EditVisible();
                world[x, y].SetColor(this);
            }
            else
            {
                if (world[x, y] != null) world[x, y].Seal();
            }
        }

        public void Purge()
        {
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    if (world[x, y] != null)
                    {
                        Destroy(world[x, y].obj.gameObject);
                    }
                }
            }
            world = new Tile[size_x, size_y];
            selectionController.Purge();
        }

        public bool SpawnPlayer()
        {
            if (editmode)
            {
                selectionController.Spawn(0,0, size_x, size_y);
            }
            else
            {
                //Check if pre set spawn point:

                for (int x = 0; x < size_x; x++)
                {
                    for (int y = 0; y < size_y; y++)
                    {
                        if (world[x, y] != null && world[x, y].currentTile == Tile.Tiletype.spawn)
                        {
                            world[x, y].Activate();
                            selectionController.Spawn(x, y, size_x, size_y);
                            return true;
                        }
                    }
                }

                //failing that create a default spawn point:

                for (int x = 0; x < size_x; x++)
                {
                    for (int y = 0; y < size_y; y++)
                    {
                        if (world[x, y] != null && world[x, y].currentTile != Tile.Tiletype.bomb)
                        {
                            world[x, y].Activate();
                            selectionController.Spawn(x, y, size_x, size_y);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void SetupCamera()
        {
            float largest = size_x;
            if (size_x < size_y) { largest = size_y; }

            if (localcamera != null) { localcamera.orthographicSize = largest / 2 + 1; }
        }
    }
}
