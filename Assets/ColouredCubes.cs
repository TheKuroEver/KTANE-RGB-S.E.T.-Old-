using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class ColouredCubes : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;

    private static int ModuleIdCounter = 1;
    private int ModuleId;
    private bool ModuleSolved;

    public CubeScript[] Cubes;
    public TextMesh ScreenText;

    void Awake()
    {
        KMSelectable tempSelectable;

        ModuleId = ModuleIdCounter++;

        foreach (CubeScript cube in Cubes)
        {
            tempSelectable = cube.GetComponentInParent<KMSelectable>();
            tempSelectable.OnInteract += delegate () { ButtonPress(cube); return false; };
            tempSelectable.OnHighlight += delegate () { ScreenText.text = cube.ColourAsName; };
            tempSelectable.OnHighlightEnded += delegate () { ScreenText.text = ""; };
        }
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    void ButtonPress(CubeScript cube)
    {
        cube.ChangeSize(Rnd.Range(0, 3));
        cube.ChangeColour(Rnd.Range(0, 3), Rnd.Range(0, 3), Rnd.Range(0, 3));
        ScreenText.text = cube.ColourAsName;
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
