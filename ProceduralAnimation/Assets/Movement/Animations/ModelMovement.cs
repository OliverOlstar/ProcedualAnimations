using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _rotationDampening = 1;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (horizontalVelocity.magnitude > 0)
            transform.forward = Vector3.Lerp(transform.forward, horizontalVelocity.normalized, Time.deltaTime * _rotationDampening);
    }
}
