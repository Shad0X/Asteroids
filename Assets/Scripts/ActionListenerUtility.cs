using System;

public class ActionListenerUtility
{
    //maybe use Generics to specify the Action<..> type ? 
    public static void UnsubscribeActionListenersFrom(ref Action<float> action)
    {
        if (action != null)
        {
            Delegate[] subscribers = action.GetInvocationList();
            foreach (Delegate subscriber in subscribers)
            {
                action -= (subscriber as Action<float>);
            }
            action = null;
        }
    }

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
