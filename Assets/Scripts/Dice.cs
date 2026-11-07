using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float minRollForce = 5f;
    [SerializeField] private float maxRollForce = 10f;
    [SerializeField] private float minTorqueForce = 5f;
    [SerializeField] private float maxTorqueForce = 5f;
    private int result;
    public int Result { get { return result; } }
    bool isRolling = false;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Wait for the linear and angular velocities to die down before checking the result
        if (isRolling && rb.linearVelocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
        {
            isRolling = false;
            CheckResult();
        }
    }
    public void Roll()
    {
        float rollForce = Random.Range(minRollForce, maxRollForce);
        float torqueForce = Random.Range(minTorqueForce, maxTorqueForce);
        rb.AddForce(Vector3.up * rollForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * torqueForce, ForceMode.Impulse);

        // Wait a short time before setting the isRolling flag to allow the dice to start moving
        Invoke("StartRolling", .1f);
    }

    private void StartRolling()
    {
        isRolling = true;
    }

    private void CheckResult()
    {
        // Get the upwards facing side of the dice by performing dot products
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;

        if (forward.y > 0.5f)
        {
            result = 1;
        }
        else if (forward.y < -0.5f)
        {
            result = 6;
        }
        else if (up.x > 0.5f)
        {
            result = 2;
        }
        else if (up.x < -0.5f)
        {
            result = 5;
        }
        else if (forward.z > 0.5f)
        {
            result = 3;
        }
        else
        {
            result = 4;
        }
        Debug.Log(result);
    }
}
