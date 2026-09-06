using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
public class FishingNetAssist : MonoBehaviour
{
    [Header("Scene References")]
    public Slider balanceSlider;
    public TMP_Text npcLabel;
    public RTLTextMeshPro timerDisplayText;
    public GameObject player;

    [Header("Quest Settings")]
    [SerializeField] GameObject npcCanvas;
    [SerializeField] float questActivationDelay = 10f;
    [SerializeField] float interactionDistance = 3f;
    [SerializeField] float sliderHeightAbovePlayer = 2f;

    float questTimer;
    public float timeInZone;
    public float totalTaskTime;

    bool questActivated;
    bool isMiniGameActive;
    bool taskFinished;

    void Start()
    {
        if (player == null)
            player = GameObject.Find("Player");

        if (npcCanvas == null)
            npcCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;

        if (npcLabel == null && npcCanvas != null)
            npcLabel = npcCanvas.GetComponentInChildren<RTLTextMeshPro>(true);

        CreateTimerDisplayIfNeeded();

        if (npcCanvas != null)
            npcCanvas.SetActive(false);

        if (balanceSlider != null)
            balanceSlider.gameObject.SetActive(false);

        if (timerDisplayText != null)
            timerDisplayText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (taskFinished)
            return;

        if (!questActivated)
        {
            questTimer += Time.deltaTime;
            if (questTimer >= questActivationDelay)
                ActivateQuest();
        }

        if (!questActivated || isMiniGameActive || player == null)
        {
            if (isMiniGameActive)
                UpdateMiniGame();
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) > interactionDistance)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            StartMiniGame();
    }

    void ActivateQuest()
    {
        questActivated = true;

        if (npcCanvas != null)
            npcCanvas.SetActive(true);

        if (npcLabel != null)
            npcLabel.text = "اضغط E للمساعدة";

        Debug.Log("Quest Activated");
    }

    void StartMiniGame()
    {
        isMiniGameActive = true;
        timeInZone = 0f;
        totalTaskTime = 0f;

        if (npcCanvas != null)
            npcCanvas.SetActive(false);

        if (balanceSlider != null)
        {
            balanceSlider.gameObject.SetActive(true);
            balanceSlider.value = 0.5f;

            // if (player != null)
            //     balanceSlider.transform.position = player.transform.position + Vector3.up * sliderHeightAbovePlayer;
        }

        if (timerDisplayText != null)
        {
            timerDisplayText.gameObject.SetActive(true);
            timerDisplayText.text = "Stability: 0.0s / 10.0s";
            PositionTimerDisplay();
        }

        Debug.Log("Task Started");
    }

    void CreateTimerDisplayIfNeeded()
    {
        if (timerDisplayText != null)
            return;

        Canvas parentCanvas = balanceSlider != null
            ? balanceSlider.GetComponentInParent<Canvas>()
            : null;

        if (parentCanvas == null)
            parentCanvas = FindFirstObjectByType<Canvas>();

        GameObject timerObject = new GameObject("Fishing Net Timer");
        Transform parent = parentCanvas != null ? parentCanvas.transform : transform;
        timerObject.transform.SetParent(parent, false);

        RTLTextMeshPro createdText = timerObject.AddComponent<RTLTextMeshPro>();
        createdText.fontSize = 50;
        createdText.alignment = TextAlignmentOptions.Center;
        createdText.color = Color.white;
        createdText.text = "Stability: 0.0s / 10.0s";

        timerDisplayText = createdText;
        timerDisplayText.gameObject.SetActive(false);
    }

    void PositionTimerDisplay()
    {
        if (timerDisplayText == null || balanceSlider == null)
            return;

        // timerDisplayText.transform.position = balanceSlider.transform.position + balanceSlider.transform.up * 0.3f;

        Canvas parentCanvas = timerDisplayText.GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            Camera mainCamera = Camera.main;
            // if (mainCamera != null)
            // {
            //     timerDisplayText.transform.rotation = Quaternion.LookRotation(
            //         timerDisplayText.transform.position - mainCamera.transform.position,
            //         Vector3.up);
            // }
        }
    }

    void UpdateMiniGame()
    {
        totalTaskTime += Time.deltaTime;

        if (balanceSlider != null)
        {
            balanceSlider.value -= 0.3f * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.R))
                balanceSlider.value += 0.1f;

            balanceSlider.value = Mathf.Clamp01(balanceSlider.value);

            if (balanceSlider.value > 0.4f && balanceSlider.value < 0.6f)
                timeInZone += Time.deltaTime;
            // else
            //     timeInZone = 0f;
        }

        if (timerDisplayText != null)
            timerDisplayText.text = $"Stability: {timeInZone:F1}s / 10.0s";

        if (timeInZone >= 10f)
        {
            CompleteTask();
            return;
        }

        if (totalTaskTime > 14f)
            FailTask();
    }

    void CompleteTask()
    {
        taskFinished = true;
        isMiniGameActive = false;

        if (balanceSlider != null)
            balanceSlider.gameObject.SetActive(false);

        if (timerDisplayText != null)
            timerDisplayText.gameObject.SetActive(false);

        Debug.Log("Win: Task Complete!");
        AddFoodStock(20);
    }

    void FailTask()
    {
        taskFinished = true;
        isMiniGameActive = false;

        if (balanceSlider != null)
            balanceSlider.gameObject.SetActive(false);

        if (timerDisplayText != null)
            timerDisplayText.gameObject.SetActive(false);

        Debug.Log("Lose: Task Failed!");
    }

    public void AddFoodStock(int amount)
    {
        Debug.Log("Food Stock increased by " + amount + ".");
    }
}
