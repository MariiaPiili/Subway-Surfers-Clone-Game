using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _changeRoadSpeed;

    private Rigidbody _rigidbody;
    private int _chosenRoad;
    private bool _jumpEnabled;
    private bool _isGrounded;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _chosenRoad = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _chosenRoad++;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _chosenRoad--;
        }

        MovementHorizontal();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpEnabled = true;
        }
    }

    private void FixedUpdate()
    {
        MovementVertical();
        Jump();
    }

    private void MovementVertical()
    {
        _rigidbody.velocity = new Vector3(_speed, _rigidbody.velocity.y, _rigidbody.velocity.z);
    }

    private void MovementHorizontal()
    {
        _chosenRoad = Mathf.Clamp(_chosenRoad, -1, 1);
        Vector3 position = transform.position;
        position.z = _chosenRoad;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * _changeRoadSpeed);
    }

    private void Jump()
    {
        if (_jumpEnabled)
        {
            _jumpEnabled = false;

            if (_isGrounded)
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        float dot = Vector3.Dot(normal, Vector3.up);
        if (dot > 0.5f)
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>())
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}
