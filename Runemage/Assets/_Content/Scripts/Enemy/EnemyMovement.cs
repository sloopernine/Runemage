using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] float initialSpeed;
    public float InitialSpeed { get => initialSpeed; }
    private float currentSpeed;
    public float CurrentSpeed { get => currentSpeed;}


    [SerializeField] float turnPower = 0.05f;

    private Rigidbody rigidbody;
    
    [SerializeField] float minDistanceToPlayer = 4f;

    [Range(0.1f, 1f)]
    [SerializeField] float closeEnoughToTarget = 0.1f;

    public bool useMovement;

    private Vector3 finalTarget;
    private Vector3 targetMovePosition;
    [SerializeField] int currentMoveCommand;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        
    }

    private void OnEnable()
    {
        //vector.zero acts as our final target for now

        finalTarget = Vector3.zero;
        currentSpeed = initialSpeed;
        currentMoveCommand = 0;
        useMovement = true;
        ChooseMoveCommand();

    }

    public void SetCurrentSpeed(float amount)
    {
        currentSpeed = amount;
    }

    private void ChooseMoveCommand()
    {
        switch (currentMoveCommand)
        {
            case 0:
                targetMovePosition = GetTargetPositionForward(finalTarget, transform.position, minDistanceToPlayer * 6);
                break;
            case 1:
                StartCoroutine(PauseMovement(2f, finalTarget, transform.position));
                break;
            case 2:
                targetMovePosition = GetTargetPositionWithAngle(finalTarget, transform.position, minDistanceToPlayer * 4, 30f);
                break;

            case 3:
                targetMovePosition = GetTargetPositionWithAngle(finalTarget, transform.position, minDistanceToPlayer * 2, -50f);
                break;

            case 4:
                StartCoroutine(PauseMovement(2f, finalTarget, transform.position));
                break;

            case 5:
                targetMovePosition = GetTargetPositionForward(finalTarget, transform.position, minDistanceToPlayer);
                break;

            default:
                print($"{transform.name} is out of movementCommands. Reseting");
                currentMoveCommand = 0;
                break;
        }
    }

    IEnumerator PauseMovement(float pauseTime, Vector3 finalTarget, Vector3 currentPosition)
    {
        useMovement = false;
        Vector3 directionToTarget = (finalTarget - currentPosition).normalized;
        RotateTowardsDirection(directionToTarget);
        
        for (float f = 0; f < pauseTime; f += Time.fixedDeltaTime)
        {
            RotateTowardsDirection(directionToTarget);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
           
        }
        useMovement = true;

    }

    public Vector3 GetTargetPositionForward(Vector3 finalTarget, Vector3 currentPosition, float targetDistanceToPlayer)
    {
        Vector3 directionToTarget = (currentPosition - finalTarget).normalized;
        Vector3 targetMovePosition = finalTarget + directionToTarget * targetDistanceToPlayer;
        return targetMovePosition;
    }

    public Vector3 GetTargetPositionWithAngle(Vector3 finalTarget, Vector3 currentPosition, float targetDistanceToPlayer, float degreeAngle)
    {
        Vector3 directionToTarget = (currentPosition - finalTarget).normalized;
        directionToTarget = Quaternion.AngleAxis(degreeAngle, Vector3.up) * directionToTarget;
        Vector3 targetMovePosition = finalTarget + directionToTarget * targetDistanceToPlayer;
        return targetMovePosition;
    }

    private void FixedUpdate()
    {
        if (!useMovement)
        {
            return;
        }

        Debug.DrawLine(transform.position, targetMovePosition, Color.red);

        //check if close to player
        if (DistanceTowardsPoint(finalTarget) <= minDistanceToPlayer)
        {
            //print("Arrived at Player");
            return;
        }

        if (DistanceTowardsPoint(targetMovePosition) < closeEnoughToTarget)
        {
            //print($"Destination reached for {transform.name}");
            currentMoveCommand++;
            ChooseMoveCommand();
            return;
        }

        MoveTowardsPoint(targetMovePosition);

    }

    public void MoveTowardsPoint(Vector3 point)
    { 
        Vector3 direction = point - transform.position;
        direction = direction.normalized;

        RotateTowardsDirection(direction);
        rigidbody.MovePosition(transform.position + direction * Time.deltaTime * CurrentSpeed);
    
    }

    public void RotateTowardsDirection(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion deltaRotation = Quaternion.Slerp(transform.rotation, targetRotation, turnPower);
        rigidbody.MoveRotation(deltaRotation);
    }

    public float DistanceTowardsPoint(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        float distance = direction.magnitude;

        return distance;
    }



}
