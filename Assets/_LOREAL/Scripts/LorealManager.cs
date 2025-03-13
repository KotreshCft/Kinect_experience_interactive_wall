

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;
//using static LorealBodySourceView;
//using TMPro;

//public class LorealManager : MonoBehaviour
//{
//    #region "Initialization"

//    private LorealBodySourceView bodySourceView;
//    private LorealGridManager tileGrid;

//    public GameObject handObject;

//    [Header("Flags")]
//    public bool buttonClicked = false;

//    [Header("Grid Sprites")]
//    public Sprite defaultImage;
//    public Sprite[] gameSprites;
//    public Sprite[] transitionSprites;

//    [Header("Variables")]
//    public int totalTiles;
//    public int changedTileCount = 0;
//    public int gridSizeX = 10;
//    public int gridSizeY = 10;
//    public float gameDuration = 60f;


//    [Header("Screen UI Reference")]
//    public GameObject[] Panels; //1. idle, 2. start, 3. main game, 4. end screen panel

//    public GameObject imagePrefab;

//    [Header("Timer Reference")]
//    public GameObject timerDial;
//    public Text timerText;

//    [Header("Video Reference")]
//    public VideoPlayer videoPlayer;
//    public RawImage rawImage;
//    public TMP_Text gameOverText; // Assign this in the Unity Inspector
//    #endregion

//    #region "Start, Update"
//    private void Start()
//    {
//        if (bodySourceView == null)
//        {
//            bodySourceView = FindAnyObjectByType<LorealBodySourceView>();
//            bodySourceView.currentGameState = GameState.Idle;
//        }
//        if (tileGrid == null)
//        {
//            tileGrid = FindAnyObjectByType<LorealGridManager>();
//        }
//        bodySourceView.StartDetection();
//        foreach (GameObject screen in Panels)
//        {
//            screen.SetActive(false);
//        }

//        Panels[0].SetActive(true); // setting idle panel visible at game start
//        timerDial.SetActive(false);
//        totalTiles = gridSizeX * gridSizeY; // calculating total number of tiles
//        buttons[1].interactable = false;
//        buttons[0].interactable = false;

//    }
//    IEnumerator EnableButtonAfterDealy()
//    {
//        yield return new WaitForSeconds(3f);
//        buttons[0].interactable = true;
//    }

//    public void ChangeGameState(GameState newState)
//    {
//        if (bodySourceView != null)
//        {
//            bodySourceView.currentGameState = newState;
//        }
//    }

//    void Update()
//    {
//        if (bodySourceView != null)
//        {
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//                RestartGame();
//            }
//            if (handObject == null)
//            {
//                handObject = bodySourceView.GetHandCircle();
//            }

//            if (handObject != null)
//            {
//                Vector3 worldPosition = handObject.transform.position;
//                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

//                PointerEventData pointerData = new PointerEventData(EventSystem.current)
//                {
//                    position = new Vector2(screenPosition.x, screenPosition.y)
//                };

//                List<RaycastResult> raycastResults = new List<RaycastResult>();
//                EventSystem.current.RaycastAll(pointerData, raycastResults);

//                if (!buttonClicked)
//                {
//                    foreach (var result in raycastResults)
//                    {
//                        var button = result.gameObject.GetComponent<Button>();
//                        if (button != null)
//                        {
//                            Debug.LogError("Hand is hovering over the button: " + result.gameObject.name);

//                            if (result.gameObject.name == "DetectButton")
//                            {
//                                ShowSecondScreen();
//                            }
//                            else if (result.gameObject.name == "StartButton")
//                            {
//                                buttonClicked = true;
//                                StartCoroutine(ShowDelayScreen());
//                                break;
//                            }
//                        }
//                    }
//                }

//                float handRadius = .5f;
//                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, handRadius);

//                Debug.DrawRay(worldPosition, Vector3.right * 50f, Color.red);
//                Debug.DrawRay(worldPosition, Vector3.up * 50f, Color.red);

//                foreach (var hitCollider in hitColliders)
//                {
//                    if (hitCollider.CompareTag("Tile"))
//                    {
//                        ChangeTileInteraction(hitCollider.gameObject);
//                    }
//                }

//            }
//            else
//            {
//                Debug.Log("No hand object detected.");
//            }
//        }
//    }
//    #endregion

//    #region Functions
//    void ChangeTileInteraction(GameObject tile)
//    {
//        if (tile.GetComponent<Tile>().isChanged)
//        {
//            Debug.Log("Tile already changed: " + tile.name);
//            return;
//        }

//        Image spriteRenderer = tile.GetComponent<Image>();

//        if (spriteRenderer == null)
//        {
//            Debug.LogError("No Image component found on tile: " + tile.name);
//            return;
//        }

//        if (defaultImage == null)
//        {
//            Debug.LogError("Default image is not set.");
//            return;
//        }

//        Sprite currentSprite = spriteRenderer.sprite;
//        int spriteIndex = System.Array.IndexOf(gameSprites, currentSprite);

//        if (spriteIndex < 0 || spriteIndex >= transitionSprites.Length)
//        {
//            Debug.LogError("Sprite index is out of bounds or not found in gameSprites array.");
//            return;
//        }

//        StartCoroutine(ChangeTileWithTransition(spriteRenderer, tile, spriteIndex));

//        tile.GetComponent<Tile>().isChanged = true;
//        changedTileCount++;

//        Debug.Log($"Changed tile: {tile.name} | Total changed: {changedTileCount}/{totalTiles}");

//        if (changedTileCount >= totalTiles)
//        {
//            Debug.Log("All tiles have been changed!");
//            GameOver(false); // Pass false to indicate all tiles changed
//        }
//    }

//    IEnumerator ChangeTileWithTransition(Image spriteRenderer, GameObject tile, int spriteIndex)
//    {
//        Sprite transitionImage = transitionSprites[spriteIndex];

//        // Fade out the first image
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 0f, 1f, 0.3f)); // Fade-out with normal brightness

//        // Change to transition image
//        spriteRenderer.sprite = transitionImage;

//        // Fade in the new image with increased brightness
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 0f, 1f, 1.5f, 0.3f)); // 1.5x brightness effect

//        // Gradually restore brightness to normal
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 1f, 1f, 0.3f)); // Restore to normal brightness

//        Debug.Log($"Tile transition complete for: {tile.name}");
//    }

//    // Helper function to fade image with brightness effect
//    IEnumerator FadeImageWithBrightness(Image image, float startAlpha, float endAlpha, float brightnessFactor, float duration)
//    {
//        float elapsedTime = 0f;
//        Color originalColor = image.color;

