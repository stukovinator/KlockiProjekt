using System;
using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.ParticleGenerators
{
    public class RandomParticleGenerator
    {
        public static SKColor[] DefaultColors => new []
        {
            SKColors.DodgerBlue,
            SKColors.CornflowerBlue,
            SKColors.PaleVioletRed,
            SKColors.LightPink,
            SKColors.Goldenrod,
            SKColors.Gold,
            SKColors.Red
        };

        public ParticleBase GetRandomParticle(SKPoint[] startPositions, DirectionRange[] directionRanges, int amount, Random rand, int i, SKColor[] colors)
        {
            var directionRange = directionRanges[rand.Next(directionRanges.Length)];
            var direction = (float) (directionRange.MinAngle + rand.NextDouble() * (directionRange.MaxAngle - directionRange.MinAngle));

            var startPositionIndex = rand.Next(startPositions.Length);
            var startPosition = startPositions[startPositionIndex];

            if (rand.NextDouble() > 0.45d)
            {
                return new RectParticle(
                    colors[rand.Next(0, colors.Length)],
                    new SKPoint3(
                        (float)(rand.NextDouble() * 360.0f * 1.2),
                        (float)(rand.NextDouble() * 360.0f * 1.2),
                        (float)(rand.NextDouble() * 360.0f * 1.2)
                    ),
                    (float)(100.0f + rand.NextDouble() * 200),
                    direction,
                    new SKPoint3(
                        (float)(rand.NextDouble() * 360.0f),
                        (float)(rand.NextDouble() * 360.0f),
                        (float)(rand.NextDouble() * 360.0f)
                    ),
                    startPosition,
                    new SKSize(
                        (float)(6 + rand.NextDouble() * 5),
                        (float)(6 + rand.NextDouble() * 5)
                    ),
                    (amount - i - 1) / amount * 6
                );
            }
            else
            {
                return new EllipseParticle(
                    colors[rand.Next(0, colors.Length)],
                    new SKPoint3(
                        (float)(rand.NextDouble() * 360.0f),
                        (float)(rand.NextDouble() * 360.0f),
                        (float)(rand.NextDouble() * 360.0f)
                    ),
                    (float)(100.0f + rand.NextDouble() * 200),
                    direction,
                    new SKPoint3(
                        (float)(rand.NextDouble() * 360.0f),
                        (float)(rand.NextDouble() * 360.0f),
                        (float)(rand.NextDouble() * 360.0f)
                    ),
                    startPosition,
                    new SKSize(
                        (float)(2 + rand.NextDouble() * 5),
                        (float)(2 + rand.NextDouble() * 5)
                    ),
                    (amount - i - 1) / amount * 6
                );
            }
        }
    }
}