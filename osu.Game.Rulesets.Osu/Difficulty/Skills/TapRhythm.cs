// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Difficulty.Skills
{
    /// <summary>
    ///
    /// </summary>
    public class TapRhythm : OsuSkill
    {
        public override double strainDecay(double ms) => Math.Pow(.55, ms / 1000);
        protected override double SkillMultiplier => 125;
        protected override double StrainDecayBase => 0;
        protected override double StarMultiplierPerRepeat => 1.05;

        private const double quarter220 = 60000 / (4 * 220);

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            if (current.BaseObject is Spinner)
                return 0;

            if (Previous.Count > 1)
            {
                var osuCurrent = (OsuDifficultyHitObject)current;
                var osuPrevious = (OsuDifficultyHitObject)Previous[0];
                var osuPrevPrev = (OsuDifficultyHitObject)Previous[1];
                double strainTime = Math.Max(osuCurrent.DeltaTime, 46.875);
                double prevStrainTime = Math.Max(osuPrevious.DeltaTime, 46.875);

                double strain = Math.Pow(75.0 / Math.Max(strainTime, prevStrainTime), 1.5);

                if (osuCurrent.BaseObject is Slider)
                    strain /= 2.0;

                double totalStrain = osuCurrent.Rhythm.Difficulty;
                double sliderRhythmDiff = 0.0;

                // If prevprev or prev is a slider, take weighted average using slider end rhythm
                if (osuPrevious.BaseObject is Slider || osuPrevPrev.BaseObject is Slider) {
                    sliderRhythmDiff = osuCurrent.SliderRhythm.Difficulty;

                    double squishConstant = 1/50.0;
                    double maxSliderWeighting = 0.5;

                    double sliderWeighting = maxSliderWeighting * erf(squishConstant * osuPrevPrev.TravelTime);

                    totalStrain = sliderWeighting * sliderRhythmDiff + (1.0 - sliderWeighting) * totalStrain;
                }

                totalStrain *= strain;

                // Apply multiplier based on the rhythm change in comparison to OD hit window.
                double msChange = Math.Abs(osuCurrent.DeltaTime - osuPrevious.DeltaTime);
                double changeWeighting = 0.5 - erf(4.0 * (-msChange / (osuCurrent.BaseObject.GreatHitWindow / osuCurrent.ClockRate) + 1.0)) / 2.0;
                totalStrain *= changeWeighting;

                return totalStrain;
            }
            return 0;
        }

        private static double erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }
    }
}
