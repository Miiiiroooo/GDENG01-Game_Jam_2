using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Zombie Waves", fileName = "New Zombie Wave")]
public class ZombieWaveTemplate : ScriptableObject
{
    #region FIELDS
    [field: SerializeField] private string waveName = "";
    [field: SerializeField] private float elapsedWaveTime = 0;
    [field: SerializeField] private float waveDuration = 30;
    [field: SerializeField] private float restDuration = 5;
    [field: SerializeField] private List<MiniZombieWaveTemplate> miniWaveList = new List<MiniZombieWaveTemplate>();
    #endregion

    #region PROPERTIES
    public string WaveName
    {
        get { return this.waveName; }
        private set { this.waveName = value; }
    }

    public float ElapsedWaveTime
    {
        get { return this.elapsedWaveTime; }
        private set { this.elapsedWaveTime = value; }
    }

    public float WaveDuration
    {
        get { return this.waveDuration; }
        private set { this.waveDuration = value; }
    }

    public float RestDuration
    {
        get { return this.restDuration; }
        private set { this.restDuration = value; }
    }

    public List<MiniZombieWaveTemplate> MiniWaveList
    {
        get { return this.miniWaveList; }
        set { this.miniWaveList = value; }
    }
    #endregion

    public void AddElapsedTime(float increase)
    {
        this.elapsedWaveTime += increase;
    }

    public void FullReset()
    {
        this.elapsedWaveTime = 0;

        foreach (MiniZombieWaveTemplate miniWave in this.miniWaveList)
        {
            miniWave.RestartElapsedTime(true);
            miniWave.RestartSingleBurst();
        }
    }
}
