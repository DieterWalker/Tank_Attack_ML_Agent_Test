using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events; 

public class TankMoveTraining : Agent{
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<float> OnMoveTurret = new UnityEvent<float>();
    
    public RayPerceptionSensorComponent2D rayPerceptionSensor;

    
    [SerializeField] private float checkRadius = 1f;
    [SerializeField] private int checkLocation = 1;
    
    // [SerializeField] private int reward = 0;
    [SerializeField] private bool aimTarget = false;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform alliesTransform;
    [SerializeField] private Transform moveTransform;
    [SerializeField] private int falsePoint = 250;

    private static float moveX = 8f;
    private static float moveY = 8f;
    public static string moveFlag = "Down";

    private static float moveAlliesX = -8f;
    private static float moveAlliesY = -8f;
    private static string moveAlliesFlag = "Up";

    private float startTime;
    // Khi một lần trainning bất đầu 
    public override void OnEpisodeBegin() {
        // falsePoint = 250;
        startTime = Time.time;
        // Debug.Log("Episode Begin");

        // transform.localPosition = new Vector3(0, 0, 0);
        // transform.rotation = Quaternion.identity;
    }

    // Hàm luôn kiểm tra va chạm của Collider
    public void OnTriggerStay2D(Collider2D collision){
         if (collision.TryGetComponent(out LimitZone limitZone)){
            Debug.Log("Your had run out!");
            AddReward(-1f/250);
            if (falsePoint > 0){
                falsePoint--;
            } else {
                Debug.Log("Failed!");
                // Gán vị trí xuất hiện ban đầu của xe tank
                EndEpisode();
            }
        } 
    }

    private void FixedUpdate() {
        AddReward(-1/MaxStep);
        // MoveTarget(targetTransform, alliesTransform); 
    }



    private void MoveTarget(Transform targetTransform, Transform alliesTransform) {
        float delay = 0.0125f;
        switch (moveFlag){
            case "Up":
                moveY = moveY + delay;
                if (moveY >= 8f) moveFlag = "Right";
                break;
            case "Down":
                moveY = moveY - delay;
                if (moveY <= -8f) {moveFlag = "Left";}
                break;
            case "Right":
                moveX = moveX + delay;
                if (moveX >= 8f) moveFlag = "Down";
                break;
            case "Left":
                moveX = moveX - delay;
                if (moveX <= -8f) moveFlag = "Up";
                break;
        }

        switch (moveAlliesFlag){
            case "Up":
                moveAlliesY = moveAlliesY + delay;
                if (moveAlliesY >= 8f) moveAlliesFlag = "Right";
                break;
            case "Down":
                moveAlliesY = moveAlliesY - delay;
                if (moveAlliesY <= -8f) {moveAlliesFlag = "Left";}
                break;
            case "Right":
                moveAlliesX = moveAlliesX + delay;
                if (moveAlliesX >= 8f) moveAlliesFlag = "Down";
                break;
            case "Left":
                moveAlliesX = moveAlliesX - delay;
                if (moveAlliesX <= -8f) moveAlliesFlag = "Up";
                break;
        }
        
        // Debug.Log("X: "+ moveX + " - Y:" + moveY);
        targetTransform.localPosition = new Vector3(moveX, moveY, 0);
        alliesTransform.localPosition = new Vector3(moveAlliesX, moveAlliesY, 0);
    }

    public override void CollectObservations(VectorSensor sensor){
        // Vị trí của Agent
        sensor.AddObservation(transform.localPosition);
        // Vị trí của Target
        if (moveTransform != null){
            sensor.AddObservation(moveTransform.localPosition);
        }
    }

    // Hàm thực hiện hành động
    public override void OnActionReceived(ActionBuffers actions){
        // Kiểm tra nồng súng đang bắn về đâu
        CheckShoot();
        // Kiểm tra xung quanh xe tank
        CheckTankMove();

        // Thực hiện bắn
        if (actions.DiscreteActions[0] > 0f){
            Shoot();
        }
        
        // Di chuyển lên xuống.
        float MoveVertical = actions.ContinuousActions[1];

        // Di chuyển trái phải
        float MoveHorizontal = actions.ContinuousActions[0];
        

        // Agent di chuyển
        Vector2 movementVector = new Vector2(MoveHorizontal , MoveVertical);
        MoveTank(movementVector);

        // Xoay xe tank
        if (actions.ContinuousActions[2] > 0f){
            RotateTurret(1);
        }
        
        if (actions.ContinuousActions[3] > 0f){
            RotateTurret(-1);
        }
    }

    // Khai báo các phím hành động cho Agent
    public override void Heuristic(in ActionBuffers actionsOut){
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = Input.GetKey(KeyCode.O) ? 1f : 0f;
        continuousActions[3] = Input.GetKey(KeyCode.P) ? 1f : 0f;
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
    }

