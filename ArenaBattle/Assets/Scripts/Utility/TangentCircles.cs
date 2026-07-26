using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangentCircles : CircleTangent
{
    public GameObject _circlePrefab;
    private GameObject _innerCircleGO, _outterCircleGO, _tangentCircleGO;
    public Vector4 _innerCircle, _outterCircle;
    private Vector4[] _tangentCircle;
    private GameObject[] _tangentObject;
    [Range(1, 64)]
    public int _circleAmount;
    bool bChange = false;
    float changeTime = 0;
    float innerDestX, innerDestZ, outterDestX, outterDestZ;
    float innerDistX, innerDistZ, outterDistX, outterDistZ;
    Color innerColor = Color.white, outterColor = Color.white, innerOldColor, outterOldColor;
//    public float _tangentCircleRadius;
//    public float _degree;

    // Start is called before the first frame update
    void Start()
    {
        _innerCircleGO = (GameObject)Instantiate(_circlePrefab);
        _innerCircleGO.name = "innerCircle";
        _innerCircleGO.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);
        _outterCircleGO = (GameObject)Instantiate(_circlePrefab);
        _outterCircleGO.name = "outterCircle";
        _outterCircleGO.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);
        _tangentCircle = new Vector4[_circleAmount];
        _tangentObject = new GameObject[_circleAmount];

        for(int i = 0; i < _circleAmount; i++)
		{
            GameObject tangentInstance = (GameObject)Instantiate(_circlePrefab);
            _tangentObject[i] = tangentInstance;
            _tangentObject[i].transform.SetParent(this.transform);
            _tangentObject[i].transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);
        }
//        _tangentCircleGO = (GameObject)Instantiate(_circlePrefab);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCircle();

        _innerCircleGO.transform.position = new Vector3(_innerCircle.x + 1000.0f, _innerCircle.y + 1000.0f, _innerCircle.z + 1000.0f);
        _innerCircleGO.transform.localScale = new Vector3(_innerCircle.w, _innerCircle.w, _innerCircle.w) * 2;
        _outterCircleGO.transform.position = new Vector3(_outterCircle.x + 1000.0f, _outterCircle.y + 1000.0f, _outterCircle.z + 1000.0f);
        _outterCircleGO.transform.localScale = new Vector3(_outterCircle.w, _outterCircle.w, _outterCircle.w) * 2;

//        _innerCircleGO.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", innerColor);
//        _outterCircleGO.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", outterColor);
        for (int i = 0; i < _circleAmount; i++)
		{
            _tangentCircle[i] = FindTangentCircle(_outterCircle, _innerCircle, (360f / +_circleAmount) * i);
            _tangentObject[i].transform.position = new Vector3(_tangentCircle[i].x + 1000.0f, _tangentCircle[i].y + 1000.0f, _tangentCircle[i].z + 1000.0f);
            _tangentObject[i].transform.localScale = new Vector3(_tangentCircle[i].w, _tangentCircle[i].w, _tangentCircle[i].w) * 2;

            _tangentObject[i].GetComponent<MeshRenderer>().material.SetColor("_BaseColor", innerColor);
        }
//        _tangentCircleGO.transform.position = GetRotatedTangent(_degree, _outterCircle.w) + _outterCircleGO.transform;
//        _tangentCircleGO.transform.localScale = new Vector3(_innerCircle.w, _innerCircle.w, _innerCircle.w) * 2;
    }

    void UpdateCircle()
	{
        if(!bChange)
		{
            innerDestX = UnityEngine.Random.Range(-5.0f, 5.0f);
            innerDestZ = UnityEngine.Random.Range(-3.0f, 3.0f);

            outterDestX = UnityEngine.Random.Range(-1.0f, 1.0f);
            outterDestZ = UnityEngine.Random.Range(-1.0f, 1.0f);

            innerDistX = (innerDestX - _innerCircle.x);
            innerDistZ = (innerDestZ - _innerCircle.z);

            outterDistX = (outterDestX - _outterCircle.x);
            outterDistZ = (outterDestZ - _outterCircle.z);

            innerOldColor = innerColor;
            outterOldColor = outterColor;
            innerColor.r = Random.Range(0f, 1f);
            innerColor.g = Random.Range(0f, 1f);
            innerColor.b = Random.Range(0f, 1f);

            outterColor.r = Random.Range(0f, 1f);
            outterColor.g = Random.Range(0f, 1f);
            outterColor.b = Random.Range(0f, 1f);

            bChange = true;
            changeTime = 2;
        }
        else
		{
            float inx = _innerCircle.x;
            float inz = _innerCircle.z;
            float outx = _outterCircle.x;
            float outz = _outterCircle.z;
            Color inColor = innerColor;
            Color outColor = outterColor;

            inx += ((innerDistX * Time.deltaTime) / 2f);
            inz += ((innerDistZ * Time.deltaTime) / 2f);
            outx += ((outterDistX * Time.deltaTime) / 2f);
            outz += ((outterDistZ * Time.deltaTime) / 2f);

            inColor.r += ((innerOldColor.r * Time.deltaTime) / 2f);
            inColor.g += ((innerOldColor.g * Time.deltaTime) / 2f);
            inColor.b += ((innerOldColor.b * Time.deltaTime) / 2f);

            outColor.r += ((outterOldColor.r * Time.deltaTime) / 2f);
            outColor.g += ((outterOldColor.g * Time.deltaTime) / 2f);
            outColor.b += ((outterOldColor.b * Time.deltaTime) / 2f);

            _innerCircle.x = inx;
            _innerCircle.z = inz;
            _outterCircle.x = outx;
            _outterCircle.z = outx;

            changeTime -= Time.deltaTime;
            if (changeTime <= 0)
            {
                bChange = false;
                innerOldColor = innerColor;
                outterOldColor = outterColor;
            }
        }
	}
}
