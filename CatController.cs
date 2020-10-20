using UnityEngine;

public class CatController : MonoBehaviour
{
    // Dependancies
    public GameObject menuHandler;
    public GameObject soundManager;

    // Adjustable attributes
    public float speed;

    // Components
    private Animator _animator;
    private SoundController _soundController;

    // Playspace
    private float height = 7.0f;
    private float width = 14.0f;

    // Movement
    private Vector3 _movePoint;
    private float _runToMovePointTimer = 4.0f;

    // Pathing
    private float _maxPathTimer = 0.1f;
    private float _pathTimer;
    private Vector3 _lastWaypoint;
    private int _currentPatrolSlot;
    private Vector3[] _waypoints;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _soundController = soundManager.GetComponent<SoundController>();
        _waypoints = new Vector3[4];
        _currentPatrolSlot = 1;
        MoveToRandomPoint();
    }

    private void FixedUpdate()
    {
        _runToMovePointTimer -= Time.deltaTime;
        _pathTimer -= Time.deltaTime;

        // Move towards initial patrol area
        if (_runToMovePointTimer > 0)
        {
            if (_movePoint != transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, _movePoint, speed * 2 * Time.deltaTime);
            }
        }

        // Normal pathing based on waypoints
        else
        {
            if (_pathTimer < 0)
            {
               
                if (_lastWaypoint != transform.position)
                { 
                    transform.position = Vector3.MoveTowards(transform.position, _waypoints[_currentPatrolSlot], speed * Time.deltaTime);
                }
                else
                {
                    _animator.SetFloat("PatrolSlot", _currentPatrolSlot + 0.5f);
                    _currentPatrolSlot += 1;

                    if (_currentPatrolSlot <= 3)
                    {
                        _lastWaypoint = _waypoints[_currentPatrolSlot];
                    }
                    else
                    {
                        _currentPatrolSlot = 0;
                        _lastWaypoint = _waypoints[_currentPatrolSlot];
                    }
                }
            }
            else
            {
                _pathTimer = _maxPathTimer;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProjectileController projectile = other.GetComponent<ProjectileController>();

        if (projectile != null)
        {
            _soundController.PlaySound("EnemyDeath");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        WolfController player = collision.gameObject.GetComponent<WolfController>();

        if (player != null)
        {
            menuHandler.GetComponent<MenuHandler>().ActivateLoseMenu();
        }

    }

    // Move to random point within play space boundaries and then create a square patrol
    // pattern for the gameObject to move along
    private void MoveToRandomPoint()
    {
        float x = Random.Range(-width, width);
        float y = Random.Range(-height, height);

        _movePoint = new Vector3(x, y, 0);

        _waypoints[0] = _movePoint;
        _waypoints[1] = new Vector3(_movePoint.x, _movePoint.y + 2, 0);
        _waypoints[2] = new Vector3(_movePoint.x - 2, _movePoint.y + 2, 0);
        _waypoints[3] = new Vector3(_movePoint.x - 2, _movePoint.y, 0);
        _lastWaypoint = _waypoints[_currentPatrolSlot];
    }
}
