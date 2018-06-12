using System;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace mc2.utils {
    public class Noise {
        

        public static float [,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {

            float [,] noiseMap = new float [mapWidth, mapHeight];

            System.Random prng = new System.Random(seed);
            Vector2 [] octaveOffsets = new Vector2 [octaves];
            for (int i = 0; i < octaves; i++) {

                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
                scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {

                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int oct = 0; oct < octaves; oct++) {

                        float simpleX = (x - halfWidth) / scale * frequency + octaveOffsets[oct].x;
                        float simpleY = (y - halfHeight) / scale * frequency + octaveOffsets[oct].y;

                        float perlinValue = Mathf.PerlinNoise(simpleX, simpleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight) {
                        maxNoiseHeight = noiseHeight;
                    } else if (noiseHeight < minNoiseHeight) {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {

                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}