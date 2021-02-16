using System;

public class ActionListenerUtility
{
    public static void UnsubscribeActionListenersFrom(ref Action<int> action)
    {
        if (action != null)
        {
            Delegate[] subscribers = action.GetInvocationList();
            foreach (Delegate subscriber in subscribers)
            {
                action -= (subscriber as Action<int>);
            }
            action = null;
        }
    }

    public static void UnsubscribeActionListenersFrom(ref Action action)
    {
        if (action != null)
        {
            Delegate[] subscribers = action.GetInvocationList();
            foreach (Delegate subscriber in subscribers)
            {
                action -= (subscriber as Action);
            }
            action = null;
        }
    }
}
