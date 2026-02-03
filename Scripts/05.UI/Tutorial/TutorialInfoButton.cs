using System.Collections.Generic;
using UnityEngine;

public class TutorialInfoButton : MonoBehaviour
{
    [SerializeField] private TutorialPopup tutorialPopup;
    [SerializeField] private List<TutorialPage> tutorialPages;

    private void Start()
    {
        GameManager.Instance.TutorialInfoButton = this;
    }

    public void OnClickInfo()
    {
        if (tutorialPopup == null || tutorialPages == null || tutorialPages.Count == 0)
            return;

        tutorialPopup.Open(tutorialPages);
    }
}
