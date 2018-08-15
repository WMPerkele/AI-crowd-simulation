using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindertester : MonoBehaviour
{
    public Shop CurrentTarget;
    public float Speed = 5;
    public Vector3[] Path;
    public int TargetIndex;


    public void Start()
    {
        OverSeer.RequestPath(OverSeer.Instance.Entrance.transform.position, OverSeer.GetShop(Shop.ShopEnum.WC).transform.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess)
        {
            Path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 _currentWayPoint = Path[0];

        while (true)
        {
            if (transform.position == _currentWayPoint)
            {
                TargetIndex++;
                if (TargetIndex >= Path.Length)
                {
                    yield break;
                }

                _currentWayPoint = Path[TargetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, _currentWayPoint, Speed);
            yield return null;

        }
    }
}
