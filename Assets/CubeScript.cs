using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour {

	private const float _biggestCubeSize = 0.03173903f;

	private MeshRenderer _cubeMeshRenderer;
	private Transform _cubeTransform;

	private int[] _position;
	private int[] _colourAsTernaryValues = new int[] { 2, 2, 2 };
	private int _size = 2;

	private bool _isChangingSize = false;
	private bool _isChangingColour = false;

	private string _colourAsName;

	public string ColourAsName
	{
		get
		{
			if (_colourAsName != null) return _colourAsName;

			string colourValuesAsString = "";

			foreach (int value in _colourAsTernaryValues)
            {
				colourValuesAsString += value.ToString();
            }

			_colourAsName = TernaryToColourName[colourValuesAsString];
			return _colourAsName;
		}
	}
	public bool IsChangingColour { get { return _isChangingColour; } }

    private readonly Dictionary<string, string> TernaryToColourName = new Dictionary<string, string>()
    {
		{"000", "Black" },
		{"001", "Indigo" },
		{"002", "Blue" },
		{"010", "Forest" },
		{"011", "Teal" },
		{"012", "Azure" },
		{"020", "Green" },
		{"021", "Jade" },
		{"022", "Cyan" },
		{"100", "Maroon" },
		{"101", "Plum" },
		{"102", "Violet" },
		{"110", "Olive" },
		{"111", "Gray" },
		{"112", "Maya" },
		{"120", "Lime" },
		{"121", "Mint" },
		{"122", "Aqua" },
		{"200", "Red" },
		{"201", "Rose" },
		{"202", "Magenta" },
		{"210", "Orange" },
		{"211", "Salmon" },
		{"212", "Pink" },
		{"220", "Yellow" },
		{"221", "Cream" },
		{"222", "White" }
	};
    
	void Start () {
		_cubeMeshRenderer = GetComponentInParent<MeshRenderer>();
		_cubeTransform = GetComponentInParent<Transform>();
		_position = GetPositionFromName();

		_cubeMeshRenderer.material.color = Color.white;
	}

	private int[] GetPositionFromName()
	{
		string name = GetComponentInParent<Transform>().name;
		int row = name[1] - '1';
		int column = "ABC".IndexOf(name[0]);

		return new int[] { row, column };
	}

	public void ChangeSize(int newSize)
    {
		if (_isChangingSize) return;

		if (newSize == _size) return;

		_isChangingSize = true;
		StartCoroutine(SetSizeTo(newSize));
    }

	public void ChangeColour(int newRedValue, int newGreenValue, int newBlueValue)
    {
		if (_isChangingColour) return;

		if ((newRedValue == _colourAsTernaryValues[0]) && (newGreenValue == _colourAsTernaryValues[1]) && (newBlueValue == _colourAsTernaryValues[2])) return;

		_isChangingColour = true;
		StartCoroutine(SetColourTo(newRedValue, newGreenValue, newBlueValue));
    }

	private IEnumerator SetSizeTo(int newSize)
    {
		float transitionTime = 1;
		float elapsedTime = 0;
		float transitionProgress;
		float sizeDifference = newSize - _size;
		float currentTransitionScale;
		float currentTransitionSize;

		yield return null;

		while (elapsedTime <= transitionTime)
        {
			elapsedTime += Time.deltaTime;
			transitionProgress = Mathf.Min(elapsedTime / transitionTime, 1);
			currentTransitionScale = (transitionProgress * sizeDifference + _size) * 0.25f + 0.5f;
			currentTransitionSize = currentTransitionScale * _biggestCubeSize;

			_cubeTransform.localScale = new Vector3(currentTransitionSize, currentTransitionSize, currentTransitionSize);
			yield return null;
        }

		_size = newSize;
		_isChangingSize = false;
    }

	private IEnumerator SetColourTo(int newRedValue, int newGreenValue, int newBlueValue)
    {
		int[] oldColourValues = _colourAsTernaryValues;
		float transitionTime = 1;
		float elapsedTime = 0;
		float transitionProgress;
		float redDifference = newRedValue - oldColourValues[0];
		float greenDifference = newGreenValue - oldColourValues[1];
		float blueDifference = newBlueValue - oldColourValues[2];
		float currentRedValue;
		float currentGreenValue;
		float currentBlueValue;

		_colourAsTernaryValues = new int[] { newRedValue, newGreenValue, newBlueValue };
		_colourAsName = null;

		yield return null;

		while (elapsedTime <= transitionTime)
        {
			elapsedTime += Time.deltaTime;
			transitionProgress = Mathf.Min(elapsedTime / transitionTime, 1);
			currentRedValue = 0.5f * (transitionProgress * redDifference + oldColourValues[0]);
			currentGreenValue = 0.5f * (transitionProgress * greenDifference + oldColourValues[1]);
			currentBlueValue = 0.5f * (transitionProgress * blueDifference + oldColourValues[2]);

			_cubeMeshRenderer.material.color = new Color(currentRedValue, currentGreenValue, currentBlueValue);
			yield return null;
		}

		_isChangingColour = false;
	}
}
