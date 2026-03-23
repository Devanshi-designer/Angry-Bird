using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;
    
    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;

    [Header("Bird")]
    [SerializeField] private AngryBird _angryBirdPrefab;
    [SerializeField] private float _andryBirdPositionOffset = 2f;

    private Vector2 _slingShotLinesPosition;

    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinArea;
    private bool _birdOnSlingshot;

    private AngryBird _spanwedAngryBird;

    private void Awake()
    {
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngryBird();
    }

    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingshotArea())
        {
            _clickedWithinArea = true;
        }

        if (InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotateAngryBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingshot)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _clickedWithinArea = false;
                _birdOnSlingshot = false;

                _spanwedAngryBird.LaunchBird(_direction, _shotForce);
                GameManager.instance.UseShot();
                SetLines(_centerPosition.position);

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngryBirdAfterTime());
                }
            }
        }
    }

    #region SlingShot Methods

    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);

        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if(!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }

    #endregion

    #region Angry Bird Methods

    private void SpawnAngryBird()
    {
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _andryBirdPositionOffset;

        _spanwedAngryBird = Instantiate(_angryBirdPrefab, spawnPosition, Quaternion.identity);
        _spanwedAngryBird.transform.right = dir;

        _birdOnSlingshot = true;
    }

    private void PositionAndRotateAngryBird()
    {
        _spanwedAngryBird.transform.position = _slingShotLinesPosition + _directionNormalized * _andryBirdPositionOffset;
        _spanwedAngryBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngryBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);

        SpawnAngryBird();
    }

    #endregion

    #region Animate SlingShot



    #endregion

}
