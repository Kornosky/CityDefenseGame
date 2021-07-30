using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FireBallAbility", menuName = "Ability/FireBallAbility", order = 0)]
public class FireBallAbility : AbilityScriptableObject
{
    public int fireDamage;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject fireArrow;
    [SerializeField] UnitScriptableObject damageInfo;
    [SerializeReference] float shakeAmount = 1.1f;
    [SerializeReference] float shakeLength = .4f;
    bool isLaunching;
    public override void Init()
    {
        isLaunching = false;
      //  throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        if(Input.touches.Length > 0 && !isLaunching)
        {
            isLaunching = true;
            
            GameManager.Instance.StartCoroutine(Launching());
        }
        
    }

    IEnumerator Launching()
    {
        bool isUnlaunched = true;
        GameObject pointingArrow = Instantiate(arrow, PlayerManager.Instance.playerBase.position, Quaternion.identity);
        Vector2 touchLocation = Vector2.zero;
        CameraController.Instance.EnableCameraTouch(false);
        while (isUnlaunched)
        { 
            if (Input.touches.Length > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchLoc = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

                var v_diff = (touchLoc - pointingArrow.transform.position);
                var atan2 = Mathf.Atan2(v_diff.y, v_diff.x);
                pointingArrow.transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
                // Handle finger movements based on touch phase.
                switch (touch.phase)
                {
                    // Record initial touch position.
                    case TouchPhase.Began:
                        break;

                    // Determine direction by comparing the current touch position with the initial one.
                    case TouchPhase.Moved:
                       

                        break;

                    // Report that a direction has been chosen when the finger is lifted.
                    case TouchPhase.Ended:
                        touchLocation = touch.position;
                        break;
                }
            }
            else
            {
                isUnlaunched = false;
            }
             yield return new WaitForEndOfFrame();
        }
        Destroy(pointingArrow);
        CameraController.Instance.EnableCameraTouch(true);
        GameObject projectile = Instantiate(fireArrow, PlayerManager.Instance.playerBase.position, Quaternion.identity);
        var proj = projectile.GetComponent<Projectile>() ?? projectile.AddComponent<GenericProjectile>();
        if (proj != null)
        {
            projectile.GetComponent<Projectile>().Init(damageInfo, false);
            projectile.GetComponent<Projectile>().LaunchProjectile((Vector3)touchLocation - PlayerManager.Instance.playerBase.position, false);
        }
           
        isLaunching = false;
    }


    protected override void OnFinished()
    {
        //Effects
        if(vfxPrefab)
        Instantiate(vfxPrefab, unit.transform.position, Quaternion.identity);

        CameraController.Instance.CameraShake(shakeLength, shakeAmount);

        OnCompleted();
    }
}
