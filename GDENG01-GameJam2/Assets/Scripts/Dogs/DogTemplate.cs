using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dog", menuName = "Dog")]
public class DogTemplate : ScriptableObject
{
    public string dogName;
    public Sprite dogImage;
    public AudioClip attackSound;
    public float cost;

    //Attack
    [Space(20)]
    public float attackCooldown;
    public float attackDamage;
    public float attackSpeed;
    public int attackUpgradeCost;
    public int attackSpeedUpgradeCost;

    [Range(1, 12)] public float range;

}
