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

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    public KMSelectable[] ColouredButtons;
    public KMSelectable[] StageLights;
    public TextMesh ScreenText;

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
        ScreenText.text = button.name;
        button.GetComponent<MeshRenderer>().material.color = GetRandomTernaryColour();
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
