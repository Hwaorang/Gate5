using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet 오브젝트를 미리 생성해두고 재사용하는 Pool
/// Instantiate / Destroy 반복 비용을 줄이기 위해 사용한다.
/// </summary>
public class BulletPool : MonoBehaviour
{
    [Header("Bullet 설정")]

    // 풀에서 사용할 Bullet Prefab
    [SerializeField]
    private GameObject bulletPrefab;

    // 게임 시작 시 미리 만들어둘 총알 개수
    [SerializeField]
    private int initialSize = 100;


    // 사용하지 않는 Bullet을 보관하는 Queue
    private readonly Queue<GameObject> pool =
        new Queue<GameObject>();


    private void Awake()
    {
        CreatePool();
    }


    /// <summary>
    /// 게임 시작 시 필요한 Bullet을 미리 생성한다.
    /// </summary>
    private void CreatePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewBullet();
        }
    }


    /// <summary>
    /// 새로운 Bullet을 하나 생성해서 Pool에 넣는다.
    /// </summary>
    private void CreateNewBullet()
    {
        GameObject bullet =
            Instantiate(
                bulletPrefab,
                transform
            );

        bullet.SetActive(false);

        pool.Enqueue(bullet);
    }


    /// <summary>
    /// Pool에서 Bullet을 하나 가져온다.
    /// 부족하면 새로운 Bullet을 추가 생성한다.
    /// </summary>
    public GameObject GetBullet(
        Vector3 position,
        Quaternion rotation)
    {
        if (pool.Count <= 0)
        {
            CreateNewBullet();
        }

        GameObject bullet =
            pool.Dequeue();

        bullet.transform.position = position;
        bullet.transform.rotation = rotation;

        bullet.SetActive(true);

        return bullet;
    }


    /// <summary>
    /// 사용이 끝난 Bullet을 다시 Pool로 반환한다.
    /// </summary>
    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        bullet.SetActive(false);

        bullet.transform.SetParent(transform);

        pool.Enqueue(bullet);
    }
}