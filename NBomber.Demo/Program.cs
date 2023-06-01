using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace MyLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenarioCount = 6;
            var scenarioDuration = TimeSpan.FromSeconds(10);
            var parallelExpectedDuration = scenarioDuration;
            var sequentialExpectedDuration = scenarioCount * scenarioDuration;

            var scenarios = new List<ScenarioProps>();

            for (var scenarioNumber = 1; scenarioNumber <= scenarioCount; scenarioNumber++)
            {
                var scenario = Scenario.Create($"scenario-{scenarioNumber}", async context =>
                {
                    await Task.Delay(200);

                    return Response.Ok();
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 10,
                                      interval: TimeSpan.FromSeconds(1),
                                      during: scenarioDuration)
                );

                scenarios.Add(scenario);
            }

            Console.WriteLine($"Scenario count: {scenarioCount}");
            Console.WriteLine($"Scenario duration: {scenarioDuration}");
            Console.WriteLine($"Expected total duration if executed in parallel: {parallelExpectedDuration}");
            Console.WriteLine($"Expected total duration if executed sequentially: {sequentialExpectedDuration}");

            NBomberRunner
                .RegisterScenarios(scenarios.ToArray())
                .Run();
        }
    }
}