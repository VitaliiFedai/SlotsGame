using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class GameStatisticsPanel : PanelUI
{
    [SerializeField] private RecordsHandler _recordsHandler;

    private const string CONTAINER_NAME = "container";
    private const string RECORD_PANEL_NAME = "record-panel";
    private const string DESCRIPTION_LABEL_NAME = "description-label";
    private const string VALUE_LABEL_NAME = "value-label";
    private const string BACK_BUTTON_NAME = "back-button";

    private const string LEVEL_DESCRIPTION = "Level";
    private const string BET_DESCRIPTION = "Max Bet per line";

    public event Action OnBackButtonClicked;
    private int _level;
    private int _betPerLine;

    public void SetLevel(int level)
    {
        _level = level;
        Refresh();
    }

    public void SetMaxBetToLine(int value)
    {
        _betPerLine = value;
        Refresh();
    }

    protected override void OnBind()
    {
        Refresh();
        RegisterCallbacks();
    }

    protected override void OnUnbind()
    {
    }

    public void Refresh()
    {
        if (GetRoot() == null)
        {
            return;
        }

        VisualElement container = GetElement<VisualElement>(CONTAINER_NAME);

        List<VisualElement> recordPanels = container.Query(RECORD_PANEL_NAME).ToList();

        SetRecordPanelData(recordPanels[0], LEVEL_DESCRIPTION, _level.ToString());
        SetRecordPanelData(recordPanels[1], BET_DESCRIPTION, _betPerLine.ToString());

        int i = 2;
        foreach (IRecord record in _recordsHandler.Records)
        {
            VisualElement recordPanel = recordPanels[i++];

            SetRecordPanelData(recordPanel, record.GetDescription(), record.GetAmount().ToString());

            if (i == recordPanels.Count + 2)
            {
                break;
            }
        }
    }

    private void SetRecordPanelData(VisualElement recordPanel, string description, string value)
    {
        Label descriptionLabel = recordPanel.Q<Label>(DESCRIPTION_LABEL_NAME);
        Label valueLabel = recordPanel.Q<Label>(VALUE_LABEL_NAME);

        descriptionLabel.text = description;
        valueLabel.text = value;
    }

    private void RegisterCallbacks()
    {
        RegisterOnClickCallback(BACK_BUTTON_NAME, OnBackButtonClicked);
    }
}
