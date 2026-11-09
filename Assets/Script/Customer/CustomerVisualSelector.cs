
using UnityEngine;

public class CustomerVisualSelector : MonoBehaviour
{
    [Header("Customer Visual Models")]
    public GameObject normalModel;
    public GameObject anomalyModel;

    public void SetModel(bool isAnomaly)
    {
        if (normalModel == null || anomalyModel == null)
        {
            Debug.LogError("Customer Visual Models are missing!");
            return;
        }

        normalModel.SetActive(!isAnomaly);
        anomalyModel.SetActive(isAnomaly);

        Debug.Log(isAnomaly
            ? "Anomaly Model Selected"
            : "Normal Model Selected");
    }
}
