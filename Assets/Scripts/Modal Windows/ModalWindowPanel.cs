using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowPanel : MonoBehaviour
{
    [SerializeField] private Transform Box;

    [Header("Header")]
    [SerializeField] private Transform headerArea;
    [SerializeField] private TextMeshProUGUI titleField;

    [Header("Content")]
    [SerializeField] private Transform contentArea;
    [SerializeField] private Transform verticalLayoutArea;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Space()]
    [SerializeField] private Transform horisontalLayoutArea;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI iconText;

    [Header("Footer")]
    [SerializeField] private Transform footerArea;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button alternativeButton;

    private Action onConfirmAction;
    private Action onCancelAction;
    private Action onAlternativeAction;

    public void Confirm()
    {
        onConfirmAction?.Invoke();
        Close();
    }

    public void Cancel()
    {
        onCancelAction?.Invoke();
        Close();
    }
    public void Alternative()
    {
        onAlternativeAction?.Invoke();
        Close();
    }

    private void Close()
    {
        Box.gameObject.SetActive(false);
    }

    public void ShowTutorial(string title, Sprite imageToShow, string message, Action confirmAction = null, Action cancelAction = null, Action alternativeAction = null)
    {
        horisontalLayoutArea.gameObject.SetActive(false); // Show horisontal layout
        verticalLayoutArea.gameObject.SetActive(true);

        bool hasTitle = string.IsNullOrEmpty(title); // Set title if is needed
        headerArea.gameObject.SetActive(hasTitle);
        titleField.text = title;

        tutorialImage.sprite = imageToShow; // Set tutorial image and text
        tutorialText.text = message;

        bool hasConfirm = (alternativeAction != null); // Set action to confirm button if given
        confirmButton.gameObject.SetActive(hasConfirm);
        onConfirmAction = confirmAction;

        bool hasCancel = (cancelAction != null);
        alternativeButton.gameObject.SetActive(hasCancel); // Set action to cancel button if given
        onCancelAction = cancelAction;
        
        bool hasAlternative = (alternativeAction != null); // Set action to alternative button if given
        alternativeButton.gameObject.SetActive(hasAlternative);
        onAlternativeAction = alternativeAction;
    }  
}
