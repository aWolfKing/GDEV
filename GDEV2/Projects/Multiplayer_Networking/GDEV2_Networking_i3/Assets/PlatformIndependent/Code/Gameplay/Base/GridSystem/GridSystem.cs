using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridSystem : MonoBehaviour {

    public struct GridCell{
        
        [Flags]
        public enum Type{
            walkable = 1 << 0,
            enemySpawn = walkable << 1,
            visualOption0 = enemySpawn << 1,
            visualOption1 = visualOption0 << 1,
            visualOption2 = visualOption1 << 1,
            option0 = visualOption2 << 1,
            option1 = option0 << 1,
            option2 = option1 << 1
        }

        private Type cellType;
        public Type CellType{
            get{
                return this.cellType;
            }
        }

        public GridCell(Type cellType){
            this.cellType = cellType;
        }

    }

    [SerializeField] private string mapResourcePath = "Maps/testMap";
    [SerializeField] private float gridCellSize = 1f;
    [SerializeField] private Transform cameraRoot = null;

    [SerializeField] private GameObject notWalkablePrefab = null,
                                        walkablePrefab = null;

    private List<Transform> mapCells = new List<Transform>();
    private List<Transform> playerSpawns = new List<Transform>();

    private GridCell.Type[] _mapTiles = null;

    private int mapWidth = 0,
                mapHeight = 0;

    private static GridSystem _this = null;
    internal static GridSystem Instance{
        get{
            return _this;
        }
    }

    private static System.Action onMapLoad = null;
    public static System.Action OnMapLoad{
        get{
            return onMapLoad;
        }
        set{
            onMapLoad = value;
        }
    }


    public static Vector3[] GetAllPlayerSpawnPoints(){
        List<Vector3> points = new List<Vector3>();
        foreach(var spawn in _this.playerSpawns){
            points.Add(spawn.position);
        }
        return points.ToArray();
    }



    private void Awake() {
        _this = this;
    }

    private void Start() {
        string mapTxt = Resources.Load<TextAsset>(this.mapResourcePath).text;

        int mapWidth = 10000;
        int mapHeight = 10000;

        List<List<string>> mapTxtTiles = new List<List<string>>();

        string[] lines = mapTxt.Split('\n');
        foreach(var line in lines){
            string[] words = line.Split(' ');
            if(words.Length > 0){
                mapTxtTiles.Add(new List<string>(words));
                if(words.Length < mapWidth) {
                    mapWidth = words.Length;
                }
            }
        }
        mapHeight = mapTxtTiles.Count;

        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;

        List<byte> mapTiles = new List<byte>();

        foreach(var mapStrokeX in mapTxtTiles){

            byte[] bytes = new byte[mapWidth];

            for(int i=0; i<mapStrokeX.Count; i++){
                bytes[i] = byte.Parse(mapStrokeX[i], System.Globalization.NumberStyles.HexNumber);
            }

            mapTiles.AddRange(bytes);

        }

        /*
        Color[] colors = new Color[]{
            Color.red,
            Color.green
        };
        */
        

        GameObject parent = new GameObject();
        parent.transform.position = Vector3.zero;
        parent.name = "MapCells";


        for(int y=0; y<mapHeight; y++){ 
            for(int x=0; x<mapWidth; x++){

                //Debug.DrawLine(new Vector3(x * this.gridCellSize, 0, y * this.gridCellSize), new Vector3(x * this.gridCellSize, 0, y * this.gridCellSize) + (Vector3.up * (mapTiles[y*mapWidth + x]+1)), colors[mapTiles[y * mapWidth + x] > 0 ? 1 : 0], 200f);

                //MonoBehaviour.print((GridSystem.GridCell.Type)mapTiles[y * mapWidth + x]);

                GameObject obj = GameObject.Instantiate(mapTiles[y * mapWidth + x] > 0 ? this.walkablePrefab : this.notWalkablePrefab);
                obj.transform.SetParent(parent.transform);
                obj.transform.position = new Vector3(x * this.gridCellSize, 0, y * this.gridCellSize);

                obj.SetActive(false);

                if(((GridCell.Type)mapTiles[y * mapWidth + x] & GridCell.Type.option2) == GridCell.Type.option2){
                    GameObject spawnPos = new GameObject();
                    spawnPos.name = "Player spawn";
                    spawnPos.transform.position = new Vector3(x * this.gridCellSize, 0, y * this.gridCellSize);
                    this.playerSpawns.Add(spawnPos.transform);
                }

                this.mapCells.Add(obj.transform);

            }
        }

        this.cameraRoot.position = new Vector3(mapWidth * this.gridCellSize * 0.5f, 0, mapHeight * this.gridCellSize * 0.5f);


        List<GridCell.Type> grid = new List<GridCell.Type>();
        foreach(var a in mapTiles){
            grid.Add((GridCell.Type)a);
        }
        this._mapTiles = grid.ToArray();

        //StartCoroutine(AutoLoadMapCoroutine());

    }



    public static Vector3 GetPositionTo(Vector3 pos, Vector3 dir){

        int x = 0,
            y = 0;

        x = Mathf.FloorToInt(pos.z / _this.gridCellSize);
        y = Mathf.FloorToInt(pos.x / _this.gridCellSize);

        int current = y * _this.mapWidth + x;
        
        if(current < _this._mapTiles.Length){

            if(x < _this.mapWidth-1 && dir.x > 0){
                return (_this._mapTiles[current + 1] & GridCell.Type.walkable) == GridCell.Type.walkable ? _this.mapCells[current + 1].position : pos;
            }
            else if(x > 1 && dir.x < 0){
                return (_this._mapTiles[current + 1] & GridCell.Type.walkable) == GridCell.Type.walkable ? _this.mapCells[current - 1].position : pos;
            }
            else if(y > 1 && dir.z < 0){
                return (_this._mapTiles[current + 1] & GridCell.Type.walkable) == GridCell.Type.walkable ? _this.mapCells[current - _this.mapWidth].position : pos;
            }
            else if(y < _this.mapHeight - 1 && dir.z > 0) {
                return (_this._mapTiles[current + 1] & GridCell.Type.walkable) == GridCell.Type.walkable ? _this.mapCells[current + _this.mapWidth].position : pos;
            }

        }

        return pos;
    }



    private IEnumerator AutoLoadMapCoroutine(){
        yield return new WaitForSeconds(3f);
        yield return LoadMapCoroutine();
    }



    public static void LoadMap(){
        Instance.LoadMap_instance();
    }

    private void LoadMap_instance(){
        StartCoroutine(LoadMapCoroutine());
    }



    private IEnumerator LoadMapCoroutine(){
        yield return new WaitForEndOfFrame();

        Dictionary<Transform, float> cell_speed_pair = new Dictionary<Transform, float>();
        Dictionary<Transform, Vector3> cell_velocity_pair = new Dictionary<Transform, Vector3>();

        float maxDuration = 5f;

        foreach(var cell in this.mapCells){

            cell.gameObject.SetActive(true);

            cell.transform.position = new Vector3(cell.transform.position.x, -600, cell.transform.position.z);
            cell_speed_pair.Add(cell, UnityEngine.Random.Range(500f, 700f));
            cell_velocity_pair.Add(cell, new Vector3(0, cell_speed_pair[cell], 0));

        }

        var wait = new WaitForFixedUpdate();

        float curr_duration = 0;

        do{ 
            bool done = true;
            foreach(var cell in this.mapCells){

                Vector3 vel = cell_velocity_pair[cell];
                cell.position = Vector3.SmoothDamp(cell.position, new Vector3(cell.position.x, 0, cell.position.z), ref vel, 0.5f, cell_speed_pair[cell], Time.fixedDeltaTime);
                cell_velocity_pair[cell] = vel;

                if(done && Vector3.Distance(cell.position, new Vector3(cell.position.x, 0, cell.position.z)) > 0.01f){
                    done = false;
                }

            }
            yield return wait;
            curr_duration += Time.fixedDeltaTime;
            if(curr_duration >= maxDuration){
                done = true;
            }
            if(done){
                break;
            }
        }
        while(true);

        foreach(var cell in this.mapCells){

            cell.position = new Vector3(cell.position.x, 0, cell.position.z);

        }

        if(onMapLoad != null){ 
            onMapLoad();
        }

    }

}
