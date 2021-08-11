
using System.Threading.Tasks;
using UnityEngine;

namespace AurecasLib.Utils {
    public class YieldableTask<T> : CustomYieldInstruction {
        public override bool keepWaiting => !task.IsCompleted;

        Task<T> task;

        public YieldableTask(Task<T> task) {
            this.task = task;
        }

        public T GetResult() {
            return task.Result;
        }
    }

    public class YieldableTask : CustomYieldInstruction {
        public override bool keepWaiting => !task.IsCompleted;

        Task task;

        public YieldableTask(Task task) {
            this.task = task;
        }
    }
}
