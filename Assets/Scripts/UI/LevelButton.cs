using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    Button _button;
    [SerializeField] LevelSettings levelSettings;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick); //Overriding the attached button component's onClick event
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    /// <summary>
    /// Sends the level settings to Level Manager
    /// </summary>
    private void OnButtonClick()
    {
        LevelManager.Instance.GenerateLevel(levelSettings);
    }
}
