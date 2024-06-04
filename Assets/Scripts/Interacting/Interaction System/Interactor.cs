using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private bool _enableInteractableOutline;

    private IInteractable _activeInteractable;
    private SpriteOutliner _activeInteractableOutliner;
    private GameObject _activeInteractableGameObject;
    private bool _isEnabled;

    public bool IsEnabled => _isEnabled;

    private void Start()
    {
        _isEnabled = true;
    }

    private void FixedUpdate()
    {
        if (!_isEnabled) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _interactionRadius, _interactableLayerMask, -1f, 1f);

        if (colliders.Length <= 0)
        {
            if (_activeInteractable != null)
            {
                DeselectActiveInteractable();
            }

            return;
        }

        Collider2D closestCollider = GetClosestCollider(colliders);

        if (closestCollider != null)
        {
            if (closestCollider.gameObject != _activeInteractableGameObject)
            {
                DeselectActiveInteractable();
                SelectInteractable(closestCollider.gameObject);
            }
        }
    }

    private Collider2D GetClosestCollider(Collider2D[] colliders)
    {
        float shortestSqrDistance = _interactionRadius;
        int shortestDistanceIndex = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 closestPoint = colliders[i].ClosestPoint(transform.position);
            float sqrDistance = (closestPoint - transform.position).sqrMagnitude;

            if (sqrDistance < shortestSqrDistance)
            {
                shortestSqrDistance = sqrDistance;
                shortestDistanceIndex = i;
            }
        }

        return colliders[shortestDistanceIndex];
    }

    private void SelectInteractable(GameObject gameObject)
    {
        _activeInteractableGameObject = gameObject;
        _activeInteractable = gameObject.GetComponentInParent<IInteractable>();
        if (_activeInteractable == null)
        {
            Debug.Log($"GameObject: \"{gameObject.name}\" doesn't have component, inherited from \"IInteractable\".");
        }

        if (_activeInteractable.GetInteractions() == null)
        {
            DeselectActiveInteractable();
            return;
        }

        if (_enableInteractableOutline)
        {
            _activeInteractableOutliner = gameObject.GetComponentInParent<SpriteOutliner>();
            if (_activeInteractableOutliner != null)
            {
                _activeInteractableOutliner.EnableOutline();
            }
            else
            {
                Debug.Log($"Interactable on GameObject: \"{gameObject.name}\" doesn't have \"SpriteOutliner\" component");
            } 
        }
    }

    private void DeselectActiveInteractable()
    {
        _activeInteractableGameObject = null;

        if (_activeInteractable == null) return;

        _activeInteractable = null;

        if (_enableInteractableOutline)
        {
            if (_activeInteractableOutliner == null) return;

            _activeInteractableOutliner.DisableOutline();
            _activeInteractableOutliner = null;
        }
    }

    public void Interact() // Interaction with active interactable
    {
        if (_activeInteractable != null)
        {
            Interaction selectedInteraction = _activeInteractable.GetInteractions()[0]; // might not be hard-coded zero if interactions choice will be possible.
            _activeInteractable.PerformInteraction(selectedInteraction, this);
            //selectedInteraction.AcceptInteractionVisitor(this);
        }
    }

    public void Enable()
    {
        _isEnabled = true;
    }

    public void Disable()
    {
        _isEnabled = false;

        DeselectActiveInteractable();
    }

    public void InteractWithGivenInteractable(IInteractable interactable) // Interaction with given interactable (no distance-checks)
    {
        Interaction interaction = interactable.GetInteractions()[0]; // might not be hard-coded zero if interactions choice will be possible.
        interactable.PerformInteraction(interaction, this);
        //interaction.AcceptInteractionVisitor(this);
    }
/*
    public abstract void Visit(PickUpInteraction pickUpInteraction);

    public abstract void Visit(InitiateDialogInteraction dialogInteraction);

    public abstract void Visit(SwitchSceneInteraction switchSceneInteraction);

    public abstract void Visit(TaskAssignmentInteraction taskAssignmentInteraction);

    public abstract void Visit(SimpleInteraction simpleInteraction);*/
}
