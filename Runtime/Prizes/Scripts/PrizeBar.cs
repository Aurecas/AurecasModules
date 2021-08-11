using AurecasLib.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PrizeBar : PopupWindow
{

    public float[] milestones;
    public ProgressBar progressBar;
    public GameObject waitBar;
    public GameObject milestoneObjectPrefab;
    public TextMeshProUGUI waitBarText;
    public float milestoneHeight;
    public float speed;
    public float initialDelay;
    public int hoursCooldown;
    public Button nextButton;

    List<RectTransform> instancedMilestones;

    float barPosition;
    float targetBarPosition;
    float overflow;
    float barSize;
    bool nextRequested;
    DateTime nextBarTime;

    Func<int, IEnumerator> onMilestone;
    Action<float> onOverflow;

    public void SetBarContents(float currentBarPosition, float maxBar, float increase, Func<int, IEnumerator> onMilestone, Action<float> onOverflow) {
        nextButton.interactable = false;
        barPosition = currentBarPosition;
        targetBarPosition = currentBarPosition + increase;
        barSize = maxBar;
        this.onMilestone = onMilestone;
        this.onOverflow = onOverflow;
        progressBar.maxValue = maxBar;
        progressBar.value = currentBarPosition;
        overflow = -1;
        if(targetBarPosition >= maxBar) {
            targetBarPosition = maxBar;
            overflow = currentBarPosition + increase - maxBar;
        }
        waitBar.gameObject.SetActive(false);
        StartCoroutine(BarProgression());
    }

    public void SetBarCooldown(DateTime nextBar) {
        nextBarTime = nextBar;
        waitBar.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);
        nextButton.interactable = true;
        nextButton.onClick.AddListener(() => { ClosePopup(); });
    }

    public void PressNext() {
        nextRequested = true;
    }

    private IEnumerator BarProgression() {

        while (!isOpen) {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSecondsRealtime(initialDelay);

        while(barPosition < targetBarPosition) {
            for(int i = 0; i < milestones.Length; i ++) {
                float milestone = milestones[i];
                float milestonePosition = milestone * barSize;
                if(barPosition < milestonePosition && barPosition + Time.unscaledDeltaTime * speed >= milestonePosition) {
                    //Acabou de passar por uma milestone
                    yield return onMilestone(i); //Espera o callback retornar true
                }
            }
            barPosition += Time.unscaledDeltaTime * speed;
            yield return new WaitForEndOfFrame();
        }

        if (overflow >= 0) {
            onOverflow(overflow);
        }

        nextButton.interactable = true;
        while (!nextRequested) {
            yield return null;
        }
        ClosePopup();
    }

    private void Update() {
        if (instancedMilestones == null) instancedMilestones = new List<RectTransform>();

        while(instancedMilestones.Count < milestones.Length) {
            instancedMilestones.Add(Instantiate(milestoneObjectPrefab, progressBar.transform).GetComponent<RectTransform>());
        }
        while(instancedMilestones.Count > milestones.Length) {
            Destroy(instancedMilestones[instancedMilestones.Count - 1].gameObject);
            instancedMilestones.RemoveAt(instancedMilestones.Count - 1);
        }

        for(int i = 0; i < instancedMilestones.Count; i++) {
            instancedMilestones[i].anchorMin = new Vector2(milestones[i], 1);
            instancedMilestones[i].anchorMax = new Vector2(milestones[i], 1);

            instancedMilestones[i].anchoredPosition = new Vector2(0, milestoneHeight);
        }

        if (Application.isPlaying) {
            progressBar.value += (barPosition - progressBar.value) / 5f * Time.deltaTime * 60f;

            if (nextBarTime != null) {
                TimeSpan span = new TimeSpan(hoursCooldown, 0, 0) - (DateTime.Now - nextBarTime);
                waitBarText.text = string.Format("WAIT FOR NEXT BAR: {0}H {1}M {2}S", span.Hours, span.Minutes, span.Seconds);
            }
        }


    }

}
