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
    private Rigidbody rb;

    [SerializeField] private Transform seedBox;
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

        if (hasSeed==false)
        {
            Vector3 dirToSeedBox = (seedBox.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(seedBox.localPosition);
        }

        else if (hasSeed == true)
        {
            sensor.AddObservation(targetPot.localPosition);
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
                HoldSeed();
            }
        }

        if (other.gameObject.CompareTag("Pot"))
        {

            GameObject potObject = other.gameObject;
            PotState currentPot = potObject.GetComponent<PotState>();
            if (hasSeed == false || currentPot.isPlanted == true)
            {
                SetReward(-1f);
            }

            else if (hasSeed == true && currentPot.isPlanted==false)
            {
                currentPot.isPlanted = true;
                PlaceSeed(potObject);
                SetReward(1f);
            }
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public void HoldSeed()
    {
        Vector3 seedSpawnPosition = transform.position + transform.forward * 1.5f;
        GameObject heldSeed = Instantiate(seedPrefab, seedSpawnPosition + Vector3.up * 0.5f, Quaternion.identity);
        heldSeed.transform.parent = transform;
        hasSeed = true;
    }

    public void PlaceSeed(GameObject pot)
    {
        Transform heldSeed = transform.Find("Seed(Clone)");
        heldSeed.parent = null;

        heldSeed.parent = pot.transform;
        heldSeed.localPosition = new Vector3(0,1.5f,0);
        hasSeed = false;
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
