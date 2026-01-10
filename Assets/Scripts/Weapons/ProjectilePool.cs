using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace LooterShooter.Weapons
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }

        [SerializeField] private int defaultPoolSize = 10;
        [SerializeField] private int maxPoolSize = 50;

        private Dictionary<GameObject, ObjectPool<Projectile>> _pools = new();
        private Dictionary<Projectile, GameObject> _projectileToPrefab = new();
        private Transform _poolContainer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _poolContainer = new GameObject("PooledProjectiles").transform;
            _poolContainer.SetParent(transform);
        }

        public Projectile Get(GameObject prefab)
        {
            if (prefab == null) return null;

            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab);
                _pools[prefab] = pool;
            }

            var projectile = pool.Get();
            _projectileToPrefab[projectile] = prefab;
            return projectile;
        }

        public void Release(Projectile projectile)
        {
            if (projectile == null) return;

            if (_projectileToPrefab.TryGetValue(projectile, out var prefab))
            {
                if (_pools.TryGetValue(prefab, out var pool))
                {
                    pool.Release(projectile);
                    return;
                }
            }

            Destroy(projectile.gameObject);
        }

        private ObjectPool<Projectile> CreatePool(GameObject prefab)
        {
            return new ObjectPool<Projectile>(
                createFunc: () => CreateProjectile(prefab),
                actionOnGet: OnGetProjectile,
                actionOnRelease: OnReleaseProjectile,
                actionOnDestroy: OnDestroyProjectile,
                collectionCheck: true,
                defaultCapacity: defaultPoolSize,
                maxSize: maxPoolSize
            );
        }

        private Projectile CreateProjectile(GameObject prefab)
        {
            var instance = Instantiate(prefab, _poolContainer);
            var projectile = instance.GetComponent<Projectile>();

            if (projectile == null)
            {
                projectile = instance.AddComponent<Projectile>();
            }

            instance.SetActive(false);
            return projectile;
        }

        private void OnGetProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
        }

        private void OnReleaseProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(_poolContainer);
        }

        private void OnDestroyProjectile(Projectile projectile)
        {
            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }
        }
    }
}
