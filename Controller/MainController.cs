using System;
using Model;
using View;

namespace Controller
{
    public class MainController
    {
        private readonly ConsoleInterface _view;

        public MainController(ConsoleInterface view)
        {
            _view = view;
        }

        public void Run()
        {
            string datasetFolder = _view.AskDatasetFolder();
            bool useThreads = _view.AskUseMultithreading();

            var data = new DatasetLoader();
            _view.ShowMessage("Loading dataset");
            data.LoadData(datasetFolder);

            string baseOutput = "output";
            string folder = useThreads ? "withMultithreading" : "withoutMultithreading";
            string outputPath = System.IO.Path.Combine(baseOutput, folder);

            var analytics = new AnalyticsGenerator(data, outputPath);

            _view.ShowMessage($"Generating reports {(useThreads ? "with" : "without")} threads");
            analytics.GenerateReports(useThreads);

            _view.ShowMessage($"Reports saved in: {outputPath}");
        }
    }
}
