using LearnGame.Movement;
using LearnGame.PickUp;
using LearnGame.Shooting;
using UnityEngine;

namespace LearnGame
{
    [RequireComponent (typeof(CharahterMovementController), typeof(ShootingController))]

    public abstract class BaseCharacter : MonoBehaviour
    {
        [SerializeField]
        private Weapon _weaponBasePrefab;

        [SerializeField]
        private Transform _hand;

        [SerializeField]
        private float _health = 2f;

        private IMovementDirectionSource _movementDirectionSource;

        private CharahterMovementController _characterMovementController;
        private ShootingController _shootingController;

        protected void Awake()
        {
            _movementDirectionSource = GetComponent<IMovementDirectionSource>();

            _characterMovementController = GetComponent<CharahterMovementController>();
            _shootingController = GetComponent<ShootingController>();
        }

        protected void Start()
        {
            SetWeapon(_weaponBasePrefab);
        }

        protected void Update()
        {
            var direction = _movementDirectionSource.MovementDirection;
            var lookDirection = direction;
            if (_shootingController.HasTarget)
                lookDirection = (_shootingController.TargetPosition - transform.position).normalized;

            _characterMovementController.MovementDirection = direction;
            _characterMovementController.LookDirection = lookDirection;

            if (_health <= 0f)
            {
                Destroy(gameObject);
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (LayerUtils.IsBullet(other.gameObject))
            {
                var bullet = other.gameObject.GetComponent<Bullet>();

                _health -= bullet.Damage;

                Destroy(other.gameObject);
            }
            else if (LayerUtils.IsPickUp(other.gameObject))
            {
                var pickUp = other.gameObject.GetComponent<PickUpWeapon>();
                pickUp.PickUp(this);

                Destroy(other.gameObject);
            }

        }

        public void SetWeapon(Weapon weapon)
        {
            _shootingController.SetWepon(weapon, _hand);
        }
    }
}
