using System;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    /// <summary>
    /// Player controller for the Cockroach.
    /// </summary>
    public class PlayerController : APlayerController
    {
        [SerializeField] private GameObject bloodSplashPrefab;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private GameObject flagColor;
        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;
        private CharacterController _controller;

        private Transform _leftLimit;
        private Transform _rightLimit;

        /// <summary>
        /// Called when the local player is ready.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            Debug.Log("Starting local player " + _playerColor + "   " + _playerName);
            base.OnStartLocalPlayer();
            SetupPlayer();
        }

        /// <summary>
        /// Sets up the player values.
        /// </summary>
        [Client]
        private void SetupPlayer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            _controller = GetComponent<CharacterController>();

            var gm = (AGameManager.Instance as GameManager);
            _leftLimit = gm.LeftTerrainLimit;
            _rightLimit = gm.RightTerrainLimit;

            //CmdSetPlayerInfo(LobbyManager.Instance.GetLocalPlayerInfo());
        }

        public override void OnStartServer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            _controller = GetComponent<CharacterController>();
            base.OnStartServer();
        }

        private void Update()
        {
            flagColor.GetComponent<MeshRenderer>().materials[1].color = _playerColor;

            if (isLocalPlayer)
            {
#if UNITY_EDITOR && !UNITY_STANDALONE
                var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
#else
                var dir = (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width / 2f) ? -1
                    : (Input.GetMouseButton(0) && Input.mousePosition.x >= Screen.width / 2f) ? 1 : 0;
                var moveDirection = new Vector3(dir, 0, 0);
#endif
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                transform.Translate(moveDirection /** Time.deltaTime*/);
                if (moveDirection != Vector3.zero)
                    visualTransform.localRotation = Quaternion.Euler(0, Mathf.Sign(moveDirection.x) * -90, 0);
                if (transform.position.x < _leftLimit.position.x)
                    transform.position = _leftLimit.position;
                else if (transform.position.x > _rightLimit.position.x)
                    transform.position = _rightLimit.position;
            }
        }

        public override void OnNetworkDestroy()
        {
            if (isLocalPlayer && Camera.main != null)
                Camera.main.GetComponent<Animator>().SetTrigger("Shake");
            SpawnBloodSplash();
            base.OnNetworkDestroy();
        }

        private void SpawnBloodSplash()
        {
            var particles = new ParticleSystem.Particle[1];
            var go = Instantiate(bloodSplashPrefab, transform.position, Quaternion.Euler(Vector3.up));
            var p = go.GetComponent<ParticleSystem>();
            particles[0].position = visualTransform.GetChild(0).position;
            particles[0].rotation3D = new Vector3(90, 0, 0);
            particles[0].startSize = 6;
            particles[0].color = Color.white;
            p.SetParticles(particles, 1);
        }
    }
}