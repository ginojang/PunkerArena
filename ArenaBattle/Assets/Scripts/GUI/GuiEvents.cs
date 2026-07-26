using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void GuiCallback<GuiObject>(GuiObject target);
public delegate void GuiCallback<GuiObject, T>(GuiObject target, T arg1);
public delegate void GuiCallback<GuiObject, T, U>(GuiObject target, T arg1, U arg2);
public delegate void GuiCallback<GuiObject, T, U, V>(GuiObject target, T arg1, U arg2, V arg3);
public delegate void GuiCallback<GuiObject, T, U, V, W>(GuiObject target, T arg1, U arg2, V arg3, W arg4);

public class GuiEvents<Z> : GuiObject where Z : class
{
    protected virtual void OnDestroy()
    {
        CleanUp();
    }

    private static Dictionary<Enum, Delegate> eventTable = new Dictionary<Enum, Delegate>();

    private static void CleanUp()
    {
        eventTable.Clear();
    }

    private static bool OnListenerAdding(Enum eventId, Delegate listenerBeingAdded)
    {
        if (!eventTable.ContainsKey(eventId))
        {
            eventTable.Add(eventId, null);
        }

        // 기존에 등록된 Handler가 있을경우, 같은 Type의 Handler만 등록 할 수 있다.
        Delegate d = eventTable[eventId];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            Debug.LogError("같은 타입의 Handler만 등록 할 수 있습니다.");
            return false;
        }