//        while (elapsedTime < duration)
//        {
//            elapsedTime += Time.deltaTime;
//            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
//            float brightness = Mathf.Lerp(1f, brightnessFactor, elapsedTime / duration); // Increase brightness

//            image.color = new Color(
//                Mathf.Clamp01(originalColor.r * brightness),
//                Mathf.Clamp01(originalColor.g * brightness),
//                Mathf.Clamp01(originalColor.b * brightness),
//                alpha
//            );

//            yield return null;
//        }

//        // Ensure final values are set
//        image.color = new Color(
//            Mathf.Clamp01(originalColor.r * brightnessFactor),
//            Mathf.Clamp01(originalColor.g * brightnessFactor),
//            Mathf.Clamp01(originalColor.b * brightnessFactor),
//            endAlpha
//        );
//    }
//    void ShowSecondScreen()
//    {
//        Panels[0].SetActive(false);
//        Panels[1].SetActive(true);
//        buttons[1].interactable = true;
//    }

//    public VideoClip congratulationClip;
//    public VideoClip timeOverClip;
//    public VideoClip gridTransitionClip;
//    public Button[] buttons;
//    public TMP_Text congratulationText;

//    public void GameOver(bool isTimeOver)
//    {
//        Debug.Log("Game Over!");

//        DestroyBodyObjects();
//        DestroyHandObject();
//        bodySourceView.StopDetection();

//        Panels[3].SetActive(true);
//        congratulationText.gameObject.SetActive(true);

//        Panels[2].SetActive(false);
//        timerDial.SetActive(false);

//        videoPlayer.Stop();

//        if (isTimeOver)
//        {
//            Debug.Log("Game Over due to time running out.");
//            videoPlayer.clip = timeOverClip; // Assign appropriate clip in the inspector
//            gameOverText.text = "Better luck next time!"; // Display message for losing
//        }
//        else
//        {
//            Debug.Log("Game Over because all tiles were changed.");
//            videoPlayer.clip = congratulationClip; // Assign appropriate clip in the inspector
//            gameOverText.text = "Congratulations! You won!"; // Display message for winning
//        }

//        videoPlayer.Play();
//        videoPlayer.loopPointReached += OnVideoFinish;
//    }


//    // This function is called when the video finishes playing
//    private void OnVideoFinish(VideoPlayer vp)
//    {
//        Debug.Log("Video finished. Restarting the game...");

//        // Call the RestartGame method
//        RestartGame();
//    }

//    IEnumerator ShowDelayScreen()
//    {
//        Debug.Log("calling here");
//        yield return new WaitForSeconds(3);
//        Debug.Log("calling here also");
//        PlayVideoThenCreateGrid();
//    }
//    void PlayVideoThenCreateGrid()
//    {
//        Panels[1].SetActive(false);
//        Panels[3].SetActive(true);
//        congratulationText.gameObject.SetActive(false);
//        handObject.SetActive(false);
//        bodySourceView.StopDetection();

//        videoPlayer.Stop();
//        videoPlayer.clip = gridTransitionClip;
//        videoPlayer.Play();
//        videoPlayer.loopPointReached -= CreateGridAfterVideo;
//        videoPlayer.loopPointReached += CreateGridAfterVideo;
//    }

//    private void CreateGridAfterVideo(VideoPlayer vp)
//    {
//        Panels[2].SetActive(true);
//        Panels[3].SetActive(false);
//        // Video has finished, now create the grid
//        ChangeGameState(GameState.Running);
//        bodySourceView.StartDetection();
//        timerDial.SetActive(true);
//        handObject.SetActive(true);
//        StartCoroutine(TimerCoroutine(gameDuration));
//        CreateGameGrid();
//        videoPlayer.loopPointReached -= CreateGridAfterVideo;
//    }

//    private void CreateGameGrid()
//    {
//        // Ensure gameSprites contains the sprites before passing them to CreateGameGrid
//        if (gameSprites == null || gameSprites.Length == 0)
//        {
//            Debug.LogError("gameSprites array is empty. Please assign sprites before creating the game grid.");
//            return;
//        }

//        // Create a new grid with actual game tiles
//        tileGrid.CreateGameGrid(gridSizeX, gridSizeY, imagePrefab, gameSprites);
//    }

//    private IEnumerator TimerCoroutine(float duration)
//    {
//        float timer = duration;

//        while (timer > 0)
//        {
//            //Debug.Log("Time left: " + timer + " seconds");
//            timerText.text = timer.ToString();
//            yield return new WaitForSeconds(1);
//            timer--;
//        }

//        Debug.Log("Timer finished!");
//        GameOver(true); // Pass true to indicate time over
//    }

//    private void DestroyBodyObjects()
//    {
//        foreach (var body in bodySourceView.GetBodies())
//        {
//            if (body != null)
//            {
//                Destroy(body);
//            }
//        }
//    }

//    private void DestroyHandObject()
//    {
//        GameObject handCircle = bodySourceView.GetHandCircle();
//        if (handCircle != null)
//        {
//            Destroy(handCircle);
//        }
//    }

//    void RestartGame()
//    {
//        string currentScene = SceneManager.GetActiveScene().name;
//        SceneManager.LoadScene(currentScene);
//    }
//    #endregion
//}
//+++++++++++++++++++++++++++++++++++++++++++++

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;
//using static LorealBodySourceView;
//using TMPro;

//public class LorealManager : MonoBehaviour
//{
//    #region "Initialization"

//    private LorealBodySourceView bodySourceView;
//    private LorealGridManager tileGrid;

//    public GameObject handObject;

//    [Header("Flags")]
//    public bool buttonClicked = false;
//    public bool sliderActivated = false;

//    [Header("Slider Configuration")]
//    public Slider mainSlider;  // Assign this in the inspector
//    public float sliderSpeed = 1.0f;  // Speed at which slider moves
//    public float sliderThreshold = 0.95f;  // Threshold to trigger next screen

//    [Header("Grid Sprites")]
//    public Sprite defaultImage;
//    public Sprite[] gameSprites;
//    public Sprite[] transitionSprites;

//    [Header("Variables")]
//    public int totalTiles;
//    public int changedTileCount = 0;
//    public int gridSizeX = 10;
//    public int gridSizeY = 10;
//    public float gameDuration = 60f;


//    [Header("Screen UI Reference")]
//    public GameObject[] Panels; //1. idle, 2. start, 3. main game, 4. end screen panel

//    public GameObject imagePrefab;

//    [Header("Timer Reference")]
//    public GameObject timerDial;
//    public Text timerText;

//    [Header("Video Reference")]
//    public VideoPlayer videoPlayer;
//    public RawImage rawImage;
//    public TMP_Text gameOverText; // Assign this in the Unity Inspector
//    #endregion

