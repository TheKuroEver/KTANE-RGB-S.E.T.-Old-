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

    private ColouredCube[] _colouredCubes = new ColouredCube[9];

    private const float bigCubeSize = 0.03173903f;

    void Awake() {
        ModuleId = ModuleIdCounter++;

        for (int i = 0; i < 9; i++)
        {
            _colouredCubes[i] = new ColouredCube(ColouredButtons[i]);
        }

        foreach (ColouredCube cube in _colouredCubes)
        {
            cube.Cube.OnInteract += delegate() { ButtonPress(cube); return false; };
            cube.Cube.OnHighlight += delegate() { ScreenText.text = cube.ColourName;};
            cube.Cube.OnHighlightEnded += delegate () { ScreenText.text = ""; };
        }

        foreach (KMSelectable light in StageLights)
        {
            light.OnInteract += delegate () { StageLightPress(light); return false; };
            light.OnHighlight += delegate () { ScreenText.text = light.name; };
            light.OnHighlightEnded += delegate () { ScreenText.text = ""; };
        }
    }

    void ButtonPress(ColouredCube cube)
    {
        int newRedValue = Rnd.Range(0, 3);
        int newGreenValue = Rnd.Range(0, 3);
        int newBlueValue = Rnd.Range(0, 3);

        float newScale = Rnd.Range(0, 3)*0.25f + 0.5f;
        StartCoroutine(SmoothCubeScale(cube.Cube, newScale));
        StartCoroutine(cube.SetColourWithRGBValues(newRedValue, newGreenValue, newBlueValue));
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

    void StageLightPress(KMSelectable light)
    {
        ScreenText.text = "HELLO!";
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
