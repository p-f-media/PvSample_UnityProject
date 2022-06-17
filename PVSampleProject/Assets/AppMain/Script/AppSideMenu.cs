using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppSideMenu : MonoBehaviour
{
    [SerializeField] UITransition bgTransition = null;
    [SerializeField] UITransition menuTransition = null;
    [SerializeField] RectTransform volumeRoot = null;
    [SerializeField] UITransition volumeTransition = null;
    // [SerializeField] Slider volumeSlider = null;
    [SerializeField] UITransition operationPcTransition = null;
    [SerializeField] UITransition operationMobileTransition = null;
    [SerializeField] UITransition operationBgTransition = null;
    

    public bool IsOpen{ get; private set; } = false;

    public bool IsVolumeWindowOpened{ get; private set; } = false;

    RectTransform operationPcRect = null;
    RectTransform operationMobileRect = null;
    RawImage operationPcRawImage = null;
    RawImage operationMobileRawImage = null;

    


    CanvasScaler canvasScaler = null;

    void Start()
    {
        AppGameManager.Instance.ScreenSizeChanged.AddListener( OnScreenSizeChanged );
        bgTransition.gameObject.SetActive( false );
        menuTransition.gameObject.SetActive( false );
        IsOpen = false;
    }


    public void Open()
    {
        if( IsVolumeWindowOpened == true ) CloseVolumeWindow();
        bgTransition.TransitionIn();
        menuTransition.TransitionIn( () => { IsOpen = true; });      

        // AppGameManager.Instance.SetMoveUI( false );
        AppGameManager.Instance.CurrentLock.Move = true;
        AppGameManager.Instance.CurrentLock.Rotation = true;
        AppGameManager.Instance.CurrentLock.Click = true;
        AppGameManager.Instance.CurrentLock.Look = true;
    }

    public void Close()
    {
        bgTransition.TransitionOut();
        menuTransition.TransitionOut( () => { IsOpen = false; });

        AppGameManager.Instance.CurrentLock.Move = false;
        AppGameManager.Instance.CurrentLock.Rotation = false;
        AppGameManager.Instance.CurrentLock.Click = false;
        AppGameManager.Instance.CurrentLock.Look = false;
    }

    public void OpenVolumeWindow()
    {
        if( IsOpen == true ) Close();
        volumeTransition.TransitionIn( () => { IsVolumeWindowOpened = true; });

        AppGameManager.Instance.CurrentLock.Rotation = true;
    } 

    public void CloseVolumeWindow()
    {
        // bgTransition.TransitionOut();
        volumeTransition.TransitionOut( () => { IsVolumeWindowOpened = false; });

        AppGameManager.Instance.CurrentLock.Rotation = false;
    } 

    public void OnCloseButtonClicked()
    {
        if( IsOpen == true ) Close();
        if( IsVolumeWindowOpened == true ) CloseVolumeWindow();

        AppGameManager.Instance.SetMoveUI( true );
        AppGameManager.Instance.ShowHeader();
    }

    public void OnVolumeSliderValueChanged( float value )
    {
        // Debug.Log( value );
        AppGameManager.Instance.SoundController.SetVolume( value );
    }

    public void SetVolumeWindowPosition()
    {
        if( canvasScaler == null ) canvasScaler = GetComponent<CanvasScaler>();

        var _left = AppGameManager.Instance.HtmlMenu_SoundButtonRect_Left;
        var _top = AppGameManager.Instance.HtmlMenu_SoundButtonRect_Top;
        var _screenWidth = AppGameManager.Instance.CurrentScreen.x;

        if( _left == 0 || _screenWidth == 0 ) return;

        var _ratio = _left / _screenWidth;
        var _width = canvasScaler.referenceResolution.x * _ratio;

        Debug.Log( _width );

        var _aPos = volumeRoot.anchoredPosition;
        _aPos.x = _width;
        volumeRoot.anchoredPosition = _aPos;
    }


    void OnScreenSizeChanged( Vector2 size )
    {
        SetVolumeWindowPosition();
    }

    public void OpenOperation()
    {
        var _mobile = ( AppGameManager.Instance.Platform.OS == AppGameManager.OS.iOS || AppGameManager.Instance.Platform.OS == AppGameManager.OS.Android );        

        if( operationPcRect == null ) operationPcRect = operationPcTransition.gameObject.GetComponent<RectTransform>();
        if( operationMobileRect == null ) operationMobileRect = operationMobileTransition.gameObject.GetComponent<RectTransform>();
        if( operationPcRawImage == null ) operationPcRawImage = operationPcTransition.gameObject.GetComponent<RawImage>();
        if( operationMobileRawImage == null ) operationMobileRawImage = operationMobileTransition.gameObject.GetComponent<RawImage>();

        operationBgTransition.TransitionIn();

        if( _mobile == false )
        {
            SetSize( true );
            if( operationMobileTransition.IsOpen == true ) operationMobileTransition.TransitionOut();
            operationPcTransition.TransitionIn();
        }
        else
        {
            SetSize( false );
            if( operationPcTransition.IsOpen == true ) operationPcTransition.TransitionOut();
            operationMobileTransition.TransitionIn();
        }
    }

    
    void SetSize( bool isPc )
    {
        var _horizontal = ( Screen.width > Screen.height );
        var _rect = ( isPc == true ) ? operationPcRect : operationMobileRect;
        var _img = ( isPc == true ) ? operationPcRawImage : operationMobileRawImage;
        var _exp = ( _horizontal == true ) ? 0.8f : 0.95f;

        var _ratio = (float)_img.texture.height / (float)_img.texture.width;
        var _w = Screen.width * _exp;
        var _h = _w * _ratio;

        var _current = _rect.sizeDelta;
        _current.x = _w;
        _current.y = _h;
        _rect.sizeDelta = _current;
        
    }

    public void OnSideBarOperationButtonClicked()
    {
        OpenOperation();
    }

    public void OnSideBarInformationButtonClicked()
    {
        Debug.Log( "Information" );
    }

    public void OnSideBarSettingButtonClicked()
    {
        Debug.Log( "設定" );
    }

    public void OnOperationCloseButtonClicked()
    {
        operationBgTransition.TransitionOut();
        if( operationMobileTransition.IsOpen == true ) operationMobileTransition.TransitionOut();
        if( operationPcTransition.IsOpen == true ) operationPcTransition.TransitionOut();
    }
}
