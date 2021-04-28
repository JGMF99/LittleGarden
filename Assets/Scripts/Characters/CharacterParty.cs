using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterParty : MonoBehaviour
{

    [SerializeField] List<Character> team;

    public List<Character> Team { get => team; set => team = value; }

    private void Start()
    {
        foreach (var character in team)
        {
            character.Init();
        }
    }

    public Character GetHealtyhCharacter()
    {
        return team.Where(x => x.HP > 0).FirstOrDefault();
    }

    public Character GetPositionCharacter(int position)
    {
        return team.Where(x => x.HP > 0 && x.Position == position).FirstOrDefault();
    }
}
