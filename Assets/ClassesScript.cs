using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using Rnd = UnityEngine.Random;

public class ColouredCube
{
    private KMSelectable _cube;
    private Color _cubeColour;
    private const float bigCubeSize = 0.03173903f;

    public KMSelectable Cube { get { return _cube; } }
    public string ColourName { get { return getColourName[_cubeColour]; } }

    // I know the way this is done is kinda stupid but I was just experimenting so let me have it.
    private Dictionary<Color, string> getColourName = new Dictionary<Color, string>()
    {
        { new Color(0, 0, 0), "Black"},
        { new Color(0, 0, 0.5f), "Indigo"},
        { new Color(0, 0, 1), "Blue"},
        { new Color(0, 0.5f, 0), "Forest"},
        { new Color(0, 0.5f, 0.5f), "Teal"},
        { new Color(0, 0.5f, 1), "Azure"},
        { new Color(0, 1, 0), "Green"},
        { new Color(0, 1, 0.5f), "Jade"},
        { new Color(0, 1, 1), "Cyan"},
        { new Color(0.5f, 0, 0), "Maroon"},
        { new Color(0.5f, 0, 0.5f), "Plum"},
        { new Color(0.5f, 0, 1), "Violet"},
        { new Color(0.5f, 0.5f, 0), "Olive"},
        { new Color(0.5f, 0.5f, 0.5f), "Grey"},
        { new Color(0.5f, 0.5f, 1), "Maya"},
        { new Color(0.5f, 1, 0), "Lime"},
        { new Color(0.5f, 1, 0.5f), "Mint"},
        { new Color(0.5f, 1, 1), "Aqua"},
        { new Color(1, 0, 0), "Red"},
        { new Color(1, 0, 0.5f), "Rose"},
        { new Color(1, 0, 1), "Magenta"},
        { new Color(1, 0.5f, 0), "Orange"},
        { new Color(1, 0.5f, 0.5f), "Salmon"},
        { new Color(1, 0.5f, 1), "Pink"},
        { new Color(1, 1, 0), "Yellow"},
        { new Color(1, 1, 0.5f), "Cream"},
        { new Color(1, 1, 1), "White"}
    };

    public ColouredCube(KMSelectable cube)
    {
        _cube = cube;
        _cubeColour = GetRandomTernaryColour();
        _cube.GetComponent<MeshRenderer>().material.color = _cubeColour;
    }

    Color GetRandomTernaryColour()
    {
        float redComponent = 0.5f * Rnd.Range(0, 3);
        float greenComponent = 0.5f * Rnd.Range(0, 3);
        float blueComponent = 0.5f * Rnd.Range(0, 3);

        return new Color(redComponent, greenComponent, blueComponent);
    }

    public IEnumerator SetColourWithRGBValues(int red, int green, int blue)
    {
        _cubeColour = new Color((float)red / 2, (float)green / 2, (float)blue / 2);
        Color currentColor = _cube.GetComponent<MeshRenderer>().material.color;
        float delta = 0;
        float transitionSeconds = 1;
        float redDifference = _cubeColour.r - currentColor.r;
        float greenDifference = _cubeColour.g - currentColor.g;
        float blueDifference = _cubeColour.b - currentColor.b;
        float colourDeltaMultiplier;

        while (delta < transitionSeconds)
        {
            delta += Time.deltaTime;
            colourDeltaMultiplier = (Time.deltaTime / transitionSeconds);
            _cube.GetComponent<MeshRenderer>().material.color = new Color(currentColor.r + colourDeltaMultiplier * redDifference, currentColor.g + colourDeltaMultiplier * greenDifference, currentColor.b + colourDeltaMultiplier * blueDifference);
            currentColor = _cube.GetComponent<MeshRenderer>().material.color;
            yield return null;
        }
        _cube.GetComponent<MeshRenderer>().material.color = _cubeColour;
    }

    public IEnumerator SetSize(float newSize)
    {
        float previousSize = _cube.transform.localScale.x;
        float delta = 0;
        float scaleSeconds = 1;
        float currentTransitionSize;

        newSize = bigCubeSize * (newSize * 0.25f + 0.5f);

        while (delta < scaleSeconds)
        {
            delta += Time.deltaTime;
            currentTransitionSize = Mathf.Min(1, delta / scaleSeconds) * (newSize - previousSize) + previousSize;
            _cube.transform.localScale = new Vector3(currentTransitionSize, currentTransitionSize, currentTransitionSize);
            yield return null;
        }
    }
}
