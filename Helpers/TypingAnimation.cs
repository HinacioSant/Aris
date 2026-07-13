using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Aris.Helpers
{
    public static class TypingAnimation
    {
        public static void Start(TextBlock target, string text, double totaltime, Action? onComplete = null)
        {
            var animation = new StringAnimationUsingKeyFrames();
            var interval = TimeSpan.FromSeconds(totaltime / text.Length);

            for (int i = 1; i <= text.Length; i++)
            {
                animation.KeyFrames.Add(new DiscreteStringKeyFrame(text.Substring(0, i),
                        KeyTime.FromTimeSpan(TimeSpan.FromTicks(interval.Ticks * i))
                        ));
            }

            if (onComplete != null) animation.Completed += (s,e) => onComplete();

            target.BeginAnimation(TextBlock.TextProperty, animation);
        }
    }
}