using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    private List<Customer> m_availableCustomers = new List<Customer>();

    public Customer CustomerPrefab;

    private static CustomerPool m_instance;
    public static CustomerPool Instance
    { get { return m_instance; } }

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }

    public static Customer GetCustomer()
    {
        Customer _customer;
        int _availableObjectIndex = m_instance.m_availableCustomers.Count - 1;
        if (_availableObjectIndex >= 0)
        {
            _customer = m_instance.m_availableCustomers[_availableObjectIndex];
            m_instance.m_availableCustomers.RemoveAt(_availableObjectIndex);
            _customer.gameObject.SetActive(true);
        }
        else
        {
            _customer = Instantiate<Customer>(m_instance.CustomerPrefab);
            _customer.transform.position = OverSeer.Instance.Entrance.transform.position;
            _customer.transform.SetParent(m_instance.transform, false);
        }
        return _customer;
    }

    public static void ReturnCustomer(Customer customer)
    {
        m_instance.m_availableCustomers.Add(customer);
        customer.gameObject.SetActive(false);
    }

    public static void AddCustomer(Customer customer)
    {
        customer.gameObject.SetActive(false);
        m_instance.m_availableCustomers.Add(customer);
    }
}
