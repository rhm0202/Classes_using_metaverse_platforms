using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // 플레이어

    [Header("Position")]
    [SerializeField] private float distance = 8f;      // 플레이어와의 거리
    [SerializeField] private float height = 5f;        // 카메라 높이
    [SerializeField] private bool lockY = true;        // 점프해도 카메라 Y 고정할지
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Orbit")]
    [SerializeField] private float directionFollowSpeed = 5f;  // 플레이어 진행 방향을 따라가는 속도

    private Vector3 _velocity;
    private float _fixedY;
    private float _currentAngle; // Y축 기준 현재 궤도 각도

    private void Start()
    {
        if (target != null)
        {
            // 초기 각도/높이 세팅
            Vector3 toCam = transform.position - target.position;
            toCam.y = 0f;
            if (toCam.sqrMagnitude > 0.0001f)
            {
                _currentAngle = Mathf.Atan2(toCam.x, toCam.z) * Mathf.Rad2Deg;
            }
            else
            {
                _currentAngle = target.eulerAngles.y;
            }

            if (lockY)
            {
                _fixedY = transform.position.y;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 플레이어가 바라보는/굴러가는 방향을 기준으로 각도 계산
        Vector3 forward = target.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude > 0.0001f)
        {
            float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            _currentAngle = Mathf.LerpAngle(_currentAngle, targetAngle, directionFollowSpeed * Time.deltaTime);
        }

        // 각도/거리/높이로 원하는 카메라 위치 계산
        Quaternion rot = Quaternion.Euler(0f, _currentAngle, 0f);
        Vector3 offset = rot * new Vector3(0f, 0f, -distance);
        Vector3 desired = target.position + offset;
        desired.y = lockY ? _fixedY : target.position.y + height;

        if (lockY)
        {
            desired.y = _fixedY;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

        // 항상 플레이어를 바라보게
        transform.LookAt(target.position);
    }
}

