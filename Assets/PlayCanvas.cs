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

    bool isMapInitialized;
    private void Start()
    {
        levels = Resources.LoadAll<LevelScriptableObject>("Levels");
        levelPrefab = Resources.Load<GameObject>("Level");
        linePrefab = Resources.Load<GameObject>("Line");
        InitMap();
    }
    public override void Visibility(bool isVisible)
    {
        base.Visibility(isVisible);
        levelsLocation.gameObject.SetActive(isVisible);
    }
    public void InitMap()
    {
        LevelMapView prevLevel = null;
        foreach(var level in levels)
        {
            var lvGO = Instantiate(levelPrefab, levelsLocation).GetComponent<LevelMapView>();
            lvGO.Init(level);

            if(prevLevel)
            {
                var line = Instantiate(linePrefab, levelsLocation).GetComponent<LineRenderer>();

                //Give random location
                lvGO.transform.position = Random.insideUnitCircle * 2 + Vector2.one + (Vector2) prevLevel.transform.position;
                //Draw line
                DrawLine(line, lvGO.transform.position, prevLevel.transform.position);
            }

            prevLevel = lvGO;
        }
    }


    void DrawLine(LineRenderer line, Vector2 startLocation, Vector2 endLocation)
    {
        line.positionCount = 2;
        line.SetPositions(new Vector3[2] { (Vector3)startLocation, (Vector3)endLocation});
    }
}
