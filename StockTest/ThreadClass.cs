using System;
using System.Collections;
using System.Threading;

public class ThreadClass
{
    public event EventHandler<string> Completed;

    public void Run(Action<object> method, ArrayList parameters)
    {
        foreach (var item in parameters)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                Completed?.Invoke(this, "Start" + item.ToString());
                method(item);
                Completed?.Invoke(this, "Completed" + item.ToString());
            }, parameters);
        }
        
    }
}
