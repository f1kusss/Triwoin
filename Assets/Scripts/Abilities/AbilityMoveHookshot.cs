using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag
{
    public class AbilityMoveHookshot : NetworkBehaviour
    {
        [field: SerializeField] public bool CanFire { get; private set; }

        [SerializeField] private GameObject _hookshotPrefab;

        [SerializeField] private CharacterMovement _movement;

        public float MaxDistanceToTarget = 100f;
        [SerializeField] private float _zippingSpeed = 5f;
        [SerializeField] private Vector2 _minMaxSpeed = new Vector2(3f, 30f);

        [SerializeField] private LayerMask _layerMask = new LayerMask();

        [Header("Debugging")] public bool Debugging;

        [SerializeField] private bool _firing;

        [SerializeField] private Vector3 _hookTargetPosition;

        [SerializeField] private LineRenderer _lineRendCircle;
        [SerializeField] private LineRenderer _lineRendShot;

        [SerializeField] private Vector3 _momentumAfterCancelHookshot;
        [SerializeField] private Vector3 _momentumAfterPerformedHookshot;


        // Start is called before the first frame update
        void Start()
        {
            _movement.StartPerformingHookshot += StartVisualizeHookshotServerRpc;
            _movement.StopPerformingHookshot += StopVisualizeHookshotServerRpc;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _movement.StartPerformingHookshot -= StartVisualizeHookshotServerRpc;
            _movement.StopPerformingHookshot -= StopVisualizeHookshotServerRpc;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsOwner)
            {
                if (Input.GetKeyDown(KeyCode.F) && CanFire)
                {
                    _movement.SwitchHookshot();

                    // _hookTargetPosition = FireWeapons();
                }
            }
        }

        public Vector3 FireHookshot()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

            if (Physics.Raycast(ray, out hit, MaxDistanceToTarget, _layerMask))
            {
                if (Debugging)
                {
                    Debug.Log(
                        $"FireWeapons. Object: {hit.transform.gameObject.name} ray.origin: {ray.origin}, hit.point: {hit.point}");
                    //Debug.DrawRay(ray.origin, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 3.0f);
                    //Instantiate(_testPrefab, hit.point, Quaternion.identity);
                }

                var hookDir = (hit.point - transform.position).normalized;
                _momentumAfterPerformedHookshot = hookDir * 1.5f;
                _momentumAfterPerformedHookshot.y = Mathf.Clamp(_momentumAfterPerformedHookshot.y + .5f, .1f, .8f);
                return hit.point;
            }
            else
            {
                // _firing = false;
                return Vector3.zero;
            }
        }

        public Vector3 Hookshot(Vector3 position)
        {
            // Debug.Log($"{Vector3.Distance(position, transform.position)}");
            VisualizePerformingHookshotClientRpc();
            if (Vector3.Distance(position, transform.position) < 1.5f)
            {
                // _hookTargetPosition = Vector3.zero;
                StopVisualizeHookshotServerRpc();
                return Vector3.zero;
            }

            Vector3 dir = (position - transform.position).normalized;
            dir.y += _momentumAfterPerformedHookshot.y / 2;
            float speed = Mathf.Clamp(Vector3.Distance(transform.position, position) * _zippingSpeed, _minMaxSpeed.x,
                _minMaxSpeed.y * Time.deltaTime);
            return dir * speed;
        }

        public Vector3 CalculateMomentum(bool canceled)
        {
            if (canceled)
            {
                _momentumAfterCancelHookshot = Camera.main
                    .ScreenPointToRay(new Vector2(Screen.width * 0.5f,
                        Screen.height * 0.5f)).direction.normalized * 2f;
                return _momentumAfterCancelHookshot;
            }
            else
            {
                return _momentumAfterPerformedHookshot;
            }
        }

        [ServerRpc]
        private void StartVisualizeHookshotServerRpc(Vector3 position)
        {
            var hookshotNetObj = Instantiate(_hookshotPrefab, position, Quaternion.identity)
                .GetComponent<NetworkObject>();
            hookshotNetObj.Spawn();
            StartVisualizeHookshotClientRpc(position);
        }

        [ServerRpc]
        private void StopVisualizeHookshotServerRpc()
        {
            StopVisualizeHookshotClientRpc();
        }

        [ClientRpc]
        private void StartVisualizeHookshotClientRpc(Vector3 position)
        {
            _lineRendCircle.enabled = true;
            _lineRendShot.enabled = true;
            _lineRendShot.SetPosition(0, transform.position);
            _lineRendShot.SetPosition(1, position);
        }

        [ClientRpc]
        private void StopVisualizeHookshotClientRpc()
        {
            _lineRendCircle.enabled = false;
            _lineRendShot.enabled = false;
        }

        [ServerRpc]
        private void VisualizePerformingHookshotServerRpc()
        {
            VisualizePerformingHookshotClientRpc();
        }

        [ClientRpc]
        private void VisualizePerformingHookshotClientRpc()
        {
            _lineRendShot.SetPosition(0, transform.position);
        }
    }
}