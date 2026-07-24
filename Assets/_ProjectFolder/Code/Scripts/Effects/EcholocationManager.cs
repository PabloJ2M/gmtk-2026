using UnityEngine;

public class EcholocationManager : SingletonBasic<EcholocationManager>
{
    [SerializeField] private float waveSpeed = 8.0f;
    [SerializeField] private float waveLifetime = 1.5f;
    
    private static readonly int MaxSimultaneousWaves = 10;
    private static readonly int WavePositionsID = Shader.PropertyToID("_GlobalWavePositions");
    private static readonly int WaveAlphasID = Shader.PropertyToID("_GlobalWaveAlphas");
    private static readonly int ActiveWavesCountID = Shader.PropertyToID("_GlobalActiveWavesCount");
    
    private Wave[] _waves;
    private Vector4[] _wavePositionsData;
    private float[] _waveAlphasData;
    private int _activeWavesCount;

    protected override void Awake()
    {
        base.Awake();
        _waves = new Wave[MaxSimultaneousWaves];
        _wavePositionsData = new Vector4[MaxSimultaneousWaves];
        _waveAlphasData = new float[MaxSimultaneousWaves];
    }
    private void Update()
    {
        UpdateWaves();
        SendShaderData();
    }

    private void UpdateWaves()
    {
        for (int i = 0; i < _activeWavesCount; i++)
        {
            if (_waves[i].LifetimeLeft <= 0) {
                _waveAlphasData[i] = 0f;
                continue;
            }
            
            _waves[i].LifetimeLeft -= Time.deltaTime;
            _waves[i].CurrentRadius += waveSpeed * Time.deltaTime;
            
            _wavePositionsData[i] = new Vector4(_waves[i].Position.x, _waves[i].Position.y, 0f, _waves[i].CurrentRadius);
            _waveAlphasData[i] = Mathf.Clamp01(_waves[i].LifetimeLeft / _waves[i].MaxLifetime);
        }
    }
    private void SendShaderData()
    {
        Shader.SetGlobalVectorArray(WavePositionsID, _wavePositionsData);
        Shader.SetGlobalFloatArray(WaveAlphasID, _waveAlphasData);
        Shader.SetGlobalInt(ActiveWavesCountID, _activeWavesCount);
    }
    
    public void EmitRipple(Vector2 worldPosition)
    {
        int index = -1;
        for (int i = 0; i < _activeWavesCount; i++)
        {
            if (_waves[i].LifetimeLeft <= 0) {
                index = i;
                break;
            }
        }

        if (index == -1 && _activeWavesCount < MaxSimultaneousWaves) {
            index = _activeWavesCount;
            _activeWavesCount++;
        }
        else if (index == -1) {
            index = 0;
        }

        _waves[index] = new(worldPosition, 0f, waveLifetime);
    }
}