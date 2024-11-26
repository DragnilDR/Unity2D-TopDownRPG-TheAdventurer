using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance;

    private Light2D globalLight;

    [Header("Light")]
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;

    [Header("Time")]
    public int hours;
    public int minutes;
    [SerializeField] private float timeSpeed;

    [SerializeField] private TextMeshProUGUI currentTimeUI;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        globalLight = GameObject.Find("GlobalLight2D").GetComponent<Light2D>();

        StartCoroutine(UpdateTime());
        UpdateCurrentTimeUI();
        UpdateLightIntensity();
    }

    private IEnumerator UpdateTime()
    {
        while (true)
        {
            minutes++;

            if (minutes >= 60)
            {
                hours++;

                if (hours >= 24)
                {
                    hours = 0;
                }

                minutes = 0;
            }

            UpdateCurrentTimeUI();
            UpdateLightIntensity();

            yield return new WaitForSeconds(timeSpeed);
        }
    }

    public void UpdateCurrentTimeUI()
    {
        currentTimeUI.text = $"{hours:00}:{minutes:00}";
    }

    public void UpdateLightIntensity()
    {
        if (SceneManager.GetActiveScene().name != "Tavern" 
            && SceneManager.GetActiveScene().name != "Shop"
            && SceneManager.GetActiveScene().name != "Cave")
        {
            float intensity = CalculateIntensity();

            if (globalLight != null)
                globalLight.intensity = intensity;
        }
    }

    private float CalculateIntensity()
    {
        // Преобразуем время в одно число для удобства вычислений
        float currentTime = hours + minutes / 60f;

        // Определяем интервалы изменения интенсивности
        float dawnStart = 4f;
        float morningStart = 7f;
        float eveningStart = 17f;
        float nightStart = 22f;

        // Интерполируем между минимальной и максимальной интенсивностью в зависимости от времени
        if (currentTime >= dawnStart && currentTime < morningStart)
        {
            return Mathf.Lerp(minIntensity, maxIntensity, (currentTime - dawnStart) / (morningStart - dawnStart));
        }
        else if (currentTime >= morningStart && currentTime < eveningStart)
        {
            return maxIntensity;
        }
        else if (currentTime >= eveningStart && currentTime < nightStart)
        {
            return Mathf.Lerp(maxIntensity, minIntensity, (currentTime - eveningStart) / (nightStart - eveningStart));
        }
        else
        {
            return minIntensity;
        }
    }
}
