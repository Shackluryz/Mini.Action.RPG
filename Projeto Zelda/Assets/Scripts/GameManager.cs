using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public enum enemyState
{
    IDLE, ALERT, PATROL, FOLLOW, FURY
}

public class GameManager : MonoBehaviour
{
    public Transform player;
    
    [Header("Slime IA")]
    public float slimeIdleWaitTime;
    public Transform[] slimeWayPoints;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay = 1f;

    [Header("Rain Manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainParticle;
    private ParticleSystem.EmissionModule rainModule;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    private void Start() {
        rainModule = rainParticle.emission;    
    }

    public void OnOffRain(bool isRain) {
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("RainManager", isRain);
        StartCoroutine("PostBManager", isRain);

    }

    IEnumerator RainManager(bool isRain) {

        if (isRain) {
            for (float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r += rainIncrement) {
                rainModule.rateOverTime = r;
                yield return new WaitForSeconds(rainIncrementDelay);
            }
            rainModule.rateOverTime = rainRateOverTime;
        } else {
            for (float r = rainModule.rateOverTime.constant; r > 0; r -= rainIncrement) {
                rainModule.rateOverTime = r;
                yield return new WaitForSeconds(rainIncrementDelay);
            }
            rainModule.rateOverTime = 0;
        }
    
    }

    IEnumerator PostBManager(bool isRain) {
        if (isRain) {
            for (float w = postB.weight; w < 1; w += 1 * Time.deltaTime) {
                postB.weight = w;
                yield return new WaitForEndOfFrame();
            }
            postB.weight = 1;
        } else {
            for (float w = postB.weight; w > 0; w -= 1 * Time.deltaTime) {
                postB.weight = w;
                yield return new WaitForEndOfFrame();
            }
            postB.weight = 0;
        }
    }
}