//    #region "Start, Update"
//    private void Start()
//    {
//        if (bodySourceView == null)
//        {
//            bodySourceView = FindAnyObjectByType<LorealBodySourceView>();
//            bodySourceView.currentGameState = GameState.Idle;
//        }
//        if (tileGrid == null)
//        {
//            tileGrid = FindAnyObjectByType<LorealGridManager>();
//        }
//        bodySourceView.StartDetection();
//        foreach (GameObject screen in Panels)
//        {
//            screen.SetActive(false);
//        }

//        Panels[0].SetActive(true); // setting idle panel visible at game start
//        timerDial.SetActive(false);
//        totalTiles = gridSizeX * gridSizeY; // calculating total number of tiles

//        // Initialize slider
//        if (mainSlider != null)
//        {
//            mainSlider.value = 0;
//            mainSlider.interactable = false; // We'll control it programmatically
//        }
//        else
//        {
//            Debug.LogError("Main Slider not assigned in the inspector!");
//        }
//    }

//    public void ChangeGameState(GameState newState)
//    {
//        if (bodySourceView != null)
//        {
//            bodySourceView.currentGameState = newState;
//        }
//    }

//    void Update()
//    {
//        if (bodySourceView != null)
//        {
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//                RestartGame();
//            }
//            if (handObject == null)
//            {
//                handObject = bodySourceView.GetHandCircle();
//            }

//            if (handObject != null)
//            {
//                Vector3 worldPosition = handObject.transform.position;
//                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

//                // Handle slider activation
//                if (Panels[0].activeInHierarchy && !sliderActivated)
//                {
//                    // Check if hand is over the slider
//                    if (IsHandOverSlider(screenPosition))
//                    {
//                        sliderActivated = true;
//                        Debug.Log("Slider activated!");
//                    }
//                }

//                // If slider is activated, update its value
//                if (sliderActivated && !buttonClicked)
//                {
//                    mainSlider.value += sliderSpeed * Time.deltaTime;

//                    // Check if slider reached threshold
//                    if (mainSlider.value >= sliderThreshold)
//                    {
//                        buttonClicked = true;
//                        StartCoroutine(ShowDelayScreen());
//                    }
//                }

//                // Handle tile interactions
//                float handRadius = .5f;
//                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, handRadius);

//                Debug.DrawRay(worldPosition, Vector3.right * 50f, Color.red);
//                Debug.DrawRay(worldPosition, Vector3.up * 50f, Color.red);

//                foreach (var hitCollider in hitColliders)
//                {
//                    if (hitCollider.CompareTag("Tile"))
//                    {
//                        ChangeTileInteraction(hitCollider.gameObject);
//                    }
//                }
//            }
//            else
//            {
//                Debug.Log("No hand object detected.");
//            }
//        }
//    }

//    // Check if hand is over the slider
//    private bool IsHandOverSlider(Vector3 screenPosition)
//    {
//        if (mainSlider == null) return false;

//        RectTransform sliderRect = mainSlider.GetComponent<RectTransform>();
//        Vector3[] corners = new Vector3[4];
//        sliderRect.GetWorldCorners(corners);

//        // Convert world coordinates to screen coordinates
//        for (int i = 0; i < 4; i++)
//        {
//            corners[i] = Camera.main.WorldToScreenPoint(corners[i]);
//        }

//        // Check if screen position is within slider bounds
//        bool isInside = screenPosition.x >= corners[0].x && screenPosition.x <= corners[2].x &&
//                         screenPosition.y >= corners[0].y && screenPosition.y <= corners[2].y;

//        return isInside;
//    }
//    #endregion

//    #region Functions
//    void ChangeTileInteraction(GameObject tile)
//    {
//        if (tile.GetComponent<Tile>().isChanged)
//        {
//            Debug.Log("Tile already changed: " + tile.name);
//            return;
//        }

//        Image spriteRenderer = tile.GetComponent<Image>();

//        if (spriteRenderer == null)
//        {
//            Debug.LogError("No Image component found on tile: " + tile.name);
//            return;
//        }

//        if (defaultImage == null)
//        {
//            Debug.LogError("Default image is not set.");
//            return;
//        }

//        Sprite currentSprite = spriteRenderer.sprite;
//        int spriteIndex = System.Array.IndexOf(gameSprites, currentSprite);

//        if (spriteIndex < 0 || spriteIndex >= transitionSprites.Length)
//        {
//            Debug.LogError("Sprite index is out of bounds or not found in gameSprites array.");
//            return;
//        }

//        StartCoroutine(ChangeTileWithTransition(spriteRenderer, tile, spriteIndex));

//        tile.GetComponent<Tile>().isChanged = true;
//        changedTileCount++;

//        Debug.Log($"Changed tile: {tile.name} | Total changed: {changedTileCount}/{totalTiles}");

//        if (changedTileCount >= totalTiles)
//        {
//            Debug.Log("All tiles have been changed!");
//            GameOver(false); // Pass false to indicate all tiles changed
//        }
//    }

//    IEnumerator ChangeTileWithTransition(Image spriteRenderer, GameObject tile, int spriteIndex)
//    {
//        Sprite transitionImage = transitionSprites[spriteIndex];

//        // Fade out the first image
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 0f, 1f, 0.3f)); // Fade-out with normal brightness

//        // Change to transition image
//        spriteRenderer.sprite = transitionImage;

//        // Fade in the new image with increased brightness
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 0f, 1f, 1.5f, 0.3f)); // 1.5x brightness effect

//        // Gradually restore brightness to normal
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 1f, 1f, 0.3f)); // Restore to normal brightness

//        Debug.Log($"Tile transition complete for: {tile.name}");




//        // Helper function to fade image with brightness effect
//        IEnumerator FadeImageWithBrightness(Image image, float startAlpha, float endAlpha, float brightnessFactor, float duration)
//        {
//            float elapsedTime = 0f;
//            Color originalColor = image.color;

//            while (elapsedTime < duration)
//            {
//                elapsedTime += Time.deltaTime;
//                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
//                float brightness = Mathf.Lerp(1f, brightnessFactor, elapsedTime / duration); // Increase brightness

//                image.color = new Color(
//                    Mathf.Clamp01(originalColor.r * brightness),
//                    Mathf.Clamp01(originalColor.g * brightness),
//                    Mathf.Clamp01(originalColor.b * brightness),
//                    alpha
//                );

//                yield return null;
//            }

//            // Ensure final values are set
//            image.color = new Color(
//                Mathf.Clamp01(originalColor.r * brightnessFactor),
//                Mathf.Clamp01(originalColor.g * brightnessFactor),
//                Mathf.Clamp01(originalColor.b * brightnessFactor),
//                endAlpha
//            );
//        }
//    }

