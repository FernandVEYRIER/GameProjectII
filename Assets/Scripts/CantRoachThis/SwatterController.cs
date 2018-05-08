using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    public class SwatterController : NetworkBehaviour
    {
        [SerializeField] private GameObject _swatterParent;
        [SerializeField] private Animator _animator;

        [SerializeField] private GameObject _leftLimit;
        [SerializeField] private GameObject _rightLimit;

        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        [SerializeField] private float _maxDelay = 4;
        [SerializeField] private float _minDelay = 1;

        [SerializeField] private SwatterCollider _collider;

        private float _currentDelay;
        private bool _isAttacking;

        private GameManager _manager;

        private void Start()
        {
            if (isServer)
            {
                _currentDelay = _maxDelay;
                _collider.OnTriggerEntered += Collider_OnTriggerEntered;
            }
            _manager = AGameManager.Instance as GameManager;
            _leftLimit = GameObject.Find("SpawnLeft");
            _rightLimit = GameObject.Find("SpawnRight");
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _collider.OnTriggerEntered -= Collider_OnTriggerEntered;
        }

        private void Collider_OnTriggerEntered(object caller, Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                _manager.KillPlayer(other.gameObject.GetComponent<APlayerController>());
            }
        }

        private void Update()
        {
            if (_manager.GameState == GAME_STATE.Play && isServer)
                UpdateSwatterActions();
        }

        [Server]
        private void UpdateSwatterActions()
        {
            _currentDelay -= Time.deltaTime;

            if (_currentDelay <= 0 && !_isAttacking)
            {
                _currentDelay = _maxDelay;
                if (_maxDelay > _minDelay)
                    _maxDelay -= 0.3f;
                if (_maxDelay < _minDelay)
                    _maxDelay = _minDelay;
                StartAttack();
            }
            else if (_isAttacking)
            {
                UpdateSwatterState();
            }
        }

        [Server]
        private void UpdateSwatterState()
        {
            _swatterParent.transform.position = Vector3.MoveTowards(_swatterParent.transform.position, _endPoint.position, 0.9f);
            if (Vector3.Distance(_swatterParent.transform.position, _endPoint.position) == 0)
            {
                _animator.SetTrigger("Hit");
                _isAttacking = false;
            }
        }

        [Server]
        private void StartAttack()
        {
            if (_isAttacking)
                return;
            _isAttacking = true;
            transform.position = new Vector3(Random.Range(_leftLimit.transform.position.x, _rightLimit.transform.position.x), transform.position.y, transform.position.z);
            _swatterParent.transform.position = _startPoint.position;
        }

        [Server]
        public void Activate(bool active)
        {
            gameObject.SetActive(active);
            RpcActivate(active);
        }

        [ClientRpc]
        private void RpcActivate(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}