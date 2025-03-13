using UnityEngine;
using UnityEngine.UI;

public class GridWaveTransition : MonoBehaviour
{
    public Image targetImage; // The UI image to apply the transition
    public int rows = 8; // Updated to 8 rows
    public int columns = 12; // Updated to 12 columns
    public float waveSpeed = 2.0f; // Speed of the wave animation
    public float waveAmplitude = 0.5f; // Amplitude of the wave effect
    public float transitionDuration = 2.0f; // How long the transition lasts

    private Material transitionMaterial;
    private float timeElapsed = 0f;

    void Start()
    {
        // Create a material instance for the transition shader
        transitionMaterial = new Material(Shader.Find("Custom/GridWaveShader"));
        targetImage.material = transitionMaterial;

        // Initialize the grid size
        transitionMaterial.SetInt("_Rows", rows);
        transitionMaterial.SetInt("_Columns", columns);
    }

    void Update()
    {
        // Update the elapsed time
        timeElapsed += Time.deltaTime;

        // Send time and parameters to the shader
        transitionMaterial.SetFloat("_CustomTime", timeElapsed * waveSpeed);
        transitionMaterial.SetFloat("_Amplitude", waveAmplitude);

        // Stop transition after the specified duration
        if (timeElapsed > transitionDuration)
        {
            enabled = false; // End the transition after the duration
        }
    }
}