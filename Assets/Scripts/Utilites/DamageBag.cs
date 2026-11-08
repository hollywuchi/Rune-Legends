using UnityEngine;

public struct DamageData
{
    // 基础伤害
    public int damage;
    public int postureDamage;
    
    // 物理信息
    public Vector3 knockBackDir;
    public float knockBackForce;
    public Vector2 hitPoint;
    
    //来源信息
    public GameObject attacker;
    
}