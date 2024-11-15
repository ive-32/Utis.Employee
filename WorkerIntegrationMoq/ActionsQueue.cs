using System.Collections.Concurrent;
using Utis.Minex.WrokerIntegration;

namespace WorkerIntegrationMoq;

public static class ActionsQueue
{
    public static ConcurrentQueue<WorkerAction> Actions = new ConcurrentQueue<WorkerAction>();
}