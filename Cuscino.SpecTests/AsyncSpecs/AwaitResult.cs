using System.Threading.Tasks;

namespace Cuscino.SpecTests.AsyncSpecs
{
    public class AwaitResult<T>
    {
        readonly Task<T> task;

        public AwaitResult(Task<T> task)
        {
            this.task = task;
        }

        public Task<T> AsTask
        {
            get { return task; }
        }

        public static implicit operator T(AwaitResult<T> m)
        {
            return m.task.Result;
        }
    }
}