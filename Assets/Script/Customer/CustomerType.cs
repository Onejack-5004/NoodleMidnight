
using UnityEngine;

public class CustomerType : MonoBehaviour
{
    public enum Category
    {
        Normal,
        Anomaly
    }

    [Header("Customer Type")]
    public Category customerCategory = Category.Normal;

    public bool IsAnomaly
    {
        get { return customerCategory == Category.Anomaly; }
    }
}
