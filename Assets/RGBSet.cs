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
    public KMSelectable ScreenButton;
    public TextMesh ScreenText;

    private ColouredCube[] _colouredCubes = new ColouredCube[9];

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
            light.OnInteract += delegate() { StageLightPress(light); return false; };
            light.OnHighlight += delegate() { ScreenText.text = light.name; };
            light.OnHighlightEnded += delegate() { ScreenText.text = ""; };
        }

        ScreenButton.OnInteract += delegate () { ScreenButtonPress(); return false; };
    }

    void ButtonPress(ColouredCube cube) // Temporary
    {
        int newRedValue = Rnd.Range(0, 3);
        int newGreenValue = Rnd.Range(0, 3);
        int newBlueValue = Rnd.Range(0, 3);

        float newScale = Rnd.Range(0, 3);
        StartCoroutine(cube.SetSize(newScale));
        StartCoroutine(cube.SetColourWithRGBValues(newRedValue, newGreenValue, newBlueValue));

        ScreenText.text = cube.ColourName;
    }

    void StageLightPress(KMSelectable light)
    {
        ScreenText.text = "HELLO!";
    }

    void ScreenButtonPress()
    {
        ScreenText.text = "AYO?? :O";
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
