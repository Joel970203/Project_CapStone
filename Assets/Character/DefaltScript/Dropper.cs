using UnityEngine;

public class Dropper : MonoBehaviour
{
    private MeshRenderer renderer;
    private Rigidbody rb;
    private bool hasCollided = false;

    [SerializeField] private float timeToWait = 5f;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        SetObjectInvisible();
    }

    void Update()
    {
        if (Time.time > timeToWait && !hasCollided)
        {
            rb.useGravity = true;
            SetObjectVisible();
        }
    }

    void SetObjectInvisible()
    {
        renderer.enabled = false;
    }

    void SetObjectVisible()
    {
        renderer.enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided && collision.collider.CompareTag("Terrain"))
        {
            hasCollided = true;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        else if (!hasCollided && collision.collider.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
                playerRb.isKinematic = true;
                Debug.Log("Player stopped!");
            }
            hasCollided = true;
        }
    }
}
