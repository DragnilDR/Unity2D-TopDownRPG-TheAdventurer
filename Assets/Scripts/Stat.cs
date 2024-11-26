[System.Serializable]
public class Stat
{
    public string statName;
    public float baseValue;
    public float modifiedValue;

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        modifiedValue = baseValue;
    }

    public void AddValue(float value)
    {
        modifiedValue += value;
    }

    public void RemoveValue(float value)
    {
        modifiedValue -= value;
    }
}
