using BenchmarkDotNet.Running;

namespace RepositoryTimings
{
    class Program
    {
        static void Main(string[] args)
        {

            //var ct = new ComparisonTests();
            //ct.Chain();
            //ct.ChainCompiled();
            //ct.Dapper ();
            //ct.EFNoviceRepo();
            //ct.EFIntermediateRepo();
            //ct.EFIntermediateNoTrackRepo();

            BenchmarkRunner.Run<ComparisonTests>();
        }
    }
}
