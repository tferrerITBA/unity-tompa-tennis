﻿using UnityEngine;

public class TennisVariables : MonoBehaviour
{
    public float WalkSpeed = 5f;
    public float RunSpeed = 10f;
    public float SprintSpeed = 15f;
    public float BackSpeed = 7f;

    public float ServiceTossSpeed = 20f;
    public float ServiceSpeed = 65f;
    public float ServiceYAttenuation = 325f;
    public float DeepHitSpeed = 55f;
    public float DeepHitYAttenuation = 225f;
    public float DropshotHitSpeed = 30f;
    public float DropshotHitYAttenuation = 150f;
    public float DeepVolleyHitSpeed = 55f;
    public float DeepVolleyHitYAttenuation = 375f;
    public float DropshotVolleyHitSpeed = 20f;
    public float DropshotVolleyHitYAttenuation = 100f;

    public float BallHitTargetRadius = 3f;
    public float BallServeTargetRadius = 2f;
    
    public float DeepBallBounceFrictionMultiplier = 0.75f;
    public float DropshotBallBounceFrictionMultiplier = 0.3f;
    public float BallBounciness = 0.8f;
    public float NetBounceFrictionMultiplier = 0.25f;
    public float DefaultBounceFrictionMultiplier = 0.3f;
    
    public float FastHitAnimationThresholdSpeed = 25f;
    
    public float BallColliderFrontDelta = 2f;

    public float AISpeedEasy = 7.5f;
    public float AISpeedNormal = 10f;
    public float AISpeedHard = 12.5f;

    [Range(0, 1)] public float AIVolleyModeProbabilityEasy = 0.15f;
    [Range(0, 1)] public float AIVolleyModeProbabilityNormal = 0.20f;
    [Range(0, 1)] public float AIVolleyModeProbabilityHard = 0.25f;
    
    public float MaxReactionTimeEasy = 1.15f;
    public float MaxReactionTimeNormal = 1f;
    public float MaxReactionTimeHard = 0.85f;
}
