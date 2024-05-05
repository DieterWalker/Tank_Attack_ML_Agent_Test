using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class MoveToTargetAgent : Agent{ 
    // public UnityEvent OnShoot = new UnityEvent();
    // public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    // public UnityEvent<Vector2> OnMoveTurret = new UnityEvent<Vector2>();

    [SerializeField] private Transform targetTransform;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

    public override void OnEpisodeBegin() {
        transform.localPosition = new Vector3(Random.Range(-5f, 2f), Random.Range(-9f, -2f));
        targetTransform.localPosition = new Vector3(Random.Range(-5f, 2f), Random.Range(-9f, -2f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition); 
        sensor.AddObservation((Vector2)targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions){
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float movementSpeed = 5f; 

        transform.localPosition += new Vector3(moveX, moveY, 0) * Time.deltaTime * movementSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("OnTriggerEnter2D");
        if (collision.TryGetComponent(out Target target)) {
           Debug.Log("+1");
           SetReward(+1f);
           backgroundSpriteRenderer.color = Color.green;
           EndEpisode();
        }

        if (collision.TryGetComponent(out LimitZone limitZone)){
           Debug.Log("Fail");
           SetReward(-1f);
           backgroundSpriteRenderer.color = Color.red;
           EndEpisode();
        }

        if (collision.TryGetComponent(out Bullet bullet)){
            Debug.Log("Hit");
            SetReward(-1f);
            backgroundSpriteRenderer.color = Color.red;
            EndEpisode();
        }
    }

}
