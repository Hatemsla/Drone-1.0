using System;
using UnityEngine;

namespace DroneFootball
{
    public class PIDController
    {
        public float derivativeGain;
        public bool derivativeInitialized;
        public float errorLast;
        public float integralGain;
        public float integralSaturation;
        public float integrationStored;
        public float outputMax = 1;
        public float outputMin = -1;
        public float proportionalGain;
        public float valueLast;

        public PIDController(float proportionalGain, float integralGain, float derivativeGain, float outputMin,
            float outputMax, float integralSaturation)
        {
            this.proportionalGain = proportionalGain;
            this.integralGain = integralGain;
            this.derivativeGain = derivativeGain;
            this.outputMin = outputMin;
            this.outputMax = outputMax;
            this.integralSaturation = integralSaturation;
        }

        public float UpdateThrottle(float dt, float currentValue, float targetValue)
        {
            if (dt <= 0) throw new ArgumentOutOfRangeException(nameof(dt));

            var error = targetValue - currentValue;

            //calculate P term
            var P = proportionalGain * error;

            //calculate I term
            integrationStored = Mathf.Clamp(integrationStored + error * dt, -integralSaturation, integralSaturation);
            var I = integralGain * integrationStored;

            //calculate both D terms
            var errorRateOfChange = (error - errorLast) / dt;
            errorLast = error;

            var valueRateOfChange = (currentValue - valueLast) / dt;
            valueLast = currentValue;
            
            //choose D term to use
            float deriveMeasure = 0;

            if (derivativeInitialized)
                deriveMeasure = -valueRateOfChange;
            else
                derivativeInitialized = true;

            var D = derivativeGain * deriveMeasure;

            var result = P + I + D;

            return Mathf.Clamp(result, outputMin, outputMax);
        }

        private float AngleDifference(float a, float b)
        {
            return (a - b + 540) % 360 - 180; //calculate modular difference, and remap to [-180, 180]
        }

        public float UpdateAngle(float dt, float currentAngle, float targetAngle)
        {
            if (dt <= 0) throw new ArgumentOutOfRangeException(nameof(dt));
            var error = AngleDifference(targetAngle, currentAngle);
            errorLast = error;

            //calculate P term
            var P = proportionalGain * error;

            //calculate I term
            integrationStored = Mathf.Clamp(integrationStored + error * dt, -integralSaturation, integralSaturation);
            var I = integralGain * integrationStored;

            //calculate both D terms
            var errorRateOfChange = AngleDifference(error, errorLast) / dt;
            errorLast = error;

            var valueRateOfChange = AngleDifference(currentAngle, valueLast) / dt;
            valueLast = currentAngle;

            //choose D term to use
            float deriveMeasure = 0;

            if (derivativeInitialized)
                deriveMeasure = -valueRateOfChange;
            else
                derivativeInitialized = true;

            var D = derivativeGain * deriveMeasure;

            var result = P + I + D;

            return Mathf.Clamp(result, outputMin, outputMax);
        }
    }
}