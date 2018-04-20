﻿using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    public class SwatterController : MonoBehaviour
    {
        [SerializeField] private GameObject _swatterParent;
        [SerializeField] private Animator _animator;

        [SerializeField] private GameObject _leftLimit;
        [SerializeField] private GameObject _rightLimit;

        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        [SerializeField] private float _maxDelay = 4;
        [SerializeField] private float _minDelay = 1;

        [SerializeField] private int _maxSwatters = 3;

        private float _currentDelay;
        private bool _isAttacking;

        private void Start()
        {
            //if (isServer)
            //{
                _currentDelay = _maxDelay;
            //}
        }

        private void Update()
        {
            UpdateSwatterActions();
        }

        //[Server]
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
                Attack();
            }
            else if (_isAttacking)
            {
                UpdateSwatterState();
            }
        }

        private void UpdateSwatterState()
        {
            _swatterParent.transform.position = Vector3.MoveTowards(_swatterParent.transform.position, _endPoint.position, 0.9f);
            if (Vector3.Distance(_swatterParent.transform.position, _endPoint.position) == 0)
            {
                _animator.SetTrigger("Hit");
                _isAttacking = false;
            }
        }

        private void Attack()
        {
            if (_isAttacking)
                return;
            _isAttacking = true;
            _swatterParent.transform.position = _startPoint.position;
        }
    }
}