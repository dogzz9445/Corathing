using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;
using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Factories;

public static partial class WorkflowFactory
{
    public static WorkflowState CreateWorkflow(Guid id, string name)
    {
        return new WorkflowState
        {
            Id = id,
            Settings = new WorkflowSettings
            {
                Name = name
            }
        };
    }

    public static WorkflowState Create()
        => new WorkflowState
        {
            Id = Guid.NewGuid(),
            Settings = new WorkflowSettings
            {
                Name = "My Workflow"
            }
        };
}
