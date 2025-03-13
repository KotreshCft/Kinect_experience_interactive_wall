using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using static LorealBodySourceView;
using TMPro;

public class LightManager : MonoBehaviour
{
    #region "Initialization"

    private LorealBodySourceView bodySourceView;
    private LorealGridManager tileGrid;

    public GameObject handObject;

    [Header("Flags")]
    public bool buttonClicked = false;

    [Header("Grid Sprites")]
    public Sprite defaultImage;
    public Sprite[] gameSprites;
    public Sprite[] transitionSprites;

    [Header("Variables")]
    public int totalTiles;
    public int changedTileCount = 0;
    public int gridSizeX = 10;
    public int gridSizeY = 10;
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

    }
    IEnumerator EnableButtonAfterDealy()
    {
        yield return new WaitForSeconds(3f);
        buttons[0].interactable = true;
    }

    public void ChangeGameState(GameState newState)
    {
        if (bodySourceView != null)
        {
            bodySourceView.currentGameState = newState;
        }
    }

    /*void Update()
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

                if (!buttonClicked)
                {
                    foreach (var result in raycastResults)
                    {
                        var button = result.gameObject.GetComponent<Button>();
                        if (button != null)
                        {
                            Debug.LogError("Hand is hovering over the button: " + result.gameObject.name);

                            if (result.gameObject.name == "DetectButton")
                            {
                                ShowSecondScreen();
                            }
                            else if (result.gameObject.name == "StartButton")
                            {
                                buttonClicked = true;
                                //PlayVideoThenCreateGrid();
                                Debug.Log("here");
                                StartCoroutine(ShowDelayScreen());
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No hand object detected.");
            }
        }
    }*/
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

                if (!buttonClicked)
                {
                    foreach (var result in raycastResults)
                    {
                        var button = result.gameObject.GetComponent<Button>();
                        if (button != null)
                        {
                            Debug.LogError("Hand is hovering over the button: " + result.gameObject.name);

                            if (result.gameObject.name == "DetectButton")
                            {
                                ShowSecondScreen();
                            }
                            else if (result.gameObject.name == "StartButton")
                            {
                                buttonClicked = true;
                                StartCoroutine(ShowDelayScreen());
                                break;
                            }
                        }
                    }
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
            GameOver();
        }
    }
    IEnumerator ChangeTileWithTransition(Image spriteRenderer, GameObject tile, int spriteIndex)
    {
        Sprite transitionImage = transitionSprites[spriteIndex];

        spriteRenderer.sprite = transitionImage;
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(1f);

        spriteRenderer.sprite = defaultImage;
        spriteRenderer.color = Color.white;

        Debug.Log($"Tile transition complete for: {tile.name}");
    }
    void ShowSecondScreen()
    {
        Panels[0].SetActive(false);
        Panels[1].SetActive(true);
        buttons[1].interactable = true;
    }

    public VideoClip congratulationClip;
    public VideoClip gridTransitionClip;
    public Button[] buttons;
    public TMP_Text congratulationText;
    public void GameOver()
    {
        Debug.Log("Game Over!");
        DestroyBodyObjects();
        DestroyHandObject();

        bodySourceView.StopDetection();

        Panels[3].SetActive(true);
        congratulationText.gameObject.SetActive(true);

        Panels[2].SetActive(false);
        timerDial.SetActive(false);

        videoPlayer.Stop();
        videoPlayer.clip = congratulationClip;
        videoPlayer.Play();
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
        // Create a new grid with actual game tiles
       // tileGrid.CreateGameGrid(imagePrefab, gameSprites);
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
        GameOver();
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
