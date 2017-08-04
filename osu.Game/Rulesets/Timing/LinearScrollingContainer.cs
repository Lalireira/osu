﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;
using osu.Game.Rulesets.Timing;

namespace osu.Game.Rulesets.Timing
{
    /// <summary>
    /// A <see cref="ScrollingContainer"/> which scrolls relative to the control point start time.
    /// This is the default <see cref="ScrollingContainer"/> returned by the base <see cref="SpeedAdjustmentContainer"/>.
    /// </summary>
    internal class LinearScrollingContainer : ScrollingContainer
    {
        private readonly Axes scrollingAxes;
        private readonly MultiplierControlPoint controlPoint;

        public LinearScrollingContainer(Axes scrollingAxes, MultiplierControlPoint controlPoint)
        {
            this.scrollingAxes = scrollingAxes;
            this.controlPoint = controlPoint;
        }

        protected override void Update()
        {
            base.Update();

            if ((scrollingAxes & Axes.X) > 0) X = (float)(controlPoint.StartTime - Time.Current);
            if ((scrollingAxes & Axes.Y) > 0) Y = (float)(controlPoint.StartTime - Time.Current);
        }
    }
}