//    public VideoClip congratulationClip;
//    public VideoClip timeOverClip;
//    public VideoClip gridTransitionClip;
//    public Button[] buttons;
//    public TMP_Text congratulationText;

//    public void GameOver(bool isTimeOver)
//    {
//        Debug.Log("Game Over!");

//        DestroyBodyObjects();
//        DestroyHandObject();
//        bodySourceView.StopDetection();

//        Panels[3].SetActive(true);
//        congratulationText.gameObject.SetActive(true);

//        Panels[2].SetActive(false);
//        timerDial.SetActive(false);

//        videoPlayer.Stop();

//        if (isTimeOver)
//        {
//            Debug.Log("Game Over due to time running out.");
//            videoPlayer.clip = timeOverClip; // Assign appropriate clip in the inspector
//            gameOverText.text = "Better luck next time!"; // Display message for losing
//        }
//        else
//        {
//            Debug.Log("Game Over because all tiles were changed.");
//            videoPlayer.clip = congratulationClip; // Assign appropriate clip in the inspector
//            gameOverText.text = "Congratulations! You won!"; // Display message for winning
//        }

//        videoPlayer.Play();
//        videoPlayer.loopPointReached += OnVideoFinish;
//    }

//    // This function is called when the video finishes playing
//    private void OnVideoFinish(VideoPlayer vp)
//    {
//        Debug.Log("Video finished. Restarting the game...");

//        // Call the RestartGame method
//        RestartGame();
//    }

//    IEnumerator ShowDelayScreen()
//    {
//        Debug.Log("Slider reached threshold. Preparing to show grid.");
//        yield return new WaitForSeconds(1); // Short delay before transition
//        PlayVideoThenCreateGrid();
//    }

//    void PlayVideoThenCreateGrid()
//    {
//        Panels[0].SetActive(false); // Hide idle panel (first panel)
//        // Skip panel 1 (the second panel) entirely
//        Panels[3].SetActive(true);
//        congratulationText.gameObject.SetActive(false);

//        if (handObject != null)
//        {
//            handObject.SetActive(false);
//        }

//        bodySourceView.StopDetection();

//        videoPlayer.Stop();
//        videoPlayer.clip = gridTransitionClip;
//        videoPlayer.Play();
//        videoPlayer.loopPointReached -= CreateGridAfterVideo;
//        videoPlayer.loopPointReached += CreateGridAfterVideo;
//    }

//    private void CreateGridAfterVideo(VideoPlayer vp)
//    {
//        Panels[2].SetActive(true);
//        Panels[3].SetActive(false);
//        // Video has finished, now create the grid
//        ChangeGameState(GameState.Running);
//        bodySourceView.StartDetection();
//        timerDial.SetActive(true);

//        if (handObject != null)
//        {
//            handObject.SetActive(true);
//        }

//        StartCoroutine(TimerCoroutine(gameDuration));
//        CreateGameGrid();
//        videoPlayer.loopPointReached -= CreateGridAfterVideo;
//    }

//    private void CreateGameGrid()
//    {
//        // Ensure gameSprites contains the sprites before passing them to CreateGameGrid
//        if (gameSprites == null || gameSprites.Length == 0)
//        {
//            Debug.LogError("gameSprites array is empty. Please assign sprites before creating the game grid.");
//            return;
//        }

//        // Create a new grid with actual game tiles
//        tileGrid.CreateGameGrid(gridSizeX, gridSizeY, imagePrefab, gameSprites);
//    }

//    private IEnumerator TimerCoroutine(float duration)
//    {
//        float timer = duration;

//        while (timer > 0)
//        {
//            //Debug.Log("Time left: " + timer + " seconds");
//            timerText.text = timer.ToString();
//            yield return new WaitForSeconds(1);
//            timer--;
//        }

//        Debug.Log("Timer finished!");
//        GameOver(true); // Pass true to indicate time over
//    }

//    private void DestroyBodyObjects()
//    {
//        foreach (var body in bodySourceView.GetBodies())
//        {
//            if (body != null)
//            {
//                Destroy(body);
//            }
//        }
//    }

//    private void DestroyHandObject()
//    {
//        GameObject handCircle = bodySourceView.GetHandCircle();
//        if (handCircle != null)
//        {
//            Destroy(handCircle);
//        }
//    }

//    void RestartGame()
//    {
//        string currentScene = SceneManager.GetActiveScene().name;
//        SceneManager.LoadScene(currentScene);
//    }
//    #endregion
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;
//using static LorealBodySourceView;
//using TMPro;

//public class LorealManager : MonoBehaviour
//{
//    #region "Initialization"

//    private LorealBodySourceView bodySourceView;
//    private LorealGridManager tileGrid;

//    public GameObject handObject;

//    [Header("Flags")]
//    public bool buttonClicked = false;
//    public bool sliderMoving = false;

//    [Header("Slider")]
//    public Slider gameSlider;
//    public float sliderSpeed = 1.0f;
//    private bool sliderTriggered = false;

//    [Header("Grid Sprites")]
//    public Sprite defaultImage;
//    public Sprite[] gameSprites;
//    public Sprite[] transitionSprites;

//    [Header("Variables")]
//    public int totalTiles;
//    public int changedTileCount = 0;
//    public int gridSizeX = 10;
//    public int gridSizeY = 10;
//    public float gameDuration = 60f;


//    [Header("Screen UI Reference")]
//    public GameObject[] Panels; //1. idle, 2. start, 3. main game, 4. end screen panel

//    public GameObject imagePrefab;

//    [Header("Timer Reference")]
//    public GameObject timerDial;
//    public Text timerText;

//    [Header("Video Reference")]
//    public VideoPlayer videoPlayer;
//    public RawImage rawImage;
//    public TMP_Text gameOverText; // Assign this in the Unity Inspector
//    #endregion

//    #region "Start, Update"
//    private void Start()
//    {
//        if (bodySourceView == null)
//        {
//            bodySourceView = FindAnyObjectByType<LorealBodySourceView>();
//            bodySourceView.currentGameState = GameState.Idle;
//        }
//        if (tileGrid == null)
//        {
//            tileGrid = FindAnyObjectByType<LorealGridManager>();
//        }
//        bodySourceView.StartDetection();
//        foreach (GameObject screen in Panels)
//        {
//            screen.SetActive(false);
//        }

//        Panels[0].SetActive(true); // setting idle panel visible at game start
//        timerDial.SetActive(false);
//        totalTiles = gridSizeX * gridSizeY; // calculating total number of tiles
//        buttons[1].interactable = false;
//        buttons[0].interactable = false;

