using System.Linq;
using UnityEngine;

namespace mc2.utils
{

    public class DiamondNoise {
        private static int _ysize, _xsize;

        private static float _roughness; //Определяет разницу высот, чем больше, тем более неравномерная карта

        private DiamondNoise(Vector2 size, int seed, float roughness = 2f) {
            _xsize = (int)size.x;
            _ysize = (int)size.y;
            _roughness = roughness;
            Heighmap = new float[_xsize, _ysize];
        }

        private DiamondNoise(int x, int y, int seed, float roughness = 2f) {
            _xsize = x;
            _ysize = y;
            _roughness = roughness;
            Heighmap = new float[x, y];
        }

        public static DiamondNoise Setup(Vector2 size, int seed) {
            return new DiamondNoise(size, seed);
        }

        public static DiamondNoise Setup(Vector2 size, float roughness, int seed) {
            return new DiamondNoise(size, seed, roughness);
        }

        public static DiamondNoise Setup(int width, int height, int seed) {
            return new DiamondNoise(width, height, seed);
        }

        public static DiamondNoise Setup(int width, int height, float roughness, int seed) {
            return new DiamondNoise(width, height, seed, roughness);
        }
        
        public static float[,] Heighmap { get; private set; }

        private static void Square(int lx, int ly, int rx, int ry) {
            var l = (rx - lx) / 2;

            var a = Heighmap[lx, ly]; //  B--------C
            var b = Heighmap[lx, ry]; //  |        |
            var c = Heighmap[rx, ry]; //  |   ce   |
            var d = Heighmap[rx, ly]; //  |        |        
            var cex = lx + l;         //  A--------D
            var cey = ly + l;

            Heighmap[cex, cey] = Average(a, b, c, d) + GetOffset(l);
        }

        private static float GetOffset(int l) {
            return Random.Range(-l * 2 * _roughness / _ysize, l * 2 * _roughness / _ysize);
        }

        private static float Average(params float[] num) {
            return num.Sum() / num.Length;
        }

        private static bool _lrflag = false;

        private static void Diamond(int tgx, int tgy, int l) {
            float a, b, c, d;

            if (tgy - l >= 0)
                a = Heighmap[tgx, tgy - l]; 
            else 
                a = Heighmap[tgx, _ysize - l]; 
             
            
            if (tgx - l >= 0)
                b = Heighmap[tgx - l, tgy];
            else if (_lrflag)
                b = Heighmap[_xsize - l, tgy];
            else
                b = Heighmap[_ysize - l, tgy];


            if (tgy + l < _ysize)
                c = Heighmap[tgx, tgy + l];
            else
                c = Heighmap[tgx, l];

            if (_lrflag)
                d = tgx + l < _xsize ? Heighmap[tgx + l, tgy] : Heighmap[l, tgy];
            else if (tgx + l < _ysize)
                d = Heighmap[tgx + l, tgy];
            else
                d = Heighmap[l, tgy];

            Heighmap[tgx, tgy] = Average(a, b, c, d) + GetOffset(l);
        }

        private static void DiamondSquare(int lx, int ly, int rx, int ry) {
            var l = (rx - lx) / 2;

            Square(lx, ly, rx, ry);

            Diamond(lx, ly + l, l);
            Diamond(rx, ry - l, l);
            Diamond(rx - l, ry, l);
            Diamond(lx + l, ly, l);
        }


        private static void MidPointDisplacement(int lx, int ly, int rx, int ry) {
            var l = (rx - lx) / 2;
            if (l > 0) {
                var a = Heighmap[lx, ly];
                var b = Heighmap[lx, ry];
                var c = Heighmap[rx, ry];
                var d = Heighmap[rx, ly];
                
                var cex = lx + l;
                var cey = ly + l;

                Heighmap[cex, cey] = (a + b + c + d) / 4 +
                                     Random.Range(-l * 2 * _roughness / _xsize, l * 2 * _roughness / _xsize);

                Heighmap[lx, cey] =
                    (a + b) / 2 + Random.Range(-l * 2 * _roughness / _xsize, l * 2 * _roughness / _xsize);
                Heighmap[rx, cey] =
                    (c + d) / 2 + Random.Range(-l * 2 * _roughness / _xsize, l * 2 * _roughness / _xsize);
                Heighmap[cex, ly] =
                    (a + d) / 2 + Random.Range(-l * 2 * _roughness / _xsize, l * 2 * _roughness / _xsize);
                Heighmap[cex, ry] =
                    (b + c) / 2 + Random.Range(-l * 2 * _roughness / _xsize, l * 2 * _roughness / _xsize);

                MidPointDisplacement(lx, ly, cex, cey);
                MidPointDisplacement(lx, ly + l, lx + l, ry);
                MidPointDisplacement(cex, cey, rx, ry);
                MidPointDisplacement(lx + l, ly, rx, cey);
            }
        }


        public float[,] GenerateMap() {
            Heighmap[0, 0] = Random.Range(0.3f, 0.6f);
            Heighmap[0, _ysize - 1] = Random.Range(0.3f, 0.6f);
            Heighmap[_xsize - 1, _ysize - 1] = Random.Range(0.3f, 0.6f);
            Heighmap[_xsize - 1, 0] = Random.Range(0.3f, 0.6f);

            Heighmap[_ysize - 1, _ysize - 1] = Random.Range(0.3f, 0.6f);
            Heighmap[_ysize - 1, 0] = Random.Range(0.3f, 0.6f);

            for (var l = (_ysize - 1) / 2; l > 0; l /= 2)
                for (var x = 0; x < _xsize - 1; x += l) {
                    _lrflag = x >= _ysize - l;

                    for (var y = 0; y < _ysize - 1; y += l)
                        DiamondSquare(x, y, x + l, y + l);
                }

            return Heighmap;
        }
    }

}