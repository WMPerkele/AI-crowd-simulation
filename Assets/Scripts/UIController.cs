using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider shopperAmount;
    public Text shopperAmountText;
    public Slider timescaleAmount;
    private Coroutine m_updateShopperAmount;

    void Start()
    {
        if (shopperAmount != null && shopperAmountText != null)
        {
            shopperAmount.value = ((float)OverSeer.Instance.DefaulCustomerAmount / 100.0f);
            shopperAmountText.text = "Amount of shoppers: " + (shopperAmount.value * 100).ToString();
        }
    }


    public void ShopperAmountUpdate()
    {
        shopperAmountText.text = "Amount of shoppers: " + ((int)(shopperAmount.value * 100)).ToString();

        if(m_updateShopperAmount != null)
            StopCoroutine(m_updateShopperAmount);
        m_updateShopperAmount = StartCoroutine(WaitToUpdateOverSeer());
    }

    public void UpdateTimeScale()
    {
        Time.timeScale = Mathf.Max(10 * timescaleAmount.value, 0.1f);
    }

    IEnumerator WaitToUpdateOverSeer()
    {
        float timeToWait = 0.4f;

        while(timeToWait > 0.0f)
        {
            timeToWait -= Time.deltaTime;
            yield return null;
        }

        OverSeer.Instance.UpdateShopperAmount((int)(shopperAmount.value * 100.0f));
    }
}
