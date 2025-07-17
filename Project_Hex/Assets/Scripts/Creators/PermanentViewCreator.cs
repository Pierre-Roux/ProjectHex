using DG.Tweening;
using UnityEngine;

public class PermanentViewCreator : Singleton<PermanentViewCreator>
{
    [SerializeField] private PermanentView PermanentViewPrefab;

    public PermanentView CreatePermanentViewCreator(Card cardReference, CardSlotView Slot)
    {
        PermanentView PermanentView = Instantiate(PermanentViewPrefab, Slot.transform.position, Slot.transform.rotation);
        Slot.currentPermanent = PermanentView;
        PermanentView.CurrentSlot = Slot;
        PermanentView.transform.localScale = Vector3.zero;
        PermanentView.transform.DOScale(PermanentViewPrefab.transform.localScale, 0.15f);
        PermanentView.Setup(cardReference);
        PermanentView.gameObject.name = cardReference.Title + " " + CombatSystem.Instance.Player_Permanents.Count ;
        return PermanentView;
    }
}
