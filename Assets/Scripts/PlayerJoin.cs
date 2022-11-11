using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoin : MonoBehaviour
{

    [SerializeField] private int PlayerIndexICareAbout;

    private void Awake()
    {
        EventManager.PlayerOneJoinedEvent += PlayerOneJoined;
        EventManager.PlayerTwoJoinedEvent += PlayerTwoJoined;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        EventManager.PlayerOneJoinedEvent -= PlayerOneJoined;
        EventManager.PlayerTwoJoinedEvent -= PlayerTwoJoined;
    }

    private void PlayerOneJoined()
    {
        if (PlayerIndexICareAbout == 1) return;
        gameObject.SetActive(false);
    }

    private void PlayerTwoJoined()
    {
        if (PlayerIndexICareAbout == 0) return;
        // Plaer two has joined, lets start the Play
        gameObject.SetActive(false);
        GameManager.OnPlay();
    }
}