    // Khi có Collider va chạm vào vật thể.
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out Bullet bullet)){
            // Debug.Log("Your has been shot!");
            AddReward(-0.1f);
        }
        // Tới mục tiêu yêu cầu
        if (collision.TryGetComponent(out MoveTarget moveTarget)){
            AddReward(+1f);
            EndEpisode();
        }
    }

    // Gọi sự kiện di chuyển
    private void MoveTank(Vector2 movement){
        OnMoveBody?.Invoke(movement.normalized);
    }
    
    // Gọi sự kiện xoay trụ bắn 
    private void RotateTurret(float rotation){
        OnMoveTurret?.Invoke(rotation);
    }

        // Gọi sự kiện bắn
    private void Shoot() {
        OnShoot?.Invoke();
    }


    // Hàm kiểm tra phía trước nòng súng là vật thể gì
    private void CheckShoot(){
        // Khai báo RayPerception trên nòng súng
        var rayOutputs = RayPerceptionSensor.Perceive(rayPerceptionSensor.GetRayPerceptionInput()).RayOutputs;
        // Lấy độ dài của RayPerceptionSensor
        int lengthOfRayOutputs = rayOutputs.Length;

        // Duyệt qua từng tia của RayPerceptionSensor
        for (int i = 0; i < lengthOfRayOutputs; i++){
            GameObject goHit = rayOutputs[i].HitGameObject;
            if (goHit != null){
                // Debug.Log("RaySesnsor has hit " + goHit.tag);
                // Nếu trước nòng súng là object có tag Target thì agent sẽ nhận được 1 phần thưởng nhỏ
                if (goHit.tag == "Target"){
                    aimTarget = true;
                    // Debug.Log("Aim Target");
                    AddReward(+1f/MaxStep);
                } else if (goHit.tag == "Enemy"){
                    // Nếu trước nòng súng là đồng minh hoặc các object khác thì agent sẽ bị phạt
                    aimTarget = false;
                    // Debug.Log("Aim Allies");
                    AddReward(-1f/MaxStep);
                } else {
                    aimTarget = false;
                    // Debug.Log("Not Target Aim");
                    AddReward(-0.1f/MaxStep);
                }
            }
        }
    }

    // Hàm kiểm tra xem xe tank đã bắn trúng đâu. 
    public void CheckTankShooter(GameObject hitBox){    
        string hitObjectTag = hitBox.tag;
        // Debug.Log("Check: "+ hitObjectTag);
        if (hitObjectTag == "Target") {
            float time = Time.time - startTime;
            Debug.Log("Your bullet had hit the player in " + time.ToString("F2") + " seconds");
            AddReward(+1f);
            EndEpisode();
        } else if (hitObjectTag == "Enemy"){
            AddReward(-1f);
            Debug.Log("Your Allies is Dead!");
            EndEpisode();
        } else {
            AddReward(-0.05f);
        }
    }

    private void CheckTankMove(){
        Vector2 tankPosition = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(tankPosition, checkRadius);
        foreach (Collider2D collider in colliders)
        {
            // Kiểm tra tag của collider
            if (collider.CompareTag("Block"))
            {
                Debug.Log("Near Brick");
                AddReward(-1f/MaxStep);
            }
            // } else if (collider.CompareTag("Enemy")){
            //     Debug.Log("Near Enemy");
            //     AddReward(-1f/MaxStep);
            // }
        }
    }

    private void SpawnTarget(Transform transform) {
        int check = checkLocation;
        while(check == checkLocation)//
        switch (Random.Range(1, 5)){
            case 1:
                transform.localPosition = new Vector3(-8, Random.Range(-8f, 9f), 0);
                checkLocation = 1;
                break;
            case 2:
                transform.localPosition = new Vector3(8, Random.Range(-8f, 9f), 0);
                checkLocation = 2;
                break;
            case 3:
                transform.localPosition = new Vector3(Random.Range(-8f, 9f), -8, 0);
                checkLocation = 3;
                break;
            case 4:
                transform.localPosition = new Vector3(Random.Range(-8f, 9f), 8, 0);
                checkLocation = 4;
                break;
        }
    }

    private void TrainingMoveUpDownSetUp(Transform transform){
        switch (checkLocation){
            case 0:
                transform.localPosition = new Vector3(0, -7, 0);
                checkLocation = 1;
                break;
            case 1:
                transform.localPosition = new Vector3(0, 7, 0);
                checkLocation = 0;
                break;
        }
    }

    private void TrainingMoveAroundMap(Transform transform) {
        switch (checkLocation){
            case 0:
                transform.localPosition = new Vector3(0, 10, 0);
                checkLocation = 1;
                break;
            case 1:
                transform.localPosition = new Vector3(-10, 0, 0);
                checkLocation = 2;
                break;
            case 2:
                transform.localPosition = new Vector3(0, -10, 0);
                checkLocation = 3;
                break;
            case 3:
                transform.localPosition = new Vector3(10, 0, 0);
                checkLocation = 0;
                break;
        }
    }

}