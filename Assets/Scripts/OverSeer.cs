using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OverSeer : MonoBehaviour
{
    //The struct for request queue
    struct PathRequest
    {
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Action<Vector3[], bool> Callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
        {
            StartPos = start;
            EndPos = end;
            Callback = callback;
        }
    }

    //Public variables for editor
    public List<Shop> ShopList;
    public Transform Entrance;
    public int DefaulCustomerAmount;

    //Private variables
    private List<Customer> m_customerList = new List<Customer>();
    private int m_customerAmount;
    private Coroutine m_recountCoroutine;

    private Pathfinder m_pathFinder;
    private Queue<PathRequest> m_requestQueue = new Queue<PathRequest>();
    private bool m_isProcessingPath;
    private PathRequest m_currentProcess;



    private static OverSeer m_instance;
    public static OverSeer Instance
    {
        get { return m_instance; }
    }


    void Awake()
    {
        m_pathFinder = GetComponent<Pathfinder>();
        if (m_instance == null)
            m_instance = this;

        m_customerAmount = DefaulCustomerAmount;
    }

    void Start()
    {
        StartUpdatingShopperAmount();
    }

    //Static function for pathfinding
    public static void RequestPath(Vector3 requesterPosition, Vector3 targetPosition, Action<Vector3[], bool> callback)
    {
        PathRequest _newRequest = new PathRequest(requesterPosition, targetPosition, callback);
        m_instance.m_requestQueue.Enqueue(_newRequest);
        m_instance.TryProcessNext();
    }

    //After done with pathfinding for one in the queue, do the next one
    private void TryProcessNext()
    {
        if (!m_isProcessingPath && m_requestQueue.Count > 0)
        {

            m_currentProcess = m_requestQueue.Dequeue();
            m_isProcessingPath = true;
            m_pathFinder.StartFindPath(m_currentProcess.StartPos, m_currentProcess.EndPos);
        }
    }
    //Send pathfinding results back to our requester
    public void FinishProcessingPath(Vector3[] path, bool success)
    {
        m_currentProcess.Callback(path, success);
        m_isProcessingPath = false;
        TryProcessNext();
    }

    //Spawn more units or decrease them
    private void StartUpdatingShopperAmount()
    {
        if (m_recountCoroutine != null)
            StopCoroutine(m_recountCoroutine);

        m_recountCoroutine = StartCoroutine(ReCount());
    }

    private void AddShopper()
    {
        Customer _newCustomer = CustomerPool.GetCustomer();
        _newCustomer.InitializeCustomer(UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(20, 40), ShopList);
        m_customerList.Add(_newCustomer);
    }

    public void RemoveShopper(Customer customer = null)
    {
        Customer _removable = customer;
        if (_removable == null)
        {
            int _lastIndex = m_customerList.Count - 1;
            _removable = m_customerList[m_customerList.Count - 1];
        }
        
        m_customerList.Remove(_removable);
        CustomerPool.AddCustomer(_removable);
    }

    public void UpdateShopperAmount(int newAmount = 0)
    {
        if(newAmount > 0)
            m_customerAmount = newAmount;
        StartUpdatingShopperAmount();
    }

    public static Shop GetShop(Shop.ShopEnum shopType, int ID = 0)
    {
        Shop _returnedShop = null;
        for (int i = 0; i < m_instance.ShopList.Count; i++)
        {
            if (m_instance.ShopList[i].ShopType == shopType)
            {
                if (ID != 0 && m_instance.ShopList[i].ShopID != ID)
                    continue;

                _returnedShop = m_instance.ShopList[i];
                break;
            }
        }
        return _returnedShop;
    }



    IEnumerator ReCount()
    {
        yield return new WaitForSeconds(1.0f);

        while (m_customerList.Count < m_customerAmount)
        {
            AddShopper();
            yield return new WaitForSeconds(0.1f);
        }
        while (m_customerList.Count > m_customerAmount)
        {
            RemoveShopper();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
