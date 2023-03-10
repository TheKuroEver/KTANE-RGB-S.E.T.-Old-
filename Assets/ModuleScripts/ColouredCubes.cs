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
    public Transform ButtonGrid;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable ScreenButton;
    public KMSelectable[] StageLights;
    public KMBombModule Module;
    public CubeScript[] Cubes;
    public TextMesh ScreenText;

    private static int ModuleIdCounter = 1;
    private int ModuleId;
    private bool ModuleSolved;

    private ScreenTextHandler _screen;
    private int _stageNumber = 0;
    private int _displayedStage;
    private int _numOfSelectedCubes = 0;
    private int _nextStageForLogging = 1;

    private int[][] _originalPositions = new int[][]
    {
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2]
    };
    private int[][] _stageThreePositions = new int[][]
    {
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2],
        new int[2]
    };

    private Color[] _stageOneLightColours = new Color[3];
    private Color[] _stageTwoLightColours = new Color[3];

    private string[] _stageOneSETValues;
    private string[] _stageTwoSETValues;
    private string[] _stageThreeSETValues;
    private string[] _stageOneCorrectValues;
    private string[] _stageTwoCorrectValues;
    private string[] _stageThreeCorrectValues;
    private string[] _stageOneLightColourNames = new string[3];
    private string[] _stageTwoLightColourNames = new string[3];

    private bool _allowButtonSelection = false;
    private bool _allowScreenSelection = true;
    private bool _displayingSizeChart = false;
    private bool _generatedStageTwo = false;

    private readonly Dictionary<string, string> BinaryColourToName = new Dictionary<string, string>()
    {
        { "111", "White" },
        { "101", "Magenta" },
        { "011", "Cyan" },
        { "110", "Yellow" },
        { "100", "Red" },
        { "010", "Green" },
        { "001", "Blue" },
        { "000", "Black" }
    };

    private readonly Dictionary<string, int> StageLightToPositionNumber = new Dictionary<string, int>()
    {
        { "Stage1Light", 0 },
        { "Stage2Light", 1 },
        { "Stage3Light", 2 },
    };

    private readonly Dictionary<int, string> BottomToTopColumnOrderToPosition = new Dictionary<int, string>()
    {
        { 0, "Bottom left" },
        { 1, "Middle left" },
        { 2, "Top left" },
        { 3, "Bottom middle" },
        { 4, "Middle centre" },
        { 5, "Top middle" },
        { 6, "Bottom right" },
        { 7, "Middle right" },
        { 8, "Top right" }
    };

    public static readonly Dictionary<int, string> ReadingOrderToPosition = new Dictionary<int, string>()
    {
        { 0, "Top left" },
        { 1, "Top middle" },
        { 2, "Top right" },
        { 3, "Middle left" },
        { 4, "Middle centre" },
        { 5, "Middle right" },
        { 6, "Bottom left" },
        { 7, "Bottom middle" },
        { 8, "Bottom right" }
    };

    void Awake()
    {
        ModuleId = ModuleIdCounter++;

        _screen = new ScreenTextHandler(ScreenText);
        KMSelectable tempSelectable;

        foreach (CubeScript cube in Cubes)
        {
            tempSelectable = cube.GetComponentInParent<KMSelectable>();
            tempSelectable.OnInteract += delegate () { ButtonPress(cube); return false; };
            tempSelectable.OnHighlight += delegate () { cube.EnableHighlightOverride(true); _screen.DisplayColourName(cube.ColourAsName); };
            tempSelectable.OnHighlightEnded += delegate () { cube.EnableHighlightOverride(false); _screen.EndColourNameDisplay(); };
        }

        ScreenButton.OnInteract += delegate () { ScreenPress(); return false; };

        foreach (KMSelectable stageLight in StageLights)
        {
            stageLight.OnInteract += delegate () { StageLightPress(stageLight); return false; };
            stageLight.OnHighlight += delegate () { DisplayStageLightColourName(stageLight); };
            stageLight.OnHighlightEnded += delegate () { _screen.EndColourNameDisplay(); };
        }

        _screen.EnableOverride("Start");
    }

    void Start()
    {
        PermsManager.GenerateRandomPermutationSequence();

        _stageOneSETValues = SETGenerator.GenerateSetList();
        _stageOneCorrectValues = SETGenerator.CorrectAnswers.ToArray();
        _stageTwoSETValues = SETGenerator.GenerateSetList();
        _stageTwoCorrectValues = SETGenerator.CorrectAnswers.ToArray();

        GenerateStageLightColoursAndStageThree();
    }

    void LogStageOne()
    {
        string position;
        int j;

        Debug.LogFormat("[Coloured Cubes #{0}] Positions are from 0-8 in reading order.", ModuleId);
        Debug.LogFormat("[Coloured Cubes #{0}] Values are in Red-Green-Blue-Size order.", ModuleId);
        Debug.LogFormat("[Coloured Cubes #{0}] Stage 1:", ModuleId);

        for (int i = 0; i < 9; i++)
        {
            j = (i - (i % 3)) + (3 - (i % 3)) - 1; // We do a little reordering.
            position = BottomToTopColumnOrderToPosition[j];
            Debug.LogFormat("[Coloured Cubes #{0}] {1} cube is {2}, size {3}. Its actual values are {4}.", ModuleId, position, Cubes[j].ColourAsName.ToLower(), Cubes[j].Size, Cubes[j].SETValue);
        }

        Debug.LogFormat("[Coloured Cubes #{0}] One possible set is {1}, {2}, and {3}.", ModuleId, _stageOneCorrectValues[0], _stageOneCorrectValues[1], _stageOneCorrectValues[2]);

        _nextStageForLogging = 2;
    }

    void LogStageTwo()
    {
        string position;
        int j;

        Debug.LogFormat("[Coloured Cubes #{0}] Stage 2:", ModuleId);
        Debug.LogFormat("[Coloured Cubes #{0}] The cycles used were as follows:", ModuleId);

        foreach (Cycle cycle in PermsManager.Cycles)
        {
            Debug.LogFormat("[Coloured Cubes #{0}] {1}", ModuleId, cycle.ToString());
        }

        for (int i = 0; i < 9; i++)
        {
            j = (i - (i % 3)) + (3 - (i % 3)) - 1; // We do a little reordering.
            position = BottomToTopColumnOrderToPosition[j];
            Debug.LogFormat("[Coloured Cubes #{0}] {1} cube is {2}, size {3}. This cube's original position was {4}. Its actual values are {5}.", ModuleId, position, Cubes[j].ColourAsName.ToLower(), Cubes[j].Size, ReadingOrderToPosition[GetPositionNumber(_originalPositions[i])].ToLower(), Cubes[j].SETValue);
        }

        Debug.LogFormat("[Coloured Cubes #{0}] One possible set is {1}, {2}, and {3}.", ModuleId, _stageTwoCorrectValues[0], _stageTwoCorrectValues[1], _stageTwoCorrectValues[2]);
        _nextStageForLogging = 3;
    }

    void GenerateStageLightColoursAndStageThree()
    {
        int[] colourValues;
        int redPosition = Rnd.Range(0, 3);
        int greenPosition = Rnd.Range(0, 3);
        int bluePosition = Rnd.Range(0, 3);
        Debug.Log("Using " + (Bomb.GetPortCount() % 3).ToString() + " ports (mod 3) and " + (Bomb.GetIndicators().Count() % 3).ToString() + " indicators (mod 3).");
        string stageOneValues = redPosition.ToString() + greenPosition.ToString() + bluePosition.ToString() + (Bomb.GetPortCount() % 3).ToString();
        string stageTwoValues;

        for (int i = 0; i < 3; i++)
        {
            colourValues = new int[] { 0, 0, 0 };
            if (i == redPosition) colourValues[0] = 1;
            if (i == greenPosition) colourValues[1] = 1;
            if (i == bluePosition) colourValues[2] = 1;

            _stageOneLightColours[i] = new Color(colourValues[0], colourValues[1], colourValues[2]);
            _stageOneLightColourNames[i] = BinaryColourToName[colourValues[0].ToString() + colourValues[1].ToString() + colourValues[2].ToString()];
        }

        redPosition = Rnd.Range(0, 3);
        greenPosition = Rnd.Range(0, 3);
        bluePosition = Rnd.Range(0, 3);

        stageTwoValues = redPosition.ToString() + greenPosition.ToString() + bluePosition.ToString() + (Bomb.GetIndicators().Count() % 3).ToString();

        for (int i = 0; i < 3; i++)
        {
            colourValues = new int[] { 1, 1, 1 };
            if (i == redPosition) colourValues[0] = 0;
            if (i == greenPosition) colourValues[1] = 0;
            if (i == bluePosition) colourValues[2] = 0;

            _stageTwoLightColours[i] = new Color(colourValues[0], colourValues[1], colourValues[2]);
            _stageTwoLightColourNames[i] = BinaryColourToName[colourValues[0].ToString() + colourValues[1].ToString() + colourValues[2].ToString()];
        }

        _stageThreeSETValues = SETGenerator.GenerateSetList(stageOneValues, stageTwoValues);
        _stageThreeCorrectValues= SETGenerator.CorrectAnswers.ToArray() ;
    }

    void SetStageLightColours(int stageNumber)
    {
        if (stageNumber == 1)
        {
            for (int i = 0; i < 3; i++) StageLights[i].GetComponentInParent<MeshRenderer>().material.color = _stageOneLightColours[i];
        }
        else if (stageNumber == 2)
        {
            for (int i = 0; i < 3; i++) StageLights[i].GetComponentInParent<MeshRenderer>().material.color = _stageTwoLightColours[i];
        }
        else
        {
            foreach (KMSelectable stageLight in StageLights) stageLight.GetComponentInParent<MeshRenderer>().material.color = Color.black;
        }
    }

    void DisplayStageLightColourName(KMSelectable stageLight)
    {
        int position = StageLightToPositionNumber[stageLight.name];

        if (_stageNumber == 0) return;

        if (_stageNumber == 1)
        {
            _screen.DisplayColourName(_stageOneLightColourNames[position]);
        }
        else if (_stageNumber == 2)
        {
            _screen.DisplayColourName(_stageTwoLightColourNames[position]);
        }
    }

    void ButtonPress(CubeScript cube) // Need to add a sound effect on selections.
    {
        if (!_allowButtonSelection) return;

        if (_numOfSelectedCubes == 3 && !cube.IsSelected) return;

        HandleSelection(cube);

        if (_numOfSelectedCubes == 3)
        {
            if (_stageNumber == 1) HandleStageOne();
            else if (_stageNumber == 2) HandleStageTwo();
        }
        else if (_numOfSelectedCubes == 2 && _stageNumber == 3) HandleStageThree();
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
        if (SubmissionCorrect(_stageOneCorrectValues))
        {
            _stageNumber = 2;
            StartCoroutine(StageTwoAnimation(true));
        }
        else
        {
            Strike();
        }
    }

    void HandleStageTwo()
    {
        if (SubmissionCorrect(_stageTwoCorrectValues))
        {
            _stageNumber = 3;
            StartCoroutine(StageThreeAnimation(true));
        }
        else
        {
            Strike();
        }
    }

    void HandleStageThree()
    {
        if (SubmissionCorrect(_stageThreeCorrectValues))
        {
            DeselectAllCubes();
            Module.HandlePass();
            ModuleSolved = true;
        }
        else
        {
            Strike();
        }
    }

    bool SubmissionCorrect(string[] correctValues)
    {
        foreach (CubeScript cube in Cubes)
        {
            if (cube.IsSelected && !Array.Exists(correctValues, element => element == cube.SETValue))
            {
                return SETGenerator.FormASet(Cubes.Where(c => c.IsSelected).Select(c => c.SETValue).ToArray());
            }
        }

        return true;
    }

    void ScreenPress()
    {
        if (ModuleSolved) { StartCoroutine(FunnyButtonGridRotation()); return; }

        if (!_allowScreenSelection) return;

        if (_stageNumber == 0)
        {
            ReorderCubes();
            _stageNumber = 1;

            foreach (CubeScript cube in Cubes) cube.Reappear();

            StartCoroutine(StageOneAnimation(true));
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

    void StageLightPress(KMSelectable pressedLight)
    {
        if (!_allowScreenSelection || _displayingSizeChart) return;

        if (pressedLight.name == "Stage1Light" && _displayedStage != 1)
        {
            StartCoroutine(StageOneAnimation(true));
        }
        else if (pressedLight.name == "Stage2Light" && _displayedStage != 2 && _stageNumber >= 2)
        {
            StartCoroutine(StageTwoAnimation(true));
        }
        else if (pressedLight.name == "Stage3Light" && _displayedStage != 3 && _stageNumber >= 3)
        {
            StartCoroutine(StageThreeAnimation(true));
        }
    }

    // This is my first experience with coroutines (also only my second mod) so the code is bad. Sorry. Not as bad as The Cipher Ever though.
    // In hindsight, I see so many things I could have written better; maybe I'll come back to clean it up at some point!
    IEnumerator ShowSizeChart()
    {
        var sizeChart = new Dictionary<int, int>() { { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 } };

        SetStageLightColours(3);

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

        if (_displayedStage == 1)
        {
            StartCoroutine(StageOneAnimation(false));
        }
        else if (_displayedStage == 2)
        {
            _screen.EnableOverride("...");
            _screen.SetText("Stage 2");

            for (int i = 0; i < 9; i++)
            {
                Cubes[i].SetStateFromSETValues(_stageTwoSETValues[i], _originalPositions[GetPositionNumberFromCube(Cubes[i])]);
                Cubes[i].SetSelectionHiding(false);
            }

            SetStageLightColours(2);
        }
        else if (_displayedStage == 3)
        {
            StartCoroutine(StageThreeAnimation(false));
        }

        do { yield return null; } while (CubesBusy());

        _screen.DisableOverride();
        _allowButtonSelection = true;
        _allowScreenSelection = true;
    }

    IEnumerator StageOneAnimation(bool deselect)
    {
        if (deselect) DeselectAllCubes();

        SetStageLightColours(3);

        _allowButtonSelection = false;
        _allowScreenSelection = false;

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

        SetStageLightColours(1);

        _displayedStage = 1;
        _screen.DisableOverride();
        _allowScreenSelection = true;

        if (_stageNumber == 1) _allowButtonSelection = true;

        if (_nextStageForLogging == 1) LogStageOne();
    }

    IEnumerator StageTwoAnimation(bool deselect)
    {
        int position;

        if (deselect) DeselectAllCubes();
        SetStageLightColours(3);

        _allowButtonSelection = false;
        _allowScreenSelection = false;
        _screen.EnableOverride("...");
        _screen.SetText("Stage 2");

        foreach (CubeScript cube in Cubes)
        {
            cube.ChangeColour(2, 2, 2);
            cube.ChangeSize(0);
        }

        do { yield return null; } while (CubesBusy());

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

        ReorderCubes();

        foreach (CubeScript cube in Cubes)
        {
            cube.SetHiddenStatus(false);
            cube.SetSelectionHiding(false);
        }

        Debug.Log(_stageTwoCorrectValues[0] + " " + _stageTwoCorrectValues[1] + " " + _stageTwoCorrectValues[2]);
        Debug.Log(_stageTwoSETValues[0] + " " + _stageTwoSETValues[1] + " " + _stageTwoSETValues[2] + " " + _stageTwoSETValues[3] + " " + _stageTwoSETValues[4] + " " + _stageTwoSETValues[5] + " " + _stageTwoSETValues[6] + " " + _stageTwoSETValues[7] + " " + _stageTwoSETValues[8]);

        if (!_generatedStageTwo) GenerateStageTwoAndThreePositions();

        for (int i = 0; i < 9; i++)
        {
            Cubes[i].SetStateFromSETValues(_stageTwoSETValues[i], _originalPositions[GetPositionNumberFromCube(Cubes[i])]);
            if (_stageTwoCorrectValues.Contains(Cubes[i].SETValue)) Debug.Log("Correct cube at position " + GetPositionNumberFromCube(Cubes[i]).ToString());
        }

        do { yield return null; } while (CubesBusy());

        SetStageLightColours(2);

        _displayedStage = 2;
        _screen.DisableOverride();
        _allowScreenSelection = true;

        if (_stageNumber == 2) _allowButtonSelection = true;
        if (_nextStageForLogging == 2) LogStageTwo();
    }

    IEnumerator StageThreeAnimation(bool deselect)
    {
        if (deselect) DeselectAllCubes();

        SetStageLightColours(3);

        _allowButtonSelection = false;
        _allowScreenSelection = false;

        _screen.EnableOverride("...");
        _screen.SetText("Stage 3");

        StartCoroutine(FunnyButtonGridRotation());

        Debug.Log(_stageThreeCorrectValues[0] + " " + _stageThreeCorrectValues[1] + " " + _stageThreeCorrectValues[2]);
        Debug.Log(_stageThreeSETValues[0] + " " + _stageThreeSETValues[1] + " " + _stageThreeSETValues[2] + " " + _stageThreeSETValues[3] + " " + _stageThreeSETValues[4] + " " + _stageThreeSETValues[5] + " " + _stageThreeSETValues[6] + " " + _stageThreeSETValues[7] + " " + _stageThreeSETValues[8]);

        for (int i = 0; i < 9; i++)
        {
            Cubes[i].SetStateFromSETValues(_stageThreeSETValues[i], _stageThreePositions[GetPositionNumberFromCube(Cubes[i])]);
            Cubes[i].SetSelectionHiding(false);
            if (_stageThreeCorrectValues.Contains(Cubes[i].SETValue)) Debug.Log("Correct cube at position " + GetPositionNumberFromCube(Cubes[i]).ToString());
        }

        do { yield return null; } while (CubesBusy());

        SetStageLightColours(3);

        _displayedStage = 3;
        _screen.DisableOverride();
        _allowScreenSelection = true;
        _allowButtonSelection = true;
    }
        
    IEnumerator FunnyButtonGridRotation()
    {
        float transitionTime = 1;
        float elapsedTime = 0;
        float transitionScale;

        yield return null;

        while (elapsedTime <= transitionTime)
        {
            elapsedTime += Time.deltaTime;
            ButtonGrid.Rotate(new Vector3(Time.deltaTime * 360 / transitionTime, 0));

            transitionScale = 1 + Mathf.Sin(Mathf.PI * elapsedTime / transitionTime) / 2;
            ButtonGrid.localScale = new Vector3(transitionScale, transitionScale, transitionScale);

            yield return null;
        }

        ButtonGrid.localRotation = new Quaternion(0, 0, 0, 0);
    }

    void GenerateStageTwoAndThreePositions()
    {
        string newPositionString;

        foreach (CubeScript cube in Cubes)
        {
            _originalPositions[GetPositionNumberFromCube(cube)] = cube.OriginalPosition;

            newPositionString = SETGenerator.FindMatchingSet((cube.OriginalPosition[0] + 1).ToString() + (cube.OriginalPosition[1] + 1).ToString(), (cube.Position[0] + 1).ToString() + (cube.Position[1] + 1).ToString());
            Debug.Log((cube.OriginalPosition[0] + 1).ToString() + (cube.OriginalPosition[1] + 1).ToString() + " and " + (cube.Position[0] + 1).ToString() + (cube.Position[1] + 1).ToString() + " became " + newPositionString);
            _stageThreePositions[GetPositionNumberFromCube(cube)] = new int[] { newPositionString[0] - '1', newPositionString[1] - '1' };
        }

        _generatedStageTwo = true;
    }

    // These two methods look a bit confusing because we are taking position in *reading order* so we have to invert z.
    int GetPositionNumberFromCube(CubeScript cube)
    {
        return (cube.Position[0] + 1) + ((cube.Position[1] + 1) * 3);
    }

    int GetPositionNumber(int[] position)
    {
        return (position[0] + 1) + ((position[1] + 1) * 3);
    }

    int[] GetPositionFromNumber(int number)
    {
        return new int[] { (number % 3) - 1, (number / 3) - 1};
    }

    void DeselectAllCubes()
    {
        _numOfSelectedCubes = 0;

        foreach (CubeScript cube in Cubes)
        {
            cube.EnableSelectionHighlight(false);
        }
    }

    void ReorderCubes() // This is mainly for gamepad support.
    {
        var newCubeOrder = new CubeScript[9];
        int row;
        int col;
        int pos;

        foreach (CubeScript cube in Cubes)
        {
            col = cube.Position[0];
            row = cube.Position[1];
            pos = 5 + col + 4 * (row + 1);

            GetComponentInParent<KMSelectable>().Children[pos] = cube.GetComponentInParent<KMSelectable>();

            newCubeOrder[4 - row + 3 * col] = cube;
        }

        Cubes = newCubeOrder; // This is to ensure that going back stages preserves the original cube positions.
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

    void Strike()
    {
        foreach (CubeScript cube in Cubes) cube.FlashRed();
        Module.HandleStrike();

        DeselectAllCubes();
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

        // Priority is Override > ColourName > Text.
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
