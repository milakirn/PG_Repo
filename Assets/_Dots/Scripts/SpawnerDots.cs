using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dots.JobsScripting
{
    public class SpawnerDots : MonoBehaviour
    {
        [SerializeField] private Cube _cubePref;
        [SerializeField] private int minFPS;

        private bool _isLowMemory;
        private int _spawnDelay = 10;
        private int _currentSpawnOffset;
        private float _deltaTime;

        private List<Cube> _spawnedCubes = new();

        private NativeArray<float3> _cubesPosition;
        private FindClosestJob _findClosestJob;
        private JobHandle _jobHandler;

        private void Awake()
        {
            SpawnCubes();
        }

        private void OnEnable()
        {
            Application.lowMemory += HandleLowMemory;
        }

        private void OnDisable()
        {
            Application.lowMemory -= HandleLowMemory;
        }

        private void Update()
        {
            _currentSpawnOffset++;

            if (_currentSpawnOffset > _spawnDelay && !_isLowMemory)
            {
                _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
                if (1.0f / _deltaTime > minFPS)
                {
                    SpawnCubes();
                }
                _currentSpawnOffset = 0;
            }

            UpdateCubePosition();
        }

        private void LateUpdate()
        {
            _jobHandler.Complete();

            int spawnedCubeCount = _spawnedCubes.Count;
            var result = _findClosestJob.result;

            for (int i = 0; i < spawnedCubeCount; i++)
            {
                var closestPositions = new Vector3[3];
                closestPositions[0] = _spawnedCubes[result[i].first].transform.position;
                closestPositions[1] = _spawnedCubes[result[i].second].transform.position;
                closestPositions[2] = _spawnedCubes[result[i].third].transform.position;

                _spawnedCubes[i].SetClosestPositions(closestPositions);
                _spawnedCubes[i].SetFarthestPosition(_spawnedCubes[result[i].last].transform.position);
            }
        }

        [ContextMenu("Spawn")]
        public void SpawnCubes()
        {
            for (int i = 0; i < 1000; i++)
            {
                _spawnedCubes.Add(Instantiate(_cubePref, UnityEngine.Random.insideUnitSphere * 100f, Quaternion.identity, transform));
            }
        }

        public void UpdateCubePosition()
        {
            int spawnedCubesCount = _spawnedCubes.Count;
            _cubesPosition = new NativeArray<float3>(spawnedCubesCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var result = new NativeArray<ClosestAndLast>(spawnedCubesCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < spawnedCubesCount; i++)
            {
                _cubesPosition[i] = _spawnedCubes[i].transform.position;
            }

            _findClosestJob = new()
            {
                cubeCount = spawnedCubesCount,
                cubesPosition = _cubesPosition,
                result = result
            };

            _jobHandler = _findClosestJob.Schedule();
        }

        private void HandleLowMemory()
        {
            _isLowMemory = true;
        }

        [BurstCompile]
        private struct FindClosestJob : IJob
        {
            public int cubeCount;
            [ReadOnly] public NativeArray<float3> cubesPosition;
            [WriteOnly] public NativeArray<ClosestAndLast> result;

            public void Execute()
            {
                for (int i = 0; i < cubeCount; i++)
                {
                    result[i] = GetClosestAndLast(i, cubesPosition[i]);
                }
            }

            private ClosestAndLast GetClosestAndLast(int currentIndex, float3 currentPosition)
            {
                var closestDistances = new NativeArray<float>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var closestIndices = new NativeArray<int>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                float lastDistance = float.MinValue;
                int lastIndex = -1;

                for (int i = 0; i < 3; i++)
                {
                    closestDistances[i] = float.MaxValue;
                    closestIndices[i] = -1;
                }

                for (int i = 0; i < cubeCount; i++)
                {
                    if (i == currentIndex) continue;

                    float distance = math.distance(currentPosition, cubesPosition[i]);

                    // Check if the current distance is smaller than the stored distances
                    for (int j = 0; j < 3; j++)
                    {
                        if (distance < closestDistances[j])
                        {
                            // Shift the other closest distances and indices
                            for (int k = 2; k > j; k--)
                            {
                                closestDistances[k] = closestDistances[k - 1];
                                closestIndices[k] = closestIndices[k - 1];
                            }

                            // Store the new closest distance and index
                            closestDistances[j] = distance;
                            closestIndices[j] = i;
                            break;
                        }
                    }
                    // Check if the current distance is larger than the farthest distance
                    if (distance > lastDistance)
                    {
                        lastDistance = distance;
                        lastIndex = i;
                    }
                }

                return new()
                {
                    first = closestIndices[0],
                    second = closestIndices[1],
                    third = closestIndices[2],
                    last = lastIndex
                };
            }
        }
    }
}