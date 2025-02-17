using System.Collections;
using UnityEngine;

public class GoblinManager : MonoBehaviour
{
    public IEnumerator RespawnGoblin(GoblinAI goblin)
    {
        goblin.gameObject.SetActive(false);
        yield return new WaitForSeconds(15f);

        goblin.health = 100;
        goblin.transform.position = goblin.spawnPosition;
        goblin.gameObject.SetActive(true);
        goblin.Idle();
    }
}
