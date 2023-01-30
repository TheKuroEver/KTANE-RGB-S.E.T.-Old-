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

    public KMSelectable RGBSetModule;
    public GameObject Cube;
    public ColouredCube[] ColouredCubes = new ColouredCube[9];
    public KMSelectable[] StageLights;
    public TextMesh ScreenText;

    void Awake() {
        ModuleId = ModuleIdCounter++;
        int counter = 0;    

        for (int row = -1; row < 2; row++)
        {
            for (int col = -1; col < 2; col++)
            {
                ColouredCubes[counter] = new ColouredCube(RGBSetModule, Instantiate(Cube, RGBSetModule.transform), new Vector3((float)row * 0.04f, 0, (float)col * 0.4f), GetRandomTernaryColour());
                ColouredCubes[counter].Button.GetComponent<KMSelectable>().Parent = RGBSetModule;
                RGBSetModule.GetComponent<KMSelectable>().Children[counter] = ColouredCubes[counter].Button;
                ColouredCubes[counter].Button.OnInteract += delegate () { ButtonPress(ColouredCubes[counter].Button); return false; };
            }
        }

        foreach (KMSelectable light in StageLights)
        {
            light.OnInteract += delegate () { StageLightPress(light); return false; };
        }
    }

    void ButtonPress(KMSelectable button)
    {
        float scale = Rnd.Range(0, 3)*0.25f + 0.5f;
        ScreenText.text = button.name;
        button.GetComponent<MeshRenderer>().material.color = GetRandomTernaryColour();
        button.GetComponent<Transform>().localScale = new Vector3(0.03173903f * scale, 0.03173903f * scale, 0.03173903f * scale);
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

public class ColouredCube
{
    private GameObject _cube;
    private Vector3 _position;

    public Vector3 Position { get { return _position; } }

    public KMSelectable Button { get { return _cube.GetComponent<KMSelectable>(); } }

    public ColouredCube(KMSelectable module, GameObject cube, Vector3 position, Color colour)
    {
        _cube = cube;
        _position = position;

        _cube.GetComponent<KMSelectable>().Parent = module;
        _cube.GetComponent<Transform>().localPosition = _position;
        _cube.GetComponent<MeshRenderer>().material.color = colour;
    }

    private void CubePress()
    {
        return;
    }
}