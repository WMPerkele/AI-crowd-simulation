using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIOverSeer : MonoBehaviour
{

    //struct PathRequest
    //{
    //    public Vector3 StartPos;
    //    public Vector3 EndPos;
    //    public Action<Vector3[], bool> Callback;

    //    public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    //    {
    //        StartPos = start;
    //        EndPos = end;
    //        Callback = callback;
    //    }
    //}

    //public List<Shop> ShopList;
    //public int DefaultShopperAmount;

    //private List<Customer> m_customerList;
    //private Pathfinder m_pathFinder;
    //private Queue<PathRequest> m_requestQueue = new Queue<PathRequest>();
    //private bool m_isProcessingPath;
    //private PathRequest m_currentProcess;

    //private static  AIOverSeer m_instance;
    //public static AIOverSeer Instance
    //{
    //    get { return m_instance; }
    //}


    //void Awake()
    //{
    //    m_pathFinder = GetComponent<Pathfinder>();
    //    if (m_instance == null)
    //        m_instance = this;
    //}

    //static void RequestPath(Vector3 requesterPosition, Vector3 targetPosition, Action<Vector3[], bool> callback)
    //{
    //    PathRequest _newRequest = new PathRequest(requesterPosition, targetPosition, callback);
    //    m_instance.m_requestQueue.Enqueue(_newRequest);
    //    m_instance.TryProcessNext();
    //}

    //private void TryProcessNext()
    //{
    //    if (!m_isProcessingPath && m_requestQueue.Count > 0)
    //    {
    //        m_currentProcess = m_requestQueue.Dequeue();
    //        m_isProcessingPath = true;
    //        m_pathFinder.StartFindPath(m_currentProcess.StartPos, m_currentProcess.EndPos);
    //    }
    //}

    //public void FinishProcessingPath(Vector3[] path, bool success)
    //{
    //    m_currentProcess.Callback(path, success);
    //    m_isProcessingPath = false;
    //    TryProcessNext();
    //}

    //public static Shop GetShop(Shop.ShopEnum shopType)
    //{
    //    Shop _returnedShop = null;
    //    for (int i = 0; i < m_instance.ShopList.Count; i++)
    //    {
    //        if (m_instance.ShopList[i].ShopType == shopType)
    //        {
    //            _returnedShop = m_instance.ShopList[i];
    //            break;
    //        }
    //    }
    //    return _returnedShop;
    //}

}
