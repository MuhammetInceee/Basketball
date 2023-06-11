using System;
using UnityEngine;
public class Shooter : MonoBehaviour
{
    public Camera cameraForShooter;
    public GameObject ballPrefab;
    public Transform shotPoint;

    public float targetZ = 12.0f;
    public float shotPowerMin = 3.0f;
    public float shotPowerMax = 12.0f;
    public float offsetY = 100.0f;
    public float shotTimeMin = 0.2f;
    public float shotTimeMax = 0.55f;
    public float torque = 30.0f;

    public float offsetZShotPoint = 1.0f;
    public float powerToRoll = 2.0f;
    public float timeoutForShot = 5.0f;

    public float ShotPower { get; private set; }
    public Vector3 Direction { get; private set; }


    GameObject _objBall;
    Rigidbody _ballRigidbody;
    float _startTime;

    Vector2 _touchPos;

    enum ShotState
    {
        Charging, 
        Ready,
        DirectionAndPower
    }
    
    ShotState _state = ShotState.Charging;
    
    private void Start()
    {
        _touchPos.x = -1.0f;
    }


    private void Update()
    {
        if(GameManager.Instance.gameState != GameState.Game) return;

        switch (_state)
        {
            case ShotState.Charging:
                ChargeBall();
                CheckTrigger();
                break;
            case ShotState.Ready:
                CheckTrigger();
                break;
            case ShotState.DirectionAndPower:
                CheckShot();
                break;
        }
    }
    
    private void FixedUpdate()
    {
        if (_state == ShotState.Charging) return;
        _ballRigidbody.velocity = Vector3.zero;
        _ballRigidbody.angularVelocity = Vector3.zero;
    }
    
    private void ChargeBall()
    {
        if (_objBall == null)
        {
            _objBall = Instantiate(ballPrefab);
            _objBall.AddComponent<ShotBall>();
            _ballRigidbody = _objBall.GetComponent<Rigidbody>();

            var shotPos = shotPoint.transform.localPosition;
            shotPos.z -= offsetZShotPoint;
            Transform transform1;
            _objBall.transform.position = (transform1 = shotPoint.transform).TransformPoint(shotPos);
            _objBall.transform.eulerAngles = transform1.eulerAngles;
            _ballRigidbody.velocity = Vector3.zero;

            _ballRigidbody.AddForce(shotPoint.transform.TransformDirection(new Vector3(0.0f, 0.0f, powerToRoll)),
                ForceMode.Impulse);
        }
        
        var dis = Vector3.Distance(shotPoint.transform.position, _objBall.transform.position);
        
        if (!(dis <= 0.2f)) return;
        _state = ShotState.Ready;
        _objBall.transform.position = shotPoint.transform.position;
    }


    private void CheckTrigger()
    {
        if (_touchPos.x < 0)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = cameraForShooter.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, 100)) return;
            var sb = hit.collider.transform.GetComponent<ShotBall>();
            
            if (sb == null || sb.isActive) return;
            sb.ChangeActive();
            _touchPos = Input.mousePosition;
            ShotPower = 0.0f;
        }
        else
        {
            if (Math.Abs(_touchPos.x - Input.mousePosition.x) < 0.01f && Math.Abs(_touchPos.y - Input.mousePosition.y) < 0.01f) return;
            
            _touchPos.x = -1.0f;
            _startTime = Time.time;
            _state = ShotState.DirectionAndPower;
        }
    }


    private void CheckShot()
    {
        var elapseTime = Time.time - _startTime;

        if (Input.GetMouseButtonUp(0))
        {
            if (_objBall != null)
            {
                ShootBall(elapseTime);
            }

            _state = ShotState.Charging;
            _objBall = null;
        }

        if (!(timeoutForShot < elapseTime)) return;
        Destroy(_objBall);
        _state = ShotState.Charging;
        _objBall = null;
    }


    private void ShootBall(float elapseTime)
    {
        if (elapseTime < shotTimeMin)
        {
            ShotPower = shotPowerMax;
        }
        else if (shotTimeMax < elapseTime)
        {
            ShotPower = shotPowerMin;
        }
        else
        {
            var tMin100 = shotTimeMin * 10000.0f;
            var tMax100 = shotTimeMax * 10000.0f;
            var ep100 = elapseTime * 10000.0f;
            var rate = (ep100 - tMin100) / (tMax100 - tMin100);
            ShotPower = shotPowerMax - ((shotPowerMax - shotPowerMin) * rate);
        }

        var screenPoint = Input.mousePosition;
        screenPoint.z = targetZ;
        var worldPoint = cameraForShooter.ScreenToWorldPoint(screenPoint);

        worldPoint.y += (offsetY / ShotPower);

        var transform1 = shotPoint.transform;
        Direction = (worldPoint - transform1.position).normalized;

        _ballRigidbody.velocity = Direction * ShotPower;
        _ballRigidbody.AddTorque(-transform1.right * torque);
    }
}