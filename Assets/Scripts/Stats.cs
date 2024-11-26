using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public int Health;
    public float Stamina;
    public int Damage;
    public int Armor;
    public float Speed;

    public static Stats operator +(Stats stats1, Stats stats2)
    {
        Stats result = new Stats();
        result.Health = stats1.Health + stats2.Health;
        result.Stamina = stats1.Stamina + stats2.Stamina;
        result.Damage = stats1.Damage + stats2.Damage;
        result.Armor = stats1.Armor + stats2.Armor;
        result.Speed = stats1.Speed + stats2.Speed;
        return result;
    }

    public static Stats operator -(Stats stats1, Stats stats2)
    {
        Stats result = new Stats();
        result.Health = stats1.Health - stats2.Health;
        result.Stamina = stats1.Stamina - stats2.Stamina;
        result.Damage = stats1.Damage - stats2.Damage;
        result.Armor = stats1.Armor - stats2.Armor;
        result.Speed = stats1.Speed - stats2.Speed;
        return result;
    }

    //public Stats(int health, int stamina, int damage, int armor, float speed)
    //{
    //    Health = health;
    //    Stamina = stamina;
    //    Damage = damage;
    //    Armor = armor;
    //    Speed = speed;
    //}
}
