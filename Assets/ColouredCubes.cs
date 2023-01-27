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

    private ScreenTextHandler _screen;

    void Awake()
    {
        _screen = new ScreenTextHandler(ScreenText);
        KMSelectable tempSelectable;

        ModuleId = ModuleIdCounter++;

        foreach (CubeScript cube in Cubes)
        {
            tempSelectable = cube.GetComponentInParent<KMSelectable>();
            tempSelectable.OnInteract += delegate () { ButtonPress(cube); return false; };
            tempSelectable.OnHighlight += delegate () { cube.EnableHighlightOverride(true);  _screen.SetText(cube.ColourAsName); };
            tempSelectable.OnHighlightEnded += delegate () { cube.EnableHighlightOverride(false); _screen.SetText(""); };
        }
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    void ButtonPress(CubeScript cube) // Need to add a sound effect on selections.
    {
        cube.ChangeSize(Rnd.Range(0, 3));
        cube.ChangeColour(Rnd.Range(0, 3), Rnd.Range(0, 3), Rnd.Range(0, 3));
        _screen.SetText(cube.ColourAsName);
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

    private class ScreenTextHandler
    {
        private TextMesh _screenText;
        private string _textValue;
        private bool _isOverride = false;

        public ScreenTextHandler(TextMesh screenText)
        {
            _screenText = screenText;
        }

        public void SetText(string value)
        {
            _textValue = value;
            if (!_isOverride) _screenText.text = _textValue;
        }

        public void EnableOverride(string overrideText)
        {
            _screenText.text = overrideText;
            _isOverride = true;
        }

        public void DisableOverride()
        {
            _screenText.text = _textValue;
            _isOverride = false;
        }
    }
}
