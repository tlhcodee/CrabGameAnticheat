using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks
{
    public class CheckBuffer
    {

        private double b;

        public double Buffer
        {
            get{  return b; }
            set{ b = value; }
        }

        public double increase()
        {
            return this.b++;
        }

        public double increaseByValue(double value)
        {
            return this.b = this.b + value;
        }

        public double decrease()
        {
            return this.b = Math.Max(0.0D, this.b - 1);
        }

        public double decreaseByValue(double value)
        {
            return this.b = Math.Max(0.0D, this.b - value);
        }

        public void setBuffer(double value) { this.b = value; }
    }
}
