using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathfinder : MonoBehaviour
{
    public float creatureSpeed;
    public float pathMultiplier;
    int pathIndex = 0;
    public DNA dna;
    public bool hasFinished = false;
    public LayerMask obstacleLayer;
    bool hasBeenInitialized = false;
    bool hasCrashed = false;
    bool noHacerMas = false;
    Vector2 target;
    Vector2 nextPoint;
    LineRenderer lr;
    List<Vector2> travelledPath = new List<Vector2>();

    public void InitCreature(DNA newDna, Vector2 _target)
    {
        travelledPath.Add(transform.position);
        lr = GetComponent<LineRenderer>();
        dna = newDna;
        target = _target;
        nextPoint = transform.position;
        hasBeenInitialized = true;
        travelledPath.Add(nextPoint);
    }
    private void FixedUpdate()
    {
        if (hasBeenInitialized && !hasFinished && !noHacerMas)
        {
            if(pathIndex == dna.genes.Count || Vector2.Distance(transform.position, target) < 2f)
            {
                hasFinished = true;
                if(Vector2.Distance(transform.position, target) < 2f){
                    noHacerMas = true;
                    Time.timeScale = 0;
                    return;
                }
            }
            if((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + dna.genes[pathIndex] * pathMultiplier;
                travelledPath.Add(nextPoint);
                pathIndex++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
            }
            RenderLine();
        }
    }

    public void RenderLine(){
        List<Vector3> linePoints = new List<Vector3>();
        if(travelledPath.Count > 2){
            for (int i = 0; i < travelledPath.Count - 1; i++)
            {
                linePoints.Add(travelledPath[i]);
            }
        }
        else{
            linePoints.Add(travelledPath[0]);
        }
        linePoints.Add(transform.position);
        lr.positionCount = linePoints.Count;
        lr.SetPositions(linePoints.ToArray());
    }

    public float fitness
    {
        get
        {
            float dist = Vector2.Distance(transform.position, target);
            if(dist == 0)
            {
                dist = 0.0001f;
            }
            RaycastHit2D[] obstacles = Physics2D.RaycastAll(transform.position, target, obstacleLayer);
            float obstacleMultiplier = 1 - (0.15f * obstacles.Length);
            return (60/dist) * (hasCrashed ? 0.65f : 1f) * obstacleMultiplier;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.layer == 8){
            hasFinished = true;
            hasCrashed = true;
        }
    }

}
