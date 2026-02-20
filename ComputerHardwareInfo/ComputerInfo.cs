using ComputerHardwareInfo.Collectors;

namespace ComputerHardwareInfo
{
    public class ComputerInfo
    {
        private readonly string targetComputerName;
        private readonly OutputType outputDestination;

        public ComputerInfo(string computerName, OutputType outputType)
        {
            targetComputerName = computerName;
            outputDestination = outputType;
        }

        public void Execute()
        {
            var hardwareCollector = new RemoteComputerHardwareCollector(targetComputerName, outputDestination);
            hardwareCollector.CollectAllHardwareInformation();
        }
    }
}
