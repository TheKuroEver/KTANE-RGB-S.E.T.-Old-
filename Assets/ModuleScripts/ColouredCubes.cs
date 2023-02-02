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
    private string[] _stageOneCorrectValues;
    private string[] _stageTwoCorrectValues;
    private string[] _stageThreeCorrectValues;
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

        StartCoroutine(StageTwoAnimation());
    }

    bool SubmissionCorrect()
    {
        foreach (CubeScript cube in Cubes)
        {
            if (cube.IsSelected && !Array.Exists(_stageOneCorrectValues, element => element == cube.SETValue))
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
            ReorderCubes();
            _stageNumber++;

            _stageOneSETValues = SETGenerator.GenerateSetList();
            _stageOneCorrectValues = SETGenerator.CorrectAnswers;
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

    // This is my first experience with coroutines (also only my second mod) so the code is bad. Sorry. Not as bad as The Cipher Ever though.
    // In hindsight, I see so many things I could have written better; maybe I'll come back to clean it up at some point!
    IEnumerator ShowSizeChart()
    {
        var sizeChart = new Dictionary<int, int>() { { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 } };

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

        do { yield return null; } while (CubesBusy());

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

        do { yield return null; } while (CubesBusy());

        _allowButtonSelection = true;
        _allowScreenSelection = true;
    }

    IEnumerator StageOneAnimation()
    {
        _screen.EnableOverride("...");
        _screen.SetText("Stage 1");

        foreach (CubeScript cube in Cubes)
        {
            cube.SetHiddenStatus(false, 2);
            cube.SetSelectionHiding(false);
        }

        do { yield return null; } while (CubesBusy());

        for (int i = 0; i < 9; i++)
        {
            Cubes[i].SetStateFromSETValues(_stageOneSETValues[i]);
        }

        do { yield return null; } while (CubesBusy());

        _screen.DisableOverride();
        _allowButtonSelection = true;
    }

    IEnumerator StageTwoAnimation()
    {
        int position;

        DeselectAllCubes();

        _allowButtonSelection = false;
        _screen.EnableOverride("...");
        _screen.SetText("Stage 2");

        foreach (CubeScript cube in Cubes)
        {
            cube.ChangeColour(1, 1, 1);
            cube.ChangeSize(0);
        }

        do { yield return null; } while (CubesBusy());

        PermsManager.GenerateRandomPermutationSequence();

        foreach (Cycle cycle in PermsManager.Cycles)
        {
            foreach (CubeScript cube in Cubes) 
            {
                position = GetPositionNumberFromCube(cube);
                if (!cycle.Contains(position)) cube.SetHiddenStatus(true);
                else cube.SetHiddenStatus(false);
            }

            do { yield return null; } while (CubesBusy());

            foreach (CubeScript cube in Cubes)
            {
                position = GetPositionNumberFromCube(cube);
                if (cycle.Contains(position)) cube.ChangePosition(GetPositionFromNumber(cycle.Permute(position)));
            }

            do { yield return null; } while (CubesBusy());
        }

        foreach (CubeScript cube in Cubes)
        {
            cube.SetHiddenStatus(false);
        }

        _stageTwoSETValues = SETGenerator.GenerateSetList();
        _stageTwoCorrectValues = SETGenerator.CorrectAnswers;

        for (int i = 0; i < 9; i++)
        {
            Cubes[i].SetStateFromSETValues(_stageTwoSETValues[i]);
        }

        do { yield return null; } while (CubesBusy());

        ReorderCubes();
        _screen.DisableOverride();
        _allowButtonSelection = true;
    }

    // These two methods look a bit confusing because we are taking position in *reading order* so we have to invert z.
    int GetPositionNumberFromCube(CubeScript cube)
    {
        return (cube.Position[0] + 1) + ((-cube.Position[1] + 1) * 3);
    }

    int[] GetPositionFromNumber(int number)
    {
        return new int[] { (number % 3) - 1, 1 - (number / 3)};
    }

    void DeselectAllCubes()
    {
        _numOfSelectedCubes = 0;
        
        foreach (CubeScript cube in Cubes)
        {
            cube.EnableSelectionHighlight(false);
        }
    }

    void ReorderCubes() // This is for gamepad support.
    {
        int row;
        int col;
        int pos;

        foreach (CubeScript cube in Cubes)
        {
            col = cube.Position[0];
            row = cube.Position[1];
            pos = 5 + col + 4 * (1 + row);

            GetComponentInParent<KMSelectable>().Children[pos] = cube.GetComponentInParent<KMSelectable>();
        }

        GetComponentInParent<KMSelectable>().UpdateChildren();
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
