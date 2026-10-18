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

    void Awake()
    {
        marioController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    public void Fire()
    {
        this.currentState.DoEventTriggeredActions(this, ActionType.Attack);
    }

}