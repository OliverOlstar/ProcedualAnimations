using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereGizmo : MonoBehaviour
{
    [SerializeField] private Color _color;
    private SphereCollider _collider;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawWireSphere(transform.position, _collider.radius);
    }
}
