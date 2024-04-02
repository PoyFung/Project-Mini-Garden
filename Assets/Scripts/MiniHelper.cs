using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.EventSystems;
using System.Diagnostics.Tracing;
using Random = UnityEngine.Random;

public class MiniHelper : Agent
{
    public GameObject seedPrefab;
    public GameObject waterPrefab;

    private Rigidbody rb;
    public PotList potList;

    [SerializeField] private Transform seedBox;
    [SerializeField] private Transform waterBox;

    private Transform currentPot;

    public bool hasSeed = false;
    public bool hasWater = false;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3,+3),0,Random.Range(-3,3));
        rb = GetComponent<Rigidbody>();
    }

    //Provide the Observations for the Agent
    public override void CollectObservations(VectorSensor sensor)
    {
        //Helper's position
        sensor.AddObservation(transform.localPosition);

        if (potList.allFull == false)
        {
            if (hasSeed == false)
            {
                sensor.AddObservation(seedBox.localPosition);
            }

            else if (hasSeed == true)
            {
                sensor.AddObservation(locatePot().localPosition);
            }
        }


        else if (potList.allWatered == false && potList.allFull == true)
        {
            if (hasWater == false)
            {
                sensor.AddObservation(waterBox.localPosition);
            }

            else if (hasWater == true)
            {
                sensor.AddObservation(locatePot().localPosition);
            }
        }
    }

    //Provide controls to the Agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 10f;
        rb.velocity = new Vector3(moveX, 0, moveZ) * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = -Input.GetAxisRaw("Horizontal");
        continuousActions[1] = -Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SeedBox"))
        {
            if (hasSeed == false && hasWater == false)
            {
                SetReward(1f);
                HoldObject(seedPrefab);
                hasSeed = true;
            }
        }

        else if (other.gameObject.CompareTag("WaterBox"))
        {
            if (hasWater == false && hasSeed == false)
            {
                SetReward(1f);
                HoldObject(waterPrefab);
                hasWater = true;
            }
        }

        if (other.gameObject.CompareTag("Pot"))
        {
            GameObject potObject = other.gameObject;
            PotState currentPot = potObject.GetComponent<PotState>();

            if (hasSeed == true && currentPot.isPlanted == false) //Planting the Seed
            {
                currentPot.isPlanted = true;
                PlaceObject(potObject, transform.Find("Seed(Clone)"));
                hasSeed = false;
                SetReward(10f);
                potList.PotChange();
            }

            if (hasWater == true && currentPot.isPlanted == true) //Planting the Water
            {
                currentPot.isWatered = true;
                PlaceObject(potObject, transform.Find("WaterDrop(Clone)"));
                hasWater = false;
                SetReward(10f);
            }
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public void HoldObject(GameObject prefab)
    {
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
        GameObject heldObject = Instantiate(prefab, spawnPosition + Vector3.up * 0.5f, Quaternion.identity);
        heldObject.transform.parent = transform;
    }

    public void PlaceObject(GameObject pot, Transform heldObject)
    {
        //heldObject.parent = null;
        heldObject.parent = pot.transform;
        heldObject.localPosition = new Vector3(0, 1.5f, 0);
    }

    void FixedUpdate()
    {
        RotateHelper();
    }

    public void RotateHelper()
    {
        Vector3 direction = rb.velocity.normalized;
        if (rb.velocity.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    public Transform locatePot()
    {
        for (int i = 0; potList; i++)
        {
            Transform currenPot = potList.list[i];
            PotState potState = currenPot.GetComponent<PotState>();

            if (potState.isPlanted == false && hasSeed == true || potState.isWatered == false && hasWater == true)
            {
                return currenPot;
            }
        }
        return null;
    }
}
