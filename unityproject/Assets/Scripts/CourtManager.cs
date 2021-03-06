﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CourtManager : MonoBehaviour
{
    private const float TotalCourtWidth = 18.0f;
    private const float CourtLength = 39.0f;
    private const float ServingAreaWidth = 13.5f;
    private const float ServingAreaLength = 21.0f;

    [Header("Deep Hit Targets - AI Player Side")]
    public Transform player2BackLeftHit;
    public Transform player2BackCenterHit;
    public Transform player2BackRightHit;
    
    [Header("Deep Hit Targets - Player Side")]
    public Transform player1BackLeftHit;
    public Transform player1BackCenterHit;
    public Transform player1BackRightHit;
    
    [Header("Front Hit Targets - AI Player Side")]
    public Transform player2FrontLeftHit;
    public Transform player2FrontCenterHit;
    public Transform player2FrontRightHit;
    
    [Header("Front Hit Targets - Player Side")]
    public Transform player1FrontLeftHit;
    public Transform player1FrontCenterHit;
    public Transform player1FrontRightHit;
    
    [Header("Service Hit Spots")]
    public Transform player1ServiceSpotLeft;
    public Transform player1ServiceSpotRight;
    public Transform player2ServiceSpotLeft;
    public Transform player2ServiceSpotRight;
    
    [Header("Service Hit Targets - AI Player Left Side")]
    public Transform player2ServiceLeftLeft;
    public Transform player2ServiceLeftCenter;
    public Transform player2ServiceLeftRight;
    
    [Header("Service Hit Targets - AI Player Right Side")]
    public Transform player2ServiceRightLeft;
    public Transform player2ServiceRightCenter;
    public Transform player2ServiceRightRight;
    
    [Header("Service Hit Targets - Player Left Side")]
    public Transform player1ServiceLeftLeft;
    public Transform player1ServiceLeftCenter;
    public Transform player1ServiceLeftRight;
    
    [Header("Service Hit Targets - Player Right Side")]
    public Transform player1ServiceRightLeft;
    public Transform player1ServiceRightCenter;
    public Transform player1ServiceRightRight;
    
    public enum CourtSection
    {
        Player1NoMansLand,
        Player1LeftServiceBox,
        Player1RightServiceBox,
        Player2NoMansLand,
        Player2LeftServiceBox,
        Player2RightServiceBox
    }
    
    public Dictionary<CourtSection, Vector2> courtSections = new Dictionary<CourtSection,Vector2>();

    private ScoreManager _scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreManager = GetComponent<ScoreManager>();
    }

    public Vector3 GetHitTargetPosition(int playerId, HitDirectionVertical? vertical, HitDirectionHorizontal? horiz)
    {
        switch (playerId)
        {
            case 0:
                switch (vertical)
                {
                    case null:
                    case HitDirectionVertical.Deep:
                        return (horiz == HitDirectionHorizontal.Center) ? player2BackCenterHit.position :
                            (horiz == HitDirectionHorizontal.Left) ? player2BackLeftHit.position : player2BackRightHit.position;
                    case HitDirectionVertical.Dropshot:
                        return (horiz == HitDirectionHorizontal.Center) ? player2FrontCenterHit.position :
                            (horiz == HitDirectionHorizontal.Left) ? player2FrontLeftHit.position : player2FrontRightHit.position;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            case 1:
                switch (vertical)
                {
                    case null:
                    case HitDirectionVertical.Deep:
                        return (horiz == HitDirectionHorizontal.Center) ? player1BackCenterHit.position :
                            (horiz == HitDirectionHorizontal.Left) ? player1BackLeftHit.position : player1BackRightHit.position;
                    case HitDirectionVertical.Dropshot:
                        return (horiz == HitDirectionHorizontal.Center) ? player1FrontCenterHit.position :
                            (horiz == HitDirectionHorizontal.Left) ? player1FrontLeftHit.position : player1FrontRightHit.position;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Vector3 GetServiceTargetPosition(int playerId, HitDirectionHorizontal? horiz)
    {
        var serviceSide = _scoreManager.currentServingSide;
        if (serviceSide == ScoreManager.ServingSide.Even)
        {
            if (playerId == 0)
            {
                return (horiz == HitDirectionHorizontal.Center) ? player2ServiceLeftCenter.position :
                    (horiz == HitDirectionHorizontal.Left) ? player2ServiceLeftLeft.position : player2ServiceLeftRight.position;
            }
            else
            {
                return (horiz == HitDirectionHorizontal.Center) ? player1ServiceRightCenter.position :
                    (horiz == HitDirectionHorizontal.Left) ? player1ServiceRightLeft.position : player1ServiceRightRight.position;
            }
        }
        else
        {
            if (playerId == 0)
            {
                return (horiz == HitDirectionHorizontal.Center) ? player2ServiceRightCenter.position :
                    (horiz == HitDirectionHorizontal.Left) ? player2ServiceRightLeft.position : player2ServiceRightRight.position;
            }
            else
            {
                return (horiz == HitDirectionHorizontal.Center) ? player1ServiceLeftCenter.position :
                    (horiz == HitDirectionHorizontal.Left) ? player1ServiceLeftLeft.position : player1ServiceLeftRight.position;
            }
        }
    }

    public CourtTargetDirections SelectFromExtremeTargets(int randomPoolSize, Vector3 position, bool hardDifficulty)
    {
        var targetDistances = new List<TargetDistance>(6);
        targetDistances.Add(new TargetDistance(player1BackLeftHit, Vector3.Distance(position, player1BackLeftHit.position)));
        targetDistances.Add(new TargetDistance(player1BackCenterHit, Vector3.Distance(position, player1BackCenterHit.position)));
        targetDistances.Add(new TargetDistance(player1BackRightHit, Vector3.Distance(position, player1BackRightHit.position)));
        targetDistances.Add(new TargetDistance(player1FrontLeftHit, Vector3.Distance(position, player1FrontLeftHit.position)));
        targetDistances.Add(new TargetDistance(player1FrontCenterHit, Vector3.Distance(position, player1FrontCenterHit.position)));
        targetDistances.Add(new TargetDistance(player1FrontRightHit, Vector3.Distance(position, player1FrontRightHit.position)));
        
        targetDistances.Sort((x,y) => (hardDifficulty? -1 : 1) * x.Distance.CompareTo(y.Distance));

        var chosenTarget = targetDistances[Random.Range(0, randomPoolSize)];
        
        var horizDirection =
            (chosenTarget.Target.Equals(player1BackLeftHit) || chosenTarget.Target.Equals(player1FrontLeftHit)) ?
                HitDirectionHorizontal.Left
                : (chosenTarget.Target.Equals(player1BackCenterHit) || chosenTarget.Target.Equals(player1FrontCenterHit)) ? 
                    HitDirectionHorizontal.Center
                    : HitDirectionHorizontal.Right;
        
        var vertDirection =
            (chosenTarget.Target.Equals(player1BackLeftHit) || chosenTarget.Target.Equals(player1BackCenterHit) || chosenTarget.Target.Equals(player1BackRightHit)) ?
                HitDirectionVertical.Deep : HitDirectionVertical.Dropshot;
        
        return new CourtTargetDirections(horizDirection, vertDirection);
    }
    
    private class TargetDistance
    {
        private Transform target;
        private float distance;

        public TargetDistance(Transform transf, float dist)
        {
            target = transf;
            distance = dist;
        }

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        public float Distance
        {
            get => distance;
            set => distance = value;
        }
    }
}

public class CourtTargetDirections
{
    private HitDirectionHorizontal _hitDirectionHorizontal;
    private HitDirectionVertical _hitDirectionVertical;

    public CourtTargetDirections(HitDirectionHorizontal horiz, HitDirectionVertical vert)
    {
        _hitDirectionHorizontal = horiz;
        _hitDirectionVertical = vert;
    }

    public HitDirectionHorizontal HitDirectionHorizontal
    {
        get => _hitDirectionHorizontal;
        set => _hitDirectionHorizontal = value;
    }

    public HitDirectionVertical HitDirectionVertical
    {
        get => _hitDirectionVertical;
        set => _hitDirectionVertical = value;
    }
}
