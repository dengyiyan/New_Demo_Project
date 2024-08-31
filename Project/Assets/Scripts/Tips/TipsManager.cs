using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{

    [SerializeField] private GameObject tipsPanel;
    [SerializeField] private Button tipsPanelButton;
    [SerializeField] private Button tipsPanelCloseButton;
    [SerializeField] private Text tipsText;
    private bool isShowing = false;
    // Start is called before the first frame update
    void Start()
    {
        TipsPanelOpenSettings();
        TipsPanelClosedSettings();
        tipsPanelButton.onClick.AddListener(OnTipsButtonClicked);
        tipsPanelCloseButton.onClick.AddListener(OnCloseButtonClicked);

        tipsPanelButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventHandler.RegisterTipsEvent += OnTipsRegistered;
        EventHandler.UnregisterTipsEvent += OnTipsUnregistered;
    }

    private void OnDisable()
    {
        EventHandler.RegisterTipsEvent -= OnTipsRegistered;
        EventHandler.UnregisterTipsEvent -= OnTipsUnregistered;
    }

    private void OnTipsRegistered(string txt)
    {
        ChangeTipsText(txt);
        TipsPanelClosedSettings();
        tipsPanelButton.gameObject.SetActive(true);
    }

    private void OnTipsUnregistered()
    {
        TipsPanelClosedSettings();
        tipsPanelButton.gameObject.SetActive(false);
    }

    private void ChangeTipsText(string txt)
    {
        tipsText.text = txt;
    }

    private void OnTipsButtonClicked()
    {
        TipsPanelOpenSettings();
    }

    private void OnCloseButtonClicked()
    {
        TipsPanelClosedSettings();
    }

    private void TipsPanelClosedSettings()
    {
        isShowing = false;
        tipsPanel.SetActive(false);
        tipsPanelButton.gameObject.SetActive(true);
        //EventHandler.CallEnableCursorEvent();
        // EventHandler.CallEnablePlayerMovementEvent();
    }

    private void TipsPanelOpenSettings()
    {
        isShowing = true;
        tipsPanel.SetActive(true);
        tipsPanelButton.gameObject.SetActive(false);
        //EventHandler.CallDisableCursorEvent();
        // EventHandler.CallDisablePlayerMovementEvent();
    }
}