        return true;
    }

    private static bool OnListenerRemoving(Enum eventId, Delegate listenerBeingRemoved)
    {
        if (!eventTable.ContainsKey(eventId)) return false;

        Delegate d = eventTable[eventId];
        if (d == null) return false;

        if (d.GetType() != listenerBeingRemoved.GetType())
        {
            Debug.LogError("같은 타입의 Handler가 아닙니다.");
            return false;
        }

        return true;
    }

    private static void OnListenerRemoved(Enum eventId)
    {
        if (eventTable[eventId] == null)
        {
            eventTable.Remove(eventId);
        }
    }

    #region AddListener
    public static void AddListener(Enum eventId, GuiCallback<GuiObject> handler)
    {
        if (!OnListenerAdding(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject>)eventTable[eventId] + handler;

        //foreach (KeyValuePair<string, Delegate> pair in eventTable)
        //{
        //    Debug.Log(pair.Key + " ::: " + pair.Value);
        //}
    }

    public static void AddListener<T>(Enum eventId, GuiCallback<GuiObject, T> handler)
    {
        if (!OnListenerAdding(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T>)eventTable[eventId] + handler;
    }

    public static void AddListener<T, U>(Enum eventId, GuiCallback<GuiObject, T, U> handler)
    {
        if (!OnListenerAdding(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T, U>)eventTable[eventId] + handler;
    }

    public static void AddListener<T, U, V>(Enum eventId, GuiCallback<GuiObject, T, U, V> handler)
    {
        if (!OnListenerAdding(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T, U, V>)eventTable[eventId] + handler;
    }

    public static void AddListener<T, U, V, W>(Enum eventId, GuiCallback<GuiObject, T, U, V, W> handler)
    {
        if (!OnListenerAdding(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T, U, V, W>)eventTable[eventId] + handler;
    }
    #endregion

    #region RemoveListener
    public static void RemoveListener(Enum eventId, GuiCallback<GuiObject> handler)
    {
        if (!OnListenerRemoving(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject>)eventTable[eventId] - handler;
        OnListenerRemoved(eventId);
    }

    public static void RemoveListener<T>(Enum eventId, GuiCallback<GuiObject, T> handler)
    {
        if (!OnListenerRemoving(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T>)eventTable[eventId] - handler;
        OnListenerRemoved(eventId);
    }

    public static void RemoveListener<T, U>(Enum eventId, GuiCallback<GuiObject, T, U> handler)
    {
        if (!OnListenerRemoving(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T, U>)eventTable[eventId] - handler;
        OnListenerRemoved(eventId);
    }

    public static void RemoveListener<T, U, V>(Enum eventId, GuiCallback<GuiObject, T, U, V> handler)
    {
        if (!OnListenerRemoving(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T, U, V>)eventTable[eventId] - handler;
        OnListenerRemoved(eventId);
    }

    public static void RemoveListener<T, U, V, W>(Enum eventId, GuiCallback<GuiObject, T, U, V, W> handler)
    {
        if (!OnListenerRemoving(eventId, handler)) return;
        eventTable[eventId] = (GuiCallback<GuiObject, T, U, V, W>)eventTable[eventId] - handler;
        OnListenerRemoved(eventId);
    }
    #endregion

    #region Broadcast
    protected void Broadcast(Enum eventId)
    {
        Debug.Log(string.Format("Broadcast: {0}, EventId: {1}", typeof(Z).Name, eventId));

        Delegate d;
        if (eventTable.TryGetValue(eventId, out d))
        {
            GuiCallback<GuiObject> callback = d as GuiCallback<GuiObject>;
            if (callback != null)
            {
                callback.Invoke(this);
            }
        }
    }

    protected void Broadcast<T>(Enum eventId, T arg1)
    {
        Debug.Log(string.Format("Broadcast: {0}, EventId: {1}, arg1: ({2}){3}", typeof(Z).Name, eventId, typeof(T).Name, arg1.ToString()));

        Delegate d;
        if (eventTable.TryGetValue(eventId, out d))
        {
            GuiCallback<GuiObject, T> callback = d as GuiCallback<GuiObject, T>;
            if (callback != null)
            {
                callback.Invoke(this, arg1);
            }
        }
    }

    protected void Broadcast<T, U>(Enum eventId, T arg1, U arg2)
    {
        Debug.Log(string.Format("Broadcast: {0}, EventId: {1}, arg1: ({2}){3}, arg2: ({4}){5}", typeof(Z).Name, eventId, typeof(T).Name, arg1.ToString(), typeof(U).Name, arg2.ToString()));

        Delegate d;
        if (eventTable.TryGetValue(eventId, out d))
        {
            GuiCallback<GuiObject, T, U> callback = d as GuiCallback<GuiObject, T, U>;
            if (callback != null)
            {
                callback.Invoke(this, arg1, arg2);
            }
        }
    }

    protected void Broadcast<T, U, V>(Enum eventId, T arg1, U arg2, V arg3)
    {
        Debug.Log(string.Format("Broadcast: {0}, EventId: {1}, arg1: ({2}){3}, arg2: ({4}){5}, arg3: ({6}){7}", typeof(Z).Name, eventId, typeof(T).Name, arg1.ToString(), typeof(U).Name, arg2.ToString(), typeof(V).Name, arg3.ToString()));

        Delegate d;
        if (eventTable.TryGetValue(eventId, out d))
        {
            GuiCallback<GuiObject, T, U, V> callback = d as GuiCallback<GuiObject, T, U, V>;
            if (callback != null)
            {
                callback.Invoke(this, arg1, arg2, arg3);
            }
        }
    }

    protected void Broadcast<T, U, V, W>(Enum eventId, T arg1, U arg2, V arg3, W arg4)
    {
        Debug.Log(string.Format("Broadcast: {0}, EventId: {1}, arg1: ({2}){3}, arg2: ({4}){5}, arg3: ({6}){7}, arg4: ({8}){9}", typeof(Z).Name, eventId, typeof(T).Name, arg1.ToString(), typeof(U).Name, arg2.ToString(), typeof(V).Name, arg3.ToString(), typeof(W).Name, arg4.ToString()));

        Delegate d;
        if (eventTable.TryGetValue(eventId, out d))
        {
            GuiCallback<GuiObject, T, U, V, W> callback = d as GuiCallback<GuiObject, T, U, V, W>;
            if (callback != null)
            {
                callback.Invoke(this, arg1, arg2, arg3, arg4);
            }
        }
    }
    #endregion
}
