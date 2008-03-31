using System;

namespace Sneal.SqlMigration
{
    public class ProgressEventArgs : EventArgs
    {
        private readonly int numObjectsCompleted;
        private readonly int totalObject;

        public ProgressEventArgs(int numObjectsCompleted, int totalObject)
        {
            this.totalObject = totalObject;
            this.numObjectsCompleted = numObjectsCompleted;
        }

        public int TotalObject
        {
            get { return totalObject; }
        }

        public int NumObjectsCompleted
        {
            get { return numObjectsCompleted; }
        }
    }
}