using UnityEngine;
using Random = System.Random;

namespace mc2.general
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves,
            float persistance, float lacunarity, Vector2 offset)
        {
            var noiseMap = new float [mapWidth, mapHeight];

            var prng = new Random(seed);
            var octaveOffsets = new Vector2 [octaves];
            for (var i = 0; i < octaves; i++)
            {
                var offsetX = prng.Next(-100000, 100000) + offset.x;
                var offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
                scale = 0.0001f;

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;

            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (var oct = 0; oct < octaves; oct++)
                {
                    var simpleX = (x - halfWidth) / scale * frequency + octaveOffsets[oct].x;
                    var simpleY = (y - halfHeight) / scale * frequency + octaveOffsets[oct].y;

                    var perlinValue = Mathf.PerlinNoise(simpleX, simpleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }

            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);

            return noiseMap;
        }
    }
}