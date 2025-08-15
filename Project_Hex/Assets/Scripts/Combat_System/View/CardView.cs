using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] public TMP_Text cost;
    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Description;
    [SerializeField] public SpriteRenderer Image;
    [SerializeField] public TMP_Text Life;
    [SerializeField] public TMP_Text Durability;
    [SerializeField] public GameObject Wrapper;
    [SerializeField] private LayerMask DropAreaLayer;

    public bool IsReward;
    public bool RewardTaken;
    [HideInInspector] public ShopSlot shopSlot;

    public bool isDragging = false;

    private Vector3 OriginalPos;
    private Quaternion OriginalRotation;

    public Card Card { get; private set; }

    public void Setup(Card card)
    {
        Card = card;
        Title.text = card.Title;
        name = Title.text;
        if (card.DecayCounter <= 0)
        {
            Description.text = card.Description;
        }
        else
        {
            Description.text = card.Description + "\n" + "Decay " + card.DecayCounter.ToString();
        }
        cost.text = card.cost.ToString();
        Image.sprite = card.Image;

        if (!card.IsSpell)
        {
            Life.gameObject.SetActive(true);
            Durability.gameObject.SetActive(true);

            Life.text = card.life.ToString();
            Durability.text = card.Durability.ToString() + "/" + card.MaxDurability.ToString();
        }
        else
        {
            Life.gameObject.SetActive(false);
            Durability.gameObject.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        if (!IsReward)
        {
            if (ActionSystem.Instance.IsPerforming) return;
            if (!CombatSystem.Instance.Interactable) return;
            if (isDragging) return;
            Wrapper.SetActive(false);
            Vector3 pos = new(transform.position.x, -2.5f, 0);
            CardViewHover.Instance.Show(Card, pos);
        }
    }

    void OnMouseExit()
    {
        if (!IsReward)
        {
            if (isDragging) return;
            CardViewHover.Instance.Hide();
            Wrapper.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        if (!IsReward)
        {
            if (ActionSystem.Instance.IsPerforming) return;
            if (!CombatSystem.Instance.Interactable) return;
            isDragging = true;
            OriginalPos = transform.position;
            OriginalRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            CardViewHover.Instance.Hide();
            Wrapper.SetActive(true);
        }
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
                    GameObject Parent = null;
                    switch (Card.permanentType)
                    {
                        case PermanentType.Weapon:
                            Parent = CombatSystem.Instance.PlayerWeaponZone.gameObject;
                            break;
                        case PermanentType.Shield:
                            Parent = CombatSystem.Instance.PlayerShieldZone.gameObject;
                            break;
                        case PermanentType.Support:
                            Parent = CombatSystem.Instance.PlayerSupportZone.gameObject;
                            break;
                        default:
                            Debug.Log("No Type For Perm " + Card.data.name);
                            break;
                    }
                    if (Parent != null)
                    {
                        int childCount = Parent.transform.childCount;
                        if (childCount >= CombatSystem.Instance.MaxPermPlayer)
                        {
                            // LimitReached
                            returnCardToHand();
                        }
                        else
                        {
                            isDragging = false;
                            SummonGA summonGA = new(Card);
                            ActionSystem.Instance.Perform(summonGA);
                        }
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
