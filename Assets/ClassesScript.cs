//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ColouredCube
//{
//    private const float _positionOffset = 0.04f;

//    private GameObject _cube;
//    private Vector3 _position;

//    public Vector3 Position { get { return _position; } }

//    public KMSelectable Button { get { return _cube.GetComponent<KMSelectable>(); } }

//    public ColouredCube(GameObject cube, Vector3 position, Color colour)
//    {
//        _cube = cube;
//        _position = position;

//        _cube.transform.position = _positionOffset * _position;
//        _cube.GetComponent<MeshRenderer>().material.color = colour;
//    }

//    private void CubePress()
//    {
//        return;
//    }
//}
