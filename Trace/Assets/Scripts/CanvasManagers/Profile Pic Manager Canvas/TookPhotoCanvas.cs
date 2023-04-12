using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TookPhotoCanvas : MonoBehaviour
{
    public Button _moveBack;
    public Button _doneButton;
    public Image _profilePicture;

    private TookPhotoCanvasController _controller;

    private void OnEnable()
    {
        if (_controller is null)
            _controller = new TookPhotoCanvasController(this);
      
        _controller.Init();
    }

    private void OnDisable()
    {
        _controller.Uninit();
    }
}
