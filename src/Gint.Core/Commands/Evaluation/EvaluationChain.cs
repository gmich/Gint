using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gint
{
    internal class EvaluationChain
    {
        public List<Func<Task>> Chain { get; } = new List<Func<Task>>();
        public List<TextSpan> Spans { get; } = new List<TextSpan>();

        public void Add(Func<Task> block, TextSpan span)
        {
            Chain.Add(block);
            Spans.Add(span);
        }

        public int Evaluated { get; set; }
        public bool Error { get; set; }
        public int LastInvokedPosition { get; set; }

        public Task EvaluateNext(int position)
        {
            if (Error) return Task.FromResult(0);
            if (Chain.Count == 0 || position >= Chain.Count) return Task.FromResult(0);

            LastInvokedPosition = position;

            return Chain[position].Invoke();
        }


    }

}
