using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Hint Popup")]
    public GameObject hintPanel;
    public Image hintImage;

    [Header("Result Popup")]
    public GameObject resultPanel;
    public Image resultImage;

    public Sprite correctResultImage;
    public Sprite wrongResultImage;

    public float popupDuration = 2f;

    [Header("Education Popup")]
    public GameObject educationPanel;
    public Image educationImage;

    private Coroutine popupRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        hintPanel.SetActive(false);
        resultPanel.SetActive(false);
        educationPanel.SetActive(false);
    }

    //==========================
    // HINT
    //==========================

    public void ShowHint(WasteItem waste)
    {
        hintImage.sprite = waste.hintImage;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    //==========================
    // RESULT
    //==========================

    public void ShowResult(bool correct)
    {
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        resultPanel.SetActive(true);

        if (correct)
            resultImage.sprite = correctResultImage;
        else
            resultImage.sprite = wrongResultImage;

        popupRoutine = StartCoroutine(HidePopup());
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(popupDuration);

        resultPanel.SetActive(false);
    }

    //==========================
    // EDUCATION
    //==========================

    public void ShowEducation(WasteItem waste)
    {
        educationImage.sprite = waste.educationImage;
        educationPanel.SetActive(true);
    }

    public void CloseEducation()
    {
        educationPanel.SetActive(false);
    }
}