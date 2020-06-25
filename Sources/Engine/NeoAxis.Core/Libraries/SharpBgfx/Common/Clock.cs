//using System.Diagnostics;

//namespace SharpBgfx.Common {
//    public class Clock {
//        long frequency;
//        long lastFrame;
//        long initialTick;

//        public Clock () {
//            frequency = Stopwatch.Frequency;
//        }

//        public void Start () {
//            initialTick = Stopwatch.GetTimestamp();
//        }

//        public float Frame () {
//            long tick = Stopwatch.GetTimestamp();

//            float elapsed = ((float)(tick - lastFrame)) / frequency;
//            lastFrame = tick;

//            return elapsed;
//        }

//        public float TotalTime () {
//            long tick = Stopwatch.GetTimestamp();
//            return ((float)(tick - initialTick)) / frequency;
//        }
//    }
//}