//        // Initialize slider if it exists
//        if (gameSlider != null)
//        {
//            gameSlider.value = 0;
//            gameSlider.onValueChanged.AddListener(SliderValueChanged);
//        }

//        StartCoroutine(EnableButtonAfterDealy());
//    }

//    IEnumerator EnableButtonAfterDealy()
//    {
//        yield return new WaitForSeconds(1f);
//        buttons[0].interactable = true;
//    }

//    public void ChangeGameState(GameState newState)
//    {
//        if (bodySourceView != null)
//        {
//            bodySourceView.currentGameState = newState;
//        }
//    }

//    void Update()
//    {
//        if (bodySourceView != null)
//        {
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//                RestartGame();
//            }
//            if (handObject == null)
//            {
//                handObject = bodySourceView.GetHandCircle();
//            }

//            if (handObject != null)
//            {
//                Vector3 worldPosition = handObject.transform.position;
//                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

//                PointerEventData pointerData = new PointerEventData(EventSystem.current)
//                {
//                    position = new Vector2(screenPosition.x, screenPosition.y)
//                };

//                List<RaycastResult> raycastResults = new List<RaycastResult>();
//                EventSystem.current.RaycastAll(pointerData, raycastResults);

//                if (!buttonClicked && !sliderMoving)
//                {
//                    foreach (var result in raycastResults)
//                    {
//                        var button = result.gameObject.GetComponent<Button>();
//                        if (button != null)
//                        {
//                            Debug.LogError("Hand is hovering over the button: " + result.gameObject.name);

//                            if (result.gameObject.name == "DetectButton")
//                            {
//                                ShowSecondScreen();
//                            }
//                            else if (result.gameObject.name == "StartButton")
//                            {
//                                TriggerSlider();
//                                break;
//                            }
//                        }
//                    }
//                }

//                // Move slider if triggered
//                if (sliderTriggered && gameSlider != null && Panels[1].activeSelf)
//                {
//                    MoveSlider();
//                }

//                float handRadius = .5f;
//                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, handRadius);

//                Debug.DrawRay(worldPosition, Vector3.right * 50f, Color.red);
//                Debug.DrawRay(worldPosition, Vector3.up * 50f, Color.red);

//                foreach (var hitCollider in hitColliders)
//                {
//                    if (hitCollider.CompareTag("Tile"))
//                    {
//                        ChangeTileInteraction(hitCollider.gameObject);
//                    }
//                }
//            }
//            else
//            {
//                Debug.Log("No hand object detected.");
//            }
//        }
//    }
//    #endregion

//    #region "Slider Functions"
//    public void TriggerSlider()
//    {
//        if (gameSlider != null && !sliderMoving)
//        {
//            sliderTriggered = true;
//            sliderMoving = true;
//            gameSlider.value = 0;
//            Debug.Log("Slider triggered");
//        }
//    }

//    private void MoveSlider()
//    {
//        if (gameSlider.value < 1.0f)
//        {
//            gameSlider.value += Time.deltaTime * sliderSpeed;

//            // Visual feedback as slider moves (optional)
//            if (gameSlider.value >= 0.25f && gameSlider.value < 0.26f ||
//                gameSlider.value >= 0.5f && gameSlider.value < 0.51f ||
//                gameSlider.value >= 0.75f && gameSlider.value < 0.76f)
//            {
//                // Optional: Add visual/audio feedback at certain thresholds
//                Debug.Log($"Slider at {gameSlider.value * 100}%");
//            }
//        }
//        else
//        {
//            // Slider reached the end
//            sliderTriggered = false;
//            sliderMoving = false;
//            buttonClicked = true;
//            StartCoroutine(ShowDelayScreen());
//        }
//    }

//    private void SliderValueChanged(float value)
//    {
//        // You can add additional effects or logic based on the slider value
//        if (value >= 1.0f && sliderTriggered)
//        {
//            Debug.Log("Slider reached maximum!");
//        }
//    }
//    #endregion

//    #region Functions
//    void ChangeTileInteraction(GameObject tile)
//    {
//        if (tile.GetComponent<Tile>().isChanged)
//        {
//            Debug.Log("Tile already changed: " + tile.name);
//            return;
//        }

//        Image spriteRenderer = tile.GetComponent<Image>();

//        if (spriteRenderer == null)
//        {
//            Debug.LogError("No Image component found on tile: " + tile.name);
//            return;
//        }

//        if (defaultImage == null)
//        {
//            Debug.LogError("Default image is not set.");
//            return;
//        }

//        Sprite currentSprite = spriteRenderer.sprite;
//        int spriteIndex = System.Array.IndexOf(gameSprites, currentSprite);

//        if (spriteIndex < 0 || spriteIndex >= transitionSprites.Length)
//        {
//            Debug.LogError("Sprite index is out of bounds or not found in gameSprites array.");
//            return;
//        }

//        StartCoroutine(ChangeTileWithTransition(spriteRenderer, tile, spriteIndex));

//        tile.GetComponent<Tile>().isChanged = true;
//        changedTileCount++;

//        Debug.Log($"Changed tile: {tile.name} | Total changed: {changedTileCount}/{totalTiles}");

//        if (changedTileCount >= totalTiles)
//        {
//            Debug.Log("All tiles have been changed!");
//            GameOver(false); // Pass false to indicate all tiles changed
//        }
//    }

//    IEnumerator ChangeTileWithTransition(Image spriteRenderer, GameObject tile, int spriteIndex)
//    {
//        Sprite transitionImage = transitionSprites[spriteIndex];

//        // Fade out the first image
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 0f, 1f, 0.3f)); // Fade-out with normal brightness

//        // Change to transition image
//        spriteRenderer.sprite = transitionImage;

//        // Fade in the new image with increased brightness
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 0f, 1f, 1.5f, 0.3f)); // 1.5x brightness effect

//        // Gradually restore brightness to normal
//        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 1f, 1f, 0.3f)); // Restore to normal brightness

//        Debug.Log($"Tile transition complete for: {tile.name}");
//    }

//    // Helper function to fade image with brightness effect
//    IEnumerator FadeImageWithBrightness(Image image, float startAlpha, float endAlpha, float brightnessFactor, float duration)
//    {
//        float elapsedTime = 0f;
//        Color originalColor = image.color;

//        while (elapsedTime < duration)
//        {
//            elapsedTime += Time.deltaTime;
//            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
//            float brightness = Mathf.Lerp(1f, brightnessFactor, elapsedTime / duration); // Increase brightness

//            image.color = new Color(
//                Mathf.Clamp01(originalColor.r * brightness),
//                Mathf.Clamp01(originalColor.g * brightness),
//                Mathf.Clamp01(originalColor.b * brightness),
//                alpha
//            );

