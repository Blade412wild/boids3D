﻿using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool randomGeneration;
    [SerializeField] private int amountBoids;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private int frameRate = 60;
    [SerializeField] private bool updateFrameRate;



    [Space]
    [Header("UpdateMethod")]
    [SerializeField] private bool updatePosition;
    [SerializeField] private bool updateWhenPress;
    [SerializeField] private bool updatePerPress;

    [Space]
    [SerializeField] private bool showFullVelocity;
    [SerializeField] private bool DontUpdatePos;

    [field: Header("Refs")]
    [field: SerializeField] public List<Boid> boids { get; private set; }
    [field: SerializeField] public TrailManager trailManager;
    [SerializeField] private Boid boidPrefab;
    [SerializeField] private List<FlockBehaviourBase> ruleList;


    private int counter = 0;
    private bool mayUpdate = true;

    private void Start()
    {
        if (randomGeneration)
        {
            GenerateRandomBoids();
        }

        InitializeBoids();

    }

    private void Update()
    {
        //if (!mayUpdate) return;
        if (boids.Count == 0) return;
        if (updatePerPress)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CalculateNewPosition();
            }
        }
        else if (updateWhenPress)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CalculateNewPosition();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                CalculateNewPosition();
            }
        }
        else if (updatePosition)
        {
            CalculateNewPosition();
        }

        if (updateFrameRate)
        {
            CalculateNewPosition();//Application.targetFrameRate = frameRate; // Limit FPS to 30
            updateFrameRate = false;
        }

    }

    private void CalculateNewPosition()
    {
        for (int i = 0; i < boids.Count; i++)
        {

            Boid boid = boids[i];

            Vector2 newVelocity = Vector2.zero;
            foreach (FlockBehaviourBase rule in ruleList)
            {
                newVelocity += rule.CalculateVelocity(boid, boids, this);
            }

            boid.Velocity = (newVelocity * Time.deltaTime);

            //float targetAngle = Mathf.Atan2(boid.Velocity.y, boid.Velocity.x) * Mathf.Rad2Deg;
            //Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90f);
            //boid.transform.rotation = targetRotation;

            //boid.CurrentAngle = Mathf.SmoothDampAngle(boid.CurrentAngle, targetAngle, ref boid.AngleVelocity, rotateSpeed * Time.deltaTime);
            //boid.transform.rotation = Quaternion.Euler(0, 0, boid.CurrentAngle);

            //boid.CurrentAngle = boid.transform.rotation.eulerAngles.z;
            //float newAngle = Mathf.SmoothDampAngle(boid.CurrentAngle, targetAngle, ref boid.AngleVelocity, rotateSpeed * Time.deltaTime);
            //boid.transform.rotation = Quaternion.Euler(0, 0, newAngle);

            if (boid.ShowDebugs)
            {
                DebugVelocity(boid, boid.Velocity);
            }

            if (DontUpdatePos != true)
            {
                boid.transform.position += boid.Velocity;
            }

            ScreenBoundry.CheckIfCrossedBoundry(boid);

        }
    }

    private void GenerateRandomBoids()
    {
        ClearAndDestoryList(boids);

        for (int i = 0; i < amountBoids; i++)
        {
            Vector2 randomPos = GetRandomPosition(Screen.width, Screen.height, 100);
            Quaternion randomRotation = GetRandomRotation();

            Boid boid = Instantiate(boidPrefab, randomPos, randomRotation);
            boids.Add(boid);
        }
    }

    private void InitializeBoids()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            //float speed = Random.Range(1, 5);
            boids[i].Init(1, i);
            if(i == 0)
            {
                boids[i].gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
            }

        }

    }

    private Vector2 GetRandomPosition(float x, float y, float BorderMarging)
    {

        Vector2 screenPos = Vector2.zero;

        float randomX = Random.Range(0 + BorderMarging, x - BorderMarging);
        float randomY = Random.Range(0 + BorderMarging, y - BorderMarging);

        screenPos = Camera.main.ScreenToWorldPoint(new Vector2(randomX, randomY));


        return screenPos;
    }

    private Quaternion GetRandomRotation()
    {
        float randomRotationZ = Random.Range(0, 360);
        Quaternion randomRotation = Quaternion.identity;
        randomRotation.eulerAngles = new Vector3(0, 0, randomRotationZ);

        return randomRotation;
    }

    private void ClearAndDestoryList<T>(List<T> list) where T : MonoBehaviour
    {
        foreach (var item in list)
        {
            Destroy(item.gameObject);
        }

        list.Clear();
    }
    public virtual void DebugVelocity(Boid boid, Vector2 targetVelocity)
    {
        if (boid.ShowDebugs != true) return;
        Debug.DrawRay(boid.transform.position, targetVelocity, Color.green);
    }
}



