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


    void Awake()
    {
        ModuleId = ModuleIdCounter++;
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
