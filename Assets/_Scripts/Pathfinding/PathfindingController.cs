using UnityEngine;


class PathfindingController : MonoBehaviour
{
    private Transform _transform;

    [SerializeField] private float SeekInterval = .5f;
    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        Invoke()
    }

    private void Update()
    {
        
    }
}