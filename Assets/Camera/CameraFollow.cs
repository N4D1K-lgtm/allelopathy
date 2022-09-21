using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _target;


    [SerializeField]
    [Range(0.01f, 1f)]
    private float _smoothTime = 0.125f;

    [SerializeField]
    private Vector3 _offset;
    private Vector3 _desiredPos;

    private Vector3 _velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 _desiredPosition = _target.position + _offset;

        // orthographic camera so we dont want camera position to change
        _desiredPosition.z = -10;
        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _velocity, _smoothTime);
    }
}