//            yield return null;
//        }

//        // Ensure final values are set
//        image.color = new Color(
//            Mathf.Clamp01(originalColor.r * brightnessFactor),
//            Mathf.Clamp01(originalColor.g * brightnessFactor),
//            Mathf.Clamp01(originalColor.b * brightnessFactor),
//            endAlpha
//        );
//    }
//    void ShowSecondScreen()
//    {
//        Panels[0].SetActive(false);
//        Panels[1].SetActive(true);
//        buttons[1].interactable = true;

//        // Reset slider
//        if (gameSlider != null)
//        {
//            gameSlider.value = 0;
//            sliderTriggered = false;
//            sliderMoving = false;
//        }
//    }

//    public VideoClip congratulationClip;
//    public VideoClip timeOverClip;
//    public VideoClip gridTransitionClip;
//    public Button[] buttons;
//    public TMP_Text congratulationText;

//    public void GameOver(bool isTimeOver)
//    {
//        Debug.Log("Game Over!");

//        DestroyBodyObjects();
//        DestroyHandObject();
//        bodySourceView.StopDetection();

//        Panels[3].SetActive(true);
//        congratulationText.gameObject.SetActive(true);

//        Panels[2].SetActive(false);
//        timerDial.SetActive(false);

//        videoPlayer.Stop();

//        if (isTimeOver)
//        {
//            Debug.Log("Game Over due to time running out.");
//            videoPlayer.clip = timeOverClip; // Assign appropriate clip in the inspector
//            gameOverText.text = "Better luck next time!"; // Display message for losing
//        }
//        else
//        {
//            Debug.Log("Game Over because all tiles were changed.");
//            videoPlayer.clip = congratulationClip; // Assign appropriate clip in the inspector
//            gameOverText.text = "Congratulations! You won!"; // Display message for winning
//        }

//        videoPlayer.Play();
//        videoPlayer.loopPointReached += OnVideoFinish;
//    }


//    // This function is called when the video finishes playing
//    private void OnVideoFinish(VideoPlayer vp)
//    {
//        Debug.Log("Video finished. Restarting the game...");

//        // Call the RestartGame method
//        RestartGame();
//    }

//    IEnumerator ShowDelayScreen()
//    {
//        Debug.Log("calling here");
//        yield return new WaitForSeconds(3);
//        Debug.Log("calling here also");
//        PlayVideoThenCreateGrid();
//    }
//    void PlayVideoThenCreateGrid()
//    {
//        Panels[1].SetActive(false);
//        Panels[3].SetActive(true);
//        congratulationText.gameObject.SetActive(false);
//        handObject.SetActive(false);
//        bodySourceView.StopDetection();

//        videoPlayer.Stop();
//        videoPlayer.clip = gridTransitionClip;
//        videoPlayer.Play();
//        videoPlayer.loopPointReached -= CreateGridAfterVideo;
//        videoPlayer.loopPointReached += CreateGridAfterVideo;
//    }

//    private void CreateGridAfterVideo(VideoPlayer vp)
//    {
//        Panels[2].SetActive(true);
//        Panels[3].SetActive(false);
//        // Video has finished, now create the grid
//        ChangeGameState(GameState.Running);
//        bodySourceView.StartDetection();
//        timerDial.SetActive(true);
//        handObject.SetActive(true);
//        StartCoroutine(TimerCoroutine(gameDuration));
//        CreateGameGrid();
//        videoPlayer.loopPointReached -= CreateGridAfterVideo;
//    }

//    private void CreateGameGrid()
//    {
//        // Ensure gameSprites contains the sprites before passing them to CreateGameGrid
//        if (gameSprites == null || gameSprites.Length == 0)
//        {
//            Debug.LogError("gameSprites array is empty. Please assign sprites before creating the game grid.");
//            return;
//        }

//        // Create a new grid with actual game tiles
//        tileGrid.CreateGameGrid(gridSizeX, gridSizeY, imagePrefab, gameSprites);
//    }

//    private IEnumerator TimerCoroutine(float duration)
//    {
//        float timer = duration;

//        while (timer > 0)
//        {
//            //Debug.Log("Time left: " + timer + " seconds");
//            timerText.text = timer.ToString();
//            yield return new WaitForSeconds(1);
//            timer--;
//        }

//        Debug.Log("Timer finished!");
//        GameOver(true); // Pass true to indicate time over
//    }

//    private void DestroyBodyObjects()
//    {
//        foreach (var body in bodySourceView.GetBodies())
//        {
//            if (body != null)
//            {
//                Destroy(body);
//            }
//        }
//    }

//    private void DestroyHandObject()
//    {
//        GameObject handCircle = bodySourceView.GetHandCircle();
//        if (handCircle != null)
//        {
//            Destroy(handCircle);
//        }
//    }

//    void RestartGame()
//    {
//        string currentScene = SceneManager.GetActiveScene().name;
//        SceneManager.LoadScene(currentScene);
//    }
//    #endregion
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using static LorealBodySourceView;
using TMPro;

public class LorealManager : MonoBehaviour
{
    #region "Initialization"

    private LorealBodySourceView bodySourceView;
    private LorealGridManager tileGrid;

    public GameObject handObject;

    [Header("Flags")]
    public bool buttonClicked = false;
    public bool sliderMoving = false;

    [Header("Slider")]
    public Slider gameSlider;
    public float sliderDuration = 3.0f; // Time in seconds for slider to complete
    private bool sliderTriggered = false;
    public float transitionDelay = 2.0f; // Wait time after slider completes

    [Header("Grid Sprites")]
    public Sprite defaultImage;
    public Sprite[] gameSprites;
    public Sprite[] transitionSprites;

    [Header("Variables")]
    public int totalTiles;
    public int changedTileCount = 0;
    //public int gridSizeX = 10;
    //public int gridSizeY = 10;
    public int gridSizeX = 11;
    public int gridSizeY = 9;
    public float gameDuration = 60f;


    [Header("Screen UI Reference")]
    public GameObject[] Panels; //1. idle, 2. start, 3. main game, 4. end screen panel

    public GameObject imagePrefab;

    [Header("Timer Reference")]
    public GameObject timerDial;
    public Text timerText;

    [Header("Video Reference")]
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    public TMP_Text gameOverText; // Assign this in the Unity Inspector
    #endregion

