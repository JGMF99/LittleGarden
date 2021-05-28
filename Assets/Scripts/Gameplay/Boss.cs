using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] List<Character> enemyTeam;

    public List<Character> EnemyTeam { get => enemyTeam; set => enemyTeam = value; }
}
