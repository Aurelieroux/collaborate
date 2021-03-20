﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class Core_Comportement : MonoBehaviour
{
    private Core_Manager _coreManager;
    private Core_Attack _coreAttack;

    public float patrolRadius;
    public GameObject patrolTarget;
    public GameObject fearTarget;
    private bool canDoTarget = true;
    
    public bool isFear;
    public bool canBeFear = true;

    public float fearCooldown;

    public Collider fearCollider;

    public Vector3 closestPoint;

    public bool canAttackDistance;
    


    private void OnEnable()
    {
        _coreManager = GetComponent<Core_Manager>();
        _coreAttack = GetComponent<Core_Attack>();
    }

    private void Start()
    {
        fearCollider = _coreManager.player.GetComponentInChildren<SphereCollider>();
    }


    private void Update()
    {
        ComportementPalier();
        
        if (_coreManager.agent.remainingDistance < 5f && isFear)
        {
            print("j'ai plus peur");
                
            StartCoroutine(fuiteCoolDown());
            isFear = false;
        }


        if (canAttackDistance && !_coreAttack.isDIstAttack)
        {
            StartCoroutine(_coreAttack.DistAttack());

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_coreManager.palier == "petit")
            {
                if (canBeFear)
                {
                    StartCoroutine(fuite()); 
                }
            }

            if (_coreManager.palier == "moyen" || _coreManager.palier == "grand")
            {
                canAttackDistance = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttackDistance = false;
        }
    }


    void CreatePatrolTarget()
    {
        Vector3 rdPosition = Random.insideUnitSphere * patrolRadius;
        rdPosition.y = transform.position.y;
        

        _coreManager.target = rdPosition;
        
        canDoTarget = false;
    }

    void Patrol()
    {
        if (_coreManager.agent.remainingDistance <= 2 && !canDoTarget)
        {
            canDoTarget = true;
        }
        
        Debug.DrawLine(transform.position, _coreManager.target, Color.magenta);
    }


    void ChasePlayer()
    {
        _coreManager.target = _coreManager.player.transform.position;
    }

     
    

    IEnumerator fuite()
    {
        isFear = true;
        canDoTarget = false;
        
            
        closestPoint = fearCollider.ClosestPoint(transform.position);
            
        
        _coreManager.target = closestPoint;
            
        
        Debug.DrawLine(transform.position, _coreManager.target, Color.yellow);
        print("fearTarget pos " +  _coreManager.target + "ma pos "+ transform.position);
        

        yield return null;
    }

    private IEnumerator fuiteCoolDown()
    {
        canBeFear = false;
        
        yield return new WaitForSeconds(fearCooldown);

        canBeFear = true;
    }
    
    
    void ComportementPalier()
    {
        switch (_coreManager.palier)
        {
            case "petit":
                if(canDoTarget)
                {
                    CreatePatrolTarget();
                }

                if(!isFear)
                {
                    Patrol();
                }
                break;
            
            case "moyen":
                if(canDoTarget)
                {
                    CreatePatrolTarget();
                }

                if (!canAttackDistance)
                {
                    Patrol();
                }

                if (canAttackDistance)
                {
                    ChasePlayer();
                }
                
                
                break;
            
            case "grand":
                ChasePlayer();
              
                break;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(closestPoint, 1f);
    }
}
