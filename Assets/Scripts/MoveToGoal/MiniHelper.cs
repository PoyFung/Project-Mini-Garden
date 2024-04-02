using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.EventSystems;
using System.Diagnostics.Tracing;

public class MiniHelper : Agent
{
    public GameObject seedPrefab;
    public GameObject waterPrefab;

    private Rigidbody rb;
    public PotList potList;

    [SerializeField] private Transform seedBox;
    [SerializeField] private Transform waterBox;
    [SerializeField] private Transform targetPot;

    private Transform currentPot;

    public bool hasSeed=false;
    public bool hasWater = false;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }

    //Provide the Observations for the Agent
    public override void CollectObservations(VectorSensor sensor)
    {
        //Helper's position
        sensor.AddObservation(transform.localPosition);

        if (potList != null && potList.list.Count > 0)
        {
            // Loop through each pot in the list
            foreach (Transform pot in potList.list)
            {
                PotState potState = pot.GetComponent<PotState>();

                // Add observations for the pot's position
                sensor.AddObservation(pot.localPosition);

                // Add observations for whether the pot is planted and watered
                sensor.AddObservation(potState.isPlanted ? 1f : 0f);
                sensor.AddObservation(potState.isWatered ? 1f : 0f);
            }
        }

        if (potList.allFull==false)
        {
            if (hasSeed == false)
            {
                sensor.AddObservation(seedBox.localPosition);
            }

            else if (hasSeed == true)
            {
                sensor.AddObservation(targetPot.localPosition);
            }
        }

        else if (potList.allWatered==false)
        {
            if (hasWater == false)
            {
                sensor.AddObservation(waterBox.localPosition);
            }

            else if (hasWater == true)
            {
                sensor.AddObservation(targetPot.localPosition);
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
        ActionSegment<float> continuousActions= actionsOut.ContinuousActions;
        continuousActions[0] = -Input.GetAxisRaw("Horizontal");
        continuousActions[1] = -Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SeedBox"))
        {
            if (hasSeed==false)
            {
                SetReward(1f);
                HoldObject(seedPrefab);
                hasSeed = true;
            }

            else if (hasSeed == true)
            {
                SetReward(-1f);
            }
        }

        else if (other.gameObject.CompareTag("WaterBox"))
        {
            if (hasWater == false)
            {
                SetReward(1f);
                HoldObject(waterPrefab);
                hasWater = true;
            }

            else if (hasWater == true)
            {
                SetReward(-1f);
            }
        }

        if (other.gameObject.CompareTag("Pot"))
        {
            GameObject potObject = other.gameObject;
            PotState currentPot = potObject.GetComponent<PotState>();

            if (hasSeed == false && currentPot.isPlanted == true)
            {
                SetReward(-1f);
            }

            else if (hasSeed == true && currentPot.isPlanted==false)
            {
                currentPot.isPlanted = true;
                PlaceObject(potObject, transform.Find("Seed(Clone)"));
                hasSeed = false;
                SetReward(1f);
                potList.PotChange();
            }

            if (hasWater == true && currentPot.isPlanted == true)
            {
                currentPot.isWatered = true;
                PlaceObject(potObject, transform.Find("WaterDrop(Clone)"));
                hasWater = false;
                SetReward(1f);
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
}
