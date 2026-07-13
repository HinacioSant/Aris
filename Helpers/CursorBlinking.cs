using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Aris.Helpers
{
    public static class CursorBlinkingAnimation
    {
        public static void Start(TextBlock target, double intervalTime = 0.5)
        {
            var animation = BuildAnimation(intervalTime);
            animation.RepeatBehavior = RepeatBehavior.Forever;
            target.BeginAnimation(TextBlock.TextProperty, animation);
        }

        public static void StarTimed(TextBlock target, double intervalTime, double totalTime, Action? onComplete = null)
        {
            var animation = BuildAnimation(intervalTime);
            animation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(totalTime));

            if (onComplete != null) animation.Completed += (s, e) => onComplete();

            target.BeginAnimation(TextBlock.TextProperty, animation);
        }

        private static StringAnimationUsingKeyFrames BuildAnimation(double intervalTime)
        {
            var animation = new StringAnimationUsingKeyFrames();            
           
            animation.KeyFrames.Add(new DiscreteStringKeyFrame(">", KeyTime.FromTimeSpan(TimeSpan.Zero)));

            animation.KeyFrames.Add(new DiscreteStringKeyFrame("_", KeyTime.FromTimeSpan(TimeSpan.FromSeconds(intervalTime))));

            animation.Duration = TimeSpan.FromSeconds(intervalTime * 2);

            return animation;
        }

        public static void Stop(TextBlock target, string fallbackText = "")
        {
            target.BeginAnimation(TextBlock.TextProperty, null);
            target.Text = fallbackText;
        }
    }
}