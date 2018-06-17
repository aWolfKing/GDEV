using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class Math {

        private static Random random = new Random();



        /// <summary>
        /// Get a random int between (and including both) <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max){
            return random.Next(min, max+1);
        }



        /// <summary>
        /// Returns a random float between and including min and max.
        /// Uses Unity.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomFloat_Unity(float min, float max){
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// Returns a random boolean based on a chance percentage from 0% to 100%.
        /// Uses Unity.
        /// </summary>
        /// <param name="percentageChance"></param>
        /// <returns></returns>
        public static bool RandomBool_Unity(float percentageChance){
            if(percentageChance == 0){
                return false;
            }
            else{
                return UnityEngine.Random.Range(0f, 100f) <= percentageChance;
            }
        }



        public static int RoundTowardsZero(float value){
            return ((int)(value * (value < 0 ? -1 : 1))) * (value < 0 ? -1 : 1);
        }

    }

}
