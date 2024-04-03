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
    private Vector3 initialPos;
    public GameObject seedPrefab;
    public GameObject waterPrefab;

    public Rigidbody rb;
    public PotList potList;

    [SerializeField] private Transform seedBox;
    [SerializeField] private Transform waterBox;
    [SerializeField] private Transform cropBox;
    [SerializeField] private Transform home;


    private Transform currentPot;

    public bool hasSeed = false;
    public bool hasWater = false;
    public bool hasCrop = false;
    private void Awake()
    {
        initialPos = transform.localPosition;
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = initialPos;
        //transform.localPosition = new Vector3(Random.Range(-3,+3),0,Random.Range(-3,3));
        rb = GetComponent<Rigidbody>();
    }

    //Provide the Observations for the Agent
    public override void CollectObservations(VectorSensor sensor)
    {
        //Helper's position
        sensor.AddObservation(transform.localPosition);

        if (potList.allSeeded == false)
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


        else if (potList.allWatered == false && potList.allSeeded == true)
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

        else if (potList.allCrop==false && potList.allWatered == true && potList.allSeeded == true)
        {
            if (hasCrop == false)
            {
                sensor.AddObservation(locatePot().localPosition);
            }

            else if (hasCrop == true)
            {
                sensor.AddObservation(cropBox.localPosition);
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
            if (hasSeed == false && hasWater == false && hasCrop == false)
            {
                SetReward(1f);
                HoldObject(seedPrefab);
                hasSeed = true;
            }
        }

        else if (other.gameObject.CompareTag("WaterBox"))
        {
            if (hasWater == false && hasSeed == false && hasCrop == false)
            {
                SetReward(1f);
                HoldObject(waterPrefab);
                hasWater = true;
            }
        }

        else if (other.gameObject.CompareTag("CropBox"))
        {
            if (hasCrop)
            {
                SetReward(10f);
                Transform potato = transform.Find("potato4(Clone)");
                Destroy(potato.gameObject);
                BoxState.cropsCollected++;
                hasCrop = false;
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

            if (hasWater == true && currentPot.isPlanted == true && currentPot.isWatered == false) //Planting the Water
            {
                currentPot.isWatered = true;
                PlaceObject(potObject, transform.Find("WaterDrop(Clone)"));
                hasWater = false;
                PotList.cropsGrowing = true;
                SetReward(10f);
                potList.PotChange();
            }

            if (currentPot.hasCrop==true && hasSeed==false && hasWater==false)
            {
                HoldObject(currentPot.finalPotato);
                potList.allSeeded = false;
                potList.allWatered = false;
                potList.allCrop = false;
                hasCrop = true;
                currentPot.finalPotato.SetActive(false);
                currentPot.hasCrop = false;
                currentPot.isPlanted = false;
                currentPot.isWatered = false;
                SetReward(10f);
                potList.PotChange();
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
        heldObject.parent = pot.transform;
        heldObject.localPosition = new Vector3(0, 0.5f, 0);
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
        for (int i = 0; i<potList.list.Count; i++)
        {
            Transform currenPot = potList.list[i];
            PotState potState = currenPot.GetComponent<PotState>();

            if (potState.isPlanted == false && hasSeed == true || potState.isWatered == false && hasWater == true
                || potState.isWatered == true && potState.hasCrop==true)
            {
                return currenPot;
            }
        }
        return home;
    }
}
