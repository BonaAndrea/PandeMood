using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class DrawLine : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Transform _newPoint;
        private Transform _startPoint;
        private int _count;
        private GameObject[] _points;

        [FormerlySerializedAs("Point")] public GameObject point;
        public int numberCells;
        [FormerlySerializedAs("AudioConnection")] public AudioSource audioConnection;

        // Start is called before the first frame update
        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount=0;
            _points = new GameObject[numberCells];
            point.SetActive(false);
        }

        public void SetNewPoint(Transform newPoint)
        {
            if (_count >= numberCells) return;
            if (_startPoint == null)
            { //START POINT
                _startPoint = newPoint;
                _newPoint = newPoint;
                _lineRenderer.positionCount++;
                var position = _startPoint.position;
                _lineRenderer.SetPosition(_count, new Vector3(position.x, position.y + 1.2f, position.z)); //create line
                _points[_count] = Instantiate(point, new Vector3(position.x, position.y + 1.2f, position.z), Quaternion.identity); //create point
                _points[_count].SetActive(true);
                _count++;
            }
            else
            { //ADD NEW POINT
                _newPoint = newPoint;
                _lineRenderer.positionCount++;
                var position = _newPoint.position;
                _lineRenderer.SetPosition(_count, new Vector3(position.x, position.y + 1.2f, position.z)); //create line
                _points[_count] = Instantiate(point, new Vector3(position.x, position.y + 1.2f, position.z), Quaternion.identity); //create point
                _points[_count].SetActive(true);
                _count++;
            }

            audioConnection.Play();

            if (_count >= numberCells)
                ResetLine();
        }

        public void ResetLine()
        {
            for (var i = 0; i < _count; i++) //DESTROY POINTS
                Destroy(_points[i]);

            _count = 0;
            _startPoint = null;
            _newPoint = null;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            _lineRenderer.SetPosition(1, new Vector3(0, 0, 0));
            _lineRenderer.positionCount = 0;
        }
    }
}
