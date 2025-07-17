using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] public TMP_Text cost;
    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Description;
    [SerializeField] public SpriteRenderer Image;
    [SerializeField] public TMP_Text Damage;
    [SerializeField] public TMP_Text Life;
    [SerializeField] public TMP_Text Shield;
    [SerializeField] public TMP_Text Durability;
    [SerializeField] public GameObject Wrapper;
    [SerializeField] private LayerMask DropAreaLayer;

    public bool isDragging = false;
    public CardSlotView Active_cardSlot;
    private Vector3 OriginalPos;
    private Quaternion OriginalRotation;

    public Card Card { get; private set; }

    public void Setup(Card card)
    {
        Card = card;
        Title.text = card.Title;
        Description.text = card.Description;
        cost.text = card.cost.ToString();
        Image.sprite = card.Image;

        if (!card.IsSpell)
        {
            Life.gameObject.SetActive(true);
            Damage.gameObject.SetActive(true);
            Shield.gameObject.SetActive(true);
            Durability.gameObject.SetActive(true);

            Life.text = card.life.ToString();
            Damage.text = card.damage.ToString();
            Shield.text = card.Shield.ToString();
            Durability.text = card.Durability.ToString();
        }
        else
        {
            Life.gameObject.SetActive(false);
            Damage.gameObject.SetActive(false);
            Shield.gameObject.SetActive(false);
            Durability.gameObject.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        if (isDragging) return;
        Wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, -2.5f, 0);
        CardViewHover.Instance.Show(Card, pos);
    }

    void OnMouseExit()
    {
        if (isDragging) return;
        CardViewHover.Instance.Hide();
        Wrapper.SetActive(true);
    }

    void OnMouseDown()
    {
        isDragging = true;
        OriginalPos = transform.position;
        OriginalRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        CardViewHover.Instance.Hide();
        Wrapper.SetActive(true);
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            if (ManaSystem.Instance.HasEnoughMana(Card.cost))
            {
                if (Card.IsSpell)
                {
                    if (Physics.Raycast(transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit hit, 10f, DropAreaLayer))
                    {
                        isDragging = false;
                        PlayCardGA playCardGA = new(Card);
                        ActionSystem.Instance.Perform(playCardGA);
                    }
                    else
                    {
                        returnCardToHand();
                    }
                }
                else
                {
                    if (Active_cardSlot != null)
                    {
                        isDragging = false;
                        if (Active_cardSlot.currentPermanent == null)
                        {
                            SummonGA summonGA = new(Card, Active_cardSlot);
                            ActionSystem.Instance.Perform(summonGA);
                        }
                        else
                        {
                            returnCardToHand();
                        }
                    }
                    else
                    {
                        returnCardToHand();
                    }
                }
            }
            else
            {
                returnCardToHand();
            }
        }
    }

    public void returnCardToHand()
    {
        isDragging = false;
        transform.DOMove(OriginalPos, 0.25f).SetEase(Ease.InOutBack);
        transform.DORotate(OriginalRotation.eulerAngles, 0.25f).SetEase(Ease.OutCubic);
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.DOMove(mousePos, 0.25f).SetEase(Ease.OutCubic);
        }
    }
}
