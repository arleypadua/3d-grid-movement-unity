using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private const string WalkingAnimation = "Male_Walk";
    public const string SwordIdleAnimation = "Male Sword Stance";

    private static KeyCode[] _movingKeys = new[] {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W};

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    [SerializeField]
    float moveSpeed = 0.25f;
    [SerializeField]
    float rayLength = 1.4f;
    [SerializeField]
    float rayOffsetX = 0.5f;
    [SerializeField]
    float rayOffsetY = 0.5f;
    [SerializeField]
    float rayOffsetZ = 0.5f;

    Vector3 _targetPosition;
    Vector3 _startPosition;
    bool _moving;

    Vector3 _xOffset;
    Vector3 _yOffset;
    Vector3 _zOffset;
    Vector3 _zAxisOriginA;
    Vector3 _zAxisOriginB;
    Vector3 _xAxisOriginA;
    Vector3 _xAxisOriginB;

    void Update()
    {
        if (_moving)
        {
            if (Vector3.Distance(_startPosition, transform.position) > 1f)
            {
                transform.position = _targetPosition;
                SetNotMoving();
                StopAnimatingMovement();
                return;
            }

            transform.position += (_targetPosition - _startPosition) * moveSpeed * Time.deltaTime;
            return;
        }

        // Set the ray positions every frame

        _yOffset = transform.position + Vector3.up * rayOffsetY;
        _zOffset = Vector3.forward * rayOffsetZ;
        _xOffset = Vector3.right * rayOffsetX;

        _zAxisOriginA = _yOffset + _xOffset;
        _zAxisOriginB = _yOffset - _xOffset;

        _xAxisOriginA = _yOffset + _zOffset;
        _xAxisOriginB = _yOffset - _zOffset;

        // Handle player input

        if (Input.GetKey(KeyCode.W))
        {
            Move(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Move(Vector3.back);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Move(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector3.right);
        }
    }

    private void Move(Vector3 to)
    {
        if (CanMove(to))
        {
            _targetPosition = transform.position + to;
            _startPosition = transform.position;
            SetMoving();
            AnimateMovement(to);
        }
    }

    private void AnimateMovement(Vector3 to)
    {
        if (anim != null)
        {
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName(WalkingAnimation))
                anim.Play(WalkingAnimation);

            if (to == Vector3.forward)
            {
                transform.LookAt(new Vector3(0, 0, 90));
            }

            if (to == Vector3.left)
            {
                transform.LookAt(new Vector3(-90, 0, 0));
            }

            if (to == Vector3.right)
            {
                transform.LookAt(new Vector3(90, 0, 0));
            }

            if (to == Vector3.back)
            {
                transform.LookAt(new Vector3(0, 0, -90));
            }
        }
    }

    private void StopAnimatingMovement()
    {
        if (anim != null && !_movingKeys.Any(Input.GetKey))
        {
            anim.Play(SwordIdleAnimation);
        }
    }

    private void SetMoving()
    {
        _moving = true;
    }

    private void SetNotMoving()
    {
        _moving = false;
    }

    bool CanMove(Vector3 direction)
    {
        if (_moving) return false;

        if (direction.z != 0)
        {
            if (Physics.Raycast(_zAxisOriginA, direction, rayLength)) return false;
            if (Physics.Raycast(_zAxisOriginB, direction, rayLength)) return false;
        }
        else if (direction.x != 0)
        {
            if (Physics.Raycast(_xAxisOriginA, direction, rayLength)) return false;
            if (Physics.Raycast(_xAxisOriginB, direction, rayLength)) return false;
        }
        return true;
    }
}
