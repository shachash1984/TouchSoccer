using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {

    #region Fields
    [SerializeField] private Transform _arrowParent;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _rotationSpeed = 0f;
    private const float X_SIZE = 1.4f;
    private const float INITIAL_Y_SIZE = 4f;
    #endregion

    #region Methods
    /// <summary>
    /// Set the direction to point at
    /// </summary>
    /// <param name="direction">Should be relative to current position</param>
    public void SetDirection(Vector3 direction)
    {
        _arrowParent.DOLookAt(direction, _rotationSpeed);
    }

    /// <summary>
    /// Set the position of the direct parent to local origin (disc position)
    /// </summary>
    public void CenterParentPosition()
    {
        transform.parent.localPosition = Vector3.zero;
    }

    /// <summary>
    /// initialize
    /// </summary>
    /// <param name="disc"></param>
    public void Init(Transform disc)
    {
        SetParent(disc);
        CenterParentPosition();
        _arrowParent.localRotation = Quaternion.Euler(Vector3.zero);
        _spriteRenderer.size = new Vector2(X_SIZE, INITIAL_Y_SIZE);
    }

    /// <summary>
    /// set the direct parent's parent (the disc)
    /// </summary>
    /// <param name="newParent"></param>
    public void SetParent(Transform newParent)
    {
        _arrowParent.parent = newParent;
    }
    #endregion



}
