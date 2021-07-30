using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Shows maps with levels
/// </summary>
public class PlayCanvas : MenuElement
{
    LevelScriptableObject[] levels;
    GameObject levelPrefab;
    GameObject linePrefab;
    [SerializeField] Transform levelsLocation;
    List<GameObject> currentMap = new List<GameObject>();

    bool isMapInitialized;
    private void Start()
    {
        levels = Resources.LoadAll<LevelScriptableObject>("Levels");
        levelPrefab = Resources.Load<GameObject>("Level");
        linePrefab = Resources.Load<GameObject>("Line");
        InitMap();
    }
    public override void Visibility(bool isVisible, bool willFade = false, float duration = .3f)
    {
        base.Visibility(isVisible);
        levelsLocation.gameObject.SetActive(isVisible);
        CameraController.Instance?.EnableCameraTouch(isVisible);
    }
    [Button("Redo Map")]
    public void Redo()
    {
        foreach (var obj in currentMap) Destroy(obj);
        currentMap.Clear();
        GameManager.Instance.data.randomSeed = Random.Range(1, 100);
        InitMap();
    }
    public void InitMap()
    {
        LevelMapView prevLevel = null;

        //Generate the same map for the player every time
        Random.InitState(GameManager.Instance.data.randomSeed);
        GameObject[] firstAndLast = new GameObject[2];
        LevelMapView lvGO = null;
        foreach (var level in levels)
        {
            lvGO = Instantiate(levelPrefab, levelsLocation).GetComponent<LevelMapView>();
            lvGO.Init(level);

            if(prevLevel)
            {
                var line = Instantiate(linePrefab, levelsLocation).GetComponent<LineRenderer>();
                currentMap.Add(line.gameObject);

                //Give random location
                lvGO.transform.position = Random.insideUnitCircle * 2 + new Vector2(1, Random.Range(-1f, 1f)) * 2.5f + (Vector2) prevLevel.transform.position;
                //Draw line
                DrawLine(line, lvGO.transform.position, prevLevel.transform.position);
            }
            else
            {
                firstAndLast[0] = lvGO.gameObject;
            }
            currentMap.Add(lvGO.gameObject);
            prevLevel = lvGO;
        }
        firstAndLast[1] = lvGO.gameObject;

    

        CameraController.Instance.ChangeCameraBounds(currentMap.ToArray());
    }


    void DrawLine(LineRenderer line, Vector2 startLocation, Vector2 endLocation)
    {
        line.positionCount = 2;
        line.SetPositions(new Vector3[2] { (Vector3)startLocation, (Vector3)endLocation});
    }
}
