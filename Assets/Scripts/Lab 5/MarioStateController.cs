using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioStateController : StateController
{
    private PlayerController marioController;
    public PowerupType currentPowerupType = PowerupType.Default;
    public MarioState shouldBeNextState = MarioState.Default;
    private SpriteRenderer spriteRenderer;

    // for starman
    [Header("Starman")]
    public LayerMask starIgnoreLayers;
    private Collider2D marioCollider;

    void Awake()
    {
        marioController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        marioCollider = GetComponent<Collider2D>();
    }

    public override void Start()
    {
        base.Start();
        GameRestart(); // clear powerup in the beginning, go to start state
    }

    public override void OnSetupState()
    {
        base.OnSetupState();
        if (marioController != null && !marioController.IsTransforming) marioController.RefreshAnimatorState();
    }

    // this should be added to the GameRestart EventListener as callback
    public void GameRestart()
    {
        EndStar();
        // clear powerup
        currentPowerupType = PowerupType.Default;
        // set the start state
        TransitionToState(startState);
    }

    public void SetPowerup(PowerupType i)
    {
        if (currentPowerupType == PowerupType.Kill) return;
        currentPowerupType = i;
    }

    public void SetRendererToFlicker()
    {
        StartCoroutine(BlinkSpriteRenderer());
    }
    private IEnumerator BlinkSpriteRenderer()
    {
        State blinkState = currentState;
        spriteRenderer.enabled = true;

        // let the other setup actions run first (e.g. the shrink animation sets IsTransforming)
        yield return null;
        yield return new WaitUntil(() => !marioController.IsTransforming);
        while (string.Equals(currentState.name, "InvincibleSmallMario", StringComparison.OrdinalIgnoreCase))
        {
            // Toggle the visibility of the sprite renderer
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Wait for the specified blink interval
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
    }

    public void StartStar()
    {
        marioCollider.excludeLayers = starIgnoreLayers;
    }

    public void EndStar()
    {
        marioCollider.excludeLayers = 0;
    }

    public void Fire()
    {
        this.currentState.DoEventTriggeredActions(this, ActionType.Attack);
    }

}