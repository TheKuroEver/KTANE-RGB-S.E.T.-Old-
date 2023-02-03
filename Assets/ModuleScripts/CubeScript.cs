using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{

	[SerializeField]
	private GameObject SelectionHighlight;

	private const float _biggestCubeSize = 0.03173903f;

	private MeshRenderer _cubeMeshRenderer;
	private Transform _cubeTransform;

	private int[] _originalPosition;
	private int[] _position;
	private int _size = 2;

	[SerializeField]
	private int[] _colourAsTernaryValues;
	private bool _isChangingSize = false;
	private bool _isChangingColour = false;
	private bool _isSelected = false;
	private bool _isHidden = true;
	private bool _isMoving = false;
	private bool _hideSelection = false;

	private string _colourAsName;
	private string _SETValue;
	private string _displayedSETValue;

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
	public string SETValue { get { return _SETValue; } }
	public bool IsChangingColour { get { return _isChangingColour; } }
	public bool IsBusy { get { return _isChangingSize || IsChangingColour || _isMoving; } }
	public bool IsSelected { get { return _isSelected; } }
	public int[] Position { get { return _position; } }
	public int[] OriginalPosition { get { return _originalPosition; } }

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
		{"111", "Grey" },
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

	void Start()
	{
		_cubeMeshRenderer = GetComponentInParent<MeshRenderer>();
		_cubeTransform = GetComponentInParent<Transform>();
		_position = GetPositionFromName();
		_originalPosition = _position;
		_cubeTransform.localPosition = new Vector3(_cubeTransform.localPosition.x, -0.04f, _cubeTransform.localPosition.z);
		Disappear();
	}

	public void SetStateFromSETValues(string SETValues)
	{
		_SETValue = SETValues;
		ModifySETValuesBasedOnPosition(_position);

		ChangeColour(_displayedSETValue[0] - '0', _displayedSETValue[1] - '0', _displayedSETValue[2] - '0');
		ChangeSize(_displayedSETValue[3] - '0');
	}

	public void SetStateFromSETValues(string SETValues, int[] modifierPositions)
	{
		_SETValue = SETValues;
		ModifySETValuesBasedOnPosition(modifierPositions);

		ChangeColour(_displayedSETValue[0] - '0', _displayedSETValue[1] - '0', _displayedSETValue[2] - '0');
		ChangeSize(_displayedSETValue[3] - '0');
	}

	private void ModifySETValuesBasedOnPosition(int[] positionToUse)
	{
		string rValue = (positionToUse[1] == -1) ? ((_SETValue[0] + '1') % 3).ToString() : _SETValue[0].ToString();
		string gValue = (positionToUse[1] == 0) ? ((_SETValue[1] + '1') % 3).ToString() : _SETValue[1].ToString();
		string bValue = (positionToUse[1] == 1) ? ((_SETValue[2] + '1') % 3).ToString() : _SETValue[2].ToString();
		string size = ((_SETValue[3] + positionToUse[0] + 1) % 3).ToString();

		_displayedSETValue = rValue + gValue + bValue + size;
	}

	private int[] GetPositionFromName()
	{
		string name = GetComponentInParent<Transform>().name;
		int row = name[1] - '2';
		int column = "ABC".IndexOf(name[0]) - 1;

		return new int[] { column, row };
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

	public void SetHiddenStatus(bool value, float transitionTime = 1)
	{
		if (value == _isHidden) return;

		_isMoving = true;
		_isHidden = value;
		StartCoroutine(MoveToHidden(value, transitionTime));
	}

	public void ChangePosition(int[] newPosition)
	{
		if (_isMoving) return;
		if (_position[0] == newPosition[0] && _position[1] == newPosition[1]) return;

		_isMoving = true;
		StartCoroutine(SetPositionTo(newPosition));
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

	private IEnumerator MoveToHidden(bool value, float transitionTime)
	{
		Vector3 startPosition = _cubeTransform.localPosition;
		float elapsedTime = 0;
		float transitionProgress;
		float newY = value ? -0.04f : 0;
		float yDifference = newY - startPosition.y;
		float currentTransitionY;

		yield return null;

		while (elapsedTime <= transitionTime)
		{
			elapsedTime += Time.deltaTime;
			transitionProgress = Mathf.Min(elapsedTime / transitionTime, 1);
			currentTransitionY = startPosition.y + transitionProgress * yDifference;
			_cubeTransform.localPosition = new Vector3(startPosition.x, currentTransitionY, startPosition.z);
			yield return null;
		}

		_isMoving = false;
	}

	private IEnumerator SetPositionTo(int[] newPosition)
	{
		float transitionTime = 1;
		float elapsedTime = 0;
		float transitionProgress;
		float xDifference = newPosition[0] - _position[0];
		float zDifference = newPosition[1] - _position[1];
		Vector3 currentTransitionPosition;

		yield return null;

		while (elapsedTime <= transitionTime)
		{
			elapsedTime += Time.deltaTime;
			transitionProgress = Mathf.Min(elapsedTime / transitionTime, 1);
			currentTransitionPosition = new Vector3(0.04f * (_position[0] + transitionProgress * xDifference), _cubeTransform.localPosition.y, -0.04f * (_position[1] + transitionProgress * zDifference));

			_cubeTransform.localPosition = currentTransitionPosition;
			yield return null;
		}

		_isMoving = false;
		_position = new int[] { newPosition[0], newPosition[1] };
	}

	private void Disappear()
    {
		_cubeTransform.localScale = new Vector3(0, 0, 0);
    }

	public void Reappear()
    {
		_cubeTransform.localScale = new Vector3(_biggestCubeSize, _biggestCubeSize, _biggestCubeSize);
    }

	public void SetSelectionHiding(bool value)
	{
		_hideSelection = value;
		SelectionHighlight.SetActive(!_hideSelection && _isSelected);
	}

	public void EnableSelectionHighlight(bool value)
	{
		_isSelected = value;
		SelectionHighlight.SetActive(value);
		EnableHighlightOverride(true);
	}

	public void EnableHighlightOverride(bool value)
	{
		if (value) SelectionHighlight.SetActive(false);
		else SelectionHighlight.SetActive(!_hideSelection && _isSelected);
	}

	public void FlashRed()
	{
		int[] cubeColour = _colourAsTernaryValues;
		_cubeMeshRenderer.material.color = Color.red;
		_colourAsTernaryValues = new int[] { 2, 0, 0 };
		ChangeColour(cubeColour[0], cubeColour[1], cubeColour[2]);
	}
}
