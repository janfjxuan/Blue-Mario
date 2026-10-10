using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObjects/IntVariable", order = 2)]
public class IntVariable : Variable<int>
{
    public int previousHighestValue;

    // backup copy of the highest value, used if the asset is ever reloaded
    private string HighestKey => "IntVariable_highest_" + name;

    void OnEnable()
    {
        // keep this asset in memory across scene changes
        hideFlags = HideFlags.DontUnloadUnusedAsset;

        previousHighestValue = Mathf.Max(previousHighestValue, PlayerPrefs.GetInt(HighestKey, 0));
    }

    public override void SetValue(int value)
    {
        if (value > previousHighestValue)
        {
            previousHighestValue = value;
            PlayerPrefs.SetInt(HighestKey, value);
        }

        _value = value;
    }

    // overload
    public void SetValue(IntVariable value)
    {
        SetValue(value.Value);
    }

    public void ApplyChange(int amount)
    {
        this.Value += amount;
    }

    public void ApplyChange(IntVariable amount)
    {
        ApplyChange(amount.Value);
    }

    public void ResetHighestValue()
    {
        previousHighestValue = 0;
        PlayerPrefs.DeleteKey(HighestKey);
    }
}