using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Factories;

public static partial class WorkflowFactory
{
    public static WorkflowState CreateWorkflow(Guid id, string name)
    {
        return new WorkflowState
        {
            Id = id,
            CoreSettings = new WorkflowCoreState
            {
                Name = name
            }
        };
    }

    public static WorkflowState Create()
        => new WorkflowState
        {
            Id = Guid.NewGuid(),
            CoreSettings = new WorkflowCoreState
            {
                Name = "My Workflow"
            }
        };
}
