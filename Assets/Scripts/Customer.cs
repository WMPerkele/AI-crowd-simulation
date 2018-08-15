using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public enum CustomerAIState
    {
        Hungry,
        Pissed,
        Walking,
        Running,
        Shopping,
        Satisfied
    }

    [System.Serializable]
    public class ShopData
    {
        public float ShoppingAmount;
        public Shop ShopObject;
    }

    public CustomerAIState CustomerState;
    public float Bladder, Hunger;
    public float BladderAddition, HungerAddition;
    public float BladderThreshold, HunderThreshold;

    private List<ShopData> m_shoppingList;
    private ShopData m_targetShop;
    private Coroutine m_shopCoroutine, m_walkCoroutine;

    public float Speed = 5;
    public Vector3[] Path;
    public int TargetIndex;

    //Initialize customer values when starting shopping
    public void InitializeCustomer(int bladder, int hunger, List<Shop> shopEntrances)
    {
        m_shoppingList = new List<ShopData>();
        Bladder = bladder;
        Hunger = hunger;
        BladderAddition = Random.Range(1.0f, 2.0f);
        HungerAddition = Random.Range(0.5f, 1.5f);
        for (int i = 0; i < shopEntrances.Count; i++)
        {
            //Check that we're adding a shop and randomize so we dont always go to each shop
            if (shopEntrances[i].ShopType == Shop.ShopEnum.Shop && Random.Range(0, 100) < 75)
            {
                ShopData _newData = new ShopData();
                _newData.ShoppingAmount = Random.Range(10, 15);
                _newData.ShopObject = shopEntrances[i];
                m_shoppingList.Add(_newData);
            }
        }

        //Randomize the color to add some variation
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        ChangeState(CustomerAIState.Shopping);

    }

    private void RequestShopPath(Shop targetShop)
    {
        OverSeer.RequestPath(gameObject.transform.position, targetShop.Entrance.transform.position, OnPathFound);
    }

    private void RequestExitPath()
    {
        OverSeer.RequestPath(gameObject.transform.position, OverSeer.Instance.Entrance.transform.position, OnPathFound);
    }

    //This function is delegated to be called when pathfinder finds a path
    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        //Check that we actually found one
        if (pathSuccess && newPath.Length > 0)
        {
            //Add the shop location to the path so the customer goes inside it
            if (m_targetShop != null)
            {
                Path = new Vector3[newPath.Length + 1];
                for (int i = 0; i < newPath.Length; i++)
                {
                    Path[i] = newPath[i];
                }

                Path[Path.Length - 1] = m_targetShop.ShopObject.transform.position;
            }
            else
                Path = newPath;
            if(m_walkCoroutine != null) StopCoroutine(m_walkCoroutine);
            m_walkCoroutine = StartCoroutine(FollowPath());
        }
        //If we dont find a path, attempt again
        else
        {
            Debug.Log("THE CUSTOMER: " + gameObject.name + " COULDNT FIND PATH TO: " + m_targetShop.ShopObject.name);
            if (CustomerState == CustomerAIState.Satisfied)
                RequestExitPath();
            else
                RequestShopPath(m_targetShop.ShopObject);

        }

    }

    //When we arrive to our target call this
    private void OnArrival()
    {
        //If we're exiting, remove our shopped and make overseer do a recount
        if (CustomerState == CustomerAIState.Satisfied)
        {
            OverSeer.Instance.RemoveShopper(this);
            OverSeer.Instance.UpdateShopperAmount();
            return;
        }
        //Start shopping
        if (m_targetShop.ShopObject != null)
        {
            if (m_shopCoroutine != null)
                StopCoroutine(m_shopCoroutine);

            m_shopCoroutine = StartCoroutine(Shopping());
        }
        
    }

    //When we exit a shop, find a new target, or start leaving shop if we are done
    private void OnExitShop()
    {
        if(m_shoppingList.Contains(m_targetShop) && m_targetShop.ShoppingAmount < 0.1f)
            m_shoppingList.Remove(m_targetShop);

        m_targetShop = null;

        if (m_shoppingList.Count == 0)
            ChangeState(CustomerAIState.Satisfied);
        else
            PickNextShop();
    }

    private void PickNextShop()
    {
        m_targetShop = m_shoppingList[Random.Range(0, m_shoppingList.Count)];
        RequestShopPath(OverSeer.GetShop(Shop.ShopEnum.Shop, m_targetShop.ShopObject.ShopID));
    }

    //State machine
    public void ChangeState(CustomerAIState newState)
    {
        switch (newState)
        {
            case CustomerAIState.Shopping:
                if (m_targetShop != null)
                {
                    if (m_shoppingList.Contains(m_targetShop) && m_targetShop.ShoppingAmount < 0.0f)
                    {
                        m_shoppingList.Remove(m_targetShop);
                        PickNextShop();
                    }
                    else
                        RequestShopPath(m_targetShop.ShopObject);

                }
                else
                    PickNextShop();
                Speed = 5.0f;
                break;
            case CustomerAIState.Hungry:
                m_targetShop = new ShopData();
                m_targetShop.ShopObject = OverSeer.GetShop(Shop.ShopEnum.Cafe);
                m_targetShop.ShoppingAmount = Hunger;
                RequestShopPath(m_targetShop.ShopObject);
                Speed = 5.0f;
                break;
            case CustomerAIState.Pissed:
                m_targetShop = new ShopData();
                m_targetShop.ShopObject = OverSeer.GetShop(Shop.ShopEnum.WC , Random.Range(1, 3));
                m_targetShop.ShoppingAmount = Bladder;
                RequestShopPath(m_targetShop.ShopObject);
                Speed = 7.5f;
                break;
            case CustomerAIState.Running:
                RequestExitPath();
                Speed = 10;
                break;
            case CustomerAIState.Satisfied:
                RequestExitPath();
                Speed = 5.0f;
                break;
            default:
                break;
        }
        CustomerState = newState;
    }

    IEnumerator FollowPath()
    {
        Vector3 _currentWayPoint = Path[0];
        TargetIndex = 0;
        
        while (true)
        {
            if (Vector3.Distance(transform.position,_currentWayPoint) < 0.1f)
            {
                TargetIndex++;
                if (TargetIndex >= Path.Length)
                {
                    OnArrival();
                    yield break;
                }

                _currentWayPoint = Path[TargetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, _currentWayPoint, Speed * Time.deltaTime);
            yield return null;

        }
    }

    IEnumerator Shopping()
    {
        float _spendRate = m_targetShop.ShopObject.TimePerUnit;
        while (m_targetShop.ShoppingAmount > 0.0f)
        {
            m_targetShop.ShoppingAmount -= Time.deltaTime * _spendRate;
            yield return null;
        }
        if (CustomerState == CustomerAIState.Shopping)
            OnExitShop();
        else if (CustomerState == CustomerAIState.Pissed)
        {
            Bladder = 0.0f;
            m_targetShop = null;
            ChangeState(CustomerAIState.Shopping);
        }
        else if (CustomerState == CustomerAIState.Hungry)
        {
            Hunger = 0.0f;
            m_targetShop = null;
            ChangeState(CustomerAIState.Shopping);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (CustomerState == CustomerAIState.Satisfied || m_shoppingList.Count <= 1)
        {
            return;
        }
        if (CustomerState == CustomerAIState.Shopping)
        {
            Bladder += BladderAddition * Time.deltaTime;
            Hunger += HungerAddition * Time.deltaTime;
        }

        if(CustomerState != CustomerAIState.Hungry && CustomerState != CustomerAIState.Pissed)
        {
            if (Bladder > BladderThreshold)
            {
                if (m_shopCoroutine != null)
                    StopCoroutine(m_shopCoroutine);
                ChangeState(CustomerAIState.Pissed);
            }
            if (Hunger > HunderThreshold)
            {
                if (m_shopCoroutine != null)
                    StopCoroutine(m_shopCoroutine);
                ChangeState(CustomerAIState.Hungry);
            }
        }
	}
}
