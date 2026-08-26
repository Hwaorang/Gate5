using UnityEngine;

public class SoldierAttack : MonoBehaviour
{
    [Header("총알 설정")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float damage = 10f;

    /// <summary>
    /// 지정된 적을 향해 총알을 발사한다.
    /// </summary>
    public void Fire(Transform target)
    {
        if (target == null)
        {
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Init(target, damage);
        }
    }
}