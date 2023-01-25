using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class RGBSet : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;

    private static int ModuleIdCounter = 1;
    private int ModuleId;
    private bool ModuleSolved;

    public KMSelectable[] ColouredButtons;
    public KMSelectable[] StageLights;
    public TextMesh ScreenText;

    private const float bigCubeSize = 0.03173903f;

    void Awake() {
        ModuleId = ModuleIdCounter++;

        foreach (KMSelectable button in ColouredButtons)
        {
            button.OnInteract += delegate() { ButtonPress(button); return false; };
        }

        foreach (KMSelectable light in StageLights)
        {
            light.OnInteract += delegate () { StageLightPress(light); return false; };
        }
    }

    void ButtonPress(KMSelectable button)
    {
        float newScale = Rnd.Range(0, 3)*0.25f + 0.5f;
        ScreenText.text = button.name;
        StartCoroutine(SmoothCubeScale(button, newScale));
        StartCoroutine(SmoothCubeColourTransition(button, GetRandomTernaryColour()));
    }

    IEnumerator SmoothCubeScale(KMSelectable cube, float newScale)
    {
        float delta = 0;
        float scaleSeconds = 1;
        float scaleDifference = bigCubeSize * newScale - cube.transform.localScale.x;
        float sizeDelta;

        while (delta < scaleSeconds)
        {
            delta += Time.deltaTime;
            sizeDelta = (Time.deltaTime / scaleSeconds) * scaleDifference;
            cube.transform.localScale += new Vector3(sizeDelta, sizeDelta, sizeDelta);
            yield return null;
        }
    }

    IEnumerator SmoothCubeColourTransition(KMSelectable cube, Color newColour)
    {
        Color currentColor = cube.GetComponent<MeshRenderer>().material.color;
        float delta = 0;
        float transitionSeconds = 1;
        float redDifference = newColour.r - currentColor.r;
        float greenDifference = newColour.g - currentColor.g;
        float blueDifference = newColour.b - currentColor.b;
        float colourDeltaMultiplier;

        while (delta < transitionSeconds)
        {
            delta += Time.deltaTime;
            colourDeltaMultiplier = (Time.deltaTime / transitionSeconds);
            cube.GetComponent<MeshRenderer>().material.color = new Color(currentColor.r + colourDeltaMultiplier * redDifference, currentColor.g + colourDeltaMultiplier * greenDifference, currentColor.b + colourDeltaMultiplier * blueDifference);
            currentColor = cube.GetComponent<MeshRenderer>().material.color;
            yield return null;
        }
    }

    void StageLightPress(KMSelectable light)
    {
        ScreenText.text = light.name;
    }

    Color GetRandomTernaryColour()
    {
        float redComponent = 0.5f * Rnd.Range(0, 3);
        float greenComponent = 0.5f * Rnd.Range(0, 3);
        float blueComponent = 0.5f * Rnd.Range(0, 3);

        return new Color(redComponent, greenComponent, blueComponent);
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