    #region "Start, Update"
    private void Start()
    {
        if (bodySourceView == null)
        {
            bodySourceView = FindAnyObjectByType<LorealBodySourceView>();
            bodySourceView.currentGameState = GameState.Idle;
        }
        if (tileGrid == null)
        {
            tileGrid = FindAnyObjectByType<LorealGridManager>();
        }
        bodySourceView.StartDetection();
        foreach (GameObject screen in Panels)
        {
            screen.SetActive(false);
        }

        Panels[0].SetActive(true); // setting idle panel visible at game start
        timerDial.SetActive(false);
        totalTiles = gridSizeX * gridSizeY; // calculating total number of tiles
        buttons[1].interactable = false;
        buttons[0].interactable = false;

        // Initialize slider if it exists
        if (gameSlider != null)
        {
            gameSlider.value = 0;
            gameSlider.onValueChanged.AddListener(SliderValueChanged);
        }

        StartCoroutine(EnableButtonAfterDealy());
    }

    IEnumerator EnableButtonAfterDealy()
    {
        yield return new WaitForSeconds(1f);
        buttons[0].interactable = true;
    }

    public void ChangeGameState(GameState newState)
    {
        if (bodySourceView != null)
        {
            bodySourceView.currentGameState = newState;
        }
    }

    void Update()
    {
        if (bodySourceView != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            if (handObject == null)
            {
                handObject = bodySourceView.GetHandCircle();
            }

            if (handObject != null)
            {
                Vector3 worldPosition = handObject.transform.position;
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = new Vector2(screenPosition.x, screenPosition.y)
                };

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                if (!buttonClicked && !sliderMoving)
                {
                    foreach (var result in raycastResults)
                    {
                        var button = result.gameObject.GetComponent<Button>();
                        if (button != null)
                        {
                            Debug.Log("Hand is hovering over the button: " + result.gameObject.name);

                            if (result.gameObject.name == "DetectButton")
                            {
                                ShowSecondScreen();
                            }
                            else if (result.gameObject.name == "StartButton")
                            {
                                TriggerSlider();
                                break;
                            }
                        }
                    }
                }

                // Move slider if triggered
                if (sliderTriggered && gameSlider != null && Panels[1].activeSelf)
                {
                    MoveSlider();
                }

                float handRadius = .5f;
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, handRadius);

                Debug.DrawRay(worldPosition, Vector3.right * 50f, Color.red);
                Debug.DrawRay(worldPosition, Vector3.up * 50f, Color.red);

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Tile"))
                    {
                        ChangeTileInteraction(hitCollider.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("No hand object detected.");
            }
        }
    }
    #endregion

    #region "Slider Functions"
    public void TriggerSlider()
    {
        if (gameSlider != null && !sliderMoving)
        {
            sliderTriggered = true;
            sliderMoving = true;
            gameSlider.value = 0;
            Debug.Log("Slider triggered");
        }
    }

    private void MoveSlider()
    {
        if (gameSlider.value < 1.0f)
        {
            // Calculate speed based on duration (1.0 / duration = increment per second)
            float increment = Time.deltaTime / sliderDuration;
            gameSlider.value += increment;

            // Visual feedback as slider moves (optional)
            if (gameSlider.value >= 0.25f && gameSlider.value < 0.25f + increment ||
                gameSlider.value >= 0.5f && gameSlider.value < 0.5f + increment ||
                gameSlider.value >= 0.75f && gameSlider.value < 0.75f + increment)
            {
                // Optional: Add visual/audio feedback at certain thresholds
                Debug.Log($"Slider at {gameSlider.value * 100}%");
            }
        }
        else
        {
            // Slider reached the end
            sliderTriggered = false;
            sliderMoving = false;
            buttonClicked = true;

            // Use the transition delay
            StartCoroutine(DelayedTransition());
        }
    }

    private IEnumerator DelayedTransition()
    {
        Debug.Log($"Slider complete, waiting {transitionDelay} seconds before transition");
        yield return new WaitForSeconds(transitionDelay);
        StartCoroutine(ShowDelayScreen());
    }

    private void SliderValueChanged(float value)
    {
        // You can add additional effects or logic based on the slider value
        if (value >= 1.0f && sliderTriggered)
        {
            Debug.Log("Slider reached maximum!");
        }
    }
    #endregion

    #region Functions
    void ChangeTileInteraction(GameObject tile)
    {
        if (tile.GetComponent<Tile>().isChanged)
        {
            Debug.Log("Tile already changed: " + tile.name);
            return;
        }

        Image spriteRenderer = tile.GetComponent<Image>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No Image component found on tile: " + tile.name);
            return;
        }

        if (defaultImage == null)
        {
            Debug.LogError("Default image is not set.");
            return;
        }

        Sprite currentSprite = spriteRenderer.sprite;
        int spriteIndex = System.Array.IndexOf(gameSprites, currentSprite);

        if (spriteIndex < 0 || spriteIndex >= transitionSprites.Length)
        {
            Debug.LogError("Sprite index is out of bounds or not found in gameSprites array.");
            return;
        }

        StartCoroutine(ChangeTileWithTransition(spriteRenderer, tile, spriteIndex));

        tile.GetComponent<Tile>().isChanged = true;
        changedTileCount++;

        Debug.Log($"Changed tile: {tile.name} | Total changed: {changedTileCount}/{totalTiles}");

