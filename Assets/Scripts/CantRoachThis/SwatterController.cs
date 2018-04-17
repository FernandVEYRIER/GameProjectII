using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    public class SwatterController : NetworkBehaviour
    {
        [SerializeField] private GameObject _leftLimit;
        [SerializeField] private GameObject _rightLimit;

        [SerializeField] private float _maxDelay = 4;
        [SerializeField] private float _minDelay = 1;

        [SerializeField] private int _maxSwatters = 3;

        private float _currentDelay;

        private void Start()
        {
            if (isServer)
            {
                _currentDelay = _maxDelay;
            }
        }

        private void Update()
        {
            UpdateSwatterActions();
        }

        [Server]
        private void UpdateSwatterActions()
        {
            _currentDelay -= Time.deltaTime;

            if (_currentDelay <= 0)
            {
                _currentDelay = _maxDelay;
                if (_maxDelay > _minDelay)
                    _maxDelay -= 0.3f;
                if (_maxDelay < _minDelay)
                    _maxDelay = _minDelay;
                Attack();
            }
        }

        private void Attack()
        {
            throw new NotImplementedException();
        }
    }
}