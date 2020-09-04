using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CanvasController : MonoBehaviour
{

    public GameObject spaceshipObject;
    public GameObject upgradeButton;
    public GameObject upgradePanel;
    public GameObject closeUpgradePanelButton;
    public GameObject upgradeMiningPowerButton;
    public GameObject miningPowerText;
    public GameObject upgradeShipSpeedButton;
    public GameObject shipSpeedText;
    public GameObject upgradeShipRotationButton;
    public GameObject shipRotationText;
    public GameObject coinsText;
    public GameObject resetDragButton;
    private Spaceship spaceship;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = upgradeButton.GetComponent<Button>();
		btn.onClick.AddListener(upgradeButtonOnClick);
        btn = closeUpgradePanelButton.GetComponent<Button>();
		btn.onClick.AddListener(closeUpgradePanelButtonOnClick);
        btn = upgradeMiningPowerButton.GetComponent<Button>();
		btn.onClick.AddListener(upgradeMiningPowerButtonOnClick);
        btn = upgradeShipSpeedButton.GetComponent<Button>();
		btn.onClick.AddListener(upgradeShipSpeedButtonOnClick);
        btn = upgradeShipRotationButton.GetComponent<Button>();
		btn.onClick.AddListener(upgradeShipRotationButtonOnClick);
        
        upgradePanel.SetActive(false);
        spaceship = spaceshipObject.GetComponent<Spaceship>();

        resetDragButton.SetActive(false);
        btn = resetDragButton.GetComponent<Button>();
		btn.onClick.AddListener(resetDrag);
    }

    public void startDrag() {
        resetDragButton.SetActive(true);
    }

    void upgradeButtonOnClick(){
		upgradePanel.SetActive(true);
        Globals.panelOpened = true;
	}

    void upgradeMiningPowerButtonOnClick() {
        bool upgraded = spaceship.upgradeMiningPower();
        if(upgraded == true) {
            refreshCoinsText();
            refreshMiningPowerText();
            refreshUpgradeMiningPowerButton();
        }
        
    }

    void upgradeShipSpeedButtonOnClick() {
        bool upgraded = spaceship.upgradeShipSpeed();
        if(upgraded == true) {
            refreshCoinsText();
            refreshShipSpeedText();
            refreshUpgradeShipSpeedButton();
        }
        
    }

    void upgradeShipRotationButtonOnClick() {
        bool upgraded = spaceship.upgradeShipRotation();
        if(upgraded == true) {
            refreshCoinsText();
            refreshShipRotationText();
            refreshUpgradeShipRotationButton();
        }
        
    }

    public void refreshCoinsText() {
        coinsText.GetComponent<TextMeshProUGUI>().SetText("Coins: " + spaceship.coins);
    }

    void refreshMiningPowerText() {
        miningPowerText.GetComponent<TextMeshProUGUI>().SetText("Now: " + spaceship.miningPower);
    }

    void refreshUpgradeMiningPowerButton() {
        upgradeMiningPowerButton.GetComponentInChildren<Text>().text = "Cost: " + spaceship.miningPowerUpgrade;
    }

    void refreshShipSpeedText() {
        shipSpeedText.GetComponent<TextMeshProUGUI>().SetText("Now: " + spaceship.shipSpeed);
    }

    void refreshUpgradeShipSpeedButton() {
        upgradeShipSpeedButton.GetComponentInChildren<Text>().text = "Cost: " + spaceship.shipSpeedUpgrade;
    }

    void refreshShipRotationText() {
        shipRotationText.GetComponent<TextMeshProUGUI>().SetText("Now: " + spaceship.shipRotation);
    }

    void refreshUpgradeShipRotationButton() {
        upgradeShipRotationButton.GetComponentInChildren<Text>().text = "Cost: " + spaceship.shipRotationUpgrade;
    }

    void closeUpgradePanelButtonOnClick() {
       upgradePanel.SetActive(false);
       Globals.panelOpened = false;
    }

    void resetDrag() {
        resetDragButton.SetActive(false);
        Globals.inputDisabled = false;
        spaceship.resetDrag();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