        if (changedTileCount >= totalTiles)
        {
            Debug.Log("All tiles have been changed!");
            GameOver(false); // Pass false to indicate all tiles changed
        }
    }

    IEnumerator ChangeTileWithTransition(Image spriteRenderer, GameObject tile, int spriteIndex)
    {
        Sprite transitionImage = transitionSprites[spriteIndex];

        // Fade out the first image
        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 0f, 1f, 0.3f)); // Fade-out with normal brightness

        // Change to transition image
        spriteRenderer.sprite = transitionImage;

        // Fade in the new image with increased brightness
        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 0f, 1f, 1.5f, 0.3f)); // 1.5x brightness effect

        // Gradually restore brightness to normal
        yield return StartCoroutine(FadeImageWithBrightness(spriteRenderer, 1f, 1f, 1f, 0.3f)); // Restore to normal brightness

        Debug.Log($"Tile transition complete for: {tile.name}");
    }

    // Helper function to fade image with brightness effect
    IEnumerator FadeImageWithBrightness(Image image, float startAlpha, float endAlpha, float brightnessFactor, float duration)
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            float brightness = Mathf.Lerp(1f, brightnessFactor, elapsedTime / duration); // Increase brightness

            image.color = new Color(
                Mathf.Clamp01(originalColor.r * brightness),
                Mathf.Clamp01(originalColor.g * brightness),
                Mathf.Clamp01(originalColor.b * brightness),
                alpha
            );

            yield return null;
        }

        // Ensure final values are set
        image.color = new Color(
            Mathf.Clamp01(originalColor.r * brightnessFactor),
            Mathf.Clamp01(originalColor.g * brightnessFactor),
            Mathf.Clamp01(originalColor.b * brightnessFactor),
            endAlpha
        );
    }
    void ShowSecondScreen()
    {
        Panels[0].SetActive(false);
        Panels[1].SetActive(true);
        buttons[1].interactable = true;

        // Reset slider
        if (gameSlider != null)
        {
            gameSlider.value = 0;
            sliderTriggered = false;
            sliderMoving = false;
        }
    }

    public VideoClip congratulationClip;
    public VideoClip timeOverClip;
    public VideoClip gridTransitionClip;
    public Button[] buttons;
    public TMP_Text congratulationText;

    //public void GameOver(bool isTimeOver)
    //{
    //    Debug.Log("Game Over!");

    //    DestroyBodyObjects();
    //    DestroyHandObject();
    //    bodySourceView.StopDetection();

    //    Panels[3].SetActive(true);
    //    congratulationText.gameObject.SetActive(true);

    //    Panels[2].SetActive(false);
    //    timerDial.SetActive(false);

    //    videoPlayer.Stop();

    //    if (isTimeOver)
    //    {
    //        Debug.Log("Game Over due to time running out.");
    //        videoPlayer.clip = timeOverClip; // Assign appropriate clip in the inspector
    //        gameOverText.text = "Better luck next time!"; // Display message for losing
    //    }
    //    else
    //    {
    //        Debug.Log("Game Over because all tiles were changed.");
    //        videoPlayer.clip = congratulationClip; // Assign appropriate clip in the inspector
    //        gameOverText.text = "Congratulations! You won!"; // Display message for winning
    //    }

    //    videoPlayer.Play();
    //    videoPlayer.loopPointReached += OnVideoFinish;
    //}
    //public void GameOver(bool isTimeOver)
    //{
    //    Debug.Log("Game Over!");

    //    DestroyBodyObjects();
    //    DestroyHandObject();
    //    bodySourceView.StopDetection();

    //    Panels[3].SetActive(true);
    //    congratulationText.gameObject.SetActive(true);

    //    Panels[2].SetActive(false);
    //    timerDial.SetActive(false);

    //    // Ensure no multiple video playbacks
    //    if (videoPlayer.isPlaying)
    //    {
    //        videoPlayer.Stop();
    //    }

    //    if (isTimeOver)
    //    {
    //        Debug.Log("Game Over due to time running out.");
    //        videoPlayer.clip = timeOverClip; // Assign appropriate clip in the inspector
    //        gameOverText.text = "Better luck next time!";
    //    }
    //    else
    //    {
    //        Debug.Log("Game Over because all tiles were changed.");
    //        videoPlayer.clip = congratulationClip; // Assign appropriate clip in the inspector
    //        gameOverText.text = "Congratulations! You won!";
    //    }

    //    videoPlayer.Play();
    //    videoPlayer.loopPointReached -= OnVideoFinish; // Prevent multiple subscriptions
    //    videoPlayer.loopPointReached += OnVideoFinish;
    //}

    private bool isGameOver = false; // Add this at the class level

    public void GameOver(bool isTimeOver)
    {
        if (isGameOver) return; // Prevent multiple calls
        isGameOver = true; // Mark game as ended

        Debug.Log("Game Over!");

        DestroyBodyObjects();
        DestroyHandObject();
        bodySourceView.StopDetection();

        Panels[3].SetActive(true);
        congratulationText.gameObject.SetActive(true);

        Panels[2].SetActive(false);
        timerDial.SetActive(false);

        // Ensure no multiple video playbacks
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        if (isTimeOver)
        {
            Debug.Log("Game Over due to time running out.");
            videoPlayer.clip = timeOverClip;
            gameOverText.text = "Better luck next time!";
        }
        else
        {
            Debug.Log("Game Over because all tiles were changed.");
            videoPlayer.clip = congratulationClip;
            gameOverText.text = "Congratulations! You won!";
        }

        videoPlayer.Play();
        videoPlayer.loopPointReached -= OnVideoFinish; // Prevent multiple event subscriptions
        videoPlayer.loopPointReached += OnVideoFinish;
    }
    // This function is called when the video finishes playing
    private void OnVideoFinish(VideoPlayer vp)
    {
        Debug.Log("Video finished. Restarting the game...");

        // Call the RestartGame method
        RestartGame();
    }

    IEnumerator ShowDelayScreen()
    {
        Debug.Log("calling here");
        yield return new WaitForSeconds(3);
        Debug.Log("calling here also");
        PlayVideoThenCreateGrid();
    }
    void PlayVideoThenCreateGrid()
    {
        Panels[1].SetActive(false);
        Panels[3].SetActive(true);
        congratulationText.gameObject.SetActive(false);
        handObject.SetActive(false);
        bodySourceView.StopDetection();

        videoPlayer.Stop();
        videoPlayer.clip = gridTransitionClip;
        videoPlayer.Play();
        videoPlayer.loopPointReached -= CreateGridAfterVideo;
        videoPlayer.loopPointReached += CreateGridAfterVideo;
    }

    private void CreateGridAfterVideo(VideoPlayer vp)
    {
        Panels[2].SetActive(true);
        Panels[3].SetActive(false);
        // Video has finished, now create the grid
        ChangeGameState(GameState.Running);
        bodySourceView.StartDetection();
        timerDial.SetActive(true);
        handObject.SetActive(true);
        StartCoroutine(TimerCoroutine(gameDuration));
        CreateGameGrid();
        videoPlayer.loopPointReached -= CreateGridAfterVideo;
    }

    private void CreateGameGrid()
    {
        // Ensure gameSprites contains the sprites before passing them to CreateGameGrid
        if (gameSprites == null || gameSprites.Length == 0)
        {
            Debug.LogError("gameSprites array is empty. Please assign sprites before creating the game grid.");
            return;
        }

        // Create a new grid with actual game tiles
       tileGrid.CreateGameGrid(gridSizeX, gridSizeY, imagePrefab, gameSprites);
     //  tileGrid.CreateGameGrid( imagePrefab, gameSprites);
    }

    private IEnumerator TimerCoroutine(float duration)
    {
        float timer = duration;

        while (timer > 0)
        {
            //Debug.Log("Time left: " + timer + " seconds");
            timerText.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }

        Debug.Log("Timer finished!");
        GameOver(true); // Pass true to indicate time over
    }

    private void DestroyBodyObjects()
    {
        foreach (var body in bodySourceView.GetBodies())
        {
            if (body != null)
            {
                Destroy(body);
            }
        }
    }

    private void DestroyHandObject()
    {
        GameObject handCircle = bodySourceView.GetHandCircle();
        if (handCircle != null)
        {
            Destroy(handCircle);
        }
    }

    void RestartGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    #endregion
}