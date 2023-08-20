using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlacableButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]private Placable placable;
    [SerializeField]private Image image;
    [SerializeField]private TextMeshProUGUI text;

    private void Start() {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerConfiguration config 
            = eventData.currentInputModule.gameObject
                .GetComponent<PlayerConfigurationReferenceHelper>()
                    .GetPlayerConfigurationReference();
        Debug.Log(
            $"Player {config.playerIndex} has clicked button {placable.name}");
        GameManager.Instance.PlayerPicked(config.playerIndex, placable);
        Destroy(this.gameObject);
    }

    public void SetPlacable(Placable p){
        placable = p;
        image.sprite = placable.GetSprite();
        text.text = placable.GetName();
    }
}
