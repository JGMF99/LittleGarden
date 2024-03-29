using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{

    [SerializeField] List<Character> enemies;

    public EnemyParty GetRandomEnemy()
    {
        var numberEnemies = Random.Range(1, 4);

        var enemyParty = new EnemyParty
        {
            Team = new List<Character>()
        };

        for (var i = 0; i < numberEnemies; i++)
        {

            var enemy = enemies[Random.Range(0, enemies.Count)];

            while(enemyParty.GetPositionCharacter(enemy.Position) != null)
            {
                enemy = enemies[Random.Range(0, enemies.Count)];
            }

            enemyParty.Team.Add(enemy);
            enemyParty.Team[i].Init();
        }

        enemyParty.Team = enemyParty.Team.OrderBy(o => o.Position).ToList();

        return enemyParty;
    }

}
