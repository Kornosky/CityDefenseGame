using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[ExecuteInEditMode]

public class CommandWheel : MonoBehaviour
{
    [SerializeField] float MAX_TRAVEL_DISTANCE = 100f;
    [SerializeField] float RETURN_TO_ORIGINAL_STEP = .2f;
    [SerializeField] List<CommandUI> commandUIs = new List<CommandUI>();
    [SerializeField] GameObject commandPrefab;
    [SerializeField] float radius;
    [SerializeField] CommandUI selectedCommand;

    void Start()
    {
        // Despawn if active
        //if(gameObject.activeSelf)
        //    Despawn();
    }

    void OnValidate()
    {
        InitVisuals();
    }

    private void InitVisuals()
    {
        Vector2[] points = CirclePoints(commandUIs.Count, radius);

        //// Nuke children if there's a different amount
        if (transform.childCount != commandUIs.Count)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < transform.childCount; i++)
            {
                StartCoroutine(DestroyGO(transform.GetChild(i).gameObject));
            }

            foreach (CommandUI command in commandUIs)
            {
                //place and modify
                // command
                GameObject go = Instantiate(commandPrefab, transform);

                go.transform.localPosition = points[commandUIs.IndexOf(command)];
                command.originalPosition = points[commandUIs.IndexOf(command)];
                go.GetComponent<Image>().sprite = command.icon;
                command.go = go;
            }
        }
        else
        {
            foreach (CommandUI commandUI in commandUIs)
            {
                // All modifyable properties
                commandUI.go.transform.localPosition = points[commandUIs.IndexOf(commandUI)];
                commandUI.go.GetComponent<Image>().sprite = commandUI.icon;
            }
        }
    }

    IEnumerator DestroyGO(GameObject go)
    {
        yield return new WaitForSeconds(0);
        DestroyImmediate(go);
    }

    public Vector2[] CirclePoints(int n, float radius)
    {
        var points = new Vector2[n];

        for (int i = 0; i < n; i++)
        {
            /* Distance around the circle */
            var radians = 2 * MathF.PI / n * i;

            /* Get the vector direction */
            var vertical = MathF.Sin(radians);
            var horizontal = MathF.Cos(radians);

            var spawnDir = new Vector2(horizontal, vertical);

            /* Get the spawn position */
            var spawnPos = spawnDir * radius; // Radius is just the distance away from the point
            points[i] = spawnPos;
        }

        return points;
    }
    public void UpdateVisuals(Vector2 location)
    {        
        Vector2 interactDirection = location - (Vector2) transform.position;
        // Iterate through commandUIs
        foreach (CommandUI commandUI in commandUIs)
        {
            Vector2 commandDirection = (commandUI.originalPosition).normalized;

            float dotProduct = Vector2.Dot(interactDirection.normalized, commandDirection);
            if (dotProduct > .5f )
            {
                float moveMagnitude = Mathf.Clamp(interactDirection.magnitude, 0f, MAX_TRAVEL_DISTANCE) * dotProduct;
                commandUI.go.transform.localPosition = (Vector3) commandUI.originalPosition + ((Vector3) commandDirection * moveMagnitude);
            }
            else
            {
                commandUI.go.transform.localPosition = Vector2.Lerp(commandUI.go.transform.localPosition, (Vector3)commandUI.originalPosition, RETURN_TO_ORIGINAL_STEP);
            }
        }         
    }

    public void Spawn(Vector3 location)
    {
        InitVisuals();
        gameObject.SetActive(true);
        selectedCommand = null;
        transform.position = location;
    }
    public void Despawn()
    {
        gameObject.SetActive(false);
        selectedCommand = commandUIs.OrderByDescending(ui => ui.GetDistanceTravelled()).First();

        if (selectedCommand != null && selectedCommand.GetDistanceTravelled() != 0f)
            selectedCommand.response.Invoke();  
    }
}

[Serializable]
public class CommandUI
{
    [SerializeField] public UnityEvent response;
    [SerializeField] public Sprite icon;
    [SerializeField] public GameObject go;
    [SerializeField] public Vector2 originalPosition;
    float minimumDistanceThreshold = 10f; 

    public float GetDistanceTravelled()
    {
        float travelled = Vector2.Distance(go.transform.localPosition, originalPosition);
        if (travelled <= minimumDistanceThreshold)
            return 0;
        else        
            return travelled;        
    }

}