using System;
using System.Collections.Generic;

namespace ColorSound.Application.Components
{
    public class Node
    {
        private List<Node> nodes;

        public Node(List<Node> nodes) 
        {
            nodes.Add(this);
            this.nodes = nodes;
        }

        public double TimeOn { get; set; }

        public double TimeOff { get; set; }

        public Func<double, double, double, float> Process { get; set; }

        public double GetValue(double time) 
        {
            if (time > TimeOff) 
            {
                nodes.Remove(this);
            }

            if (time - TimeOff > int.MaxValue / 2) 
            {
                nodes.Remove(this);
            }

            return Process(time, TimeOn, TimeOff);
        }
    }
}
