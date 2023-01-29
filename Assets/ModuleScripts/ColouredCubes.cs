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
    public KMSelectable ScreenButton;
    public CubeScript[] Cubes;
    public TextMesh ScreenText;

    private static int ModuleIdCounter = 1;
    private int ModuleId;
    private bool ModuleSolved;

    private ScreenTextHandler _screen;
    private int _stageNumber = 0;
    private int _numOfSelectedCubes = 0;
    private string[] _stageOneSETValues;
    private string[] _stageTwoSETValues;
    private string[] _stageThreeSETValues;
    private string[] _currentCorrectValues;
    private bool _allowButtonSelection = false;
    private bool _allowScreenSelection = true;
    private bool _displayingSizeChart = false;

    void Awake()
    {
        _screen = new ScreenTextHandler(ScreenText);
        KMSelectable tempSelectable;

        ModuleId = ModuleIdCounter++;

        foreach (CubeScript cube in Cubes)
        {
            tempSelectable = cube.GetComponentInParent<KMSelectable>();
            tempSelectable.OnInteract += delegate () { ButtonPress(cube); return false; };
            tempSelectable.OnHighlight += delegate () { cube.EnableHighlightOverride(true);  _screen.DisplayColourName(cube.ColourAsName); };
            tempSelectable.OnHighlightEnded += delegate () { cube.EnableHighlightOverride(false); _screen.EndColourNameDisplay(); };
        }

        ScreenButton.OnInteract += delegate () { ScreenPress(); return false; };

        _screen.EnableOverride("Start");
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void ButtonPress(CubeScript cube) // Need to add a sound effect on selections.
    {
        if (!_allowButtonSelection) return;

        if (_numOfSelectedCubes == 3 && !cube.IsSelected) return;

        HandleSelection(cube);

        if (_numOfSelectedCubes == 3)
        {
            if (_stageNumber == 1) HandleStageOne();
        }
    }

    void HandleSelection(CubeScript cube)
    {
        if (cube.IsSelected)
        {
            cube.EnableSelectionHighlight(false);
            _numOfSelectedCubes--;
            return;
        }

        cube.EnableSelectionHighlight(true);
        _numOfSelectedCubes++;
    }

    void HandleStageOne()
    {
        if (SubmissionCorrect())
        {
            _screen.EnableOverride("CORRECT!");
        }
        else
        {
            _screen.EnableOverride("INCORRECT!");
        }
    }

    bool SubmissionCorrect()
    {
        foreach (CubeScript cube in Cubes)
        {
            if (cube.IsSelected && !Array.Exists(_currentCorrectValues, element => element == cube.SETValue))
            {
                return false;
            }
        }

        return true;
    }

    void ScreenPress()
    {
        if (!_allowScreenSelection) return;

        if (_stageNumber == 0)
        {
            _stageNumber++;

            _stageOneSETValues = SETGenerator.GenerateSetList();
            _currentCorrectValues = SETGenerator.CorrectAnswers;
            StartCoroutine(StageOneAnimation());
        }
        else if (!_displayingSizeChart)
        {
            StartCoroutine(ShowSizeChart());
        }
        else if (_displayingSizeChart)
        {
            StartCoroutine(HideSizeChart());
        }
    }

    IEnumerator ShowSizeChart()
    {
        var sizeChart = new Dictionary<int, int>() { { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 1 }, { 4, 0 } };

        _screen.EnableOverride("...");
        _allowButtonSelection = false;
        _allowScreenSelection = false;
        _displayingSizeChart = true;

        foreach (CubeScript cube in Cubes)
        {
            cube.SetSelectionHiding(true);
            cube.ChangeColour(2, 1, 2);
            cube.ChangeSize(sizeChart[cube.Position[0] + cube.Position[1]]);
        }

        yield return null;
        while (CubesBusy()) yield return null;

        _screen.EnableOverride("Size Chart");
        _allowScreenSelection = true;
    }

    IEnumerator HideSizeChart()
    {
        _screen.DisableOverride();
        _displayingSizeChart = false;
        _allowScreenSelection = false;

        if (_stageNumber == 1)
        {
            StartCoroutine(StageOneAnimation());
        }

        yield return null;
        while (CubesBusy()) yield return null;

        _allowButtonSelection = true;
        _allowScreenSelection = true;
    }

    IEnumerator StageOneAnimation()
    {
        _screen.EnableOverride("...");
        _screen.SetText("Stage 1");

        foreach (CubeScript cube in Cubes)
        {
            cube.SetHiddenStatus(false);
            cube.SetSelectionHiding(false);
        }
        yield return null;

        while (CubesBusy()) yield return null;

        for (int i = 0; i < 9; i++)
        {
            Cubes[i].SetStateFromSETValues(_stageOneSETValues[i]);
        }
        yield return null;

        while (CubesBusy()) yield return null;

        _screen.DisableOverride();
        _allowButtonSelection = true;
    }

    bool CubesBusy()
    {
        foreach (CubeScript cube in Cubes)
        {
            if (cube.IsBusy)
            {
                return true;
            }
        }

        return false;
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
        private string _colourName;
        private bool _showingColourName = false;
        private bool _isOverride = false;

        public ScreenTextHandler(TextMesh screenText)
        {
            _screenText = screenText;
        }

        private void UpdateScreen()
        {
            if (_isOverride) return;
            else if (_showingColourName) _screenText.text = _colourName;
            else _screenText.text = _textValue;
        }

        public void SetText(string value)
        {
            _textValue = value;
            UpdateScreen();
        }

        public void DisplayColourName(string name)
        {
            _colourName = name;
            _showingColourName = true;
            UpdateScreen();
        }

        public void EndColourNameDisplay()
        {
            _showingColourName = false;
            UpdateScreen();
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
