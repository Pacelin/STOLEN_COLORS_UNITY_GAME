using Gameplay.Map.Enemies;
using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class SpawnTest : MonoBehaviour
    {
        [Inject] private WarriorsSpawner _spawner;
        [Inject] private CastlesCollection _castles;
        [Inject] private WaveManager _waveManager;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _waveManager.StartWave();
            else if (Input.GetKeyDown(KeyCode.Z))
                _spawner.SpawnAlly(EWarriorClass.Tank);
            else if (Input.GetKeyDown(KeyCode.X))
                _spawner.SpawnAlly(EWarriorClass.Soldier);
            else if (Input.GetKeyDown(KeyCode.C))
                _spawner.SpawnAlly(EWarriorClass.Mage);
            else if (Input.GetKeyDown(KeyCode.V))
                _spawner.SpawnEnemy(EWarriorClass.Tank);
            else if (Input.GetKeyDown(KeyCode.B))
                _spawner.SpawnEnemy(EWarriorClass.Soldier);
            else if (Input.GetKeyDown(KeyCode.N))
                _spawner.SpawnEnemy(EWarriorClass.Mage);
        }
    }
}