using System.ComponentModel;

namespace WorkerIntegrationMoq.Configurations;

public class AddWorkerActionsJobConfiguration
{
    public int MinActionsPerJob { get; set; } = 10;

    public int MaxActionsPerJob { get; set; } = 100;

    public int AddWorkersInterval { get; set; } = 5;

    public int MaxQueueSize { get; set; } = 500;

}