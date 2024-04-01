using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MiniHelper : Agent
{
    [SerializeField] private Transform seedBox;
    [SerializeField] private Transform targetPot;
    [SerializeField] private Transform waterBox;
    [SerializeField] private Transform goalBox;

    private Transform currentPot;
    private bool isPlanting = false;
    private bool isWatering = false;
    private float timeSincePlanted = 0f;
    private float growTime = 10f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
    }

    //Provide the Observations for the Agent
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetPot.localPosition);
    }

    //Provide controls to the Agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isPlanting && !isWatering)
        {

        }
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 10f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions= actionsOut.ContinuousActions;
        continuousActions[0] = -Input.GetAxisRaw("Horizontal");
        continuousActions[1] = -Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}
